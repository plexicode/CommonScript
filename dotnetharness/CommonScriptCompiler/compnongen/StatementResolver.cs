using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class StatementResolverUtil
    {
        public static Statement StatementResolver_ResolveStatementFirstPass(Resolver resolver, Statement s)
        {
            switch (s.type)
            {
                case (int) StatementType.ASSIGNMENT: return StatementResolver_FirstPass_Assignment(resolver, s);
                case (int) StatementType.BREAK: return StatementResolver_FirstPass_Break(resolver, s);
                case (int) StatementType.CONTINUE: return StatementResolver_FirstPass_Continue(resolver, s);
                case (int) StatementType.DO_WHILE_LOOP: return StatementResolver_FirstPass_DoWhileLoop(resolver, s);
                case (int) StatementType.FOR_EACH_LOOP: return StatementResolver_FirstPass_ForEachLoop(resolver, s);
                case (int) StatementType.FOR_LOOP: return StatementResolver_FirstPass_ForLoop(resolver, s);
                case (int) StatementType.IF_STATEMENT: return StatementResolver_FirstPass_IfStatement(resolver, s);
                case (int) StatementType.SWITCH_STATEMENT: return StatementResolver_FirstPass_SwitchStatement(resolver, s);
                case (int) StatementType.TRY: return StatementResolver_FirstPass_Try(resolver, s);
                case (int) StatementType.WHILE_LOOP: return StatementResolver_FirstPass_WhileLoop(resolver, s);

                case (int) StatementType.RETURN:
                case (int) StatementType.THROW:
                case (int) StatementType.EXPRESSION_AS_STATEMENT:
                    s.expression = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, s.expression);
                    break;

                default:
                    FunctionWrapper.fail("Not implemented");
                    break;
            }
            return s;
        }

        public static Statement StatementResolver_ResolveStatementSecondPass(Resolver resolver, Statement s)
        {
            switch (s.type)
            {
                case (int) StatementType.ASSIGNMENT: return FunctionWrapper.StatementResolver_SecondPass_Assignment(resolver, s);
                case (int) StatementType.BREAK: return FunctionWrapper.StatementResolver_SecondPass_Break(resolver, s);
                case (int) StatementType.CONTINUE: return FunctionWrapper.StatementResolver_SecondPass_Continue(resolver, s);
                case (int) StatementType.DO_WHILE_LOOP: return FunctionWrapper.StatementResolver_SecondPass_DoWhileLoop(resolver, s);
                case (int) StatementType.EXPRESSION_AS_STATEMENT: return FunctionWrapper.StatementResolver_SecondPass_ExpressionAsStatement(resolver, s);
                case (int) StatementType.FOR_LOOP: return FunctionWrapper.StatementResolver_SecondPass_ForLoop(resolver, s);
                case (int) StatementType.FOR_EACH_LOOP: return FunctionWrapper.StatementResolver_SecondPass_ForEachLoop(resolver, s);
                case (int) StatementType.IF_STATEMENT: return FunctionWrapper.StatementResolver_SecondPass_IfStatement(resolver, s);
                case (int) StatementType.RETURN: return FunctionWrapper.StatementResolver_SecondPass_Return(resolver, s);
                case (int) StatementType.SWITCH_STATEMENT: return FunctionWrapper.StatementResolver_SecondPass_SwitchStatement(resolver, s);
                case (int) StatementType.THROW: return FunctionWrapper.StatementResolver_SecondPass_ThrowStatement(resolver, s);
                case (int) StatementType.TRY: return FunctionWrapper.StatementResolver_SecondPass_TryStatement(resolver, s);
                case (int) StatementType.WHILE_LOOP: return FunctionWrapper.StatementResolver_SecondPass_WhileLoop(resolver, s);
            }

            FunctionWrapper.fail("Not implemented");
            return null;
        }

        private static Statement StatementResolver_FirstPass_Assignment(Resolver resolver, Statement assign)
        {
            assign.assignTarget = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, assign.assignTarget);
            assign.assignValue = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, assign.assignValue);

            if (assign.assignTarget.type == (int) ExpressionType.VARIABLE)
            {
                // register that this variable was assigned to in this variable scope
                ((FunctionEntity)resolver.activeEntity.specificData).variableScope[assign.assignTarget.strVal] = true;
            }

            return assign;
        }

        private static Statement StatementResolver_FirstPass_Break(Resolver resolver, Statement br)
        {
            if (resolver.breakContext == null)
            {
                FunctionWrapper.Errors_Throw(br.firstToken, "The 'break' keyword can only be used inside loops and switch statements.");
            }
            else if (resolver.breakContext.type == (int) StatementType.TRY)
            {
                FunctionWrapper.Errors_Throw(br.firstToken, "The 'break' keyword cannot be used inside a try/catch/finally block");
            }

            return br;
        }

        private static Statement StatementResolver_FirstPass_Continue(Resolver resolver, Statement cont)
        {
            if (resolver.breakContext == null)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword can only be used inside loops.");
            }
            else if (resolver.breakContext.type == (int) StatementType.SWITCH_STATEMENT)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword cannot be used in switch statements, even if nested in a loop.");
            }
            else if (resolver.breakContext.type == (int) StatementType.TRY)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword cannot be used inside a try/catch/finally block");
            }

            return cont;
        }

        private static Statement StatementResolver_FirstPass_DoWhileLoop(Resolver resolver, Statement doWhileLoop)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = doWhileLoop;
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, doWhileLoop.code);
            resolver.breakContext = oldBreakContext;
            doWhileLoop.condition = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, doWhileLoop.condition);
            return doWhileLoop;
        }

        private static Statement StatementResolver_FirstPass_ForEachLoop(Resolver resolver, Statement forEachLoop)
        {
            forEachLoop.autoId = FunctionWrapper.EntityResolver_GetNextAutoVarId(resolver);
            forEachLoop.expression = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, forEachLoop.expression);
            ((FunctionEntity)resolver.activeEntity.specificData).variableScope[forEachLoop.varToken.Value] = true;
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, forEachLoop.code);
            return forEachLoop;
        }

        private static Statement StatementResolver_FirstPass_ForLoop(Resolver resolver, Statement forLoop)
        {
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.forInit);
            forLoop.condition = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, forLoop.condition);
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.forStep);

            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = forLoop;
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.code);
            resolver.breakContext = oldBreakContext;
            return forLoop;
        }

        private static Statement StatementResolver_FirstPass_IfStatement(Resolver resolver, Statement ifStatement)
        {
            ifStatement.condition = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, ifStatement.condition);
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement.code);
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement.elseCode);
            return ifStatement;
        }

        private static Statement StatementResolver_FirstPass_SwitchStatement(Resolver resolver, Statement switchStmnt)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = switchStmnt;
            switchStmnt.condition = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, switchStmnt.condition);
            for (int i = 0; i < switchStmnt.switchChunks.Length; i += 1)
            {
                SwitchChunk chunk = switchStmnt.switchChunks[i];
                for (int j = 0; j < chunk.Cases.Count; j += 1)
                {
                    Expression expr = chunk.Cases[j];
                    if (expr != null)
                    {
                        chunk.Cases[j] = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, expr);
                    }
                }

                for (int j = 0; j < chunk.Code.Count; j += 1)
                {
                    chunk.Code[j] = resolver.ResolveStatementFirstPass(resolver, chunk.Code[j]);
                }
            }
            resolver.breakContext = oldBreakContext;
            return switchStmnt;
        }

        private static Statement StatementResolver_FirstPass_Try(Resolver resolver, Statement tryStatement)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = tryStatement;
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement.code);
            for (int i = 0; i < tryStatement.catchChunks.Length; i += 1)
            {
                CatchChunk cc = tryStatement.catchChunks[i];
                if (cc.exceptionVarName != null)
                {
                    ((FunctionEntity)resolver.activeEntity.specificData).variableScope[cc.exceptionVarName.Value] = true;
                }

                if (cc.ExceptionNames.Length > 0)
                {
                    // TODO: the code for resolving a dotted name for a class (particularly in the base class resolution) needs 
                    // to be refactored and used here as well.
                    FunctionWrapper.fail("Not implemented");
                }
                else
                {
                    cc.IsCatchAll = true;
                    cc.ClassDefinitions = [];
                }

                FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, cc.Code);
            }

            // TODO: check order and redundancy of exceptions.

            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement.finallyCode);

            resolver.breakContext = oldBreakContext;

            return tryStatement;
        }

        private static Statement StatementResolver_FirstPass_WhileLoop(Resolver resolver, Statement whileLoop)
        {
            whileLoop.condition = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, whileLoop.condition);
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = whileLoop;
            FunctionWrapper.StatementResolver_ResolveStatementArrayFirstPass(resolver, whileLoop.code);
            resolver.breakContext = oldBreakContext;
            return whileLoop;
        }
    }
}
