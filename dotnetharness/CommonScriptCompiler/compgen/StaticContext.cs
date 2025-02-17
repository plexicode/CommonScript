using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class StaticContext
    {
        public TokenizerStaticContext tokenizerCtx;

        public StaticContext(TokenizerStaticContext tokenizerCtx)
        {
            this.tokenizerCtx = tokenizerCtx;
        }
    }
}
