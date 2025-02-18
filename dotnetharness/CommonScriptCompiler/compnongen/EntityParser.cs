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
            TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
            while (!TokenStreamUtil.Tokens_popIfPresent(this.tokens, ")"))
            {
                if (tokensOut.Count > 0) TokenStreamUtil.Tokens_popExpected(this.tokens, ",");
                tokensOut.Add(TokenStreamUtil.Tokens_popName(this.tokens, "argument name"));
                Expression argValue = null;
                if (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "="))
                {
                    argValue = this.expressionParser.ParseExpression();
                }
                defaultValuesOut.Add(argValue);
            }
        }

        public AbstractEntity ParseField(Dictionary<string, Token> annotations)
        {
            Token fieldKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "field");
            Token nameToken = TokenStreamUtil.Tokens_popName(this.tokens, "field name");
            Expression defaultValue = null;
            Token equalToken = null;
            if (TokenStreamUtil.Tokens_isNext(this.tokens, "="))
            {
                equalToken = TokenStreamUtil.Tokens_pop(this.tokens);
                defaultValue = this.expressionParser.ParseExpression();
            }
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");
            AbstractEntity entity = new FieldEntity(fieldKeyword, nameToken, equalToken, defaultValue).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseFunctionDefinition(
            Dictionary<string, Token> annotations,
            ClassEntity optionalParentClass)
        {
            Token functionKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "function");
            Token nameToken = TokenStreamUtil.Tokens_popName(this.tokens, "function name");
            bool isStatic = annotations.ContainsKey("@static");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            AbstractEntity entity = FunctionLikeEntity.BuildMethodOrStandalone(
                functionKeyword, nameToken, args, argValues, code, isStatic, optionalParentClass).baseData;
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseConstructor(Dictionary<string, Token> annotations)
        {
            Token ctorKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "constructor");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Expression[] baseArgs = null;
            if (TokenStreamUtil.Tokens_popIfPresent(this.tokens, ":"))
            {
                Token baseKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "base");
                List<Expression> bargs = new List<Expression>();
                TokenStreamUtil.Tokens_popExpected(this.tokens, "(");
                while (!TokenStreamUtil.Tokens_popIfPresent(this.tokens, ")"))
                {
                    if (bargs.Count > 0) TokenStreamUtil.Tokens_popExpected(this.tokens, ",");
                    bargs.Add(this.expressionParser.ParseExpression());
                }
                baseArgs = bargs.ToArray();
            }

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            AbstractEntity ctor = FunctionLikeEntity.BuildConstructor(
                ctorKeyword,
                args,
                argValues,
                baseArgs,
                code,
                annotations.ContainsKey("static")).baseData;

            ctor.annotations = annotations;
            return ctor;
        }

        public AbstractEntity ParseConst()
        {
            Token constKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "const");
            Token nameToken = TokenStreamUtil.Tokens_popName(this.tokens, "constant name");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "=");
            Expression constValue = this.expressionParser.ParseExpression();
            TokenStreamUtil.Tokens_popExpected(this.tokens, ";");

            return new ConstEntity(constKeyword, nameToken, constValue).baseData;
        }

        public AbstractEntity ParseEnum()
        {
            Token enumKeyword = TokenStreamUtil.Tokens_popKeyword(this.tokens, "enum");
            Token nameToken = TokenStreamUtil.Tokens_popName(this.tokens, "enum name");
            TokenStreamUtil.Tokens_popExpected(this.tokens, "{");
            bool nextAllowed = true;
            List<Token> names = new List<Token>();
            List<Expression> values = new List<Expression>();
            while (nextAllowed && !TokenStreamUtil.Tokens_isNext(this.tokens, "}"))
            {
                names.Add(TokenStreamUtil.Tokens_popName(this.tokens, "enum member name"));
                Expression value = null;
                if (TokenStreamUtil.Tokens_popIfPresent(this.tokens, "="))
                {
                    value = this.expressionParser.ParseExpression();
                }
                values.Add(value);
                nextAllowed = TokenStreamUtil.Tokens_popIfPresent(this.tokens, ",");
            }

            TokenStreamUtil.Tokens_popExpected(this.tokens, "}");

            return new EnumEntity(enumKeyword, nameToken, names.ToArray(), values.ToArray()).baseData;
        }
    }
}
