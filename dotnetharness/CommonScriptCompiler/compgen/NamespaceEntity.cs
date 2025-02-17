using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class NamespaceEntity
    {
        public System.Collections.Generic.Dictionary<string, AbstractEntity> nestedMembers;
        public AbstractEntity baseData;

        public NamespaceEntity(System.Collections.Generic.Dictionary<string, AbstractEntity> nestedMembers, AbstractEntity baseData)
        {
            this.nestedMembers = nestedMembers;
            this.baseData = baseData;
        }
    }
}
