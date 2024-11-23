using CommonScript.Runtime;
using System;
using System.Collections.Generic;

namespace CommonScriptCli
{
#pragma warning disable CS8603 // Possible null reference return.
    internal class VanillaExtensionSet
    {
        private Dictionary<string, Func<object, object[], object>> lookup;
        private CanonicalEventLoop eventLoop;
        private RuntimeContext rtCtx;

        public VanillaExtensionSet()
        {
            this.lookup = this.BuildExtensionSet();
        }

        public void SetEventLoop(CanonicalEventLoop eventLoop, RuntimeContext rtCtx)
        {
            this.eventLoop = eventLoop;
            this.rtCtx = rtCtx;
        }

        public Dictionary<string, Func<object, object[], object>> GetExtensionsLookup()
        {
            return new Dictionary<string, Func<object, object[], object>>(this.lookup);
        }

        public string[] ExtensionIds { get { return [.. this.lookup.Keys]; } }

        public Dictionary<string, Func<object, object[], object>> BuildExtensionSet()
        {
            Dictionary<string, Func<object, object[], object>> output = [];

            output["io_stdout"] = (object task, object[] args) =>
            {
                string val = ValueConverter.RTValueToReadableString(args[0]);
                System.Console.WriteLine(val);
                return null;
            };

            output["delay_invoke"] = (object task, object[] args) =>
            {
                object fnPtr = args[0];
                RuntimeTask rtTask = this.rtCtx.CreateTaskWithFunctionPointer(fnPtr);
                double delaySec = ValueConverter.RTValueToFloat(args[1]);
                this.eventLoop.QueueTask(rtTask, delaySec);
                return null;
            };

            output["sleep"] = (object task, object[] args) =>
            {
                double delaySec = ValueConverter.RTValueToFloat(args[0]);
                RuntimeTask rtTask = this.rtCtx.GetTaskFromNativePtr(task);
                rtTask.RequestTimedSuspend((int)(delaySec * 1000));
                return null;
            };

            return output;
        }
    }
#pragma warning restore CS8603 // Possible null reference return.
}
