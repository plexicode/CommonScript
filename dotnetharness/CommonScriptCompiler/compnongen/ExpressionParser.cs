using System.Collections.Generic;
using CommonScript.Compiler.Internal;

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
            if (FunctionWrapper.Tokens_isNext(this.tokens, "?"))
            {
                Token qmark = FunctionWrapper.Tokens_pop(this.tokens);
                Expression trueValue = this.ParseTernary();
                FunctionWrapper.Tokens_popExpected(this.tokens, ":");
                Expression falseValue = this.ParseTernary();
                root = FunctionWrapper.Expression_createTernary(root, qmark, trueValue, falseValue);
            }
            return root;
        }

        private Expression ParseNullCoalesce()
        {
            Expression root = this.ParseBooleanCombination();
            if (FunctionWrapper.Tokens_isNext(this.tokens, "??"))
            {
                Token op = FunctionWrapper.Tokens_pop(this.tokens);
                Expression next = this.ParseNullCoalesce();
                root = FunctionWrapper.Expression_createBinaryOp(root, op, next);
            }
            return root;
        }

        private Expression ParseBooleanCombination()
        {
            Expression root = this.ParseBitwise();
            if (FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "||", "&&"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "||", "&&"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseBitwise());
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseBitwise()
        {
            Expression root = this.ParseEquality();
            if (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "&", "|", "^"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();

                while (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "&", "|", "^"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseEquality());
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseEquality()
        {
            Expression root = this.ParseInequality();
            if (FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "==", "!="))
            {
                Token op = FunctionWrapper.Tokens_pop(this.tokens);
                Expression right = this.ParseInequality();
                return FunctionWrapper.Expression_createBinaryOp(root, op, right);
            }
            return root;
        }

        private Expression ParseInequality()
        {
            Expression root = this.ParseBitshift();
            if (FunctionWrapper.Tokens_doesNextInclude5(this.tokens, "<", ">", "<=", ">=", "is"))
            {
                Token op = FunctionWrapper.Tokens_pop(this.tokens);
                Expression right = this.ParseBitshift();
                root = FunctionWrapper.Expression_createBinaryOp(root, op, right);
            }
            return root;
        }

        private Expression ParseBitshift()
        {
            Expression root = this.ParseAddition();
            if (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "<<", ">>", ">>>"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "<<", ">>", ">>>"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseAddition());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseAddition()
        {
            Expression root = this.ParseMultiplication();
            if (FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "+", "-"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "+", "-"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseMultiplication());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseMultiplication()
        {
            Expression root = this.ParseExponent();
            if (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "*", "/", "%"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "*", "/", "%"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseExponent());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseExponent()
        {
            Expression root = this.ParseUnaryPrefix();
            if (FunctionWrapper.Tokens_isNext(this.tokens, "**"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_isNext(this.tokens, "**"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(this.tokens));
                    expressions.Add(this.ParseUnaryPrefix());
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private Expression ParseUnaryPrefix()
        {
            string next = FunctionWrapper.Tokens_peekValue(this.tokens);
            if (next == null) FunctionWrapper.Tokens_ensureMore(this.tokens);
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
                case 't':
                    if (next == "typeof") return this.ParseTypeofPrefix();
                    break;
            }

            return this.ParseUnarySuffix();
        }

        private Expression ParseTypeofPrefix()
        {
            Token typeofToken = FunctionWrapper.Tokens_popKeyword(this.tokens, "typeof");
            Expression root = this.ParseUnaryPrefix();
            return FunctionWrapper.Expression_createTypeof(typeofToken, root);
        }

        private Expression ParseNegatePrefix()
        {
            if (!FunctionWrapper.Tokens_doesNextInclulde3(this.tokens, "-", "!", "~"))
            {
                FunctionWrapper.Tokens_popExpected(this.tokens, "-"); // assert
            }
            Token op = FunctionWrapper.Tokens_pop(this.tokens);
            Expression root = this.ParseUnaryPrefix();
            return FunctionWrapper.Expression_createNegatePrefix(op, root);
        }

        private Expression ParseUnarySuffix()
        {
            Expression root = this.ParseAtomicExpression();
            string next = FunctionWrapper.Tokens_peekValue(this.tokens);
            bool checkForSuffixes = true;
            while (checkForSuffixes && next != null)
            {
                checkForSuffixes = false;
                switch (next)
                {
                    case ".":
                        Token dotToken = FunctionWrapper.Tokens_pop(this.tokens);
                        Token nameToken = FunctionWrapper.Tokens_popName(this.tokens, "field name");
                        root = FunctionWrapper.Expression_createDotField(root, dotToken, nameToken.Value);
                        checkForSuffixes = true;
                        break;

                    case "(":
                        Token openParen = FunctionWrapper.Tokens_pop(this.tokens);
                        List<Expression> args = new List<Expression>();
                        while (!FunctionWrapper.Tokens_popIfPresent(this.tokens, ")"))
                        {
                            if (args.Count > 0) FunctionWrapper.Tokens_popExpected(this.tokens, ",");
                            args.Add(this.ParseExpression());
                        }
                        root = FunctionWrapper.Expression_createFunctionInvocation(root, openParen, args.ToArray());
                        checkForSuffixes = true;
                        break;

                    case "[":

                        Token openBracket = FunctionWrapper.Tokens_pop(this.tokens);
                        FunctionWrapper.Tokens_ensureMore(this.tokens);
                        Token throwTokenOnInvalidSlice = FunctionWrapper.Tokens_peek(this.tokens);
                        List<Expression> sliceNums = new List<Expression>();
                        bool nextExpected = true;

                        while (nextExpected && FunctionWrapper.Tokens_hasMore(this.tokens))
                        {
                            if (FunctionWrapper.Tokens_popIfPresent(this.tokens, ":"))
                            {
                                sliceNums.Add(null);
                            }
                            else if (FunctionWrapper.Tokens_isNext(this.tokens, "]"))
                            {
                                sliceNums.Add(null);
                                nextExpected = false;
                            }
                            else
                            {
                                sliceNums.Add(this.ParseExpression());
                                if (!FunctionWrapper.Tokens_popIfPresent(this.tokens, ":"))
                                {
                                    nextExpected = false;
                                }
                            }
                        }

                        FunctionWrapper.Tokens_popExpected(this.tokens, "]");

                        if (sliceNums.Count < 0 || sliceNums.Count > 3)
                        {
                            FunctionWrapper.Errors_Throw(throwTokenOnInvalidSlice, "Invalid index or slice expression");
                        }

                        if (sliceNums.Count == 1)
                        {
                            if (sliceNums[0] == null)
                            {
                                FunctionWrapper.Errors_Throw(throwTokenOnInvalidSlice, "Expected index expression.");
                            }

                            root = FunctionWrapper.Expression_createBracketIndex(root, openBracket, sliceNums[0]);
                        }
                        else
                        {
                            root = FunctionWrapper.Expression_createSliceExpression(root, openBracket, sliceNums[0], sliceNums[1], sliceNums.Count == 3 ? sliceNums[2] : null);
                        }

                        checkForSuffixes = true;
                        break;

                    case "++":
                    case "--":
                        Token ppToken = FunctionWrapper.Tokens_pop(this.tokens);
                        root = FunctionWrapper.Expression_createInlineIncrement(root.firstToken, root, ppToken, false);
                        break;

                    default:
                        break;
                }
                next = FunctionWrapper.Tokens_peekValue(this.tokens);
            }
            return root;
        }

        private Expression ParseInlineIncrementPrefix()
        {
            if (!FunctionWrapper.Tokens_doesNextInclulde2(this.tokens, "++", "--"))
            {
                // more of an assert
                FunctionWrapper.Tokens_popExpected(this.tokens, "++");
            }
            Token op = FunctionWrapper.Tokens_pop(this.tokens);
            Expression root = this.ParseUnarySuffix();
            return FunctionWrapper.Expression_createInlineIncrement(op, root, op, true);
        }

        private static Expression FlattenBinaryOpChain(List<Expression> expressions, List<Token> ops)
        {
            string opType = ops[0].Value;
            bool isShortCircuit = opType == "??" || opType == "&&" || opType == "||";
            Expression acc;
            int length = expressions.Count;
            if (isShortCircuit)
            {
                acc = FunctionWrapper.Expression_createBinaryOp(expressions[length - 2], ops[length - 2], expressions[length - 1]);
                for (int i = length - 3; i >= 0; i--)
                {
                    acc = FunctionWrapper.Expression_createBinaryOp(expressions[i], ops[i], acc);
                }
            }
            else
            {
                acc = FunctionWrapper.Expression_createBinaryOp(expressions[0], ops[0], expressions[1]);

                for (int i = 2; i < length; i++)
                {
                    acc = FunctionWrapper.Expression_createBinaryOp(acc, ops[i - 1], expressions[i]);
                }
            }

            return acc;
        }

        private Expression ParseListDefinition()
        {
            Token openListToken = FunctionWrapper.Tokens_popExpected(this.tokens, "[");
            List<Expression> items = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(this.tokens, "]"))
            {
                items.Add(this.ParseExpression());
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(this.tokens, ",");
            }
            FunctionWrapper.Tokens_popExpected(this.tokens, "]");
            return FunctionWrapper.Expression_createListDefinition(openListToken, items.ToArray());
        }

        private Expression ParseDictionaryDefinition()
        {
            Token openDictionaryToken = FunctionWrapper.Tokens_popExpected(this.tokens, "{");
            List<Expression> keys = new List<Expression>();
            List<Expression> values = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(this.tokens, "}"))
            {
                keys.Add(this.ParseExpression());
                FunctionWrapper.Tokens_popExpected(this.tokens, ":");
                values.Add(this.ParseExpression());
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(this.tokens, ",");
            }
            FunctionWrapper.Tokens_popExpected(this.tokens, "}");
            return FunctionWrapper.Expression_createDictionaryDefinition(openDictionaryToken, keys.ToArray(), values.ToArray());
        }

        private Expression ParseAtomicExpression()
        {
            Token nextToken = FunctionWrapper.Tokens_peek(this.tokens);
            if (nextToken == null) FunctionWrapper.Tokens_ensureMore(this.tokens);
            string next = nextToken.Value;
            switch (nextToken.Type)
            {
                case (int)TokenType.PUNCTUATION:

                    if (next == "(")
                    {
                        // () =>
                        if (FunctionWrapper.Tokens_isSequenceNext3(this.tokens, "(", ")", "=>")) return this.ParseLambda();

                        // (a, ...
                        if (FunctionWrapper.Tokens_isSequenceNext3(this.tokens, "(", null, ",") &&
                            FunctionWrapper.Tokens_peekAhead(this.tokens, 1).Type == (int)TokenType.NAME) return this.ParseLambda();

                        // (a = ...
                        if (FunctionWrapper.Tokens_isSequenceNext3(this.tokens, "(", null, "=") &&
                            FunctionWrapper.Tokens_peekAhead(this.tokens, 1).Type == (int)TokenType.NAME) return this.ParseLambda();

                        // (a) => 
                        if (FunctionWrapper.Tokens_isSequenceNext4(this.tokens, "(", null, ")", "=>")) return this.ParseLambda();

                        FunctionWrapper.Tokens_pop(this.tokens);
                        Expression expr = this.ParseExpression();
                        FunctionWrapper.Tokens_popExpected(this.tokens, ")");
                        return expr;

                    }
                    if (next == "[") return this.ParseListDefinition();
                    if (next == "{") return this.ParseDictionaryDefinition();
                    if (next == "$")
                    {
                        Token builtinPrefix = FunctionWrapper.Tokens_pop(this.tokens);
                        Token builtinName = FunctionWrapper.Tokens_popName(this.tokens, "built-in function name");
                        return FunctionWrapper.Expression_createExtensionReference(builtinPrefix, builtinName.Value);
                    }
                    break;

                case (int) TokenType.KEYWORD:
                    if (next == "true" || next == "false")
                    {
                        Token boolTok = FunctionWrapper.Tokens_pop(this.tokens);
                        return FunctionWrapper.Expression_createBoolConstant(boolTok, next == "true");
                    }

                    if (next == "null")
                    {
                        return FunctionWrapper.Expression_createNullConstant(FunctionWrapper.Tokens_pop(this.tokens));
                    }

                    if (next == "new")
                    {
                        Token newTok = FunctionWrapper.Tokens_pop(this.tokens);
                        List<Token> nameChain = new List<Token>() { FunctionWrapper.Tokens_popName(this.tokens, "class name") };
                        while (FunctionWrapper.Tokens_isNext(this.tokens, "."))
                        {
                            nameChain.Add(FunctionWrapper.Tokens_pop(this.tokens));
                            nameChain.Add(FunctionWrapper.Tokens_popName(this.tokens, "class name"));
                        }
                        Expression ctorChain = FunctionWrapper.Expression_createVariable(nameChain[0], nameChain[0].Value);
                        for (int i = 1; i < nameChain.Count; i += 2)
                        {
                            ctorChain = FunctionWrapper.Expression_createDotField(ctorChain, nameChain[i], nameChain[i + 1].Value);
                        }
                        return FunctionWrapper.Expression_createConstructorReference(newTok, ctorChain);
                    }

                    if (next == "this")
                    {
                        return FunctionWrapper.Expression_createThisReference(FunctionWrapper.Tokens_pop(this.tokens));
                    }

                    if (next == "base")
                    {
                        return FunctionWrapper.Expression_createBaseReference(FunctionWrapper.Tokens_pop(this.tokens));
                    }

                    break;

                case (int) TokenType.INTEGER:
                    int intVal = ExpressionParser.TryParseInteger(nextToken, next, false);
                    return FunctionWrapper.Expression_createIntegerConstant(FunctionWrapper.Tokens_pop(this.tokens), intVal);

                case (int) TokenType.FLOAT:
                    double floatVal = ExpressionParser.TryParseFloat(nextToken, next);
                    return FunctionWrapper.Expression_createFloatConstant(FunctionWrapper.Tokens_pop(this.tokens), floatVal);

                case (int) TokenType.HEX_INTEGER:
                    int intValHex = ExpressionParser.TryParseInteger(nextToken, next, true);
                    return FunctionWrapper.Expression_createIntegerConstant(FunctionWrapper.Tokens_pop(this.tokens), intValHex);

                case (int) TokenType.STRING:
                    string strVal = ExpressionParser.TryParseString(nextToken, next);
                    return FunctionWrapper.Expression_createStringConstant(FunctionWrapper.Tokens_pop(this.tokens), strVal);

                case (int) TokenType.NAME:
                    if (FunctionWrapper.Tokens_isSequenceNext2(this.tokens, null, "=>")) return this.ParseLambda();

                    Token varName = FunctionWrapper.Tokens_popName(this.tokens, "variable name");
                    return FunctionWrapper.Expression_createVariable(varName, varName.Value);
            }

            FunctionWrapper.Errors_Throw(nextToken, "Expected an expression but found '" + next + "' instead.");
            return null;
        }

        private Expression ParseLambda()
        {
            Token firstToken = FunctionWrapper.Tokens_peek(this.tokens);
            List<Token> argTokens = new List<Token>();
            List<Expression> argDefaultValues = new List<Expression>();
            if (FunctionWrapper.Tokens_popIfPresent(this.tokens, "("))
            {
                while (!FunctionWrapper.Tokens_popIfPresent(this.tokens, ")"))
                {
                    if (argTokens.Count > 0) FunctionWrapper.Tokens_popExpected(this.tokens, ",");
                    argTokens.Add(FunctionWrapper.Tokens_popName(this.tokens, "argument name"));
                    Expression defaultVal = null;
                    if (FunctionWrapper.Tokens_popIfPresent(this.tokens, "="))
                    {
                        defaultVal = this.ParseExpression();
                    }
                    argDefaultValues.Add(defaultVal);
                }
            }
            else
            {
                argTokens.Add(FunctionWrapper.Tokens_popName(this.tokens, "argument name"));
                argDefaultValues.Add(null);
            }

            Token arrow = FunctionWrapper.Tokens_popExpected(this.tokens, "=>");

            Statement[] code;
            if (FunctionWrapper.Tokens_isNext(this.tokens, "{"))
            {
                code = this.statementParser.ParseCodeBlock(true);
            }
            else
            {
                Expression codeExpr = this.ParseExpression();
                code = new Statement[] {
                    StatementUtil.createReturn(arrow, codeExpr)
                };
            }
            return FunctionWrapper.Expression_createLambda(firstToken, argTokens.ToArray(), argDefaultValues.ToArray(), arrow, code);
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
                    if (i == length) FunctionWrapper.Errors_Throw(throwToken, "Invalid backslash in string constant.");

                    c = rawValue.Substring(i, 1);
                    if (c == "n") c = "\n";
                    else if (c == "r") c = "\r";
                    else if (c == "'" || c == "\"" || c == "\\") { }
                    else if (c == "t") c = "\t";
                    else
                    {
                        FunctionWrapper.Errors_Throw(throwToken, "Unrecognized string escape sequence: '\\" + c + "'");
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
                FunctionWrapper.Errors_Throw(throwToken, "Invalid float constant");
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
                    if (isHex) FunctionWrapper.Errors_Throw(throwToken, "Invalid hexadecimal constant.");
                    FunctionWrapper.Errors_Throw(throwToken, "Invalid integer constant");
                }

                output = output * baseMultiplier + digitVal;
            }

            return output;
        }
    }
}
