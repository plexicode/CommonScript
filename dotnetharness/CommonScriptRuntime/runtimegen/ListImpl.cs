using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ListImpl
    {
        public int id;
        public int length;
        public int capacity;
        public Value[] items;

        public ListImpl(int id, int length, int capacity, Value[] items)
        {
            this.id = id;
            this.length = length;
            this.capacity = capacity;
            this.items = items;
        }
    }

}
