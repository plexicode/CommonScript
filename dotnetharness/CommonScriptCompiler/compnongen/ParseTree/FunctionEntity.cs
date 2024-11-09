namespace CommonScript.Compiler
{
    internal class FunctionEntity : FunctionLikeEntity
    {
        public FunctionEntity(Token funcToken, Token nameToken, Token[] args, Expression[] argValues, Statement[] code)
            : base(funcToken, EntityType.FUNCTION)
        {
            this.nameToken = nameToken;
            this.simpleName = nameToken.Value;
            this.argTokens = args;
            this.argDefaultValues = argValues;
            this.code = code;
        }
    }
}
