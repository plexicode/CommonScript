using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class CompilerContext
    {
        public StaticContext staticCtx;
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
            this.staticCtx = FunctionWrapper.StaticContext_new();
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
                FileContext fileCtx = FunctionWrapper.FileContext_new(compiler.staticCtx, path, fileLookup[path]);
                fileCtx.isCoreBuiltin = isCoreBuiltin;
                fileCtx.isBuiltInLib = isBuiltInLib;
                files.Add(fileCtx);
                fileCtx.imports = FunctionWrapper.ImportParser_AdvanceThroughImports(fileCtx.tokens, isCoreBuiltin);
                FunctionWrapper.FileContext_initializeImportLookup(fileCtx);
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
                Token danglingToken = FunctionWrapper.Tokens_peek(file.tokens);
                if (danglingToken != null)
                {
                    FunctionWrapper.Errors_Throw(danglingToken, "Unexpected token: '" + danglingToken.Value + "'. You might have too many close parentheses in this file.");
                }
            }

            Resolver resolverCtx = new Resolver(compiler.staticCtx, rootEntities, compiler.extensionNames);

            resolverCtx.Resolve();

            CompiledModule m = FunctionWrapper.CompiledModule_new(moduleId);
            m.codeFiles = sourceCode;
            FunctionWrapper.CompiledModule_AddLambdas(m, resolverCtx.lambdas);
            FunctionWrapper.CompiledModule_InitializeLookups(m, resolverCtx.nestedEntities, resolverCtx.flattenedEntities);
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
            while (FunctionWrapper.Tokens_peekType(tokens) == (int) TokenType.ANNOTATION)
            {
                Token token = FunctionWrapper.Tokens_pop(tokens);
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
            bool keepChecking = FunctionWrapper.Tokens_hasMore(tokens);

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
                Token firstToken = FunctionWrapper.Tokens_peek(tokens);
                Dictionary<string, Token> annotationTokens = ParseOutAnnotations(tokens);

                string nextToken = FunctionWrapper.Tokens_peekValueNonNull(tokens);
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
                        FunctionWrapper.Errors_Throw(FunctionWrapper.Tokens_peek(tokens), "All imports must appear at the top of the file.");
                        break;

                    case "}":
                        keepChecking = false;
                        break;

                    default:
                        // Unexpected EOF or stray tokens.
                        FunctionWrapper.Tokens_ensureMore(tokens);
                        FunctionWrapper.Errors_Throw(FunctionWrapper.Tokens_peek(tokens), "Unexpected token: '" + FunctionWrapper.Tokens_peekValueNonNull(tokens) + "'");
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

                if (!FunctionWrapper.Tokens_hasMore(tokens))
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
            Token classToken = FunctionWrapper.Tokens_popKeyword(tokens, "class");
            Token classNameToken = FunctionWrapper.Tokens_popName(tokens, "class name");
            Token[] baseClassTokens = null;
            if (FunctionWrapper.Tokens_popIfPresent(tokens, ":"))
            {
                string errMsg = "base class or interface name";
                List<Token> parent = new List<Token>() { FunctionWrapper.Tokens_popName(tokens, errMsg) };
                while (FunctionWrapper.Tokens_isNext(tokens, "."))
                {
                    parent.Add(FunctionWrapper.Tokens_pop(tokens));
                    parent.Add(FunctionWrapper.Tokens_popName(tokens, errMsg));
                }
                baseClassTokens = parent.ToArray();
            }

            string classFqName = namespacePrefix == ""
                ? classNameToken.Value
                : (namespacePrefix + "." + classNameToken.Value);

            ClassEntity classDef = FunctionWrapper.ClassEntity_new(classToken, classNameToken, classFqName);
            classDef.baseClassTokens = baseClassTokens;
            FunctionWrapper.Tokens_popExpected(tokens, "{");
            ParseOutEntities(ctx, file, classDef.classMembers, classDef.baseData, classFqName);
            FunctionWrapper.Tokens_popExpected(tokens, "}");

            // inject a fake do-nothing constructor if one was not declared.
            if (!classDef.classMembers.ContainsKey("@ctor"))
            {
                Expression[] baseArgs = null;
                if (classDef.baseClassTokens != null)
                {
                    baseArgs = [];
                }
                AbstractEntity ctor = FunctionWrapper.FunctionEntity_BuildConstructor(
                    classToken, 
                    [], 
                    [], 
                    baseArgs == null ? null : [..baseArgs], 
                    [], 
                    false).baseData;
                AttachEntityToParseTree(ctor, classDef.baseData,
                    classDef.baseData.fileContext, classDef.baseData.fqName,
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
            Token nsToken = FunctionWrapper.Tokens_popKeyword(tokens, "namespace");
            List<string> namespaceChain = new List<string>();
            if (namespacePrefix != "") namespaceChain.Add(namespacePrefix);
            Token nsFirst = FunctionWrapper.Tokens_popName(tokens, "namespace name");
            namespaceChain.Add(nsFirst.Value);
            Dictionary<string, AbstractEntity> entityBucket = new Dictionary<string, AbstractEntity>();
            while (FunctionWrapper.Tokens_popIfPresent(tokens, "."))
            {
                throw new NotImplementedException();
            }
            NamespaceEntity nsEntity = FunctionWrapper.NamespaceEntity_new(nsToken, nsFirst, namespacePrefix);
            FunctionWrapper.Tokens_popExpected(tokens, "{");
            namespacePrefix = string.Join(".", namespaceChain);
            ParseOutEntities(ctx, file, nsEntity.nestedMembers, nsEntity.baseData, namespacePrefix);
            FunctionWrapper.Tokens_popExpected(tokens, "}");
            return nsEntity.baseData;
        }
    }
}
