using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ByteCodeBuffer
    {
        public int length;
        public bool isLeaf;
        public ByteCodeBuffer left;
        public ByteCodeBuffer right;
        public ByteCodeRow row;
        public ByteCodeRow first;
        public ByteCodeRow last;

        public ByteCodeBuffer(int length, bool isLeaf, ByteCodeBuffer left, ByteCodeBuffer right, ByteCodeRow row, ByteCodeRow first, ByteCodeRow last)
        {
            this.length = length;
            this.isLeaf = isLeaf;
            this.left = left;
            this.right = right;
            this.row = row;
            this.first = first;
            this.last = last;
        }
    }
}
