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

    internal class AbstractEntity
    {
        public Token firstToken;
        public int type;

        public string simpleName;
        public Token nameToken;
        public string fqName;
        public Dictionary<string, Token> annotations;
        public bool isStatic;
        public AbstractEntity nestParent;

        public object OBJ_TEMP_CAST_fileContext;
        public int serializationIndex = -1;

        public object specificData;
        
        public AbstractEntity(Token firstToken, int type, object specificData)
        {
            this.firstToken = firstToken;
            this.type = type;
            this.isStatic = false;
            this.specificData = specificData;
        }

        private static Dictionary<string, AbstractEntity> EMPTY = new Dictionary<string, AbstractEntity>();

        public Dictionary<string, AbstractEntity> getMemberLookup()
        {
            if (this.type == (int)EntityType.CLASS) return ((ClassEntity)this.specificData).classMembers;
            if (this.type == (int)EntityType.NAMESPACE) return ((NamespaceEntity)this.specificData).nestedMembers;
            if (this.type == (int)EntityType.MODULE_REF) return ((ModuleWrapperEntity)this.specificData).publicMembers;
            return EMPTY;
        }
    }
}
