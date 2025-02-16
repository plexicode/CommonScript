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
            bool curlyBraceNext = requireCurlyBrace || this.tokens.isNext("{");

            List<Statement> output = new List<Statement>();
            if (curlyBraceNext)
            {
                this.tokens.popExpected("{"); // will throw when required but not present.
                while (!this.tokens.popIfPresent("}"))
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
                switch (this.tokens.peekValueNonNull())
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
                this.tokens.popExpected(";");
            }

            return s;
        }

        private Token TryPopAssignmentOp()
        {
            string op = this.tokens.peekValue();
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

            if (isOp) return this.tokens.pop();
            return null;
        }

        private Statement ParseBreakContinue()
        {
            Token token = this.tokens.popKeyword(this.tokens.isNext("continue") ? "continue" : "break");
            this.tokens.popExpected(";");
            return Statement.createBreakContinue(token);
        }

        private Statement ParseDoWhileLoop()
        {
            Token doToken = this.tokens.popKeyword("do");
            Statement[] code = this.ParseCodeBlock(false);
            Token whileToken = this.tokens.popKeyword("while");
            this.tokens.popExpected("(");
            Expression condition = this.expressionParser.ParseExpression();
            this.tokens.popExpected(")");
            this.tokens.popExpected(";");

            return Statement.createDoWhile(doToken, code, whileToken, condition);
        }

        private Statement ParseAnyForLoop()
        {
            // assert that this isn't used on a non-for loop
            if (!this.tokens.isNext("for")) this.tokens.popKeyword("for");

            Token openParen = this.tokens.PeekAhead(1);
            Token varName = this.tokens.PeekAhead(2);
            Token colon = this.tokens.PeekAhead(3);

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
            Token forToken = this.tokens.popKeyword("for");
            this.tokens.popExpected("(");
            Token varToken = this.tokens.popName("for loop iteration variable name");
            this.tokens.popExpected(":");
            Expression listExpr = this.expressionParser.ParseExpression();
            this.tokens.popExpected(")");
            Statement[] code = this.ParseCodeBlock(false);
            return Statement.createForEachLoop(forToken, varToken, listExpr, code);
        }

        private Statement ParseTraditionalForLoop()
        {
            Token forToken = this.tokens.popKeyword("for");
            this.tokens.popExpected("(");
            List<Statement> init = new List<Statement>();
            Expression condition = null;
            List<Statement> step = new List<Statement>();
            if (!this.tokens.isNext(";"))
            {
                init.Add(this.ParseStatement(true));
                while (this.tokens.popIfPresent(","))
                {
                    init.Add(this.ParseStatement(true));
                }
            }

            this.tokens.popExpected(";");
            if (!this.tokens.isNext(";"))
            {
                condition = this.expressionParser.ParseExpression();
            }
            this.tokens.popExpected(";");

            if (!this.tokens.isNext(")"))
            {
                step.Add(this.ParseStatement(true));
                while (this.tokens.popIfPresent(","))
                {
                    step.Add(this.ParseStatement(true));
                }
            }
            this.tokens.popExpected(")");

            Statement[] code = this.ParseCodeBlock(false);

            return Statement.createForLoop(forToken, init.ToArray(), condition, step.ToArray(), code);
        }

        private Statement ParseIfStatement()
        {
            Token ifToken = this.tokens.popKeyword("if");
            this.tokens.popExpected("(");
            Expression condition = this.expressionParser.ParseExpression();
            this.tokens.popExpected(")");
            Statement[] ifCode = this.ParseCodeBlock(false);
            Statement[] elseCode = [];
            if (this.tokens.popIfPresent("else"))
            {
                elseCode = this.ParseCodeBlock(false);
            }

            return Statement.createIfStatement(ifToken, condition, ifCode, elseCode);
        }

        private Statement ParseReturn()
        {
            Token retToken = this.tokens.popKeyword("return");
            Expression expr = null;
            if (!this.tokens.isNext(";"))
            {
                expr = this.expressionParser.ParseExpression();
            }
            else
            {
                expr = Expression.createNullConstant(null);
            }
            this.tokens.popExpected(";");
            return Statement.createReturn(retToken, expr);
        }

        private Statement ParseSwitch()
        {
            Token switchToken = this.tokens.popKeyword("switch");
            this.tokens.popExpected("(");
            Expression condition = this.expressionParser.ParseExpression();
            this.tokens.popExpected(")");
            List<SwitchChunk> chunks = new List<SwitchChunk>();
            this.tokens.popExpected("{");
            bool defaultEncountered = false;
            while (!this.tokens.popIfPresent("}"))
            {
                this.tokens.ensureMore();
                SwitchChunk activeChunk = new SwitchChunk();
                chunks.Add(activeChunk);
                while (this.tokens.isNext("case") || this.tokens.isNext("default"))
                {
                    if (defaultEncountered)
                    {
                        Errors.ThrowError(this.tokens.peek(), "The default case for a switch statement must appear at the end.");
                    }

                    if (this.tokens.isNext("case"))
                    {
                        Token caseToken = this.tokens.popKeyword("case");
                        Expression caseValue = this.expressionParser.ParseExpression();
                        activeChunk.CaseTokens.Add(caseToken);
                        activeChunk.Cases.Add(caseValue);
                    }
                    else
                    {
                        Token defaultToken = this.tokens.popKeyword("default");
                        activeChunk.CaseTokens.Add(defaultToken);
                        activeChunk.Cases.Add(null);
                        defaultEncountered = true;
                    }
                    this.tokens.popExpected(":");
                }

                while (!this.tokens.isNext("case") &&
                    !this.tokens.isNext("default") &&
                    !this.tokens.isNext("}") &&
                    this.tokens.hasMore())
                {
                    activeChunk.Code.Add(this.ParseStatement(false));
                }
            }

            return Statement.createSwitchStatement(switchToken, condition, chunks.ToArray());
        }

        private Statement ParseThrow()
        {
            Token throwToken = this.tokens.popKeyword("throw");
            Expression value = this.expressionParser.ParseExpression();
            this.tokens.popExpected(";");
            return Statement.createThrow(throwToken, value);
        }

        private Statement ParseTry()
        {
            Token tryToken = this.tokens.popKeyword("try");
            Statement[] tryCode = this.ParseCodeBlock(true);
            List<CatchChunk> catches = new List<CatchChunk>();
            Token finallyToken = null;
            Statement[] finallyCode = null;
            while (this.tokens.isNext("catch"))
            {
                Token catchToken = this.tokens.popKeyword("catch");
                List<Token[]> classNamesRaw = new List<Token[]>();
                Token exceptionVarToken = null;
                this.tokens.popExpected("(");
                Token mysteryToken = this.tokens.popName("exception name or variable");
                this.tokens.ensureMore();
                if (this.tokens.popIfPresent(")"))
                {
                    // single variable catch
                    exceptionVarToken = mysteryToken;
                }
                else
                {
                    List<Token> classNameBuilder = new List<Token>() { mysteryToken };
                    while (this.tokens.popIfPresent("."))
                    {
                        classNameBuilder.Add(this.tokens.popName("exception name"));
                    }
                    classNamesRaw.Add(classNameBuilder.ToArray());
                    while (this.tokens.popIfPresent("|"))
                    {
                        classNameBuilder.Clear();
                        classNameBuilder.Add(this.tokens.popName("exception name"));
                        while (this.tokens.popIfPresent("."))
                        {
                            classNameBuilder.Add(this.tokens.popName("exception name"));
                        }
                        classNamesRaw.Add(classNameBuilder.ToArray());
                    }

                    if (!this.tokens.isNext(")"))
                    {
                        exceptionVarToken = this.tokens.popName("exception variable name");
                    }
                    this.tokens.popExpected(")");
                }
                Statement[] catchCode = this.ParseCodeBlock(true);
                catches.Add(new CatchChunk()
                {
                    CatchToken = catchToken,
                    Code = catchCode,
                    ExceptionNames = classNamesRaw.ToArray(),
                    exceptionVarName = exceptionVarToken,
                });
            }

            if (this.tokens.isNext("finally"))
            {
                finallyToken = this.tokens.popKeyword("finally");
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
            Token whileToken = this.tokens.popKeyword("while");
            this.tokens.popExpected("(");
            Expression condition = this.expressionParser.ParseExpression();
            this.tokens.popExpected(")");
            Statement[] code = this.ParseCodeBlock(false);
            return Statement.createWhileLoop(whileToken, condition, code);
        }
    }
}
