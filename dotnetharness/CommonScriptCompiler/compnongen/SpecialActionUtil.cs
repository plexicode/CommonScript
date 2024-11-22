using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class SpecialActionCodes
    {
        public const int B64_FROM_BYTES = 16;
        public const int B64_TO_BYTES = 17;
        public const int CMP = 6;
        public const int JSON_PARSE = 20;
        public const int JSON_SERIALIZE = 21;
        public const int MATH_ARCCOS = 8;
        public const int MATH_ARCSIN = 9;
        public const int MATH_ARCTAN = 10;
        public const int MATH_COS = 11;
        public const int MATH_LOG = 12;
        public const int MATH_SIN = 13;
        public const int MATH_TAN = 14;
        public const int PARSE_INT = 15;
        public const int RANDOM_FLOAT = 7;
        public const int SORT_START = 2;
        public const int SORT_END = 3;
        public const int SORT_GET_NEXT_CMP = 4;
        public const int SORT_PROCEED = 5;
        public const int TXT_BYTES_TO_STRING = 23;
        public const int TXT_IS_VALID_ENC = 22;
        public const int TXT_STRING_TO_BYTES = 24;
        public const int UNIX_TIME = 1;

        // Next ID: 25
    }

    internal static class SpecialActionUtil
    {
        private static readonly Dictionary<string, int> SPECIAL_ACTION_BY_FUNC_NAME = new Dictionary<string, int>()
        {
            { "b64_from_bytes", SpecialActionCodes.B64_FROM_BYTES },
            { "b64_to_bytes", SpecialActionCodes.B64_TO_BYTES },
            { "cmp", SpecialActionCodes.CMP },
            { "json_parse", SpecialActionCodes.JSON_PARSE },
            { "json_serialize", SpecialActionCodes.JSON_SERIALIZE },
            { "math_arccos", SpecialActionCodes.MATH_ARCCOS },
            { "math_arcsin", SpecialActionCodes.MATH_ARCSIN },
            { "math_arctan", SpecialActionCodes.MATH_ARCTAN },
            { "math_cos", SpecialActionCodes.MATH_COS },
            { "math_floor", -1 }, // compiled as a main op
            { "math_log", SpecialActionCodes.MATH_LOG },
            { "math_sin", SpecialActionCodes.MATH_SIN },
            { "math_tan", SpecialActionCodes.MATH_TAN },
            { "parse_int", SpecialActionCodes.PARSE_INT },
            { "random_float", SpecialActionCodes.RANDOM_FLOAT },
            { "sort_end", SpecialActionCodes.SORT_END },
            { "sort_get_next_cmp", SpecialActionCodes.SORT_GET_NEXT_CMP },
            { "sort_proceed", SpecialActionCodes.SORT_PROCEED },
            { "sort_start", SpecialActionCodes.SORT_START },
            { "txt_bytes_to_string", SpecialActionCodes.TXT_BYTES_TO_STRING },
            { "txt_is_valid_enc", SpecialActionCodes.TXT_IS_VALID_ENC },
            { "txt_string_to_bytes", SpecialActionCodes.TXT_STRING_TO_BYTES},
            { "unix_time", SpecialActionCodes.UNIX_TIME },
        };

        private static readonly Dictionary<string, int> SPECIAL_ACTION_ARGC = new Dictionary<string, int>()
        {
            { "b64_from_bytes", 2},
            { "b64_to_bytes", 1 },
            { "cmp", 2},
            { "json_parse", 2 },
            { "json_serialize", 2 },
            { "math_arccos", 1 },
            { "math_arcsin", 1 },
            { "math_arctan", 2 },
            { "math_cos", 1},
            { "math_floor", 1 },
            { "math_log", 2 },
            { "math_sin", 1 },
            { "math_tan", 1 },
            { "parse_int", 1 },
            { "random_float", 0 },
            { "sort_end", 1 },
            { "sort_get_next_cmp", 2 },
            { "sort_proceed", 2 },
            { "sort_start", 2 },
            { "txt_bytes_to_string", 2 },
            { "txt_is_valid_enc", 1 },
            { "txt_string_to_bytes", 2 },
            { "unix_time", 1 },
        };

        public static bool IsSpecialActionAndNotExtension(string name)
        {
            return SPECIAL_ACTION_BY_FUNC_NAME.ContainsKey(name);
        }

        public static int GetSpecialActionOpCode(string name)
        {
            return SPECIAL_ACTION_BY_FUNC_NAME[name];
        }

        public static int GetSpecialActionArgc(string name)
        {
            return SPECIAL_ACTION_ARGC[name];
        }
    }
}
