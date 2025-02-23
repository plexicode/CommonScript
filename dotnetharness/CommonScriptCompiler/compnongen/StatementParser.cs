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
                    output.Add(ParseStatement(tokens, false));
                }
            }
            else
            {
                output.Add(ParseStatement(tokens, false));
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
                    
                    case (int) StatementKeyword.DO: return ParseDoWhileLoop(tokens);
                    case (int) StatementKeyword.FOR: return ParseAnyForLoop(tokens);
                    case (int) StatementKeyword.IF: return ParseIfStatement(tokens);
                    case (int) StatementKeyword.RETURN: return ParseReturn(tokens);
                    case (int) StatementKeyword.SWITCH: return ParseSwitch(tokens);
                    case (int) StatementKeyword.THROW: return ParseThrow(tokens);
                    case (int) StatementKeyword.TRY: return ParseTry(tokens);
                    case (int) StatementKeyword.WHILE: return ParseWhileLoop(tokens);
                    case (int) StatementKeyword.YIELD:
                        FunctionWrapper.fail("Not implemented");
                        return null;
                    
                    default:
                        FunctionWrapper.fail(""); // should not happen.
                        break;
                }
            }

            Expression expr = ExpressionParser.ParseExpression(tokens);
            Token assignOp = FunctionWrapper.TryPopAssignmentOp(tokens);
            Statement s;
            if (assignOp != null)
            {
                Expression assignValue = ExpressionParser.ParseExpression(tokens);
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

        private static Statement ParseDoWhileLoop(TokenStream tokens)
        {
            Token doToken = FunctionWrapper.Tokens_popKeyword(tokens, "do");
            Statement[] code = ParseCodeBlock(tokens, false);
            Token whileToken = FunctionWrapper.Tokens_popKeyword(tokens, "while");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            Expression condition = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ")");
            FunctionWrapper.Tokens_popExpected(tokens, ";");

            return FunctionWrapper.Statement_createDoWhile(doToken, code, whileToken, condition);
        }

        private static Statement ParseAnyForLoop(TokenStream tokens)
        {
            // assert that this isn't used on a non-for loop
            if (!FunctionWrapper.Tokens_isNext(tokens, "for")) FunctionWrapper.Tokens_popKeyword(tokens, "for");

            Token openParen = FunctionWrapper.Tokens_peekAhead(tokens, 1);
            Token varName = FunctionWrapper.Tokens_peekAhead(tokens, 2);
            Token colon = FunctionWrapper.Tokens_peekAhead(tokens, 3);

            if (colon != null &&
                openParen.Value == "(" &&
                varName.Type == (int) TokenType.NAME &&
                colon.Value == ":")
            {
                return ParseForEachLoop(tokens);
            }

            return ParseTraditionalForLoop(tokens);
        }

        private static Statement ParseForEachLoop(TokenStream tokens)
        {
            Token forToken = FunctionWrapper.Tokens_popKeyword(tokens, "for");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            Token varToken = FunctionWrapper.Tokens_popName(tokens, "for loop iteration variable name");
            FunctionWrapper.Tokens_popExpected(tokens, ":");
            Expression listExpr = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ")");
            Statement[] code = ParseCodeBlock(tokens, false);
            return FunctionWrapper.Statement_createForEachLoop(forToken, varToken, listExpr, code);
        }

        private static Statement ParseTraditionalForLoop(TokenStream tokens)
        {
            Token forToken = FunctionWrapper.Tokens_popKeyword(tokens, "for");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            List<Statement> init = new List<Statement>();
            Expression condition = null;
            List<Statement> step = new List<Statement>();
            if (!FunctionWrapper.Tokens_isNext(tokens, ";"))
            {
                init.Add(ParseStatement(tokens, true));
                while (FunctionWrapper.Tokens_popIfPresent(tokens, ","))
                {
                    init.Add(ParseStatement(tokens, true));
                }
            }

            FunctionWrapper.Tokens_popExpected(tokens, ";");
            if (!FunctionWrapper.Tokens_isNext(tokens, ";"))
            {
                condition = ExpressionParser.ParseExpression(tokens);
            }
            FunctionWrapper.Tokens_popExpected(tokens, ";");

            if (!FunctionWrapper.Tokens_isNext(tokens, ")"))
            {
                step.Add(ParseStatement(tokens, true));
                while (FunctionWrapper.Tokens_popIfPresent(tokens, ","))
                {
                    step.Add(ParseStatement(tokens, true));
                }
            }
            FunctionWrapper.Tokens_popExpected(tokens, ")");

            Statement[] code = ParseCodeBlock(tokens, false);

            return FunctionWrapper.Statement_createForLoop(forToken, init.ToArray(), condition, step.ToArray(), code);
        }

        private static Statement ParseIfStatement(TokenStream tokens)
        {
            Token ifToken = FunctionWrapper.Tokens_popKeyword(tokens, "if");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            Expression condition = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ")");
            Statement[] ifCode = ParseCodeBlock(tokens, false);
            Statement[] elseCode = [];
            if (FunctionWrapper.Tokens_popIfPresent(tokens, "else"))
            {
                elseCode = ParseCodeBlock(tokens, false);
            }

            return FunctionWrapper.Statement_createIfStatement(ifToken, condition, ifCode, elseCode);
        }

        private static Statement ParseReturn(TokenStream tokens)
        {
            Token retToken = FunctionWrapper.Tokens_popKeyword(tokens, "return");
            Expression expr = null;
            if (!FunctionWrapper.Tokens_isNext(tokens, ";"))
            {
                expr = ExpressionParser.ParseExpression(tokens);
            }
            else
            {
                expr = FunctionWrapper.Expression_createNullConstant(null);
            }
            FunctionWrapper.Tokens_popExpected(tokens, ";");
            return FunctionWrapper.Statement_createReturn(retToken, expr);
        }

        private static Statement ParseSwitch(TokenStream tokens)
        {
            Token switchToken = FunctionWrapper.Tokens_popKeyword(tokens, "switch");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            Expression condition = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ")");
            List<SwitchChunk> chunks = new List<SwitchChunk>();
            FunctionWrapper.Tokens_popExpected(tokens, "{");
            bool defaultEncountered = false;
            while (!FunctionWrapper.Tokens_popIfPresent(tokens, "}"))
            {
                FunctionWrapper.Tokens_ensureMore(tokens);
                SwitchChunk activeChunk = FunctionWrapper.SwitchChunk_new();
                chunks.Add(activeChunk);
                while (FunctionWrapper.Tokens_isNext(tokens, "case") || FunctionWrapper.Tokens_isNext(tokens, "default"))
                {
                    if (defaultEncountered)
                    {
                        FunctionWrapper.Errors_Throw(FunctionWrapper.Tokens_peek(tokens), "The default case for a switch statement must appear at the end.");
                    }

                    if (FunctionWrapper.Tokens_isNext(tokens, "case"))
                    {
                        Token caseToken = FunctionWrapper.Tokens_popKeyword(tokens, "case");
                        Expression caseValue = ExpressionParser.ParseExpression(tokens);
                        activeChunk.CaseTokens.Add(caseToken);
                        activeChunk.Cases.Add(caseValue);
                    }
                    else
                    {
                        Token defaultToken = FunctionWrapper.Tokens_popKeyword(tokens, "default");
                        activeChunk.CaseTokens.Add(defaultToken);
                        activeChunk.Cases.Add(null);
                        defaultEncountered = true;
                    }
                    FunctionWrapper.Tokens_popExpected(tokens, ":");
                }

                while (!FunctionWrapper.Tokens_isNext(tokens, "case") &&
                    !FunctionWrapper.Tokens_isNext(tokens, "default") &&
                    !FunctionWrapper.Tokens_isNext(tokens, "}") &&
                    FunctionWrapper.Tokens_hasMore(tokens))
                {
                    activeChunk.Code.Add(ParseStatement(tokens, false));
                }
            }

            return FunctionWrapper.Statement_createSwitchStatement(switchToken, condition, chunks.ToArray());
        }

        private static Statement ParseThrow(TokenStream tokens)
        {
            Token throwToken = FunctionWrapper.Tokens_popKeyword(tokens, "throw");
            Expression value = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ";");
            return FunctionWrapper.Statement_createThrow(throwToken, value);
        }

        private static Statement ParseTry(TokenStream tokens)
        {
            Token tryToken = FunctionWrapper.Tokens_popKeyword(tokens, "try");
            Statement[] tryCode = ParseCodeBlock(tokens, true);
            List<CatchChunk> catches = new List<CatchChunk>();
            Token finallyToken = null;
            Statement[] finallyCode = null;
            while (FunctionWrapper.Tokens_isNext(tokens, "catch"))
            {
                FunctionWrapper.Tokens_popKeyword(tokens, "catch");
                List<Token[]> classNamesRaw = new List<Token[]>();
                Token exceptionVarToken = null;
                FunctionWrapper.Tokens_popExpected(tokens, "(");
                Token mysteryToken = FunctionWrapper.Tokens_popName(tokens, "exception name or variable");
                FunctionWrapper.Tokens_ensureMore(tokens);
                if (FunctionWrapper.Tokens_popIfPresent(tokens, ")"))
                {
                    // single variable catch
                    exceptionVarToken = mysteryToken;
                }
                else
                {
                    List<Token> classNameBuilder = new List<Token>() { mysteryToken };
                    while (FunctionWrapper.Tokens_popIfPresent(tokens, "."))
                    {
                        classNameBuilder.Add(FunctionWrapper.Tokens_popName(tokens, "exception name"));
                    }
                    classNamesRaw.Add(classNameBuilder.ToArray());
                    while (FunctionWrapper.Tokens_popIfPresent(tokens, "|"))
                    {
                        classNameBuilder.Clear();
                        classNameBuilder.Add(FunctionWrapper.Tokens_popName(tokens, "exception name"));
                        while (FunctionWrapper.Tokens_popIfPresent(tokens, "."))
                        {
                            classNameBuilder.Add(FunctionWrapper.Tokens_popName(tokens, "exception name"));
                        }
                        classNamesRaw.Add(classNameBuilder.ToArray());
                    }

                    if (!FunctionWrapper.Tokens_isNext(tokens, ")"))
                    {
                        exceptionVarToken = FunctionWrapper.Tokens_popName(tokens, "exception variable name");
                    }
                    FunctionWrapper.Tokens_popExpected(tokens, ")");
                }
                Statement[] catchCode = ParseCodeBlock(tokens, true);
                catches.Add(FunctionWrapper.CatchChunk_new(
                    catchCode,
                    classNamesRaw,
                    exceptionVarToken));
            }

            if (FunctionWrapper.Tokens_isNext(tokens, "finally"))
            {
                finallyToken = FunctionWrapper.Tokens_popKeyword(tokens, "finally");
                finallyCode = ParseCodeBlock(tokens, true);
            }
            else
            {
                finallyCode = new Statement[0];
            }

            return FunctionWrapper.Statement_createTry(tryToken, tryCode, catches.ToArray(), finallyToken, finallyCode);
        }

        private static Statement ParseWhileLoop(TokenStream tokens)
        {
            Token whileToken = FunctionWrapper.Tokens_popKeyword(tokens, "while");
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            Expression condition = ExpressionParser.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ")");
            Statement[] code = ParseCodeBlock(tokens, false);
            return FunctionWrapper.Statement_createWhileLoop(whileToken, condition, code);
        }
    }
}
