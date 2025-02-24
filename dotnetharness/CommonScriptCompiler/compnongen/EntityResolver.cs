using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class EntityResolverUtil
    {
        public static void EntityResolver_DetermineMemberOffsets(ClassEntity classDef)
        {
            int i = 0;
            // TODO: you need to ensure that the overridden members are exclusively methods, not fields.

            if (classDef.directMemberToOffset != null) return;
            ClassEntity parent = classDef.baseClassEntity;
            if (parent != null) EntityResolver_DetermineMemberOffsets(parent);

            classDef.directMemberToOffset = new Dictionary<string, int>();
            classDef.flattenedMemberOffsetLookup = new Dictionary<string, int>();
            List<string> newDirectMembers = new List<string>();
            List<string> staticFieldNames = new List<string>();
            List<string> staticMethodNames = new List<string>();

            if (parent != null)
            {
                string[] parentKeys = parent.flattenedMemberOffsetLookup.Keys.ToArray();
                for (i = 0; i < parentKeys.Length; i += 1)
                {
                    string parentKey = parentKeys[i];
                    classDef.flattenedMemberOffsetLookup[parentKey] = parent.flattenedMemberOffsetLookup[parentKey];
                }
            }

            int nextOffset = classDef.flattenedMemberOffsetLookup.Count;
            string[] memberNames = classDef.classMembers.Keys.OrderBy(k => k).ToArray();
            for (i = 0; i < memberNames.Length; i += 1)
            {
                string memberName = memberNames[i];
                AbstractEntity member = classDef.classMembers[memberName];

                if (!member.isStatic &&
                   (member.type == (int)EntityType.FIELD || member.type == (int)EntityType.FUNCTION))
                {
                    int offset = 0;
                    if (!classDef.flattenedMemberOffsetLookup.ContainsKey(memberName))
                    {
                        offset = nextOffset;
                        nextOffset += 1;
                        newDirectMembers.Add(memberName);
                        classDef.directMemberToOffset[memberName] = offset;
                        classDef.flattenedMemberOffsetLookup[memberName] = offset;
                    }
                }
            }

            classDef.newDirectMemberOffsets = newDirectMembers.ToArray();
        }

    }
}
