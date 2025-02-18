using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class CatchChunk
    {
        public Token[][] ExceptionNames;
        public ClassEntity[] ClassDefinitions;
        public Token exceptionVarName;
        public Statement[] Code;
        public bool IsCatchAll;

        public CatchChunk(Token[][] ExceptionNames, ClassEntity[] ClassDefinitions, Token exceptionVarName, Statement[] Code, bool IsCatchAll)
        {
            this.ExceptionNames = ExceptionNames;
            this.ClassDefinitions = ClassDefinitions;
            this.exceptionVarName = exceptionVarName;
            this.Code = Code;
            this.IsCatchAll = IsCatchAll;
        }
    }
}
