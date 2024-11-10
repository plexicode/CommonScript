using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    public class AdaptiveCompilation
    {
        private object genCompiler;
        private bool isDone = false;
        private string nextModuleIdCache = null;

        internal AdaptiveCompilation(string langId, string ver, string rootModuleId, ICollection<string> extensionNames)
        {
            this.genCompiler = new CompilerContext(rootModuleId, langId, ver, extensionNames.ToArray());
            this.nextModuleIdCache = CompilerContext.GetNextRequiredModuleId(this.genCompiler);
            this.isDone = this.nextModuleIdCache == null;
        }

        public bool IsComplete { get { return this.isDone; } }
        public string NextRequiredModule { get { return this.nextModuleIdCache; } }

        public void ProvideFilesForUserModuleCompilation(string moduleId, Dictionary<string, string> codeFiles)
        {
            ProvideFilesForModuleCompilationImpl(moduleId, codeFiles, false);
        }

        public void ProvideFilesForBuiltinLibraryModuleCompilation(string moduleId, Dictionary<string, string> codeFiles)
        {
            ProvideFilesForModuleCompilationImpl(moduleId, codeFiles, true);
        }

        private void ProvideFilesForModuleCompilationImpl(string moduleId, Dictionary<string, string> codeFiles, bool isBuiltin)
        {
            if (moduleId != this.NextRequiredModule) throw new InvalidOperationException();
            CompilerContext.SupplyFilesForModule(this.genCompiler, moduleId, codeFiles, false, false);
            this.nextModuleIdCache = CompilerContext.GetNextRequiredModuleId(this.genCompiler);
            this.isDone = this.nextModuleIdCache == null;
        }

        public CompilationResult GetCompilation()
        {
            if (!this.isDone) throw new InvalidOperationException();

            CompilerContext.EnsureDependenciesFulfilled(this.genCompiler);
            byte[] output = CompilerContext.CompleteCompilation(this.genCompiler);
            return new CompilationResult(output, null);
        }
    }
}
