using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ByteCodeRow
    {
        public int op;
        public int[] args;
        public int firstArg;
        public int secondArg;
        public int stringId;
        public string stringArg;
        public int tokenId;
        public Token token;
        public Value valueCache;
        public bool boolArg;
        public bool boolArg2;

        public ByteCodeRow(int op, int[] args, int firstArg, int secondArg, int stringId, string stringArg, int tokenId, Token token, Value valueCache, bool boolArg, bool boolArg2)
        {
            this.op = op;
            this.args = args;
            this.firstArg = firstArg;
            this.secondArg = secondArg;
            this.stringId = stringId;
            this.stringArg = stringArg;
            this.tokenId = tokenId;
            this.token = token;
            this.valueCache = valueCache;
            this.boolArg = boolArg;
            this.boolArg2 = boolArg2;
        }
    }

}
