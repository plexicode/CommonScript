namespace CommonScript.Compiler
{
    internal enum StatementType
    {
        ASSIGNMENT,
        BREAK,
        CONTINUE,
        DO_WHILE_LOOP,
        EXPRESSION_AS_STATEMENT,
        FOR_LOOP,
        FOR_EACH_LOOP,
        IF_STATEMENT,
        RETURN,
        SWITCH_STATEMENT,
        THROW,
        TRY,
        WHILE_LOOP,
    }

    internal class Statement
    {
        public Token firstToken;
        public StatementType type;

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

        public int autoId = 0;

        private Statement(Token firstToken, StatementType type)
        {
            this.firstToken = firstToken;
            this.type = type;
        }

        public static Statement createAssignment(Expression targetExpr, Token assignOp, Expression valueExpr)
        {
            Statement assign = new Statement(targetExpr.firstToken, StatementType.ASSIGNMENT);
            assign.assignTarget = targetExpr;
            assign.assignValue = valueExpr;
            assign.assignOp = assignOp;

            return assign;
        }

        public static Statement createBreakContinue(Token breakContinueToken)
        {
            bool isBreak = breakContinueToken.Value == "break";
            Statement bc = new Statement(breakContinueToken, isBreak ? StatementType.BREAK : StatementType.CONTINUE);
            return bc;
        }

        public static Statement createDoWhile(Token doToken, Statement[] code, Token whileToken, Expression condition)
        {
            Statement doWhile = new Statement(doToken, StatementType.DO_WHILE_LOOP);
            doWhile.condition = condition;
            doWhile.code = code;
            doWhile.assignOp = whileToken; // probably not necessary.
            return doWhile;
        }

        public static Statement createExpressionAsStatement(Expression expr)
        {
            Statement wrapper = new Statement(expr.firstToken, StatementType.EXPRESSION_AS_STATEMENT);
            wrapper.expression = expr;
            return wrapper;
        }

        public static Statement createForLoop(Token forToken, Statement[] init, Expression condition, Statement[] step, Statement[] code)
        {
            Statement forLoop = new Statement(forToken, StatementType.FOR_LOOP);
            forLoop.condition = condition;
            forLoop.forInit = init;
            forLoop.forStep = step;
            forLoop.code = code;
            return forLoop;
        }

        public static Statement createIfStatement(Token ifToken, Expression condition, Statement[] ifCode, Statement[] elseCode)
        {
            Statement ifStatement = new Statement(ifToken, StatementType.IF_STATEMENT);
            ifStatement.condition = condition;
            ifStatement.code = ifCode;
            ifStatement.elseCode = elseCode;
            return ifStatement;
        }

        public static Statement createForEachLoop(Token forToken, Token varName, Expression listExpr, Statement[] code)
        {
            Statement forEachLoop = new Statement(forToken, StatementType.FOR_EACH_LOOP);
            forEachLoop.varToken = varName;
            forEachLoop.expression = listExpr;
            forEachLoop.code = code;
            return forEachLoop;
        }

        public static Statement createReturn(Token returnToken, Expression expr)
        {
            Statement ret = new Statement(returnToken, StatementType.RETURN);
            ret.expression = expr;
            return ret;
        }

        public static Statement createSwitchStatement(Token switchToken, Expression condition, SwitchChunk[] chunks)
        {
            Statement swtStmnt = new Statement(switchToken, StatementType.SWITCH_STATEMENT);
            swtStmnt.condition = condition;
            swtStmnt.switchChunks = chunks;
            return swtStmnt;
        }

        public static Statement createThrow(Token throwToken, Expression value)
        {
            Statement throwStmnt = new Statement(throwToken, StatementType.THROW);
            throwStmnt.expression = value;
            return throwStmnt;
        }

        public static Statement createTry(Token tryToken, Statement[] tryCode, CatchChunk[] catches, Token finallyToken, Statement[] finallyCode)
        {
            Statement tryStmnt = new Statement(tryToken, StatementType.TRY);
            tryStmnt.code = tryCode;
            tryStmnt.catchChunks = catches;
            tryStmnt.finallyCode = finallyCode;
            tryStmnt.finallyToken = finallyToken;
            return tryStmnt;
        }

        public static Statement createWhileLoop(Token whileToken, Expression condition, Statement[] code)
        {
            Statement whileLoop = new Statement(whileToken, StatementType.WHILE_LOOP);
            whileLoop.condition = condition;
            whileLoop.code = code;
            return whileLoop;
        }
    }
}
