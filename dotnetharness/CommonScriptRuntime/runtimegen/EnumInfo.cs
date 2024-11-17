using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class EnumInfo
    {
        public int enumId;
        public string[] names;
        public int[] values;

        public EnumInfo(int enumId, string[] names, int[] values)
        {
            this.enumId = enumId;
            this.names = names;
            this.values = values;
        }
    }
}
