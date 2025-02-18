using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class EnumEntity
    {
        public Token[] memberNameTokens;
        public Expression[] memberValues;
        public AbstractEntity baseData;

        public EnumEntity(Token[] memberNameTokens, Expression[] memberValues, AbstractEntity baseData)
        {
            this.memberNameTokens = memberNameTokens;
            this.memberValues = memberValues;
            this.baseData = baseData;
        }
    }
}
