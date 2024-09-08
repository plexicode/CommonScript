using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Runtime
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string msg) : base(msg) { }
    }
}
