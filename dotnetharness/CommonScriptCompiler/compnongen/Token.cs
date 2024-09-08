using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Compiler
{
    internal enum TokenType
    {
        KEYWORD,
        NAME,
        PUNCTUATION,
        STRING,
        INTEGER,
        HEX_INTEGER,
        FLOAT,
        ANNOTATION,

        EOF,
    }

    internal class Token
    {
        public string Value { get; set; }
        public string File { get; set; }
        public TokenType Type { get; set; }
        public int Line { get; set; }
        public int Col { get; set; }

        public Token(string value, TokenType type, string file, int line, int col)
        {
            this.File = file;
            this.Value = value;
            this.Type = type;
            this.Line = line;
            this.Col = col;
        }

        public static Token createFakeToken(TokenStream tokens, TokenType type, string value, int line, int col)
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
