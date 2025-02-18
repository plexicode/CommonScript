using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class TokenStream
    {
        public int index;
        public int length;
        public Token[] tokens;
        public string file;

        public TokenStream(int index, int length, Token[] tokens, string file)
        {
            this.index = index;
            this.length = length;
            this.tokens = tokens;
            this.file = file;
        }
    }
}
