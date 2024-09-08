using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class LambdaEntity : FunctionLikeEntity
    {
        public LambdaEntity(FileContext ctx, Token firstToken, Token[] argNames, Expression[] argDefaultValues, Statement[] code)
            : base(firstToken, EntityType.LAMBDA_ENTITY)
        {
            this.fileContext = ctx;
            this.argTokens = argNames;
            this.argDefaultValues = argDefaultValues;
            this.code = code;
        }
    }
}
