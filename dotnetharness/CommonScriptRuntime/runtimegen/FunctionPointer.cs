using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class FunctionPointer
    {
        public int funcType;
        public int argcMin;
        public int argcMax;
        public int pcOrId;
        public FunctionInfo func;
        public Value ctx;

        public FunctionPointer(int funcType, int argcMin, int argcMax, int pcOrId, FunctionInfo func, Value ctx)
        {
            this.funcType = funcType;
            this.argcMin = argcMin;
            this.argcMax = argcMax;
            this.pcOrId = pcOrId;
            this.func = func;
            this.ctx = ctx;
        }
    }
}
