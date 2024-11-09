namespace CommonScript.Compiler
{
    internal class FieldEntity : AbstractEntity
    {
        public Expression defaultValue;
        public Token opToken;

        public FieldEntity(Token fieldToken, Token nameToken, Token equalToken, Expression defaultValueOrNull)
            : base(fieldToken, EntityType.FIELD)
        {
            this.defaultValue = defaultValueOrNull;
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.opToken = equalToken;
        }
    }
}
