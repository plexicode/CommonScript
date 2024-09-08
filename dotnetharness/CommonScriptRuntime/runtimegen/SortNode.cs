using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class SortNode
    {
        public Value value;
        public Value outputValue;
        public SortNode next;

        public SortNode(Value value, Value outputValue, SortNode next)
        {
            this.value = value;
            this.outputValue = outputValue;
            this.next = next;
        }
    }

}
