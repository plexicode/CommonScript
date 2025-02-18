using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ConstEntity
    {
        public Expression constValue;
        public AbstractEntity baseData;

        public ConstEntity(Expression constValue, AbstractEntity baseData)
        {
            this.constValue = constValue;
            this.baseData = baseData;
        }
    }
}
