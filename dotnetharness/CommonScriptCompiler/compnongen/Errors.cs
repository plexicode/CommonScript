using System;

namespace CommonScript.Compiler
{
    internal static class Errors
    {
        public static void ThrowError(Token token, string msg)
        {
            throw new ParserException(token, msg);
        }

        public static void ThrowEof(string file, string msg)
        {
            throw new ParserException(file, msg);
        }

        public static void ThrowGeneralError(string msg)
        {
            throw new ParserException(msg);
        }

        public static void ThrowNotImplemented(Token token, string? msg)
        {
            ThrowError(token, ("***NOT IMPLEMENTED*** " + msg).Trim());
        }
    }
}
