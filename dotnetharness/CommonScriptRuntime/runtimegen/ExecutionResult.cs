using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ExecutionResult
    {
        public int type;
        public ExecutionTask task;
        public int sleepMillis;
        public string errorMessage;
        public System.Collections.Generic.List<string> stackTrace;

        public ExecutionResult(int type, ExecutionTask task, int sleepMillis, string errorMessage, System.Collections.Generic.List<string> stackTrace)
        {
            this.type = type;
            this.task = task;
            this.sleepMillis = sleepMillis;
            this.errorMessage = errorMessage;
            this.stackTrace = stackTrace;
        }
    }
}
