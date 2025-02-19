using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class StaticContext
    {
        public TokenizerStaticContext tokenizerCtx;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> emptyLookup;
        public SpecialActionUtil specialActionUtil;

        public StaticContext(TokenizerStaticContext tokenizerCtx, System.Collections.Generic.Dictionary<string, AbstractEntity> emptyLookup, SpecialActionUtil specialActionUtil)
        {
            this.tokenizerCtx = tokenizerCtx;
            this.emptyLookup = emptyLookup;
            this.specialActionUtil = specialActionUtil;
        }
    }
}
