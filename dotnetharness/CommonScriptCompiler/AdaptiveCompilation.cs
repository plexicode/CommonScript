using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    public class AdaptiveCompilation
    {
        private object genCompiler;
        private bool isDone = false;
        private string nextModuleIdCache = null;

        static AdaptiveCompilation()
        {
            FunctionWrapper.PST_RegisterExtensibleCallback("throwParserException", (args) =>
            {
                int type = (int) args[0];
                if (type == 1) throw new ParserException((Token) args[1], (string) args[2]);
                if (type == 2) throw new ParserException((string)args[1], (string)args[2]);
                if (type == 3) throw new ParserException((string)args[1]);
                throw new InvalidOperationException();
            });
        }
        
        internal AdaptiveCompilation(string langId, string ver, string rootModuleId, ICollection<string> extensionNames)
        {
            this.genCompiler = FunctionWrapper.CompilerContext_new(rootModuleId, langId, ver, extensionNames.ToArray());
            this.nextModuleIdCache = FunctionWrapper.PUBLIC_GetNextRequiredModuleId(this.genCompiler);
            this.isDone = this.nextModuleIdCache == null;
        }

        public bool IsComplete { get { return this.isDone; } }
        public string NextRequiredModule { get { return this.nextModuleIdCache; } }

        public void ProvideFilesForUserModuleCompilation(
            string moduleId,
            Dictionary<string, string> codeFiles,
            Dictionary<string, string> textResources,
            Dictionary<string, byte[]> binaryResources,
            Dictionary<string, object> imageResources)
        {
            ProvideFilesForModuleCompilationImpl(
                moduleId, codeFiles, false, textResources, binaryResources, imageResources);
        }

        public void ProvideFilesForBuiltinLibraryModuleCompilation(
            string moduleId, 
            Dictionary<string, string> codeFiles,
            Dictionary<string, string> textResources, 
            Dictionary<string, byte[]> binaryResources, 
            Dictionary<string, object> imageResources)
        {
            ProvideFilesForModuleCompilationImpl(
                moduleId, codeFiles, true, textResources, binaryResources, imageResources);
        }

        private void ProvideFilesForModuleCompilationImpl(
            string moduleId, 
            Dictionary<string, string> codeFiles, 
            bool isBuiltin, 
            Dictionary<string, string> textResources, 
            Dictionary<string, byte[]> binaryResources, 
            Dictionary<string, object> imageResources)
        {
            if (moduleId != this.NextRequiredModule) throw new InvalidOperationException();
            FunctionWrapper.PUBLIC_SupplyFilesForModule(this.genCompiler, moduleId, codeFiles, false, isBuiltin, textResources, ConvertByteArrayDictToIntArrayDict(binaryResources), imageResources);
            this.nextModuleIdCache = FunctionWrapper.PUBLIC_GetNextRequiredModuleId(this.genCompiler);
            this.isDone = this.nextModuleIdCache == null;
        }

        private static Dictionary<string, int[]> ConvertByteArrayDictToIntArrayDict(Dictionary<string, byte[]> original)
        {
            Dictionary<string, int[]> output = [];
            foreach (string key in original.Keys)
            {
                byte[] arr = original[key];
                int len = arr.Length;
                int[] arrOut = new int[len];
                for (int i = 0; i < len; i++) arrOut[i] = arr[i];
                output[key] = arrOut;
            }

            return output;
        }

        public CompilationResult GetCompilation()
        {
            if (CompilationEngine.IS_DEBUG)
            {
                return this.GetCompilationImpl();
            }

            try
            {
                return this.GetCompilationImpl();
            }
            catch (ParserException ex)
            {
                return new CompilationResult(null, ex.Message, null);
            }
        }

        private CompilationResult GetCompilationImpl()
        {
            if (!this.isDone) throw new InvalidOperationException();
            FunctionWrapper.PUBLIC_EnsureDependenciesFulfilled(this.genCompiler);
            int[] output = FunctionWrapper.PUBLIC_CompleteCompilation(this.genCompiler);
            byte[] outputBytes = output.Select(i => (byte)i).ToArray();
            string modDeps = FunctionWrapper.PUBLIC_getModuleDependencyInfo(this.genCompiler);
            return new CompilationResult(outputBytes, null, modDeps);
        }
    }
}
