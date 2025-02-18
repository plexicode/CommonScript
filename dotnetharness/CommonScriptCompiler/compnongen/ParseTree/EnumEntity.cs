using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class EnumEntity
    {
        public Token[] memberNameTokens;
        public Expression[] memberValues;
        public AbstractEntity baseData;

        public EnumEntity(Token enumToken, Token nameToken, Token[] memberNames, Expression[] memberValues)
        {
            this.baseData = FunctionWrapper.AbstractEntity_new(enumToken, (int)EntityType.ENUM, this);
            this.baseData.nameToken = nameToken;
            this.baseData.simpleName = nameToken.Value;
            this.memberNameTokens = memberNames;
            this.memberValues = memberValues;

            if (memberNames.Length == 0)
            {
                FunctionWrapper.Errors_Throw(enumToken, "This enum definition is empty.");
            }

            Dictionary<string, bool> collisionCheck = new Dictionary<string, bool>();
            bool isImplicit = memberValues[0] == null;
            for (int i = 0; i < memberNames.Length; i++)
            {
                Token name = memberNames[i];
                if (collisionCheck.ContainsKey(name.Value)) FunctionWrapper.Errors_Throw(name, "This enum value name collides with a previous definition.");
                bool valueIsImplicit = memberValues[i] == null;
                if (valueIsImplicit != isImplicit)
                {
                    FunctionWrapper.Errors_Throw(enumToken, "This enum definition defines values for some but not all members. Mixed implicit/explicit definitions are not allowed.");
                }

                if (isImplicit)
                {
                    this.memberValues[i] = FunctionWrapper.Expression_createIntegerConstant(null, i + 1);
                }
            }
        }
    }
}
