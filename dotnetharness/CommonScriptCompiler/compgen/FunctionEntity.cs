using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class FunctionEntity
    {
        public Token[] argTokens;
        public Expression[] argDefaultValues;
        public Expression[] baseCtorArgValues;
        public AbstractEntity baseData;
        public Statement[] code;
        public System.Collections.Generic.Dictionary<string, bool> variableScope;
        public int FunctionSubtype;

        public FunctionEntity(Token[] argTokens, Expression[] argDefaultValues, Expression[] baseCtorArgValues, AbstractEntity baseData, Statement[] code, System.Collections.Generic.Dictionary<string, bool> variableScope, int FunctionSubtype)
        {
            this.argTokens = argTokens;
            this.argDefaultValues = argDefaultValues;
            this.baseCtorArgValues = baseCtorArgValues;
            this.baseData = baseData;
            this.code = code;
            this.variableScope = variableScope;
            this.FunctionSubtype = FunctionSubtype;
        }
    }
}
