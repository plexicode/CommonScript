using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class BundleFunctionInfo
    {
        public ByteCodeRow[] code;
        public int argcMin;
        public int argcMax;
        public string name;
        public string moduleId;

        public BundleFunctionInfo(ByteCodeRow[] code, int argcMin, int argcMax, string name, string moduleId)
        {
            this.code = code;
            this.argcMin = argcMin;
            this.argcMax = argcMax;
            this.name = name;
            this.moduleId = moduleId;
        }
    }
}
