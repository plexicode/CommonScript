using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class EntityParser
    {
        private TokenStream tokens;
        public ExpressionParser expressionParser;
        public StatementParser statementParser;

        public EntityParser(TokenStream tokens)
        {
            this.tokens = tokens;
        }

        private void ParseArgDefinitionList(List<Token> tokensOut, List<Expression> defaultValuesOut)
        {
            FunctionWrapper.Tokens_popExpected(this.tokens, "(");
            while (!FunctionWrapper.Tokens_popIfPresent(this.tokens, ")"))
            {
                if (tokensOut.Count > 0) FunctionWrapper.Tokens_popExpected(this.tokens, ",");
                tokensOut.Add(FunctionWrapper.Tokens_popName(this.tokens, "argument name"));
                Expression argValue = null;
                if (FunctionWrapper.Tokens_popIfPresent(this.tokens, "="))
                {
                    argValue = this.expressionParser.ParseExpression();
                }
                defaultValuesOut.Add(argValue);
            }
        }

        public AbstractEntity ParseField(Dictionary<string, Token> annotations)
        {
            Token fieldKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "field");
            Token nameToken = FunctionWrapper.Tokens_popName(this.tokens, "field name");
            Expression defaultValue = null;
            Token equalToken = null;
            if (FunctionWrapper.Tokens_isNext(this.tokens, "="))
            {
                equalToken = FunctionWrapper.Tokens_pop(this.tokens);
                defaultValue = this.expressionParser.ParseExpression();
            }
            FunctionWrapper.Tokens_popExpected(this.tokens, ";");
            AbstractEntity entity = FunctionWrapper.FieldEntity_new(fieldKeyword, nameToken, equalToken, defaultValue).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseFunctionDefinition(
            Dictionary<string, Token> annotations,
            ClassEntity optionalParentClass)
        {
            Token functionKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "function");
            Token nameToken = FunctionWrapper.Tokens_popName(this.tokens, "function name");
            bool isStatic = annotations.ContainsKey("@static");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            AbstractEntity entity = FunctionWrapper.FunctionEntity_BuildMethodOrStandalone(
                functionKeyword, nameToken, args, argValues, [..code], isStatic, optionalParentClass).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseConstructor(Dictionary<string, Token> annotations)
        {
            Token ctorKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "constructor");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Expression[] baseArgs = null;
            if (FunctionWrapper.Tokens_popIfPresent(this.tokens, ":"))
            {
                Token baseKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "base");
                List<Expression> bargs = new List<Expression>();
                FunctionWrapper.Tokens_popExpected(this.tokens, "(");
                while (!FunctionWrapper.Tokens_popIfPresent(this.tokens, ")"))
                {
                    if (bargs.Count > 0) FunctionWrapper.Tokens_popExpected(this.tokens, ",");
                    bargs.Add(this.expressionParser.ParseExpression());
                }
                baseArgs = bargs.ToArray();
            }

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            AbstractEntity ctor = FunctionWrapper.FunctionEntity_BuildConstructor(
                ctorKeyword,
                args,
                argValues,
                baseArgs == null ? null : [..baseArgs],
                [..code],
                annotations.ContainsKey("static")).baseData;

            ctor.annotations = annotations;
            return ctor;
        }

        public AbstractEntity ParseConst()
        {
            Token constKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "const");
            Token nameToken = FunctionWrapper.Tokens_popName(this.tokens, "constant name");
            FunctionWrapper.Tokens_popExpected(this.tokens, "=");
            Expression constValue = this.expressionParser.ParseExpression();
            FunctionWrapper.Tokens_popExpected(this.tokens, ";");

            return FunctionWrapper.ConstEntity_new(constKeyword, nameToken, constValue).baseData;
        }

        public AbstractEntity ParseEnum()
        {
            Token enumKeyword = FunctionWrapper.Tokens_popKeyword(this.tokens, "enum");
            Token nameToken = FunctionWrapper.Tokens_popName(this.tokens, "enum name");
            FunctionWrapper.Tokens_popExpected(this.tokens, "{");
            bool nextAllowed = true;
            List<Token> names = new List<Token>();
            List<Expression> values = new List<Expression>();
            while (nextAllowed && !FunctionWrapper.Tokens_isNext(this.tokens, "}"))
            {
                names.Add(FunctionWrapper.Tokens_popName(this.tokens, "enum member name"));
                Expression value = null;
                if (FunctionWrapper.Tokens_popIfPresent(this.tokens, "="))
                {
                    value = this.expressionParser.ParseExpression();
                }
                values.Add(value);
                nextAllowed = FunctionWrapper.Tokens_popIfPresent(this.tokens, ",");
            }

            FunctionWrapper.Tokens_popExpected(this.tokens, "}");

            return FunctionWrapper.EnumEntity_new(enumKeyword, nameToken, names.ToArray(), values.ToArray()).baseData;
        }
    }
}
