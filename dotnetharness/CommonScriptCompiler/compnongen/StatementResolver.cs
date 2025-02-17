using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class StatementResolver
    {
        private Resolver resolver;
        public ExpressionResolver expressionResolver;
        public EntityResolver entityResolver;

        public StatementResolver(Resolver resolver)
        {
            this.resolver = resolver;
        }

        public void ResolveStatementArrayFirstPass(Statement[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.ResolveStatementFirstPass(arr[i]);
            }
        }

        public void ResolveStatementArraySecondPass(Statement[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.ResolveStatementSecondPass(arr[i]);
            }
        }

        public Statement ResolveStatementFirstPass(Statement s)
        {
            switch (s.type)
            {
                case StatementType.ASSIGNMENT: return FirstPass_Assignment(s);
                case StatementType.BREAK: return FirstPass_Break(s);
                case StatementType.CONTINUE: return FirstPass_Continue(s);
                case StatementType.DO_WHILE_LOOP: return FirstPass_DoWhileLoop(s);
                case StatementType.FOR_EACH_LOOP: return FirstPass_ForEachLoop(s);
                case StatementType.FOR_LOOP: return FirstPass_ForLoop(s);
                case StatementType.IF_STATEMENT: return FirstPass_IfStatement(s);
                case StatementType.SWITCH_STATEMENT: return FirstPass_SwitchStatement(s);
                case StatementType.TRY: return FirstPass_Try(s);
                case StatementType.WHILE_LOOP: return FirstPass_WhileLoop(s);

                case StatementType.RETURN:
                case StatementType.THROW:
                case StatementType.EXPRESSION_AS_STATEMENT:
                    s.expression = this.expressionResolver.ResolveExpressionFirstPass(s.expression);
                    break;

                default:
                    throw new NotImplementedException();
            }
            return s;
        }

        public Statement ResolveStatementSecondPass(Statement s)
        {
            switch (s.type)
            {
                case StatementType.ASSIGNMENT: return SecondPass_Assignment(s);
                case StatementType.BREAK: return SecondPass_Break(s);
                case StatementType.CONTINUE: return SecondPass_Continue(s);
                case StatementType.DO_WHILE_LOOP: return SecondPass_DoWhileLoop(s);
                case StatementType.EXPRESSION_AS_STATEMENT: return SecondPass_ExpressionAsStatement(s);
                case StatementType.FOR_LOOP: return SecondPass_ForLoop(s);
                case StatementType.FOR_EACH_LOOP: return SecondPass_ForEachLoop(s);
                case StatementType.IF_STATEMENT: return SecondPass_IfStatement(s);
                case StatementType.RETURN: return SecondPass_Return(s);
                case StatementType.SWITCH_STATEMENT: return SecondPass_SwitchStatement(s);
                case StatementType.THROW: return SecondPass_ThrowStatement(s);
                case StatementType.TRY: return SecondPass_TryStatement(s);
                case StatementType.WHILE_LOOP: return SecondPass_WhileLoop(s);
            }

            throw new NotImplementedException();
        }

        private Statement FirstPass_Assignment(Statement assign)
        {
            assign.assignTarget = this.expressionResolver.ResolveExpressionFirstPass(assign.assignTarget);
            assign.assignValue = this.expressionResolver.ResolveExpressionFirstPass(assign.assignValue);

            if (assign.assignTarget.type == ExpressionType.VARIABLE)
            {
                // register that this variable was assigned to in this variable scope
                ((FunctionLikeEntity)this.resolver.activeEntity.specificData).variableScope[assign.assignTarget.strVal] = true;
            }

            return assign;
        }

        private Statement FirstPass_Break(Statement br)
        {
            if (this.resolver.breakContext == null)
            {
                FunctionWrapper.Errors_Throw(br.firstToken, "The 'break' keyword can only be used inside loops and switch statements.");
            }
            else if (this.resolver.breakContext.type == StatementType.TRY)
            {
                FunctionWrapper.Errors_Throw(br.firstToken, "The 'break' keyword cannot be used inside a try/catch/finally block");
            }

            return br;
        }

        private Statement FirstPass_Continue(Statement cont)
        {
            if (this.resolver.breakContext == null)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword can only be used inside loops.");
            }
            else if (this.resolver.breakContext.type == StatementType.SWITCH_STATEMENT)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword cannot be used in switch statements, even if nested in a loop.");
            }
            else if (this.resolver.breakContext.type == StatementType.TRY)
            {
                FunctionWrapper.Errors_Throw(cont.firstToken, "The 'continue' keyword cannot be used inside a try/catch/finally block");
            }

            return cont;
        }

        private Statement FirstPass_DoWhileLoop(Statement doWhileLoop)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = doWhileLoop;
            this.ResolveStatementArrayFirstPass(doWhileLoop.code);
            this.resolver.breakContext = oldBreakContext;
            doWhileLoop.condition = this.expressionResolver.ResolveExpressionFirstPass(doWhileLoop.condition);
            return doWhileLoop;
        }

        private Statement FirstPass_ForEachLoop(Statement forEachLoop)
        {
            forEachLoop.autoId = this.entityResolver.GetNextAutoVarId();
            forEachLoop.expression = this.expressionResolver.ResolveExpressionFirstPass(forEachLoop.expression);
            ((FunctionLikeEntity)this.resolver.activeEntity.specificData).variableScope[forEachLoop.varToken.Value] = true;
            this.ResolveStatementArrayFirstPass(forEachLoop.code);
            return forEachLoop;
        }

        private Statement FirstPass_ForLoop(Statement forLoop)
        {
            this.ResolveStatementArrayFirstPass(forLoop.forInit);
            forLoop.condition = this.expressionResolver.ResolveExpressionFirstPass(forLoop.condition);
            this.ResolveStatementArrayFirstPass(forLoop.forStep);

            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = forLoop;
            this.ResolveStatementArrayFirstPass(forLoop.code);
            this.resolver.breakContext = oldBreakContext;
            return forLoop;
        }

        private Statement FirstPass_IfStatement(Statement ifStatement)
        {
            ifStatement.condition = this.expressionResolver.ResolveExpressionFirstPass(ifStatement.condition);
            this.ResolveStatementArrayFirstPass(ifStatement.code);
            this.ResolveStatementArrayFirstPass(ifStatement.elseCode);
            return ifStatement;
        }

        private Statement FirstPass_SwitchStatement(Statement switchStmnt)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = switchStmnt;
            switchStmnt.condition = this.expressionResolver.ResolveExpressionFirstPass(switchStmnt.condition);
            foreach (SwitchChunk chunk in switchStmnt.switchChunks)
            {
                for (int i = 0; i < chunk.Cases.Count; i++)
                {
                    Expression expr = chunk.Cases[i];
                    if (expr != null)
                    {
                        chunk.Cases[i] = this.expressionResolver.ResolveExpressionFirstPass(expr);
                    }
                }

                for (int i = 0; i < chunk.Code.Count; i++)
                {
                    chunk.Code[i] = this.ResolveStatementFirstPass(chunk.Code[i]);
                }
            }
            this.resolver.breakContext = oldBreakContext;
            return switchStmnt;
        }

        private Statement FirstPass_Try(Statement tryStatement)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = tryStatement;
            this.ResolveStatementArrayFirstPass(tryStatement.code);
            foreach (CatchChunk cc in tryStatement.catchChunks)
            {
                if (cc.exceptionVarName != null)
                {
                    ((FunctionLikeEntity)this.resolver.activeEntity.specificData).variableScope[cc.exceptionVarName.Value] = true;
                }

                if (cc.ExceptionNames.Length > 0)
                {
                    // TODO: the code for resolving a dotted name for a class (particularly in the base class resolution) needs 
                    // to be refactored and used here as well.
                    throw new NotImplementedException();
                }
                else
                {
                    cc.IsCatchAll = true;
                    cc.ClassDefinitions = [];
                }

                this.ResolveStatementArrayFirstPass(cc.Code);
            }

            // TODO: check order and redundancy of exceptions.

            this.ResolveStatementArrayFirstPass(tryStatement.finallyCode);

            this.resolver.breakContext = oldBreakContext;

            return tryStatement;
        }

        private Statement FirstPass_WhileLoop(Statement whileLoop)
        {
            whileLoop.condition = this.expressionResolver.ResolveExpressionFirstPass(whileLoop.condition);
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = whileLoop;
            this.ResolveStatementArrayFirstPass(whileLoop.code);
            this.resolver.breakContext = oldBreakContext;
            return whileLoop;
        }

        private Statement SecondPass_Assignment(Statement assignment)
        {
            assignment.assignTarget = this.expressionResolver.ResolveExpressionSecondPass(assignment.assignTarget);
            assignment.assignValue = this.expressionResolver.ResolveExpressionSecondPass(assignment.assignValue);

            Expression target = assignment.assignTarget;
            switch (target.type)
            {
                case ExpressionType.INDEX:
                case ExpressionType.DOT_FIELD:
                case ExpressionType.VARIABLE:
                    // These are fine.
                    break;

                default:
                    FunctionWrapper.Errors_Throw(assignment.assignOp, "Invalid assignment. Cannot assign to this type of expression.");
                    break;
            }
            return assignment;
        }

        private Statement SecondPass_Break(Statement br)
        {
            return br;
        }

        private Statement SecondPass_Continue(Statement cont)
        {
            return cont;
        }

        private Statement SecondPass_DoWhileLoop(Statement doWhileLoop)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = doWhileLoop;
            this.ResolveStatementArraySecondPass(doWhileLoop.code);
            doWhileLoop.condition = this.expressionResolver.ResolveExpressionSecondPass(doWhileLoop.condition);
            this.resolver.breakContext = oldBreakContext;
            return doWhileLoop;
        }

        private Statement SecondPass_ExpressionAsStatement(Statement exprAsStmnt)
        {
            exprAsStmnt.expression = this.expressionResolver.ResolveExpressionSecondPass(exprAsStmnt.expression);
            switch (exprAsStmnt.expression.type)
            {
                case ExpressionType.FUNCTION_INVOKE:
                case ExpressionType.EXTENSION_INVOCATION:
                case ExpressionType.INLINE_INCREMENT:
                    // these are fine 
                    break;

                default:
                    FunctionWrapper.Errors_Throw(exprAsStmnt.firstToken, "This type of expression cannot exist by itself. Did you mean to assign it to a variable?");
                    break;
            }

            return exprAsStmnt;
        }

        private Statement SecondPass_ForLoop(Statement forLoop)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = forLoop;
            this.ResolveStatementArraySecondPass(forLoop.forInit);
            forLoop.condition = this.expressionResolver.ResolveExpressionSecondPass(forLoop.condition);
            this.ResolveStatementArraySecondPass(forLoop.forStep);
            this.ResolveStatementArraySecondPass(forLoop.code);
            this.resolver.breakContext = oldBreakContext;
            return forLoop;
        }

        private Statement SecondPass_ForEachLoop(Statement forEachLoop)
        {
            forEachLoop.expression = this.expressionResolver.ResolveExpressionSecondPass(forEachLoop.expression);
            this.ResolveStatementArraySecondPass(forEachLoop.code);
            return forEachLoop;
        }

        private Statement SecondPass_IfStatement(Statement ifStatement)
        {
            ifStatement.condition = this.expressionResolver.ResolveExpressionSecondPass(ifStatement.condition);
            this.ResolveStatementArraySecondPass(ifStatement.code);
            this.ResolveStatementArraySecondPass(ifStatement.elseCode);
            return ifStatement;
        }

        private Statement SecondPass_Return(Statement ret)
        {
            ret.expression = this.expressionResolver.ResolveExpressionSecondPass(ret.expression);
            return ret;
        }

        private Statement SecondPass_SwitchStatement(Statement switchStmnt)
        {
            switchStmnt.condition = this.expressionResolver.ResolveExpressionSecondPass(switchStmnt.condition);
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
                        expr = this.expressionResolver.ResolveExpressionSecondPass(expr);
                        chunk.Cases[i] = expr;
                        if (!Resolver.IsExpressionConstant(expr))
                        {
                            FunctionWrapper.Errors_Throw(expr.firstToken, "Only constant expressions are allowed in switch statement cases.");
                        }
                        int currentType = -1;
                        bool hadCollision = false;
                        if (expr.type == ExpressionType.INTEGER_CONST)
                        {
                            currentType = 1;
                            hadCollision = intCollisions.ContainsKey(expr.intVal);
                            intCollisions[expr.intVal] = true;
                        }
                        else if (expr.type == ExpressionType.STRING_CONST)
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
                    chunk.Code[i] = this.ResolveStatementSecondPass(chunk.Code[i]);
                }

                switch (chunk.Code[chunk.Code.Count - 1].type)
                {
                    case StatementType.BREAK:
                    case StatementType.RETURN:
                    case StatementType.THROW:
                        // these are fine.
                        break;

                    default:
                        FunctionWrapper.Errors_Throw(chunk.CaseTokens[chunk.CaseTokens.Count - 1], "This switch statement case has a fall-through.");
                        break;
                }
            }

            return switchStmnt;
        }

        private Statement SecondPass_ThrowStatement(Statement throwStmnt)
        {
            throwStmnt.expression = this.expressionResolver.ResolveExpressionSecondPass(throwStmnt.expression);
            if (Resolver.IsExpressionConstant(throwStmnt.expression))
            {
                FunctionWrapper.Errors_Throw(throwStmnt.expression.firstToken, "Only instances of Exception are throwable.");
            }
            return throwStmnt;
        }

        private Statement SecondPass_TryStatement(Statement tryStmnt)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = tryStmnt;
            this.ResolveStatementArraySecondPass(tryStmnt.code);
            foreach (CatchChunk cc in tryStmnt.catchChunks)
            {
                this.ResolveStatementArraySecondPass(cc.Code);
            }
            this.ResolveStatementArraySecondPass(tryStmnt.finallyCode);

            this.resolver.breakContext = oldBreakContext;

            return tryStmnt;
        }

        private Statement SecondPass_WhileLoop(Statement whileLoop)
        {
            Statement oldBreakContext = this.resolver.breakContext;
            this.resolver.breakContext = whileLoop;
            whileLoop.condition = this.expressionResolver.ResolveExpressionSecondPass(whileLoop.condition);
            this.ResolveStatementArraySecondPass(whileLoop.code);
            this.resolver.breakContext = oldBreakContext;
            return whileLoop;
        }
    }
}
