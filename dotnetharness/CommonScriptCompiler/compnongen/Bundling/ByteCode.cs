using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ByteCode
    {
        public static ByteCodeBuffer ensureIntegerExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null) throw new InvalidOperationException();

            ByteCodeRow last = buf.last;
            if (last.opCode == OpCodes.OP_PUSH_INT) return buf;
            if (last.opCode == OpCodes.OP_BITWISE_NOT) return buf;
            return FunctionWrapper.join2(buf, FunctionWrapper.create0(OpCodes.OP_ENSURE_INT, throwToken, null));
        }

        public static ByteCodeBuffer ensureBooleanExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null) throw new InvalidOperationException();

            ByteCodeRow last = buf.last;
            if (last.opCode == OpCodes.OP_BIN_OP)
            {
                switch (last.stringArg)
                {
                    case "||":
                    case "&&":
                    case "==":
                    case "!=":
                    case "<":
                    case ">":
                    case "<=":
                    case ">=":
                        return buf;
                }
            }

            if (last.opCode == OpCodes.OP_ENSURE_BOOL) return buf;
            if (last.opCode == OpCodes.OP_PUSH_BOOL) return buf;
            if (last.opCode == OpCodes.OP_BOOLEAN_NOT) return buf;

            return FunctionWrapper.join2(buf, FunctionWrapper.create0(OpCodes.OP_ENSURE_BOOL, throwToken, null));
        }
    }
}
