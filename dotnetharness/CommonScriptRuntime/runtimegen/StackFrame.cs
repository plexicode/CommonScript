using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class StackFrame
    {
        public StackFrame previous;
        public int pc;
        public Value[] args;
        public int argc;
        public int valueStackSize;
        public int valueStackBaseSize;
        public System.Collections.Generic.Dictionary<string, Value> locals;
        public Value context;
        public bool useContextAsReturnValue;
        public Value bubblingValue;
        public bool bubblingValueUncaught;

        public StackFrame(StackFrame previous, int pc, Value[] args, int argc, int valueStackSize, int valueStackBaseSize, System.Collections.Generic.Dictionary<string, Value> locals, Value context, bool useContextAsReturnValue, Value bubblingValue, bool bubblingValueUncaught)
        {
            this.previous = previous;
            this.pc = pc;
            this.args = args;
            this.argc = argc;
            this.valueStackSize = valueStackSize;
            this.valueStackBaseSize = valueStackBaseSize;
            this.locals = locals;
            this.context = context;
            this.useContextAsReturnValue = useContextAsReturnValue;
            this.bubblingValue = bubblingValue;
            this.bubblingValueUncaught = bubblingValueUncaught;
        }
    }
}
