using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class TryDescriptor
    {
        public int tryPc;
        public int routerPc;
        public int finallyPc;
        public int finallyEnd;

        public TryDescriptor(int tryPc, int routerPc, int finallyPc, int finallyEnd)
        {
            this.tryPc = tryPc;
            this.routerPc = routerPc;
            this.finallyPc = finallyPc;
            this.finallyEnd = finallyEnd;
        }
    }

}
