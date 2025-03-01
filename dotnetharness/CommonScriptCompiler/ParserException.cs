using System;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ParserException : Exception
    {
        public ParserException(Token? token, string msg)
            : base(FunctionWrapper.PUBLIC_getTokenErrPrefix(token) + msg) { }

        public ParserException(string file, string msg) : base("[" + file + "] " + msg) { }

        public ParserException(string msg) : base(msg) { }
    }
}
