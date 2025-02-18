using System.Collections.Generic;

namespace CommonScript.Compiler
{
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
            { "parse_float", SpecialActionCodes.PARSE_FLOAT },
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
            { "xml_parse", SpecialActionCodes.XML_PARSE },
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
            { "parse_float", 1 },
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
            { "xml_parse", 2 },
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
