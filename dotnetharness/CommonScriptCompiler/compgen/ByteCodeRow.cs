using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ByteCodeRow
    {
        public int opCode;
        public string stringArg;
        public Token token;
        public int[] args;
        public int stringId;
        public int tokenId;
        public int[] tryCatchInfo;

        public ByteCodeRow(int opCode, string stringArg, Token token, int[] args, int stringId, int tokenId, int[] tryCatchInfo)
        {
            this.opCode = opCode;
            this.stringArg = stringArg;
            this.token = token;
            this.args = args;
            this.stringId = stringId;
            this.tokenId = tokenId;
            this.tryCatchInfo = tryCatchInfo;
        }
    }
}
