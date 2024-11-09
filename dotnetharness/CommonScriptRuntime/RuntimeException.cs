using System;

namespace CommonScript.Runtime
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string msg) : base(msg) { }
    }
}
