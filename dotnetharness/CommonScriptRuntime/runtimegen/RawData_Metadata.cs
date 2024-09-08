using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class RawData_Metadata
    {
        public int mainFunctionId;
        public int builtinCount;

        public RawData_Metadata(int mainFunctionId, int builtinCount)
        {
            this.mainFunctionId = mainFunctionId;
            this.builtinCount = builtinCount;
        }
    }

}
