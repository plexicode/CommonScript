using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ExecutionContext
    {
        public string errMsg;
        public GlobalValues globalValues;
        public System.Collections.Generic.Dictionary<string, System.Func<object, object[], object>> extensions;
        public System.Collections.Generic.Dictionary<string, int> significantFunctions;
        public FunctionInfo[] functions;
        public Value[] functionsAsValues;
        public ClassInfo[] classes;
        public Value[] classRefValues;
        public EnumInfo[] enums;
        public ByteCodeRow[] byteCode;
        public TryDescriptor[] tryDescriptors;
        public Token[] tokensById;
        public string[] stringsById;
        public System.Collections.Generic.Dictionary<int, int>[] switchIntLookupsByPc;
        public System.Collections.Generic.Dictionary<string, int>[] switchStrLookupsByPc;
        public NameLookup nameLookup;
        public int nextTaskId;
        public int nextRefId;
        public System.Collections.Generic.Dictionary<int, ExecutionTask> tasks;
        public object appCtx;

        public ExecutionContext(string errMsg, GlobalValues globalValues, System.Collections.Generic.Dictionary<string, System.Func<object, object[], object>> extensions, System.Collections.Generic.Dictionary<string, int> significantFunctions, FunctionInfo[] functions, Value[] functionsAsValues, ClassInfo[] classes, Value[] classRefValues, EnumInfo[] enums, ByteCodeRow[] byteCode, TryDescriptor[] tryDescriptors, Token[] tokensById, string[] stringsById, System.Collections.Generic.Dictionary<int, int>[] switchIntLookupsByPc, System.Collections.Generic.Dictionary<string, int>[] switchStrLookupsByPc, NameLookup nameLookup, int nextTaskId, int nextRefId, System.Collections.Generic.Dictionary<int, ExecutionTask> tasks, object appCtx)
        {
            this.errMsg = errMsg;
            this.globalValues = globalValues;
            this.extensions = extensions;
            this.significantFunctions = significantFunctions;
            this.functions = functions;
            this.functionsAsValues = functionsAsValues;
            this.classes = classes;
            this.classRefValues = classRefValues;
            this.enums = enums;
            this.byteCode = byteCode;
            this.tryDescriptors = tryDescriptors;
            this.tokensById = tokensById;
            this.stringsById = stringsById;
            this.switchIntLookupsByPc = switchIntLookupsByPc;
            this.switchStrLookupsByPc = switchStrLookupsByPc;
            this.nameLookup = nameLookup;
            this.nextTaskId = nextTaskId;
            this.nextRefId = nextRefId;
            this.tasks = tasks;
            this.appCtx = appCtx;
        }
    }
}
