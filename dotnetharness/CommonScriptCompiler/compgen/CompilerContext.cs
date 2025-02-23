using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class CompilerContext
    {
        public StaticContext staticCtx;
        public string rootId;
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> depIdsByModuleId;
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<FileContext>> filesByModuleId;
        public System.Collections.Generic.List<string> finalCompilationOrder;
        public System.Collections.Generic.Dictionary<string, bool> unfulfilledDependencies;
        public System.Collections.Generic.Dictionary<string, CompiledModule> compiledModulesById;
        public string extensionVersionId;
        public string flavorId;
        public System.Collections.Generic.List<string> extensionNames;

        public CompilerContext(StaticContext staticCtx, string rootId, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> depIdsByModuleId, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<FileContext>> filesByModuleId, System.Collections.Generic.List<string> finalCompilationOrder, System.Collections.Generic.Dictionary<string, bool> unfulfilledDependencies, System.Collections.Generic.Dictionary<string, CompiledModule> compiledModulesById, string extensionVersionId, string flavorId, System.Collections.Generic.List<string> extensionNames)
        {
            this.staticCtx = staticCtx;
            this.rootId = rootId;
            this.depIdsByModuleId = depIdsByModuleId;
            this.filesByModuleId = filesByModuleId;
            this.finalCompilationOrder = finalCompilationOrder;
            this.unfulfilledDependencies = unfulfilledDependencies;
            this.compiledModulesById = compiledModulesById;
            this.extensionVersionId = extensionVersionId;
            this.flavorId = flavorId;
            this.extensionNames = extensionNames;
        }
    }
}
