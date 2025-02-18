using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class FileContext
    {
        public StaticContext staticCtx;
        public string path;
        public string content;
        public TokenStream tokens;
        public ImportStatement[] imports = null;
        public Dictionary<string, ImportStatement> importsByVar;
        public bool isBuiltInLib = false;
        public bool isCoreBuiltin = false;
        public CompiledModule compiledModule = null;

        public FileContext(StaticContext staticCtx, string path, string content)
        {
            this.staticCtx = staticCtx;
            this.content = content.Replace("\r\n", "\n").TrimEnd();
            this.path = path;
            this.tokens = FunctionWrapper.TokenStream_new(path, FunctionWrapper.Tokenize(this.path, this.content, staticCtx));
        }

        public void InitializeImportLookup()
        {
            this.importsByVar = new Dictionary<string, ImportStatement>();
            for (int i = 0; i < this.imports.Length; i++)
            {
                ImportStatement imp = this.imports[i];
                string varName = null;
                if (imp.importTargetVariableName != null)
                {
                    varName = imp.importTargetVariableName.Value;
                }
                else
                {
                    if (imp.flatName.Contains('.')) FunctionWrapper.Errors_Throw(imp.importToken, "Dot-delimited import paths must use an alias.");
                    varName = imp.flatName;
                }
                
                if (varName != "*" && this.importsByVar.ContainsKey(varName))
                {
                    FunctionWrapper.Errors_Throw(
                        imp.importTargetVariableName,
                        "There are multiple imports loaded as the variable '" + varName + "'");
                }
                this.importsByVar[varName] = imp;
            }
        }
    }
}
