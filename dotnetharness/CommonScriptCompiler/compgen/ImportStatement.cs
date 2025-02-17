using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ImportStatement
    {
        public Token importToken;
        public Token[] nameChain;
        public string flatName;
        public Token importTargetVariableName;
        public bool isPollutionImport;
        public CompiledModule compiledModuleRef;

        public ImportStatement(Token importToken, Token[] nameChain, string flatName, Token importTargetVariableName, bool isPollutionImport, CompiledModule compiledModuleRef)
        {
            this.importToken = importToken;
            this.nameChain = nameChain;
            this.flatName = flatName;
            this.importTargetVariableName = importTargetVariableName;
            this.isPollutionImport = isPollutionImport;
            this.compiledModuleRef = compiledModuleRef;
        }
    }
}
