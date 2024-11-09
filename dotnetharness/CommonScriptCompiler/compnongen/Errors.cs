using System;

namespace CommonScript.Compiler
{
    internal static class Errors
    {
        public static void ThrowError(Token token, string msg)
        {
            string fullMsg = "[" + token.File + " Line " + token.Line + " Col " + token.Col + "] " + msg;
            throw new Exception(fullMsg);
        }

        public static void ThrowEof(string file, string msg)
        {
            string fullMsg = "[" + file + "] " + msg;
            throw new Exception(fullMsg);
        }

        public static void ThrowGeneralError(string msg)
        {
            throw new Exception(msg);
        }

        public static void ThrowNotImplemented(Token token, string msg)
        {
            ThrowError(token, "Not implemented: " + msg);
        }
    }
}
