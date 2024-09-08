using System;
using System.Collections.Generic;
using System.Linq;

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
        public EntityType type;

        public string simpleName;
        public Token nameToken;
        public string fqName;
        public Dictionary<string, Token> annotations;
        public bool isStatic;
        public AbstractEntity nestParent;

        public Statement[] code;
        public FileContext fileContext;
        public int serializationIndex = -1;

        protected AbstractEntity(Token firstToken, EntityType type)
        {
            this.firstToken = firstToken;
            this.type = type;
            this.isStatic = false;
        }

        private static Dictionary<string, AbstractEntity> EMPTY = new Dictionary<string, AbstractEntity>();

        public Dictionary<string, AbstractEntity> getMemberLookup()
        {
            if (this.type == EntityType.CLASS) return ((ClassEntity)this).classMembers;
            if (this.type == EntityType.NAMESPACE) return ((NamespaceEntity)this).nestedMembers;
            if (this.type == EntityType.MODULE_REF) return ((ModuleWrapperEntity)this).publicMembers;
            return EMPTY;
        }
    }
}
