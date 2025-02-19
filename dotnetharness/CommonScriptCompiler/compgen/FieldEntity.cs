using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class FieldEntity
    {
        public Expression defaultValue;
        public Token opToken;
        public AbstractEntity baseData;

        public FieldEntity(Expression defaultValue, Token opToken, AbstractEntity baseData)
        {
            this.defaultValue = defaultValue;
            this.opToken = opToken;
            this.baseData = baseData;
        }
    }
}
