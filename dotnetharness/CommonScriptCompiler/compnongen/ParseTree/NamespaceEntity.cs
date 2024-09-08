using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class NamespaceEntity : AbstractEntity
    {
        public Dictionary<string, AbstractEntity> nestedMembers;

        public NamespaceEntity(Token nsToken,
            Token nameToken,
            string fqName) 
            : base(nsToken, EntityType.NAMESPACE)
        {
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.fqName = fqName;
            this.nestedMembers = new Dictionary<string, AbstractEntity>();
        }
    }
}
