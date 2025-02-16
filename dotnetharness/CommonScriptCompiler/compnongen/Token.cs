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

    internal class Token
    {
        public string Value { get; set; }
        public string File { get; set; }
        public int Type { get; set; }
        public int Line { get; set; }
        public int Col { get; set; }

        public Token(string value, int type, string file, int line, int col)
        {
            this.File = file;
            this.Value = value;
            this.Type = type;
            this.Line = line;
            this.Col = col;
        }

        public static Token createFakeToken(TokenStream tokens, int type, string value, int line, int col)
        {
            return new Token(value, type, tokens.GetFile(), line, col);
        }

        public string Fingerprint { get; set; }
        public string getFingerprint()
        {
            if (this.Fingerprint == null)
            {
                this.Fingerprint = this.File + ("," + this.Line + "," + this.Col);
            }
            return this.Fingerprint;
        }
    }
}
