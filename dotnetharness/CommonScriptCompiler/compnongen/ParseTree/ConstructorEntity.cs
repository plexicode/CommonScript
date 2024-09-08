using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class ConstructorEntity : FunctionLikeEntity
    {
        public Expression[] baseCtorArgValues;

        public ConstructorEntity(
            Token ctorToken, 
            Token[] args, 
            Expression[] argDefaultValues, 
            Expression[] baseArgs, 
            Statement[] code, 
            bool isStatic)
            : base(ctorToken, EntityType.CONSTRUCTOR)
        {
            this.simpleName = isStatic ? "@cctor" : "@ctor";
            this.argTokens = args;
            this.argDefaultValues = argDefaultValues;
            this.code = code;
            this.baseCtorArgValues = baseArgs;
        }
    }
}
