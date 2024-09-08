using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonScript.Compiler
{
    internal class CompilerContext
    {
        public string rootId;
        public Dictionary<string, List<string>> depIdsByModuleId;
        public Dictionary<string, List<FileContext>> filesByModuleId;
        public List<string> finalCompilationOrder = null;
        public List<string> requests;
        public Dictionary<string, bool> unfulfilledDependencies;
        public Dictionary<string, CompiledModule> compiledModulesById = null;
        public string extensionVersionId;
        public string flavorId;
        public List<string> extensionNames;

        public CompilerContext(string rootId, string flavorId, string extensionVersionId, IList<string> extensionNames)
        {
            this.rootId = rootId;
            this.flavorId = flavorId;
            this.extensionVersionId = extensionVersionId;
            this.extensionNames = new List<string>(extensionNames);
            this.depIdsByModuleId = new Dictionary<string, List<string>>();
            this.filesByModuleId = new Dictionary<string, List<FileContext>>();
            this.unfulfilledDependencies = new Dictionary<string, bool>();
            this.unfulfilledDependencies[rootId] = true;

            Dictionary<string, string> builtinFiles = new Dictionary<string, string>()
            {
                { "builtins.script", BuiltinUtil.GetBuiltinSource() },
            };

            SupplyFilesForModule(this, "{BUILTIN}", builtinFiles, true);
        }

        private static CompilerContext GetCompiler(object compObj)
        {
            return (CompilerContext)compObj;
        }

        public static void SupplyFilesForModule(
            object compObj,
            string moduleId,
            Dictionary<string, string> fileLookup,
            bool isBuiltIn)
        {
            CompilerContext compiler = GetCompiler(compObj);
            bool moduleIsBuiltin = moduleId == "{BUILTIN}";

            compiler.depIdsByModuleId[moduleId] = new List<string>();

            List<FileContext> files = new List<FileContext>();
            Dictionary<string, ImportStatement> imports = new Dictionary<string, ImportStatement>();
            foreach (string path in fileLookup.Keys.OrderBy(n => n))
            {
                FileContext fileCtx = new FileContext(path, fileLookup[path]);
                fileCtx.isBuiltIn = isBuiltIn;
                files.Add(fileCtx);
                fileCtx.imports = ImportParser.AdvanceThroughImports(fileCtx.tokens, moduleIsBuiltin);
                fileCtx.InitializeImportLookup();
                foreach (ImportStatement impStmnt in fileCtx.imports)
                {
                    imports[impStmnt.flatName] = impStmnt;
                }
            }
            compiler.filesByModuleId[moduleId] = files;

            if (compiler.unfulfilledDependencies.ContainsKey(moduleId))
            {
                compiler.unfulfilledDependencies.Remove(moduleId);
            }

            List<string> allDeps = new List<string>(imports.Keys);
            compiler.depIdsByModuleId[moduleId] = allDeps;
            foreach (string depId in allDeps)
            {
                if (!compiler.filesByModuleId.ContainsKey(depId))
                {
                    compiler.unfulfilledDependencies[depId] = true;
                }
            }
        }

        public static string GetNextRequiredModuleId(object compObj)
        {
            CompilerContext compiler = GetCompiler(compObj);
            string[] keys = compiler.unfulfilledDependencies.Keys.ToArray();
            if (keys.Length == 0) return null;
            return keys[0];
        }

        public static void EnsureDependenciesFulfilled(object compObj)
        {
            CompilerContext compiler = GetCompiler(compObj);
            if (compiler.unfulfilledDependencies.Count > 0)
            {
                throw new Exception("Not all dependencies were fulfilled");
            }
        }

        private static string[] CalculateCompilationOrder(CompilerContext compiler)
        {
            Dictionary<string, int> recurseState = new Dictionary<string, int>();
            List<string> order = new List<string>();
            List<string> queue = new List<string>() { compiler.rootId };

            while (queue.Count > 0)
            {
                int last = queue.Count - 1;
                string currentId = queue[last];
                queue.RemoveAt(last);
                int currentRecurseState = 0;
                if (recurseState.ContainsKey(currentId)) currentRecurseState = recurseState[currentId];

                if (currentRecurseState == 2)
                {
                    // It's already handled. Move on 
                }
                else
                {
                    List<string> deps = compiler.depIdsByModuleId[currentId];
                    List<string> newDeps = new List<string>();

                    foreach (string depId in deps)
                    {
                        if (recurseState.ContainsKey(depId))
                        {
                            if (recurseState[depId] == 2)
                            {
                                // already added, just ignore it.
                            }
                            else if (recurseState[depId] == 1)
                            {
                                // in the process of adding. We have a dependency cycle!
                                throw new Exception("There is a cyclical dependency involving " + depId + " and " + currentId);
                            }
                        }
                        else
                        {
                            newDeps.Add(depId);
                        }
                    }

                    if (newDeps.Count == 0)
                    {
                        recurseState[currentId] = 2;
                        order.Add(currentId);
                    }
                    else
                    {
                        recurseState[currentId] = 1;
                        queue.Add(currentId);
                        queue.AddRange(newDeps);
                    }
                }
            }

            return order.ToArray();
        }

        public static byte[] CompleteCompilation(object compObj)
        {
            CompilerContext compiler = GetCompiler(compObj);

            string[] moduleCompilationOrder = CalculateCompilationOrder(compiler);

            compiler.compiledModulesById = new Dictionary<string, CompiledModule>();

            foreach (string moduleId in moduleCompilationOrder)
            {
                CompiledModule module = CompileModule(compiler, moduleId);
                compiler.compiledModulesById[moduleId] = module;
            }

            CompiledModule[] modules = compiler.compiledModulesById.Values.ToArray();
            CompilationBundle bundle = Bundler.bundleCompilation(compiler.rootId, modules);

            return Exporter.exportBundle(compiler.flavorId, compiler.extensionVersionId, bundle);
        }

        public static CompiledModule CompileModule(CompilerContext compiler, string moduleId)
        {
            List<FileContext> files = compiler.filesByModuleId[moduleId];

            Dictionary<string, AbstractEntity> rootEntities = new Dictionary<string, AbstractEntity>();
            Dictionary<string, string> sourceCode = new Dictionary<string, string>();
            foreach (FileContext file in files)
            {
                sourceCode[file.path] = file.content;
                foreach (ImportStatement importStatement in file.imports)
                {
                    importStatement.compiledModuleRef = compiler.compiledModulesById[importStatement.flatName];
                }
                ParseOutEntities(compiler, file, rootEntities, null, "");
            }

            Resolver resolverCtx = new Resolver(rootEntities, compiler.extensionNames);

            resolverCtx.Resolve();

            CompiledModule m = new CompiledModule(moduleId);
            m.codeFiles = sourceCode;
            m.nestedEntities = resolverCtx.nestedEntities;
            m.flattenedEntities = resolverCtx.flattenedEntities;

            return m;
        }

        private static HashSet<string> VALID_ANNOTATIONS = new HashSet<string>("public static".Split(' '));

        private static Dictionary<string, Token> ParseOutAnnotations(TokenStream tokens)
        {
            Dictionary<string, Token> output = new Dictionary<string, Token>();
            while (tokens.peekType() == TokenType.ANNOTATION)
            {
                Token token = tokens.pop();
                string annotationName = token.Value.Substring(1);
                if (output.ContainsKey(annotationName))
                {
                    Errors.ThrowError(token, "Multiplie redundant annotations.");
                }
                if (!VALID_ANNOTATIONS.Contains(annotationName))
                {
                    Errors.ThrowError(token, "Unrecognized annotation: '@" + annotationName + "'");
                }
                output[annotationName] = token;
            }
            return output;
        }

        private static void ParseOutEntities(
            CompilerContext compiler,
            FileContext file,
            Dictionary<string, AbstractEntity> currentEntityBucket,
            AbstractEntity nestParent,
            string namespacePrefix)
        {
            TokenStream tokens = file.tokens;
            bool keepChecking = tokens.hasMore();

            EntityParser entityParser = new EntityParser(tokens);
            StatementParser statementParser = new StatementParser(tokens);
            ExpressionParser expressionParser = new ExpressionParser(tokens);
            entityParser.expressionParser = expressionParser;
            entityParser.statementParser = statementParser;
            statementParser.expressionParser = expressionParser;
            statementParser.entityParser = entityParser;
            expressionParser.statementParser = statementParser;
            expressionParser.entityParser = entityParser;

            while (keepChecking)
            {
                Dictionary<string, Token> annotationTokens = ParseOutAnnotations(tokens);

                string nextToken = file.tokens.peekValueNonNull();
                AbstractEntity entity = null;
                switch (nextToken)
                {
                    case "function":
                        entity = entityParser.ParseFunctionDefinition(annotationTokens);
                        break;

                    case "namespace":
                        entity = ParseNamespace(compiler, file, namespacePrefix);
                        break;

                    case "const":
                        entity = entityParser.ParseConst();
                        break;

                    case "enum":
                        entity = entityParser.ParseEnum();
                        break;

                    case "class":
                        entity = ParseClass(compiler, file, namespacePrefix);
                        break;

                    case "constructor":
                        entity = entityParser.ParseConstructor(annotationTokens);
                        break;

                    case "field":
                        entity = entityParser.ParseField(annotationTokens);
                        break;

                    case "property":
                        throw new NotImplementedException(nextToken);

                    case "import":
                        Errors.ThrowError(tokens.peek(), "All imports must appear at the top of the file.");
                        break;

                    default:
                        keepChecking = false;
                        break;
                }

                if (entity != null)
                {
                    entity.isStatic = entity.annotations != null && entity.annotations.ContainsKey("static");
                    AttachEntityToParseTree(entity, nestParent, file, namespacePrefix, currentEntityBucket, annotationTokens);
                }

                if (!tokens.hasMore())
                {
                    keepChecking = false;
                }
            }
        }

        internal static void AttachEntityToParseTree(
            AbstractEntity child,
            AbstractEntity parent,
            FileContext file,
            string activeNsPrefix,
            Dictionary<string, AbstractEntity> activeEntityBucket,
            Dictionary<string, Token> annotationTokens)
        {
            child.fileContext = file;
            child.annotations = annotationTokens;

            string fqName = child.simpleName;
            if (activeNsPrefix != "")
            {
                fqName = activeNsPrefix + "." + fqName;
            }
            child.fqName = fqName;
            child.nestParent = parent;

            bool isStatic = annotationTokens.ContainsKey("static");
            bool isAttachingToClass = parent != null && parent.type == EntityType.CLASS;
            bool isClass = child.type == EntityType.CLASS;
            bool isCtor = child.type == EntityType.CONSTRUCTOR;
            if (isCtor && !isAttachingToClass)
            {
                Errors.ThrowError(child.firstToken, "Cannot place a constructor here. Constructors can only be added to classes.");
            }

            if (isStatic && !isClass && !isAttachingToClass)
            {
                Errors.ThrowError(child.firstToken, "@static is not applicable to this type of entity.");
            }

            if (activeEntityBucket.ContainsKey(child.simpleName))
            {
                Errors.ThrowError(child.firstToken, "There are multiple entities named " + child.fqName + ".");
            }

            activeEntityBucket[child.simpleName] = child;
        }

        private static AbstractEntity ParseClass(
            CompilerContext ctx,
            FileContext file,
            string namespacePrefix)
        {
            TokenStream tokens = file.tokens;
            Token classToken = tokens.popKeyword("class");
            Token classNameToken = tokens.popName("class name");
            Token[] baseClassTokens = null;
            if (tokens.popIfPresent(":"))
            {
                string errMsg = "base class or interface name";
                List<Token> parent = new List<Token>() { tokens.popName(errMsg) };
                while (tokens.isNext("."))
                {
                    parent.Add(tokens.pop());
                    parent.Add(tokens.popName(errMsg));
                }
                baseClassTokens = parent.ToArray();
            }

            string classFqName = namespacePrefix == ""
                ? classNameToken.Value
                : (namespacePrefix + "." + classNameToken.Value);

            ClassEntity classDef = new ClassEntity(classToken, classNameToken, classFqName);
            classDef.baseClassTokens = baseClassTokens;
            tokens.popExpected("{");
            ParseOutEntities(ctx, file, classDef.classMembers, classDef, classFqName);
            tokens.popExpected("}");

            // inject a fake do-nothing constructor if one was not declared.
            if (!classDef.classMembers.ContainsKey("@ctor"))
            {
                Expression[] baseArgs = null;
                if (classDef.baseClassTokens != null)
                {
                    baseArgs = new Expression[0];
                }
                AbstractEntity ctor = new ConstructorEntity(classToken, new Token[0], new Expression[0], baseArgs, new Statement[0], false);
                AttachEntityToParseTree(ctor, classDef, classDef.fileContext, classDef.fqName, classDef.classMembers, new Dictionary<string, Token>());
            }

            return classDef;
        }

        private static AbstractEntity ParseNamespace(
            CompilerContext ctx,
            FileContext file,
            string namespacePrefix)
        {
            TokenStream tokens = file.tokens;
            Token nsToken = tokens.popKeyword("namespace");
            List<string> namespaceChain = new List<string>();
            if (namespacePrefix != "") namespaceChain.Add(namespacePrefix);
            Token nsFirst = tokens.popName("namespace name");
            namespaceChain.Add(nsFirst.Value);
            Dictionary<string, AbstractEntity> entityBucket = new Dictionary<string, AbstractEntity>();
            while (tokens.popIfPresent("."))
            {
                throw new NotImplementedException();
            }
            NamespaceEntity nsEntity = new NamespaceEntity(nsToken, nsFirst, namespacePrefix);
            tokens.popExpected("{");
            namespacePrefix = string.Join(".", namespaceChain);
            ParseOutEntities(ctx, file, nsEntity.nestedMembers, nsEntity, namespacePrefix);
            tokens.popExpected("}");
            return nsEntity;
        }
    }
}
