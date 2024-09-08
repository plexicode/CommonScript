using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class GlobalValues
    {
        public Value nullValue;
        public Value trueValue;
        public Value falseValue;
        public Value[] posIntegers;
        public Value[] negIntegers;
        public Value emptyString;
        public System.Collections.Generic.Dictionary<string, Value> commonStrings;
        public Value intZero;
        public Value intOne;
        public Value[] floatsBy4x;

        public GlobalValues(Value nullValue, Value trueValue, Value falseValue, Value[] posIntegers, Value[] negIntegers, Value emptyString, System.Collections.Generic.Dictionary<string, Value> commonStrings, Value intZero, Value intOne, Value[] floatsBy4x)
        {
            this.nullValue = nullValue;
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            this.posIntegers = posIntegers;
            this.negIntegers = negIntegers;
            this.emptyString = emptyString;
            this.commonStrings = commonStrings;
            this.intZero = intZero;
            this.intOne = intOne;
            this.floatsBy4x = floatsBy4x;
        }
    }

}
