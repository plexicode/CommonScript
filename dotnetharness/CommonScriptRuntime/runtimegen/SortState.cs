using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class SortState
    {
        public SortNode output;
        public Value leftCmp;
        public Value rightCmp;
        public bool isDone;
        public SortTask taskQueue;
        public Value copyBackBuffer;

        public SortState(SortNode output, Value leftCmp, Value rightCmp, bool isDone, SortTask taskQueue, Value copyBackBuffer)
        {
            this.output = output;
            this.leftCmp = leftCmp;
            this.rightCmp = rightCmp;
            this.isDone = isDone;
            this.taskQueue = taskQueue;
            this.copyBackBuffer = copyBackBuffer;
        }
    }
}
