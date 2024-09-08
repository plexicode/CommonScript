using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class Instance
    {
        public int id;
        public ClassInfo classDef;
        public Value[] members;

        public Instance(int id, ClassInfo classDef, Value[] members)
        {
            this.id = id;
            this.classDef = classDef;
            this.members = members;
        }
    }

}
