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
        internal static readonly bool IS_DEBUG = true;
#else
        internal static readonly bool IS_DEBUG = false;
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
            Dictionary<string, Dictionary<string, string>> builtinCodeFilesByModuleId, 
            Dictionary<string, Dictionary<string, string>> textResourcesByModuleId, 
            Dictionary<string, Dictionary<string, byte[]>> binaryResourcesByModuleId, 
            Dictionary<string, Dictionary<string, ImageResource>> imageResourcesByModuleId)
        {
            AdaptiveCompilation comp = this.CreateAdaptiveCompilation(rootModuleId);
            userCodeFilesByModuleId = userCodeFilesByModuleId ?? new Dictionary<string, Dictionary<string, string>>();
            builtinCodeFilesByModuleId = builtinCodeFilesByModuleId ?? new Dictionary<string, Dictionary<string, string>>();
            textResourcesByModuleId = textResourcesByModuleId ?? new Dictionary<string, Dictionary<string, string>>();
            binaryResourcesByModuleId = binaryResourcesByModuleId ?? new Dictionary<string, Dictionary<string, byte[]>>();
            imageResourcesByModuleId = imageResourcesByModuleId ?? new Dictionary<string, Dictionary<string, ImageResource>>();

            string[] allModuleIds = [..userCodeFilesByModuleId.Keys, ..builtinCodeFilesByModuleId.Keys];
            if (allModuleIds.Length != new HashSet<string>(allModuleIds).Count)
            {
                return new CompilationResult(null, "Builtin and user modules have a name conflict.", null);
            }
            
            foreach (string moduleId in allModuleIds)
            {
                if (!textResourcesByModuleId.ContainsKey(moduleId)) textResourcesByModuleId[moduleId] = null;
                if (!binaryResourcesByModuleId.ContainsKey(moduleId)) binaryResourcesByModuleId[moduleId] = null;
                if (!imageResourcesByModuleId.ContainsKey(moduleId)) imageResourcesByModuleId[moduleId] = null;
            }

            while (!comp.IsComplete)
            {
                string moduleId = comp.NextRequiredModule;

                Dictionary<string, Dictionary<string, string>>[] sources = [
                    userCodeFilesByModuleId,
                    builtinCodeFilesByModuleId,
                ];
                Dictionary<string, string> textResources = textResourcesByModuleId[moduleId];
                Dictionary<string, byte[]> binaryResources = binaryResourcesByModuleId[moduleId];
                Dictionary<string, ImageResource> imageResources = imageResourcesByModuleId[moduleId];

                bool found = false;
                for (int i = 0; i < 2; i++)
                {
                    bool isUserCode = i == 0;
                    if (sources[i].ContainsKey(moduleId))
                    {
                        found = true;
                        if (IS_DEBUG)
                        {
                            if (isUserCode) comp.ProvideFilesForUserModuleCompilation(moduleId, sources[i][moduleId], textResources, binaryResources, imageResources);
                            else comp.ProvideFilesForBuiltinLibraryModuleCompilation(moduleId, sources[i][moduleId], textResources, binaryResources, imageResources);
                        }
                        else
                        {
                            try
                            {
                                if (isUserCode) comp.ProvideFilesForUserModuleCompilation(moduleId, sources[i][moduleId], textResources, binaryResources, imageResources);
                                else comp.ProvideFilesForBuiltinLibraryModuleCompilation(moduleId, sources[i][moduleId], textResources, binaryResources, imageResources);
                            }
                            catch (ParserException ex)
                            {
                                return new CompilationResult(null, ex.Message, null);
                            }
                        }
                    }
                }

                if (!found)
                {
                    return new CompilationResult(null, "The module '" + moduleId + "' could not be loaded.", null);
                }
            }

            return comp.GetCompilation();
        }
    }
}
