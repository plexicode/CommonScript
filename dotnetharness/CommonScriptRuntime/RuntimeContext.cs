using CommonScript.Runtime.Internal;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Runtime
{
    public class RuntimeContext
    {
        static RuntimeContext()
        {
            FunctionWrapper.PST_RegisterExtensibleCallback("hardCrash", (object[] args) =>
            {
                string msg = args[0].ToString();
                throw new RuntimeException(msg);
            });

            FunctionWrapper.PST_RegisterExtensibleCallback("jsonParse", (object[] args) =>
            {
                string rawValue = args[0].ToString();
                List<object> bufOut = (List<object>)args[1];
                JsonUtil.Parse(rawValue, bufOut);
                return null;
            });

            FunctionWrapper.PST_RegisterExtensibleCallback("jsonSerialize", (object[] args) =>
            {
                return JsonUtil.Serialize((Value)args[0], (bool)args[1]);
            });
        }

        private object execContext;

        public RuntimeTask MainTask { get; private set; }
        private bool mainStarted = false;

        public RuntimeContext(
            string langId,
            string version,
            byte[] executableBytes,
            Dictionary<string, System.Func<object, object[], object>> extensions,
            IList<string> cliArgs)
        {
            int[] byteInts = executableBytes.Select(b => 0xFF & (int)b).ToArray();
            this.execContext = FunctionWrapper.PUBLIC_initializeExecutionContext(
                byteInts,
                new Dictionary<string, System.Func<object, object[], object>>(extensions),
                null, 
                o => null);

            string initError = FunctionWrapper.PUBLIC_getExecutionContextError(this.execContext);
            // TODO: this may not be a RuntimeError
            if (initError != null) throw new RuntimeException("Could not initialize executable: " + initError);

            // This needs to be in pastel-generated code.
            object mainTaskObj = FunctionWrapper.createMainTask((ExecutionContext)this.execContext, cliArgs.ToArray());
            this.MainTask = new RuntimeTask(mainTaskObj);
        }

        public bool HasActiveTasks
        {
            get
            {
                return ((ExecutionContext)this.execContext).tasks.Count > 0;
            }
        }

        public TaskResult StartMainTask()
        {
            return this.MainTask.Resume();
        }

        public RuntimeTask CreateTaskWithFunctionPointer(object fp)
        {
            object taskObj = FunctionWrapper.PUBLIC_createTaskForFunction(this.execContext, fp);
            return new RuntimeTask(taskObj);
        }

        public RuntimeTask GetTaskFromNativePtr(object rtTask)
        {
            return new RuntimeTask(rtTask);
        }
    }
}
