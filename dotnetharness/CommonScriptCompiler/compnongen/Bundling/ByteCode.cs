using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ByteCode
    {
        public static ByteCodeBuffer join2(ByteCodeBuffer a, ByteCodeBuffer b)
        {
            if (a == null) return b;
            if (b == null) return a;

            return FunctionWrapper.ByteCodeBuffer_from2(a, b);
        }

        public static ByteCodeBuffer join3(
            ByteCodeBuffer a,
            ByteCodeBuffer b,
            ByteCodeBuffer c)
        {
            return join2(a, join2(b, c));
        }

        public static ByteCodeBuffer join4(
            ByteCodeBuffer a,
            ByteCodeBuffer b,
            ByteCodeBuffer c,
            ByteCodeBuffer d)
        {
            return join2(join2(a, b), join2(c, d));
        }

        public static ByteCodeBuffer join5(
            ByteCodeBuffer a,
            ByteCodeBuffer b,
            ByteCodeBuffer c,
            ByteCodeBuffer d,
            ByteCodeBuffer e)
        {
            return join2(join2(a, b), join3(c, d, e));
        }

        public static ByteCodeBuffer join6(
            ByteCodeBuffer a,
            ByteCodeBuffer b,
            ByteCodeBuffer c,
            ByteCodeBuffer d,
            ByteCodeBuffer e,
            ByteCodeBuffer f)
        {
            return join3(join2(a, b), join2(c, d), join2(e, f));
        }

        public static ByteCodeBuffer join7(
            ByteCodeBuffer a,
            ByteCodeBuffer b,
            ByteCodeBuffer c,
            ByteCodeBuffer d,
            ByteCodeBuffer e,
            ByteCodeBuffer f,
            ByteCodeBuffer g)
        {
            return join4(join2(a, b), join2(c, d), join2(e, f), g);
        }

        public static ByteCodeBuffer create0(int opCode, Token token, string stringArg)
        {
            return FunctionWrapper.ByteCodeBuffer_fromRow(FunctionWrapper.ByteCodeRow_new(opCode, token, stringArg, new int[0]));
        }

        public static ByteCodeBuffer create1(int opCode, Token token, string stringArg, int arg1)
        {
            int[] args = new int[] { arg1 };
            return FunctionWrapper.ByteCodeBuffer_fromRow(FunctionWrapper.ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create2(int opCode, Token token, string stringArg, int arg1, int arg2)
        {
            int[] args = new int[] { arg1, arg2 };
            return FunctionWrapper.ByteCodeBuffer_fromRow(FunctionWrapper.ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create3(int opCode, Token token, string stringArg, int arg1, int arg2, int arg3)
        {
            int[] args = new int[] { arg1, arg2, arg3 };
            return FunctionWrapper.ByteCodeBuffer_fromRow(FunctionWrapper.ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer ensureIntegerExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null) throw new InvalidOperationException();

            ByteCodeRow last = buf.last;
            if (last.opCode == OpCodes.OP_PUSH_INT) return buf;
            if (last.opCode == OpCodes.OP_BITWISE_NOT) return buf;
            return join2(buf, create0(OpCodes.OP_ENSURE_INT, throwToken, null));
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

            return join2(buf, create0(OpCodes.OP_ENSURE_BOOL, throwToken, null));
        }

        public static ByteCodeRow[] flatten(ByteCodeBuffer buffer)
        {
            if (buffer == null) return new ByteCodeRow[0];

            List<ByteCodeBuffer> q = new List<ByteCodeBuffer>();
            q.Add(buffer);

            List<ByteCodeRow> output = new List<ByteCodeRow>();

            while (q.Count > 0)
            {
                ByteCodeBuffer current = q[q.Count - 1];
                q.RemoveAt(q.Count - 1);
                if (current.isLeaf)
                {
                    output.Add(current.row);
                }
                else
                {
                    q.Add(current.right);
                    q.Add(current.left);
                }
            }
            return output.ToArray();
        }

        public static ByteCodeBuffer convertToBuffer(ByteCodeRow[] flatRows)
        {
            ByteCodeBuffer buf = null;
            foreach (ByteCodeRow row in flatRows)
            {
                buf = join2(buf, FunctionWrapper.ByteCodeBuffer_fromRow(row));
            }
            return buf;
        }
    }
}
