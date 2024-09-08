using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class Token
    {
        public string filename;
        public int line;
        public int col;

        public Token(string filename, int line, int col)
        {
            this.filename = filename;
            this.line = line;
            this.col = col;
        }
    }

}
