namespace CommonScript.Compiler
{
    internal class SpecialActionCodes
    {
        public const int CMP = 6;
        public const int MATH_ARCCOS = 8;
        public const int MATH_ARCSIN = 9;
        public const int MATH_ARCTAN = 10;
        public const int MATH_COS = 11;
        public const int MATH_LOG = 12;
        public const int MATH_SIN = 13;
        public const int MATH_TAN = 14;
        public const int RANDOM_FLOAT = 7;
        public const int SORT_START = 2;
        public const int SORT_END = 3;
        public const int SORT_GET_NEXT_CMP = 4;
        public const int SORT_PROCEED = 5;
        public const int UNIX_TIME = 1;
    }

    internal class OpCodes
    {
        public const int OP_BREAK_DUMMY = -1;
        public const int OP_CONTINUE_DUMMY = -2;

        public const int OP_ASSIGN_FIELD = 1;
        public const int OP_ASSIGN_INDEX = 2;
        public const int OP_ASSIGN_VAR = 3;
        public const int OP_BIN_OP = 4;
        public const int OP_BIN_OP_ADD = 5;
        public const int OP_BIN_OP_BIT_MATH = 6;
        public const int OP_BIN_OP_COMPARE = 7;
        public const int OP_BIN_OP_EQUAL = 8;
        public const int OP_BIN_OP_MATH = 9;
        public const int OP_BITWISE_NOT = 10;
        public const int OP_BOOLEAN_NOT = 11;
        public const int OP_BUILD_LIST = 12;
        public const int OP_BUILD_DICT = 13;
        public const int OP_CTOR_REF = 14;
        public const int OP_DOT_FIELD = 15;
        public const int OP_ENSURE_BOOL = 16;
        public const int OP_ENSURE_INT_OR_STRING = 17;
        public const int OP_ENSURE_INT = 18;
        public const int OP_ENSURE_STRING = 19;
        public const int OP_EXT_INVOKE = 20;
        public const int OP_FUNCTION_INVOKE = 21;
        public const int OP_INDEX = 22;
        public const int OP_INT_INCR = 23;
        public const int OP_JUMP = 24;
        public const int OP_MATH_FLOOR = 25;
        public const int OP_NEGATIVE_SIGN = 26;
        public const int OP_POP = 27;
        public const int OP_POP_AND_JUMP_IF_FALSE = 28;
        public const int OP_POP_AND_JUMP_IF_TRUE = 29;
        public const int OP_POP_IF_FALSE_OR_JUMP = 30;
        public const int OP_POP_IF_NULL_OR_JUMP = 31;
        public const int OP_POP_IF_TRUE_OR_JUMP = 32;
        public const int OP_PUSH_ARG = 33;
        public const int OP_PUSH_ARG_IF_PRESENT = 34;
        public const int OP_PUSH_BASE_CTOR = 35;
        public const int OP_PUSH_BOOL = 36;
        public const int OP_PUSH_CLASS_REF = 37;
        public const int OP_PUSH_FLOAT = 38;
        public const int OP_PUSH_FUNC_PTR = 39;
        public const int OP_PUSH_INT = 40;
        public const int OP_PUSH_NULL = 41;
        public const int OP_PUSH_STRING = 42;
        public const int OP_PUSH_THIS = 43;
        public const int OP_PUSH_VALUE = 44;
        public const int OP_PUSH_VAR = 45;
        public const int OP_RETURN = 46;
        public const int OP_SLICE = 47;
        public const int OP_SPECIAL_ACTION = 48;
        public const int OP_STACK_DO_SI_DO_4A = 49;
        public const int OP_STACK_DO_SI_DUP_1 = 50; // AB --> BBA
        public const int OP_STACK_DO_SI_DUP_2 = 51; // ABC --> BCA
        public const int OP_STACK_DUPLICATE = 52;
        public const int OP_STACK_DUPLICATE_2 = 53;
        public const int OP_SWITCH_ADD = 54;
        public const int OP_SWITCH_BUILD = 55;
        public const int OP_SWITCH_FINALIZE = 56;
        public const int OP_SWITCH_INT = 57;
        public const int OP_SWITCH_STRING = 58;
        public const int OP_THROW = 59;
        public const int OP_TRY_FINALLY_END = 60;
        public const int OP_TRY_CATCH_ROUTER = 61;
        public const int OP_TRY_INFO = 62;

        // NEXT: 63
    }
}
