using CommonScript.Runtime;
using System;
using System.Collections.Generic;

namespace CommonScriptCli
{
#pragma warning disable CS8603 // Possible null reference return.
    internal static class VanillaExtensions
    {
        public static Dictionary<string, Func<object, object[], object>> BuildExtensionSet()
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
                throw new NotImplementedException();
            };

            output["sleep"] = (object task, object[] args) =>
            {
                throw new NotImplementedException();
            };

            return output;
        }
    }
#pragma warning restore CS8603 // Possible null reference return.
}
