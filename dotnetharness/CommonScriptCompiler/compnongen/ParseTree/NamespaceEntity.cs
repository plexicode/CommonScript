using System.Collections.Generic;

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
