using CommonScript.Runtime.Internal;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CommonScript.Runtime
{
    public class ValueConverter
    {
        private static ExecutionContext CastExecCtx(object obj)
        {
            if (obj == null) throw new ArgumentException("Cannot pass null as execution context");
            return obj as ExecutionContext
                ?? throw new ArgumentException("Did not pass an execution context object");
        }

        public static string RTValueToReadableString(object rtVal)
        {
            return FunctionWrapper.PUBLIC_valueToString(rtVal);
        }

        public static double RTValueToFloat(object rtVal)
        {
            return (double)((Value)rtVal).internalValue;
        }

        public static object NativeValueToRTValue(object execCtxObj, object value)
        {
            ExecutionContext ec = CastExecCtx(execCtxObj);
            GlobalValues g = ec.globalValues;
            if (value is null) return g.nullValue;
            if (value is Value) return (Value)value;
            if (value is string)
            {
                string strVal = (string)value;
                if (strVal.Length < 2)
                {
                    if (strVal.Length == 0) return g.emptyString;
                    return FunctionWrapper.buildString(g, strVal, true);
                }
                return FunctionWrapper.buildString(g, strVal, false);
            }
            if (value is bool) return (bool)value ? g.trueValue : g.falseValue;
            if (value is int) return FunctionWrapper.buildInteger(g, (int)value);
            if (value is double) return FunctionWrapper.buildFloat((double)value);
            if (value is float) return FunctionWrapper.buildFloat((double)(float)value);

            throw new ArgumentException("unable to convert value: " + value.GetType().ToString());
        }

        public static object ValuesToRTList(object execCtxObj, IEnumerable<object> values)
        {
            ExecutionContext ec = CastExecCtx(execCtxObj);
            return FunctionWrapper.buildList(
                ec,
                values
                    .Select(val => (Value)NativeValueToRTValue(ec, val))
                    .ToArray(),
                false,
                -1);
        }
    }
}
