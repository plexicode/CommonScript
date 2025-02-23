using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ByteStringBuilder
    {
        public bool isLeaf;
        public int length;
        public int[] bytes;
        public ByteStringBuilder left;
        public ByteStringBuilder right;

        public ByteStringBuilder(bool isLeaf, int length, int[] bytes, ByteStringBuilder left, ByteStringBuilder right)
        {
            this.isLeaf = isLeaf;
            this.length = length;
            this.bytes = bytes;
            this.left = left;
            this.right = right;
        }
    }
}
