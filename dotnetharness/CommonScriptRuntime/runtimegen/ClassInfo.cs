using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ClassInfo
    {
        public int id;
        public int parentId;
        public ClassInfo parent;
        public string name;
        public Value ctor;
        public int staticCtorFuncId;
        public bool staticInitialized;
        public System.Collections.Generic.Dictionary<string, FunctionInfo> methods;
        public System.Collections.Generic.Dictionary<string, int> nameToOffset;
        public string[] nameByOffset;
        public Value[] initialValues;
        public System.Collections.Generic.List<string> newMembersInOffsetOrder;
        public System.Collections.Generic.List<int> newMemberInfoFlags;
        public Value classRef;
        public System.Collections.Generic.Dictionary<string, Value> staticMembers;
        public System.Collections.Generic.Dictionary<string, bool> staticMemberIsMutable;

        public ClassInfo(int id, int parentId, ClassInfo parent, string name, Value ctor, int staticCtorFuncId, bool staticInitialized, System.Collections.Generic.Dictionary<string, FunctionInfo> methods, System.Collections.Generic.Dictionary<string, int> nameToOffset, string[] nameByOffset, Value[] initialValues, System.Collections.Generic.List<string> newMembersInOffsetOrder, System.Collections.Generic.List<int> newMemberInfoFlags, Value classRef, System.Collections.Generic.Dictionary<string, Value> staticMembers, System.Collections.Generic.Dictionary<string, bool> staticMemberIsMutable)
        {
            this.id = id;
            this.parentId = parentId;
            this.parent = parent;
            this.name = name;
            this.ctor = ctor;
            this.staticCtorFuncId = staticCtorFuncId;
            this.staticInitialized = staticInitialized;
            this.methods = methods;
            this.nameToOffset = nameToOffset;
            this.nameByOffset = nameByOffset;
            this.initialValues = initialValues;
            this.newMembersInOffsetOrder = newMembersInOffsetOrder;
            this.newMemberInfoFlags = newMemberInfoFlags;
            this.classRef = classRef;
            this.staticMembers = staticMembers;
            this.staticMemberIsMutable = staticMemberIsMutable;
        }
    }

}
