using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    // ternary -> null coalesce -> boolean combine -> bitwise ->
    // equality -> inequality -> bitshift ->
    // addition -> multiplication -> unary prefix -> unary suffix ->
    // atomic

    internal static class ExpressionParser
    {
        public static Expression ParseExpression(TokenStream tokens)
        {
            return ParseTernary(tokens);
        }

        private static Expression ParseTernary(TokenStream tokens)
        {
            Expression root = ParseNullCoalesce(tokens);
            if (FunctionWrapper.Tokens_isNext(tokens, "?"))
            {
                Token qmark = FunctionWrapper.Tokens_pop(tokens);
                Expression trueValue = ParseTernary(tokens);
                FunctionWrapper.Tokens_popExpected(tokens, ":");
                Expression falseValue = ParseTernary(tokens);
                root = FunctionWrapper.Expression_createTernary(root, qmark, trueValue, falseValue);
            }
            return root;
        }

        private static Expression ParseNullCoalesce(TokenStream tokens)
        {
            Expression root = ParseBooleanCombination(tokens);
            if (FunctionWrapper.Tokens_isNext(tokens, "??"))
            {
                Token op = FunctionWrapper.Tokens_pop(tokens);
                Expression next = ParseNullCoalesce(tokens);
                root = FunctionWrapper.Expression_createBinaryOp(root, op, next);
            }
            return root;
        }

        private static Expression ParseBooleanCombination(TokenStream tokens)
        {
            Expression root = ParseBitwise(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde2(tokens, "||", "&&"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde2(tokens, "||", "&&"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseBitwise(tokens));
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseBitwise(TokenStream tokens)
        {
            Expression root = ParseEquality(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "&", "|", "^"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();

                while (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "&", "|", "^"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseEquality(tokens));
                }
                return FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseEquality(TokenStream tokens)
        {
            Expression root = ParseInequality(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde2(tokens, "==", "!="))
            {
                Token op = FunctionWrapper.Tokens_pop(tokens);
                Expression right = ParseInequality(tokens);
                return FunctionWrapper.Expression_createBinaryOp(root, op, right);
            }
            return root;
        }

        private static Expression ParseInequality(TokenStream tokens)
        {
            Expression root = ParseBitshift(tokens);
            if (FunctionWrapper.Tokens_doesNextInclude5(tokens, "<", ">", "<=", ">=", "is"))
            {
                Token op = FunctionWrapper.Tokens_pop(tokens);
                Expression right = ParseBitshift(tokens);
                root = FunctionWrapper.Expression_createBinaryOp(root, op, right);
            }
            return root;
        }

        private static Expression ParseBitshift(TokenStream tokens)
        {
            Expression root = ParseAddition(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "<<", ">>", ">>>"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "<<", ">>", ">>>"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseAddition(tokens));
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseAddition(TokenStream tokens)
        {
            Expression root = ParseMultiplication(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde2(tokens, "+", "-"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde2(tokens, "+", "-"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseMultiplication(tokens));
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseMultiplication(TokenStream tokens)
        {
            Expression root = ParseExponent(tokens);
            if (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "*", "/", "%"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_doesNextInclulde3(tokens, "*", "/", "%"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseExponent(tokens));
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseExponent(TokenStream tokens)
        {
            Expression root = ParseUnaryPrefix(tokens);
            if (FunctionWrapper.Tokens_isNext(tokens, "**"))
            {
                List<Expression> expressions = new List<Expression>() { root };
                List<Token> ops = new List<Token>();
                while (FunctionWrapper.Tokens_isNext(tokens, "**"))
                {
                    ops.Add(FunctionWrapper.Tokens_pop(tokens));
                    expressions.Add(ParseUnaryPrefix(tokens));
                }
                root = FlattenBinaryOpChain(expressions, ops);
            }
            return root;
        }

        private static Expression ParseUnaryPrefix(TokenStream tokens)
        {
            string next = FunctionWrapper.Tokens_peekValue(tokens);
            if (next == null) FunctionWrapper.Tokens_ensureMore(tokens);
            switch (next[0])
            {
                case '-':
                    if (next == "--") return ParseInlineIncrementPrefix(tokens);
                    if (next == "-") return ParseNegatePrefix(tokens);
                    break;
                case '+':
                    if (next == "++") return ParseInlineIncrementPrefix(tokens);
                    break;
                case '!':
                    if (next == "!") return ParseNegatePrefix(tokens);
                    break;
                case '~':
                    if (next == "~") return ParseNegatePrefix(tokens);
                    break;
                case 't':
                    if (next == "typeof") return ParseTypeofPrefix(tokens);
                    break;
            }

            return ParseUnarySuffix(tokens);
        }

        private static Expression ParseTypeofPrefix(TokenStream tokens)
        {
            Token typeofToken = FunctionWrapper.Tokens_popKeyword(tokens, "typeof");
            Expression root = ParseUnaryPrefix(tokens);
            return FunctionWrapper.Expression_createTypeof(typeofToken, root);
        }

        private static Expression ParseNegatePrefix(TokenStream tokens)
        {
            if (!FunctionWrapper.Tokens_doesNextInclulde3(tokens, "-", "!", "~"))
            {
                FunctionWrapper.Tokens_popExpected(tokens, "-"); // assert
            }
            Token op = FunctionWrapper.Tokens_pop(tokens);
            Expression root = ParseUnaryPrefix(tokens);
            return FunctionWrapper.Expression_createNegatePrefix(op, root);
        }

        private static Expression ParseUnarySuffix(TokenStream tokens)
        {
            Expression root = ParseAtomicExpression(tokens);
            string next = FunctionWrapper.Tokens_peekValue(tokens);
            bool checkForSuffixes = true;
            while (checkForSuffixes && next != null)
            {
                checkForSuffixes = false;
                if (next == ".") {
                    Token dotToken = FunctionWrapper.Tokens_pop(tokens);
                    Token nameToken = FunctionWrapper.Tokens_popName(tokens, "field name");
                    root = FunctionWrapper.Expression_createDotField(root, dotToken, nameToken.Value);
                    checkForSuffixes = true;
                } else if (next == "(") {
                    Token openParen = FunctionWrapper.Tokens_pop(tokens);
                    List<Expression> args = new List<Expression>();
                    while (!FunctionWrapper.Tokens_popIfPresent(tokens, ")")) {
                        if (args.Count > 0) FunctionWrapper.Tokens_popExpected(tokens, ",");
                        args.Add(ParseExpression(tokens));
                    }
                    root = FunctionWrapper.Expression_createFunctionInvocation(root, openParen, args.ToArray());
                    checkForSuffixes = true;
                } else if (next == "[") {

                    Token openBracket = FunctionWrapper.Tokens_pop(tokens);
                    FunctionWrapper.Tokens_ensureMore(tokens);
                    Token throwTokenOnInvalidSlice = FunctionWrapper.Tokens_peek(tokens);
                    List<Expression> sliceNums = new List<Expression>();
                    bool nextExpected = true;

                    while (nextExpected && FunctionWrapper.Tokens_hasMore(tokens))
                    {
                        if (FunctionWrapper.Tokens_popIfPresent(tokens, ":"))
                        {
                            sliceNums.Add(null);
                        }
                        else if (FunctionWrapper.Tokens_isNext(tokens, "]"))
                        {
                            sliceNums.Add(null);
                            nextExpected = false;
                        }
                        else
                        {
                            sliceNums.Add(ParseExpression(tokens));
                            if (!FunctionWrapper.Tokens_popIfPresent(tokens, ":"))
                            {
                                nextExpected = false;
                            }
                        }
                    }

                    FunctionWrapper.Tokens_popExpected(tokens, "]");

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
                        Expression thirdSliceNum = null;
                        if (sliceNums.Count == 3) thirdSliceNum = sliceNums[2];
                        root = FunctionWrapper.Expression_createSliceExpression(root, openBracket, sliceNums[0],
                            sliceNums[1], thirdSliceNum);
                    }

                    checkForSuffixes = true;
                } else if (next == "++" || next == "--") {
                    Token ppToken = FunctionWrapper.Tokens_pop(tokens);
                    root = FunctionWrapper.Expression_createInlineIncrement(root.firstToken, root, ppToken, false);
                }

                next = FunctionWrapper.Tokens_peekValue(tokens);
            }
            return root;
        }

        private static Expression ParseInlineIncrementPrefix(TokenStream tokens)
        {
            if (!FunctionWrapper.Tokens_doesNextInclulde2(tokens, "++", "--"))
            {
                // more of an assert
                FunctionWrapper.Tokens_popExpected(tokens, "++");
            }
            Token op = FunctionWrapper.Tokens_pop(tokens);
            Expression root = ParseUnarySuffix(tokens);
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

        private static Expression ParseListDefinition(TokenStream tokens)
        {
            Token openListToken = FunctionWrapper.Tokens_popExpected(tokens, "[");
            List<Expression> items = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(tokens, "]"))
            {
                items.Add(ParseExpression(tokens));
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(tokens, ",");
            }
            FunctionWrapper.Tokens_popExpected(tokens, "]");
            return FunctionWrapper.Expression_createListDefinition(openListToken, items.ToArray());
        }

        private static Expression ParseDictionaryDefinition(TokenStream tokens)
        {
            Token openDictionaryToken = FunctionWrapper.Tokens_popExpected(tokens, "{");
            List<Expression> keys = new List<Expression>();
            List<Expression> values = new List<Expression>();
            bool nextAllowed = true;
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(tokens, "}"))
            {
                keys.Add(ParseExpression(tokens));
                FunctionWrapper.Tokens_popExpected(tokens, ":");
                values.Add(ParseExpression(tokens));
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(tokens, ",");
            }
            FunctionWrapper.Tokens_popExpected(tokens, "}");
            return FunctionWrapper.Expression_createDictionaryDefinition(openDictionaryToken, keys.ToArray(), values.ToArray());
        }

        private static Expression ParseAtomicExpression(TokenStream tokens)
        {
            Token nextToken = FunctionWrapper.Tokens_peek(tokens);
            if (nextToken == null) FunctionWrapper.Tokens_ensureMore(tokens);
            string next = nextToken.Value;
            switch (nextToken.Type)
            {
                case (int)TokenType.PUNCTUATION:

                    if (next == "(")
                    {
                        // () =>
                        if (FunctionWrapper.Tokens_isSequenceNext3(tokens, "(", ")", "=>")) return ParseLambda(tokens);

                        // (a, ...
                        if (FunctionWrapper.Tokens_isSequenceNext3(tokens, "(", null, ",") &&
                            FunctionWrapper.Tokens_peekAhead(tokens, 1).Type == (int)TokenType.NAME) return ParseLambda(tokens);

                        // (a = ...
                        if (FunctionWrapper.Tokens_isSequenceNext3(tokens, "(", null, "=") &&
                            FunctionWrapper.Tokens_peekAhead(tokens, 1).Type == (int)TokenType.NAME) return ParseLambda(tokens);

                        // (a) => 
                        if (FunctionWrapper.Tokens_isSequenceNext4(tokens, "(", null, ")", "=>")) return ParseLambda(tokens);

                        FunctionWrapper.Tokens_pop(tokens);
                        Expression expr = ParseExpression(tokens);
                        FunctionWrapper.Tokens_popExpected(tokens, ")");
                        return expr;

                    }
                    if (next == "[") return ParseListDefinition(tokens);
                    if (next == "{") return ParseDictionaryDefinition(tokens);
                    if (next == "$")
                    {
                        Token builtinPrefix = FunctionWrapper.Tokens_pop(tokens);
                        Token builtinName = FunctionWrapper.Tokens_popName(tokens, "built-in function name");
                        return FunctionWrapper.Expression_createExtensionReference(builtinPrefix, builtinName.Value);
                    }
                    break;

                case (int) TokenType.KEYWORD:
                    if (next == "true" || next == "false")
                    {
                        Token boolTok = FunctionWrapper.Tokens_pop(tokens);
                        return FunctionWrapper.Expression_createBoolConstant(boolTok, next == "true");
                    }

                    if (next == "null")
                    {
                        return FunctionWrapper.Expression_createNullConstant(FunctionWrapper.Tokens_pop(tokens));
                    }

                    if (next == "new")
                    {
                        Token newTok = FunctionWrapper.Tokens_pop(tokens);
                        List<Token> nameChain = new List<Token>() { FunctionWrapper.Tokens_popName(tokens, "class name") };
                        while (FunctionWrapper.Tokens_isNext(tokens, "."))
                        {
                            nameChain.Add(FunctionWrapper.Tokens_pop(tokens));
                            nameChain.Add(FunctionWrapper.Tokens_popName(tokens, "class name"));
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
                        return FunctionWrapper.Expression_createThisReference(FunctionWrapper.Tokens_pop(tokens));
                    }

                    if (next == "base")
                    {
                        return FunctionWrapper.Expression_createBaseReference(FunctionWrapper.Tokens_pop(tokens));
                    }

                    break;

                case (int) TokenType.INTEGER:
                    int intVal = FunctionWrapper.TryParseInteger(nextToken, next, false);
                    return FunctionWrapper.Expression_createIntegerConstant(FunctionWrapper.Tokens_pop(tokens), intVal);

                case (int) TokenType.FLOAT:
                    double floatVal = FunctionWrapper.TryParseFloat(nextToken, next);
                    return FunctionWrapper.Expression_createFloatConstant(FunctionWrapper.Tokens_pop(tokens), floatVal);

                case (int) TokenType.HEX_INTEGER:
                    int intValHex = FunctionWrapper.TryParseInteger(nextToken, next, true);
                    return FunctionWrapper.Expression_createIntegerConstant(FunctionWrapper.Tokens_pop(tokens), intValHex);

                case (int) TokenType.STRING:
                    string strVal = FunctionWrapper.TryParseString(nextToken, next);
                    return FunctionWrapper.Expression_createStringConstant(FunctionWrapper.Tokens_pop(tokens), strVal);

                case (int) TokenType.NAME:
                    if (FunctionWrapper.Tokens_isSequenceNext2(tokens, null, "=>")) return ParseLambda(tokens);

                    Token varName = FunctionWrapper.Tokens_popName(tokens, "variable name");
                    return FunctionWrapper.Expression_createVariable(varName, varName.Value);
            }

            FunctionWrapper.Errors_Throw(nextToken, "Expected an expression but found '" + next + "' instead.");
            return null;
        }

        private static Expression ParseLambda(TokenStream tokens)
        {
            Token firstToken = FunctionWrapper.Tokens_peek(tokens);
            List<Token> argTokens = new List<Token>();
            List<Expression> argDefaultValues = new List<Expression>();
            if (FunctionWrapper.Tokens_popIfPresent(tokens, "("))
            {
                while (!FunctionWrapper.Tokens_popIfPresent(tokens, ")"))
                {
                    if (argTokens.Count > 0) FunctionWrapper.Tokens_popExpected(tokens, ",");
                    argTokens.Add(FunctionWrapper.Tokens_popName(tokens, "argument name"));
                    Expression defaultVal = null;
                    if (FunctionWrapper.Tokens_popIfPresent(tokens, "="))
                    {
                        defaultVal = ParseExpression(tokens);
                    }
                    argDefaultValues.Add(defaultVal);
                }
            }
            else
            {
                argTokens.Add(FunctionWrapper.Tokens_popName(tokens, "argument name"));
                argDefaultValues.Add(null);
            }

            Token arrow = FunctionWrapper.Tokens_popExpected(tokens, "=>");

            Statement[] code;
            if (FunctionWrapper.Tokens_isNext(tokens, "{"))
            {
                code = StatementParser.ParseCodeBlock(tokens, true);
            }
            else
            {
                Expression codeExpr = ParseExpression(tokens);
                code = new Statement[] {
                    StatementUtil.createReturn(arrow, codeExpr)
                };
            }
            return FunctionWrapper.Expression_createLambda(firstToken, argTokens.ToArray(), argDefaultValues.ToArray(), arrow, code);
        }
    }
}
