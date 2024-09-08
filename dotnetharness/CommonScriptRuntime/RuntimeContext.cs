using System.Collections.Generic;
using System.Linq;
using CommonScript.Runtime.Internal;

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
                null);

            string initError = FunctionWrapper.PUBLIC_getExecutionContextError(this.execContext);
            // TODO: this may not be a RuntimeError
            if (initError != null) throw new RuntimeException("Could not initialize executable: " + initError);

            // This needs to be in pastel-generated code.
            object mainTaskObj = FunctionWrapper.createMainTask((ExecutionContext)this.execContext, cliArgs.ToArray());
            this.MainTask = new RuntimeTask(this.execContext, mainTaskObj);
        }

        public TaskResult StartMainTask()
        {
            return this.MainTask.Resume();
        }
    }
}
