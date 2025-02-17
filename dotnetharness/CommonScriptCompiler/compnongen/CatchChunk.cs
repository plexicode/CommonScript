using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class CatchChunk
    {
        // public Token CatchToken;
        public Token[][] ExceptionNames;
        public ClassEntity[] ClassDefinitions;
        public Token exceptionVarName;
        public Statement[] Code;
        public bool IsCatchAll;
    }
}
