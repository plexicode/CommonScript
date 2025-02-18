using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class StatementParser
    {
        private TokenStream tokens;
        public ExpressionParser expressionParser;
        public EntityParser entityParser;

        public StatementParser(TokenStream tokens)
        {
            this.tokens = tokens;
        }

        public Statement[] ParseCodeBlock(bool requireCurlyBrace)
        {
            bool curlyBraceNext = requireCurlyBrace || TokenStreamUtil.Tokens_isNext(this.tokens, "{");

            List<Statement> output = new List<Statement>();
            if (curlyBraceNext)
            {
                TokenStreamUtil.Tokens_popExpected(this.tokens, "{"); // will throw when required but not present.
                while (!TokenStreamUtil.Tokens_popIfPresent(this.tokens, "}"))
                {
                    output.Add(this.ParseStatement(false));
                }
            }
            else
            {
                output.Add(this.ParseStatement(false));
            }
            return output.ToArray();
        }

        public Statement ParseStatement(bool isForLoop)
        {
            if (!isForLoop)
            {
                switch (TokenStreamUtil.Tokens_peekValueNonNull(this.tokens))
                {
                    case "break": return this.ParseBreakContinue();
                    case "continue": return this.ParseBreakContinue();
                    case "do": return this.ParseDoWhileLoop();
                    case "for": return this.ParseAnyForLoop();
                    case "if": return this.ParseIfStatement();
                    case "return": return this.ParseReturn();
                    case "switch": return this.ParseSwitch();
                    case "throw": return this.ParseThrow();
                    case "try": return this.ParseTry();
                    case "while": return this.ParseWhileLoop();
                    case "yield": throw new NotImplementedException();

                    default: break;
                }
            }

            Expression expr = this.expressionParser.ParseExpression();
            Token assignOp = this.TryPopAssignmentOp();
            Statement s;
            if (assignOp != null)
            {
                Expression assignValue = this.expressionParser.ParseExpression();
                s = Statement.createAssignment(expr, assignOp, assignValue);
            }
            else
            {
                s = Statement.createExpressionAsStatement(expr);
            }

            if (!isForLoop)
            {
                TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            }

            return s;
        }

        private Token TryPopAssignmentOp()
        {
            string op = TokenStreamUtil.Tokens_peekValue(this.tokens);
            if (op == null) return null;
            bool isOp = false;
            switch (op[0])
            {
                case '=':
                    if (op == "=") isOp = true;
                    break;
                case '+':
                    if (op == "+=") isOp = true;
                    break;
                case '-':
                    if (op == "-=") isOp = true;
                    break;
                case '*':
                    if (op == "*=") isOp = true;
                    if (op == "**=") isOp = true;
                    break;
                case '/':
                    if (op == "/=") isOp = true;
                    break;
                case '%':
                    if (op == "%=") isOp = true;
                    break;
                case '<':
                    if (op == "<<=") isOp = true;
                    break;
                case '>':
                    if (op == ">>=") isOp = true;
                    if (op == ">>>=") isOp = true;
                    break;
                case '&':
                    if (op == "&=") isOp = true;
                    break;
                case '|':
                    if (op == "|=") isOp = true;
                    break;
                case '^':
                    if (op == "^=") isOp = true;
                    break;
            }

            if (isOp) return TokenStreamUtil.Tokens_pop(this.tokens);
            return null;
        }

        private Statement ParseBreakContinue()
        {
            Token token = TokenStreamUtil.Tokens_popKeyword(this.tokens, TokenStreamUtil.Tokens_isNext(this.tokens, "continue") ? "continue" : "break");
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            return Statement.createBreakContinue(token);
        }

        private Statement ParseDoWhileLoop()
        {
            Token doToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "do");
            Statement[] code = this.ParseCodeBlock(false);
            Token whileToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "while");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            Expression condition = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");

            return Statement.createDoWhile(doToken, code, whileToken, condition);
        }

        private Statement ParseAnyForLoop()
        {
            // assert that this isn't used on a non-for loop
            if (!TokenStreamUtil.Tokens_isNext(this.tokens, "for")) TokenStreamUtil.Tokens_popKeyword(this.tokens, "for");

            Token openParen = TokenStreamUtil.Tokens_peekAhead(this.tokens, 1);
            Token varName = TokenStreamUtil.Tokens_peekAhead(this.tokens, 2);
            Token colon = TokenStreamUtil.Tokens_peekAhead(this.tokens, 3);

            if (colon != null &&
                openParen.Value == "(" &&
                varName.Type == (int) TokenType.NAME &&
                colon.Value == ":")
            {
                return this.ParseForEachLoop();
            }

            return this.ParseTraditionalForLoop();
        }

        private Statement ParseForEachLoop()
        {
            Token forToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "for");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            Token varToken = TokenStreamUtil.Tokens_popName(this.tokens, "for loop iteration variable name");
            TokenStreamUtil.Tokens_popExpected(this.tokens, ":");
            Expression listExpr = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
            Statement[] code = this.ParseCodeBlock(false);
            return Statement.createForEachLoop(forToken, varToken, listExpr, code);
        }

        private Statement ParseTraditionalForLoop()
        {
            Token forToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "for");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            List<Statement> init = new List<Statement>();
            Expression condition = null;
            List<Statement> step = new List<Statement>();
            if (!TokenStreamUtil.Tokens_isNext(this.tokens, ";"))
            {
                init.Add(this.ParseStatement(true));
                while (TokenStreamUtil.Tokens_popIfPresent(this.tokens, ","))
                {
                    init.Add(this.ParseStatement(true));
                }
            }

            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            if (!TokenStreamUtil.Tokens_isNext(this.tokens, ";"))
            {
                condition = this.expressionParser.ParseExpression();
            }
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");

            if (!TokenStreamUtil.Tokens_isNext(this.tokens, ")"))
            {
                step.Add(this.ParseStatement(true));
                while (TokenStreamUtil.Tokens_popIfPresent(this.tokens, ","))
                {
                    step.Add(this.ParseStatement(true));
                }
            }
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");

            Statement[] code = this.ParseCodeBlock(false);

            return Statement.createForLoop(forToken, init.ToArray(), condition, step.ToArray(), code);
        }

        private Statement ParseIfStatement()
        {
            Token ifToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "if");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            Expression condition = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
            Statement[] ifCode = this.ParseCodeBlock(false);
            Statement[] elseCode = [];
            if (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "else"))
            {
                elseCode = this.ParseCodeBlock(false);
            }

            return Statement.createIfStatement(ifToken, condition, ifCode, elseCode);
        }

        private Statement ParseReturn()
        {
            Token retToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "return");
            Expression expr = null;
            if (!TokenStreamUtil.Tokens_isNext(this.tokens, ";"))
            {
                expr = this.expressionParser.ParseExpression();
            }
            else
            {
                expr = Expression.createNullConstant(null);
            }
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            return Statement.createReturn(retToken, expr);
        }

        private Statement ParseSwitch()
        {
            Token switchToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "switch");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            Expression condition = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
            List<SwitchChunk> chunks = new List<SwitchChunk>();
            TokenStreamUtil.Tokens_popExpected(this.tokens, "{");
            bool defaultEncountered = false;
            while (!TokenStreamUtil.Tokens_popIfPresent(this.tokens, "}"))
            {
                TokenStreamUtil.Tokens_ensureMore(this.tokens);
                SwitchChunk activeChunk = new SwitchChunk();
                chunks.Add(activeChunk);
                while (TokenStreamUtil.Tokens_isNext(this.tokens, "case") || TokenStreamUtil.Tokens_isNext(this.tokens, "default"))
                {
                    if (defaultEncountered)
                    {
                        FunctionWrapper.Errors_Throw(TokenStreamUtil.Tokens_peek(this.tokens), "The default case for a switch statement must appear at the end.");
                    }

                    if (TokenStreamUtil.Tokens_isNext(this.tokens, "case"))
                    {
                        Token caseToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "case");
                        Expression caseValue = this.expressionParser.ParseExpression();
                        activeChunk.CaseTokens.Add(caseToken);
                        activeChunk.Cases.Add(caseValue);
                    }
                    else
                    {
                        Token defaultToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "default");
                        activeChunk.CaseTokens.Add(defaultToken);
                        activeChunk.Cases.Add(null);
                        defaultEncountered = true;
                    }
                    TokenStreamUtil.Tokens_popExpected(this.tokens, ":");
                }

                while (!TokenStreamUtil.Tokens_isNext(this.tokens, "case") &&
                    !TokenStreamUtil.Tokens_isNext(this.tokens, "default") &&
                    !TokenStreamUtil.Tokens_isNext(this.tokens, "}") &&
                    TokenStreamUtil.Tokens_hasMore(this.tokens))
                {
                    activeChunk.Code.Add(this.ParseStatement(false));
                }
            }

            return Statement.createSwitchStatement(switchToken, condition, chunks.ToArray());
        }

        private Statement ParseThrow()
        {
            Token throwToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "throw");
            Expression value = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            return Statement.createThrow(throwToken, value);
        }

        private Statement ParseTry()
        {
            Token tryToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "try");
            Statement[] tryCode = this.ParseCodeBlock(true);
            List<CatchChunk> catches = new List<CatchChunk>();
            Token finallyToken = null;
            Statement[] finallyCode = null;
            while (TokenStreamUtil.Tokens_isNext(this.tokens, "catch"))
            {
                TokenStreamUtil.Tokens_popKeyword(this.tokens, "catch");
                List<Token[]> classNamesRaw = new List<Token[]>();
                Token exceptionVarToken = null;
                TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
                Token mysteryToken = TokenStreamUtil.Tokens_popName(this.tokens, "exception name or variable");
                TokenStreamUtil.Tokens_ensureMore(this.tokens);
                if (TokenStreamUtil.Tokens_popIfPresent(this.tokens, ")"))
                {
                    // single variable catch
                    exceptionVarToken = mysteryToken;
                }
                else
                {
                    List<Token> classNameBuilder = new List<Token>() { mysteryToken };
                    while (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "."))
                    {
                        classNameBuilder.Add(TokenStreamUtil.Tokens_popName(this.tokens, "exception name"));
                    }
                    classNamesRaw.Add(classNameBuilder.ToArray());
                    while (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "|"))
                    {
                        classNameBuilder.Clear();
                        classNameBuilder.Add(TokenStreamUtil.Tokens_popName(this.tokens, "exception name"));
                        while (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "."))
                        {
                            classNameBuilder.Add(TokenStreamUtil.Tokens_popName(this.tokens, "exception name"));
                        }
                        classNamesRaw.Add(classNameBuilder.ToArray());
                    }

                    if (!TokenStreamUtil.Tokens_isNext(this.tokens, ")"))
                    {
                        exceptionVarToken = TokenStreamUtil.Tokens_popName(this.tokens, "exception variable name");
                    }
                    TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
                }
                Statement[] catchCode = this.ParseCodeBlock(true);
                catches.Add(new CatchChunk()
                {
                    Code = catchCode,
                    ExceptionNames = classNamesRaw.ToArray(),
                    exceptionVarName = exceptionVarToken,
                });
            }

            if (TokenStreamUtil.Tokens_isNext(this.tokens, "finally"))
            {
                finallyToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "finally");
                finallyCode = this.ParseCodeBlock(true);
            }
            else
            {
                finallyCode = new Statement[0];
            }

            return Statement.createTry(tryToken, tryCode, catches.ToArray(), finallyToken, finallyCode);
        }

        private Statement ParseWhileLoop()
        {
            Token whileToken = TokenStreamUtil.Tokens_popKeyword(this.tokens, "while");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            Expression condition = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ")");
            Statement[] code = this.ParseCodeBlock(false);
            return Statement.createWhileLoop(whileToken, condition, code);
        }
    }
}
