using System;
using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal static class ByteCode
    {
        public static ByteCodeBuffer join2(ByteCodeBuffer a, ByteCodeBuffer b)
        {
            if (a == null) return b;
            if (b == null) return a;

            return new ByteCodeBuffer(a, b);
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
            return new ByteCodeBuffer(new ByteCodeRow(opCode, token, stringArg, new int[0]));
        }

        public static ByteCodeBuffer create1(int opCode, Token token, string stringArg, int arg1)
        {
            int[] args = new int[] { arg1 };
            return new ByteCodeBuffer(new ByteCodeRow(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create2(int opCode, Token token, string stringArg, int arg1, int arg2)
        {
            int[] args = new int[] { arg1, arg2 };
            return new ByteCodeBuffer(new ByteCodeRow(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create3(int opCode, Token token, string stringArg, int arg1, int arg2, int arg3)
        {
            int[] args = new int[] { arg1, arg2, arg3 };
            return new ByteCodeBuffer(new ByteCodeRow(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer ensureIntegerExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null) throw new InvalidOperationException();

            ByteCodeRow last = buf.last;
            if (last.opCode == OpCodes.OP_PUSH_INT) return buf;
            if (last.opCode == OpCodes.OP_BITWISE_NOT) return buf;
            return ByteCode.join2(buf, ByteCode.create0(OpCodes.OP_ENSURE_INT, throwToken, null));
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

            return ByteCode.join2(buf, ByteCode.create0(OpCodes.OP_ENSURE_BOOL, throwToken, null));
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
                buf = ByteCode.join2(buf, new ByteCodeBuffer(row));
            }
            return buf;
        }
    }

    internal class ByteCodeBuffer
    {
        public int length;
        public bool isLeaf;
        public ByteCodeBuffer left = null;
        public ByteCodeBuffer right = null;
        public ByteCodeRow row = null;
        public ByteCodeRow last;
        public ByteCodeRow first;

        // TODO: add an optimization here that tracks whether or not the buffer contains a break/continue

        public ByteCodeBuffer(ByteCodeBuffer left, ByteCodeBuffer right)
        {
            this.length = left.length + right.length;
            this.isLeaf = false;
            this.left = left;
            this.right = right;
            this.first = left.first;
            this.last = right.last;
        }

        public ByteCodeBuffer(ByteCodeRow row)
        {
            this.length = 1;
            this.isLeaf = true;
            this.row = row;
            this.last = row;
            this.first = row;
        }
    }

    internal class ByteCodeRow
    {
        public int opCode;
        public string stringArg;
        public Token token;
        public int[] args;
        public int stringId;
        public int tokenId;
        public int[] tryCatchInfo;

        public ByteCodeRow(int opCode, Token token, string stringArg, int[] args)
        {
            this.opCode = opCode;
            this.token = token;
            this.stringArg = stringArg;
            this.args = args;
            this.stringId = 0;
            this.tokenId = 0;
            this.tryCatchInfo = null;
        }
    }
}
