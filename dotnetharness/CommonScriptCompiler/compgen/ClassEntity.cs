using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ClassEntity
    {
        public Token[] baseClassTokens;
        public ClassEntity baseClassEntity;
        public System.Collections.Generic.Dictionary<string, int> directMemberToOffset;
        public System.Collections.Generic.Dictionary<string, int> flattenedMemberOffsetLookup;
        public string[] newDirectMemberOffsets;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> classMembers;
        public AbstractEntity baseData;

        public ClassEntity(Token[] baseClassTokens, ClassEntity baseClassEntity, System.Collections.Generic.Dictionary<string, int> directMemberToOffset, System.Collections.Generic.Dictionary<string, int> flattenedMemberOffsetLookup, string[] newDirectMemberOffsets, System.Collections.Generic.Dictionary<string, AbstractEntity> classMembers, AbstractEntity baseData)
        {
            this.baseClassTokens = baseClassTokens;
            this.baseClassEntity = baseClassEntity;
            this.directMemberToOffset = directMemberToOffset;
            this.flattenedMemberOffsetLookup = flattenedMemberOffsetLookup;
            this.newDirectMemberOffsets = newDirectMemberOffsets;
            this.classMembers = classMembers;
            this.baseData = baseData;
        }
    }
}
