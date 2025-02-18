using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class Statement
    {
        public Token firstToken;
        public int type;
        public Expression expression;
        public Expression condition;
        public Expression assignTarget;
        public Expression assignValue;
        public Token assignOp;
        public Token varToken;
        public Token finallyToken;
        public Statement[] forInit;
        public Statement[] forStep;
        public Statement[] code;
        public Statement[] elseCode;
        public Statement[] finallyCode;
        public SwitchChunk[] switchChunks;
        public CatchChunk[] catchChunks;
        public int autoId;

        public Statement(Token firstToken, int type, Expression expression, Expression condition, Expression assignTarget, Expression assignValue, Token assignOp, Token varToken, Token finallyToken, Statement[] forInit, Statement[] forStep, Statement[] code, Statement[] elseCode, Statement[] finallyCode, SwitchChunk[] switchChunks, CatchChunk[] catchChunks, int autoId)
        {
            this.firstToken = firstToken;
            this.type = type;
            this.expression = expression;
            this.condition = condition;
            this.assignTarget = assignTarget;
            this.assignValue = assignValue;
            this.assignOp = assignOp;
            this.varToken = varToken;
            this.finallyToken = finallyToken;
            this.forInit = forInit;
            this.forStep = forStep;
            this.code = code;
            this.elseCode = elseCode;
            this.finallyCode = finallyCode;
            this.switchChunks = switchChunks;
            this.catchChunks = catchChunks;
            this.autoId = autoId;
        }
    }
}
