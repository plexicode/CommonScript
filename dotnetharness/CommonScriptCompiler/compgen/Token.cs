using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class Token
    {
        public string Value;
        public string File;
        public int Type;
        public int Line;
        public int Col;
        public string Fingerprint;

        public Token(string Value, string File, int Type, int Line, int Col, string Fingerprint)
        {
            this.Value = Value;
            this.File = File;
            this.Type = Type;
            this.Line = Line;
            this.Col = Col;
            this.Fingerprint = Fingerprint;
        }
    }
}
