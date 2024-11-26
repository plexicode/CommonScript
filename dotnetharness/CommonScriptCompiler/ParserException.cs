using System;

namespace CommonScript.Compiler
{
    internal class ParserException : Exception
    {
        public ParserException(Token? token, string msg)
            : base(ConstructPrefix(token) + msg) { }

        public ParserException(string file, string msg) : base("[" + file + "] " + msg) { }

        public ParserException(string msg) : base(msg) { }

        private static string ConstructPrefix(Token? token)
        {
            if (token == null) return "";
            return "[" + token.File + " Line " + token.Line + " Col " + token.Col + "] ";
        }
    }
}
