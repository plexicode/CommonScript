using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Runtime
{
    public class TaskResult
    {
        private object resObj;
        internal TaskResult(object resObj)
        {
            this.resObj = resObj;
        }
    }
}
