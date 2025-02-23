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
                case (int) ExpressionType.BINARY_OP: return ExpressionResolver_FirstPass_BinaryOp(resolver, expr);
                case (int) ExpressionType.BITWISE_NOT: return ExpressionResolver_FirstPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return ExpressionResolver_FirstPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_INVOCATION: return FunctionWrapper.ExpressionResolver_FirstPass_ConstructorInvocation(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return ExpressionResolver_FirstPass_ConstructorReference(resolver, expr);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return ExpressionResolver_FirstPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return ExpressionResolver_FirstPass_DotField(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return ExpressionResolver_FirstPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.INDEX: return ExpressionResolver_FirstPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return ExpressionResolver_FirstPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_FirstPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return ExpressionResolver_FirstPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return ExpressionResolver_FirstPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_NullConst(resolver, expr);
                case (int) ExpressionType.SLICE: return ExpressionResolver_FirstPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return ExpressionResolver_FirstPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_FirstPass_This(resolver, expr);
                case (int) ExpressionType.TYPEOF: return ExpressionResolver_FirstPass_TypeOf(resolver, expr);
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
                case (int) ExpressionType.BITWISE_NOT: return ExpressionResolver_SecondPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return ExpressionResolver_SecondPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CLASS_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ClassReference(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return ExpressionResolver_SecondPass_ConstructorReference(resolver, expr, false);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return ExpressionResolver_SecondPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return ExpressionResolver_SecondPass_DotField(resolver, expr);
                case (int) ExpressionType.ENUM_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_EnumConstant(resolver, expr);
                case (int) ExpressionType.EXTENSION_INVOCATION: return ExpressionResolver_SecondPass_ExtensionInvocation(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return ExpressionResolver_SecondPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.FUNCTION_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_FunctionReference(resolver, expr);
                case (int) ExpressionType.IMPORT_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ImportReference(resolver, expr);
                case (int) ExpressionType.INDEX: return ExpressionResolver_SecondPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return ExpressionResolver_SecondPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_SecondPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return ExpressionResolver_SecondPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NAMESPACE_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_NamespaceReference(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return ExpressionResolver_SecondPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_NullConstant(resolver, expr);
                case (int) ExpressionType.SLICE: return ExpressionResolver_SecondPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return ExpressionResolver_SecondPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_SecondPass_ThisConstant(resolver, expr);
                case (int) ExpressionType.TYPEOF: return ExpressionResolver_SecondPass_TypeOf(resolver, expr);
                case (int) ExpressionType.VARIABLE: return FunctionWrapper.ExpressionResolver_SecondPass_Variable(resolver, expr);
                default:
                    FunctionWrapper.Errors_ThrowNotImplemented(expr.firstToken, "second pass for this type");
                    break;
            }
            return expr;
        }

        public static void ExpressionResolver_ResolveExpressionArrayFirstPass(Resolver resolver, Expression[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, arr[i]);
            }
        }

        public static void ExpressionResolver_ResolveExpressionArraySecondPass(Resolver resolver, Expression[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, arr[i]);
            }
        }

        private static Expression ExpressionResolver_FirstPass_BinaryOp(Resolver resolver, Expression binOp)
        {
            binOp.left = ExpressionResolver_ResolveExpressionFirstPass(resolver, binOp.left);
            binOp.right = ExpressionResolver_ResolveExpressionFirstPass(resolver, binOp.right);
            switch (binOp.opToken.Value)
            {
                case "|":
                case "&":
                case "^":
                case "<<":
                case ">>":
                    binOp.left = FunctionWrapper.ExpressionResolver_IntegerRequired(resolver, binOp.left);
                    binOp.right = FunctionWrapper.ExpressionResolver_IntegerRequired(resolver, binOp.right);
                    break;
            }
            return binOp;
        }

        private static Expression ExpressionResolver_FirstPass_BitwiseNot(Resolver resolver, Expression bwn)
        {
            bwn.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, bwn.root);
            bwn.root = FunctionWrapper.ExpressionResolver_IntegerRequired(resolver, bwn.root);
            return bwn;
        }

        private static Expression ExpressionResolver_FirstPass_BoolNot(Resolver resolver, Expression booNot)
        {
            booNot.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, booNot.root);
            return booNot;
        }

        private static Expression ExpressionResolver_FirstPass_ConstructorReference(Resolver resolver, Expression ctorRef)
        {
            ctorRef.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, ctorRef.root);
            return ctorRef;
        }

        private static Expression ExpressionResolver_FirstPass_DictionaryDefinition(Resolver resolver, Expression dictDef)
        {
            int length = dictDef.keys.Length;
            for (int i = 0; i < length; i++)
            {
                dictDef.keys[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, dictDef.keys[i]);
                dictDef.values[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, dictDef.values[i]);
            }
            return dictDef;
        }

        private static Expression ExpressionResolver_FirstPass_DotField(Resolver resolver, Expression dotField)
        {
            dotField.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, dotField.root);
            string fieldName = dotField.strVal;

            switch (dotField.root.type)
            {
                case (int) ExpressionType.IMPORT_REFERENCE:
                    ImportStatement importRef = dotField.root.importPtr;
                    CompiledModule moduleRef = importRef.compiledModuleRef;
                    Expression output = FunctionWrapper.LookupUtil_tryCreateModuleMemberReference(moduleRef, dotField.firstToken, fieldName);
                    if (output == null)
                    {
                        FunctionWrapper.Errors_Throw(dotField.opToken, "The module does not have a member named '" + fieldName + "'");
                    }
                    return output;

                case (int) ExpressionType.ENUM_REFERENCE:
                    EnumEntity enumRef = (EnumEntity)dotField.root.entityPtr.specificData;
                    // TODO: ew, gross, figure out a way to cache and stash a dictionary somewhere on the entity.
                    // Otherwise this could potentially turn problematic in the off chance there's an enum with a
                    // few hundred members in it (e.g. generated code).
                    for (int i = 0; i < enumRef.memberNameTokens.Length; i++)
                    {
                        if (enumRef.memberNameTokens[i].Value == fieldName)
                        {
                            return FunctionWrapper.Expression_createEnumConstant(dotField.firstToken, enumRef.baseData, fieldName, enumRef.memberValues[i].intVal);
                        }
                    }
                    FunctionWrapper.Errors_Throw(dotField.opToken, "The enum " + enumRef.baseData.fqName + " does not have a member named '" + fieldName + "'");
                    break;

                case (int) ExpressionType.NAMESPACE_REFERENCE:
                    NamespaceEntity nsEntity = (NamespaceEntity)dotField.root.entityPtr.specificData;
                    if (!nsEntity.nestedMembers.ContainsKey(fieldName))
                    {
                        FunctionWrapper.Errors_Throw(dotField.opToken, "There is no member of this namespace named '" + fieldName + "'.");
                    }
                    AbstractEntity referencedEntity = nsEntity.nestedMembers[fieldName];
                    return FunctionWrapper.ExpressionResolver_WrapEntityIntoReferenceExpression(resolver, dotField.firstToken, referencedEntity);
            }

            return dotField;
        }

        private static Expression ExpressionResolver_FirstPass_FunctionInvocation(Resolver resolver, Expression funcInvoke)
        {
            if (funcInvoke.root.type == (int) ExpressionType.EXTENSION_REFERENCE)
            {
                ExpressionResolver_ResolveExpressionArrayFirstPass(resolver, funcInvoke.args);
                return FunctionWrapper.Expression_createExtensionInvocation(funcInvoke.firstToken, funcInvoke.root.strVal, funcInvoke.args);
            }
            else
            {
                funcInvoke.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, funcInvoke.root);
                ExpressionResolver_ResolveExpressionArrayFirstPass(resolver, funcInvoke.args);
            }

            return funcInvoke;
        }

        private static Expression ExpressionResolver_FirstPass_Index(Resolver resolver, Expression indexExpr)
        {
            indexExpr.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, indexExpr.root);
            indexExpr.right = ExpressionResolver_ResolveExpressionFirstPass(resolver, indexExpr.right);
            return indexExpr;
        }

        private static Expression ExpressionResolver_FirstPass_InlineIncrement(Resolver resolver, Expression inlineIncr)
        {
            inlineIncr.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, inlineIncr.root);
            return inlineIncr;
        }

        private static Expression ExpressionResolver_FirstPass_ListDefinition(Resolver resolver, Expression listDef)
        {
            for (int i = 0; i < listDef.values.Length; i++)
            {
                listDef.values[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, listDef.values[i]);
            }
            return listDef;
        }

        private static Expression ExpressionResolver_FirstPass_NegativeSign(Resolver resolver, Expression negSign)
        {
            negSign.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, negSign.root);
            return negSign;
        }

        private static Expression ExpressionResolver_FirstPass_Slice(Resolver resolver, Expression slice)
        {
            slice.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, slice.root);
            for (int i = 0; i < 3; i++)
            {
                if (slice.args[i] != null) slice.args[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, slice.args[i]);
            }
            return slice;
        }

        private static Expression ExpressionResolver_FirstPass_Ternary(Resolver resolver, Expression ternary)
        {
            ternary.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary.root);
            ternary.left = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary.left);
            ternary.right = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary.right);
            return ternary;
        }

        private static Expression ExpressionResolver_FirstPass_TypeOf(Resolver resolver, Expression typeofExpr)
        {
            typeofExpr.root = ExpressionResolver_ResolveExpressionFirstPass(resolver, typeofExpr.root);
            return typeofExpr;
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
                                for (int i = 0; i < size; i++)
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

        private static Expression ExpressionResolver_SecondPass_ConstructorReference(Resolver resolver, Expression ctorRef, bool isExpected)
        {
            if (isExpected)
            {
                ctorRef.root.boolVal = true;
                ctorRef.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, ctorRef.root);
                if (ctorRef.root.type != (int) ExpressionType.CLASS_REFERENCE) FunctionWrapper.Errors_Throw(ctorRef.root.firstToken, "This is not a valid class definition.");
                ctorRef.entityPtr = ctorRef.root.entityPtr;
                ctorRef.root = null;
                return ctorRef;
            }

            FunctionWrapper.Errors_Throw(ctorRef.firstToken, "A constructor must be immediately invoked.");
            return ctorRef;
        }

        private static Expression ExpressionResolver_SecondPass_BitwiseNot(Resolver resolver, Expression bwn)
        {
            bwn.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, bwn.root);

            if (FunctionWrapper.IsExpressionConstant(bwn.root))
            {
                if (bwn.root.type != (int) ExpressionType.INTEGER_CONST)
                {
                    FunctionWrapper.Errors_Throw(bwn.firstToken, "Bitwise-NOT operator can only be applied on integers.");
                }
                return FunctionWrapper.Expression_createIntegerConstant(bwn.firstToken, ~bwn.root.intVal);
            }
            return bwn;
        }

        private static Expression ExpressionResolver_SecondPass_BoolNot(Resolver resolver, Expression bn)
        {
            bn.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, bn.root);

            if (FunctionWrapper.IsExpressionConstant(bn.root))
            {
                if (bn.root.type != (int) ExpressionType.BOOLEAN_NOT)
                {
                    FunctionWrapper.Errors_Throw(bn.firstToken, "Boolean-NOT operator can only be applied to booleans.");
                }
                return FunctionWrapper.Expression_createBoolConstant(bn.firstToken, !bn.root.boolVal);
            }

            return bn;
        }

        private static Expression ExpressionResolver_SecondPass_DictionaryDefinition(Resolver resolver, Expression dictDef)
        {
            int length = dictDef.keys.Length;
            Dictionary<string, bool> strCollide = new Dictionary<string, bool>();
            Dictionary<int, bool> intCollide = new Dictionary<int, bool>();
            for (int i = 0; i < length; i++)
            {
                Expression key = ExpressionResolver_ResolveExpressionSecondPass(resolver, dictDef.keys[i]);
                bool isMixed = false;
                bool isCollide = false;
                if (key.type == (int) ExpressionType.INTEGER_CONST)
                {
                    isMixed = strCollide.Count > 0;
                    isCollide = intCollide.ContainsKey(key.intVal);
                    intCollide[key.intVal] = true;
                }
                else if (key.type == (int) ExpressionType.STRING_CONST)
                {
                    isMixed = intCollide.Count > 0;
                    isCollide = strCollide.ContainsKey(key.strVal);
                    strCollide[key.strVal] = true;
                }
                else
                {
                    FunctionWrapper.Errors_Throw(key.firstToken, "This type of expression cannot be used as a dictionary key. Dictionary keys must be constant integer or string expressions.");
                }

                if (isMixed)
                {
                    FunctionWrapper.Errors_Throw(key.firstToken, "Dictionary cannot contain mixed types for keys.");
                }

                if (isCollide)
                {
                    FunctionWrapper.Errors_Throw(key.firstToken, "There are multiple keys with this same value.");
                }

                dictDef.keys[i] = key;
                dictDef.values[i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, dictDef.values[i]);
            }
            return dictDef;
        }

        private static Expression ExpressionResolver_SecondPass_DotField(Resolver resolver, Expression df)
        {
            // This is the only way to refer to a class reference.
            if (df.root.type == (int) ExpressionType.CLASS_REFERENCE) df.root.boolVal = true;

            df.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, df.root);
            switch (df.root.type)
            {
                case (int) ExpressionType.STRING_CONST:
                    if (df.strVal == "length")
                    {
                        return FunctionWrapper.Expression_createIntegerConstant(df.firstToken, df.root.strVal.Length);
                    }
                    break;

                case (int) ExpressionType.THIS:
                case (int) ExpressionType.BASE:
                    // TODO: check to see if field exists.
                    // For now, just trust it at runtime as if it were a blind variable.
                    break;

                case (int) ExpressionType.CLASS_REFERENCE:
                    ClassEntity classDef = (ClassEntity)df.root.entityPtr.specificData;
                    AbstractEntity member = null;
                    if (classDef.classMembers.ContainsKey(df.strVal))
                    {
                        member = classDef.classMembers[df.strVal];
                        if (!member.isStatic)
                        {
                            FunctionWrapper.Errors_Throw(df.opToken, classDef.baseData.fqName + "." + df.strVal + " is not static.");
                        }
                    }
                    else
                    {
                        FunctionWrapper.Errors_Throw(df.opToken, "The class " + classDef.baseData.fqName + " does not have a member named '." + df.strVal + "'.");
                    }
                    break;

                default:
                    break;
            }

            return df;
        }

        private static Expression ExpressionResolver_SecondPass_ExtensionInvocation(Resolver resolver, Expression expr)
        {
            ExpressionResolver_ResolveExpressionArraySecondPass(resolver, expr.args);
            int argc = -1;
            if (FunctionWrapper.SpecialActionUtil_IsSpecialActionAndNotExtension(resolver.staticCtx.specialActionUtil, expr.strVal))
            {
                argc = FunctionWrapper.SpecialActionUtil_GetSpecialActionArgc(resolver.staticCtx.specialActionUtil, expr.strVal);
            }
            else
            {
                string name = expr.strVal;
                if (name == "delay_invoke") argc = 2;
                else if (name == "io_stdout") argc = 1;
                else if (name == "sleep") argc = 1;
                else if (ResolverUtil.isValidRegisteredExtension(resolver, expr.strVal)) {
                    return expr;
                } else {
                    FunctionWrapper.Errors_Throw(expr.firstToken, "Extension is not registered: " + expr.strVal);
                }
            }

            if (argc != -1 && expr.args.Length != argc) {
                FunctionWrapper.Errors_Throw(expr.firstToken, "Incorrect number of arguments to extension");
            }

            return expr;
        }

        private static Expression ExpressionResolver_SecondPass_FunctionInvocation(Resolver resolver, Expression funcInvoke)
        {
            if (funcInvoke.root.type == (int) ExpressionType.CONSTRUCTOR_REFERENCE)
            {
                Expression ctorRef = ExpressionResolver_SecondPass_ConstructorReference(resolver, funcInvoke.root, true);
                if (ctorRef.type != (int) ExpressionType.CONSTRUCTOR_REFERENCE) FunctionWrapper.fail(""); // this shouldn't happen. 

                ExpressionResolver_ResolveExpressionArraySecondPass(resolver, funcInvoke.args);

                return FunctionWrapper.Expression_createConstructorInvocation(funcInvoke.firstToken, (AbstractEntity)ctorRef.entityPtr, funcInvoke.opToken, funcInvoke.args);
            }

            funcInvoke.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, funcInvoke.root);

            switch (funcInvoke.root.type)
            {
                case (int) ExpressionType.VARIABLE:
                case (int) ExpressionType.BASE:
                case (int) ExpressionType.FUNCTION_REFERENCE:
                case (int) ExpressionType.DOT_FIELD:
                case (int) ExpressionType.BASE_CTOR_REFERENCE:
                case (int) ExpressionType.INDEX:
                    break;

                default:
                    FunctionWrapper.Errors_Throw(funcInvoke.opToken, "Cannot invoke this type of expression like a function.");
                    break;
            }

            ExpressionResolver_ResolveExpressionArraySecondPass(resolver, funcInvoke.args);

            return funcInvoke;
        }

        private static Expression ExpressionResolver_SecondPass_Index(Resolver resolver, Expression indexExpr)
        {
            indexExpr.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, indexExpr.root);
            indexExpr.right = ExpressionResolver_ResolveExpressionSecondPass(resolver, indexExpr.right);
            return indexExpr;
        }

        private static Expression ExpressionResolver_SecondPass_InlineIncrement(Resolver resolver, Expression inlineIncr)
        {
            inlineIncr.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, inlineIncr.root);
            switch (inlineIncr.root.type)
            {
                case (int) ExpressionType.VARIABLE:
                case (int) ExpressionType.INDEX:
                case (int) ExpressionType.DOT_FIELD:
                    // these are fine
                    break;

                default:
                    FunctionWrapper.Errors_Throw(inlineIncr.opToken, "Cannot use the '" + inlineIncr.opToken.Value + "' operator on this type of expression.");
                    break;
            }
            return inlineIncr;
        }

        private static Expression ExpressionResolver_SecondPass_ListDefinition(Resolver resolver, Expression listDef)
        {
            for (int i = 0; i < listDef.values.Length; i++)
            {
                listDef.values[i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, listDef.values[i]);
            }
            return listDef;
        }

        private static Expression ExpressionResolver_SecondPass_NegativeSign(Resolver resolver, Expression negSign)
        {
            Expression root = ExpressionResolver_ResolveExpressionSecondPass(resolver, negSign.root);
            negSign.root = root;
            if (FunctionWrapper.IsExpressionNumericConstant(root))
            {
                switch (root.type)
                {
                    case (int) ExpressionType.INTEGER_CONST:
                        root.intVal *= -1;
                        break;
                    case (int) ExpressionType.FLOAT_CONST:
                        root.floatVal *= -1;
                        break;
                    case (int) ExpressionType.ENUM_CONST:
                        root = FunctionWrapper.Expression_createIntegerConstant(root.firstToken, -root.intVal);
                        break;
                    default:
                        FunctionWrapper.fail("Not implemented");
                        break;
                }
                return root;
            }

            return negSign;
        }

        private static Expression ExpressionResolver_SecondPass_Slice(Resolver resolver, Expression slice)
        {
            slice.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, slice.root);
            for (int i = 0; i < 3; i++)
            {
                if (slice.args[i] != null)
                {
                    Expression expr = ExpressionResolver_ResolveExpressionSecondPass(resolver, slice.args[i]);
                    slice.args[i] = expr;
                    if (FunctionWrapper.IsExpressionConstant(expr))
                    {
                        if (expr.type != (int) ExpressionType.INTEGER_CONST && expr.type != (int) ExpressionType.ENUM_CONST)
                        {
                            FunctionWrapper.Errors_Throw(expr.firstToken, "Only integers may be used in a slice expression.");
                        }
                    }
                }
            }

            return slice;
        }

        private static Expression ExpressionResolver_SecondPass_Ternary(Resolver resolver, Expression ternary)
        {
            ternary.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary.root);
            ternary.left = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary.left);
            ternary.right = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary.right);

            if (FunctionWrapper.IsExpressionConstant(ternary.root))
            {
                if (ternary.root.type != (int) ExpressionType.BOOL_CONST)
                {
                    FunctionWrapper.Errors_Throw(ternary.root.firstToken, "Only booleans can be used as ternary conditions.");
                }

                if (ternary.root.boolVal) return ternary.left;
                return ternary.right;
            }

            return ternary;
        }

        private static Expression ExpressionResolver_SecondPass_TypeOf(Resolver resolver, Expression typeofExpr)
        {
            typeofExpr.root = ExpressionResolver_ResolveExpressionSecondPass(resolver, typeofExpr.root);
            string stringConst = null;
            switch (typeofExpr.root.type)
            {
                case (int) ExpressionType.INTEGER_CONST: stringConst = "int"; break;
                case (int) ExpressionType.FLOAT_CONST: stringConst = "float"; break;
                case (int) ExpressionType.NULL_CONST: stringConst = "null"; break;
                case (int) ExpressionType.BOOL_CONST: stringConst = "bool"; break;
                case (int) ExpressionType.STRING_CONST: stringConst = "string"; break;
                case (int) ExpressionType.FUNCTION_REFERENCE: stringConst = "function"; break;

                case (int) ExpressionType.LIST_DEFINITION:
                    if (typeofExpr.root.values.Length == 0) stringConst = "list";
                    break;

                case (int) ExpressionType.DICTIONARY_DEFINITION:
                    if (typeofExpr.root.keys.Length == 0) stringConst = "dict";
                    break;
            }

            if (stringConst != null)
            {
                return FunctionWrapper.Expression_createStringConstant(typeofExpr.firstToken, stringConst);
            }

            return typeofExpr;
        }
    }
}
