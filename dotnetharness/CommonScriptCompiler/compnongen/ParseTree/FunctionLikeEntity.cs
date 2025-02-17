using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    public enum FunctionType
    {
        FUNCTION,
        METHOD,
        CONSTRUCTOR,
        STATIC_METHOD,
        STATIC_CONSTRUCTOR,
        LAMBDA,
    }

    internal class FunctionLikeEntity
    {
        public Token[] argTokens;
        public Expression[] argDefaultValues;
        public Expression[] baseCtorArgValues;
        public AbstractEntity baseData;

        public Dictionary<string, bool> variableScope;
        public FunctionType FunctionSubtype { get; set; }

        private static EntityType Func2Entity(FunctionType ft)
        {
            switch (ft)
            {
                case FunctionType.STATIC_CONSTRUCTOR: return EntityType.CONSTRUCTOR;
                case FunctionType.STATIC_METHOD: return EntityType.FUNCTION;
                case FunctionType.METHOD: return EntityType.FUNCTION;
                case FunctionType.LAMBDA: return EntityType.LAMBDA_ENTITY;
                case FunctionType.CONSTRUCTOR: return EntityType.CONSTRUCTOR;
                case FunctionType.FUNCTION: return EntityType.FUNCTION;
            }

            throw new System.NotImplementedException();
        }

        private FunctionLikeEntity(
            Token firstToken,
            EntityType type,
            IList<Token> argNames,
            IList<Expression> argDefaultValues,
            IList<Statement> code
        )
        {
            this.baseData = new AbstractEntity(firstToken, type, this);
            this.argTokens = [.. argNames];
            this.argDefaultValues = [.. argDefaultValues];
            this.baseData.code = [.. code];
        }

        public static FunctionLikeEntity BuildMethodOrStandalone(
            Token funcToken,
            Token nameToken,
            IList<Token> args,
            IList<Expression> argValues,
            IList<Statement> code,
            bool isStatic,
            ClassEntity classParent)
        {
            bool isMethod = classParent != null;
            FunctionLikeEntity fle = new FunctionLikeEntity(
                funcToken,
                EntityType.FUNCTION,
                args,
                argValues,
                code)
            {
                FunctionSubtype = isMethod
                    ? isStatic
                        ? FunctionType.STATIC_METHOD
                        : FunctionType.METHOD
                    : FunctionType.FUNCTION,
            };
            fle.baseData.nameToken = nameToken;
            fle.baseData.simpleName = nameToken.Value;
            return fle;
        }

        public static FunctionLikeEntity BuildLambda(
            FileContext ctx,
            Token firstToken,
            IList<Token> argNames,
            IList<Expression> argDefaultValues,
            IList<Statement> code)
        {
            FunctionLikeEntity fle = new FunctionLikeEntity(firstToken, EntityType.LAMBDA_ENTITY, argNames, argDefaultValues, code)
            {
                FunctionSubtype = FunctionType.LAMBDA,
            };
            fle.baseData.fileContext = ctx;
            return fle;
        }

        public static FunctionLikeEntity BuildConstructor(
            Token ctorToken,
            IList<Token> args,
            IList<Expression> argDefaultValues,
            IList<Expression> baseArgs,
            IList<Statement> code,
            bool isStatic)
        {
            FunctionLikeEntity fle = new FunctionLikeEntity(ctorToken, EntityType.CONSTRUCTOR, args, argDefaultValues, code)
            {
                FunctionSubtype = isStatic ? FunctionType.STATIC_CONSTRUCTOR : FunctionType.CONSTRUCTOR,
                baseCtorArgValues = baseArgs == null ? null : [.. baseArgs],
            };
            fle.baseData.simpleName = isStatic ? "@cctor" : "@ctor";
            fle.baseData.isStatic = isStatic;
            return fle;
        }
    }
}
