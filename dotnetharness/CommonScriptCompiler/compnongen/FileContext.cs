using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class FileContext
    {
        public string path;
        public string content;
        public TokenStream tokens;
        public ImportStatement[] imports = null;
        public Dictionary<string, ImportStatement> importsByVar;
        public bool isBuiltIn =false;

        public FileContext(string path, string content)
        {
            this.content = content.Replace("\r\n", "\n").TrimEnd();
            this.path = path;
            this.tokens = Tokenizer.Tokenize(path, this.content);
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
                    if (imp.flatName.Contains('.')) Errors.ThrowError(imp.importToken, "Dot-delimited import paths must use an alias.");
                    varName = imp.flatName;
                }
                
                if (varName != "*" && this.importsByVar.ContainsKey(varName))
                {
                    Errors.ThrowError(
                        imp.importTargetVariableName,
                        "There are multiple imports loaded as the variable '" + varName + "'");
                }
                this.importsByVar[varName] = imp;
            }
        }
    }
}
