using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class EntityParser
    {
        private static void ParseArgDefinitionList(TokenStream tokens, List<Token> tokensOut, List<Expression> defaultValuesOut)
        {
            FunctionWrapper.Tokens_popExpected(tokens, "(");
            while (!FunctionWrapper.Tokens_popIfPresent(tokens, ")"))
            {
                if (tokensOut.Count > 0) FunctionWrapper.Tokens_popExpected(tokens, ",");
                tokensOut.Add(FunctionWrapper.Tokens_popName(tokens, "argument name"));
                Expression argValue = null;
                if (FunctionWrapper.Tokens_popIfPresent(tokens, "="))
                {
                    argValue = FunctionWrapper.ParseExpression(tokens);
                }
                defaultValuesOut.Add(argValue);
            }
        }

        public static AbstractEntity ParseField(TokenStream tokens, Dictionary<string, Token> annotations)
        {
            Token fieldKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "field");
            Token nameToken = FunctionWrapper.Tokens_popName(tokens, "field name");
            Expression defaultValue = null;
            Token equalToken = null;
            if (FunctionWrapper.Tokens_isNext(tokens, "="))
            {
                equalToken = FunctionWrapper.Tokens_pop(tokens);
                defaultValue = FunctionWrapper.ParseExpression(tokens);
            }
            FunctionWrapper.Tokens_popExpected(tokens, ";");
            AbstractEntity entity = FunctionWrapper.FieldEntity_new(fieldKeyword, nameToken, equalToken, defaultValue).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public static AbstractEntity ParseFunctionDefinition(
            TokenStream tokens,
            Dictionary<string, Token> annotations,
            ClassEntity optionalParentClass)
        {
            Token functionKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "function");
            Token nameToken = FunctionWrapper.Tokens_popName(tokens, "function name");
            bool isStatic = annotations.ContainsKey("@static");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            ParseArgDefinitionList(tokens, args, argValues);

            List<Statement> code = FunctionWrapper.ParseCodeBlockList(tokens, true);

            AbstractEntity entity = FunctionWrapper.FunctionEntity_BuildMethodOrStandalone(
                functionKeyword, nameToken, args, argValues, code, isStatic, optionalParentClass).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public static AbstractEntity ParseConstructor(TokenStream tokens, Dictionary<string, Token> annotations)
        {
            Token ctorKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "constructor");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            ParseArgDefinitionList(tokens, args, argValues);

            List<Expression> baseArgs = null;
            if (FunctionWrapper.Tokens_popIfPresent(tokens, ":"))
            {
                Token baseKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "base");
                baseArgs = new List<Expression>();
                FunctionWrapper.Tokens_popExpected(tokens, "(");
                while (!FunctionWrapper.Tokens_popIfPresent(tokens, ")"))
                {
                    if (baseArgs.Count > 0) FunctionWrapper.Tokens_popExpected(tokens, ",");
                    baseArgs.Add(FunctionWrapper.ParseExpression(tokens));
                }

            }

            List<Statement> code = FunctionWrapper.ParseCodeBlockList(tokens, true);

            AbstractEntity ctor = FunctionWrapper.FunctionEntity_BuildConstructor(
                ctorKeyword,
                args,
                argValues,
                baseArgs,
                code,
                annotations.ContainsKey("static")).baseData;

            ctor.annotations = annotations;
            return ctor;
        }

        public static AbstractEntity ParseConst(TokenStream tokens)
        {
            Token constKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "const");
            Token nameToken = FunctionWrapper.Tokens_popName(tokens, "constant name");
            FunctionWrapper.Tokens_popExpected(tokens, "=");
            Expression constValue = FunctionWrapper.ParseExpression(tokens);
            FunctionWrapper.Tokens_popExpected(tokens, ";");

            return FunctionWrapper.ConstEntity_new(constKeyword, nameToken, constValue).baseData;
        }

        public static AbstractEntity ParseEnum(TokenStream tokens)
        {
            Token enumKeyword = FunctionWrapper.Tokens_popKeyword(tokens, "enum");
            Token nameToken = FunctionWrapper.Tokens_popName(tokens, "enum name");
            FunctionWrapper.Tokens_popExpected(tokens, "{");
            bool nextAllowed = true;
            List<Token> names = new List<Token>();
            List<Expression> values = new List<Expression>();
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(tokens, "}"))
            {
                names.Add(FunctionWrapper.Tokens_popName(tokens, "enum member name"));
                Expression value = null;
                if (FunctionWrapper.Tokens_popIfPresent(tokens, "="))
                {
                    value = FunctionWrapper.ParseExpression(tokens);
                }
                values.Add(value);
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(tokens, ",");
            }

            FunctionWrapper.Tokens_popExpected(tokens, "}");

            return FunctionWrapper.EnumEntity_new(enumKeyword, nameToken, names.ToArray(), values.ToArray()).baseData;
        }
    }
}
