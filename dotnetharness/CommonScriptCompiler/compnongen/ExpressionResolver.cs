using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ExpressionResolverUtil
    {
        public static Expression ExpressionResolver_ResolveExpressionFirstPass(Resolver resolver, Expression expr)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.BASE_CTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_FirstPass_BaseCtorReference(resolver, expr);
                case (int) ExpressionType.BINARY_OP: return FunctionWrapper.ExpressionResolver_FirstPass_BinaryOp(resolver, expr);
                case (int) ExpressionType.BITWISE_NOT: return FunctionWrapper.ExpressionResolver_FirstPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return FunctionWrapper.ExpressionResolver_FirstPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_INVOCATION: return FunctionWrapper.ExpressionResolver_FirstPass_ConstructorInvocation(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_FirstPass_ConstructorReference(resolver, expr);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return FunctionWrapper.ExpressionResolver_FirstPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return FunctionWrapper.ExpressionResolver_FirstPass_DotField(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return FunctionWrapper.ExpressionResolver_FirstPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.INDEX: return FunctionWrapper.ExpressionResolver_FirstPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return FunctionWrapper.ExpressionResolver_FirstPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_FirstPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return FunctionWrapper.ExpressionResolver_FirstPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return FunctionWrapper.ExpressionResolver_FirstPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_NullConst(resolver, expr);
                case (int) ExpressionType.SLICE: return FunctionWrapper.ExpressionResolver_FirstPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return FunctionWrapper.ExpressionResolver_FirstPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_FirstPass_This(resolver, expr);
                case (int) ExpressionType.TYPEOF: return FunctionWrapper.ExpressionResolver_FirstPass_TypeOf(resolver, expr);
                case (int) ExpressionType.VARIABLE: return FunctionWrapper.ExpressionResolver_FirstPass_Variable(resolver, expr);

                case (int) ExpressionType.EXTENSION_REFERENCE:
                    FunctionWrapper.Errors_Throw(expr.firstToken, "Extension method references must be invoked immediately.");
                    break;

                default:
                    FunctionWrapper.Errors_ThrowNotImplemented(expr.firstToken, "first pass for this type");
                    break;
            }
            return expr;
        }

        public static Expression ExpressionResolver_ResolveExpressionSecondPass(Resolver resolver, Expression expr)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.BASE_CTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_BaseCtorReference(resolver, expr);
                case (int) ExpressionType.BINARY_OP: return ExpressionResolver_SecondPass_BinaryOp(resolver, expr);
                case (int) ExpressionType.BITWISE_NOT: return FunctionWrapper.ExpressionResolver_SecondPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return FunctionWrapper.ExpressionResolver_SecondPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CLASS_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ClassReference(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ConstructorReference(resolver, expr, false);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return FunctionWrapper.ExpressionResolver_SecondPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return FunctionWrapper.ExpressionResolver_SecondPass_DotField(resolver, expr);
                case (int) ExpressionType.ENUM_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_EnumConstant(resolver, expr);
                case (int) ExpressionType.EXTENSION_INVOCATION: return FunctionWrapper.ExpressionResolver_SecondPass_ExtensionInvocation(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return FunctionWrapper.ExpressionResolver_SecondPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.FUNCTION_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_FunctionReference(resolver, expr);
                case (int) ExpressionType.IMPORT_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ImportReference(resolver, expr);
                case (int) ExpressionType.INDEX: return FunctionWrapper.ExpressionResolver_SecondPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return FunctionWrapper.ExpressionResolver_SecondPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_SecondPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return FunctionWrapper.ExpressionResolver_SecondPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NAMESPACE_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_NamespaceReference(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return FunctionWrapper.ExpressionResolver_SecondPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_NullConstant(resolver, expr);
                case (int) ExpressionType.SLICE: return FunctionWrapper.ExpressionResolver_SecondPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return FunctionWrapper.ExpressionResolver_SecondPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_SecondPass_ThisConstant(resolver, expr);
                case (int) ExpressionType.TYPEOF: return FunctionWrapper.ExpressionResolver_SecondPass_TypeOf(resolver, expr);
                case (int) ExpressionType.VARIABLE: return FunctionWrapper.ExpressionResolver_SecondPass_Variable(resolver, expr);
                default:
                    FunctionWrapper.Errors_ThrowNotImplemented(expr.firstToken, "second pass for this type");
                    break;
            }
            return expr;
        }

        private static Expression ExpressionResolver_SecondPass_BinaryOp(Resolver resolver, Expression expr)
        {
            Token firstToken = expr.firstToken;
            Token opToken = expr.opToken;
            string op = opToken.Value;

            expr.left = ExpressionResolver_ResolveExpressionSecondPass(resolver, expr.left);
            if (expr.right.type == (int)ExpressionType.CLASS_REFERENCE && op == "is")
            {
                expr.right.boolVal = true;
            }
            expr.right = ExpressionResolver_ResolveExpressionSecondPass(resolver, expr.right);

            // TODO: resolve constants
            if (FunctionWrapper.IsExpressionConstant(expr.left) && 
                FunctionWrapper.IsExpressionConstant(expr.right))
            {
                bool isLeftNumeric = FunctionWrapper.IsExpressionNumericConstant(expr.left);
                bool isRightNumeric = FunctionWrapper.IsExpressionNumericConstant(expr.right);
                bool isRightZero = expr.right.intVal == 0;
                if (expr.right.type == (int)ExpressionType.FLOAT_CONST) isRightZero = expr.right.floatVal == 0;

                if (isRightNumeric)
                {
                    if (op == "/" || op == "%")
                    {
                        if (isRightZero)
                        {
                            if (op == "%") FunctionWrapper.Errors_Throw(opToken, "Modulo by zero");
                            FunctionWrapper.Errors_Throw(opToken, "Division by zero");
                        }
                    }
                }

                if (isLeftNumeric && isRightNumeric)
                {
                    double floatLeft = FunctionWrapper.GetNumericValueOfConstantExpression(expr.left);
                    double floatRight = FunctionWrapper.GetNumericValueOfConstantExpression(expr.right);

                    if (op == "<") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft < floatRight);
                    if (op == ">") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft > floatRight);
                    if (op == "<=") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft <= floatRight);
                    if (op == ">=") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft >= floatRight);

                    // TODO
                    // strict equality comparison on floats is okay in the case that these are converted
                    // from integers because they'll still be whole numbers. There is a bug here where they'll
                    // be inaccurate for float comparisons which is to be expected BUT they'll be slightly
                    // inaccurate in different ways across platforms.
                    // As a hypothetical example:
                    //     value = ln(e ** 2) == 2;
                    //       vs
                    //     x = 2
                    //     value = ln(e ** x) == x;
                    // This value may different if the runtime language and compile time language are different.
                    if (op == "==") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft == floatRight);
                    if (op == "!=") return FunctionWrapper.Expression_createBoolConstant(firstToken, floatLeft != floatRight);
                }

                if (op == "+" && (expr.left.type == (int) ExpressionType.STRING_CONST || expr.right.type == (int) ExpressionType.STRING_CONST))
                {
                    string leftStr = FunctionWrapper.GetStringFromConstantExpression(expr.left);
                    string rightStr = FunctionWrapper.GetStringFromConstantExpression(expr.right);
                    return FunctionWrapper.Expression_createStringConstant(expr.firstToken, leftStr + rightStr);
                }

                switch (expr.left.type * (int)ExpressionType.MAX_VALUE + expr.right.type)
                {
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                        int intLeft = expr.left.intVal;
                        int intRight = expr.right.intVal;
                        if (op == "**")
                        {
                            return FunctionWrapper.Expression_createFloatConstant(opToken, Math.Pow(intLeft, intRight));
                        }
                        int resultInt = 0;
                        if (op == "+") {
                            resultInt = intLeft + intRight;
                        } else if (op == "-") {
                            resultInt = intLeft - intRight;
                        } else if (op == "*") {
                            resultInt = intLeft * intRight;
                        } else if (op == "/") {
                            resultInt = intLeft / intRight;
                        } else if (op == "%") {
                            if (intRight < 0) {
                                intRight = -intRight;
                                resultInt = intLeft % intRight;
                                if (resultInt > 0) resultInt -= intRight;
                            } else {
                                resultInt = intLeft % intRight;
                                if (resultInt < 0) resultInt += intRight;
                            }
                        } else if (op == "|") {
                            resultInt = intLeft | intRight;   
                        } else if (op == "&") {
                            resultInt = intLeft & intRight;   
                        } else if (op == "^") {
                            resultInt = intLeft ^ intRight;
                        } else if (op == "<<") {
                            resultInt = intLeft << intRight;
                        } else if (op == ">>") {
                            resultInt = intLeft >> intRight;   
                        } else if (op == ">>>") {
                            resultInt = intLeft >>> intRight;
                        } else {
                            FunctionWrapper.ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        }
                        
                        return FunctionWrapper.Expression_createIntegerConstant(opToken, resultInt);

                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                        double floatLeft = FunctionWrapper.GetNumericValueOfConstantExpression(expr.left);
                        double floatRight = FunctionWrapper.GetNumericValueOfConstantExpression(expr.right);
                        double floatResult = 0.0;
                        if (op == "+")
                        {
                            floatResult = floatLeft + floatRight; 
                        } else if (op == "-")
                        {
                            floatResult = floatLeft - floatRight; 
                        } else if (op == "*")
                        {
                            floatResult = floatLeft * floatRight; 
                        } else if (op == "/")
                        {
                            floatResult = floatLeft / floatRight; 
                        } else if (op == "%")
                        {
                            if (floatRight < 0)
                            {
                                floatRight = -floatRight;
                                floatResult = floatLeft % floatRight;
                                if (floatResult > 0) floatResult -= floatRight;
                            }
                            else
                            {
                                floatResult = floatLeft % floatRight;
                                if (floatResult < 0) floatResult += floatRight;
                            }
                        } else {
                            FunctionWrapper.ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        }
                        
                        return FunctionWrapper.Expression_createFloatConstant(opToken, floatResult);

                    case (int)ExpressionType.STRING_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.STRING_CONST:
                        if (op == "*")
                        {
                            Expression strExpr = expr.left;
                            Expression intExpr = expr.right;
                            if (expr.left.type == (int)ExpressionType.INTEGER_CONST)
                            {
                                strExpr = expr.right;
                                intExpr = expr.left;
                            }
                            
                            int size = intExpr.intVal;
                            string val = strExpr.strVal;
                            if (size == 0) return FunctionWrapper.Expression_createStringConstant(expr.firstToken, "");
                            if (size == 1) return strExpr;
                            if (val.Length * size < 12)
                            {
                                List<string> sb = new List<string>();
                                for (int i = 0; i < size; i += 1)
                                {
                                    sb.Add(val);
                                }
                                return FunctionWrapper.Expression_createStringConstant(expr.firstToken, string.Join("", sb));
                            }
                            return expr;
                        }

                        FunctionWrapper.ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        break;

                    default:
                        FunctionWrapper.ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        break;
                }
                
                FunctionWrapper.fail(""); // all code paths above cause an exit.
            }

            return expr;
        }
    }
}
