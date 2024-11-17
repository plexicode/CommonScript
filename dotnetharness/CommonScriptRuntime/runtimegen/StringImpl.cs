using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class StringImpl
    {
        public int length;
        public bool isBuilder;
        public int[] uChars;
        public string nativeStr;
        public StringImpl left;
        public StringImpl right;

        public StringImpl(int length, bool isBuilder, int[] uChars, string nativeStr, StringImpl left, StringImpl right)
        {
            this.length = length;
            this.isBuilder = isBuilder;
            this.uChars = uChars;
            this.nativeStr = nativeStr;
            this.left = left;
            this.right = right;
        }
    }
}
