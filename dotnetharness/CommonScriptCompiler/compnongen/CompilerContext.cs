using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class CompilerContextUtil
    {

        public static int[] PUBLIC_CompleteCompilation(object compObj)
        {
            CompilerContext compiler = (CompilerContext) compObj;

            string[] moduleCompilationOrder = FunctionWrapper.CompilerContext_CalculateCompilationOrder(compiler);

            compiler.compiledModulesById = new Dictionary<string, CompiledModule>();

            for (int i = 0; i < moduleCompilationOrder.Length; i += 1) 
            {
                string moduleId = moduleCompilationOrder[i];
                CompiledModule module = CompilerContext_CompileModule(compiler, moduleId);
                compiler.compiledModulesById[moduleId] = module;
            }

            CompiledModule[] modules = compiler.compiledModulesById.Values.ToArray();
            CompilationBundle bundle = FunctionWrapper.bundleCompilation(compiler.staticCtx, compiler.rootId, modules);

            return FunctionWrapper.ExportUtil_exportBundle(compiler.flavorId, compiler.extensionVersionId, bundle);
        }

        public static CompiledModule CompilerContext_CompileModule(CompilerContext compiler, string moduleId)
        {
            List<FileContext> files = compiler.filesByModuleId[moduleId];

            Dictionary<string, AbstractEntity> rootEntities = new Dictionary<string, AbstractEntity>();
            Dictionary<string, string> sourceCode = new Dictionary<string, string>();
            for (int i = 0; i < files.Count; i++)
            {
                FileContext file = files[i];
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

            Resolver resolverCtx = FunctionWrapper.Resolver_new(compiler.staticCtx, rootEntities, compiler.extensionNames);

            ResolverUtil.Resolve(resolverCtx);

            CompiledModule m = FunctionWrapper.CompiledModule_new(moduleId);
            m.codeFiles = sourceCode;
            FunctionWrapper.CompiledModule_AddLambdas(m, resolverCtx.lambdas);
            FunctionWrapper.CompiledModule_InitializeLookups(m, resolverCtx.nestedEntities, resolverCtx.flattenedEntities);
            for (int i = 0; i < files.Count; i++)
            {
                FileContext file = files[i];
                file.compiledModule = m;
            }

            return m;
        }

        private static Dictionary<string, Token> CompilerContext_ParseOutAnnotations(CompilerContext compCtx, TokenStream tokens)
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
                if (!FunctionWrapper.StringSet_has(compCtx.staticCtx.validAnnotationNames, annotationName))
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

            // note that casting can fail as a namespace
            ClassEntity wrappingClass = nestParent == null ? null : (nestParent.specificData as ClassEntity);

            while (keepChecking)
            {
                Token firstToken = FunctionWrapper.Tokens_peek(tokens);
                Dictionary<string, Token> annotationTokens = CompilerContext_ParseOutAnnotations(compiler, tokens);

                string nextToken = FunctionWrapper.Tokens_peekValueNonNull(tokens);
                AbstractEntity entity = null;
                switch (nextToken)
                {
                    case "function":
                        entity = EntityParser.ParseFunctionDefinition(tokens, annotationTokens, wrappingClass);
                        break;

                    case "namespace":
                        entity = ParseNamespace(compiler, file, namespacePrefix);
                        break;

                    case "const":
                        entity = EntityParser.ParseConst(tokens);
                        break;

                    case "enum":
                        entity = EntityParser.ParseEnum(tokens);
                        break;

                    case "class":
                        entity = ParseClass(compiler, file, namespacePrefix).baseData;
                        break;

                    case "constructor":
                        entity = EntityParser.ParseConstructor(tokens, annotationTokens);
                        break;

                    case "field":
                        entity = EntityParser.ParseField(tokens, annotationTokens);
                        break;

                    case "property":
                        FunctionWrapper.fail("Not implemented");
                        break;

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
                FunctionWrapper.fail("Not implemented");
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
