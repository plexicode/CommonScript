using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class XmlParseContext
    {
        public int line;
        public int col;
        public bool hasError;
        public string errorMessage;
        public int index;
        public int[] chars;
        public int length;
        public System.Collections.Generic.List<Value> buffer;
        public GlobalValues globals;

        public XmlParseContext(int line, int col, bool hasError, string errorMessage, int index, int[] chars, int length, System.Collections.Generic.List<Value> buffer, GlobalValues globals)
        {
            this.line = line;
            this.col = col;
            this.hasError = hasError;
            this.errorMessage = errorMessage;
            this.index = index;
            this.chars = chars;
            this.length = length;
            this.buffer = buffer;
            this.globals = globals;
        }
    }
}
