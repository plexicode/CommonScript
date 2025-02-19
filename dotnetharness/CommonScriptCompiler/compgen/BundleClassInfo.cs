using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class BundleClassInfo
    {
        public int id;
        public int parentId;
        public string name;
        public int ctorId;
        public int staticCtorId;
        public System.Collections.Generic.Dictionary<string, int> methodsToId;
        public string[] newDirectMembersByNextOffsets;
        public System.Collections.Generic.List<string> staticMethods;
        public System.Collections.Generic.List<string> staticFields;

        public BundleClassInfo(int id, int parentId, string name, int ctorId, int staticCtorId, System.Collections.Generic.Dictionary<string, int> methodsToId, string[] newDirectMembersByNextOffsets, System.Collections.Generic.List<string> staticMethods, System.Collections.Generic.List<string> staticFields)
        {
            this.id = id;
            this.parentId = parentId;
            this.name = name;
            this.ctorId = ctorId;
            this.staticCtorId = staticCtorId;
            this.methodsToId = methodsToId;
            this.newDirectMembersByNextOffsets = newDirectMembersByNextOffsets;
            this.staticMethods = staticMethods;
            this.staticFields = staticFields;
        }
    }
}
