using System.Collections.Generic;
using CommonScript.Compiler.Internal;

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
        TYPEOF = 33,
        VARIABLE = 31,

        MAX_VALUE = 34,
    }

    internal class ExpressionUtil
    {
        public static Expression cloneExpressionWithNewToken(Token token, Expression other)
        {
            Expression cloned = FunctionWrapper.Expression_new(token, other.type);
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
            return FunctionWrapper.Expression_new(nullToken, (int) ExpressionType.NULL_CONST);
        }

        public static Expression createBoolConstant(Token token, bool val)
        {
            Expression expr = FunctionWrapper.Expression_new(token, (int) ExpressionType.BOOL_CONST);
            expr.boolVal = val;
            return expr;
        }

        public static Expression createIntegerConstant(Token token, int val)
        {
            Expression expr = FunctionWrapper.Expression_new(token, (int) ExpressionType.INTEGER_CONST);
            expr.intVal = val;
            return expr;
        }

        public static Expression createFloatConstant(Token token, double val)
        {
            Expression expr = FunctionWrapper.Expression_new(token, (int) ExpressionType.FLOAT_CONST);
            expr.floatVal = val;
            return expr;
        }

        public static Expression createStringConstant(Token token, string val)
        {
            Expression expr = FunctionWrapper.Expression_new(token, (int) ExpressionType.STRING_CONST);
            expr.strVal = val;
            return expr;
        }

        public static Expression createLambda(Token firstToken, Token[] argNameTokens, Expression[] argDefaultValues, Token arrowToken, Statement[] code)
        {
            Expression expr = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.LAMBDA);
            expr.argNames = argNameTokens;
            expr.values = argDefaultValues;
            expr.opToken = arrowToken;
            expr.nestedCode = code;
            return expr;
        }

        public static Expression createVariable(Token token, string varName)
        {
            Expression expr = FunctionWrapper.Expression_new(token, (int) ExpressionType.VARIABLE);
            expr.strVal = varName;
            return expr;
        }

        public static Expression createThisReference(Token token)
        {
            return FunctionWrapper.Expression_new(token, (int) ExpressionType.THIS);
        }

        public static Expression createBaseReference(Token token)
        {
            return FunctionWrapper.Expression_new(token, (int) ExpressionType.BASE);
        }

        public static Expression createBaseCtorReference(Token token)
        {
            return FunctionWrapper.Expression_new(token, (int) ExpressionType.BASE_CTOR_REFERENCE);
        }

        public static Expression createConstructorReference(Token newToken, Expression nameChain)
        {
            Expression ctor = FunctionWrapper.Expression_new(newToken, (int) ExpressionType.CONSTRUCTOR_REFERENCE);
            ctor.root = nameChain;
            return ctor;
        }

        public static Expression createConstructorInvocation(Token firstToken, AbstractEntity classDef, Token invokeToken, Expression[] args)
        {
            Expression ctorInvoke = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.CONSTRUCTOR_INVOCATION);
            ctorInvoke.entityPtr = classDef;
            ctorInvoke.args = args;
            ctorInvoke.opToken = invokeToken;
            return ctorInvoke;
        }

        public static Expression createListDefinition(Token openList, Expression[] items)
        {
            Expression expr = FunctionWrapper.Expression_new(openList, (int) ExpressionType.LIST_DEFINITION);
            expr.values = items;
            return expr;
        }

        public static Expression createDictionaryDefinition(Token openDict, Expression[] keys, Expression[] values)
        {
            Expression expr = FunctionWrapper.Expression_new(openDict, (int) ExpressionType.DICTIONARY_DEFINITION);
            expr.keys = keys;
            expr.values = values;
            for (int i = 0; i < keys.Length; i++)
            {
                Expression key = keys[i];
                if (key.type != (int) ExpressionType.STRING_CONST && key.type != (int) ExpressionType.INTEGER_CONST)
                {
                    FunctionWrapper.Errors_Throw(key.firstToken, "Only string and integer constants can be used as dictionary keys");
                }
            }
            return expr;
        }

        public static Expression createExtensionReference(Token prefixToken, string name)
        {
            Expression extRef = FunctionWrapper.Expression_new(prefixToken, (int) ExpressionType.EXTENSION_REFERENCE);
            extRef.strVal = name;
            return extRef;
        }

        public static Expression createExtensionInvocation(Token firstToken, string name, Expression[] args)
        {
            Expression extInvoke = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.EXTENSION_INVOCATION);
            extInvoke.strVal = name;
            extInvoke.args = args;
            return extInvoke;
        }

        public static Expression createTernary(Expression condition, Token op, Expression trueValue, Expression falseValue)
        {
            Expression ternary = FunctionWrapper.Expression_new(condition.firstToken, (int) ExpressionType.TERNARY);
            ternary.root = condition;
            ternary.opToken = op;
            ternary.left = trueValue;
            ternary.right = falseValue;
            return ternary;
        }

        public static Expression createBinaryOp(Expression left, Token op, Expression right)
        {
            Expression pair = FunctionWrapper.Expression_new(left.firstToken, (int) ExpressionType.BINARY_OP);
            pair.opToken = op;
            pair.left = left;
            pair.right = right;
            return pair;
        }

        public static Expression createInlineIncrement(Token firstToken, Expression root, Token incrementOp, bool isPrefix)
        {
            Expression expr = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.INLINE_INCREMENT);
            expr.opToken = incrementOp;
            expr.root = root;
            expr.boolVal = isPrefix;

            switch (root.type)
            {
                case (int) ExpressionType.DOT_FIELD:
                case (int) ExpressionType.INDEX:
                case (int) ExpressionType.VARIABLE:
                    // These are fine.
                    break;

                default:
                    FunctionWrapper.Errors_Throw(incrementOp, "The '" + incrementOp.Value + "' operator is not allowed on this type of expression.");
                    break;
            }

            return expr;
        }

        public static Expression createNegatePrefix(Token opToken, Expression root)
        {
            ExpressionType t = ExpressionType.NEGATIVE_SIGN;
            if (opToken.Value == "!") t = ExpressionType.BOOLEAN_NOT;
            if (opToken.Value == "~") t = ExpressionType.BITWISE_NOT;
            Expression expr = FunctionWrapper.Expression_new(opToken, (int) t);
            expr.opToken = opToken;
            expr.root = root;
            return expr;
        }

        public static Expression createDotField(Expression root, Token dotToken, string name)
        {
            Expression df = FunctionWrapper.Expression_new(root.firstToken, (int) ExpressionType.DOT_FIELD);
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
                if (walker.type == (int) ExpressionType.DOT_FIELD)
                {
                    walker = walker.root;
                }
                else if (walker.type == (int) ExpressionType.VARIABLE)
                {
                    walker = null;
                }
                else if (errorMessage != null)
                {
                    FunctionWrapper.Errors_Throw(walker.firstToken, errorMessage);
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
            Expression funcInvoke = FunctionWrapper.Expression_new(root.firstToken, (int) ExpressionType.FUNCTION_INVOKE);
            funcInvoke.root = root;
            funcInvoke.opToken = parenToken;
            funcInvoke.args = args;
            return funcInvoke;
        }

        public static Expression createBracketIndex(Expression root, Token bracketToken, Expression index)
        {
            Expression bracketIndex = FunctionWrapper.Expression_new(root.firstToken, (int) ExpressionType.INDEX);
            bracketIndex.root = root;
            bracketIndex.opToken = bracketToken;
            bracketIndex.right = index;
            return bracketIndex;
        }

        public static Expression createClassReference(Token firstToken, AbstractEntity classDef)
        {
            Expression classRef = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.CLASS_REFERENCE);
            classRef.entityPtr = classDef;
            classRef.boolVal = false; // verified safe usage
            return classRef;
        }

        public static Expression createEnumConstant(Token firstToken, AbstractEntity enumDef, string name, int value)
        {
            Expression enumConst = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.ENUM_CONST);
            enumConst.entityPtr = enumDef;
            enumConst.strVal = name;
            enumConst.intVal = value;
            return enumConst;
        }

        public static Expression createEnumReference(Token firstToken, AbstractEntity enumDef)
        {
            Expression enumRef = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.ENUM_REFERENCE);
            enumRef.entityPtr = enumDef;
            return enumRef;
        }

        public static Expression createFunctionReference(Token firstToken, string name, AbstractEntity funcDef)
        {
            Expression funcRef = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.FUNCTION_REFERENCE);
            funcRef.strVal = name;
            funcRef.entityPtr = funcDef;
            return funcRef;
        }

        public static Expression createImportReference(Token firstToken, ImportStatement importStatement)
        {
            Expression impRef = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.IMPORT_REFERENCE);
            impRef.importPtr = importStatement;
            return impRef;
        }

        public static Expression createNamespaceReference(Token firstToken, AbstractEntity nsDef)
        {
            Expression nsRef = FunctionWrapper.Expression_new(firstToken, (int) ExpressionType.NAMESPACE_REFERENCE);
            nsRef.entityPtr = nsDef;
            return nsRef;
        }

        public static Expression createSliceExpression(
            Expression rootExpression,
            Token bracketToken,
            Expression start,
            Expression end,
            Expression step)
        {
            Expression sliceExpr = FunctionWrapper.Expression_new(rootExpression.firstToken, (int) ExpressionType.SLICE);
            sliceExpr.root = rootExpression;
            sliceExpr.opToken = bracketToken;
            sliceExpr.args = new Expression[] { start, end, step };
            return sliceExpr;
        }
        
        public static Expression createTypeof(Token typeofToken, Expression root)
        {
            Expression typeofExpr = FunctionWrapper.Expression_new(typeofToken, (int) ExpressionType.TYPEOF);
            typeofExpr.root = root;
            return typeofExpr;
        }
    }
}
