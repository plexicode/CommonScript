using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class BuiltinUtil
    {
        public static string GetBuiltinSource()
        {
            return GetSourceFilesFor("builtins");
        }

        public static string GetSourceFilesFor(string moduleId)
        {
            return CommonScript.Compiler.Internal.FunctionWrapper.GetBuiltin(moduleId);
        }

        public static bool IsBuiltInModule(string moduleId)
        {
            return moduleId != "builtins" &&
                   CommonScript.Compiler.Internal.FunctionWrapper.GetBuiltinRawStoredString(moduleId) != null;
        }
    }
}
