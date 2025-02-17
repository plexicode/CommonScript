using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class FieldEntity
    {
        public Expression defaultValue;
        public Token opToken;
        public AbstractEntity baseData;

        public FieldEntity(Token fieldToken, Token nameToken, Token equalToken, Expression defaultValueOrNull)
        {
            this.baseData = new AbstractEntity(fieldToken, EntityType.FIELD, this);
            this.defaultValue = defaultValueOrNull;
            this.baseData.nameToken = nameToken;
            this.baseData.simpleName = nameToken.Value;
            this.opToken = equalToken;
        }
    }
}
