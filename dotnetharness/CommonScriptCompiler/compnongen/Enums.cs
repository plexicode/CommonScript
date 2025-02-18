namespace CommonScript.Compiler
{
    internal enum EntityType
    {
        CLASS = 1,
        CONST = 2,
        CONSTRUCTOR = 3,
        ENUM = 4,
        FIELD = 5,
        FUNCTION = 6,
        NAMESPACE = 7,
        PROPERTY = 8,
        MODULE_REF = 9, // used in resolution process to refer to a module's public-exported members
        LAMBDA_ENTITY = 10, // despite being an expression, are exported like top level functions
    }
    
    internal enum ExpressionType
    {
        BASE = 1,
        BASE_CTOR_REFERENCE = 2,
        BINARY_OP = 3,
        BITWISE_NOT = 4,
        BOOL_CONST = 5,
        BOOLEAN_NOT = 6,
        CLASS_REFERENCE = 7,
        CONSTRUCTOR_INVOCATION = 8,
        CONSTRUCTOR_REFERENCE = 9,
        DICTIONARY_DEFINITION = 10,
        DOT_FIELD = 11,
        ENUM_CONST = 12, // generated in resolver, has both intVal and strVal set
        ENUM_REFERENCE = 13,
        EXTENSION_INVOCATION = 14,
        EXTENSION_REFERENCE = 15,
        FLOAT_CONST = 16,
        FUNCTION_INVOKE = 17,
        FUNCTION_REFERENCE = 18,
        IMPORT_REFERENCE = 19,
        INDEX = 20,
        INLINE_INCREMENT = 21,
        INTEGER_CONST = 22,
        LAMBDA = 23,
        LIST_DEFINITION = 24,
        NAMESPACE_REFERENCE = 32,
        NEGATIVE_SIGN = 25,
        NULL_CONST = 26,
        SLICE = 27,
        STRING_CONST = 28,
        TERNARY = 29,
        THIS = 30,
        TYPEOF = 33,
        VARIABLE = 31,

        MAX_VALUE = 34,
    }

    internal enum StatementType
    {
        ASSIGNMENT = 1,
        BREAK = 2,
        CONTINUE = 3,
        DO_WHILE_LOOP = 4,
        EXPRESSION_AS_STATEMENT = 5,
        FOR_LOOP = 6,
        FOR_EACH_LOOP = 7,
        IF_STATEMENT = 8,
        RETURN = 9,
        SWITCH_STATEMENT = 10,
        THROW = 11,
        TRY = 12,
        WHILE_LOOP = 13,
    }

    public enum FunctionType
    {
        FUNCTION = 1,
        METHOD = 2,
        CONSTRUCTOR = 3,
        STATIC_METHOD = 4,
        STATIC_CONSTRUCTOR = 5,
        LAMBDA = 6,
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
        public const int OP_BIN_OP_IS = 64;
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
        public const int OP_ENSURE_LIST = 66;
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
        public const int OP_PUSH_LAMBDA = 63;
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
        public const int OP_TYPEOF = 65;

        // NEXT: 67
    }
    
    internal enum TokenType
    {
        KEYWORD = 1,
        NAME = 2,
        PUNCTUATION = 3,
        STRING = 4,
        INTEGER = 5,
        HEX_INTEGER = 6,
        FLOAT = 7,
        ANNOTATION = 8,

        EOF = 9,
    }
    
    // Not an enum per se but the same sorta thing.
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
        public const int PARSE_FLOAT = 25;
        public const int RANDOM_FLOAT = 7;
        public const int SORT_START = 2;
        public const int SORT_END = 3;
        public const int SORT_GET_NEXT_CMP = 4;
        public const int SORT_PROCEED = 5;
        public const int TXT_BYTES_TO_STRING = 23;
        public const int TXT_IS_VALID_ENC = 22;
        public const int TXT_STRING_TO_BYTES = 24;
        public const int UNIX_TIME = 1;
        public const int XML_PARSE = 26;

        // Next ID: 27
    }
}
