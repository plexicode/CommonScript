using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class SpecialActionUtil
    {
        public System.Collections.Generic.Dictionary<string, int> SPECIAL_ACTION_BY_FUNC_NAME;
        public System.Collections.Generic.Dictionary<string, int> SPECIAL_ACTION_ARGC;

        public SpecialActionUtil(System.Collections.Generic.Dictionary<string, int> SPECIAL_ACTION_BY_FUNC_NAME, System.Collections.Generic.Dictionary<string, int> SPECIAL_ACTION_ARGC)
        {
            this.SPECIAL_ACTION_BY_FUNC_NAME = SPECIAL_ACTION_BY_FUNC_NAME;
            this.SPECIAL_ACTION_ARGC = SPECIAL_ACTION_ARGC;
        }
    }
}
