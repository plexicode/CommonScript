using CommonScript.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Compiler
{
    internal class CatchChunk
    {
        public Token CatchToken;
        public Token[][] ExceptionNames;
        public AbstractEntity[] ClassDefinitions;
        public Token exceptionVarName;
        public Statement[] Code;
        public bool IsCatchAll;
    }
}
