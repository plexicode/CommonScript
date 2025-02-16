using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal enum TokenType
    {
        KEYWORD = 1,
        NAME = 2,
        PUNCTUATION = 3,
        STRING = 4,
        INTEGER = 5,
        HEX_INTEGER = 6,
        FLOAT = 7,
        ANNOTATION = 8,

        EOF = 9,
    }

    internal class TokenUtil
    {
        public static Token createFakeToken(TokenStream tokens, int type, string value, int line, int col)
        {
            return FunctionWrapper.Token_new(value, type, tokens.GetFile(), line, col);
            
        }

        public string getFingerprint(Token token)
        {
            return FunctionWrapper.Token_getFingerprint(token);
        }
    }
}
