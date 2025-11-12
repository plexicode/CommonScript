using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class FunctionInfo
    {
        public int argcMin;
        public int argcMax;
        public int pc;
        public ClassInfo classParent;
        public string name;
        public string moduleId;

        public FunctionInfo(int argcMin, int argcMax, int pc, ClassInfo classParent, string name, string moduleId)
        {
            this.argcMin = argcMin;
            this.argcMax = argcMax;
            this.pc = pc;
            this.classParent = classParent;
            this.name = name;
            this.moduleId = moduleId;
        }
    }
}
