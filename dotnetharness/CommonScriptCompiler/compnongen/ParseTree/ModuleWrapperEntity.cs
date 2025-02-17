using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ModuleWrapperEntity
    {
        public Dictionary<string, AbstractEntity> publicMembers;
        public AbstractEntity baseData;

        public ModuleWrapperEntity(Token token, ImportStatement imp)
        {
            this.baseData = FunctionWrapper.AbstractEntity_new(token, (int)EntityType.MODULE_REF, this);
            // TODO: filter this down to JUST the @public-anotated members
            this.publicMembers = imp.compiledModuleRef.nestedEntities;
        }
    }
}
