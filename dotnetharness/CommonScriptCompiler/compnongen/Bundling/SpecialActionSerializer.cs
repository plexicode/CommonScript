using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class SpecialActionSerializer
    {
        public static ByteCodeBuffer serializeSpecialAction(Expression action)
        {
            ByteCodeBuffer argBuffer = null;
            int argc = action.args.Length;
            for (int i = 0; i < argc; i++)
            {
                argBuffer = ByteCode.join2(argBuffer, ExpressionSerializer.serializeExpression(action.args[i]));
            }

            ByteCodeBuffer actionBuf;

            switch (action.strVal)
            {
                case "math_floor":
                    // Uses a specific op code for speed, rather than a special action nested op code.
                    actionBuf = ByteCode.create0(OpCodes.OP_MATH_FLOOR, action.firstToken, null);
                    break;

                case "unix_time":
                    // return the arg as a hard-coded op, discard the arg buffer.
                    return ByteCode.create2(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.UNIX_TIME, action.args[0].intVal);

                default:
                    actionBuf = ByteCode.create1(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionUtil.GetSpecialActionOpCode(action.strVal));
                    break;

            }

            return ByteCode.join2(argBuffer, actionBuf);
        }
    }
}
