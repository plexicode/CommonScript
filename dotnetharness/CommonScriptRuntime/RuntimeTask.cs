using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonScript.Runtime.Internal;

namespace CommonScript.Runtime
{
    public class RuntimeTask
    {
        private object ecObj;
        private object taskObj;

        internal RuntimeTask(object ecObj, object taskObj)
        {
            this.ecObj = ecObj;
            this.taskObj = taskObj;
        }

        public TaskResult Resume()
        {
            object result = FunctionWrapper.RunInterpreter((ExecutionTask)this.taskObj);
            return new TaskResult(result);
        }
    }
}
