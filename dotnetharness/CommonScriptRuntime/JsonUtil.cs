using CommonScript.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CommonScript.Runtime
{
    internal static class JsonUtil
    {
        public static string Serialize(Value obj, bool isPretty)
        {
            StringBuilder sb = new StringBuilder();
            SerializeImpl(obj, isPretty, 0, sb, []);
            return sb.ToString();
        }

        private static string SerializeString(string rawValue, Dictionary<string, string> valueCache)
        {
            string output;
            if (!valueCache.TryGetValue(rawValue, out output))
            {
                output = JsonSerializer.Serialize(rawValue, typeof(string));
                valueCache[rawValue] = output;
            }
            return output;
        }

        private static void SerializeImpl(Value obj, bool isPretty, int indentLevel, StringBuilder sb, Dictionary<string, string> stringCache)
        {
            switch ((InternalType)obj.type)
            {
                case InternalType.INTEGER:
                    sb.Append(((int)obj.internalValue).ToString());
                    break;

                case InternalType.BOOLEAN:
                    sb.Append((bool)obj.internalValue ? "true" : "false");
                    break;

                case InternalType.NULL:
                    sb.Append("null");
                    break;

                case InternalType.FLOAT:
                    sb.Append(FunctionWrapper.valueToHumanString(obj));
                    break;

                case InternalType.STRING:
                    sb.Append(SerializeString(FunctionWrapper.stringUtil_getFlatValue(obj), stringCache));
                    break;

                case InternalType.LIST:
                    sb.Append(isPretty ? "[\n" : "[");
                    ListImpl list = (ListImpl)obj.internalValue;
                    for (int i = 0; i < list.length; i++)
                    {
                        if (isPretty) Indent(sb, indentLevel + 1);
                        SerializeImpl(list.items[i], isPretty, indentLevel + 1, sb, stringCache);
                        if (i + 1 < list.length) sb.Append(',');
                        if (isPretty) sb.Append('\n');
                    }
                    if (isPretty) Indent(sb, indentLevel);
                    sb.Append(']');
                    break;

                case InternalType.DICTIONARY:
                    sb.Append(isPretty ? "{\n" : "{");
                    DictImpl dict = (DictImpl)obj.internalValue;
                    for (int i = 0; i < dict.size; i++)
                    {
                        if (isPretty) Indent(sb, indentLevel + 1);
                        SerializeImpl(dict.keys[i], isPretty, 0, sb, stringCache);
                        sb.Append(isPretty ? ": " : ":");
                        SerializeImpl(dict.values[i], isPretty, indentLevel + 1, sb, stringCache);
                        if (i + 1 < dict.size) sb.Append(',');
                        if (isPretty) sb.Append('\n');
                    }
                    if (isPretty) Indent(sb, indentLevel);
                    sb.Append('}');
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static void Indent(StringBuilder sb, int indentLevel)
        {
            while (indentLevel-- > 0)
            {
                sb.Append('\t');
            }
        }

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
                    List<Value> items = [];
                    foreach (JsonElement item in el.EnumerateArray())
                    {
                        items.Add(ConvertValue(ec, item, stringCache));
                    }
                    return FunctionWrapper.buildList(ec, [.. items], false, items.Count);

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
