using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Compiler
{
    internal class CompiledModule
    {
        public string id { get; set; }
        public Dictionary<string, string> codeFiles;
        public Dictionary<string, string> textResources;
        public Dictionary<string, AbstractEntity> nestedEntities;
        public Dictionary<string, AbstractEntity> flattenedEntities;

        public CompiledModule(string id)
        {
            this.id = id;
            this.codeFiles = new Dictionary<string, string>();
            this.textResources = new Dictionary<string, string>();
        }
    }

    internal class CompiledFunctionMetadata
    {
        public string fqName { get; set; }
        public int argc { get; set; }
        public int argcMin { get; set; }
        public int pc { get; set; }
    }

    internal class CompiledByteCodeRow
    {
        public int op { get; set; }
        public int[] args { get; set; }
        public string stringArg { get; set; }
        public string stringId { get; set; }
        public Token token { get; set; }
    }
}
