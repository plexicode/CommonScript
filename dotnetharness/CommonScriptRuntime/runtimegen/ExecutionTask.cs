using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class ExecutionTask
    {
        public int taskId;
        public ExecutionContext execCtx;
        public StackFrame stack;
        public bool suspendRequested;
        public int sleepMillis;
        public Value[] valueStack;
        public StackFrame framePool;

        public ExecutionTask(int taskId, ExecutionContext execCtx, StackFrame stack, bool suspendRequested, int sleepMillis, Value[] valueStack, StackFrame framePool)
        {
            this.taskId = taskId;
            this.execCtx = execCtx;
            this.stack = stack;
            this.suspendRequested = suspendRequested;
            this.sleepMillis = sleepMillis;
            this.valueStack = valueStack;
            this.framePool = framePool;
        }
    }

}
