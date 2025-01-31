﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScript.Compiler
{
    internal static class StatementSerializer
    {
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
                case StatementType.FOR_EACH_LOOP: return serializeForEachLoop(stmnt);
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

        private static ByteCodeBuffer serializeAssignVariable(Statement assignVar, string baseOp)
        {
            ByteCodeBuffer bufVal = ExpressionSerializer.serializeExpression(assignVar.assignValue);
            ByteCodeBuffer bufVar = ExpressionSerializer.serializeExpression(assignVar.assignTarget);
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
            ByteCodeBuffer bufVal = ExpressionSerializer.serializeExpression(assignIndex.assignValue);
            ByteCodeBuffer bufIndex = ExpressionSerializer.serializeExpression(assignIndex.assignTarget.right);
            ByteCodeBuffer bufRoot = ExpressionSerializer.serializeExpression(assignIndex.assignTarget.root);
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
            ByteCodeBuffer bufVal = ExpressionSerializer.serializeExpression(assignField.assignValue);
            ByteCodeBuffer bufRoot = ExpressionSerializer.serializeExpression(df.root);
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

        private static ByteCodeBuffer serializeBreak(Statement br)
        {
            return ByteCode.create0(OpCodes.OP_BREAK_DUMMY, br.firstToken, null);
        }

        private static ByteCodeBuffer serializeContinue(Statement cont)
        {
            return ByteCode.create0(OpCodes.OP_CONTINUE_DUMMY, cont.firstToken, null);
        }

        private static ByteCodeBuffer serializeDoWhileLoop(Statement doWhileLoop)
        {
            ByteCodeBuffer body = serializeCodeBlock(doWhileLoop.code);
            ByteCodeBuffer condition = ByteCode.ensureBooleanExpression(doWhileLoop.condition.firstToken, ExpressionSerializer.serializeExpression(doWhileLoop.condition));
            return ByteCode.join4(
                finalizeBreakContinue(body, condition.length + 2, true, 0),
                condition,
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_FALSE, null, null, 1),
                ByteCode.create1(OpCodes.OP_JUMP, null, null, -(1 + 1 + condition.length + body.length)));
        }

        private static ByteCodeBuffer serializeExpressionStatement(Statement exprStmnt)
        {
            return ByteCode.join2(
                ExpressionSerializer.serializeExpression(exprStmnt.expression),
                ByteCode.create0(OpCodes.OP_POP, null, null)
            );
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
            ByteCodeBuffer bufCondition = ExpressionSerializer.serializeExpression(condition);

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
        }

        private static ByteCodeBuffer serializeForEachLoop(Statement forEachLoop)
        {
            string loopExpr = "@fe" + forEachLoop.autoId;
            string iteratorVar = "@fi" + forEachLoop.autoId;

            ByteCodeBuffer setup = ByteCode.join5(
                ExpressionSerializer.serializeExpression(forEachLoop.expression),
                ByteCode.create0(OpCodes.OP_ENSURE_LIST, forEachLoop.expression.firstToken, null),
                ByteCode.create0(OpCodes.OP_ASSIGN_VAR, null, loopExpr),
                ByteCode.create1(OpCodes.OP_PUSH_INT, null, null, 0),
                ByteCode.create0(OpCodes.OP_ASSIGN_VAR, null, iteratorVar)
            );

            ByteCodeBuffer bufBody = serializeCodeBlock(forEachLoop.code);

            ByteCodeBuffer increment = ByteCode.join4(
                ByteCode.create0(OpCodes.OP_PUSH_VAR, null, iteratorVar),
                ByteCode.create1(OpCodes.OP_PUSH_INT, null, null, 1),
                ByteCode.create0(OpCodes.OP_BIN_OP, null, "+"),
                ByteCode.create0(OpCodes.OP_ASSIGN_VAR, null, iteratorVar)
            );

            ByteCodeBuffer doAssign = ByteCode.join4(
                ByteCode.create0(OpCodes.OP_PUSH_VAR, null, loopExpr),
                ByteCode.create0(OpCodes.OP_PUSH_VAR, null, iteratorVar),
                ByteCode.create0(OpCodes.OP_INDEX, null, null),
                ByteCode.create0(OpCodes.OP_ASSIGN_VAR, forEachLoop.varToken, forEachLoop.varToken.Value)
            );

            ByteCodeBuffer lengthCheck = ByteCode.join5(
                ByteCode.create0(OpCodes.OP_PUSH_VAR, null, iteratorVar),
                ByteCode.create0(OpCodes.OP_PUSH_VAR, null, loopExpr),
                ByteCode.create0(OpCodes.OP_DOT_FIELD, null, "length"),
                ByteCode.create0(OpCodes.OP_BIN_OP, null, ">="),
                ByteCode.create1(OpCodes.OP_POP_AND_JUMP_IF_TRUE, null, null, doAssign.length + bufBody.length + increment.length + 1)
            );

            bufBody = finalizeBreakContinue(bufBody, 5, true, 0);

            int reverseJumpDistance = -1 - increment.length - bufBody.length - doAssign.length - lengthCheck.length;

            ByteCodeBuffer fullCode = ByteCode.join6(
                setup,
                lengthCheck,
                doAssign,
                bufBody,
                increment,
                ByteCode.create1(OpCodes.OP_JUMP, null, null, reverseJumpDistance)
            );

            return fullCode;
        }

        private static ByteCodeBuffer serializeIfStatement(Statement ifStmnt)
        {
            Expression condition = ifStmnt.condition;
            Statement[] ifCode = ifStmnt.code;
            Statement[] elseCode = ifStmnt.elseCode; // if not present, it's an empty array, not null

            ByteCodeBuffer buf = ExpressionSerializer.serializeExpression(condition);
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

        private static ByteCodeBuffer serializeReturn(Statement ret)
        {
            ByteCodeBuffer buf;
            if (ret.expression == null)
            {
                buf = ByteCode.create0(OpCodes.OP_PUSH_NULL, null, null);
            }
            else
            {
                buf = ExpressionSerializer.serializeExpression(ret.expression);
            }
            return ByteCode.join2(buf, ByteCode.create0(OpCodes.OP_RETURN, ret.firstToken, null));
        }

        private static ByteCodeBuffer serializeSwitchStatement(Statement switchStmnt)
        {
            // switch (cond) { } <-- needs to ensure that the condition is an int or string.
            if (switchStmnt.switchChunks.Length == 0)
            {
                return ByteCode.join3(
                    ExpressionSerializer.serializeExpression(switchStmnt.condition),
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
                ExpressionSerializer.serializeExpression(switchStmnt.condition),
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

        // All breaks and continues default to jumping to the end of the buffer in
        // its current state. The additional break and continue offset are the
        // distance from the end.
        // For example, if this is a loop and the last instruction is a reverse JUMP,
        // then the break offset will be 0 and the continue offset will be -1.
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

        private static ByteCodeBuffer serializeThrowStatement(Statement thrw)
        {
            return ByteCode.join2(
                ExpressionSerializer.serializeExpression(thrw.expression),
                ByteCode.create0(OpCodes.OP_THROW, thrw.firstToken, null));
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
                foreach (ClassEntity cdef in cc.ClassDefinitions)
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
            ByteCodeBuffer condBuf = ExpressionSerializer.serializeExpression(whileLoop.condition);
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
