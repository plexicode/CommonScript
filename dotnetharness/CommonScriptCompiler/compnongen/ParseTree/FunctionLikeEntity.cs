using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal abstract class FunctionLikeEntity : AbstractEntity
    {
        public Token[] argTokens;
        public Expression[] argDefaultValues;

        public Dictionary<string, bool> variableScope;

        public FunctionLikeEntity(Token firstToken, EntityType type) : base(firstToken, type) { }
    }
}
