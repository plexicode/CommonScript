using CommonScript.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommonScript.Runtime
{
    internal static class JsonUtil
    {
        public static Value Parse(ExecutionContext ec, string rawStr, int[] errOut)
        {
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(rawStr);
                errOut[0] = 0;
            }
            catch (JsonException jsonex)
            {
                errOut[0] = 1;
                errOut[1] = (int)(jsonex.LineNumber ?? 0);
                errOut[2] = (int)(jsonex.BytePositionInLine ?? 0);
                return null;
            }

            Dictionary<string, Value> strings = new Dictionary<string, Value>();
            return ConvertValue(ec, doc.RootElement, strings);
        }

        private static Value ConvertString(ExecutionContext ec, string strVal, Dictionary<string, Value> stringCache)
        {
            Value output;
            if (!stringCache.TryGetValue(strVal, out output))
            {
                output = FunctionWrapper.buildString(ec.globalValues, strVal, false);
                stringCache[strVal] = output;
            }
            return output;
        }

        private static Value ConvertValue(ExecutionContext ec, JsonElement el, Dictionary<string, Value> stringCache)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.True: return ec.globalValues.trueValue;
                case JsonValueKind.False: return ec.globalValues.falseValue;
                case JsonValueKind.Null: return ec.globalValues.nullValue;
                case JsonValueKind.String: return ConvertString(ec, el.GetString(), stringCache);

                case JsonValueKind.Object:
                    List<Value> objKeys = [];
                    List<Value> objValues = [];
                    foreach (JsonProperty prop in el.EnumerateObject())
                    {
                        objKeys.Add(ConvertString(ec, prop.Name, stringCache));
                        objValues.Add(ConvertValue(ec, prop.Value, stringCache));
                    }
                    return FunctionWrapper.buildStringDictionary(ec, [.. objKeys], [.. objValues]);

                case JsonValueKind.Array:
                    throw new NotImplementedException();

                case JsonValueKind.Number:
                    if (el.TryGetInt32(out int intVal))
                    {
                        return FunctionWrapper.buildInteger(ec.globalValues, intVal);
                    }
                    if (el.TryGetDouble(out double floatVal))
                    {
                        return FunctionWrapper.buildFloat(floatVal);
                    }
                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
