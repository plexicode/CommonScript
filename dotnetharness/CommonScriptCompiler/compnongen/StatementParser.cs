using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class StatementParser
    {
        public static Statement[] ParseCodeBlock(TokenStream tokens, bool requireCurlyBrace)
        {
            return ParseCodeBlockList(tokens, requireCurlyBrace).ToArray();
        }
        public static List<Statement> ParseCodeBlockList(TokenStream tokens, bool requireCurlyBrace)
        {
            bool curlyBraceNext = requireCurlyBrace || FunctionWrapper.Tokens_isNext(tokens, "{");

            List<Statement> output = new List<Statement>();
            if (curlyBraceNext)
            {
                FunctionWrapper.Tokens_popExpected(tokens, "{"); // will throw when required but not present.
                while (!FunctionWrapper.Tokens_popIfPresent(tokens, "}"))
                {
                    output.Add(tokens.parseStatement(tokens, false));
                }
            }
            else
            {
                output.Add(tokens.parseStatement(tokens, false));
            }
            return output;
        }

        private enum StatementKeyword
        {
            UNKNOWN = 0,
            BREAK = 1,
            CONTINUE = 2,
            DO = 3,
            FOR = 4, 
            IF = 5,
            RETURN = 6, 
            SWITCH = 7,
            THROW = 8,
            TRY = 9, 
            WHILE = 10,
            YIELD = 11, 
        }

        public static Statement ParseStatement(TokenStream tokens, bool isForLoop)
        {
            Token nextToken = FunctionWrapper.Tokens_peek(tokens);
            if (!isForLoop && 
                nextToken != null && 
                nextToken.Type == (int) TokenType.KEYWORD)
            {
                switch (FunctionWrapper.StatementParser_IdentifyKeywordType(nextToken.Value))
                {
                    case (int) StatementKeyword.BREAK:
                    case (int) StatementKeyword.CONTINUE: 
                        return FunctionWrapper.ParseBreakContinue(tokens);
                    
                    case (int) StatementKeyword.DO: return FunctionWrapper.ParseDoWhileLoop(tokens);
                    case (int) StatementKeyword.FOR: return FunctionWrapper.ParseAnyForLoop(tokens);
                    case (int) StatementKeyword.IF: return FunctionWrapper.ParseIfStatement(tokens);
                    case (int) StatementKeyword.RETURN: return FunctionWrapper.ParseReturn(tokens);
                    case (int) StatementKeyword.SWITCH: return FunctionWrapper.ParseSwitch(tokens);
                    case (int) StatementKeyword.THROW: return FunctionWrapper.ParseThrow(tokens);
                    case (int) StatementKeyword.TRY: return FunctionWrapper.ParseTry(tokens);
                    case (int) StatementKeyword.WHILE: return FunctionWrapper.ParseWhileLoop(tokens);
                    case (int) StatementKeyword.YIELD:
                        FunctionWrapper.fail("Not implemented");
                        return null;
                    
                    default:
                        FunctionWrapper.fail(""); // should not happen.
                        break;
                }
            }

            Expression expr = FunctionWrapper.ParseExpression(tokens);
            Token assignOp = FunctionWrapper.TryPopAssignmentOp(tokens);
            Statement s;
            if (assignOp != null)
            {
                Expression assignValue = FunctionWrapper.ParseExpression(tokens);
                s = FunctionWrapper.Statement_createAssignment(expr, assignOp, assignValue);
            }
            else
            {
                s = FunctionWrapper.Statement_createExpressionAsStatement(expr);
            }

            if (!isForLoop)
            {
                FunctionWrapper.Tokens_popExpected(tokens, ";");
            }

            return s;
        }
    }
}
