using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class TokenizerStaticContext
    {
        public System.Collections.Generic.Dictionary<int, bool> alphanumerics;
        public System.Collections.Generic.Dictionary<int, bool> numerics;
        public System.Collections.Generic.Dictionary<int, bool> whitespace;
        public StringSet keywords;
        public System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int[]>> multicharTokensByFirstChar;

        public TokenizerStaticContext(System.Collections.Generic.Dictionary<int, bool> alphanumerics, System.Collections.Generic.Dictionary<int, bool> numerics, System.Collections.Generic.Dictionary<int, bool> whitespace, StringSet keywords, System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int[]>> multicharTokensByFirstChar)
        {
            this.alphanumerics = alphanumerics;
            this.numerics = numerics;
            this.whitespace = whitespace;
            this.keywords = keywords;
            this.multicharTokensByFirstChar = multicharTokensByFirstChar;
        }
    }
}
