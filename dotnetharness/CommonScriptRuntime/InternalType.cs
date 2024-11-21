using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Runtime
{
    internal enum InternalType
    {
        // copied directly from runtime/src/constants.pst
        UNKNOWN = 0,

        NULL = 1,
        BOOLEAN = 2,
        INTEGER = 3,
        FLOAT = 4,
        STRING = 5,
        ENUM = 6,
        BIG_INT = 7,
        BYTEBUF = 8,
        LIST = 9,
        DICTIONARY = 10,
        FUNCTION = 11,
        INSTANCE = 12,
        CLASS = 13,
        NATIVE_HANDLE = 14,
        GENERATOR = 15,
    }
}
