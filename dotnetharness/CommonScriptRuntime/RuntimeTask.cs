using CommonScript.Runtime.Internal;

namespace CommonScript.Runtime
{
    public class RuntimeTask
    {
        private object ecObj;
        private object taskObj;

        internal RuntimeTask(object taskObj)
        {
            this.ecObj = FunctionWrapper.PUBLIC_getExecutionContextFromTask(taskObj);
            this.taskObj = taskObj;
        }

        public TaskResult Resume()
        {
            object result = FunctionWrapper.RunInterpreter((ExecutionTask)this.taskObj);
            return new TaskResult(result);
        }

        public void RequestTimedSuspend(int millis)
        {
            FunctionWrapper.PUBLIC_requestTaskSuspension(this.taskObj, true, millis);
        }

        public void RequestSuspend()
        {
            FunctionWrapper.PUBLIC_requestTaskSuspension(this.taskObj, false, 0);
        }
    }
}
