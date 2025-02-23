using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class TokenStream
    {
        public int index;
        public int length;
        public Token[] tokens;
        public string file;
        public System.Func<TokenStream, bool, Statement> parseStatement;
        public System.Func<TokenStream, bool, Statement[]> parseCodeBlock;

        public TokenStream(int index, int length, Token[] tokens, string file, System.Func<TokenStream, bool, Statement> parseStatement, System.Func<TokenStream, bool, Statement[]> parseCodeBlock)
        {
            this.index = index;
            this.length = length;
            this.tokens = tokens;
            this.file = file;
            this.parseStatement = parseStatement;
            this.parseCodeBlock = parseCodeBlock;
        }
    }
}
