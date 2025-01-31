﻿using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class EnumEntity : AbstractEntity
    {
        public Token[] memberNameTokens;
        public Expression[] memberValues;

        public EnumEntity(Token enumToken, Token nameToken, Token[] memberNames, Expression[] memberValues)
            : base(enumToken, EntityType.ENUM)
        {
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.memberNameTokens = memberNames;
            this.memberValues = memberValues;

            if (memberNames.Length == 0)
            {
                Errors.ThrowError(enumToken, "This enum definition is empty.");
            }

            Dictionary<string, bool> collisionCheck = new Dictionary<string, bool>();
            bool isImplicit = memberValues[0] == null;
            for (int i = 0; i < memberNames.Length; i++)
            {
                Token name = memberNames[i];
                if (collisionCheck.ContainsKey(name.Value)) Errors.ThrowError(name, "This enum value name collides with a previous definition.");
                bool valueIsImplicit = memberValues[i] == null;
                if (valueIsImplicit != isImplicit)
                {
                    Errors.ThrowError(enumToken, "This enum definition defines values for some but not all members. Mixed implicit/explicit definitions are not allowed.");
                }

                if (isImplicit)
                {
                    this.memberValues[i] = Expression.createIntegerConstant(null, i + 1);
                }
            }
        }
    }
}
