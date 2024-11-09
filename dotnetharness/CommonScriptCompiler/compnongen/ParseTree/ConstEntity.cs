namespace CommonScript.Compiler
{
    internal class ConstEntity : AbstractEntity
    {
        public Expression constValue;

        public ConstEntity(Token constToken, Token nameToken, Expression constValue)
            : base(constToken, EntityType.CONST)
        {
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.constValue = constValue;
        }
    }
}
