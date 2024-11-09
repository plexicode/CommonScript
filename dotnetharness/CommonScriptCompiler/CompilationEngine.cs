using System;
using System.Collections.Generic;

namespace CommonScript.Compiler
{
    public class CompilationEngine
    {
        private string languageId = null;
        private string version = null;
        private HashSet<string> extensions;

        public string LanguageID { get { return this.languageId; } }
        public string Version { get { return this.version; } }


#if DEBUG 
        private static readonly bool isDebug = true;
#else
        private static readonly bool isDebug = false;
#endif

        public CompilationEngine(string languageId, string version, ICollection<string> extensions)
        {
            this.languageId = languageId;
            this.version = version;
            this.extensions = new HashSet<string>(extensions);
        }

        public AdaptiveCompilation CreateAdaptiveCompilation(string rootModuleId)
        {
            return new AdaptiveCompilation(this.languageId, this.version, rootModuleId, this.extensions);
        }

        public CompilationResult DoStaticCompilation(
            string rootModuleId,
            Dictionary<string, Dictionary<string, string>> codeFilesByModuleId)
        {
            AdaptiveCompilation comp = this.CreateAdaptiveCompilation(rootModuleId);

            while (!comp.IsComplete)
            {
                string moduleId = comp.NextRequiredModule;
                if (!codeFilesByModuleId.ContainsKey(moduleId)) throw new InvalidOperationException();
                Dictionary<string, string> files = codeFilesByModuleId[moduleId];
                if (isDebug)
                {
                    comp.ProvideFilesForModuleCompilation(moduleId, files);
                }
                else
                {
                    try
                    {
                        comp.ProvideFilesForModuleCompilation(moduleId, files);
                    }
                    catch (Exception ex)
                    {
                        return new CompilationResult(null, ex.Message);
                    }
                }
            }

            return comp.GetCompilation();
        }
    }
}
