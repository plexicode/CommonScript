using System;
using System.Collections.Generic;
using System.Linq;

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
            this.tokens.popExpected("(");
            while (!this.tokens.popIfPresent(")"))
            {
                if (tokensOut.Count > 0) this.tokens.popExpected(",");
                tokensOut.Add(this.tokens.popName("argument name"));
                Expression argValue = null;
                if (this.tokens.popIfPresent("="))
                {
                    argValue = this.expressionParser.ParseExpression();
                }
                defaultValuesOut.Add(argValue);
            }
        }

        public AbstractEntity ParseField(Dictionary<string, Token> annotations)
        {
            Token fieldKeyword = this.tokens.popKeyword("field");
            Token nameToken = this.tokens.popName("field name");
            Expression defaultValue = null;
            Token equalToken = null;
            if (this.tokens.isNext("="))
            {
                equalToken = this.tokens.pop();
                defaultValue = this.expressionParser.ParseExpression();
            }
            this.tokens.popExpected(";");
            AbstractEntity entity = new FieldEntity(fieldKeyword, nameToken, equalToken, defaultValue);
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseFunctionDefinition(Dictionary<string, Token> annotations)
        {
            Token functionKeyword = this.tokens.popKeyword("function");
            Token nameToken = this.tokens.popName("function name");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            AbstractEntity entity = new FunctionEntity(functionKeyword, nameToken, args.ToArray(), argValues.ToArray(), code);
            entity.annotations = annotations;
            return entity;
        }

        public AbstractEntity ParseConstructor(Dictionary<string, Token> annotations)
        {
            Token ctorKeyword = this.tokens.popKeyword("constructor");
            List<Token> args = new List<Token>();
            List<Expression> argValues = new List<Expression>();
            this.ParseArgDefinitionList(args, argValues);

            Expression[] baseArgs = null;
            if (this.tokens.popIfPresent(":"))
            {
                Token baseKeyword = this.tokens.popKeyword("base");
                List<Expression> bargs = new List<Expression>();
                this.tokens.popExpected("(");
                while (!this.tokens.popIfPresent(")"))
                {
                    if (bargs.Count > 0) this.tokens.popExpected(",");
                    bargs.Add(this.expressionParser.ParseExpression());
                }
                baseArgs = bargs.ToArray();
            }

            Statement[] code = this.statementParser.ParseCodeBlock(true);

            return new ConstructorEntity(
                ctorKeyword,
                args.ToArray(),
                argValues.ToArray(),
                baseArgs,
                code,
                annotations.ContainsKey("static"));
        }

        public AbstractEntity ParseConst()
        {
            Token constKeyword = this.tokens.popKeyword("const");
            Token nameToken = this.tokens.popName("constant name");
            this.tokens.popExpected("=");
            Expression constValue = this.expressionParser.ParseExpression();
            this.tokens.popExpected(";");

            return new ConstEntity(constKeyword, nameToken, constValue);
        }

        public AbstractEntity ParseEnum()
        {
            Token enumKeyword = this.tokens.popKeyword("enum");
            Token nameToken = this.tokens.popName("enum name");
            this.tokens.popExpected("{");
            bool nextAllowed = true;
            List<Token> names = new List<Token>();
            List<Expression> values = new List<Expression>();
            while (nextAllowed && !this.tokens.isNext("}"))
            {
                names.Add(this.tokens.popName("enum member name"));
                Expression value = null;
                if (this.tokens.popIfPresent("="))
                {
                    value = this.expressionParser.ParseExpression();
                }
                values.Add(value);
                nextAllowed = this.tokens.popIfPresent(",");
            }

            return new EnumEntity(enumKeyword, nameToken, names.ToArray(), values.ToArray());
        }
    }
}
