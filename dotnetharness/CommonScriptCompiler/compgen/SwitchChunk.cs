using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class SwitchChunk
    {
        public System.Collections.Generic.List<Token> CaseTokens;
        public System.Collections.Generic.List<Expression> Cases;
        public System.Collections.Generic.List<Statement> Code;

        public SwitchChunk(System.Collections.Generic.List<Token> CaseTokens, System.Collections.Generic.List<Expression> Cases, System.Collections.Generic.List<Statement> Code)
        {
            this.CaseTokens = CaseTokens;
            this.Cases = Cases;
            this.Code = Code;
        }
    }
}
