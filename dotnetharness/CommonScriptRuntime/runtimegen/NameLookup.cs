using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class NameLookup
    {
        public int lengthId;
        public FunctionPointer[][] primitiveMethodsByTypeByNameId;

        public NameLookup(int lengthId, FunctionPointer[][] primitiveMethodsByTypeByNameId)
        {
            this.lengthId = lengthId;
            this.primitiveMethodsByTypeByNameId = primitiveMethodsByTypeByNameId;
        }
    }
}
