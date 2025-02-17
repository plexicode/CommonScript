using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ModuleWrapperEntity
    {
        public System.Collections.Generic.Dictionary<string, AbstractEntity> publicMembers;
        public AbstractEntity baseData;

        public ModuleWrapperEntity(System.Collections.Generic.Dictionary<string, AbstractEntity> publicMembers, AbstractEntity baseData)
        {
            this.publicMembers = publicMembers;
            this.baseData = baseData;
        }
    }
}
