using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class NamespaceEntity
    {
        public Dictionary<string, AbstractEntity> nestedMembers;
        public AbstractEntity baseData;
        
        public NamespaceEntity(Token nsToken,
            Token nameToken,
            string fqName)
        {
            this.baseData = new AbstractEntity(nsToken, (int)EntityType.NAMESPACE, this);
            this.baseData.nameToken = nameToken;
            this.baseData.simpleName = nameToken.Value;
            this.baseData.fqName = fqName;
            this.nestedMembers = new Dictionary<string, AbstractEntity>();
        }
    }
}
