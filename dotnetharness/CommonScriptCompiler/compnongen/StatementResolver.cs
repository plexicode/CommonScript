using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class StatementResolverUtil
    {
        public static void StatementResolver_ResolveStatementArrayFirstPass(Resolver resolver, Statement[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = StatementResolver_ResolveStatementFirstPass(resolver, arr[i]);
            }
        }

        public static void StatementResolver_ResolveStatementArraySecondPass(Resolver resolver, Statement[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = StatementResolver_ResolveStatementSecondPass(resolver, arr[i]);
            }
        }

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
                    s.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, s.expression);
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
                case (int) StatementType.ASSIGNMENT: return StatementResolver_SecondPass_Assignment(resolver, s);
                case (int) StatementType.BREAK: return StatementResolver_SecondPass_Break(resolver, s);
                case (int) StatementType.CONTINUE: return StatementResolver_SecondPass_Continue(resolver, s);
                case (int) StatementType.DO_WHILE_LOOP: return StatementResolver_SecondPass_DoWhileLoop(resolver, s);
                case (int) StatementType.EXPRESSION_AS_STATEMENT: return StatementResolver_SecondPass_ExpressionAsStatement(resolver, s);
                case (int) StatementType.FOR_LOOP: return StatementResolver_SecondPass_ForLoop(resolver, s);
                case (int) StatementType.FOR_EACH_LOOP: return StatementResolver_SecondPass_ForEachLoop(resolver, s);
                case (int) StatementType.IF_STATEMENT: return StatementResolver_SecondPass_IfStatement(resolver, s);
                case (int) StatementType.RETURN: return StatementResolver_SecondPass_Return(resolver, s);
                case (int) StatementType.SWITCH_STATEMENT: return StatementResolver_SecondPass_SwitchStatement(resolver, s);
                case (int) StatementType.THROW: return StatementResolver_SecondPass_ThrowStatement(resolver, s);
                case (int) StatementType.TRY: return StatementResolver_SecondPass_TryStatement(resolver, s);
                case (int) StatementType.WHILE_LOOP: return StatementResolver_SecondPass_WhileLoop(resolver, s);
            }

            FunctionWrapper.fail("Not implemented");
            return null;
        }

        private static Statement StatementResolver_FirstPass_Assignment(Resolver resolver, Statement assign)
        {
            assign.assignTarget = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, assign.assignTarget);
            assign.assignValue = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, assign.assignValue);

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
            StatementResolver_ResolveStatementArrayFirstPass(resolver, doWhileLoop.code);
            resolver.breakContext = oldBreakContext;
            doWhileLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, doWhileLoop.condition);
            return doWhileLoop;
        }

        private static Statement StatementResolver_FirstPass_ForEachLoop(Resolver resolver, Statement forEachLoop)
        {
            forEachLoop.autoId = FunctionWrapper.EntityResolver_GetNextAutoVarId(resolver);
            forEachLoop.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, forEachLoop.expression);
            ((FunctionEntity)resolver.activeEntity.specificData).variableScope[forEachLoop.varToken.Value] = true;
            StatementResolver_ResolveStatementArrayFirstPass(resolver, forEachLoop.code);
            return forEachLoop;
        }

        private static Statement StatementResolver_FirstPass_ForLoop(Resolver resolver, Statement forLoop)
        {
            StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.forInit);
            forLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, forLoop.condition);
            StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.forStep);

            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = forLoop;
            StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop.code);
            resolver.breakContext = oldBreakContext;
            return forLoop;
        }

        private static Statement StatementResolver_FirstPass_IfStatement(Resolver resolver, Statement ifStatement)
        {
            ifStatement.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, ifStatement.condition);
            StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement.code);
            StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement.elseCode);
            return ifStatement;
        }

        private static Statement StatementResolver_FirstPass_SwitchStatement(Resolver resolver, Statement switchStmnt)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = switchStmnt;
            switchStmnt.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, switchStmnt.condition);
            foreach (SwitchChunk chunk in switchStmnt.switchChunks)
            {
                for (int i = 0; i < chunk.Cases.Count; i++)
                {
                    Expression expr = chunk.Cases[i];
                    if (expr != null)
                    {
                        chunk.Cases[i] = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, expr);
                    }
                }

                for (int i = 0; i < chunk.Code.Count; i++)
                {
                    chunk.Code[i] = StatementResolver_ResolveStatementFirstPass(resolver, chunk.Code[i]);
                }
            }
            resolver.breakContext = oldBreakContext;
            return switchStmnt;
        }

        private static Statement StatementResolver_FirstPass_Try(Resolver resolver, Statement tryStatement)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = tryStatement;
            StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement.code);
            foreach (CatchChunk cc in tryStatement.catchChunks)
            {
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

                StatementResolver_ResolveStatementArrayFirstPass(resolver, cc.Code);
            }

            // TODO: check order and redundancy of exceptions.

            StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement.finallyCode);

            resolver.breakContext = oldBreakContext;

            return tryStatement;
        }

        private static Statement StatementResolver_FirstPass_WhileLoop(Resolver resolver, Statement whileLoop)
        {
            whileLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, whileLoop.condition);
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = whileLoop;
            StatementResolver_ResolveStatementArrayFirstPass(resolver, whileLoop.code);
            resolver.breakContext = oldBreakContext;
            return whileLoop;
        }

        private static Statement StatementResolver_SecondPass_Assignment(Resolver resolver, Statement assignment)
        {
            assignment.assignTarget = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, assignment.assignTarget);
            assignment.assignValue = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, assignment.assignValue);

            Expression target = assignment.assignTarget;
            switch (target.type)
            {
                case (int) ExpressionType.INDEX:
                case (int) ExpressionType.DOT_FIELD:
                case (int) ExpressionType.VARIABLE:
                    // These are fine.
                    break;

                default:
                    FunctionWrapper.Errors_Throw(assignment.assignOp, "Invalid assignment. Cannot assign to this type of expression.");
                    break;
            }
            return assignment;
        }

        private static Statement StatementResolver_SecondPass_Break(Resolver resolver, Statement br)
        {
            return br;
        }

        private static Statement StatementResolver_SecondPass_Continue(Resolver resolver, Statement cont)
        {
            return cont;
        }

        private static Statement StatementResolver_SecondPass_DoWhileLoop(Resolver resolver, Statement doWhileLoop)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = doWhileLoop;
            StatementResolver_ResolveStatementArraySecondPass(resolver, doWhileLoop.code);
            doWhileLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, doWhileLoop.condition);
            resolver.breakContext = oldBreakContext;
            return doWhileLoop;
        }

        private static Statement StatementResolver_SecondPass_ExpressionAsStatement(Resolver resolver, Statement exprAsStmnt)
        {
            exprAsStmnt.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, exprAsStmnt.expression);
            switch (exprAsStmnt.expression.type)
            {
                case (int) ExpressionType.FUNCTION_INVOKE:
                case (int) ExpressionType.EXTENSION_INVOCATION:
                case (int) ExpressionType.INLINE_INCREMENT:
                    // these are fine 
                    break;

                default:
                    FunctionWrapper.Errors_Throw(exprAsStmnt.firstToken, "This type of expression cannot exist by itself. Did you mean to assign it to a variable?");
                    break;
            }

            return exprAsStmnt;
        }

        private static Statement StatementResolver_SecondPass_ForLoop(Resolver resolver, Statement forLoop)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = forLoop;
            StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop.forInit);
            forLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, forLoop.condition);
            StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop.forStep);
            StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop.code);
            resolver.breakContext = oldBreakContext;
            return forLoop;
        }

        private static Statement StatementResolver_SecondPass_ForEachLoop(Resolver resolver, Statement forEachLoop)
        {
            forEachLoop.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, forEachLoop.expression);
            StatementResolver_ResolveStatementArraySecondPass(resolver, forEachLoop.code);
            return forEachLoop;
        }

        private static Statement StatementResolver_SecondPass_IfStatement(Resolver resolver, Statement ifStatement)
        {
            ifStatement.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, ifStatement.condition);
            StatementResolver_ResolveStatementArraySecondPass(resolver, ifStatement.code);
            StatementResolver_ResolveStatementArraySecondPass(resolver, ifStatement.elseCode);
            return ifStatement;
        }

        private static Statement StatementResolver_SecondPass_Return(Resolver resolver, Statement ret)
        {
            ret.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, ret.expression);
            return ret;
        }

        private static Statement StatementResolver_SecondPass_SwitchStatement(Resolver resolver, Statement switchStmnt)
        {
            switchStmnt.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, switchStmnt.condition);
            int exprType = -1; // { -1: indeterminite | 1: ints | 2: strings }
            Dictionary<string, bool> strCollisions = new Dictionary<string, bool>();
            Dictionary<int, bool> intCollisions = new Dictionary<int, bool>();

            foreach (SwitchChunk chunk in switchStmnt.switchChunks)
            {
                for (int i = 0; i < chunk.Cases.Count; i++)
                {
                    Expression expr = chunk.Cases[i];
                    if (expr != null)
                    {
                        expr = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, expr);
                        chunk.Cases[i] = expr;
                        if (!FunctionWrapper.IsExpressionConstant(expr))
                        {
                            FunctionWrapper.Errors_Throw(expr.firstToken, "Only constant expressions are allowed in switch statement cases.");
                        }
                        int currentType = -1;
                        bool hadCollision = false;
                        if (expr.type == (int) ExpressionType.INTEGER_CONST)
                        {
                            currentType = 1;
                            hadCollision = intCollisions.ContainsKey(expr.intVal);
                            intCollisions[expr.intVal] = true;
                        }
                        else if (expr.type == (int) ExpressionType.STRING_CONST)
                        {
                            currentType = 2;
                            hadCollision = strCollisions.ContainsKey(expr.strVal);
                            strCollisions[expr.strVal] = true;
                        }
                        else
                        {
                            FunctionWrapper.Errors_Throw(expr.firstToken, "Only integer and string constants are allowed to be used as switch statement cases.");
                        }
                        if (exprType == -1) exprType = currentType;
                        if (exprType != currentType) FunctionWrapper.Errors_Throw(expr.firstToken, "Switch statement cases must use the same type for all cases.");
                        if (hadCollision) FunctionWrapper.Errors_Throw(expr.firstToken, "Switch statement contains multiple cases with the same value.");
                    }
                }

                for (int i = 0; i < chunk.Code.Count; i++)
                {
                    chunk.Code[i] = StatementResolver_ResolveStatementSecondPass(resolver, chunk.Code[i]);
                }

                switch (chunk.Code[chunk.Code.Count - 1].type)
                {
                    case (int) StatementType.BREAK:
                    case (int) StatementType.RETURN:
                    case (int) StatementType.THROW:
                        // these are fine.
                        break;

                    default:
                        FunctionWrapper.Errors_Throw(chunk.CaseTokens[chunk.CaseTokens.Count - 1], "This switch statement case has a fall-through.");
                        break;
                }
            }

            return switchStmnt;
        }

        private static Statement StatementResolver_SecondPass_ThrowStatement(Resolver resolver, Statement throwStmnt)
        {
            throwStmnt.expression = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, throwStmnt.expression);
            if (FunctionWrapper.IsExpressionConstant(throwStmnt.expression))
            {
                FunctionWrapper.Errors_Throw(throwStmnt.expression.firstToken, "Only instances of Exception are throwable.");
            }
            return throwStmnt;
        }

        private static Statement StatementResolver_SecondPass_TryStatement(Resolver resolver, Statement tryStmnt)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = tryStmnt;
            StatementResolver_ResolveStatementArraySecondPass(resolver, tryStmnt.code);
            foreach (CatchChunk cc in tryStmnt.catchChunks)
            {
                StatementResolver_ResolveStatementArraySecondPass(resolver, cc.Code);
            }
            StatementResolver_ResolveStatementArraySecondPass(resolver, tryStmnt.finallyCode);

            resolver.breakContext = oldBreakContext;

            return tryStmnt;
        }

        private static Statement StatementResolver_SecondPass_WhileLoop(Resolver resolver, Statement whileLoop)
        {
            Statement oldBreakContext = resolver.breakContext;
            resolver.breakContext = whileLoop;
            whileLoop.condition = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, whileLoop.condition);
            StatementResolver_ResolveStatementArraySecondPass(resolver, whileLoop.code);
            resolver.breakContext = oldBreakContext;
            return whileLoop;
        }
    }
}
