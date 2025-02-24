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
                case (int) StatementType.ASSIGNMENT: return FunctionWrapper.StatementResolver_FirstPass_Assignment(resolver, s);
                case (int) StatementType.BREAK: return FunctionWrapper.StatementResolver_FirstPass_Break(resolver, s);
                case (int) StatementType.CONTINUE: return FunctionWrapper.StatementResolver_FirstPass_Continue(resolver, s);
                case (int) StatementType.DO_WHILE_LOOP: return FunctionWrapper.StatementResolver_FirstPass_DoWhileLoop(resolver, s);
                case (int) StatementType.FOR_EACH_LOOP: return FunctionWrapper.StatementResolver_FirstPass_ForEachLoop(resolver, s);
                case (int) StatementType.FOR_LOOP: return FunctionWrapper.StatementResolver_FirstPass_ForLoop(resolver, s);
                case (int) StatementType.IF_STATEMENT: return FunctionWrapper.StatementResolver_FirstPass_IfStatement(resolver, s);
                case (int) StatementType.SWITCH_STATEMENT: return FunctionWrapper.StatementResolver_FirstPass_SwitchStatement(resolver, s);
                case (int) StatementType.TRY: return FunctionWrapper.StatementResolver_FirstPass_Try(resolver, s);
                case (int) StatementType.WHILE_LOOP: return FunctionWrapper.StatementResolver_FirstPass_WhileLoop(resolver, s);

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

    }
}
