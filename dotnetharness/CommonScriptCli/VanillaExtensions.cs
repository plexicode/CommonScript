using CommonScript.Runtime;
using System;
using System.Collections.Generic;

namespace CommonScriptCli
{
    internal static class VanillaExtensions
    {
        public static Dictionary<string, Func<object, object[], object>> BuildExtensionSet()
        {
            Dictionary<string, Func<object, object[], object>> output = new Dictionary<string, Func<object, object[], object>>();

            output["io_stdout"] = (object task, object[] args) =>
            {
                string val = ValueConverter.RTValueToReadableString(args[0]);
                System.Console.WriteLine(val);
                return null;
            };

            return output;
        }
    }
}
