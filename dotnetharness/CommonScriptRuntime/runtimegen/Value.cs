using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class Value
    {
        public int type;
        public object internalValue;

        public Value(int type, object internalValue)
        {
            this.type = type;
            this.internalValue = internalValue;
        }
    }
}
