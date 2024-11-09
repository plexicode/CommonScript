using System.Collections.Generic;

namespace CommonScript.Compiler
{
    // ternary -> null coalesce -> boolean combine -> bitwise ->
    // equality -> inequality -> bitshift ->
    // addition -> multiplication -> unary prefix -> unary suffix ->
    // atomic

    internal class ExpressionParser
    {
        private TokenStream tokens;
        public StatementParser statementParser;
        public EntityParser entityParser;


        public ExpressionParser(TokenStream tokens)
        {
            this.tokens = tokens;
        }
        public Expression ParseExpression()
        {
            return ParseTernary();
        }

        private Expression ParseTernary()
        {
            Expression root = this.ParseNullCoalesce();
            if (this.tokens.isNext("?"))
            {
                Token qmark = this.tokens.pop();
                Expression trueValue = this.ParseTernary();
                this.tokens.popExpected(":");
                Expression falseValue = this.ParseTernary();
                root = Expression.createTernary(root, qmark, trueValue, falseValue);
            }
            return root;
        }

        private Expression ParseNullCoalesce()
        {
            Expression root = this.ParseBooleanCombination();
            if (this.tokens.isNext("??"))
            {
                Token op = this.tokens.pop();
                Expression next = this.ParseNullCoalesce();
                root = Expression.createBinaryOp(root, op, next);
            }
            return root;
        }

        private Expression ParseBooleanCombination()
        {
            Expression root = this.ParseBitwise();
            if (this.tokens.doesNextInclulde2("||", "&&"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (this.tokens.doesNextInclulde2("||", "&&"))
                {
                    ops.Add(this.tokens.pop());
                    expressions.Add(this.ParseBitwise());
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseBitwise()
        {
            Expression root = this.ParseEquality();
            if (this.tokens.doesNextInclulde3("&", "|", "^"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();

                while (this.tokens.doesNextInclulde3("&", "|", "^"))
                {
                    ops.Add(this.tokens.pop());
                    expressions.Add(this.ParseEquality());
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseEquality()
        {
            Expression root = this.ParseInequality();
            if (this.tokens.doesNextInclulde2("==", "!="))
            {
                Token op = this.tokens.pop();
                Expression right = this.ParseInequality();
                return Expression.createBinaryOp(root, op, right);
            }
            return root;
        }

        private Expression ParseInequality()
        {
            Expression root = this.ParseBitshift();
            if (this.tokens.doesNextInclulde4("<", ">", "<=", ">="))
            {
                Token op = this.tokens.pop();
                Expression right = this.ParseBitshift();
                root = Expression.createBinaryOp(root, op, right);
            }
            return root;
        }

        private Expression ParseBitshift()
        {
            Expression root = this.ParseAddition();
            if (this.tokens.doesNextInclulde3("<<", ">>", ">>>"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (this.tokens.doesNextInclulde3("<<", ">>", ">>>"))
                {
                    ops.Add(this.tokens.pop());
                    expressions.Add(this.ParseAddition());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseAddition()
        {
            Expression root = this.ParseMultiplication();
            if (this.tokens.doesNextInclulde2("+", "-"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (this.tokens.doesNextInclulde2("+", "-"))
                {
                    ops.Add(this.tokens.pop());
                    expressions.Add(this.ParseMultiplication());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseMultiplication()
        {
            Expression root = this.ParseUnaryPrefix();
            if (this.tokens.doesNextInclulde3("*", "/", "%"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (this.tokens.doesNextInclulde3("*", "/", "%"))
                {
                    ops.Add(this.tokens.pop());
                    expressions.Add(this.ParseUnaryPrefix());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseUnaryPrefix()
        {
            string next = this.tokens.peekValue();
            if (next == null) this.tokens.ensureMore();
            switch (next[0])
            {
                case '-':
                    if (next == "--") return this.ParseInlineIncrementPrefix();
                    if (next == "-") return this.ParseNegatePrefix();
                    break;
                case '+':
                    if (next == "++") return this.ParseInlineIncrementPrefix();
                    break;
                case '!':
                    if (next == "!") return this.ParseNegatePrefix();
                    break;
                case '~':
                    if (next == "~") return this.ParseNegatePrefix();
                    break;
            }

            return this.ParseUnarySuffix();
        }

        private Expression ParseNegatePrefix()
        {
            if (!this.tokens.doesNextInclulde3("-", "!", "~"))
            {
                this.tokens.popExpected("-"); // assert
            }
            Token op = this.tokens.pop();
            Expression root = this.ParseUnaryPrefix();
            return Expression.createNegatePrefix(op, root);
        }

        private Expression ParseUnarySuffix()
        {
            Expression root = this.ParseAtomicExpression();
            string next = this.tokens.peekValue();
            bool checkForSuffixes = true;
            while (checkForSuffixes && next != null)
            {
                checkForSuffixes = false;
                switch (next)
                {
                    case ".":
                        Token dotToken = this.tokens.pop();
                        Token nameToken = this.tokens.popName("field name");
                        root = Expression.createDotField(root, dotToken, nameToken.Value);
                        checkForSuffixes = true;
                        break;

                    case "(":
                        Token openParen = this.tokens.pop();
                        List<Expression> args = new List<Expression>();
                        while (!this.tokens.popIfPresent(")"))
                        {
                            if (args.Count > 0) this.tokens.popExpected(",");
                            args.Add(this.ParseExpression());
                        }
                        root = Expression.createFunctionInvocation(root, openParen, args.ToArray());
                        checkForSuffixes = true;
                        break;

                    case "[":

                        Token openBracket = this.tokens.pop();
                        this.tokens.ensureMore();
                        Token throwTokenOnInvalidSlice = this.tokens.peek();
                        List<Expression> sliceNums = new List<Expression>();
                        bool nextExpected = true;

                        while (nextExpected && this.tokens.hasMore())
                        {
                            if (this.tokens.popIfPresent(":"))
                            {
                                sliceNums.Add(null);
                            }
                            else if (this.tokens.isNext("]"))
                            {
                                sliceNums.Add(null);
                                nextExpected = false;
                            }
                            else
                            {
                                sliceNums.Add(this.ParseExpression());
                                if (!this.tokens.popIfPresent(":"))
                                {
                                    nextExpected = false;
                                }
                            }
                        }

                        this.tokens.popExpected("]");

                        if (sliceNums.Count < 0 || sliceNums.Count > 3)
                        {
                            Errors.ThrowError(throwTokenOnInvalidSlice, "Invalid index or slice expression");
                        }

                        if (sliceNums.Count == 1)
                        {
                            if (sliceNums[0] == null)
                            {
                                Errors.ThrowError(throwTokenOnInvalidSlice, "Expected index expression.");
                            }

                            root = Expression.createBracketIndex(root, openBracket, sliceNums[0]);
                        }
                        else
                        {
                            root = Expression.createSliceExpression(root, openBracket, sliceNums[0], sliceNums[1], sliceNums.Count == 3 ? sliceNums[2] : null);
                        }

                        checkForSuffixes = true;
                        break;

                    case "++":
                    case "--":
                        Token ppToken = this.tokens.pop();
                        root = Expression.createInlineIncrement(root.firstToken, root, ppToken, false);
                        break;

                    default:
                        break;
                }
                next = this.tokens.peekValue();
            }
            return root;
        }

        private Expression ParseInlineIncrementPrefix()
        {
            if (!this.tokens.doesNextInclulde2("++", "--"))
            {
                // more of an assert
                this.tokens.popExpected("++");
            }
            Token op = this.tokens.pop();
            Expression root = this.ParseUnarySuffix();
            return Expression.createInlineIncrement(op, root, op, true);
        }

        private static Expression FlattenBinaryOpChain(List<Expression> expressions, List<Token> ops)
        {
            string opType = ops[0].Value;
            bool isShortCircuit = opType == "??" || opType == "&&" || opType == "||";
            Expression acc;
            int length = expressions.Count;
            if (isShortCircuit)
            {
                acc = Expression.createBinaryOp(expressions[length - 2], ops[length - 2], expressions[length - 1]);
                for (int i = length - 3; i >= 0; i--)
                {
                    acc = Expression.createBinaryOp(expressions[i], ops[i], acc);
                }
            }
            else
            {
                acc = Expression.createBinaryOp(expressions[0], ops[0], expressions[1]);

                for (int i = 2; i < length; i++)
                {
                    acc = Expression.createBinaryOp(acc, ops[i - 1], expressions[i]);
                }
            }

            return acc;
        }

        private Expression ParseListDefinition()
        {
            Token openListToken = this.tokens.popExpected("[");
            List<Expression> items = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !this.tokens.isNext("]"))
            {
                items.Add(this.ParseExpression());
                nextAllowed = this.tokens.popIfPresent(",");
            }
            this.tokens.popExpected("]");
            return Expression.createListDefinition(openListToken, items.ToArray());
        }

        private Expression ParseDictionaryDefinition()
        {
            Token openDictionaryToken = this.tokens.popExpected("{");
            List<Expression> keys = new List<Expression>();
            List<Expression> values = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !this.tokens.isNext("}"))
            {
                keys.Add(this.ParseExpression());
                this.tokens.popExpected(":");
                values.Add(this.ParseExpression());
                nextAllowed = this.tokens.popIfPresent(",");
            }
            this.tokens.popExpected("}");
            return Expression.createDictionaryDefinition(openDictionaryToken, keys.ToArray(), values.ToArray());
        }

        private Expression ParseAtomicExpression()
        {
            Token nextToken = this.tokens.peek();
            if (nextToken == null) this.tokens.ensureMore();
            string next = nextToken.Value;
            switch (nextToken.Type)
            {
                case TokenType.PUNCTUATION:

                    if (next == "(")
                    {
                        // () =>
                        if (this.tokens.isSequenceNext3("(", ")", "=>")) return this.ParseLambda();

                        // (a, ...
                        if (this.tokens.isSequenceNext3("(", null, ",") &&
                            this.tokens.PeekAhead(1).Type == TokenType.NAME) return this.ParseLambda();

                        // (a = ...
                        if (this.tokens.isSequenceNext3("(", null, "=") &&
                            this.tokens.PeekAhead(1).Type == TokenType.NAME) return this.ParseLambda();

                        // (a) => 
                        if (this.tokens.isSequenceNext4("(", null, ")", "=>")) return this.ParseLambda();

                        this.tokens.pop();
                        Expression expr = this.ParseExpression();
                        this.tokens.popExpected(")");
                        return expr;

                    }
                    if (next == "[") return this.ParseListDefinition();
                    if (next == "{") return this.ParseDictionaryDefinition();
                    if (next == "$")
                    {
                        Token builtinPrefix = this.tokens.pop();
                        Token builtinName = this.tokens.popName("built-in function name");
                        return Expression.createExtensionReference(builtinPrefix, builtinName.Value);
                    }
                    break;

                case TokenType.KEYWORD:
                    if (next == "true" || next == "false")
                    {
                        Token boolTok = this.tokens.pop();
                        return Expression.createBoolConstant(boolTok, next == "true");
                    }

                    if (next == "null")
                    {
                        return Expression.createNullConstant(this.tokens.pop());
                    }

                    if (next == "new")
                    {
                        Token newTok = this.tokens.pop();
                        List<Token> nameChain = new List<Token>() { this.tokens.popName("class name") };
                        while (this.tokens.isNext("."))
                        {
                            nameChain.Add(this.tokens.pop());
                            nameChain.Add(this.tokens.popName("class name"));
                        }
                        Expression ctorChain = Expression.createVariable(nameChain[0], nameChain[0].Value);
                        for (int i = 1; i < nameChain.Count; i += 2)
                        {
                            ctorChain = Expression.createDotField(ctorChain, nameChain[i], nameChain[i + 1].Value);
                        }
                        return Expression.createConstructorReference(newTok, ctorChain);
                    }

                    if (next == "this")
                    {
                        return Expression.createThisReference(this.tokens.pop());
                    }

                    if (next == "base")
                    {
                        return Expression.createBaseReference(this.tokens.pop());
                    }

                    break;

                case TokenType.INTEGER:
                    int intVal = ExpressionParser.TryParseInteger(nextToken, next, false);
                    return Expression.createIntegerConstant(this.tokens.pop(), intVal);

                case TokenType.FLOAT:
                    double floatVal = ExpressionParser.TryParseFloat(nextToken, next);
                    return Expression.createFloatConstant(this.tokens.pop(), floatVal);

                case TokenType.HEX_INTEGER:
                    int intValHex = ExpressionParser.TryParseInteger(nextToken, next, true);
                    return Expression.createIntegerConstant(this.tokens.pop(), intValHex);

                case TokenType.STRING:
                    string strVal = ExpressionParser.TryParseString(nextToken, next);
                    return Expression.createStringConstant(this.tokens.pop(), strVal);

                case TokenType.NAME:
                    if (this.tokens.isSequenceNext2(null, "=>")) return this.ParseLambda();

                    Token varName = this.tokens.popName("variable name");
                    return Expression.createVariable(varName, varName.Value);
            }

            Errors.ThrowError(nextToken, "Expected an expression but found '" + next + "' instead.");
            return null;
        }

        private Expression ParseLambda()
        {
            Token firstToken = this.tokens.peek();
            List<Token> argTokens = new List<Token>();
            List<Expression> argDefaultValues = new List<Expression>();
            if (this.tokens.popIfPresent("("))
            {
                while (!this.tokens.popIfPresent(")"))
                {
                    if (argTokens.Count > 0) this.tokens.popExpected(",");
                    argTokens.Add(this.tokens.popName("argument name"));
                    Expression defaultVal = null;
                    if (this.tokens.popIfPresent("="))
                    {
                        defaultVal = this.ParseExpression();
                    }
                    argDefaultValues.Add(defaultVal);
                }
            }
            else
            {
                argTokens.Add(this.tokens.popName("argument name"));
                argDefaultValues.Add(null);
            }

            Token arrow = this.tokens.popExpected("=>");

            Statement[] code;
            if (this.tokens.isNext("{"))
            {
                code = this.statementParser.ParseCodeBlock(true);
            }
            else
            {
                Expression codeExpr = this.ParseExpression();
                code = new Statement[] {
                    Statement.createReturn(arrow, codeExpr)
                };
            }
            return Expression.createLambda(firstToken, argTokens.ToArray(), argDefaultValues.ToArray(), arrow, code);
        }

        private static string TryParseString(Token throwToken, string rawValue)
        {
            List<string> output = new List<string>();

            int length = rawValue.Length - 1;
            string c = "";
            for (int i = 1; i < length; i++)
            {
                c = rawValue.Substring(i, 1);
                if (c == "\\")
                {
                    i++;

                    // More of an assert. The tokenizer should outright prevent this from happening ever.
                    if (i == length) Errors.ThrowError(throwToken, "Invalid backslash in string constant.");

                    c = rawValue.Substring(i, 1);
                    if (c == "n") c = "\n";
                    else if (c == "r") c = "\r";
                    else if (c == "'" || c == "\"" || c == "\\") { }
                    else if (c == "t") c = "\t";
                    else
                    {
                        Errors.ThrowError(throwToken, "Unrecognized string escape sequence: '\\" + c + "'");
                    }
                }
                output.Add(c);
            }

            return string.Join("", output);
        }

        private static double TryParseFloat(Token throwToken, string rawValue)
        {
            double output;
            if (!double.TryParse(rawValue, out output))
            {
                Errors.ThrowError(throwToken, "Invalid float constant");
                return 0;
            }
            return output;
        }

        private static int TryParseInteger(Token throwToken, string rawValue, bool isHex)
        {
            // TODO: this should actually be a long or big int to preserve accuracy end-to-end.
            int output = 0;
            int start = 0;
            int baseMultiplier = 10;
            if (isHex)
            {
                start = 2; // skip the "0x" prefix
                baseMultiplier = 16;
                rawValue = rawValue.ToLowerInvariant();
            }
            for (int i = start; i < rawValue.Length; i++)
            {
                char d = rawValue[i];
                int digitVal = 0;
                if (d >= '0' && d <= '9')
                {
                    digitVal = d - '0';
                }
                else if (isHex && d >= 'a' && d <= 'f')
                {
                    digitVal = d - 'a' + 10;
                }
                else
                {
                    if (isHex) Errors.ThrowError(throwToken, "Invalid hexadecimal constant.");
                    Errors.ThrowError(throwToken, "Invalid integer constant");
                }

                output = output * baseMultiplier + digitVal;
            }

            return output;
        }
    }
}
