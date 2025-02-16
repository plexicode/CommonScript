using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class StringSet
    {
        public System.Collections.Generic.Dictionary<string, bool> items;

        public StringSet(System.Collections.Generic.Dictionary<string, bool> items)
        {
            this.items = items;
        }
    }
}
