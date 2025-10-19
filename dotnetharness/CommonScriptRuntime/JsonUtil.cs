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

        private static readonly char[] HEX_CHARS = "0123456789ABCDEF".ToCharArray();

        private static string SerializeString(string rawValue, Dictionary<string, string> valueCache)
        {
            string output;
            if (!valueCache.TryGetValue(rawValue, out output))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('"');
                char[] chars = rawValue.ToCharArray();
                char c;
                int length = chars.Length;
                for (int i = 0; i < length; i++)
                {
                    c = chars[i];
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        default:
                            if (c < 32)
                            {
                                sb.Append("\\u");
                                int h4 = c;
                                sb.Append(HEX_CHARS[(h4 >> 12) & 15]);
                                sb.Append(HEX_CHARS[(h4 >> 8) & 15]);
                                sb.Append(HEX_CHARS[(h4 >> 4) & 15]);
                                sb.Append(HEX_CHARS[h4 & 15]);
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }
                sb.Append('"');
                output = sb.ToString();

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

        public static Value Parse(string rawStr, List<object> bufOut)
        {
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(rawStr);
            }
            catch (JsonException jsonex)
            {
                bufOut.Add(false);
                bufOut.Add((int)(jsonex.LineNumber ?? 0));
                bufOut.Add((int)(jsonex.BytePositionInLine ?? 0));
                return null;
            }

            bufOut.Add(true);
            ConvertValue(doc.RootElement, bufOut);
            return null;
        }

        private static void ConvertValue(JsonElement el, List<object> bufOut)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Null:
                    bufOut.Add(0);
                    return;

                case JsonValueKind.True:
                    bufOut.Add(1);
                    bufOut.Add(true);
                    return;

                case JsonValueKind.False:
                    bufOut.Add(1);
                    bufOut.Add(false);
                    return;

                case JsonValueKind.Number:
                    if (el.TryGetInt32(out int intVal))
                    {
                        bufOut.Add(2);
                        bufOut.Add(intVal);
                        return;
                    }

                    if (el.TryGetDecimal(out decimal decVal))
                    {
                        bufOut.Add(3);
                        double doubleVal = (double)decVal;
                        bufOut.Add(doubleVal);
                        return;
                    }
                    
                    if (el.TryGetDouble(out double floatVal))
                    {
                        bufOut.Add(3);
                        bufOut.Add(floatVal);
                    }

                    throw new NotImplementedException();

                case JsonValueKind.String:
                    bufOut.Add(4);
                    bufOut.Add(el.GetString());
                    return;

                case JsonValueKind.Object:
                    bufOut.Add(6);
                    int objSizeIndex = bufOut.Count;
                    bufOut.Add(-1);
                    int objSize = 0;
                    foreach (JsonProperty prop in el.EnumerateObject())
                    {
                        objSize++;
                        bufOut.Add(prop.Name);
                        ConvertValue(prop.Value, bufOut);
                    }

                    bufOut[objSizeIndex] = objSize;
                    return;

                case JsonValueKind.Array:
                    bufOut.Add(5);
                    int arrayLengthIndex = bufOut.Count;
                    bufOut.Add(-1);
                    int itemCount = 0;
                    foreach (JsonElement item in el.EnumerateArray())
                    {
                        itemCount++;
                        ConvertValue(item, bufOut);
                    }

                    bufOut[arrayLengthIndex] = itemCount;
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
