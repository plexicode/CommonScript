using CommonScript.Runtime.Internal;
using System;

namespace CommonScript.Runtime
{
    public enum TaskResultStatus
    {
        DONE,
        ERROR,
        SLEEP,
        SUSPEND,
    }

    public class TaskResult
    {
        private object resObj;
        internal TaskResult(object resObj)
        {
            this.resObj = resObj;
        }

        public int SleepMillis
        {
            get
            {
                return FunctionWrapper.PUBLIC_getTaskResultSleepAmount(this.resObj);
            }
        }

        public string[] ErrorMessage
        {
            get
            {
                return FunctionWrapper.PUBLIC_getTaskResultError(this.resObj, true);
            }
        }

        public TaskResultStatus Status
        {
            get
            {
                int status = FunctionWrapper.PUBLIC_getTaskResultStatus(this.resObj);
                switch (status)
                {
                    case 1: return TaskResultStatus.DONE;
                    case 2: return TaskResultStatus.ERROR;
                    case 3: return TaskResultStatus.SUSPEND;
                    case 4: return TaskResultStatus.SLEEP;
                }
                throw new NotImplementedException();
            }
        }
    }
}
