using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class StatementUtil
    {
        public static Statement createAssignment(Expression targetExpr, Token assignOp, Expression valueExpr)
        {
            Statement assign = FunctionWrapper.Statement_new(targetExpr.firstToken, (int) StatementType.ASSIGNMENT);
            assign.assignTarget = targetExpr;
            assign.assignValue = valueExpr;
            assign.assignOp = assignOp;

            return assign;
        }

        public static Statement createBreakContinue(Token breakContinueToken)
        {
            bool isBreak = breakContinueToken.Value == "break";
            Statement bc = FunctionWrapper.Statement_new(breakContinueToken, isBreak ? (int) StatementType.BREAK : (int) StatementType.CONTINUE);
            return bc;
        }

        public static Statement createDoWhile(Token doToken, Statement[] code, Token whileToken, Expression condition)
        {
            Statement doWhile = FunctionWrapper.Statement_new(doToken, (int) StatementType.DO_WHILE_LOOP);
            doWhile.condition = condition;
            doWhile.code = code;
            doWhile.assignOp = whileToken; // probably not necessary.
            return doWhile;
        }

        public static Statement createExpressionAsStatement(Expression expr)
        {
            Statement wrapper = FunctionWrapper.Statement_new(expr.firstToken, (int) StatementType.EXPRESSION_AS_STATEMENT);
            wrapper.expression = expr;
            return wrapper;
        }

        public static Statement createForLoop(Token forToken, Statement[] init, Expression condition, Statement[] step, Statement[] code)
        {
            Statement forLoop = FunctionWrapper.Statement_new(forToken, (int) StatementType.FOR_LOOP);
            forLoop.condition = condition;
            forLoop.forInit = init;
            forLoop.forStep = step;
            forLoop.code = code;
            return forLoop;
        }

        public static Statement createIfStatement(Token ifToken, Expression condition, Statement[] ifCode, Statement[] elseCode)
        {
            Statement ifStatement = FunctionWrapper.Statement_new(ifToken, (int) StatementType.IF_STATEMENT);
            ifStatement.condition = condition;
            ifStatement.code = ifCode;
            ifStatement.elseCode = elseCode;
            return ifStatement;
        }

        public static Statement createForEachLoop(Token forToken, Token varName, Expression listExpr, Statement[] code)
        {
            Statement forEachLoop = FunctionWrapper.Statement_new(forToken, (int) StatementType.FOR_EACH_LOOP);
            forEachLoop.varToken = varName;
            forEachLoop.expression = listExpr;
            forEachLoop.code = code;
            return forEachLoop;
        }

        public static Statement createReturn(Token returnToken, Expression expr)
        {
            Statement ret = FunctionWrapper.Statement_new(returnToken, (int) StatementType.RETURN);
            ret.expression = expr;
            return ret;
        }

        public static Statement createSwitchStatement(Token switchToken, Expression condition, SwitchChunk[] chunks)
        {
            Statement swtStmnt = FunctionWrapper.Statement_new(switchToken, (int) StatementType.SWITCH_STATEMENT);
            swtStmnt.condition = condition;
            swtStmnt.switchChunks = chunks;
            return swtStmnt;
        }

        public static Statement createThrow(Token throwToken, Expression value)
        {
            Statement throwStmnt = FunctionWrapper.Statement_new(throwToken, (int) StatementType.THROW);
            throwStmnt.expression = value;
            return throwStmnt;
        }

        public static Statement createTry(Token tryToken, Statement[] tryCode, CatchChunk[] catches, Token finallyToken, Statement[] finallyCode)
        {
            Statement tryStmnt = FunctionWrapper.Statement_new(tryToken, (int) StatementType.TRY);
            tryStmnt.code = tryCode;
            tryStmnt.catchChunks = catches;
            tryStmnt.finallyCode = finallyCode;
            tryStmnt.finallyToken = finallyToken;
            return tryStmnt;
        }

        public static Statement createWhileLoop(Token whileToken, Expression condition, Statement[] code)
        {
            Statement whileLoop = FunctionWrapper.Statement_new(whileToken, (int) StatementType.WHILE_LOOP);
            whileLoop.condition = condition;
            whileLoop.code = code;
            return whileLoop;
        }
    }
}
