using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
