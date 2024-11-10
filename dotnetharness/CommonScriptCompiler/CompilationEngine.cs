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
            Dictionary<string, Dictionary<string, string>> userCodeFilesByModuleId,
            Dictionary<string, Dictionary<string, string>> builtinCodeFilesByModuleId)
        {
            AdaptiveCompilation comp = this.CreateAdaptiveCompilation(rootModuleId);
            userCodeFilesByModuleId = userCodeFilesByModuleId ?? new Dictionary<string, Dictionary<string, string>>();
            builtinCodeFilesByModuleId = builtinCodeFilesByModuleId ?? new Dictionary<string, Dictionary<string, string>>();

            while (!comp.IsComplete)
            {
                string moduleId = comp.NextRequiredModule;

                Dictionary<string, Dictionary<string, string>>[] sources = [
                    userCodeFilesByModuleId,
                    builtinCodeFilesByModuleId,
                ];

                bool found = false;
                for (int i = 0; i < 2; i++)
                {
                    bool isUserCode = i == 0;
                    if (sources[i].ContainsKey(moduleId))
                    {
                        found = true;
                        if (isDebug)
                        {
                            if (isUserCode) comp.ProvideFilesForUserModuleCompilation(moduleId, sources[i][moduleId]);
                            else comp.ProvideFilesForBuiltinLibraryModuleCompilation(moduleId, sources[i][moduleId]);
                        }
                        else
                        {
                            try
                            {
                                if (isUserCode) comp.ProvideFilesForUserModuleCompilation(moduleId, sources[i][moduleId]);
                                else comp.ProvideFilesForBuiltinLibraryModuleCompilation(moduleId, sources[i][moduleId]);
                            }
                            catch (Exception ex)
                            {
                                return new CompilationResult(null, ex.Message);
                            }
                        }
                    }
                }

                if (!found)
                {
                    return new CompilationResult(null, "The module '" + moduleId + "' could not be loaded.");
                }
            }

            return comp.GetCompilation();
        }
    }
}
