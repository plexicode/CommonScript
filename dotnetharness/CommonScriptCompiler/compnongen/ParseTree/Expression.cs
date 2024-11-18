using System.Collections.Generic;

namespace CommonScript.Compiler
{
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
        VARIABLE = 31,

        MAX_VALUE = 33,
    }

    internal class Expression
    {
        public Token firstToken;
        public ExpressionType type;
        public Expression root = null;
        public Expression left = null;
        public Expression right = null;
        public Token opToken = null;
        public bool boolVal = false;
        public string strVal = null;
        public int intVal = 0;
        public double floatVal = 0.0;
        public object objPtr = null;
        public Expression[] args = null;
        public Expression[] keys = null;
        public Expression[] values = null;
        public Token[] argNames = null;
        public Statement[] nestedCode = null;
        // Remember to add any new fields to BOTH the constructor and to the clone method.

        private Expression(Token firstToken, ExpressionType type)
        {
            this.firstToken = firstToken;
            this.type = type;
        }

        public static Expression cloneExpressionWithNewToken(Token token, Expression other)
        {
            Expression cloned = new Expression(token, other.type);
            cloned.root = other.root;
            cloned.left = other.left;
            cloned.right = other.right;
            cloned.opToken = other.opToken;
            cloned.boolVal = other.boolVal;
            cloned.strVal = other.strVal;
            cloned.intVal = other.intVal;
            cloned.floatVal = other.floatVal;
            cloned.args = other.args;
            cloned.keys = other.keys;
            cloned.values = other.values;
            cloned.argNames = other.argNames;
            cloned.nestedCode = other.nestedCode;
            return cloned;
        }

        public static Expression createNullConstant(Token nullToken)
        {
            return new Expression(nullToken, ExpressionType.NULL_CONST);
        }

        public static Expression createBoolConstant(Token token, bool val)
        {
            Expression expr = new Expression(token, ExpressionType.BOOL_CONST);
            expr.boolVal = val;
            return expr;
        }

        public static Expression createIntegerConstant(Token token, int val)
        {
            Expression expr = new Expression(token, ExpressionType.INTEGER_CONST);
            expr.intVal = val;
            return expr;
        }

        public static Expression createFloatConstant(Token token, double val)
        {
            Expression expr = new Expression(token, ExpressionType.FLOAT_CONST);
            expr.floatVal = val;
            return expr;
        }

        public static Expression createStringConstant(Token token, string val)
        {
            Expression expr = new Expression(token, ExpressionType.STRING_CONST);
            expr.strVal = val;
            return expr;
        }

        public static Expression createLambda(Token firstToken, Token[] argNameTokens, Expression[] argDefaultValues, Token arrowToken, Statement[] code)
        {
            Expression expr = new Expression(firstToken, ExpressionType.LAMBDA);
            expr.argNames = argNameTokens;
            expr.values = argDefaultValues;
            expr.opToken = arrowToken;
            expr.nestedCode = code;
            return expr;
        }

        public static Expression createVariable(Token token, string varName)
        {
            Expression expr = new Expression(token, ExpressionType.VARIABLE);
            expr.strVal = varName;
            return expr;
        }

        public static Expression createThisReference(Token token)
        {
            return new Expression(token, ExpressionType.THIS);
        }

        public static Expression createBaseReference(Token token)
        {
            return new Expression(token, ExpressionType.BASE);
        }

        public static Expression createBaseCtorReference(Token token)
        {
            return new Expression(token, ExpressionType.BASE_CTOR_REFERENCE);
        }

        public static Expression createConstructorReference(Token newToken, Expression nameChain)
        {
            Expression ctor = new Expression(newToken, ExpressionType.CONSTRUCTOR_REFERENCE);
            ctor.root = nameChain;
            return ctor;
        }

        public static Expression createConstructorInvocation(Token firstToken, AbstractEntity classDef, Token invokeToken, Expression[] args)
        {
            Expression ctorInvoke = new Expression(firstToken, ExpressionType.CONSTRUCTOR_INVOCATION);
            ctorInvoke.objPtr = classDef;
            ctorInvoke.args = args;
            ctorInvoke.opToken = invokeToken;
            return ctorInvoke;
        }

        public static Expression createListDefinition(Token openList, Expression[] items)
        {
            Expression expr = new Expression(openList, ExpressionType.LIST_DEFINITION);
            expr.values = items;
            return expr;
        }

        public static Expression createDictionaryDefinition(Token openDict, Expression[] keys, Expression[] values)
        {
            Expression expr = new Expression(openDict, ExpressionType.DICTIONARY_DEFINITION);
            expr.keys = keys;
            expr.values = values;
            for (int i = 0; i < keys.Length; i++)
            {
                Expression key = keys[i];
                if (key.type != ExpressionType.STRING_CONST && key.type != ExpressionType.INTEGER_CONST)
                {
                    Errors.ThrowError(key.firstToken, "Only string and integer constants can be used as dictionary keys");
                }
            }
            return expr;
        }

        public static Expression createExtensionReference(Token prefixToken, string name)
        {
            Expression extRef = new Expression(prefixToken, ExpressionType.EXTENSION_REFERENCE);
            extRef.strVal = name;
            return extRef;
        }

        public static Expression createExtensionInvocation(Token firstToken, string name, Expression[] args)
        {
            Expression extInvoke = new Expression(firstToken, ExpressionType.EXTENSION_INVOCATION);
            extInvoke.strVal = name;
            extInvoke.args = args;
            return extInvoke;
        }

        public static Expression createTernary(Expression condition, Token op, Expression trueValue, Expression falseValue)
        {
            Expression ternary = new Expression(condition.firstToken, ExpressionType.TERNARY);
            ternary.root = condition;
            ternary.opToken = op;
            ternary.left = trueValue;
            ternary.right = falseValue;
            return ternary;
        }

        public static Expression createBinaryOp(Expression left, Token op, Expression right)
        {
            Expression pair = new Expression(left.firstToken, ExpressionType.BINARY_OP);
            pair.opToken = op;
            pair.left = left;
            pair.right = right;
            return pair;
        }

        public static Expression createInlineIncrement(Token firstToken, Expression root, Token incrementOp, bool isPrefix)
        {
            Expression expr = new Expression(firstToken, ExpressionType.INLINE_INCREMENT);
            expr.opToken = incrementOp;
            expr.root = root;
            expr.boolVal = isPrefix;

            switch (root.type)
            {
                case ExpressionType.DOT_FIELD:
                case ExpressionType.INDEX:
                case ExpressionType.VARIABLE:
                    // These are fine.
                    break;

                default:
                    Errors.ThrowError(incrementOp, "The '" + incrementOp.Value + "' operator is not allowed on this type of expression.");
                    break;
            }

            return expr;
        }

        public static Expression createNegatePrefix(Token opToken, Expression root)
        {
            ExpressionType t = ExpressionType.NEGATIVE_SIGN;
            if (opToken.Value == "!") t = ExpressionType.BOOLEAN_NOT;
            if (opToken.Value == "~") t = ExpressionType.BITWISE_NOT;
            Expression expr = new Expression(opToken, t);
            expr.opToken = opToken;
            expr.root = root;
            return expr;
        }

        public static Expression createDotField(Expression root, Token dotToken, string name)
        {
            Expression df = new Expression(root.firstToken, ExpressionType.DOT_FIELD);
            df.root = root;
            df.opToken = dotToken;
            df.strVal = name;
            return df;
        }

        public static string[] DotField_getVariableRootedDottedChain(Expression outermostDotField, string errorMessage)
        {
            List<string> chain = [outermostDotField.strVal];
            Expression walker = outermostDotField.root;
            while (walker != null)
            {
                chain.Add(walker.strVal);
                if (walker.type == ExpressionType.DOT_FIELD)
                {
                    walker = walker.root;
                }
                else if (walker.type == ExpressionType.VARIABLE)
                {
                    walker = null;
                }
                else if (errorMessage != null)
                {
                    Errors.ThrowError(walker.firstToken, errorMessage);
                }
                else
                {
                    return null;
                }
            }
            chain.Reverse();
            return [.. chain];
        }

        public static Expression createFunctionInvocation(Expression root, Token parenToken, Expression[] args)
        {
            Expression funcInvoke = new Expression(root.firstToken, ExpressionType.FUNCTION_INVOKE);
            funcInvoke.root = root;
            funcInvoke.opToken = parenToken;
            funcInvoke.args = args;
            return funcInvoke;
        }

        public static Expression createBracketIndex(Expression root, Token bracketToken, Expression index)
        {
            Expression bracketIndex = new Expression(root.firstToken, ExpressionType.INDEX);
            bracketIndex.root = root;
            bracketIndex.opToken = bracketToken;
            bracketIndex.right = index;
            return bracketIndex;
        }

        public static Expression createClassReference(Token firstToken, AbstractEntity classDef)
        {
            Expression classRef = new Expression(firstToken, ExpressionType.CLASS_REFERENCE);
            classRef.objPtr = classDef;
            classRef.boolVal = false; // verified safe usage
            return classRef;
        }

        public static Expression createEnumConstant(Token firstToken, AbstractEntity enumDef, string name, int value)
        {
            Expression enumConst = new Expression(firstToken, ExpressionType.ENUM_CONST);
            enumConst.objPtr = enumDef;
            enumConst.strVal = name;
            enumConst.intVal = value;
            return enumConst;
        }

        public static Expression createEnumReference(Token firstToken, AbstractEntity enumDef)
        {
            Expression enumRef = new Expression(firstToken, ExpressionType.ENUM_REFERENCE);
            enumRef.objPtr = enumDef;
            return enumRef;
        }

        public static Expression createFunctionReference(Token firstToken, string name, AbstractEntity funcDef)
        {
            Expression funcRef = new Expression(firstToken, ExpressionType.FUNCTION_REFERENCE);
            funcRef.strVal = name;
            funcRef.objPtr = funcDef;
            return funcRef;
        }

        public static Expression createImportReference(Token firstToken, ImportStatement importStatement)
        {
            Expression impRef = new Expression(firstToken, ExpressionType.IMPORT_REFERENCE);
            impRef.objPtr = importStatement;
            return impRef;
        }

        public static Expression createNamespaceReference(Token firstToken, AbstractEntity nsDef)
        {
            Expression nsRef = new Expression(firstToken, ExpressionType.NAMESPACE_REFERENCE);
            nsRef.objPtr = nsDef;
            return nsRef;
        }

        public static Expression createSliceExpression(
            Expression rootExpression,
            Token bracketToken,
            Expression start,
            Expression end,
            Expression step)
        {
            Expression sliceExpr = new Expression(rootExpression.firstToken, ExpressionType.SLICE);
            sliceExpr.root = rootExpression;
            sliceExpr.opToken = bracketToken;
            sliceExpr.args = new Expression[] { start, end, step };
            return sliceExpr;
        }
    }
}
