using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class RawDataParser
    {
        public int[] rawBytes;
        public int index;
        public int length;
        public int intOut;

        public RawDataParser(int[] rawBytes, int index, int length, int intOut)
        {
            this.rawBytes = rawBytes;
            this.index = index;
            this.length = length;
            this.intOut = intOut;
        }
    }

}
