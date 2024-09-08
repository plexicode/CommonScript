using CommonScript.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class SwitchChunk
    {
        public List<Token> CaseTokens { get; private set; }
        public List<Expression> Cases { get; private set; }
        public List<Statement> Code { get; private set; }

        public SwitchChunk()
        {
            this.CaseTokens = new List<Token>();
            this.Cases = new List<Expression>();
            this.Code = new List<Statement>();
        }
    }
}
