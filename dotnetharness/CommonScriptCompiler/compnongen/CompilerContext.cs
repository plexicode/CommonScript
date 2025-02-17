using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal enum ModuleType
    {
        CORE_BUILTIN,
        LIB_BUILTIN,
        USER,
    }

    internal class CompilerContext
    {
        public string rootId;
        public Dictionary<string, List<string>> depIdsByModuleId;
        public Dictionary<string, List<FileContext>> filesByModuleId;
        public List<string> finalCompilationOrder = null;
        public Dictionary<string, bool> unfulfilledDependencies;
        public Dictionary<string, CompiledModule> compiledModulesById = null;
        public string extensionVersionId;
        public string flavorId;
        public List<string> extensionNames;

        public CompilerContext(string rootId, string flavorId, string extensionVersionId, string[] extensionNames)
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

            SupplyFilesForModule(this, "{BUILTIN}", builtinFiles, true, true);
        }

        private static CompilerContext GetCompiler(object compObj)
        {
            return (CompilerContext)compObj;
        }

        public static void SupplyFilesForModule(
            object compObj,
            string moduleId,
            Dictionary<string, string> fileLookup,
            bool isCoreBuiltin,
            bool isBuiltInLib)
        {
            CompilerContext compiler = GetCompiler(compObj);

            compiler.depIdsByModuleId[moduleId] = new List<string>();

            List<FileContext> files = new List<FileContext>();
            Dictionary<string, ImportStatement> imports = new Dictionary<string, ImportStatement>();
            foreach (string path in fileLookup.Keys.OrderBy(n => n))
            {
                FileContext fileCtx = new FileContext(path, fileLookup[path]);
                fileCtx.isCoreBuiltin = isCoreBuiltin;
                fileCtx.isBuiltInLib = isBuiltInLib;
                files.Add(fileCtx);
                fileCtx.imports = ImportParser.AdvanceThroughImports(fileCtx.tokens, isCoreBuiltin);
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

        public static string? GetNextRequiredModuleId(object compObj)
        {
            CompilerContext compiler = GetCompiler(compObj);
            while (true)
            {
                string nextKey = compiler.unfulfilledDependencies.Keys.OrderBy(k => k).FirstOrDefault();
                if (nextKey == null) return null;

                if (!BuiltinUtil.IsBuiltInModule(nextKey))
                {
                    return nextKey;
                }

                SupplyFilesForModule(
                    compiler,
                    nextKey,
                    new Dictionary<string, string>() { { nextKey + ".script", BuiltinUtil.GetSourceFilesFor(nextKey) } },
                    false,
                    true);
            }
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
                Token danglingToken = file.tokens.peek();
                if (danglingToken != null)
                {
                    FunctionWrapper.Errors_Throw(danglingToken, "Unexpected token: '" + danglingToken.Value + "'. You might have too many close parentheses in this file.");
                }
            }

            Resolver resolverCtx = new Resolver(rootEntities, compiler.extensionNames);

            resolverCtx.Resolve();

            CompiledModule m = new CompiledModule(moduleId);
            m.codeFiles = sourceCode;
            m.AddLambdas(resolverCtx.lambdas);
            m.InitializeCompieldModuleLookups(resolverCtx.nestedEntities, resolverCtx.flattenedEntities);
            foreach (FileContext file in files)
            {
                file.compiledModule = m;
            }

            return m;
        }

        private static StringSet VALID_ANNOTATIONS = FunctionWrapper.StringSet_fromArray("public static".Split(' '));

        private static Dictionary<string, Token> ParseOutAnnotations(TokenStream tokens)
        {
            Dictionary<string, Token> output = new Dictionary<string, Token>();
            while (tokens.peekType() == (int) TokenType.ANNOTATION)
            {
                Token token = tokens.pop();
                string annotationName = token.Value.Substring(1);
                if (output.ContainsKey(annotationName))
                {
                    FunctionWrapper.Errors_Throw(token, "Multiplie redundant annotations.");
                }
                if (!FunctionWrapper.StringSet_has(VALID_ANNOTATIONS, annotationName))
                {
                    FunctionWrapper.Errors_Throw(token, "Unrecognized annotation: '@" + annotationName + "'");
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

            // note that casting can fail as a namespace
            ClassEntity wrappingClass = nestParent == null ? null : (nestParent.specificData as ClassEntity);

            while (keepChecking)
            {
                Token firstToken = tokens.peek();
                Dictionary<string, Token> annotationTokens = ParseOutAnnotations(tokens);

                string nextToken = file.tokens.peekValueNonNull();
                AbstractEntity entity = null;
                switch (nextToken)
                {
                    case "function":
                        entity = entityParser.ParseFunctionDefinition(annotationTokens, wrappingClass);
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
                        entity = ParseClass(compiler, file, namespacePrefix).baseData;
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
                        FunctionWrapper.Errors_Throw(tokens.peek(), "All imports must appear at the top of the file.");
                        break;

                    case "}":
                        keepChecking = false;
                        break;

                    default:
                        // Unexpected EOF or stray tokens.
                        tokens.ensureMore();
                        FunctionWrapper.Errors_Throw(tokens.peek(), "Unexpected token: '" + tokens.peekValueNonNull() + "'");
                        break;
                }

                if (entity != null)
                {
                    entity.isStatic = entity.annotations != null && entity.annotations.ContainsKey("static");
                    AttachEntityToParseTree(entity, nestParent, file, namespacePrefix, currentEntityBucket, annotationTokens);
                }

                if (entity == null && annotationTokens.Count > 0)
                {
                    FunctionWrapper.Errors_Throw(firstToken, "This annotation is not attached to any entity.");
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
            child.OBJ_TEMP_CAST_fileContext = file;
            child.annotations = annotationTokens;

            string fqName = child.simpleName;
            if (activeNsPrefix != "")
            {
                fqName = activeNsPrefix + "." + fqName;
            }
            child.fqName = fqName;
            child.nestParent = parent;

            bool isStatic = annotationTokens.ContainsKey("static");
            bool isAttachingToClass = parent != null && parent.type == (int)EntityType.CLASS;
            bool isClass = child.type == (int)EntityType.CLASS;
            bool isCtor = child.type == (int)EntityType.CONSTRUCTOR;
            if (isCtor && !isAttachingToClass)
            {
                FunctionWrapper.Errors_Throw(child.firstToken, "Cannot place a constructor here. Constructors can only be added to classes.");
            }

            if (isStatic && !isClass && !isAttachingToClass)
            {
                FunctionWrapper.Errors_Throw(child.firstToken, "@static is not applicable to this type of entity.");
            }

            if (activeEntityBucket.ContainsKey(child.simpleName))
            {
                FunctionWrapper.Errors_Throw(child.firstToken, "There are multiple entities named " + child.fqName + ".");
            }

            activeEntityBucket[child.simpleName] = child;
        }

        private static ClassEntity ParseClass(
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

            ClassEntity classDef = FunctionWrapper.ClassEntity_new(classToken, classNameToken, classFqName);
            classDef.baseClassTokens = baseClassTokens;
            tokens.popExpected("{");
            ParseOutEntities(ctx, file, classDef.classMembers, classDef.baseData, classFqName);
            tokens.popExpected("}");

            // inject a fake do-nothing constructor if one was not declared.
            if (!classDef.classMembers.ContainsKey("@ctor"))
            {
                Expression[] baseArgs = null;
                if (classDef.baseClassTokens != null)
                {
                    baseArgs = [];
                }
                AbstractEntity ctor = FunctionLikeEntity.BuildConstructor(classToken, [], [], baseArgs, [], false).baseData;
                AttachEntityToParseTree(ctor, classDef.baseData,
                    (FileContext)classDef.baseData.OBJ_TEMP_CAST_fileContext, classDef.baseData.fqName,
                    classDef.classMembers, new Dictionary<string, Token>());
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
            ParseOutEntities(ctx, file, nsEntity.nestedMembers, nsEntity.baseData, namespacePrefix);
            tokens.popExpected("}");
            return nsEntity.baseData;
        }
    }
}
