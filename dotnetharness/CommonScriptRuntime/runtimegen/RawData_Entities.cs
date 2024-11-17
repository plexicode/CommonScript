using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class RawData_Entities
    {
        public System.Collections.Generic.List<FunctionInfo> functionsById;
        public System.Collections.Generic.List<EnumInfo> enumsById;
        public System.Collections.Generic.List<ClassInfo> classesById;

        public RawData_Entities(System.Collections.Generic.List<FunctionInfo> functionsById, System.Collections.Generic.List<EnumInfo> enumsById, System.Collections.Generic.List<ClassInfo> classesById)
        {
            this.functionsById = functionsById;
            this.enumsById = enumsById;
            this.classesById = classesById;
        }
    }
}
