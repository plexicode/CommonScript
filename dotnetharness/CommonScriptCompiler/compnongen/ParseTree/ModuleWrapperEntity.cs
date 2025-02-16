using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ModuleWrapperEntity : AbstractEntity
    {
        public Dictionary<string, AbstractEntity> publicMembers;

        public ModuleWrapperEntity(Token token, ImportStatement imp)
            : base(token, EntityType.MODULE_REF)
        {
            // TODO: filter this down to JUST the @public-anotated members
            this.publicMembers = imp.compiledModuleRef.nestedEntities;
        }
    }
}
