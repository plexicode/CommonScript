using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class ImportStatement
    {
        public Token importToken;
        public Token[] nameChain;
        public string flatName;
        public Token importTargetVariableName;
        public bool isPollutionImport;
        public CompiledModule compiledModuleRef = null;

        public ImportStatement(Token importToken, List<Token> tokenChain, Token targetVarName)
        {
            List<string> flatName = new List<string>();
            for (int i = 0; i < tokenChain.Count; i++)
            {
                flatName.Add(tokenChain[i].Value);
            }

            this.importToken = importToken;
            this.nameChain = tokenChain.ToArray();
            this.flatName = string.Join(".", flatName);
            this.importTargetVariableName = targetVarName;
            this.isPollutionImport = targetVarName != null && targetVarName.Value == "*";
        }
    }
}
