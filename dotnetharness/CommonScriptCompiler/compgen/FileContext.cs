using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class FileContext
    {
        public StaticContext staticCtx;
        public string path;
        public string content;
        public TokenStream tokens;
        public ImportStatement[] imports;
        public System.Collections.Generic.Dictionary<string, ImportStatement> importsByVar;
        public bool isBuiltInLib;
        public bool isCoreBuiltin;
        public CompiledModule compiledModule;

        public FileContext(StaticContext staticCtx, string path, string content, TokenStream tokens, ImportStatement[] imports, System.Collections.Generic.Dictionary<string, ImportStatement> importsByVar, bool isBuiltInLib, bool isCoreBuiltin, CompiledModule compiledModule)
        {
            this.staticCtx = staticCtx;
            this.path = path;
            this.content = content;
            this.tokens = tokens;
            this.imports = imports;
            this.importsByVar = importsByVar;
            this.isBuiltInLib = isBuiltInLib;
            this.isCoreBuiltin = isCoreBuiltin;
            this.compiledModule = compiledModule;
        }
    }
}
