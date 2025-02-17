using System;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ExpressionSerializer
    {
        public static ByteCodeBuffer serializeExpression(Expression expr)
        {
            switch (expr.type)
            {
                // These should have been resolved out with proper errors thrown before this point.
                case ExpressionType.EXTENSION_REFERENCE:
                    throw new InvalidOperationException();

                case ExpressionType.BASE_CTOR_REFERENCE: return serializeBaseCtorReference(expr);
                case ExpressionType.BINARY_OP: return serializeBinaryOp(expr);
                case ExpressionType.BITWISE_NOT: return serializeBitwiseNot(expr);
                case ExpressionType.BOOL_CONST: return serializeBoolConst(expr);
                case ExpressionType.BOOLEAN_NOT: return serializeBooleanNot(expr);
                case ExpressionType.CLASS_REFERENCE: return serializeClassReference(expr);
                case ExpressionType.CONSTRUCTOR_INVOCATION: return serializeConstructorInvocation(expr);
                case ExpressionType.DICTIONARY_DEFINITION: return serializeDictionaryDefinition(expr);
                case ExpressionType.DOT_FIELD: return serializeDotField(expr);
                case ExpressionType.EXTENSION_INVOCATION: return serializeExtensionInvocation(expr);
                case ExpressionType.FLOAT_CONST: return serializeFloatConstant(expr);
                case ExpressionType.FUNCTION_INVOKE: return serializeFunctionInvocation(expr);
                case ExpressionType.FUNCTION_REFERENCE: return serializeFunctionReference(expr);
                case ExpressionType.INDEX: return serializeIndex(expr);
                case ExpressionType.INLINE_INCREMENT: return serializeInlineIncrement(expr);
                case ExpressionType.INTEGER_CONST: return serializeIntegerConstant(expr);
                case ExpressionType.LAMBDA: return serializeLambda(expr);
                case ExpressionType.LIST_DEFINITION: return serializeListDefinition(expr);
                case ExpressionType.NEGATIVE_SIGN: return serializeNegativeSign(expr);
                case ExpressionType.NULL_CONST: return serializeNullConstant(expr);
                case ExpressionType.SLICE: return serializeSlice(expr);
                case ExpressionType.STRING_CONST: return serializeStringConstant(expr);
                case ExpressionType.TERNARY: return serializeTernary(expr);
                case ExpressionType.THIS: return serializeThis(expr);
                case ExpressionType.TYPEOF: return serializeTypeOf(expr);
                case ExpressionType.VARIABLE: return serializeVariable(expr);
                default:
                    throw new NotImplementedException();
            }
        }

        private static ByteCodeBuffer serializeBaseCtorReference(Expression baseCtor)
        {
            AbstractEntity baseClass = (AbstractEntity)baseCtor.objPtr;
            return FunctionWrapper.create1(OpCodes.OP_PUSH_BASE_CTOR, baseCtor.firstToken, null, baseClass.serializationIndex);
        }

        private static ByteCodeBuffer serializeBinaryOp(Expression binOp)
        {
            Token opToken = binOp.opToken;
            string op = opToken.Value;
            Expression left = binOp.left;
            Expression right = binOp.right;

            ByteCodeBuffer leftBuf = serializeExpression(left);
            ByteCodeBuffer rightBuf = serializeExpression(right);

            if (op == "??")
            {
                return FunctionWrapper.join3(
                    leftBuf,
                    FunctionWrapper.create1(OpCodes.OP_POP_IF_NULL_OR_JUMP, opToken, null, rightBuf.length),
                    rightBuf);
            }

            if (op == "&&" || op == "||")
            {
                leftBuf = ByteCode.ensureBooleanExpression(left.firstToken, leftBuf);
                rightBuf = ByteCode.ensureBooleanExpression(right.firstToken, rightBuf);
                return FunctionWrapper.join3(
                    leftBuf,
                    FunctionWrapper.create1(
                        op == "&&" ? OpCodes.OP_POP_IF_TRUE_OR_JUMP : OpCodes.OP_POP_IF_FALSE_OR_JUMP,
                        null, null, rightBuf.length),
                    rightBuf);
            }

            return FunctionWrapper.join3(
                leftBuf,
                rightBuf,
                FunctionWrapper.create0(OpCodes.OP_BIN_OP, opToken, op)
            );
        }

        private static ByteCodeBuffer serializeDotField(Expression df)
        {
            return FunctionWrapper.join2(
                serializeExpression(df.root),
                FunctionWrapper.create0(OpCodes.OP_DOT_FIELD, df.opToken, df.strVal));
        }

        private static ByteCodeBuffer serializeExtensionInvocation(Expression extInvoke)
        {
            if (SpecialActionUtil.IsSpecialActionAndNotExtension(extInvoke.strVal))
            {
                return SpecialActionSerializer.serializeSpecialAction(extInvoke);
            }

            ByteCodeBuffer buf = null;
            int argc = extInvoke.args.Length;
            for (int i = 0; i < argc; i++)
            {
                buf = FunctionWrapper.join2(buf, serializeExpression(extInvoke.args[i]));
            }

            return FunctionWrapper.join2(
                buf,
                FunctionWrapper.create1(OpCodes.OP_EXT_INVOKE, extInvoke.firstToken, extInvoke.strVal, argc));
        }

        private static ByteCodeBuffer serializeBitwiseNot(Expression bwn)
        {
            return FunctionWrapper.join2(
                ByteCode.ensureIntegerExpression(bwn.root.firstToken, serializeExpression(bwn.root)),
                FunctionWrapper.create0(OpCodes.OP_BITWISE_NOT, bwn.firstToken, null)
                );
        }

        private static ByteCodeBuffer serializeBoolConst(Expression bc)
        {
            return FunctionWrapper.create1(OpCodes.OP_PUSH_BOOL, bc.firstToken, null, bc.boolVal ? 1 : 0); ;
        }

        private static ByteCodeBuffer serializeBooleanNot(Expression bn)
        {
            // OP_BOOLEAN_NOT includes assertions for a boolean value, so ensureBooleanExpression is not needed here.
            // ! unary op is unlikely to be used in expressions that are verifiable as boolean at compile time, so it's faster to just bundle them together. 
            return FunctionWrapper.join2(
                serializeExpression(bn.root),
                FunctionWrapper.create0(OpCodes.OP_BOOLEAN_NOT, bn.firstToken, null));
        }

        private static ByteCodeBuffer serializeClassReference(Expression classRef)
        {
            return FunctionWrapper.create1(OpCodes.OP_PUSH_CLASS_REF, classRef.firstToken, null, ((AbstractEntity)classRef.objPtr).serializationIndex);
        }

        private static ByteCodeBuffer serializeConstructorInvocation(Expression ctorInvoke)
        {
            AbstractEntity classDef = (AbstractEntity)ctorInvoke.objPtr;
            ByteCodeBuffer buf = null;
            for (int i = 0; i < ctorInvoke.args.Length; i++)
            {
                buf = FunctionWrapper.join2(buf, serializeExpression(ctorInvoke.args[i]));
            }
            return FunctionWrapper.join3(
                FunctionWrapper.create1(OpCodes.OP_CTOR_REF, ctorInvoke.firstToken, null, classDef.serializationIndex),
                buf,
                FunctionWrapper.create1(OpCodes.OP_FUNCTION_INVOKE, ctorInvoke.opToken, null, ctorInvoke.args.Length));
        }

        private static ByteCodeBuffer serializeDictionaryDefinition(Expression dictDef)
        {
            ByteCodeBuffer buf = null;
            for (int i = 0; i < dictDef.keys.Length; i++)
            {
                buf = FunctionWrapper.join3(
                    buf,
                    serializeExpression(dictDef.keys[i]),
                    serializeExpression(dictDef.values[i]));
            }
            return FunctionWrapper.join2(
                buf,
                FunctionWrapper.create1(OpCodes.OP_BUILD_DICT, dictDef.firstToken, null, dictDef.keys.Length));
        }

        private static ByteCodeBuffer serializeFloatConstant(Expression floatConst)
        {
            double val = floatConst.floatVal;
            if (val * 4 % 1 == 0)
            {
                return FunctionWrapper.create1(OpCodes.OP_PUSH_FLOAT, null, null, (int)(val * 4));
            }
            return FunctionWrapper.create0(OpCodes.OP_PUSH_FLOAT, null, val + "");
        }

        private static ByteCodeBuffer serializeFunctionInvocation(Expression funcInvoke)
        {
            ByteCodeBuffer buf = serializeExpression(funcInvoke.root);
            int argc = funcInvoke.args.Length;
            for (int i = 0; i < argc; i++)
            {
                buf = FunctionWrapper.join2(buf, serializeExpression(funcInvoke.args[i]));
            }
            return FunctionWrapper.join2(
                buf,
                FunctionWrapper.create1(OpCodes.OP_FUNCTION_INVOKE, funcInvoke.opToken, null, argc));
        }

        private static ByteCodeBuffer serializeFunctionReference(Expression funcRef)
        {
            AbstractEntity funcDef = (AbstractEntity)funcRef.objPtr;
            int index = funcDef.serializationIndex;
            if (index == -1) throw new InvalidOperationException();
            return FunctionWrapper.create1(OpCodes.OP_PUSH_FUNC_PTR, funcRef.firstToken, null, index);
        }

        private static ByteCodeBuffer serializeIndex(Expression index)
        {
            return FunctionWrapper.join3(
                serializeExpression(index.root),
                serializeExpression(index.right),
                FunctionWrapper.create0(OpCodes.OP_INDEX, index.opToken, null)
            );
        }

        private static ByteCodeBuffer serializeInlineIncrement(Expression ii)
        {
            switch (ii.root.type)
            {
                case ExpressionType.VARIABLE: return serializeInlineIncrementVar(ii);
                case ExpressionType.INDEX: return serializeInlineIncrementIndex(ii);
                case ExpressionType.DOT_FIELD: return serializeInlineIncrementDotField(ii);
                default:
                    break;
            }
            throw new InvalidOperationException(); // should have been caught before now.
        }

        private static ByteCodeBuffer serializeInlineIncrementVar(Expression ii)
        {
            // TODO: expression as statement -- remove duplication and pop.

            if (ii.boolVal) // is prefix? duplicate after incrementation
            {
                return FunctionWrapper.join4(
                    serializeExpression(ii.root),
                    FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                    FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                    FunctionWrapper.create0(OpCodes.OP_ASSIGN_VAR, null, ii.root.strVal)
                );
            }
            else // suffix. duplicate before incrementation
            {
                return FunctionWrapper.join4(
                    serializeExpression(ii.root),
                    FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                    FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                    FunctionWrapper.create0(OpCodes.OP_ASSIGN_VAR, null, ii.root.strVal)
                );
            }
        }

        private static ByteCodeBuffer serializeInlineIncrementDotField(Expression ii)
        {
            ByteCodeBuffer root = serializeExpression(ii.root.root);
            bool isPrefix = ii.boolVal;

            return FunctionWrapper.join5(

                // []
                root,
                // [root]
                FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                // [root, root]
                FunctionWrapper.create0(OpCodes.OP_DOT_FIELD, ii.root.opToken, ii.root.strVal),
                // [root, originalValue]
                isPrefix
                    ? FunctionWrapper.join2(
                        FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                        // [root, return/finalValue]
                        FunctionWrapper.create0(OpCodes.OP_STACK_DO_SI_DUP_1, null, null))
                    // [returnValue, finalValue, root]
                    : FunctionWrapper.join3(
                        FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                        // [root, returnValue, originalValue]
                        FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                        // [root, returnValue, finalValue]
                        FunctionWrapper.create0(OpCodes.OP_STACK_DO_SI_DUP_2, null, null)),
                // [returnValue, finalValue, root]

                // [returnValue, finalValue, root]
                FunctionWrapper.create0(OpCodes.OP_ASSIGN_FIELD, ii.opToken, ii.root.strVal)
            // [returnValue]
            );

            /*
            return ByteCode.join2(
                serializeExpression(df.root),
                ByteCode.create0(OpCodes.OP_DOT_FIELD, df.opToken, df.strVal));
            //*/
        }

        private static ByteCodeBuffer serializeInlineIncrementIndex(Expression ii)
        {
            ByteCodeBuffer root = serializeExpression(ii.root.root);
            ByteCodeBuffer index = serializeExpression(ii.root.right);
            return FunctionWrapper.join7(
                root, // root 
                index, // root, index
                FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE_2, null, null), // root, index, root, index 
                FunctionWrapper.create0(OpCodes.OP_INDEX, ii.root.opToken, null), // root, index, value
                ii.boolVal
                    ? FunctionWrapper.join2(
                        FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1), // root, index, value+1
                        FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null) // root, index, returnValue, value+1
                    )
                    : FunctionWrapper.join2(
                        FunctionWrapper.create0(OpCodes.OP_STACK_DUPLICATE, null, null), // root, index, returnValue, value
                        FunctionWrapper.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1) // root, index, returnValue, value+1
                    ),
                FunctionWrapper.create0(OpCodes.OP_STACK_DO_SI_DO_4A, null, null), // returnValue, value+1, root, index
                FunctionWrapper.create0(OpCodes.OP_ASSIGN_INDEX, ii.root.opToken, null)
            );
        }

        private static ByteCodeBuffer serializeIntegerConstant(Expression intConst)
        {
            return FunctionWrapper.create1(OpCodes.OP_PUSH_INT, intConst.firstToken, null, intConst.intVal);
        }

        private static ByteCodeBuffer serializeLambda(Expression lambda)
        {
            FunctionLikeEntity lambdaEntity = (FunctionLikeEntity)lambda.objPtr;
            return FunctionWrapper.create1(OpCodes.OP_PUSH_LAMBDA, lambda.firstToken, null, lambdaEntity.serializationIndex);
        }

        private static ByteCodeBuffer serializeListDefinition(Expression listDef)
        {
            ByteCodeBuffer buf = null;
            for (int i = 0; i < listDef.values.Length; i++)
            {
                buf = FunctionWrapper.join2(buf, serializeExpression(listDef.values[i]));
            }
            return FunctionWrapper.join2(
                buf,
                FunctionWrapper.create1(OpCodes.OP_BUILD_LIST, listDef.firstToken, null, listDef.values.Length));
        }

        private static ByteCodeBuffer serializeNegativeSign(Expression negSign)
        {
            return FunctionWrapper.join2(
                serializeExpression(negSign.root),
                FunctionWrapper.create0(OpCodes.OP_NEGATIVE_SIGN, negSign.opToken, null)
            );
        }

        private static ByteCodeBuffer serializeNullConstant(Expression nullConst)
        {
            return FunctionWrapper.create0(OpCodes.OP_PUSH_NULL, nullConst.firstToken, null);
        }

        private static ByteCodeBuffer serializeSlice(Expression slice)
        {
            Expression start = slice.args[0];
            Expression end = slice.args[1];
            Expression step = slice.args[2];
            int sliceMask =
                (start != null ? 1 : 0) |
                (end != null ? 2 : 0) |
                (step != null ? 4 : 0);

            return FunctionWrapper.join5(
                serializeExpression(slice.root),
                start != null ? ByteCode.ensureIntegerExpression(start.firstToken, serializeExpression(start)) : null,
                end != null ? ByteCode.ensureIntegerExpression(end.firstToken, serializeExpression(end)) : null,
                step != null ? ByteCode.ensureIntegerExpression(step.firstToken, serializeExpression(step)) : null,
                FunctionWrapper.create1(OpCodes.OP_SLICE, slice.opToken, null, sliceMask));
        }

        private static ByteCodeBuffer serializeStringConstant(Expression strConst)
        {
            return FunctionWrapper.create0(OpCodes.OP_PUSH_STRING, strConst.firstToken, strConst.strVal);
        }

        private static ByteCodeBuffer serializeTernary(Expression ternaryExpression)
        {
            ByteCodeBuffer condBuf = serializeExpression(ternaryExpression.root);
            ByteCodeBuffer leftBuf = serializeExpression(ternaryExpression.left);
            ByteCodeBuffer rightBuf = serializeExpression(ternaryExpression.right);
            condBuf = ByteCode.ensureBooleanExpression(ternaryExpression.opToken, condBuf);

            return FunctionWrapper.join5(
                condBuf,
                FunctionWrapper.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, leftBuf.length + 1),
                leftBuf,
                FunctionWrapper.create1(OpCodes.OP_JUMP, null, null, rightBuf.length),
                rightBuf);
        }

        private static ByteCodeBuffer serializeThis(Expression thisKeyword)
        {
            return FunctionWrapper.create0(OpCodes.OP_PUSH_THIS, thisKeyword.firstToken, null);
        }

        private static ByteCodeBuffer serializeTypeOf(Expression typeOfExpr)
        {
            ByteCodeBuffer root = serializeExpression(typeOfExpr.root);
            return FunctionWrapper.join2(
                root,
                FunctionWrapper.create0(OpCodes.OP_TYPEOF, typeOfExpr.firstToken, null));
        }

        private static ByteCodeBuffer serializeVariable(Expression v)
        {
            if (v.strVal == "print") throw new InvalidOperationException();
            return FunctionWrapper.create0(OpCodes.OP_PUSH_VAR, v.firstToken, v.strVal);
        }
    }
}
