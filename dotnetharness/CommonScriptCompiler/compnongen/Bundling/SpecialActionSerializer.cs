using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class SpecialActionSerializer
    {
        private static StaticContext staticCtx = FunctionWrapper.StaticContext_new();
        public static ByteCodeBuffer serializeSpecialAction(Expression action)
        {
            ByteCodeBuffer argBuffer = null;
            int argc = action.args.Length;
            for (int i = 0; i < argc; i++)
            {
                argBuffer = FunctionWrapper.join2(argBuffer, ExpressionSerializer.serializeExpression(action.args[i]));
            }

            ByteCodeBuffer actionBuf;

            switch (action.strVal)
            {
                case "math_floor":
                    // Uses a specific op code for speed, rather than a special action nested op code.
                    actionBuf = FunctionWrapper.create0(OpCodes.OP_MATH_FLOOR, action.firstToken, null);
                    break;

                case "unix_time":
                    // return the arg as a hard-coded op, discard the arg buffer.
                    return FunctionWrapper.create2(OpCodes.OP_SPECIAL_ACTION, null, null, SpecialActionCodes.UNIX_TIME, action.args[0].intVal);

                default:
                    int specialActionOpCode = FunctionWrapper.SpecialActionUtil_GetSpecialActionOpCode(staticCtx.specialActionUtil, action.strVal);
                    actionBuf = FunctionWrapper.create1(OpCodes.OP_SPECIAL_ACTION, null, null, specialActionOpCode);
                    break;

            }

            return FunctionWrapper.join2(argBuffer, actionBuf);
        }
    }
}
