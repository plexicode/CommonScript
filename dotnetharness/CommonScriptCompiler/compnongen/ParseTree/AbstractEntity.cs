using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal enum EntityType
    {
        CLASS = 1,
        CONST = 2,
        CONSTRUCTOR = 3,
        ENUM = 4,
        FIELD = 5,
        FUNCTION = 6,
        NAMESPACE = 7,
        PROPERTY = 8,
        MODULE_REF = 9, // used in resolution process to refer to a module's public-exported members
        LAMBDA_ENTITY = 10, // despite being an expression, are exported like top level functions
    }

    internal class AbstractEntityUtil
    {
        private static Dictionary<string, AbstractEntity> EMPTY = new Dictionary<string, AbstractEntity>();

        public static Dictionary<string, AbstractEntity> getMemberLookup(AbstractEntity entity)
        {
            if (entity.type == (int)EntityType.CLASS) return ((ClassEntity)entity.specificData).classMembers;
            if (entity.type == (int)EntityType.NAMESPACE) return ((NamespaceEntity)entity.specificData).nestedMembers;
            if (entity.type == (int)EntityType.MODULE_REF) return ((ModuleWrapperEntity)entity.specificData).publicMembers;
            return EMPTY;
        }
    }
}
