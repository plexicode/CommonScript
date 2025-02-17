﻿using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ConstEntity
    {
        public Expression constValue;
        public AbstractEntity baseData;
        
        public ConstEntity(Token constToken, Token nameToken, Expression constValue)
        {
            this.baseData = FunctionWrapper.AbstractEntity_new(constToken, (int)EntityType.CONST, this);
            this.baseData.nameToken = nameToken;
            this.baseData.simpleName = nameToken.Value;
            this.constValue = constValue;
        }
    }
}
