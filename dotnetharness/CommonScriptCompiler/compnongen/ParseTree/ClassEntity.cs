using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ClassEntity
    {
        public Token[] baseClassTokens;
        public ClassEntity baseClassEntity;
        public Dictionary<string, int> directMemberToOffset;
        public Dictionary<string, int> flattenedMemberOffsetLookup;
        public int classDepth; // for sorting parents first
        public string[] newDirectMemberOffsets;
        public Dictionary<string, AbstractEntity> classMembers;
        public AbstractEntity baseData;

        public ClassEntity(Token classToken, Token nameToken, string fqName)
        {
            this.baseData = new AbstractEntity(classToken, (int)EntityType.CLASS, this);
            this.baseData.nameToken = nameToken;
            this.baseData.simpleName = nameToken.Value;
            this.baseData.fqName = fqName;
            this.classMembers = new Dictionary<string, AbstractEntity>();
        }
    }
}
