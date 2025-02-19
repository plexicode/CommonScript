using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class BundleEnumInfo
    {
        public int enumId;
        public string[] names;
        public int[] values;

        public BundleEnumInfo(int enumId, string[] names, int[] values)
        {
            this.enumId = enumId;
            this.names = names;
            this.values = values;
        }
    }
}
