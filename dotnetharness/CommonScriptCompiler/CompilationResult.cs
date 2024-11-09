namespace CommonScript.Compiler
{
    public class CompilationResult
    {
        public byte[] ByteCodePayload { get; private set; }
        public string ErrorMessage { get; private set; }
        // TODO: structured error message result 
        // TODO: base64 generator

        internal CompilationResult(byte[] successOutput, string errorMessage)
        {
            this.ByteCodePayload = successOutput;
            this.ErrorMessage = errorMessage;
        }
    }
}
