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
                for (int j = 0; j < file.imports.Length; j++)
                {
                    ImportStatement importStatement = file.imports[j];
                    importStatement.compiledModuleRef = compiler.compiledModulesById[importStatement.flatName];
                }
                FunctionWrapper.ParseOutEntities(compiler, file, rootEntities, null, "");
                Token danglingToken = FunctionWrapper.Tokens_peek(file.tokens);
                if (danglingToken != null)
                {
                    FunctionWrapper.Errors_Throw(danglingToken, "Unexpected token: '" + danglingToken.Value + "'. You might have too many close parentheses in this file.");
                }
            }

            Resolver resolverCtx = FunctionWrapper.Resolver_new(compiler.staticCtx, rootEntities, compiler.extensionNames);

            resolverCtx.ResolveExpressionFirstPass = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass;
            resolverCtx.ResolveExpressionSecondPass = FunctionWrapper.ExpressionResolver_ResolveExpressionSecondPass;
            resolverCtx.ResolveStatementFirstPass = StatementResolverUtil.StatementResolver_ResolveStatementFirstPass;
            resolverCtx.ResolveStatementSecondPass = StatementResolverUtil.StatementResolver_ResolveStatementSecondPass;
            
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
    }
}
