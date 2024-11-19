using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class Serializer
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
                case ExpressionType.VARIABLE: return serializeVariable(expr);
                default:
                    throw new NotImplementedException();
            }
        }

        public static ByteCodeBuffer serializeStatement(Statement stmnt)
        {
            switch (stmnt.type)
            {
                case StatementType.ASSIGNMENT:
                    string op = stmnt.assignOp.Value;
                    op = op == "=" ? op : op.Substring(0, op.Length - 1);
                    switch (stmnt.assignTarget.type)
                    {
                        case ExpressionType.VARIABLE: return serializeAssignVariable(stmnt, op);
                        case ExpressionType.INDEX: return serializeAssignIndex(stmnt, op);
                        case ExpressionType.DOT_FIELD: return serializeAssignField(stmnt, op);
                    }
                    throw new InvalidOperationException(); // should have been removed by now.

                case StatementType.BREAK: return serializeBreak(stmnt);
                case StatementType.CONTINUE: return serializeContinue(stmnt);
                case StatementType.DO_WHILE_LOOP: return serializeDoWhileLoop(stmnt);
                case StatementType.EXPRESSION_AS_STATEMENT: return serializeExpressionStatement(stmnt);
                case StatementType.FOR_LOOP: return serializeForLoop(stmnt);
                case StatementType.IF_STATEMENT: return serializeIfStatement(stmnt);
                case StatementType.RETURN: return serializeReturn(stmnt);
                case StatementType.SWITCH_STATEMENT: return serializeSwitchStatement(stmnt);
                case StatementType.THROW: return serializeThrowStatement(stmnt);
                case StatementType.TRY: return serializeTryStatement(stmnt);
                case StatementType.WHILE_LOOP: return serializeWhileLoop(stmnt);

                default: throw new NotImplementedException();
            }
        }

        public static ByteCodeBuffer serializeCodeBlock(Statement[] block)
        {
            ByteCodeBuffer buf = null;
            for (int i = 0; i < block.Length; i++)
            {
                buf = ByteCode.join2(buf, serializeStatement(block[i]));
            }
            return buf;
        }

        ///////////////////////////////////////////////////

        private static ByteCodeBuffer serializeAssignVariable(Statement assignVar, string baseOp)
        {
            ByteCodeBuffer bufVal = serializeExpression(assignVar.assignValue);
            ByteCodeBuffer bufVar = serializeExpression(assignVar.assignTarget);
            if (baseOp != "=")
            {
                bufVal = ByteCode.join3(
                    bufVar,
                    bufVal,
                    ByteCode.create0(OpCodes.OP_BIN_OP, assignVar.assignOp, baseOp));
            }

            return ByteCode.join2(
                bufVal,
                ByteCode.create0(OpCodes.OP_ASSIGN_VAR, assignVar.assignOp, assignVar.assignTarget.strVal));
        }

        private static ByteCodeBuffer serializeAssignIndex(Statement assignIndex, string baseOp)
        {
            Token bracketToken = assignIndex.assignTarget.opToken;
            ByteCodeBuffer bufVal = serializeExpression(assignIndex.assignValue);
            ByteCodeBuffer bufIndex = serializeExpression(assignIndex.assignTarget.right);
            ByteCodeBuffer bufRoot = serializeExpression(assignIndex.assignTarget.root);
            if (baseOp != "=")
            {
                ByteCodeBuffer incrBuf = ByteCode.join3(
                    bufRoot,
                    bufIndex,
                    ByteCode.create0(OpCodes.OP_STACK_DUPLICATE_2, null, null));
                // [root, index, root, index] 
                incrBuf = ByteCode.join2(incrBuf, ByteCode.create0(OpCodes.OP_INDEX, bracketToken, null));
                // [root, index, originalValue]
                incrBuf = ByteCode.join3(incrBuf, bufVal, ByteCode.create0(OpCodes.OP_BIN_OP, assignIndex.assignOp, baseOp));
                // [root, index, finalValue]
                return ByteCode.join2(incrBuf, ByteCode.create1(OpCodes.OP_ASSIGN_INDEX, assignIndex.assignOp, null, 1));
            }

            return ByteCode.join4(
                bufVal,
                bufRoot,
                bufIndex,
                ByteCode.create0(OpCodes.OP_ASSIGN_INDEX, assignIndex.assignOp, null)
            );
        }

        private static ByteCodeBuffer serializeAssignField(Statement assignField, string baseOp)
        {
            Expression df = assignField.assignTarget;
            string fieldName = df.strVal;
            ByteCodeBuffer bufVal = serializeExpression(assignField.assignValue);
            ByteCodeBuffer bufRoot = serializeExpression(df.root);
            if (baseOp != "=")
            {
                ByteCodeBuffer incrBuf = ByteCode.join2(
                    bufRoot,
                    ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null));
                // [root, root]
                incrBuf = ByteCode.join2(incrBuf, ByteCode.create0(OpCodes.OP_DOT_FIELD, df.opToken, fieldName));
                // [root, originalValue]
                incrBuf = ByteCode.join3(incrBuf, bufVal, ByteCode.create0(OpCodes.OP_BIN_OP, assignField.assignOp, baseOp));
                // [root, finalValue]
                return ByteCode.join2(
                    incrBuf,
                    ByteCode.create1(OpCodes.OP_ASSIGN_FIELD, assignField.assignOp, fieldName, 1));
            }

            return ByteCode.join3(
                bufVal,
                bufRoot,
                ByteCode.create0(OpCodes.OP_ASSIGN_FIELD, assignField.assignOp, fieldName)
            );
        }

        private static ByteCodeBuffer serializeBaseCtorReference(Expression baseCtor)
        {
            AbstractEntity baseClass = (AbstractEntity)baseCtor.objPtr;
            return ByteCode.create1(OpCodes.OP_PUSH_BASE_CTOR, baseCtor.firstToken, null, baseClass.serializationIndex);
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
                return ByteCode.join3(
                    leftBuf,
                    ByteCode.create1(OpCodes.OP_POP_IF_NULL_OR_JUMP, opToken, null, rightBuf.length),
                    rightBuf);
            }

            if (op == "&&" || op == "||")
            {
                leftBuf = ByteCode.ensureBooleanExpression(left.firstToken, leftBuf);
                rightBuf = ByteCode.ensureBooleanExpression(right.firstToken, rightBuf);
                return ByteCode.join3(
                    leftBuf,
                    ByteCode.create1(
                        op == "&&" ? OpCodes.OP_POP_IF_TRUE_OR_JUMP : OpCodes.OP_POP_IF_FALSE_OR_JUMP,
                        null, null, rightBuf.length),
                    rightBuf);
            }

            return ByteCode.join3(
                leftBuf,
                rightBuf,
                ByteCode.create0(OpCodes.OP_BIN_OP, opToken, op)
            );
        }

        private static ByteCodeBuffer serializeBreak(Statement br)
        {
            return ByteCode.create0(OpCodes.OP_BREAK_DUMMY, br.firstToken, null);
        }

        private static ByteCodeBuffer serializeContinue(Statement cont)
        {
            return ByteCode.create0(OpCodes.OP_CONTINUE_DUMMY, cont.firstToken, null);
        }

        private static ByteCodeBuffer serializeDotField(Expression df)
        {
            return ByteCode.join2(
                serializeExpression(df.root),
                ByteCode.create0(OpCodes.OP_DOT_FIELD, df.opToken, df.strVal));
        }

        private static ByteCodeBuffer serializeDoWhileLoop(Statement doWhileLoop)
        {
            ByteCodeBuffer body = serializeCodeBlock(doWhileLoop.code);
            ByteCodeBuffer condition = ByteCode.ensureBooleanExpression(doWhileLoop.condition.firstToken, serializeExpression(doWhileLoop.condition));
            return ByteCode.join4(
                finalizeBreakContinue(body, condition.length + 2, true, 0),
                condition,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, 1),
                ByteCode.create1(OpCodes.OP_JUMP, null, null, -(1 + 1 + condition.length + body.length)));
        }

        private static ByteCodeBuffer serializeExpressionStatement(Statement exprStmnt)
        {
            return ByteCode.join2(
                serializeExpression(exprStmnt.expression),
                ByteCode.create0(OpCodes.OP_POP, null, null)
            );
        }

        private static readonly Dictionary<string, int> SPECIAL_ACTION_BY_FUNC_NAME = new Dictionary<string, int>()
        {
            { "math_sin", SpecialActionCodes.MATH_SIN },
            { "math_cos", SpecialActionCodes.MATH_COS },
            { "math_tan", SpecialActionCodes.MATH_TAN },
            { "math_arcsin", SpecialActionCodes.MATH_ARCSIN },
            { "math_arccos", SpecialActionCodes.MATH_ARCCOS },
            { "math_arctan", SpecialActionCodes.MATH_ARCTAN },
            { "math_log", SpecialActionCodes.MATH_LOG },
            { "parse_int", SpecialActionCodes.PARSE_INT },
        };

        private static ByteCodeBuffer serializeExtensionInvocation(Expression extInvoke)
        {
            ByteCodeBuffer buf = null;
            int argc = extInvoke.args.Length;
            for (int i = 0; i < argc; i++)
            {
                buf = ByteCode.join2(buf, serializeExpression(extInvoke.args[i]));
            }

            switch (extInvoke.strVal)
            {
                case "cmp":
                    return ByteCode.join3(
                        serializeExpression(extInvoke.args[0]),
                        serializeExpression(extInvoke.args[1]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.CMP));

                case "math_floor":
                    return ByteCode.join2(
                        buf,
                        ByteCode.create0(OpCodes.OP_MATH_FLOOR, extInvoke.firstToken, null));

                case "random_float":
                    return ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.RANDOM_FLOAT);

                case "unix_time":
                    return ByteCode.create2(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.UNIX_TIME, extInvoke.args[0].intVal);

                case "sort_get_next_cmp":
                    return ByteCode.join3(
                        serializeExpression(extInvoke.args[0]),
                        serializeExpression(extInvoke.args[1]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.SORT_GET_NEXT_CMP));

                case "sort_start":
                    return ByteCode.join3(
                        serializeExpression(extInvoke.args[0]),
                        serializeExpression(extInvoke.args[1]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.SORT_START));

                case "sort_proceed":
                    return ByteCode.join3(
                        serializeExpression(extInvoke.args[0]),
                        serializeExpression(extInvoke.args[1]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.SORT_PROCEED));

                case "sort_end":
                    return ByteCode.join2(
                        serializeExpression(extInvoke.args[0]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.SORT_END));

                case "math_sin":
                case "math_cos":
                case "math_tan":
                case "math_arccos":
                case "math_arcsin":
                case "parse_int":
                    return ByteCode.join2(
                        serializeExpression(extInvoke.args[0]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SPECIAL_ACTION_BY_FUNC_NAME[extInvoke.strVal]));

                case "math_arctan":
                case "math_log":
                    return ByteCode.join3(
                        serializeExpression(extInvoke.args[0]),
                        serializeExpression(extInvoke.args[1]),
                        ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SPECIAL_ACTION_BY_FUNC_NAME[extInvoke.strVal]));

                default:
                    return ByteCode.join2(
                        buf,
                        ByteCode.create1(OpCodes.OP_EXT_INVOKE, extInvoke.firstToken, extInvoke.strVal, argc));
            }
        }

        private static ByteCodeBuffer serializeBitwiseNot(Expression bwn)
        {
            return ByteCode.join2(
                ByteCode.ensureIntegerExpression(bwn.root.firstToken, serializeExpression(bwn.root)),
                ByteCode.create0(OpCodes.OP_BITWISE_NOT, bwn.firstToken, null)
                );
        }

        private static ByteCodeBuffer serializeBoolConst(Expression bc)
        {
            return ByteCode.create1(OpCodes.OP_PUSH_BOOL, bc.firstToken, null, bc.boolVal ? 1 : 0); ;
        }

        private static ByteCodeBuffer serializeBooleanNot(Expression bn)
        {
            // OP_BOOLEAN_NOT includes assertions for a boolean value, so ensureBooleanExpression is not needed here.
            // ! unary op is unlikely to be used in expressions that are verifiable as boolean at compile time, so it's faster to just bundle them together. 
            return ByteCode.join2(
                serializeExpression(bn.root),
                ByteCode.create0(OpCodes.OP_BOOLEAN_NOT, bn.firstToken, null));
        }

        private static ByteCodeBuffer serializeClassReference(Expression classRef)
        {
            return ByteCode.create1(OpCodes.OP_PUSH_CLASS_REF, classRef.firstToken, null, ((AbstractEntity)classRef.objPtr).serializationIndex);
        }

        private static ByteCodeBuffer serializeConstructorInvocation(Expression ctorInvoke)
        {
            AbstractEntity classDef = (AbstractEntity)ctorInvoke.objPtr;
            ByteCodeBuffer buf = null;
            for (int i = 0; i < ctorInvoke.args.Length; i++)
            {
                buf = ByteCode.join2(buf, serializeExpression(ctorInvoke.args[i]));
            }
            return ByteCode.join3(
                ByteCode.create1(OpCodes.OP_CTOR_REF, ctorInvoke.firstToken, null, classDef.serializationIndex),
                buf,
                ByteCode.create1(OpCodes.OP_FUNCTION_INVOKE, ctorInvoke.opToken, null, ctorInvoke.args.Length));
        }

        private static ByteCodeBuffer serializeDictionaryDefinition(Expression dictDef)
        {
            ByteCodeBuffer buf = null;
            for (int i = 0; i < dictDef.keys.Length; i++)
            {
                buf = ByteCode.join3(
                    buf,
                    serializeExpression(dictDef.keys[i]),
                    serializeExpression(dictDef.values[i]));
            }
            return ByteCode.join2(
                buf,
                ByteCode.create1(OpCodes.OP_BUILD_DICT, dictDef.firstToken, null, dictDef.keys.Length));
        }

        private static ByteCodeBuffer serializeFloatConstant(Expression floatConst)
        {
            double val = floatConst.floatVal;
            if (val * 4 % 1 == 0)
            {
                return ByteCode.create1(OpCodes.OP_PUSH_FLOAT, null, null, (int)(val * 4));
            }
            return ByteCode.create0(OpCodes.OP_PUSH_FLOAT, null, val + "");
        }

        private static ByteCodeBuffer serializeFunctionInvocation(Expression funcInvoke)
        {
            ByteCodeBuffer buf = serializeExpression(funcInvoke.root);
            int argc = funcInvoke.args.Length;
            for (int i = 0; i < argc; i++)
            {
                buf = ByteCode.join2(buf, serializeExpression(funcInvoke.args[i]));
            }
            return ByteCode.join2(
                buf,
                ByteCode.create1(OpCodes.OP_FUNCTION_INVOKE, funcInvoke.opToken, null, argc));
        }

        private static ByteCodeBuffer serializeForLoop(Statement forLoop)
        {
            Expression condition = forLoop.condition;
            Statement[] code = forLoop.code;
            Statement[] init = forLoop.forInit;
            Statement[] step = forLoop.forStep;


            ByteCodeBuffer bufInit = serializeCodeBlock(init);
            ByteCodeBuffer bufStep = serializeCodeBlock(step);
            ByteCodeBuffer bufBody = serializeCodeBlock(code);
            ByteCodeBuffer bufCondition = serializeExpression(condition);

            bufCondition = ByteCode.ensureBooleanExpression(condition.firstToken, bufCondition);

            int stepSize = 0;
            int bodySize = 0;
            int conditionSize = bufCondition.length;
            if (bufStep != null) stepSize = bufStep.length;
            if (bufBody != null) bodySize = bufBody.length;

            return ByteCode.join6(
                bufInit,
                bufCondition,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, bodySize + stepSize + 1),
                finalizeBreakContinue(bufBody, bufStep.length + 1, true, 0),
                bufStep,
                ByteCode.create1(OpCodes.OP_JUMP, null, null, -(1 + bodySize + stepSize + 1 + conditionSize)));

            throw new NotImplementedException();
        }

        private static ByteCodeBuffer serializeFunctionReference(Expression funcRef)
        {
            AbstractEntity funcDef = (AbstractEntity)funcRef.objPtr;
            int index = funcDef.serializationIndex;
            if (index == -1) throw new InvalidOperationException();
            return ByteCode.create1(OpCodes.OP_PUSH_FUNC_PTR, funcRef.firstToken, null, index);
        }

        private static ByteCodeBuffer serializeIfStatement(Statement ifStmnt)
        {
            Expression condition = ifStmnt.condition;
            Statement[] ifCode = ifStmnt.code;
            Statement[] elseCode = ifStmnt.elseCode; // if not present, it's an empty array, not null

            ByteCodeBuffer buf = serializeExpression(condition);
            ByteCodeBuffer bufTrue = serializeCodeBlock(ifCode);
            ByteCodeBuffer bufFalse = serializeCodeBlock(elseCode);
            buf = ByteCode.ensureBooleanExpression(condition.firstToken, buf);

            int trueSize = 0;
            int falseSize = 0;
            if (bufTrue != null) trueSize = bufTrue.length;
            if (bufFalse != null) falseSize = bufFalse.length;

            if (trueSize + falseSize == 0)
            {
                return ByteCode.join2(buf, ByteCode.create0(OpCodes.OP_POP, null, null));
            }

            if (falseSize == 0)
            {
                return ByteCode.join3(
                    buf,
                    ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, trueSize),
                    bufTrue);
            }

            if (trueSize == 0)
            {
                return ByteCode.join3(
                    buf,
                    ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_TRUE, null, null, falseSize),
                    bufFalse);
            }

            return ByteCode.join5(
                buf,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, trueSize + 1),
                bufTrue,
                ByteCode.create1(OpCodes.OP_JUMP, null, null, falseSize),
                bufFalse);

            throw new NotImplementedException();
        }

        private static ByteCodeBuffer serializeIndex(Expression index)
        {
            return ByteCode.join3(
                serializeExpression(index.root),
                serializeExpression(index.right),
                ByteCode.create0(OpCodes.OP_INDEX, index.opToken, null)
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
                return ByteCode.join4(
                    serializeExpression(ii.root),
                    ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                    ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                    ByteCode.create0(OpCodes.OP_ASSIGN_VAR, null, ii.root.strVal)
                );
            }
            else // suffix. duplicate before incrementation
            {
                return ByteCode.join4(
                    serializeExpression(ii.root),
                    ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                    ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                    ByteCode.create0(OpCodes.OP_ASSIGN_VAR, null, ii.root.strVal)
                );
            }
        }

        private static ByteCodeBuffer serializeInlineIncrementDotField(Expression ii)
        {
            ByteCodeBuffer root = serializeExpression(ii.root.root);
            bool isPrefix = ii.boolVal;

            return ByteCode.join5(

                // []
                root,
                // [root]
                ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                // [root, root]
                ByteCode.create0(OpCodes.OP_DOT_FIELD, ii.root.opToken, ii.root.strVal),
                // [root, originalValue]
                isPrefix
                    ? ByteCode.join2(
                        ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                        // [root, return/finalValue]
                        ByteCode.create0(OpCodes.OP_STACK_DO_SI_DUP_1, null, null))
                    // [returnValue, finalValue, root]
                    : ByteCode.join3(
                        ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null),
                        // [root, returnValue, originalValule]
                        ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1),
                        // [root, returnValue, finalValue]
                        ByteCode.create0(OpCodes.OP_STACK_DO_SI_DUP_2, null, null)),
                // [returnValue, finalValue, root]

                // [returnValue, finalValue, root]
                ByteCode.create0(OpCodes.OP_ASSIGN_FIELD, ii.opToken, ii.root.strVal)
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
            return ByteCode.join7(
                root, // root 
                index, // root, index
                ByteCode.create0(OpCodes.OP_STACK_DUPLICATE_2, null, null), // root, index, root, index 
                ByteCode.create0(OpCodes.OP_INDEX, ii.root.opToken, null), // root, index, value
                ii.boolVal
                    ? ByteCode.join2(
                        ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1), // root, index, value+1
                        ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null) // root, index, returnValue, value+1
                    )
                    : ByteCode.join2(
                        ByteCode.create0(OpCodes.OP_STACK_DUPLICATE, null, null), // root, index, returnValue, value
                        ByteCode.create1(OpCodes.OP_INT_INCR, ii.opToken, null, ii.opToken.Value == "++" ? 1 : -1) // root, index, returnValue, value+1
                    ),
                ByteCode.create0(OpCodes.OP_STACK_DO_SI_DO_4A, null, null), // returnValue, value+1, root, index
                ByteCode.create0(OpCodes.OP_ASSIGN_INDEX, ii.root.opToken, null)
            );
        }

        private static ByteCodeBuffer serializeIntegerConstant(Expression intConst)
        {
            return ByteCode.create1(OpCodes.OP_PUSH_INT, intConst.firstToken, null, intConst.intVal);
        }

        private static ByteCodeBuffer serializeLambda(Expression lambda)
        {
            LambdaEntity lambdaEntity = (LambdaEntity)lambda.objPtr;
            return ByteCode.create1(OpCodes.OP_PUSH_LAMBDA, lambda.firstToken, null, lambdaEntity.serializationIndex);
        }

        private static ByteCodeBuffer serializeListDefinition(Expression listDef)
        {
            ByteCodeBuffer buf = null;
            for (int i = 0; i < listDef.values.Length; i++)
            {
                buf = ByteCode.join2(buf, serializeExpression(listDef.values[i]));
            }
            return ByteCode.join2(
                buf,
                ByteCode.create1(OpCodes.OP_BUILD_LIST, listDef.firstToken, null, listDef.values.Length));
        }

        private static ByteCodeBuffer serializeNegativeSign(Expression negSign)
        {
            return ByteCode.join2(
                serializeExpression(negSign.root),
                ByteCode.create0(OpCodes.OP_NEGATIVE_SIGN, negSign.opToken, null)
            );
        }

        private static ByteCodeBuffer serializeNullConstant(Expression nullConst)
        {
            return ByteCode.create0(OpCodes.OP_PUSH_NULL, nullConst.firstToken, null);
        }

        private static ByteCodeBuffer serializeReturn(Statement ret)
        {
            ByteCodeBuffer buf;
            if (ret.expression == null)
            {
                buf = ByteCode.create0(OpCodes.OP_PUSH_NULL, null, null);
            }
            else
            {
                buf = serializeExpression(ret.expression);
            }
            return ByteCode.join2(buf, ByteCode.create0(OpCodes.OP_RETURN, ret.firstToken, null));
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

            return ByteCode.join5(
                serializeExpression(slice.root),
                start != null ? ByteCode.ensureIntegerExpression(start.firstToken, serializeExpression(start)) : null,
                end != null ? ByteCode.ensureIntegerExpression(end.firstToken, serializeExpression(end)) : null,
                step != null ? ByteCode.ensureIntegerExpression(step.firstToken, serializeExpression(step)) : null,
                ByteCode.create1(OpCodes.OP_SLICE, slice.opToken, null, sliceMask));
        }

        private static ByteCodeBuffer serializeStringConstant(Expression strConst)
        {
            return ByteCode.create0(OpCodes.OP_PUSH_STRING, strConst.firstToken, strConst.strVal);
        }

        private static ByteCodeBuffer serializeSwitchStatement(Statement switchStmnt)
        {
            // switch (cond) { } <-- needs to ensure that the condition is an int or string.
            if (switchStmnt.switchChunks.Length == 0)
            {
                return ByteCode.join3(
                    serializeExpression(switchStmnt.condition),
                    ByteCode.create0(OpCodes.OP_ENSURE_INT_OR_STRING, switchStmnt.condition.firstToken, null),
                    ByteCode.create0(OpCodes.OP_POP, null, null));
            }

            int conditionTypeEnsuranceOpCode = OpCodes.OP_ENSURE_INT_OR_STRING;
            Expression firstCaseExpr = switchStmnt.switchChunks[0].Cases[0];
            if (firstCaseExpr != null)
            {
                if (firstCaseExpr.type == ExpressionType.INTEGER_CONST)
                {
                    conditionTypeEnsuranceOpCode = OpCodes.OP_ENSURE_INT;
                }
                else if (firstCaseExpr.type == ExpressionType.STRING_CONST)
                {
                    conditionTypeEnsuranceOpCode = OpCodes.OP_ENSURE_STRING;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            ByteCodeBuffer condBuf = ByteCode.join2(
                serializeExpression(switchStmnt.condition),
                ByteCode.create0(conditionTypeEnsuranceOpCode, switchStmnt.condition.firstToken, null));

            ByteCodeBuffer caseBuf = null;
            int currentJumpOffset = 0;
            bool hasDefault = false;
            Dictionary<string, int> stringJumpOffset = new Dictionary<string, int>();
            Dictionary<int, int> intJumpOffset = new Dictionary<int, int>();
            int defaultJumpOffset = -1;
            foreach (SwitchChunk chunk in switchStmnt.switchChunks)
            {
                foreach (Expression expr in chunk.Cases)
                {
                    if (expr == null)
                    {
                        defaultJumpOffset = currentJumpOffset;
                        hasDefault = true;
                    }
                    else if (conditionTypeEnsuranceOpCode == OpCodes.OP_ENSURE_INT)
                    {
                        intJumpOffset[expr.intVal] = currentJumpOffset;
                    }
                    else if (conditionTypeEnsuranceOpCode == OpCodes.OP_ENSURE_STRING)
                    {
                        stringJumpOffset[expr.strVal] = currentJumpOffset;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                ByteCodeBuffer chunkBuf = serializeCodeBlock(chunk.Code.ToArray());
                currentJumpOffset += chunkBuf.length;
                caseBuf = ByteCode.join2(caseBuf, chunkBuf);
            }

            if (!hasDefault) defaultJumpOffset = currentJumpOffset; // bypass all if no default provided

            caseBuf = finalizeBreakContinue(caseBuf, 0, false, 0);

            ByteCodeBuffer jumpBuf = null;

            /*
                OP_SWITCH_BUILD count defaultJumpOffset type (1 = int, 2 = string)
                
                OP_SWITCH_ADD offset [str value]
                ... repeated
                    OR 
                OP_SWITCH_ADD -1 offset1, value1, offset2, value2, etc...
                OP_SWITCH_FINALIZE (no args)

                This will:
                - initialize a native dictionary of the correct type
                - add items to that dictionary for the given offset
                - finalize by jumping BACK to the OP_SWITCH_BUILD row and swapping it out with a OP_SWITCH_INT or OP_SWITCH_STR instruction.
                - gives the created dictionary an ID# and adds it to a lookup array on the execution context.

                All offsets are the jump offset from the final OP_SWITCH_INT so no adjusting needs to occur at runtime.

                OP_SWITCH_INT/STR id
                The lookup table is located on the execution context
            */

            if (conditionTypeEnsuranceOpCode == OpCodes.OP_ENSURE_INT)
            {
                List<int> jumpArgs = new List<int>() { -1 };
                foreach (int k in intJumpOffset.Keys.OrderBy(k => k).ToArray())
                {
                    jumpArgs.Add(intJumpOffset[k] + 2);
                    jumpArgs.Add(k);
                }

                ByteCodeBuffer lookup = ByteCode.create0(OpCodes.OP_SWITCH_ADD, null, null);
                lookup.row.args = jumpArgs.ToArray();

                jumpBuf = ByteCode.join3(
                    ByteCode.create2(OpCodes.OP_SWITCH_BUILD, null, null, defaultJumpOffset + 2, 1),
                    lookup,
                    ByteCode.create1(OpCodes.OP_SWITCH_FINALIZE, null, null, 2)
                );

            }
            else if (conditionTypeEnsuranceOpCode == OpCodes.OP_ENSURE_STRING)
            {
                string[] keys = stringJumpOffset.Keys.OrderBy(k => k).ToArray();
                jumpBuf = ByteCode.create2(OpCodes.OP_SWITCH_BUILD, null, null, defaultJumpOffset + keys.Length + 1, 2);
                foreach (string key in keys)
                {
                    jumpBuf = ByteCode.join2(
                        jumpBuf,
                        ByteCode.create1(OpCodes.OP_SWITCH_ADD, null, key, stringJumpOffset[key] + keys.Length + 1));
                }
                /*
                ByteCodeRow[] rows = ByteCode.flatten(jumpBuf);
                int additionalOffset = jumpBuf.length - 1;
                for (int i = 1; i < rows.Length; i++)
                {
                    rows[i].args[0] += additionalOffset;
                }//*/

                jumpBuf = ByteCode.join2(
                    jumpBuf,
                    ByteCode.create1(OpCodes.OP_SWITCH_FINALIZE, null, null, jumpBuf.length));
            }
            else
            {
                jumpBuf = ByteCode.create0(OpCodes.OP_POP, null, null);
            }

            return ByteCode.join3(condBuf, jumpBuf, caseBuf);
        }

        private static ByteCodeBuffer finalizeBreakContinue(ByteCodeBuffer originalBuffer, int additionalBreakOffset, bool allowContinue, int additionalContinueOffset)
        {
            ByteCodeRow[] rows = ByteCode.flatten(originalBuffer);
            for (int i = 0; i < rows.Length; i++)
            {
                int op = rows[i].opCode;
                if (op == OpCodes.OP_BREAK_DUMMY || op == OpCodes.OP_CONTINUE_DUMMY)
                {
                    int additionalOffset = additionalBreakOffset;
                    if (op == OpCodes.OP_CONTINUE_DUMMY)
                    {
                        if (!allowContinue) throw new InvalidOperationException(); // this should have been caught before now. 
                        additionalOffset = additionalContinueOffset;
                    }
                    rows[i].opCode = OpCodes.OP_JUMP;
                    int offsetToEnd = rows.Length - i - 1 + additionalOffset;
                    rows[i].args = new int[] { offsetToEnd };
                }
            }

            ByteCodeBuffer output = null;
            for (int i = 0; i < rows.Length; i++)
            {
                output = ByteCode.join2(output, new ByteCodeBuffer(rows[i]));
            }

            return output;
        }

        private static ByteCodeBuffer serializeTernary(Expression ternaryExpression)
        {
            ByteCodeBuffer condBuf = serializeExpression(ternaryExpression.root);
            ByteCodeBuffer leftBuf = serializeExpression(ternaryExpression.left);
            ByteCodeBuffer rightBuf = serializeExpression(ternaryExpression.right);
            condBuf = ByteCode.ensureBooleanExpression(ternaryExpression.opToken, condBuf);

            return ByteCode.join5(
                condBuf,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, leftBuf.length + 1),
                leftBuf,
                ByteCode.create1(OpCodes.OP_JUMP, null, null, rightBuf.length),
                rightBuf);
        }

        private static ByteCodeBuffer serializeThis(Expression thisKeyword)
        {
            return ByteCode.create0(OpCodes.OP_PUSH_THIS, thisKeyword.firstToken, null);
        }

        private static ByteCodeBuffer serializeThrowStatement(Statement thrw)
        {
            return ByteCode.join2(
                serializeExpression(thrw.expression),
                ByteCode.create0(OpCodes.OP_THROW, thrw.firstToken, null));
        }

        private static ByteCodeBuffer serializeVariable(Expression v)
        {
            if (v.strVal == "print") throw new InvalidOperationException();
            return ByteCode.create0(OpCodes.OP_PUSH_VAR, v.firstToken, v.strVal);
        }

        private static ByteCodeBuffer serializeTryStatement(Statement tryStmnt)
        {
            ByteCodeBuffer tryBuf = serializeCodeBlock(tryStmnt.code);
            List<ByteCodeBuffer> catchBufs = new List<ByteCodeBuffer>();
            foreach (CatchChunk cc in tryStmnt.catchChunks)
            {
                ByteCodeBuffer catchBuf = serializeCodeBlock(cc.Code);

                // the value stack is cleared to its base level and the exception is added as the only item.
                catchBuf = ByteCode.join2(
                    cc.exceptionVarName != null
                        ? ByteCode.create0(OpCodes.OP_ASSIGN_VAR, cc.exceptionVarName, cc.exceptionVarName.Value)
                        : ByteCode.create0(OpCodes.OP_POP, null, null),
                    catchBuf);

                catchBufs.Add(catchBuf);
            }

            ByteCodeBuffer finallyBuf = ByteCode.join2(
                serializeCodeBlock(tryStmnt.finallyCode),
                ByteCode.create0(OpCodes.OP_TRY_FINALLY_END, null, null));

            // Add the jumps at the end of each catch to the finally
            int jumpOffset = 0;
            for (int i = catchBufs.Count - 2; i >= 0; i--)
            {
                jumpOffset += catchBufs[i + 1].length;
                catchBufs[i] = ByteCode.join2(
                    catchBufs[i],
                    ByteCode.create1(OpCodes.OP_JUMP, null, null, jumpOffset)); // jump to finally
            }

            // Add a router chunk for each class type
            // If none are found, it will flag the exception as bubbling and go to the finally.
            /*
                Try
                Route exceptions
                Catch 1:
                Catch 2:
                Catch n:
                Finally (always present as stub, the OP_TRY_FINALLY_END needs to run to re-bubble)
            */
            // ByteCodeBuffer exceptionRouterBuf = ByteCode.create0

            // OP_TRY_CATCH_ROUTER [finallyJumpOffset jumpOffset1 classId1 classId2 ...[0 jumpOffset2 classId3] ] <-- 0's delimit multiple catches
            // If a jump offset has no class associated with it, then that's the general Exception catcher
            jumpOffset = 0;
            List<int> catchRouterArgs = new List<int>();
            for (int i = 0; i < catchBufs.Count; i++)
            {
                if (i > 0) catchRouterArgs.Add(0);
                catchRouterArgs.Add(jumpOffset);
                CatchChunk cc = tryStmnt.catchChunks[i];
                foreach (AbstractEntity cdef in cc.ClassDefinitions)
                {
                    catchRouterArgs.Add(cdef.serializationIndex);
                }

                jumpOffset += catchBufs[i].length;
            }
            // TODO: theoretically, you can leave off the finally jump offset and read it from the previous instruction
            // which will probably be the OP_JUMP for the finally.
            catchRouterArgs.Insert(0, jumpOffset); // jump to finally

            ByteCodeBuffer catchRouterBuf = new ByteCodeBuffer(new ByteCodeRow(OpCodes.OP_TRY_CATCH_ROUTER, null, null, catchRouterArgs.ToArray()));

            ByteCodeBuffer routeAndCatches = catchRouterBuf;
            foreach (ByteCodeBuffer catchBuf in catchBufs)
            {
                routeAndCatches = ByteCode.join2(routeAndCatches, catchBuf);
            }
            tryBuf = ByteCode.join2(tryBuf, ByteCode.create1(OpCodes.OP_JUMP, null, null, routeAndCatches.length)); // jump to finally

            tryBuf.first.tryCatchInfo = new int[] {
                0, // offset to where the try starts.
                tryBuf.length,
                routeAndCatches.length,
                finallyBuf.length,
            };

            return ByteCode.join3(tryBuf, routeAndCatches, finallyBuf);
        }

        private static ByteCodeBuffer serializeWhileLoop(Statement whileLoop)
        {
            ByteCodeBuffer condBuf = serializeExpression(whileLoop.condition);
            condBuf = ByteCode.ensureBooleanExpression(whileLoop.condition.firstToken, condBuf);
            ByteCodeBuffer loopBody = serializeCodeBlock(whileLoop.code);
            return ByteCode.join4(
                condBuf,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, loopBody.length + 1),
                finalizeBreakContinue(loopBody, 1, true, -loopBody.length - 1 - condBuf.length), // could just leave the continue offset as 0 but this skips an unnecessary extra jump
                ByteCode.create1(OpCodes.OP_JUMP, null, null, -(loopBody.length + condBuf.length + 1 + 1))
            );
        }
    }
}
