using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ClassEntity : AbstractEntity
    {
        public Token[] baseClassTokens;
        public ClassEntity baseClassEntity;
        public Dictionary<string, int> directMemberToOffset;
        public Dictionary<string, int> flattenedMemberOffsetLookup;
        public int classDepth; // for sorting parents first
        public string[] newDirectMemberOffsets;
        public Dictionary<string, AbstractEntity> classMembers;

        public ClassEntity(Token classToken, Token nameToken, string fqName) : base(classToken, EntityType.CLASS)
        {
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.fqName = fqName;
            this.classMembers = new Dictionary<string, AbstractEntity>();
        }
    }
}
