using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class DictImpl
    {
        public int id;
        public int keyType;
        public int size;
        public int capacity;
        public Value[] keys;
        public Value[] values;
        public System.Collections.Generic.Dictionary<int, int> intKeyLookup;
        public System.Collections.Generic.Dictionary<string, int> strKeyLookup;

        public DictImpl(int id, int keyType, int size, int capacity, Value[] keys, Value[] values, System.Collections.Generic.Dictionary<int, int> intKeyLookup, System.Collections.Generic.Dictionary<string, int> strKeyLookup)
        {
            this.id = id;
            this.keyType = keyType;
            this.size = size;
            this.capacity = capacity;
            this.keys = keys;
            this.values = values;
            this.intKeyLookup = intKeyLookup;
            this.strKeyLookup = strKeyLookup;
        }
    }

}
