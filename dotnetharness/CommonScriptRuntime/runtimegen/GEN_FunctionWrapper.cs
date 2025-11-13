using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Runtime.Internal
{
    internal static class FunctionWrapper
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private static int[] PST_stringToUtf8Bytes(string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            int len = bytes.Length;
            int[] output = new int[len];
            for (int i = 0; i < len; i++)
            {
                output[i] = ((int)bytes[i]) & 255;
            }
            return output;
        }

        private static Dictionary<string, System.Func<object[], object>> PST_ExtCallbacks = new Dictionary<string, System.Func<object[], object>>();

        private static T PST_ListPop<T>(List<T> list)
        {
            if (list.Count == 0) throw new System.InvalidOperationException();
            int lastIndex = list.Count - 1;
            T val = list[lastIndex];
            list.RemoveAt(lastIndex);
            return val;
        }

        private static string PST_FloatToString(double value)
        {
            string output = value.ToString();
            if (output[0] == '.') output = "0" + output;
            if (!output.Contains('.')) output += ".0";
            return output;
        }

        private static bool[] PST_ToCodeStringSafeChars = null;
        private static string[] PST_ToCodeStringSwapChars = null;
        private static string PST_ToCodeString(string str)
        {
            int i;
            if (PST_ToCodeStringSafeChars == null)
            {
                bool[] safe = new bool[128];
                PST_ToCodeStringSafeChars = safe;
                foreach (char sc in ".,/:;'[]{}|()!@#$%^&*-_=+`~<>? ".ToCharArray())
                {
                    safe[(int)sc] = true;
                }
                for (i = 0; i < 10; ++i) safe['0' + i] = true;
                for (i = 0; i < 26; ++i)
                {
                    safe['A' + i] = true;
                    safe['a' + i] = true;
                }
                string[] swaps = new string[128];
                swaps[0] = "\\0";
                swaps['\n'] = "\\n";
                swaps['\t'] = "\\t";
                swaps['\r'] = "\\r";
                swaps['\\'] = "\\\\";
                swaps['"'] = "\\\"";
                PST_ToCodeStringSwapChars = swaps;
            }

            char[] chars = str.ToCharArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append('"');
            int len = chars.Length;
            char c;
            int b;
            string swap;
            for (i = 0; i < len; ++i)
            {
                c = chars[i];
                b = c;
                if (b >= 0 && c < 128)
                {
                    if (PST_ToCodeStringSafeChars[b])
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        swap = PST_ToCodeStringSwapChars[b];
                        if (swap != null)
                        {
                            sb.Append(swap);
                        }
                        else
                        {
                            sb.Append("\\u");
                            sb.Append(b.ToString("X4"));
                        }
                    }
                }
                else
                {
                    sb.Append("\\u");
                    sb.Append(b.ToString("X4"));
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static readonly string[] PST_SplitSep = new string[1];

        private static string[] PST_StringSplit(string value, string sep)
        {
            if (sep.Length == 1) return value.Split(sep[0]);
            if (sep.Length == 0) return value.ToCharArray().Select<char, string>(c => "" + c).ToArray();
            PST_SplitSep[0] = sep;
            return value.Split(PST_SplitSep, System.StringSplitOptions.None);
        }

        public static void PST_ParseFloat(string strValue, double[] output)
        {
            double num = 0.0;
            output[0] = double.TryParse(strValue, out num) ? 1 : -1;
            output[1] = num;
        }

        private static readonly System.Random PST_Random = new System.Random();

        private static readonly System.DateTime PST_UnixEpoch = new System.DateTime(1970, 1, 1);

        private static double PST_CurrentTime
        {
            get { return System.DateTime.UtcNow.Subtract(PST_UnixEpoch).TotalSeconds; }
        }

        public static void PST_RegisterExtensibleCallback(string name, System.Func<object[], object> func)
        {
            PST_ExtCallbacks[name] = func;
        }

        public static bool bubbleException(ExecutionTask task, Value exceptionValue)
        {
            StackFrame frame = task.stack;
            ExecutionContext ec = task.execCtx;
            bool keepGoing = true;
            while (keepGoing)
            {
                if (frame == null)
                {
                    task.stack = null;
                    return true;
                }
                int pc = frame.pc;
                TryDescriptor td = ec.tryDescriptors[pc];
                if (td != null)
                {
                    keepGoing = false;
                    if (pc < td.routerPc)
                    {
                        frame.pc = td.routerPc - 1;
                        frame.bubblingValue = exceptionValue;
                    }
                    else if (pc >= td.finallyPc)
                    {
                        frame.pc = td.finallyEnd - 1;
                        frame.bubblingValue = exceptionValue;
                        frame.bubblingValueUncaught = true;
                    }
                    else
                    {
                        frame.pc = td.finallyPc;
                        frame.bubblingValue = exceptionValue;
                        frame.bubblingValueUncaught = true;
                    }
                }
                else
                {
                    frame = frame.previous;
                }
            }
            task.stack = frame;
            frame.valueStackSize = frame.valueStackBaseSize;
            return false;
        }

        public static StringImpl buildAsciiStringImpl(string rawValue)
        {
            return new StringImpl(rawValue.Length, false, PST_stringToUtf8Bytes(rawValue), rawValue, null, null);
        }

        public static string buildBase64String(int[] rawBytes)
        {
            System.Collections.Generic.List<int> pairs = new List<int>();
            int sz = rawBytes.Length;
            int n = 0;
            int i = 0;
            while (i < sz)
            {
                n = rawBytes[i];
                pairs.Add(n >> 6 & 3);
                pairs.Add(n >> 4 & 3);
                pairs.Add(n >> 2 & 3);
                pairs.Add(n & 3);
                i += 1;
            }
            string[] chars = new string[64];
            int j = 0;
            while (j < 26)
            {
                chars[j] = ((char)(65 + j)).ToString();
                chars[j + 26] = ((char)(97 + j)).ToString();
                if (j < 10)
                {
                    chars[j + 52] = ((char)(48 + j)).ToString();
                }
                j += 1;
            }
            chars[62] = "+";
            chars[63] = "/";
            int pairSize = pairs.Count;
            pairs.Add(0);
            pairs.Add(0);
            System.Collections.Generic.List<string> sb = new List<string>();
            int k = 0;
            while (k < pairSize)
            {
                n = pairs[k] << 4 | pairs[k + 1] << 2 | pairs[k + 2];
                sb.Add(chars[n]);
                k += 3;
            }
            while (sb.Count % 4 != 0)
            {
                sb.Add("=");
            }
            return string.Join("", sb);
        }

        public static Value buildFloat(double v)
        {
            return new Value(4, v);
        }

        public static Value buildFunctionFromInfo(FunctionInfo fn)
        {
            FunctionPointer fp = new FunctionPointer(1, fn.argcMin, fn.argcMax, fn.pc, fn, null);
            if (fn.name == null)
            {
                fp.funcType = 7;
            }
            return new Value(11, fp);
        }

        public static Value buildInteger(GlobalValues g, int value)
        {
            if (value < 0)
            {
                if (value > -1200)
                {
                    return g.negIntegers[-value];
                }
            }
            else if (value < 1200)
            {
                return g.posIntegers[value];
            }
            return new Value(3, value);
        }

        public static Value buildIntegerListValue(ExecutionContext ec, int[] ints)
        {
            int sz = ints.Length;
            GlobalValues g = ec.globalValues;
            Value[] vals = new Value[sz];
            int n = 0;
            Value val = null;
            int i = 0;
            while (i < sz)
            {
                n = ints[i];
                if (n < 1200 && n > -1200)
                {
                    if (n < 0)
                    {
                        val = g.negIntegers[-n];
                    }
                    else
                    {
                        val = g.posIntegers[n];
                    }
                }
                else
                {
                    val = new Value(3, n);
                }
                vals[i] = val;
                i += 1;
            }
            return buildList(ec, vals, false, sz);
        }

        public static Value buildList(ExecutionContext ec, Value[] values, bool copyValues, int lengthOrNegativeOne)
        {
            int id = ec.nextRefId;
            ec.nextRefId += 1;
            int size = lengthOrNegativeOne;
            if (size == -1)
            {
                size = values.Length;
            }
            if (copyValues)
            {
                Value[] buf = new Value[size];
                int i = 0;
                while (i < size)
                {
                    buf[i] = values[i];
                    i += 1;
                }
                values = buf;
            }
            return new Value(9, new ListImpl(id, size, size, values));
        }

        public static Value buildListFromStringArray(ExecutionContext ec, string[] arr)
        {
            Value[] valArr = new Value[arr.Length];
            int i = 0;
            while (i < arr.Length)
            {
                valArr[i] = buildString(ec.globalValues, arr[i], false);
                i += 1;
            }
            return buildList(ec, valArr, false, -1);
        }

        public static Value buildString(GlobalValues g, string rawValue, bool isCommon)
        {
            if (g.commonStrings.ContainsKey(rawValue))
            {
                return g.commonStrings[rawValue];
            }
            int[] charValues = PST_stringToUtf8Bytes(rawValue);
            StringImpl s = new StringImpl(charValues.Length, false, charValues, rawValue, null, null);
            Value v = new Value(5, s);
            if (isCommon)
            {
                g.commonStrings[rawValue] = v;
            }
            return v;
        }

        public static Value buildStringDictionary(ExecutionContext ec, Value[] keys, Value[] values)
        {
            DictImpl dict = new DictImpl(ec.nextRefId, 5, keys.Length, keys.Length, keys, values, null, new Dictionary<string, int>());
            ec.nextRefId += 1;
            System.Collections.Generic.Dictionary<string, int> lookup = dict.strKeyLookup;
            int i = 0;
            while (i < keys.Length)
            {
                lookup[stringUtil_getFlatValue(keys[i])] = i;
                i += 1;
            }
            return new Value(10, dict);
        }

        public static int[] convertListToByteArray(ListImpl list)
        {
            int sz = list.length;
            int[] bytesOut = new int[sz];
            int val = 0;
            Value value = null;
            int i = 0;
            while (i < sz)
            {
                value = list.items[i];
                if (value.type != 3)
                {
                    return null;
                }
                val = (int)value.internalValue;
                if (val < 0 || val >= 256)
                {
                    return null;
                }
                bytesOut[i] = val;
                i += 1;
            }
            return bytesOut;
        }

        public static StringImpl convertToStringImpl(GlobalValues g, Value v)
        {
            switch (v.type)
            {
                case 1:
                    return (StringImpl)buildString(g, "null", true).internalValue;
                case 2:
                    if ((bool)v.internalValue)
                    {
                        return (StringImpl)buildString(g, "true", true).internalValue;
                    }
                    return (StringImpl)buildString(g, "false", true).internalValue;
                case 3:
                    int n = (int)v.internalValue;
                    if (n < 20 && n > -20)
                    {
                        return (StringImpl)buildString(g, n.ToString(), true).internalValue;
                    }
                    return buildAsciiStringImpl(n.ToString());
                case 4:
                    return (StringImpl)buildString(g, valueToHumanString(v), false).internalValue;
                case 5:
                    return (StringImpl)v.internalValue;
                default:
                    break;
            }
            return buildAsciiStringImpl("TODO: string for this type");
        }

        public static ClassInfo createClassInfo(int id, int parentId, string name, FunctionInfo ctor, System.Collections.Generic.List<string> newFields, System.Collections.Generic.List<int> newMemberInfoFlags, int cctorIdOrZero)
        {
            Value ctorVal = new Value(11, new FunctionPointer(5, ctor.argcMin, ctor.argcMax, ctor.pc, ctor, null));
            int fieldCount = newFields.Count;
            ClassInfo info = new ClassInfo(id, parentId, null, name, ctorVal, cctorIdOrZero, false, new Dictionary<string, FunctionInfo>(), new Dictionary<string, int>(), new string[fieldCount], new Value[fieldCount], newFields, newMemberInfoFlags, null, new Dictionary<string, Value>(), new Dictionary<string, bool>());
            info.classRef = new Value(13, info);
            return info;
        }

        public static ExecutionTask createMainTask(ExecutionContext ec, string[] cliArgs)
        {
            if (ec.nextTaskId != 1)
            {
                return null;
            }
            finalizeExecutionContext(ec);
            Value mainFn = ec.functionsAsValues[ec.significantFunctions["main"]];
            Value args = ec.globalValues.nullValue;
            Value[] argList = new Value[1];
            Value[] argValues = new Value[cliArgs.Length];
            int i = 0;
            while (i < cliArgs.Length)
            {
                argValues[i] = buildString(ec.globalValues, cliArgs[i], true);
                i += 1;
            }
            argList[0] = buildList(ec, argValues, false, -1);
            return createNewTask(ec, mainFn, argList);
        }

        public static ExecutionTask createNewTask(ExecutionContext ec, Value fpValue, Value[] args)
        {
            if (fpValue.type != 11)
            {
                return null;
            }
            FunctionPointer fp = (FunctionPointer)fpValue.internalValue;
            int pc = 0;
            int argc = args.Length;
            int argcMax = argc;
            Value context = null;
            switch (fp.funcType)
            {
                case 1:
                    pc = fp.pcOrId;
                    if (argc < fp.argcMin || argc > fp.argcMax)
                    {
                        return null;
                    }
                    argcMax = fp.argcMax;
                    break;
                case 2:
                    pc = fp.pcOrId;
                    if (argc < fp.argcMin || argc > fp.argcMax)
                    {
                        return null;
                    }
                    argcMax = fp.argcMax;
                    break;
                case 3:
                    pc = fp.pcOrId;
                    if (argc < fp.argcMin || argc > fp.argcMax)
                    {
                        return null;
                    }
                    argcMax = fp.argcMax;
                    break;
                default:
                    return null;
            }
            if (fp.funcType == 2)
            {
                context = ((FunctionPointer)((Value)fpValue).internalValue).ctx;
            }
            Value[] argsClone = new Value[argc];
            int i = 0;
            while (i < argc)
            {
                argsClone[i] = args[i];
                i += 1;
            }
            StackFrame frame = new StackFrame(null, pc, argsClone, argc, 0, 0, new Dictionary<string, Value>(), context, false, null, false, fp.func);
            ExecutionTask task = new ExecutionTask(ec.nextTaskId, ec, frame, false, 0, new Value[1], null, null);
            ec.nextTaskId += 1;
            ec.tasks[task.taskId] = task;
            return task;
        }

        public static Value createStringFromUnicodeArray(GlobalValues g, int[] buffer, bool copyBuffer)
        {
            int[] uchars = buffer;
            int sz = buffer.Length;
            if (sz < 2)
            {
                if (sz == 0)
                {
                    return g.emptyString;
                }
                string singleChar = ((char)buffer[0]).ToString();
                return buildString(g, singleChar, true);
            }
            if (copyBuffer)
            {
                uchars = new int[sz];
                int i = 0;
                while (i < sz)
                {
                    uchars[i] = buffer[i];
                    i += 1;
                }
            }
            System.Collections.Generic.List<string> sb = new List<string>();
            int j = 0;
            while (j < sz)
            {
                sb.Add(((char)uchars[j]).ToString());
                j += 1;
            }
            string finalString = string.Join("", sb);
            if (g.commonStrings.ContainsKey(finalString))
            {
                return g.commonStrings[finalString];
            }
            return new Value(5, new StringImpl(sz, false, uchars, finalString, null, null));
        }

        public static Value createStringFromUnicodeArraySegment(GlobalValues g, int[] buffer, int start, int length)
        {
            if (length == buffer.Length)
            {
                return createStringFromUnicodeArray(g, buffer, true);
            }
            int[] newBuf = new int[length];
            int i = 0;
            while (i < length)
            {
                newBuf[i] = buffer[start + i];
                i += 1;
            }
            return createStringFromUnicodeArray(g, newBuf, false);
        }

        public static int decToInt(int charCode)
        {
            if (charCode >= 48 && charCode <= 57)
            {
                return charCode - 48;
            }
            return -1;
        }

        public static DictImpl DictImpl_clone(ExecutionContext ec, DictImpl original)
        {
            DictImpl output = new DictImpl(ec.nextRefId, original.keyType, original.size, 0, null, null, null, null);
            ec.nextRefId += 1;
            int sz = output.size;
            if (sz > 0)
            {
                output.capacity = sz;
                output.keys = new Value[sz];
                output.values = new Value[sz];
                bool isInt = original.keyType == 3;
                if (isInt)
                {
                    output.intKeyLookup = new Dictionary<int, int>();
                }
                else
                {
                    output.strKeyLookup = new Dictionary<string, int>();
                }
                Value key = null;
                int i = 0;
                while (i < sz)
                {
                    key = original.keys[i];
                    output.keys[i] = key;
                    output.values[i] = original.values[i];
                    if (isInt)
                    {
                        output.intKeyLookup[(int)key.internalValue] = i;
                    }
                    else
                    {
                        output.strKeyLookup[stringUtil_getFlatValue(key)] = i;
                    }
                    i += 1;
                }
            }
            return output;
        }

        public static void DictImpl_ensureCapacity(DictImpl dict)
        {
            int size = dict.size;
            if (size < dict.capacity)
            {
                return;
            }
            int newCapacity = dict.capacity * 2;
            if (newCapacity < 4)
            {
                newCapacity = 4;
            }
            Value[] newKeys = new Value[newCapacity];
            Value[] newValues = new Value[newCapacity];
            Value[] oldKeys = dict.keys;
            Value[] oldValues = dict.values;
            int i = 0;
            while (i < size)
            {
                newKeys[i] = oldKeys[i];
                newValues[i] = oldValues[i];
                i += 1;
            }
            dict.keys = newKeys;
            dict.values = newValues;
            dict.capacity = newCapacity;
        }

        public static Value dictionaryRemove(DictImpl dict, Value key)
        {
            if (dict.keyType != key.type || dict.size == 0)
            {
                return null;
            }
            int index = 0;
            if (key.type == 3)
            {
                int iKey = (int)key.internalValue;
                if (!dict.intKeyLookup.ContainsKey(iKey))
                {
                    return null;
                }
                index = dict.intKeyLookup[iKey];
                dict.intKeyLookup.Remove(iKey);
            }
            else if (key.type == 5)
            {
                string sKey = stringUtil_getFlatValue(key);
                if (!dict.strKeyLookup.ContainsKey(sKey))
                {
                    return null;
                }
                index = dict.strKeyLookup[sKey];
                dict.strKeyLookup.Remove(sKey);
            }
            else
            {
                return null;
            }
            int sz = dict.size - 1;
            dict.size = sz;
            Value output = dict.values[index];
            if (sz == 0)
            {
                dict.intKeyLookup = null;
                dict.strKeyLookup = null;
                dict.keyType = 1;
                dict.keys = null;
                dict.values = null;
            }
            else
            {
                if (index < sz)
                {
                    Value lastKey = dict.keys[sz];
                    dict.keys[index] = lastKey;
                    dict.values[index] = dict.values[sz];
                    if (key.type == 3)
                    {
                        dict.intKeyLookup[(int)lastKey.internalValue] = index;
                    }
                    else
                    {
                        dict.strKeyLookup[stringUtil_getFlatValue(lastKey)] = index;
                    }
                }
                dict.values[sz] = null;
                dict.keys[sz] = null;
            }
            return output;
        }

        public static Value doExponent(GlobalValues g, double left, double right)
        {
            if (left == 0)
            {
                if (right == 0)
                {
                    return g.nullValue;
                }
                return g.intZero;
            }
            if (left == 1)
            {
                return g.intOne;
            }
            if (left < 0 && right % 1 != 0)
            {
                return g.nullValue;
            }
            return new Value(4, System.Math.Pow(left, right));
        }

        public static bool exceptionCatcherRouteException(Value exceptionInstance, int[] args, int[] outBuffer)
        {
            Instance inst = (Instance)exceptionInstance.internalValue;
            int exClassId = inst.classDef.id;
            int groupStartIndex = 1;
            while (groupStartIndex < args.Length)
            {
                int groupJumpPc = args[groupStartIndex];
                int totalExceptionsChecked = 0;
                int i = groupStartIndex + 1;
                while (i < args.Length)
                {
                    groupStartIndex = i + 1;
                    if (args[i] == 0)
                    {
                        i += args.Length;
                    }
                    else
                    {
                        totalExceptionsChecked += 1;
                        if (args[i] == exClassId)
                        {
                            outBuffer[0] = groupJumpPc;
                            return false;
                        }
                    }
                    i += 1;
                }
                if (totalExceptionsChecked == 0)
                {
                    outBuffer[0] = groupJumpPc;
                    return false;
                }
            }
            outBuffer[0] = args[0];
            return true;
        }

        public static ExecutionResult ExRes_Done(ExecutionTask task)
        {
            return new_ExecutionResult(1, task);
        }

        public static ExecutionResult ExRes_HardCrash(ExecutionTask task, string message)
        {
            int isDebug = 1;
            ExecutionResult res = new_ExecutionResult(2, task);
            res.errorMessage = message;
            if (isDebug == 1)
            {
                object[] failArgs = new object[1];
                failArgs[0] = message;
                object ignore = PST_ExtCallbacks["hardCrash"].Invoke(failArgs);
            }
            return res;
        }

        public static ExecutionResult ExRes_Suspend(ExecutionTask task, bool useSleep, int sleepMillis)
        {
            ExecutionResult res = new_ExecutionResult(3, task);
            if (useSleep)
            {
                res.type = 4;
                res.sleepMillis = sleepMillis;
            }
            else
            {
                res.sleepMillis = -1;
            }
            return res;
        }

        public static object fail(string msg)
        {
            object[] failArgs = new object[1];
            failArgs[0] = msg;
            return PST_ExtCallbacks["hardCrash"].Invoke(failArgs);
        }

        public static void finalizeExecutionContext(ExecutionContext ec)
        {
            ByteCodeRow[] byteCode = ec.byteCode;
            int length = ec.byteCode.Length;
            ByteCodeRow row = null;
            int i = 0;
            while (i < length)
            {
                row = byteCode[i];
                if (row.stringId != 0)
                {
                    row.stringArg = ec.stringsById[row.stringId];
                }
                if (row.tokenId != 0)
                {
                    row.token = ec.tokensById[row.tokenId];
                }
                i += 1;
            }
        }

        public static FunctionPointer FunctionPointer_cloneWithNewType(FunctionPointer old, int newType)
        {
            return new FunctionPointer(newType, old.argcMin, old.argcMax, old.pcOrId, old.func, old.ctx);
        }

        public static void generateNameLookup(ExecutionContext ec)
        {
            NameLookup lookup = new NameLookup(0, null);
            System.Collections.Generic.Dictionary<string, int> stringsToId = new Dictionary<string, int>();
            int i = 1;
            int sz = ec.stringsById.Length;
            while (i < sz)
            {
                stringsToId[ec.stringsById[i]] = i;
                i += 1;
            }
            if (stringsToId.ContainsKey("length"))
            {
                lookup.lengthId = stringsToId["length"];
            }
            FunctionPointer[] deadArray = new FunctionPointer[16];
            FunctionPointer[][] fpMap = new FunctionPointer[sz][];
            lookup.primitiveMethodsByTypeByNameId = fpMap;
            injectNameLookup(fpMap, 41, 8, tryGetNameId(stringsToId, "getIntBE"), 0, 2);
            injectNameLookup(fpMap, 42, 8, tryGetNameId(stringsToId, "getIntLE"), 0, 2);
            injectNameLookup(fpMap, 40, 8, tryGetNameId(stringsToId, "toBase64"), 0, 0);
            injectNameLookup(fpMap, 43, 8, tryGetNameId(stringsToId, "toString"), 0, 0);
            injectNameLookup(fpMap, 1, 10, tryGetNameId(stringsToId, "clear"), 0, 0);
            injectNameLookup(fpMap, 2, 10, tryGetNameId(stringsToId, "clone"), 0, 0);
            injectNameLookup(fpMap, 3, 10, tryGetNameId(stringsToId, "contains"), 1, 1);
            injectNameLookup(fpMap, 4, 10, tryGetNameId(stringsToId, "get"), 1, 2);
            injectNameLookup(fpMap, 5, 10, tryGetNameId(stringsToId, "keys"), 0, 0);
            injectNameLookup(fpMap, 6, 10, tryGetNameId(stringsToId, "map"), 1, 1);
            injectNameLookup(fpMap, 7, 10, tryGetNameId(stringsToId, "merge"), 1, 2);
            injectNameLookup(fpMap, 8, 10, tryGetNameId(stringsToId, "remove"), 1, 1);
            injectNameLookup(fpMap, 9, 10, tryGetNameId(stringsToId, "values"), 0, 0);
            injectNameLookup(fpMap, 44, 11, tryGetNameId(stringsToId, "invoke"), 1, 1);
            injectNameLookup(fpMap, 10, 9, tryGetNameId(stringsToId, "add"), 1, 1);
            injectNameLookup(fpMap, 11, 9, tryGetNameId(stringsToId, "clear"), 0, 0);
            injectNameLookup(fpMap, 12, 9, tryGetNameId(stringsToId, "clone"), 0, 0);
            injectNameLookup(fpMap, 14, 9, tryGetNameId(stringsToId, "filter"), 1, 1);
            injectNameLookup(fpMap, 15, 9, tryGetNameId(stringsToId, "find"), 1, 3);
            injectNameLookup(fpMap, 16, 9, tryGetNameId(stringsToId, "findReverse"), 1, 3);
            injectNameLookup(fpMap, 45, 9, tryGetNameId(stringsToId, "insert"), 2, 2);
            injectNameLookup(fpMap, 17, 9, tryGetNameId(stringsToId, "join"), 0, 1);
            injectNameLookup(fpMap, 18, 9, tryGetNameId(stringsToId, "map"), 1, 1);
            injectNameLookup(fpMap, 19, 9, tryGetNameId(stringsToId, "pop"), 0, 0);
            injectNameLookup(fpMap, 21, 9, tryGetNameId(stringsToId, "reduce"), 1, 2);
            injectNameLookup(fpMap, 22, 9, tryGetNameId(stringsToId, "remove"), 1, 1);
            injectNameLookup(fpMap, 23, 9, tryGetNameId(stringsToId, "reverse"), 0, 0);
            injectNameLookup(fpMap, 20, 9, tryGetNameId(stringsToId, "sort"), 0, 1);
            injectNameLookup(fpMap, 39, 9, tryGetNameId(stringsToId, "sortByKey"), 1, 1);
            injectNameLookup(fpMap, 24, 9, tryGetNameId(stringsToId, "toBytes"), 0, 0);
            injectNameLookup(fpMap, 26, 5, tryGetNameId(stringsToId, "endsWith"), 1, 1);
            injectNameLookup(fpMap, 27, 5, tryGetNameId(stringsToId, "find"), 1, 3);
            injectNameLookup(fpMap, 28, 5, tryGetNameId(stringsToId, "findReverse"), 1, 3);
            injectNameLookup(fpMap, 29, 5, tryGetNameId(stringsToId, "getCodePoint"), 1, 1);
            injectNameLookup(fpMap, 30, 5, tryGetNameId(stringsToId, "lower"), 0, 0);
            injectNameLookup(fpMap, 31, 5, tryGetNameId(stringsToId, "replace"), 2, 3);
            injectNameLookup(fpMap, 32, 5, tryGetNameId(stringsToId, "split"), 1, 1);
            injectNameLookup(fpMap, 33, 5, tryGetNameId(stringsToId, "startsWith"), 1, 1);
            injectNameLookup(fpMap, 34, 5, tryGetNameId(stringsToId, "toBytes"), 0, 1);
            injectNameLookup(fpMap, 35, 5, tryGetNameId(stringsToId, "toChars"), 0, 0);
            injectNameLookup(fpMap, 36, 5, tryGetNameId(stringsToId, "toUnicodePoints"), 0, 0);
            injectNameLookup(fpMap, 37, 5, tryGetNameId(stringsToId, "trim"), 0, 0);
            injectNameLookup(fpMap, 38, 5, tryGetNameId(stringsToId, "upper"), 0, 0);
            i = 0;
            while (i < sz)
            {
                if (fpMap[i] == null)
                {
                    fpMap[i] = deadArray;
                }
                i += 1;
            }
            ec.nameLookup = lookup;
        }

        public static Value generateStackTrace(ExecutionTask task)
        {
            StackFrame frame = task.stack;
            ExecutionContext ec = task.execCtx;
            System.Collections.Generic.List<string> trace = new List<string>();
            while (frame != null)
            {
                Token invokeToken = ec.byteCode[frame.pc].token;
                if (invokeToken != null)
                {
                    string info = string.Join("", new string[] { invokeToken.filename, " Line ", invokeToken.line.ToString(), " Col ", invokeToken.col.ToString() });
                    trace.Add(info);
                }
                else
                {
                    trace.Add("PC:" + frame.pc.ToString());
                }
                frame = frame.previous;
            }
            int sz = trace.Count;
            Value[] traceArr = new Value[sz];
            int i = 0;
            while (i < sz)
            {
                traceArr[i] = buildString(ec.globalValues, trace[i], false);
                i += 1;
            }
            return buildList(ec, traceArr, false, traceArr.Length);
        }

        public static void generateTryDescriptors(ExecutionContext ec)
        {
            ByteCodeRow[] bc = ec.byteCode;
            int sz = bc.Length;
            ec.tryDescriptors = new TryDescriptor[sz];
            int i = 0;
            while (i < sz)
            {
                ec.tryDescriptors[i] = null;
                ByteCodeRow row = bc[i];
                if (row.op == 62)
                {
                    int pcTry = i + row.args[0];
                    int pcRouter = pcTry + row.args[1];
                    int pcFinally = pcRouter + row.args[2];
                    int pcEnd = pcFinally + row.args[3];
                    TryDescriptor td = new TryDescriptor(pcTry, pcRouter, pcFinally, pcEnd);
                    pcEnd -= 1;
                    int j = pcTry;
                    while (j < pcEnd)
                    {
                        ec.tryDescriptors[j] = td;
                        j += 1;
                    }
                }
                i += 1;
            }
        }

        public static string getExceptionMessage(Value exceptionInstance, bool includeStackTrace)
        {
            Instance inst = (Instance)exceptionInstance.internalValue;
            Value msgField = inst.members[inst.classDef.nameToOffset["message"]];
            System.Collections.Generic.List<string> lines = new List<string>();
            lines.Add(valueToHumanString(msgField));
            if (includeStackTrace)
            {
                Value stackTrace = inst.members[inst.classDef.nameToOffset["trace"]];
                if (stackTrace.type == 9)
                {
                    ListImpl trace = (ListImpl)stackTrace.internalValue;
                    lines.Add("Stack trace:");
                    int i = 0;
                    while (i < trace.length)
                    {
                        lines.Add("  at " + valueToHumanString(trace.items[i]));
                        i += 1;
                    }
                }
                else
                {
                    lines.Add("(no stack trace available)");
                }
            }
            return string.Join("\n", lines);
        }

        public static GlobalValues getGlobalsFromTask(object taskObj)
        {
            return ((ExecutionTask)taskObj).execCtx.globalValues;
        }

        public static int hexToInt(int charCode)
        {
            if (charCode >= 48 && charCode <= 57)
            {
                return charCode - 48;
            }
            if (charCode >= 97 && charCode <= 122)
            {
                return charCode + -87;
            }
            if (charCode >= 65 && charCode <= 90)
            {
                return charCode + -55;
            }
            return -1;
        }

        public static Value[] increaseValueStackCapacity(ExecutionTask task)
        {
            task.valueStack = valueArrayIncreaseCapacity(task.valueStack);
            return task.valueStack;
        }

        public static void injectNameLookup(FunctionPointer[][] lookup, int primitiveMethodId, int typeId, int nameId, int argcMin, int argcMax)
        {
            if (nameId == -1)
            {
                return;
            }
            FunctionPointer fp = new FunctionPointer(4, argcMin, argcMax, primitiveMethodId, null, null);
            if (lookup[nameId] == null)
            {
                lookup[nameId] = new FunctionPointer[16];
            }
            lookup[nameId][typeId] = fp;
        }

        public static bool isValueEqual(Value a, Value b)
        {
            if (a.type != b.type)
            {
                if (a.type == 3)
                {
                    if (b.type == 4)
                    {
                        return 0.0 + (int)a.internalValue == (double)b.internalValue;
                    }
                }
                else if (a.type == 4)
                {
                    if (b.type == 3)
                    {
                        return (double)a.internalValue == 0.0 + (int)b.internalValue;
                    }
                }
                return false;
            }
            switch (a.type)
            {
                case 1:
                    return true;
                case 2:
                    return (bool)a.internalValue == (bool)b.internalValue;
                case 3:
                    return (int)a.internalValue == (int)b.internalValue;
                case 4:
                    return (double)a.internalValue == (double)b.internalValue;
                case 5:
                    StringImpl s1 = (StringImpl)a.internalValue;
                    StringImpl s2 = (StringImpl)b.internalValue;
                    if (s1 == s2)
                    {
                        return true;
                    }
                    int sLen = s1.length;
                    if (sLen != s2.length)
                    {
                        return false;
                    }
                    if (s1.isBuilder)
                    {
                        stringFlatten(s1);
                    }
                    if (s2.isBuilder)
                    {
                        stringFlatten(s2);
                    }
                    if (sLen == 0)
                    {
                        return true;
                    }
                    if (s1.uChars[0] != s2.uChars[0])
                    {
                        return false;
                    }
                    sLen -= 1;
                    if (s1.uChars[sLen] != s2.uChars[sLen])
                    {
                        return false;
                    }
                    int i = 1;
                    while (i < sLen)
                    {
                        if (s1.uChars[i] != s2.uChars[i])
                        {
                            return false;
                        }
                        i += 1;
                    }
                    return true;
                case 9:
                    return ((ListImpl)a.internalValue).id == ((ListImpl)b.internalValue).id;
                case 10:
                    return ((DictImpl)a.internalValue).id == ((DictImpl)b.internalValue).id;
                case 12:
                    return ((Instance)a.internalValue).id == ((Instance)b.internalValue).id;
                default:
                    return a.internalValue == b.internalValue;
            }
        }

        public static bool json_util_cycle_check_push(System.Collections.Generic.List<int> refIds, int refId)
        {
            int sz = refIds.Count;
            int i = 0;
            while (i < sz)
            {
                if (refIds[i] == refId)
                {
                    return true;
                }
                i += 1;
            }
            refIds.Add(refId);
            return false;
        }

        public static Value json_util_parse(ExecutionContext ec, string rawValue, int[] errOut)
        {
            errOut[0] = 0;
            System.Collections.Generic.List<object> bufferOut = new List<object>();
            object[] args = new object[2];
            args[0] = rawValue;
            args[1] = bufferOut;
            PST_ExtCallbacks["jsonParse"].Invoke(args);
            if (!(bool)bufferOut[0])
            {
                errOut[0] = 1;
                errOut[1] = (int)bufferOut[1];
                errOut[2] = (int)bufferOut[2];
                return ec.globalValues.nullValue;
            }
            int[] index = new int[1];
            index[0] = 1;
            return json_util_parseFromBuf(ec, index, bufferOut);
        }

        public static Value json_util_parseFromBuf(ExecutionContext ec, int[] index, System.Collections.Generic.List<object> buf)
        {
            int i = index[0];
            switch ((int)buf[i])
            {
                case 0:
                    index[0] = i + 1;
                    return ec.globalValues.nullValue;
                case 1:
                    index[0] = i + 2;
                    if ((bool)buf[i + 1])
                    {
                        return ec.globalValues.trueValue;
                    }
                    return ec.globalValues.falseValue;
                case 2:
                    index[0] = i + 2;
                    return buildInteger(ec.globalValues, (int)buf[i + 1]);
                case 3:
                    index[0] = i + 2;
                    return buildFloat((double)buf[i + 1]);
                case 4:
                    index[0] = i + 2;
                    return buildString(ec.globalValues, (string)buf[i + 1], false);
                case 5:
                    int len = (int)buf[i + 1];
                    index[0] = i + 2;
                    Value[] listItems = new Value[len];
                    int j = 0;
                    while (j < len)
                    {
                        listItems[j] = json_util_parseFromBuf(ec, index, buf);
                        j += 1;
                    }
                    return buildList(ec, listItems, false, len);
                case 6:
                    int sz = (int)buf[i + 1];
                    index[0] = i + 2;
                    Value[] keys = new Value[sz];
                    Value[] values = new Value[sz];
                    int k = 0;
                    while (k < sz)
                    {
                        keys[k] = buildString(ec.globalValues, (string)buf[index[0]], false);
                        index[0] = index[0] + 1;
                        values[k] = json_util_parseFromBuf(ec, index, buf);
                        k += 1;
                    }
                    return buildStringDictionary(ec, keys, values);
            }
            fail("");
            return null;
        }

        public static string json_util_serialize(GlobalValues g, Value root, bool useIndent, ListImpl errOut)
        {
            System.Collections.Generic.List<string> stringBuilder = new List<string>();
            System.Collections.Generic.List<int> objectIds = new List<int>();
            System.Collections.Generic.List<string> tabs = new List<string>();
            tabs.Add("");
            string err = json_util_serialize_impl_any(root, useIndent, 0, tabs, stringBuilder, new List<int>());
            if (err != null)
            {
                List_set(errOut, 0, buildString(g, err, false));
                return "";
            }
            return string.Join("", stringBuilder);
        }

        public static string json_util_serialize_dict(Value value, bool useIndent, int currentDepth, System.Collections.Generic.List<string> tabs, System.Collections.Generic.List<string> sb, System.Collections.Generic.List<int> refCycleCheck)
        {
            DictImpl dict = (DictImpl)value.internalValue;
            int sz = dict.size;
            if (sz == 0)
            {
                sb.Add("{}");
                return null;
            }
            if (json_util_cycle_check_push(refCycleCheck, dict.id))
            {
                return "JSON serialization encountered a reference cycle.";
            }
            Value[] keys = dict.keys;
            Value[] values = dict.values;
            int newDepth = currentDepth + 1;
            if (useIndent)
            {
                tabs.Add(tabs[tabs.Count - 1] + "\t");
            }
            sb.Add("{");
            if (useIndent)
            {
                sb.Add("\n");
            }
            int i = 0;
            while (i < sz)
            {
                Value k = keys[i];
                Value v = values[i];
                if (useIndent)
                {
                    sb.Add(tabs[newDepth]);
                }
                json_util_serialize_impl_any(k, useIndent, newDepth, tabs, sb, refCycleCheck);
                if (useIndent)
                {
                    sb.Add(": ");
                }
                else
                {
                    sb.Add(":");
                }
                string err = json_util_serialize_impl_any(v, useIndent, newDepth, tabs, sb, refCycleCheck);
                if (err != null)
                {
                    return err;
                }
                if (i < sz - 1)
                {
                    sb.Add(",");
                }
                if (useIndent)
                {
                    sb.Add("\n");
                }
                i += 1;
            }
            if (useIndent)
            {
                sb.Add(tabs[currentDepth]);
            }
            sb.Add("}");
            PST_ListPop(refCycleCheck);
            return null;
        }

        public static string json_util_serialize_impl_any(Value value, bool useIndent, int currentDepth, System.Collections.Generic.List<string> tabs, System.Collections.Generic.List<string> sb, System.Collections.Generic.List<int> refCycleCheck)
        {
            switch (value.type)
            {
                case 10:
                    return json_util_serialize_dict(value, useIndent, currentDepth, tabs, sb, refCycleCheck);
                case 9:
                    return json_util_serialize_list(value, useIndent, currentDepth, tabs, sb, refCycleCheck);
                case 1:
                    sb.Add("null");
                    return null;
                case 2:
                    if ((bool)value.internalValue)
                    {
                        sb.Add("true");
                    }
                    else
                    {
                        sb.Add("false");
                    }
                    return null;
                case 3:
                    sb.Add(((int)value.internalValue).ToString());
                    return null;
                case 4:
                    sb.Add(PST_FloatToString((double)value.internalValue));
                    return null;
                case 5:
                    StringImpl strImpl = (StringImpl)value.internalValue;
                    if (strImpl.nativeStr == null)
                    {
                        stringFlatten(strImpl);
                    }
                    sb.Add(PST_ToCodeString(strImpl.nativeStr));
                    return null;
                default:
                    return "Invalid JSON serialization type";
            }
        }

        public static string json_util_serialize_list(Value value, bool useIndent, int currentDepth, System.Collections.Generic.List<string> tabs, System.Collections.Generic.List<string> sb, System.Collections.Generic.List<int> refCycleCheck)
        {
            ListImpl list = (ListImpl)value.internalValue;
            int sz = list.length;
            if (sz == 0)
            {
                sb.Add("[]");
                return null;
            }
            if (json_util_cycle_check_push(refCycleCheck, list.id))
            {
                return "JSON serialization encountered a reference cycle.";
            }
            int newDepth = currentDepth + 1;
            if (useIndent)
            {
                tabs.Add(tabs[tabs.Count - 1] + "\t");
            }
            sb.Add("[");
            if (useIndent)
            {
                sb.Add("\n");
            }
            int i = 0;
            while (i < sz)
            {
                Value item = list.items[i];
                if (useIndent)
                {
                    sb.Add(tabs[newDepth]);
                }
                string err = json_util_serialize_impl_any(item, useIndent, newDepth, tabs, sb, refCycleCheck);
                if (err != null)
                {
                    return err;
                }
                if (i < sz - 1)
                {
                    sb.Add(",");
                }
                if (useIndent)
                {
                    sb.Add("\n");
                }
                i += 1;
            }
            if (useIndent)
            {
                sb.Add(tabs[currentDepth]);
            }
            sb.Add("]");
            PST_ListPop(refCycleCheck);
            return null;
        }

        public static void List_add(ListImpl o, Value v)
        {
            if (o.capacity == o.length)
            {
                List_expandCapacity(o);
            }
            o.items[o.length] = v;
            o.length += 1;
        }

        public static void List_expandCapacity(ListImpl o)
        {
            int oldCapacity = o.capacity;
            int newCapacity = oldCapacity * 2;
            if (newCapacity < 4)
            {
                newCapacity = 4;
            }
            Value[] newItems = new Value[newCapacity];
            Value[] oldItems = o.items;
            int i = 0;
            while (i < oldCapacity)
            {
                newItems[i] = oldItems[i];
                i += 1;
            }
            o.capacity = newCapacity;
            o.items = newItems;
        }

        public static Value List_get(ListImpl o, int index)
        {
            if (index < 0)
            {
                index += o.length;
            }
            if (index < 0 || index >= o.length)
            {
                return null;
            }
            return o.items[index];
        }

        public static bool List_insert(ListImpl list, Value value, int index)
        {
            if (index < 0)
            {
                index += list.length;
            }
            if (index < 0 || index > list.length)
            {
                return false;
            }
            if (list.capacity == list.length)
            {
                List_expandCapacity(list);
            }
            int i = list.length - 1;
            while (i >= index)
            {
                list.items[i + 1] = list.items[i];
                i -= 1;
            }
            list.items[index] = value;
            list.length += 1;
            return true;
        }

        public static Value List_join(GlobalValues g, Value v, string sep)
        {
            ListImpl o = (ListImpl)v.internalValue;
            if (o.length == 0)
            {
                return g.emptyString;
            }
            System.Collections.Generic.List<string> output = new List<string>();
            output.Add(valueToHumanString(o.items[0]));
            int sz = o.length;
            int i = 1;
            while (i < sz)
            {
                output.Add(valueToHumanString(o.items[i]));
                i += 1;
            }
            string val = string.Join(sep, output);
            return buildString(g, val, false);
        }

        public static Value List_pop(ListImpl o, Value v)
        {
            if (o.length == 0)
            {
                return null;
            }
            o.length -= 1;
            return o.items[o.length];
        }

        public static bool List_removeAt(ListImpl o, int index)
        {
            if (index < 0)
            {
                index += o.length;
            }
            if (index < 0 || index >= o.length)
            {
                return false;
            }
            o.length -= 1;
            while (index < o.length)
            {
                o.items[index] = o.items[index + 1];
                index += 1;
            }
            return true;
        }

        public static bool List_set(ListImpl o, int index, Value v)
        {
            if (index < 0)
            {
                index += o.length;
            }
            if (index < 0 || index >= o.length)
            {
                return false;
            }
            o.items[index] = v;
            return true;
        }

        public static ByteCodeRow new_ByteCodeRow(int op, int[] args, int stringId, int tokenId)
        {
            int arg1 = 0;
            int arg2 = 0;
            if (args.Length > 0)
            {
                arg1 = args[0];
            }
            if (args.Length > 1)
            {
                arg2 = args[1];
            }
            return new ByteCodeRow(op, args, arg1, arg2, stringId, null, tokenId, null, null, false, false);
        }

        public static ExecutionContext new_ExecutionContext(int[] rawBytes, System.Collections.Generic.Dictionary<string, System.Func<object, object[], object>> extensions, object appCtx, System.Func<object, object> onLastTaskComplete)
        {
            ExecutionContext ec = new ExecutionContext(null, new_GlobalValues(), null, extensions, new Dictionary<string, int>(), null, null, null, null, null, null, null, null, null, null, null, null, null, null, 1, 1, new Dictionary<int, ExecutionTask>(), appCtx, onLastTaskComplete);
            string err = ParseRawData(rawBytes, ec);
            if (err == null)
            {
                ec.errMsg = "CORRUPT_EXECUTABLE";
            }
            else if (err != "OK")
            {
                ec.errMsg = err;
            }
            else
            {
                int i = 0;
                ec.functionsAsValues = new Value[ec.functions.Length];
                i = 1;
                while (i < ec.functions.Length)
                {
                    ec.functionsAsValues[i] = buildFunctionFromInfo(ec.functions[i]);
                    i += 1;
                }
                ec.statelessLambdasAsValues = new Value[ec.lambdas.Length];
                i = 1;
                while (i < ec.lambdas.Length)
                {
                    ec.statelessLambdasAsValues[i] = buildFunctionFromInfo(ec.lambdas[i]);
                    i += 1;
                }
                ec.classRefValues = new Value[ec.classes.Length];
                i = 1;
                while (i < ec.classes.Length)
                {
                    ec.classRefValues[i] = new Value(13, ec.classes[i]);
                    i += 1;
                }
                ec.switchIntLookupsByPc = new System.Collections.Generic.Dictionary<int, int>[ec.byteCode.Length];
                ec.switchStrLookupsByPc = new System.Collections.Generic.Dictionary<string, int>[ec.byteCode.Length];
                generateNameLookup(ec);
            }
            return ec;
        }

        public static ExecutionResult new_ExecutionResult(int type, ExecutionTask task)
        {
            return new ExecutionResult(type, task, 0, null, null);
        }

        public static GlobalValues new_GlobalValues()
        {
            GlobalValues g = new GlobalValues(new Value(1, null), new Value(2, true), new Value(2, false), null, null, null, new Dictionary<string, Value>(), null, null, new Value[5]);
            g.posIntegers = new Value[1200];
            g.negIntegers = new Value[1200];
            int i = 0;
            while (i < 1200)
            {
                g.posIntegers[i] = new Value(3, i);
                g.negIntegers[i] = new Value(3, -i);
                i += 1;
            }
            g.intZero = g.posIntegers[0];
            g.intOne = g.posIntegers[1];
            g.negIntegers[0] = g.intZero;
            g.emptyString = buildString(g, "", true);
            int j = 0;
            while (j < 5)
            {
                g.floatsBy4x[j] = new Value(4, j * 0.25);
                j += 1;
            }
            return g;
        }

        public static bool ParseRaw_entitiesSection_classMemberResolver(System.Collections.Generic.List<ClassInfo> classes, GlobalValues globalValues)
        {
            int id = 1;
            while (id < classes.Count)
            {
                ClassInfo cd = classes[id];
                cd.parent = classes[cd.parentId];
                int newFieldCount = cd.newMembersInOffsetOrder.Count;
                int parentFieldCount = 0;
                if (cd.parent != null)
                {
                    parentFieldCount = cd.parent.nameByOffset.Length;
                }
                int fieldCount = newFieldCount + parentFieldCount;
                string[] nameByOffset = new string[fieldCount];
                int flatOffset = 0;
                while (flatOffset < parentFieldCount)
                {
                    nameByOffset[flatOffset] = cd.parent.nameByOffset[flatOffset];
                    flatOffset += 1;
                }
                int localOffset = 0;
                while (flatOffset < fieldCount)
                {
                    nameByOffset[flatOffset] = cd.newMembersInOffsetOrder[localOffset];
                    flatOffset += 1;
                    localOffset += 1;
                }
                cd.nameByOffset = nameByOffset;
                cd.nameToOffset = new Dictionary<string, int>();
                cd.initialValues = new Value[fieldCount];
                int i = 0;
                while (i < fieldCount)
                {
                    string memberName = nameByOffset[i];
                    cd.nameToOffset[memberName] = i;
                    localOffset = i - parentFieldCount;
                    if (localOffset < 0)
                    {
                        cd.initialValues[i] = cd.parent.initialValues[i];
                        if (cd.parent.methods.ContainsKey(memberName) && !cd.methods.ContainsKey(memberName))
                        {
                            cd.methods[memberName] = cd.parent.methods[memberName];
                        }
                    }
                    else
                    {
                        int memberInfoFlag = cd.newMemberInfoFlags[localOffset];
                        bool isMethod = (memberInfoFlag & 1) != 0;
                        if (isMethod)
                        {
                            cd.initialValues[i] = null;
                        }
                        else
                        {
                            cd.initialValues[i] = globalValues.nullValue;
                        }
                    }
                    i += 1;
                }
                id += 1;
            }
            return true;
        }

        public static System.Collections.Generic.List<ClassInfo> ParseRaw_entitiesSection_parseClasses(RawDataParser rdp, int classCount, System.Collections.Generic.List<FunctionInfo> functions, GlobalValues globalValues)
        {
            int id = 0;
            int i = 0;
            int j = 0;
            System.Collections.Generic.List<ClassInfo> classes = new List<ClassInfo>();
            classes.Add(null);
            ClassInfo cd = null;
            id = 1;
            while (id <= classCount)
            {
                string className = ParseRaw_popLenString(rdp);
                if (className == null)
                {
                    return null;
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int parentId = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int ctorId = rdp.intOut;
                if (ctorId >= functions.Count || ctorId < 1)
                {
                    return null;
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int cctorId = rdp.intOut;
                if (cctorId >= functions.Count || cctorId < 0)
                {
                    return null;
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int fieldCount = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int methodCount = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int staticFieldCount = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int staticMethodCount = rdp.intOut;
                System.Collections.Generic.List<string> newMembersByOffset = new List<string>();
                System.Collections.Generic.List<int> newMemberInfo = new List<int>();
                j = 0;
                while (j < fieldCount)
                {
                    string memberName = ParseRaw_popLenString(rdp);
                    if (memberName == null)
                    {
                        return null;
                    }
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    newMemberInfo.Add(rdp.intOut);
                    newMembersByOffset.Add(memberName);
                    j += 1;
                }
                cd = createClassInfo(id, parentId, className, functions[ctorId], newMembersByOffset, newMemberInfo, cctorId);
                functions[ctorId].classParent = cd;
                j = 0;
                while (j < methodCount)
                {
                    string methodName = ParseRaw_popLenString(rdp);
                    if (methodName == null)
                    {
                        return null;
                    }
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    int methodFuncId = rdp.intOut;
                    cd.methods[methodName] = functions[methodFuncId];
                    j += 1;
                }
                j = 0;
                while (j < staticFieldCount)
                {
                    string stFieldName = ParseRaw_popLenString(rdp);
                    cd.staticMembers[stFieldName] = globalValues.nullValue;
                    cd.staticMemberIsMutable[stFieldName] = true;
                    j += 1;
                }
                j = 0;
                while (j < staticMethodCount)
                {
                    string stMethodName = ParseRaw_popLenString(rdp);
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    int staticMethodFuncId = rdp.intOut;
                    FunctionInfo staticMethodInfo = functions[staticMethodFuncId];
                    cd.methods[stMethodName] = staticMethodInfo;
                    cd.staticMembers[stMethodName] = new Value(11, new FunctionPointer(3, staticMethodInfo.argcMin, staticMethodInfo.argcMax, staticMethodInfo.pc, staticMethodInfo, cd.classRef));
                    cd.staticMemberIsMutable[stMethodName] = false;
                    j += 1;
                }
                classes.Add(cd);
                id += 1;
            }
            return classes;
        }

        public static System.Collections.Generic.List<EnumInfo> ParseRaw_entitiesSection_parseEnums(RawDataParser rdp, int enumCount)
        {
            System.Collections.Generic.List<EnumInfo> enums = new List<EnumInfo>();
            enums.Add(null);
            int i = 0;
            while (i < enumCount)
            {
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int memberCount = rdp.intOut;
                int id = i + 1;
                int[] enumValues = new int[memberCount];
                string[] enumNames = new string[memberCount];
                int j = 0;
                while (j < memberCount)
                {
                    string enumName = ParseRaw_popLenString(rdp);
                    if (enumName == null || !ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    enumNames[j] = enumName;
                    enumValues[j] = rdp.intOut;
                    j++;
                }
                enums.Add(new EnumInfo(id, enumNames, enumValues));
                i += 1;
            }
            return enums;
        }

        public static System.Collections.Generic.List<FunctionInfo> ParseRaw_entitiesSection_parseFunctions(RawDataParser rdp, int fnCount, System.Collections.Generic.List<ByteCodeRow> byteCodeOut, bool isLambda)
        {
            string activeModuleId = null;
            System.Collections.Generic.List<FunctionInfo> functions = new List<FunctionInfo>();
            functions.Add(null);
            int i = 0;
            while (i < fnCount)
            {
                string moduleId = ParseRaw_popLenString(rdp);
                if (moduleId == null)
                {
                    return null;
                }
                if (moduleId.Length > 0)
                {
                    activeModuleId = moduleId;
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int argcMin = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int argcMax = rdp.intOut;
                if (argcMax < argcMin)
                {
                    return null;
                }
                string fnName = null;
                if (!isLambda)
                {
                    fnName = ParseRaw_popLenString(rdp);
                    if (fnName == null)
                    {
                        return null;
                    }
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int codeLen = rdp.intOut;
                ByteCodeRow[] byteCode = ParseRaw_popByteCodeRows(rdp, codeLen);
                if (byteCode == null)
                {
                    return null;
                }
                int pc = byteCodeOut.Count;
                FunctionInfo fn = new FunctionInfo(argcMin, argcMax, pc, null, fnName, activeModuleId);
                int j = 0;
                while (j < codeLen)
                {
                    byteCodeOut.Add(byteCode[j]);
                    j += 1;
                }
                functions.Add(fn);
                i += 1;
            }
            return functions;
        }

        public static RawData_Entities ParseRaw_parseEntityData(RawDataParser rdp, System.Collections.Generic.List<ByteCodeRow> byteCodeOut, GlobalValues globalValues)
        {
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int fnCount = rdp.intOut;
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int enumCount = rdp.intOut;
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int classCount = rdp.intOut;
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int lambdaCount = rdp.intOut;
            System.Collections.Generic.List<FunctionInfo> functions = ParseRaw_entitiesSection_parseFunctions(rdp, fnCount, byteCodeOut, false);
            if (functions == null)
            {
                return null;
            }
            System.Collections.Generic.List<FunctionInfo> lambdas = ParseRaw_entitiesSection_parseFunctions(rdp, lambdaCount, byteCodeOut, true);
            if (lambdas == null)
            {
                return null;
            }
            System.Collections.Generic.List<EnumInfo> enums = ParseRaw_entitiesSection_parseEnums(rdp, enumCount);
            if (enums == null)
            {
                return null;
            }
            System.Collections.Generic.List<ClassInfo> classes = ParseRaw_entitiesSection_parseClasses(rdp, classCount, functions, globalValues);
            if (classes == null)
            {
                return null;
            }
            bool ok = ParseRaw_entitiesSection_classMemberResolver(classes, globalValues);
            if (!ok)
            {
                return null;
            }
            return new RawData_Entities(functions, lambdas, enums, classes);
        }

        public static RawData_Metadata ParseRaw_parseMetadata(RawDataParser rdp)
        {
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int mainFn = rdp.intOut;
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int builtinCount = rdp.intOut;
            return new RawData_Metadata(mainFn, builtinCount);
        }

        public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, EmbeddedResource>> ParseRaw_parseResourceData(RawDataParser rdp)
        {
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, EmbeddedResource>> output = new Dictionary<string, System.Collections.Generic.Dictionary<string, EmbeddedResource>>();
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int moduleCount = rdp.intOut;
            while (moduleCount > 0)
            {
                moduleCount -= 1;
                string moduleId = ParseRaw_popLenString(rdp);
                output[moduleId] = new Dictionary<string, EmbeddedResource>();
                if (moduleId == null)
                {
                    return null;
                }
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int resCount = rdp.intOut;
                while (resCount > 0)
                {
                    resCount -= 1;
                    string path = ParseRaw_popLenString(rdp);
                    int resType = ParseRaw_popSingleByte(rdp, -1);
                    EmbeddedResource res = new EmbeddedResource(moduleId, path, 0, null, 0, -1, -1);
                    if (resType == 1)
                    {
                        string text = ParseRaw_popLenString(rdp);
                        if (text == null)
                        {
                            return null;
                        }
                        res.payload = text;
                        res.type = 1;
                        res.currentState = 1;
                    }
                    else if (resType == 2)
                    {
                        if (!ParseRaw_popInt(rdp))
                        {
                            return null;
                        }
                        int byteLen = rdp.intOut;
                        int[] bytes = ParseRaw_popBytes(rdp, byteLen);
                        if (bytes == null)
                        {
                            return null;
                        }
                        res.payload = bytes;
                        res.type = 2;
                        res.currentState = 1;
                    }
                    else if (resType == 3)
                    {
                        int formatFlag = ParseRaw_popSingleByte(rdp, -1);
                        if (formatFlag < 0)
                        {
                            return null;
                        }
                        bool isJpeg = formatFlag == 1;
                        if (!ParseRaw_popInt(rdp))
                        {
                            return null;
                        }
                        res.imageWidth = rdp.intOut;
                        if (!ParseRaw_popInt(rdp))
                        {
                            return null;
                        }
                        res.imageHeight = rdp.intOut;
                        if (!ParseRaw_popInt(rdp))
                        {
                            return null;
                        }
                        int fileSize = rdp.intOut;
                        res.payload = ParseRaw_popBytes(rdp, fileSize);
                        if (res.payload == null)
                        {
                            return null;
                        }
                        res.type = 3;
                        res.currentState = 2;
                    }
                    else
                    {
                        return null;
                    }
                    output[moduleId][path] = res;
                }
            }
            return output;
        }

        public static string[] ParseRaw_parseStringData(RawDataParser rdp)
        {
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int count = rdp.intOut;
            string[] output = new string[count + 1];
            output[0] = null;
            int i = 0;
            while (i < count)
            {
                string s = ParseRaw_popLenString(rdp);
                if (s == null)
                {
                    return null;
                }
                output[i + 1] = s;
                i += 1;
            }
            return output;
        }

        public static Token[] ParseRaw_parseTokenData(RawDataParser rdp)
        {
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int tokenCount = rdp.intOut;
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            int fileCount = rdp.intOut;
            System.Collections.Generic.List<string> fileNames = new List<string>();
            while (fileCount > 0)
            {
                fileCount -= 1;
                string fileName = ParseRaw_popLenString(rdp);
                if (fileName == null)
                {
                    return null;
                }
                fileNames.Add(fileName);
            }
            System.Collections.Generic.List<Token> output = new List<Token>();
            output.Add(null);
            while (tokenCount > 0)
            {
                tokenCount -= 1;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int fileId = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int line = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int col = rdp.intOut;
                if (fileId < 0 || fileId >= fileNames.Count)
                {
                    return null;
                }
                output.Add(new Token(fileNames[fileId], line, col));
            }
            return output.ToArray();
        }

        public static ByteCodeRow[] ParseRaw_popByteCodeRows(RawDataParser rdp, int rowCount)
        {
            ByteCodeRow[] rows = new ByteCodeRow[rowCount];
            int i = 0;
            while (i < rowCount)
            {
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int op = rdp.intOut;
                if (!ParseRaw_popInt(rdp))
                {
                    return null;
                }
                int flags = rdp.intOut;
                bool hasString = (flags & 1) != 0;
                bool hasToken = (flags & 2) != 0;
                int argc = flags >> 2;
                int stringId = 0;
                if (hasString)
                {
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    stringId = rdp.intOut;
                }
                int tokenId = 0;
                if (hasToken)
                {
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    tokenId = rdp.intOut;
                }
                int[] args = new int[argc];
                int j = 0;
                while (j < argc)
                {
                    if (!ParseRaw_popInt(rdp))
                    {
                        return null;
                    }
                    args[j] = rdp.intOut;
                    j += 1;
                }
                rows[i] = new_ByteCodeRow(op, args, stringId, tokenId);
                i += 1;
            }
            return rows;
        }

        public static int[] ParseRaw_popBytes(RawDataParser rdp, int byteCount)
        {
            if (rdp.index + byteCount > rdp.length)
            {
                return null;
            }
            int[] output = new int[byteCount];
            int i = 0;
            while (i < byteCount)
            {
                output[i] = rdp.rawBytes[i + rdp.index];
                i += 1;
            }
            rdp.index += byteCount;
            return output;
        }

        public static string ParseRaw_popFixedLenString(RawDataParser rdp, int expectedSize)
        {
            if (rdp.index + expectedSize > rdp.length)
            {
                return null;
            }
            int[] strBytes = new int[expectedSize];
            int i = 0;
            while (i < expectedSize)
            {
                strBytes[i] = rdp.rawBytes[rdp.index + i] & 255;
                i += 1;
            }
            rdp.index += expectedSize;
            string value = System.Text.Encoding.UTF8.GetString((strBytes).Select(v => (byte)v).ToArray());
            return value;
        }

        public static bool ParseRaw_popInt(RawDataParser rdp)
        {
            if (rdp.index >= rdp.length)
            {
                return false;
            }
            int b = rdp.rawBytes[rdp.index];
            if (b < 128)
            {
                rdp.intOut = b;
                rdp.index += 1;
                return true;
            }
            if (b == 192)
            {
                rdp.intOut = -2147483647;
                rdp.index += 1;
                return true;
            }
            if (b == 224)
            {
                rdp.index += 1;
                return false;
            }
            int sign = 1;
            if ((b & 16) != 0)
            {
                sign = -1;
            }
            int byteCount = b & 15;
            int output = 0;
            rdp.index += 1;
            int i = 0;
            while (i < byteCount)
            {
                output = output << 8;
                output += rdp.rawBytes[rdp.index];
                rdp.index += 1;
                i += 1;
            }
            rdp.intOut = output * sign;
            return true;
        }

        public static string ParseRaw_popLenString(RawDataParser rdp)
        {
            if (!ParseRaw_popInt(rdp))
            {
                return null;
            }
            return ParseRaw_popFixedLenString(rdp, rdp.intOut);
        }

        public static int ParseRaw_popSingleByte(RawDataParser rdp, int fallback)
        {
            if (rdp.index >= rdp.length)
            {
                return fallback;
            }
            int val = rdp.rawBytes[rdp.index];
            rdp.index += 1;
            return val;
        }

        public static string ParseRawData(int[] rawBytes, ExecutionContext ec)
        {
            RawDataParser rdp = new RawDataParser(rawBytes, 0, rawBytes.Length, 0);
            if (ParseRaw_popFixedLenString(rdp, 4) != "PXCS")
            {
                return null;
            }
            if (ParseRaw_popSingleByte(rdp, -1) != 0)
            {
                return null;
            }
            int[] versionData = ParseRaw_popBytes(rdp, 3);
            if (versionData == null)
            {
                return null;
            }
            int majorVersion = versionData[0];
            int minorVersion = versionData[1];
            int patchVersion = versionData[2];
            string flavor = ParseRaw_popLenString(rdp);
            string flavorVersion = ParseRaw_popLenString(rdp);
            if (flavor == null || flavorVersion == null)
            {
                return null;
            }
            RawData_Metadata mtd = null;
            string[] stringById = null;
            Token[] tokensById = null;
            RawData_Entities ent = null;
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, EmbeddedResource>> resourcesByPathByModuleId = null;
            System.Collections.Generic.List<ByteCodeRow> byteCodeAcc = new List<ByteCodeRow>();
            while (rdp.index < rdp.length)
            {
                string chunkType = ParseRaw_popFixedLenString(rdp, 3);
                if (chunkType == null)
                {
                    return null;
                }
                if (chunkType == "MTD")
                {
                    if (mtd != null)
                    {
                        return null;
                    }
                    mtd = ParseRaw_parseMetadata(rdp);
                    if (mtd == null)
                    {
                        return null;
                    }
                }
                else if (chunkType == "TOK")
                {
                    if (tokensById != null)
                    {
                        return null;
                    }
                    tokensById = ParseRaw_parseTokenData(rdp);
                    if (tokensById == null)
                    {
                        return null;
                    }
                }
                else if (chunkType == "STR")
                {
                    if (stringById != null)
                    {
                        return null;
                    }
                    stringById = ParseRaw_parseStringData(rdp);
                    if (stringById == null)
                    {
                        return null;
                    }
                }
                else if (chunkType == "ENT")
                {
                    if (ent != null)
                    {
                        return null;
                    }
                    ent = ParseRaw_parseEntityData(rdp, byteCodeAcc, ec.globalValues);
                    if (ent == null)
                    {
                        return null;
                    }
                }
                else if (chunkType == "RSC")
                {
                    if (resourcesByPathByModuleId != null)
                    {
                        return null;
                    }
                    resourcesByPathByModuleId = ParseRaw_parseResourceData(rdp);
                    if (resourcesByPathByModuleId == null)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            if (mtd == null || (stringById == null || ent == null))
            {
                return null;
            }
            ec.functions = ent.functionsById.ToArray();
            ec.lambdas = ent.lambdasById.ToArray();
            ec.enums = ent.enumsById.ToArray();
            ec.classes = ent.classesById.ToArray();
            ec.byteCode = byteCodeAcc.ToArray();
            ec.tokensById = tokensById;
            ec.stringsById = stringById;
            ec.embeddedResources = resourcesByPathByModuleId;
            ec.significantFunctions["main"] = mtd.mainFunctionId;
            int i = 1;
            while (i <= mtd.builtinCount)
            {
                string fnName = ec.functions[i].name;
                if (fnName == "map" || (fnName == "filter" || (fnName == "kvpMap" || (fnName == "reduce" || (fnName == "thrw" || (fnName == "sort" || fnName == "sortK"))))))
                {
                    ec.significantFunctions[fnName] = i;
                }
                i += 1;
            }
            generateTryDescriptors(ec);
            return "OK";
        }

        public static int[] PUBLIC_base64ToBytes(string b64str)
        {
            return System.Convert.FromBase64String(b64str).Select(b => (int)b).ToArray();
        }

        public static object PUBLIC_createTaskForFunction(object ecCtx, object fpValueNoArgs)
        {
            return PUBLIC_createTaskForFunctionWithWrappedArgs(ecCtx, fpValueNoArgs, new object[0]);
        }

        public static object PUBLIC_createTaskForFunctionWithWrappedArgs(object ecCtx, object fpValue, object[] wrappedArgs)
        {
            int argc = wrappedArgs.Length;
            Value[] args = new Value[argc];
            int i = 0;
            while (i < argc)
            {
                args[i] = (Value)wrappedArgs[i];
                i += 1;
            }
            return createNewTask((ExecutionContext)ecCtx, (Value)fpValue, args);
        }

        public static object PUBLIC_getApplicationContextFromTask(object taskObj)
        {
            ExecutionTask task = (ExecutionTask)taskObj;
            return task.execCtx.appCtx;
        }

        public static string PUBLIC_getExecutionContextError(object ecObj)
        {
            ExecutionContext ec = (ExecutionContext)ecObj;
            return ec.errMsg;
        }

        public static object PUBLIC_getExecutionContextFromTask(object taskObj)
        {
            return ((ExecutionTask)taskObj).execCtx;
        }

        public static object PUBLIC_getTaskAsyncHandle(object taskObj)
        {
            return ((ExecutionTask)taskObj).nativeAsyncHandle;
        }

        public static string[] PUBLIC_getTaskResultError(object resObj, bool includeStackTrace)
        {
            ExecutionResult result = (ExecutionResult)resObj;
            System.Collections.Generic.List<string> o = new List<string>();
            if (result.type != 2)
            {
                return null;
            }
            o.Add(result.errorMessage);
            System.Collections.Generic.List<string> tr = result.stackTrace;
            if (tr != null)
            {
                int i = 0;
                while (i < tr.Count)
                {
                    o.Add(tr[i]);
                    i += 1;
                }
            }
            return o.ToArray();
        }

        public static int PUBLIC_getTaskResultSleepAmount(object resObj)
        {
            ExecutionResult result = (ExecutionResult)resObj;
            return result.sleepMillis;
        }

        public static int PUBLIC_getTaskResultStatus(object resObj)
        {
            ExecutionResult result = (ExecutionResult)resObj;
            return result.type;
        }

        public static object PUBLIC_initializeExecutionContext(int[] rawBytes, System.Collections.Generic.Dictionary<string, System.Func<object, object[], object>> extensions, object appCtx, System.Func<object, object> onLastTaskComplete)
        {
            return new_ExecutionContext(rawBytes, extensions, appCtx, onLastTaskComplete);
        }

        public static object PUBLIC_listGet(object listObj, int i)
        {
            return ((ListImpl)((Value)listObj).internalValue).items[i];
        }

        public static int PUBLIC_listLength(object listObj)
        {
            return ((ListImpl)((Value)listObj).internalValue).items.Length;
        }

        public static void PUBLIC_listSet(object listObj, int i, object valObj)
        {
            ((ListImpl)((Value)listObj).internalValue).items[i] = (Value)valObj;
        }

        public static void PUBLIC_listValueAdd(object listObj, object wrappedValue)
        {
            ListImpl list = (ListImpl)((Value)listObj).internalValue;
            List_add(list, (Value)wrappedValue);
        }

        public static void PUBLIC_requestTaskSuspension(object taskObj, bool withSleep, int sleepMillis)
        {
            ExecutionTask task = (ExecutionTask)taskObj;
            task.suspendRequested = true;
            if (sleepMillis < 0)
            {
                sleepMillis = 0;
            }
            if (withSleep)
            {
                task.sleepMillis = sleepMillis;
            }
            else
            {
                task.sleepMillis = -1;
            }
        }

        public static object PUBLIC_resumeTask(object taskObj)
        {
            return RunInterpreter((ExecutionTask)taskObj);
        }

        public static void PUBLIC_setTaskAsyncHandle(object taskObj, object handle)
        {
            ((ExecutionTask)taskObj).nativeAsyncHandle = handle;
        }

        public static void PUBLIC_setTaskStackTopValue(object taskObj, Value newValue)
        {
            ExecutionTask task = (ExecutionTask)taskObj;
            int valueStackSize = task.stack.valueStackSize;
            if (valueStackSize > 0)
            {
                task.valueStack[valueStackSize - 1] = newValue;
            }
        }

        public static object PUBLIC_startMainTask(object ecObj, string[] args)
        {
            ExecutionTask mainTask = createMainTask((ExecutionContext)ecObj, args);
            return PUBLIC_resumeTask(mainTask);
        }

        public static double PUBLIC_unwrapFloat(object val)
        {
            return 1.0 * (double)((Value)val).internalValue;
        }

        public static int PUBLIC_unwrapInteger(object val)
        {
            return (int)((Value)val).internalValue;
        }

        public static object PUBLIC_unwrapNativeHandle(object val)
        {
            return ((Value)val).internalValue;
        }

        public static string PUBLIC_valueToString(object valueObj)
        {
            return valueToHumanString((Value)valueObj);
        }

        public static Value PUBLIC_wrapBoolean(object taskObj, bool val)
        {
            GlobalValues g = getGlobalsFromTask(taskObj);
            if (val)
            {
                return g.trueValue;
            }
            return g.falseValue;
        }

        public static Value PUBLIC_wrapInteger(object taskObj, int val)
        {
            GlobalValues g = getGlobalsFromTask(taskObj);
            return buildInteger(g, val);
        }

        public static object PUBLIC_wrapNativeHandle(object val)
        {
            return new Value(14, val);
        }

        public static Value PUBLIC_wrapString(object taskObj, string val, bool isCommon)
        {
            GlobalValues g = getGlobalsFromTask(taskObj);
            return buildString(g, val, isCommon);
        }

        public static void removeFinishedTaskFromExecContext(ExecutionTask task)
        {
            task.execCtx.tasks.Remove(task.taskId);
            if (task.execCtx.tasks.Count == 0)
            {
                task.execCtx.onLastTaskComplete((object)null);
            }
        }

        public static ExecutionResult RunInterpreter(ExecutionTask task)
        {
            bool reinvoke = true;
            ExecutionResult result = null;
            while (reinvoke)
            {
                result = RunInterpreterImpl(task);
                reinvoke = result.type == 5;
            }
            if (result.type != 3 && result.type != 4)
            {
                removeFinishedTaskFromExecContext(task);
            }
            return result;
        }

        public static ExecutionResult RunInterpreterImpl(ExecutionTask task)
        {
            ExecutionContext ec = task.execCtx;
            StackFrame frame = task.stack;
            StackFrame nextFrame = null;
            ByteCodeRow[] byteCode = ec.byteCode;
            ByteCodeRow row = null;
            GlobalValues globalValues = ec.globalValues;
            TryDescriptor td = null;
            NameLookup nameLookup = ec.nameLookup;
            int LENGTH_ID = nameLookup.lengthId;
            FunctionPointer[][] primitiveMethodLookup = nameLookup.primitiveMethodsByTypeByNameId;
            int pc = frame.pc;
            Value[] valueStack = task.valueStack;
            int valueStackSize = frame.valueStackSize;
            int valueStackCapacity = valueStack.Length;
            System.Collections.Generic.Dictionary<string, Value> locals = frame.locals;
            bool interrupt = false;
            bool doInvoke = false;
            bool overrideReturnValueWithContext = false;
            int errorId = 0;
            string errorMsg = null;
            int i = 0;
            int j = 0;
            int argc = 0;
            int int1 = 0;
            int int2 = 0;
            int int3 = 0;
            int sz = 0;
            int leftType = 0;
            int rightType = 0;
            bool bool1 = true;
            bool bool2 = true;
            bool bool3 = true;
            string str1 = null;
            string str2 = null;
            string name = null;
            string strOut = null;
            double float1 = 0.0;
            double float2 = 0.0;
            double float3 = 0.0;
            object object1 = null;
            Value value = null;
            Value value1 = null;
            Value value2 = null;
            Value output = null;
            Value left = null;
            Value right = null;
            FunctionPointer fp = null;
            FunctionInfo fn = null;
            StringImpl stringImpl1 = null;
            StringImpl stringImpl2 = null;
            ListImpl listImpl1 = null;
            ListImpl listImpl2 = null;
            DictImpl dictImpl1 = null;
            Instance instance1 = null;
            ClassInfo classDef = null;
            ClassInfo classDef2 = null;
            Value[] args = null;
            Value[] valueArr = null;
            int[] intArray1 = null;
            object[] objArr = null;
            Value[] keys = null;
            Value[] values = null;
            string[] strArr = null;
            System.Func<object, object[], object> extensionFunc = null;
            System.Collections.Generic.Dictionary<string, int> opMap = null;
            System.Collections.Generic.Dictionary<string, FunctionInfo> str2FuncDef = null;
            System.Collections.Generic.Dictionary<string, Value> str2Val = null;
            System.Collections.Generic.Dictionary<int, int> switchIntLookup = null;
            System.Collections.Generic.Dictionary<string, int> switchStrLookup = null;
            System.Collections.Generic.Dictionary<string, EmbeddedResource> resourceLookup = null;
            EmbeddedResource resource = null;
            System.Collections.Generic.List<int> intList = null;
            System.Collections.Generic.List<Value> valueList = null;
            Value VALUE_TRUE = globalValues.trueValue;
            Value VALUE_FALSE = globalValues.falseValue;
            Value VALUE_NULL = globalValues.nullValue;
            Value[] value16 = new Value[16];
            int[] intBuffer16 = new int[16];
            double[] floatBuffer16 = new double[16];
            while (true)
            {
                row = byteCode[pc];
                switch (row.op)
                {
                    case 1:
                        // OP_ASSIGN_FIELD;
                        valueStackSize -= 2;
                        if (row.firstArg == 0)
                        {
                            right = valueStack[valueStackSize];
                            left = valueStack[valueStackSize + 1];
                        }
                        else
                        {
                            left = valueStack[valueStackSize];
                            right = valueStack[valueStackSize + 1];
                        }
                        switch (left.type)
                        {
                            case 12:
                                instance1 = (Instance)left.internalValue;
                                if (!instance1.classDef.nameToOffset.ContainsKey(row.stringArg))
                                {
                                    errorId = 3;
                                    errorMsg = string.Join("", new string[] { "The ", instance1.classDef.name, " class does not have a field named .", row.stringArg, " and therefore cannot be assigned to." });
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                else
                                {
                                    int1 = instance1.classDef.nameToOffset[row.stringArg];
                                    instance1.members[int1] = right;
                                }
                                break;
                            case 13:
                                classDef = (ClassInfo)left.internalValue;
                                str1 = row.stringArg;
                                if (classDef.staticMemberIsMutable[str1])
                                {
                                    classDef.staticMembers[str1] = right;
                                }
                                else
                                {
                                    errorId = 11;
                                    errorMsg = "Static methods cannot be overwritten.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                break;
                            default:
                                errorId = 3;
                                errorMsg = "Cannot assign fields on this type.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 2:
                        // OP_ASSIGN_INDEX;
                        valueStackSize -= 3;
                        if (row.firstArg == 0)
                        {
                            value = valueStack[valueStackSize];
                            left = valueStack[valueStackSize + 1];
                            right = valueStack[valueStackSize + 2];
                        }
                        else
                        {
                            left = valueStack[valueStackSize];
                            right = valueStack[valueStackSize + 1];
                            value = valueStack[valueStackSize + 2];
                        }
                        switch (left.type * 16 + right.type)
                        {
                            case 147:
                                i = (int)right.internalValue;
                                listImpl1 = (ListImpl)left.internalValue;
                                if (i < 0)
                                {
                                    i += listImpl1.length;
                                    if (i < 0)
                                    {
                                        bool1 = true;
                                    }
                                    else
                                    {
                                        bool1 = false;
                                    }
                                }
                                else if (i >= listImpl1.length)
                                {
                                    bool1 = true;
                                }
                                else
                                {
                                    bool1 = false;
                                }
                                if (bool1)
                                {
                                    errorId = 8;
                                    errorMsg = "Array is out of bounds.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                else
                                {
                                    listImpl1.items[i] = value;
                                }
                                break;
                            case 165:
                                stringImpl1 = (StringImpl)right.internalValue;
                                if (stringImpl1.isBuilder)
                                {
                                    stringFlatten(stringImpl1);
                                }
                                str1 = stringImpl1.nativeStr;
                                dictImpl1 = (DictImpl)left.internalValue;
                                if (dictImpl1.keyType == 1)
                                {
                                    dictImpl1.keyType = 5;
                                    dictImpl1.strKeyLookup = new Dictionary<string, int>();
                                    dictImpl1.strKeyLookup[str1] = 0;
                                    dictImpl1.keys = new Value[4];
                                    dictImpl1.values = new Value[4];
                                    dictImpl1.capacity = 4;
                                    dictImpl1.keys[0] = right;
                                    dictImpl1.values[0] = value;
                                    dictImpl1.size = 1;
                                }
                                else if (dictImpl1.keyType == 5)
                                {
                                    if (dictImpl1.size == dictImpl1.capacity)
                                    {
                                        DictImpl_ensureCapacity(dictImpl1);
                                    }
                                    if (dictImpl1.strKeyLookup.ContainsKey(str1))
                                    {
                                        dictImpl1.values[dictImpl1.strKeyLookup[str1]] = value;
                                    }
                                    else
                                    {
                                        i = dictImpl1.size;
                                        dictImpl1.size = i + 1;
                                        dictImpl1.keys[i] = right;
                                        dictImpl1.values[i] = value;
                                        dictImpl1.strKeyLookup[str1] = i;
                                    }
                                }
                                else
                                {
                                    errorId = 4;
                                    errorMsg = "Cannot mix types of dictionary keys in the same dictionary.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                break;
                            case 163:
                                int1 = (int)right.internalValue;
                                dictImpl1 = (DictImpl)left.internalValue;
                                if (dictImpl1.keyType == 1)
                                {
                                    dictImpl1.keyType = 3;
                                    dictImpl1.intKeyLookup = new Dictionary<int, int>();
                                    dictImpl1.intKeyLookup[int1] = 0;
                                    dictImpl1.keys = new Value[4];
                                    dictImpl1.values = new Value[4];
                                    dictImpl1.capacity = 4;
                                    dictImpl1.keys[0] = right;
                                    dictImpl1.values[0] = value;
                                    dictImpl1.size = 1;
                                }
                                else
                                {
                                    if (dictImpl1.size == dictImpl1.capacity)
                                    {
                                        DictImpl_ensureCapacity(dictImpl1);
                                    }
                                    if (dictImpl1.intKeyLookup.ContainsKey(int1))
                                    {
                                        dictImpl1.values[dictImpl1.intKeyLookup[int1]] = value;
                                    }
                                    else
                                    {
                                        i = dictImpl1.size;
                                        dictImpl1.size = i + 1;
                                        dictImpl1.keys[i] = right;
                                        dictImpl1.values[i] = value;
                                        dictImpl1.intKeyLookup[int1] = i;
                                    }
                                }
                                break;
                            default:
                                errorId = 4;
                                errorMsg = "Cannot assign an index to this type.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 3:
                        // OP_ASSIGN_VAR;
                        valueStackSize -= 1;
                        locals[row.stringArg] = valueStack[valueStackSize];
                        break;
                    case 4:
                        // OP_BIN_OP;
                        if (opMap == null)
                        {
                            opMap = new Dictionary<string, int>();
                            opMap["+"] = 1;
                            opMap["-"] = 2;
                            opMap["*"] = 3;
                            opMap["/"] = 4;
                            opMap["%"] = 5;
                            opMap["**"] = 20;
                            opMap["&&"] = 6;
                            opMap["||"] = 7;
                            opMap["=="] = 8;
                            opMap["!="] = 9;
                            opMap[">"] = 10;
                            opMap["<"] = 11;
                            opMap[">="] = 12;
                            opMap["<="] = 13;
                            opMap["&"] = 14;
                            opMap["|"] = 15;
                            opMap["^"] = 16;
                            opMap["<<"] = 17;
                            opMap[">>"] = 18;
                            opMap[">>>"] = 19;
                        }
                        str1 = row.stringArg;
                        if (str1 == "+")
                        {
                            row.op = 5;
                        }
                        else if (str1 == "==" || str1 == "!=")
                        {
                            row.boolArg = str1 == "!=";
                            row.op = 8;
                        }
                        else if (str1 == "<" || (str1 == ">" || (str1 == "<=" || str1 == ">=")))
                        {
                            row.boolArg = str1 == "<" || str1 == "<=";
                            row.boolArg2 = str1 == "<=" || str1 == ">=";
                            row.firstArg = opMap[str1];
                            row.op = 7;
                        }
                        else if (str1 == "&" || (str1 == "|" || (str1 == "^" || (str1 == "<<" || (str1 == ">>" || str1 == ">>>")))))
                        {
                            row.firstArg = opMap[str1];
                            row.op = 6;
                        }
                        else if (str1 == "is")
                        {
                            row.op = 64;
                        }
                        else
                        {
                            row.firstArg = opMap[str1];
                            row.op = 9;
                        }
                        pc -= 1;
                        break;
                    case 5:
                        // OP_BIN_OP_ADD;
                        valueStackSize -= 2;
                        left = valueStack[valueStackSize];
                        right = valueStack[valueStackSize + 1];
                        switch (left.type << 5 | right.type)
                        {
                            case 99:
                                i = (int)left.internalValue + (int)right.internalValue;
                                if (i < 1200 && i >= 1200)
                                {
                                    if (i < 0)
                                    {
                                        value = globalValues.negIntegers[-i];
                                    }
                                    else
                                    {
                                        value = globalValues.posIntegers[i];
                                    }
                                }
                                else
                                {
                                    value = new Value(3, i);
                                }
                                break;
                            case 132:
                                value = new Value(4, (double)left.internalValue + (double)right.internalValue);
                                break;
                            case 131:
                                value = new Value(4, (double)left.internalValue + (int)right.internalValue);
                                break;
                            case 100:
                                value = new Value(4, (int)left.internalValue + (double)right.internalValue);
                                break;
                            default:
                                if (left.type == 5 || right.type == 5)
                                {
                                    stringImpl1 = convertToStringImpl(globalValues, left);
                                    stringImpl2 = convertToStringImpl(globalValues, right);
                                    stringImpl1 = new StringImpl(stringImpl1.length + stringImpl2.length, true, null, null, stringImpl1, stringImpl2);
                                    value = new Value(5, stringImpl1);
                                }
                                else
                                {
                                    errorId = 4;
                                    errorMsg = "Unsupported addition operation between types.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                break;
                        }
                        valueStack[valueStackSize] = value;
                        valueStackSize += 1;
                        break;
                    case 7:
                        // OP_BIN_OP_COMPARE;
                        valueStackSize -= 2;
                        left = valueStack[valueStackSize];
                        right = valueStack[valueStackSize + 1];
                        switch (left.type * 16 + right.type)
                        {
                            case 51:
                                int1 = (int)left.internalValue;
                                int2 = (int)right.internalValue;
                                bool1 = int1 == int2;
                                bool2 = int1 < int2;
                                break;
                            case 52:
                                int1 = (int)left.internalValue;
                                float2 = (double)right.internalValue;
                                bool1 = int1 == float2;
                                bool2 = int1 < float2;
                                break;
                            case 67:
                                float1 = (double)left.internalValue;
                                int2 = (int)right.internalValue;
                                bool1 = float1 == int2;
                                bool2 = float1 < int2;
                                break;
                            case 68:
                                float1 = (double)left.internalValue;
                                float2 = (double)right.internalValue;
                                bool1 = float1 == float2;
                                bool2 = float1 < float2;
                                break;
                            default:
                                errorId = 4;
                                errorMsg = "Comparisons are only applicable for numeric types.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        value = VALUE_FALSE;
                        switch (row.firstArg)
                        {
                            case 11:
                                if (bool2)
                                {
                                    value = VALUE_TRUE;
                                }
                                break;
                            case 13:
                                if (bool2 || bool1)
                                {
                                    value = VALUE_TRUE;
                                }
                                break;
                            case 10:
                                if (!bool2 && !bool1)
                                {
                                    value = VALUE_TRUE;
                                }
                                break;
                            case 12:
                                if (!bool2)
                                {
                                    value = VALUE_TRUE;
                                }
                                break;
                            default:
                                break;
                        }
                        valueStack[valueStackSize] = value;
                        valueStackSize += 1;
                        break;
                    case 8:
                        // OP_BIN_OP_EQUAL;
                        valueStackSize -= 2;
                        left = valueStack[valueStackSize];
                        right = valueStack[valueStackSize + 1];
                        if (left.type != right.type)
                        {
                            if (left.type == 3)
                            {
                                if (right.type == 4)
                                {
                                    int1 = (int)left.internalValue;
                                    bool1 = int1 == (double)right.internalValue;
                                }
                                else
                                {
                                    bool1 = false;
                                }
                            }
                            else if (left.type == 4)
                            {
                                if (left.type == 3)
                                {
                                    int1 = (int)right.internalValue;
                                    bool1 = int1 == (double)left.internalValue;
                                }
                                else
                                {
                                    bool1 = false;
                                }
                            }
                            else
                            {
                                bool1 = false;
                            }
                        }
                        else
                        {
                            switch (left.type)
                            {
                                case 1:
                                    bool1 = true;
                                    break;
                                case 2:
                                    bool1 = (bool)left.internalValue == (bool)right.internalValue;
                                    break;
                                case 3:
                                    bool1 = (int)left.internalValue == (int)right.internalValue;
                                    break;
                                case 4:
                                    bool1 = (double)left.internalValue == (double)right.internalValue;
                                    break;
                                case 5:
                                    if (left.internalValue == right.internalValue)
                                    {
                                        bool1 = true;
                                    }
                                    else
                                    {
                                        stringImpl1 = (StringImpl)left.internalValue;
                                        stringImpl2 = (StringImpl)right.internalValue;
                                        if (stringImpl1.length != stringImpl2.length)
                                        {
                                            bool1 = false;
                                        }
                                        else
                                        {
                                            if (stringImpl1.isBuilder)
                                            {
                                                stringFlatten(stringImpl1);
                                            }
                                            if (stringImpl2.isBuilder)
                                            {
                                                stringFlatten(stringImpl2);
                                            }
                                            bool1 = stringImpl1.nativeStr == stringImpl2.nativeStr;
                                        }
                                    }
                                    break;
                                default:
                                    bool1 = left.internalValue == right.internalValue;
                                    break;
                            }
                        }
                        if (row.boolArg)
                        {
                            bool1 = !bool1;
                        }
                        if (bool1)
                        {
                            valueStack[valueStackSize] = globalValues.trueValue;
                        }
                        else
                        {
                            valueStack[valueStackSize] = globalValues.falseValue;
                        }
                        valueStackSize += 1;
                        break;
                    case 64:
                        // OP_BIN_OP_IS;
                        valueStackSize -= 1;
                        left = valueStack[valueStackSize - 1];
                        right = valueStack[valueStackSize];
                        if (right.type != 13)
                        {
                            errorId = 4;
                            errorMsg = "'is' operator can only be used with class names.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        if (left.type != 12)
                        {
                            output = VALUE_FALSE;
                        }
                        else
                        {
                            classDef2 = (ClassInfo)right.internalValue;
                            instance1 = (Instance)left.internalValue;
                            classDef = instance1.classDef;
                            output = VALUE_FALSE;
                            while (classDef != null)
                            {
                                if (classDef2.id == classDef.id)
                                {
                                    output = VALUE_TRUE;
                                    classDef = null;
                                }
                                else
                                {
                                    classDef = classDef.parent;
                                }
                            }
                        }
                        valueStack[valueStackSize - 1] = output;
                        break;
                    case 9:
                        // OP_BIN_OP_MATH;
                        valueStackSize -= 2;
                        left = valueStack[valueStackSize];
                        right = valueStack[valueStackSize + 1];
                        switch ((left.type * 21 + row.firstArg) * 16 + right.type)
                        {
                            case 1331:
                                value = doExponent(globalValues, 0.0 + (int)left.internalValue, 0.0 + (int)right.internalValue);
                                break;
                            case 1667:
                                value = doExponent(globalValues, (double)left.internalValue, 0.0 + (int)right.internalValue);
                                break;
                            case 1332:
                                value = doExponent(globalValues, 0.0 + (int)left.internalValue, (double)right.internalValue);
                                break;
                            case 1668:
                                value = doExponent(globalValues, (double)left.internalValue, (double)right.internalValue);
                                break;
                            case 1043:
                                int1 = (int)left.internalValue;
                                int2 = (int)right.internalValue;
                                value = buildInteger(globalValues, int1 - int2);
                                break;
                            case 1059:
                                int1 = (int)left.internalValue;
                                int2 = (int)right.internalValue;
                                value = buildInteger(globalValues, int1 * int2);
                                break;
                            case 1395:
                                value = new Value(4, (double)left.internalValue * (int)right.internalValue);
                                break;
                            case 1060:
                                value = new Value(4, (int)left.internalValue * (double)right.internalValue);
                                break;
                            case 1396:
                                value = new Value(4, (double)left.internalValue * (double)right.internalValue);
                                break;
                            case 1075:
                                int1 = (int)left.internalValue;
                                int2 = (int)right.internalValue;
                                if (int2 == 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Cannot divide by zero";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                value = buildInteger(globalValues, int1 / int2);
                                break;
                            case 1411:
                                int1 = (int)right.internalValue;
                                if (int1 == 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Cannot divide by zero";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                value = new Value(4, (double)left.internalValue / int1);
                                break;
                            case 1076:
                                float1 = (double)right.internalValue;
                                if (float1 == 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Cannot divide by zero";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                value = new Value(4, (int)left.internalValue / float1);
                                break;
                            case 1412:
                                float1 = (double)right.internalValue;
                                if (float1 == 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Cannot divide by zero";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                value = new Value(4, (double)left.internalValue / float1);
                                break;
                            case 1091:
                                int1 = (int)left.internalValue;
                                int2 = (int)right.internalValue;
                                if (int2 <= 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Modulo only applicable to positive divisors.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                int1 = int1 % int2;
                                if (int1 < 0)
                                {
                                    int1 += int2;
                                }
                                value = buildInteger(globalValues, int1);
                                break;
                            case 1428:
                                if (left.type == 3)
                                {
                                    float1 = 0.0 + (int)left.internalValue;
                                }
                                else
                                {
                                    float1 = (double)left.internalValue;
                                }
                                if (right.type == 3)
                                {
                                    float2 = 0.0 + (int)right.internalValue;
                                }
                                else
                                {
                                    float2 = (double)right.internalValue;
                                }
                                if (float2 <= 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Modulo only applicable to positive divisors.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                float1 = float1 % float2;
                                if (float1 < 0)
                                {
                                    float1 += float2;
                                }
                                value = new Value(4, float1);
                                break;
                            case 1427:
                                if (left.type == 3)
                                {
                                    float1 = 0.0 + (int)left.internalValue;
                                }
                                else
                                {
                                    float1 = (double)left.internalValue;
                                }
                                if (right.type == 3)
                                {
                                    float2 = 0.0 + (int)right.internalValue;
                                }
                                else
                                {
                                    float2 = (double)right.internalValue;
                                }
                                if (float2 <= 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Modulo only applicable to positive divisors.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                float1 = float1 % float2;
                                if (float1 < 0)
                                {
                                    float1 += float2;
                                }
                                value = new Value(4, float1);
                                break;
                            case 1092:
                                if (left.type == 3)
                                {
                                    float1 = 0.0 + (int)left.internalValue;
                                }
                                else
                                {
                                    float1 = (double)left.internalValue;
                                }
                                if (right.type == 3)
                                {
                                    float2 = 0.0 + (int)right.internalValue;
                                }
                                else
                                {
                                    float2 = (double)right.internalValue;
                                }
                                if (float2 <= 0)
                                {
                                    errorId = 10;
                                    errorMsg = "Modulo only applicable to positive divisors.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                float1 = float1 % float2;
                                if (float1 < 0)
                                {
                                    float1 += float2;
                                }
                                value = new Value(4, float1);
                                break;
                            case 1380:
                                value = new Value(4, (double)left.internalValue - (double)right.internalValue);
                                break;
                            case 1044:
                                value = new Value(4, (int)left.internalValue - (double)right.internalValue);
                                break;
                            case 1379:
                                value = new Value(4, (double)left.internalValue - (int)right.internalValue);
                                break;
                            case 1731:
                                if (left.type == 5)
                                {
                                    value2 = left;
                                    stringImpl1 = (StringImpl)left.internalValue;
                                    sz = (int)right.internalValue;
                                }
                                else
                                {
                                    value2 = right;
                                    stringImpl1 = (StringImpl)right.internalValue;
                                    sz = (int)left.internalValue;
                                }
                                if (sz == 0)
                                {
                                    value = globalValues.emptyString;
                                }
                                else if (sz == 1)
                                {
                                    value = value2;
                                }
                                else
                                {
                                    stringImpl2 = stringImpl1;
                                    i = 1;
                                    while (i < sz)
                                    {
                                        stringImpl2 = new StringImpl(stringImpl1.length + stringImpl2.length, true, null, null, stringImpl1, stringImpl2);
                                        i += 1;
                                    }
                                    value = new Value(5, stringImpl2);
                                }
                                break;
                            case 1061:
                                if (left.type == 5)
                                {
                                    value2 = left;
                                    stringImpl1 = (StringImpl)left.internalValue;
                                    sz = (int)right.internalValue;
                                }
                                else
                                {
                                    value2 = right;
                                    stringImpl1 = (StringImpl)right.internalValue;
                                    sz = (int)left.internalValue;
                                }
                                if (sz == 0)
                                {
                                    value = globalValues.emptyString;
                                }
                                else if (sz == 1)
                                {
                                    value = value2;
                                }
                                else
                                {
                                    stringImpl2 = stringImpl1;
                                    i = 1;
                                    while (i < sz)
                                    {
                                        stringImpl2 = new StringImpl(stringImpl1.length + stringImpl2.length, true, null, null, stringImpl1, stringImpl2);
                                        i += 1;
                                    }
                                    value = new Value(5, stringImpl2);
                                }
                                break;
                            default:
                                errorId = 4;
                                errorMsg = "Math operator not defined for these types";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        valueStack[valueStackSize] = value;
                        valueStackSize += 1;
                        break;
                    case 6:
                        // OP_BIN_OP_BIT_MATH;
                        valueStackSize -= 2;
                        left = valueStack[valueStackSize];
                        right = valueStack[valueStackSize + 1];
                        if (left.type != 3 || right.type != 3)
                        {
                            errorId = 9;
                            errorMsg = "Expected integers for this operator.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        int1 = (int)left.internalValue;
                        int2 = (int)right.internalValue;
                        switch (row.firstArg)
                        {
                            case 14:
                                int3 = int1 & int2;
                                break;
                            case 15:
                                int3 = int1 | int2;
                                break;
                            case 16:
                                int3 = int1 ^ int2;
                                break;
                            case 17:
                                int3 = int1 << int2;
                                break;
                            case 18:
                                int3 = int1 >> int2;
                                break;
                            default:
                                return ExRes_HardCrash(task, "Op not implemented yet");
                        }
                        if (int1 == int3)
                        {
                            value = left;
                        }
                        else if (int2 == int3)
                        {
                            value = right;
                        }
                        else
                        {
                            value = buildInteger(globalValues, int3);
                        }
                        valueStack[valueStackSize] = value;
                        valueStackSize += 1;
                        break;
                    case 10:
                        // OP_BITWISE_NOT;
                        i = valueStackSize - 1;
                        value = valueStack[i];
                        int1 = -(int)value.internalValue - 1;
                        valueStack[i] = buildInteger(globalValues, int1);
                        break;
                    case 11:
                        // OP_BOOLEAN_NOT;
                        i = valueStackSize - 1;
                        value = valueStack[i];
                        if (value.type != 2)
                        {
                            errorId = 9;
                            errorMsg = "Only a boolean can be used here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        if ((bool)value.internalValue)
                        {
                            valueStack[i] = VALUE_FALSE;
                        }
                        else
                        {
                            valueStack[i] = VALUE_TRUE;
                        }
                        break;
                    case 13:
                        // OP_BUILD_DICT;
                        sz = row.firstArg;
                        int1 = 1;
                        if (sz == 0)
                        {
                            if (valueStackSize == valueStackCapacity)
                            {
                                valueStack = increaseValueStackCapacity(task);
                                valueStackCapacity = valueStack.Length;
                            }
                            int2 = 0;
                            keys = null;
                            values = null;
                        }
                        else
                        {
                            int2 = sz;
                            keys = new Value[sz];
                            values = new Value[sz];
                            valueStackSize -= sz * 2;
                            j = valueStackSize;
                            i = 0;
                            while (i < sz)
                            {
                                keys[i] = valueStack[j];
                                values[i] = valueStack[j + 1];
                                j += 2;
                                i += 1;
                            }
                            int1 = keys[0].type;
                        }
                        dictImpl1 = new DictImpl(ec.nextRefId, int1, sz, int2, keys, values, null, null);
                        ec.nextRefId += 1;
                        if (dictImpl1.keyType == 3)
                        {
                            dictImpl1.intKeyLookup = new Dictionary<int, int>();
                            i = 0;
                            while (i < sz)
                            {
                                dictImpl1.intKeyLookup[(int)keys[i].internalValue] = i;
                                i += 1;
                            }
                        }
                        else if (dictImpl1.keyType == 5)
                        {
                            dictImpl1.strKeyLookup = new Dictionary<string, int>();
                            i = 0;
                            while (i < sz)
                            {
                                value = keys[i];
                                stringImpl1 = (StringImpl)value.internalValue;
                                if (stringImpl1.isBuilder)
                                {
                                    stringFlatten(stringImpl1);
                                }
                                dictImpl1.strKeyLookup[(string)stringImpl1.nativeStr] = i;
                                i += 1;
                            }
                        }
                        valueStack[valueStackSize] = new Value(10, dictImpl1);
                        valueStackSize += 1;
                        keys = null;
                        values = null;
                        break;
                    case 12:
                        // OP_BUILD_LIST;
                        sz = row.firstArg;
                        args = new Value[sz];
                        valueStackSize -= sz;
                        i = 0;
                        while (i < sz)
                        {
                            args[i] = valueStack[valueStackSize + i];
                            i += 1;
                        }
                        if (sz == 0)
                        {
                            if (valueStackSize == valueStackCapacity)
                            {
                                valueStack = increaseValueStackCapacity(task);
                                valueStackCapacity = valueStack.Length;
                            }
                        }
                        valueStack[valueStackSize] = new Value(9, new ListImpl(ec.nextRefId, sz, sz, args));
                        valueStackSize += 1;
                        ec.nextRefId += 1;
                        args = null;
                        break;
                    case 14:
                        // OP_CTOR_REF;
                        int1 = row.firstArg;
                        row.valueCache = ec.classes[int1].ctor;
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 15:
                        // OP_DOT_FIELD;
                        i = valueStackSize - 1;
                        value = valueStack[i];
                        int3 = -1;
                        j = row.stringId;
                        name = row.stringArg;
                        switch (value.type)
                        {
                            case 8:
                                if (j == LENGTH_ID)
                                {
                                    sz = ((int[])value.internalValue).Length;
                                    if (sz < 1200)
                                    {
                                        output = globalValues.posIntegers[sz];
                                    }
                                    else
                                    {
                                        output = buildInteger(globalValues, sz);
                                    }
                                }
                                else
                                {
                                    fp = primitiveMethodLookup[j][8];
                                    if (fp == null)
                                    {
                                        output = null;
                                    }
                                    else
                                    {
                                        output = new Value(11, new FunctionPointer(4, fp.argcMin, fp.argcMax, fp.pcOrId, null, value));
                                    }
                                }
                                break;
                            case 5:
                                if (j == LENGTH_ID)
                                {
                                    sz = ((StringImpl)value.internalValue).length;
                                    if (sz < 1200)
                                    {
                                        output = globalValues.posIntegers[sz];
                                    }
                                    else
                                    {
                                        output = buildInteger(globalValues, sz);
                                    }
                                }
                                else
                                {
                                    fp = primitiveMethodLookup[j][5];
                                    if (fp == null)
                                    {
                                        output = null;
                                    }
                                    else
                                    {
                                        output = new Value(11, new FunctionPointer(4, fp.argcMin, fp.argcMax, fp.pcOrId, null, value));
                                    }
                                }
                                break;
                            case 9:
                                if (j == LENGTH_ID)
                                {
                                    sz = ((ListImpl)value.internalValue).length;
                                    if (sz < 1200)
                                    {
                                        output = globalValues.posIntegers[sz];
                                    }
                                    else
                                    {
                                        output = buildInteger(globalValues, sz);
                                    }
                                }
                                else
                                {
                                    fp = primitiveMethodLookup[j][9];
                                    if (fp == null)
                                    {
                                        output = null;
                                    }
                                    else
                                    {
                                        output = new Value(11, new FunctionPointer(4, fp.argcMin, fp.argcMax, fp.pcOrId, null, value));
                                    }
                                }
                                break;
                            case 10:
                                fp = primitiveMethodLookup[j][10];
                                if (fp == null)
                                {
                                    output = null;
                                }
                                else
                                {
                                    output = new Value(11, new FunctionPointer(4, fp.argcMin, fp.argcMax, fp.pcOrId, null, value));
                                }
                                break;
                            case 11:
                                fp = primitiveMethodLookup[j][11];
                                if (fp == null)
                                {
                                    output = null;
                                }
                                else
                                {
                                    output = new Value(11, new FunctionPointer(4, fp.argcMin, fp.argcMax, fp.pcOrId, null, value));
                                }
                                break;
                            case 12:
                                instance1 = (Instance)value.internalValue;
                                if (!instance1.classDef.nameToOffset.ContainsKey(name))
                                {
                                    errorId = 3;
                                    errorMsg = string.Join("", new string[] { "The class '", instance1.classDef.name, "' does not contain a field named '.", name, "'." });
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                int1 = instance1.classDef.nameToOffset[name];
                                output = instance1.members[int1];
                                if (output == null)
                                {
                                    str2FuncDef = instance1.classDef.methods;
                                    fn = str2FuncDef[name];
                                    fp = new FunctionPointer(2, fn.argcMin, fn.argcMax, fn.pc, fn, value);
                                    output = new Value(11, fp);
                                    instance1.members[int1] = output;
                                }
                                break;
                            case 13:
                                str2Val = ((ClassInfo)value.internalValue).staticMembers;
                                if (!str2Val.ContainsKey(name))
                                {
                                    frame.pc = pc;
                                    frame.valueStackSize = valueStackSize;
                                    task.stack = frame;
                                    return ExRes_HardCrash(task, "TODO: all static fields need to be initialized. This should eventually not happen.");
                                }
                                output = str2Val[name];
                                break;
                            default:
                                output = null;
                                break;
                        }
                        if (output == null)
                        {
                            errorId = 3;
                            errorMsg = "Field not found: ." + row.stringArg;
                            if (value.type == 1)
                            {
                                errorId = 7;
                                errorMsg = "Cannot access fields on null.";
                                if (byteCode[pc - 1].op == 21)
                                {
                                    errorMsg = "The function returned null and the field could not be accessed.";
                                }
                            }
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        valueStack[i] = output;
                        break;
                    case 16:
                        // OP_ENSURE_BOOL;
                        if (valueStack[valueStackSize - 1].type != 2)
                        {
                            errorId = 9;
                            errorMsg = "Expected a boolean here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 18:
                        // OP_ENSURE_INT;
                        if (valueStack[valueStackSize - 1].type != 3)
                        {
                            errorId = 9;
                            errorMsg = "Expected an integer here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 66:
                        // OP_ENSURE_LIST;
                        if (valueStack[valueStackSize - 1].type != 9)
                        {
                            errorId = 9;
                            errorMsg = "Expected a list here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 17:
                        // OP_ENSURE_INT_OR_STRING;
                        i = valueStack[valueStackSize - 1].type;
                        if (i != 3 && i != 5)
                        {
                            errorId = 9;
                            errorMsg = "Expected an integer or string here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 19:
                        // OP_ENSURE_STRING;
                        if (valueStack[valueStackSize - 1].type != 5)
                        {
                            errorId = 9;
                            errorMsg = "Expected a string here.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 20:
                        // OP_EXT_INVOKE;
                        name = row.stringArg;
                        argc = row.firstArg;
                        if (ec.extensions.ContainsKey(name))
                        {
                            objArr = new object[argc];
                            i = argc - 1;
                            while (i >= 0)
                            {
                                valueStackSize -= 1;
                                objArr[i] = valueStack[valueStackSize];
                                i -= 1;
                            }
                            frame.pc = pc;
                            frame.valueStackSize = valueStackSize;
                            task.stack = frame;
                            extensionFunc = ec.extensions[name];
                            value = (Value)(extensionFunc((object)task, objArr));
                            objArr = null;
                            if (value == null)
                            {
                                value = globalValues.nullValue;
                            }
                            valueStack[valueStackSize] = value;
                            valueStackSize += 1;
                            if (task.suspendRequested)
                            {
                                frame.pc = pc + 1;
                                task.suspendRequested = false;
                                frame.valueStackSize = valueStackSize;
                                return ExRes_Suspend(task, task.sleepMillis >= 0, task.sleepMillis);
                            }
                        }
                        else
                        {
                            errorId = 5;
                            errorMsg = string.Join("", new string[] { "There is no extension function named '", name, "'." });
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 21:
                        // OP_FUNCTION_INVOKE;
                        argc = row.firstArg;
                        args = new Value[argc];
                        i = argc - 1;
                        while (i >= 0)
                        {
                            valueStackSize -= 1;
                            args[i] = valueStack[valueStackSize];
                            i -= 1;
                        }
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        if (value.type != 11)
                        {
                            errorId = 5;
                            errorMsg = "This is not a function";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        fp = (FunctionPointer)value.internalValue;
                        if (argc < fp.argcMin || argc > fp.argcMax)
                        {
                            errorId = 5;
                            errorMsg = "Incorrect number of arguments.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        doInvoke = false;
                        switch (fp.funcType)
                        {
                            case 1:
                                doInvoke = true;
                                value = fp.ctx;
                                overrideReturnValueWithContext = false;
                                break;
                            case 2:
                                doInvoke = true;
                                value = fp.ctx;
                                overrideReturnValueWithContext = false;
                                break;
                            case 3:
                                doInvoke = true;
                                value = fp.ctx;
                                overrideReturnValueWithContext = false;
                                break;
                            case 7:
                                doInvoke = true;
                                value = fp.ctx;
                                overrideReturnValueWithContext = false;
                                break;
                            case 5:
                                classDef = fp.func.classParent;
                                valueArr = classDef.initialValues;
                                sz = valueArr.Length;
                                values = new Value[sz];
                                i = 0;
                                while (i < sz)
                                {
                                    values[i] = valueArr[i];
                                    i += 1;
                                }
                                instance1 = new Instance(ec.nextRefId, classDef, values);
                                ec.nextRefId += 1;
                                value = new Value(12, instance1);
                                doInvoke = true;
                                overrideReturnValueWithContext = true;
                                break;
                            case 6:
                                doInvoke = true;
                                value = frame.context;
                                overrideReturnValueWithContext = false;
                                break;
                            case 4:
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                switch (fp.pcOrId)
                                {
                                    case 44:
                                        if (args[0].type != 9)
                                        {
                                            errorId = 4;
                                            errorMsg = "function.invoke(args) requires an array of arguments";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        listImpl1 = (ListImpl)args[0].internalValue;
                                        argc = listImpl1.length;
                                        args = new Value[argc];
                                        i = 0;
                                        while (i < listImpl1.length)
                                        {
                                            args[i] = listImpl1.items[i];
                                            i += 1;
                                        }
                                        doInvoke = true;
                                        fp = (FunctionPointer)fp.ctx.internalValue;
                                        value = fp.ctx;
                                        overrideReturnValueWithContext = false;
                                        if (args.Length < fp.argcMin || args.Length > fp.argcMax)
                                        {
                                            errorId = 5;
                                            errorMsg = "Incorrect number of arguments.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        break;
                                    case 40:
                                        output = buildString(globalValues, buildBase64String((int[])fp.ctx.internalValue), false);
                                        break;
                                    case 1:
                                        dictImpl1 = (DictImpl)fp.ctx.internalValue;
                                        if (dictImpl1.size > 0)
                                        {
                                            dictImpl1.keyType = 1;
                                            dictImpl1.keys = null;
                                            dictImpl1.values = null;
                                            dictImpl1.intKeyLookup = null;
                                            dictImpl1.strKeyLookup = null;
                                            dictImpl1.size = 0;
                                        }
                                        output = VALUE_NULL;
                                        break;
                                    case 2:
                                        output = new Value(10, DictImpl_clone(ec, (DictImpl)fp.ctx.internalValue));
                                        break;
                                    case 3:
                                        dictImpl1 = (DictImpl)fp.ctx.internalValue;
                                        output = VALUE_FALSE;
                                        value = args[0];
                                        if (dictImpl1.keyType == value.type)
                                        {
                                            if (value.type == 5 && dictImpl1.strKeyLookup != null)
                                            {
                                                if (dictImpl1.strKeyLookup.ContainsKey(stringUtil_getFlatValue(value)))
                                                {
                                                    output = VALUE_TRUE;
                                                }
                                            }
                                            else if (value.type == 3 && dictImpl1.intKeyLookup != null)
                                            {
                                                if (dictImpl1.intKeyLookup.ContainsKey((int)value.internalValue))
                                                {
                                                    output = VALUE_TRUE;
                                                }
                                            }
                                        }
                                        break;
                                    case 4:
                                        dictImpl1 = (DictImpl)fp.ctx.internalValue;
                                        if (argc == 2)
                                        {
                                            output = args[1];
                                        }
                                        else
                                        {
                                            output = VALUE_NULL;
                                        }
                                        value = args[0];
                                        if (dictImpl1.keyType == value.type)
                                        {
                                            if (value.type == 5 && dictImpl1.strKeyLookup != null)
                                            {
                                                str1 = stringUtil_getFlatValue(value);
                                                if (dictImpl1.strKeyLookup.ContainsKey(str1))
                                                {
                                                    output = dictImpl1.values[dictImpl1.strKeyLookup[str1]];
                                                }
                                            }
                                            else if (value.type == 3 && dictImpl1.intKeyLookup != null)
                                            {
                                                i = (int)value.internalValue;
                                                if (dictImpl1.intKeyLookup.ContainsKey(i))
                                                {
                                                    output = dictImpl1.values[dictImpl1.intKeyLookup[i]];
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        dictImpl1 = (DictImpl)fp.ctx.internalValue;
                                        output = buildList(ec, dictImpl1.keys, true, dictImpl1.size);
                                        break;
                                    case 6:
                                        value16[0] = fp.ctx;
                                        value16[1] = args[0];
                                        args = value16;
                                        argc += 1;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["kvpMap"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 8:
                                        output = dictionaryRemove((DictImpl)fp.ctx.internalValue, args[0]);
                                        if (output == null)
                                        {
                                            errorId = 6;
                                            errorMsg = "Dictionary does not have that key";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        break;
                                    case 9:
                                        dictImpl1 = (DictImpl)fp.ctx.internalValue;
                                        output = buildList(ec, dictImpl1.values, true, dictImpl1.size);
                                        break;
                                    case 10:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        if (listImpl1.length == listImpl1.capacity)
                                        {
                                            List_expandCapacity(listImpl1);
                                        }
                                        output = globalValues.nullValue;
                                        listImpl1.items[listImpl1.length] = args[0];
                                        listImpl1.length += 1;
                                        break;
                                    case 11:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length;
                                        valueArr = listImpl1.items;
                                        i = 0;
                                        while (i < sz)
                                        {
                                            valueArr[i] = null;
                                            i += 1;
                                        }
                                        listImpl1.length = 0;
                                        output = VALUE_NULL;
                                        break;
                                    case 12:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        output = buildList(ec, listImpl1.items, true, listImpl1.length);
                                        break;
                                    case 14:
                                        value16[0] = fp.ctx;
                                        value16[1] = args[0];
                                        args = value16;
                                        argc += 1;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["filter"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 15:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length;
                                        int2 = 0;
                                        int3 = sz;
                                        if (argc > 1)
                                        {
                                            value = args[1];
                                            if (value.type != 3)
                                            {
                                                errorId = 4;
                                                errorMsg = "starting index must be an integer.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                            int2 = (int)value.internalValue;
                                            if (int2 < 0)
                                            {
                                                int2 += sz;
                                            }
                                            if (int2 < 0 || int2 >= sz)
                                            {
                                                errorId = 8;
                                                errorMsg = "starting index out of range.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                        }
                                        if (argc == 3)
                                        {
                                            value = args[2];
                                            if (value.type != 3)
                                            {
                                                errorId = 4;
                                                errorMsg = "end index must be an integer.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                            int3 = (int)value.internalValue;
                                            if (int3 < 0)
                                            {
                                                int3 += sz;
                                            }
                                            if (int3 < 0 || int3 >= sz)
                                            {
                                                errorId = 8;
                                                errorMsg = "end index out of range.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                        }
                                        valueArr = listImpl1.items;
                                        left = args[0];
                                        leftType = left.type;
                                        object1 = left.internalValue;
                                        bool1 = false;
                                        output = globalValues.negIntegers[1];
                                        i = int2;
                                        while (i < int3)
                                        {
                                            if (isValueEqual(left, valueArr[i]))
                                            {
                                                output = buildInteger(globalValues, i);
                                                i += sz;
                                            }
                                            i += 1;
                                        }
                                        break;
                                    case 16:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length;
                                        int2 = sz - 1;
                                        int3 = -1;
                                        if (argc > 1)
                                        {
                                            value = args[1];
                                            if (value.type != 3)
                                            {
                                                errorId = 4;
                                                errorMsg = "starting index must be an integer.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                            int2 = (int)value.internalValue;
                                            if (int2 < 0)
                                            {
                                                int2 += sz;
                                            }
                                            if (int2 < 0 || int2 >= sz)
                                            {
                                                errorId = 8;
                                                errorMsg = "starting index out of range.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                        }
                                        if (argc == 3)
                                        {
                                            value = args[2];
                                            if (value.type != 3)
                                            {
                                                errorId = 4;
                                                errorMsg = "end index must be an integer.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                            int3 = (int)value.internalValue;
                                            if (int3 < -1)
                                            {
                                                int3 += sz;
                                            }
                                            if (int3 < -1 || int3 > sz - 1)
                                            {
                                                errorId = 8;
                                                errorMsg = "end index out of range.";
                                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                            }
                                        }
                                        valueArr = listImpl1.items;
                                        left = args[0];
                                        leftType = left.type;
                                        object1 = left.internalValue;
                                        bool1 = false;
                                        output = globalValues.negIntegers[1];
                                        i = int2;
                                        while (i > int3)
                                        {
                                            if (isValueEqual(left, valueArr[i]))
                                            {
                                                output = buildInteger(globalValues, i);
                                                i -= int3;
                                            }
                                            i -= 1;
                                        }
                                        break;
                                    case 45:
                                        value = args[1];
                                        if (value.type != 3)
                                        {
                                            errorId = 4;
                                            errorMsg = "List index must be an integer.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        if (!List_insert((ListImpl)fp.ctx.internalValue, args[0], (int)value.internalValue))
                                        {
                                            errorId = 8;
                                            errorMsg = "Index is out of range.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        output = VALUE_NULL;
                                        break;
                                    case 17:
                                        str1 = "";
                                        if (argc == 1)
                                        {
                                            value1 = args[0];
                                            if (value1.type != 5)
                                            {
                                                return ThrowErrorImpl(task, 4, "list.join(sep) requires a string");
                                            }
                                            stringImpl1 = (StringImpl)value1.internalValue;
                                            if (stringImpl1.isBuilder)
                                            {
                                                stringFlatten(stringImpl1);
                                            }
                                            str1 = stringImpl1.nativeStr;
                                        }
                                        output = List_join(globalValues, fp.ctx, str1);
                                        break;
                                    case 18:
                                        value16[0] = fp.ctx;
                                        value16[1] = args[0];
                                        args = value16;
                                        argc += 1;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["map"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 19:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length - 1;
                                        if (sz == -1)
                                        {
                                            errorId = 8;
                                            errorMsg = "Cannot pop from an empty list.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        output = listImpl1.items[sz];
                                        listImpl1.items[sz] = null;
                                        listImpl1.length = sz;
                                        break;
                                    case 21:
                                        value16[0] = fp.ctx;
                                        value16[1] = args[0];
                                        value16[2] = null;
                                        if (args.Length == 2)
                                        {
                                            value16[2] = args[1];
                                        }
                                        argc += 1;
                                        args = value16;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["reduce"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 23:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length;
                                        int1 = sz >> 1;
                                        valueArr = listImpl1.items;
                                        j = sz - 1;
                                        i = 0;
                                        while (i < int1)
                                        {
                                            value = valueArr[i];
                                            valueArr[i] = valueArr[j];
                                            valueArr[j] = value;
                                            j -= 1;
                                            i += 1;
                                        }
                                        output = fp.ctx;
                                        break;
                                    case 22:
                                        value = args[0];
                                        if (value.type != 3)
                                        {
                                            errorId = 4;
                                            errorMsg = "list.remove() requires a valid index integer.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        j = (int)value.internalValue;
                                        sz = listImpl1.length;
                                        if (j < 0)
                                        {
                                            j += sz;
                                        }
                                        if (j >= sz || j < 0)
                                        {
                                            errorId = 8;
                                            errorMsg = "list.remove() was given an index out of range.";
                                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                        }
                                        valueArr = listImpl1.items;
                                        output = valueArr[j];
                                        i = j + 1;
                                        while (i < sz)
                                        {
                                            valueArr[i - 1] = valueArr[i];
                                            i += 1;
                                        }
                                        sz -= 1;
                                        listImpl1.length = sz;
                                        listImpl1.items[sz] = null;
                                        break;
                                    case 20:
                                        value16[0] = fp.ctx;
                                        if (args.Length == 1)
                                        {
                                            value16[1] = args[0];
                                        }
                                        argc += 1;
                                        args = value16;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["sort"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 39:
                                        value16[0] = fp.ctx;
                                        value16[1] = args[0];
                                        argc += 1;
                                        args = value16;
                                        fp = (FunctionPointer)ec.functionsAsValues[ec.significantFunctions["sortK"]].internalValue;
                                        doInvoke = true;
                                        overrideReturnValueWithContext = false;
                                        break;
                                    case 24:
                                        listImpl1 = (ListImpl)fp.ctx.internalValue;
                                        sz = listImpl1.length;
                                        intArray1 = new int[sz];
                                        i = 0;
                                        while (i < sz)
                                        {
                                            left = listImpl1.items[i];
                                            if (left.type != 3)
                                            {
                                                return ThrowErrorImpl(task, 4, "Only lists of integers can be converted to byte buffers.");
                                            }
                                            intArray1[i] = 255 & (int)left.internalValue;
                                            i += 1;
                                        }
                                        output = new Value(8, intArray1);
                                        intArray1 = null;
                                        break;
                                    case 29:
                                        value1 = args[0];
                                        if (value1.type != 3)
                                        {
                                            return ThrowErrorImpl(task, 4, "string.getCodePoint() requires an integer index ");
                                        }
                                        i = (int)value1.internalValue;
                                        stringImpl1 = (StringImpl)fp.ctx.internalValue;
                                        sz = stringImpl1.length;
                                        if (i < 0)
                                        {
                                            i += sz;
                                        }
                                        if (i < 0 || i >= sz)
                                        {
                                            return ThrowErrorImpl(task, 8, "Index out of range.");
                                        }
                                        if (stringImpl1.isBuilder)
                                        {
                                            stringFlatten(stringImpl1);
                                        }
                                        output = buildInteger(globalValues, stringImpl1.uChars[i]);
                                        break;
                                    case 30:
                                        output = stringUtil_changeCase(fp.ctx, false);
                                        break;
                                    case 31:
                                        left = args[0];
                                        right = args[1];
                                        if (left.type != 5 || right.type != 5)
                                        {
                                            return ThrowErrorImpl(task, 4, "string.replace(searchVal, newValue) requires a string arguments");
                                        }
                                        str1 = stringUtil_getFlatValue(fp.ctx);
                                        strArr = PST_StringSplit(str1, stringUtil_getFlatValue(left));
                                        if (strArr.Length == 1)
                                        {
                                            output = fp.ctx;
                                        }
                                        else
                                        {
                                            str1 = string.Join(stringUtil_getFlatValue(right), strArr);
                                            output = buildString(globalValues, str1, false);
                                        }
                                        strArr = null;
                                        break;
                                    case 32:
                                        value1 = args[0];
                                        if (value1.type != 5)
                                        {
                                            return ThrowErrorImpl(task, 4, "string.split(sep) requires a string separator");
                                        }
                                        str1 = valueToHumanString(value1);
                                        valueArr = stringUtil_split(globalValues, fp.ctx, str1);
                                        sz = valueArr.Length;
                                        output = new Value(9, new ListImpl(ec.nextRefId, sz, sz, valueArr));
                                        valueArr = null;
                                        ec.nextRefId += 1;
                                        break;
                                    case 36:
                                        stringImpl1 = (StringImpl)fp.ctx.internalValue;
                                        sz = stringImpl1.length;
                                        valueArr = new Value[sz];
                                        intArray1 = stringImpl1.uChars;
                                        i = 0;
                                        while (i < sz)
                                        {
                                            j = intArray1[i];
                                            if (j < 1200)
                                            {
                                                valueArr[i] = globalValues.posIntegers[j];
                                            }
                                            else
                                            {
                                                valueArr[i] = buildInteger(globalValues, j);
                                            }
                                            i += 1;
                                        }
                                        output = buildList(ec, valueArr, true, -1);
                                        break;
                                    case 37:
                                        output = stringUtil_trim(fp.ctx, true, true);
                                        break;
                                    case 38:
                                        output = stringUtil_changeCase(fp.ctx, true);
                                        break;
                                    default:
                                        frame.pc = pc;
                                        frame.valueStackSize = valueStackSize;
                                        task.stack = frame;
                                        return ExRes_HardCrash(task, "Corrupted method pointer.");
                                }
                                valueStack[valueStackSize] = output;
                                valueStackSize += 1;
                                break;
                            default:
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                return ExRes_HardCrash(task, "support for this function pointer type is not done yet.");
                        }
                        if (doInvoke)
                        {
                            frame.pc = pc;
                            frame.valueStackSize = valueStackSize;
                            task.stack = frame;
                            fn = fp.func;
                            nextFrame = task.framePool;
                            if (nextFrame != null)
                            {
                                task.framePool = nextFrame.previous;
                            }
                            else
                            {
                                nextFrame = new StackFrame(null, 0, null, 0, 0, 0, null, null, false, null, false, null);
                            }
                            nextFrame.previous = frame;
                            nextFrame.pc = fn.pc - 1;
                            nextFrame.args = args;
                            nextFrame.argc = argc;
                            nextFrame.valueStackSize = valueStackSize;
                            nextFrame.valueStackBaseSize = valueStackSize;
                            nextFrame.locals = new Dictionary<string, Value>();
                            nextFrame.context = value;
                            nextFrame.useContextAsReturnValue = overrideReturnValueWithContext;
                            nextFrame.bubblingValue = null;
                            nextFrame.functionInfo = fn;
                            frame = nextFrame;
                            task.stack = frame;
                            pc = frame.pc;
                            locals = frame.locals;
                        }
                        break;
                    case 22:
                        // OP_INDEX;
                        valueStackSize -= 1;
                        left = valueStack[valueStackSize - 1];
                        right = valueStack[valueStackSize];
                        switch (left.type * 16 + right.type)
                        {
                            case 83:
                                stringImpl1 = (StringImpl)left.internalValue;
                                if (stringImpl1.isBuilder)
                                {
                                    stringFlatten(stringImpl1);
                                }
                                sz = stringImpl1.length;
                                i = (int)right.internalValue;
                                bool1 = false;
                                if (i < 0)
                                {
                                    i += sz;
                                    if (i < 0)
                                    {
                                        bool1 = true;
                                    }
                                }
                                else if (i >= sz)
                                {
                                    bool1 = true;
                                }
                                if (bool1)
                                {
                                    errorId = 8;
                                    errorMsg = "String index out of range.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                intArray1 = new int[1];
                                intArray1[0] = stringImpl1.uChars[i];
                                str1 = stringImpl1.nativeStr.Substring(i, 1);
                                valueStack[valueStackSize - 1] = new Value(5, new StringImpl(1, false, intArray1, str1, null, null));
                                break;
                            case 147:
                                listImpl1 = (ListImpl)left.internalValue;
                                i = (int)right.internalValue;
                                sz = listImpl1.length;
                                bool1 = false;
                                if (i < 0)
                                {
                                    i += sz;
                                    if (i < 0)
                                    {
                                        bool1 = true;
                                    }
                                }
                                else if (i >= sz)
                                {
                                    bool1 = true;
                                }
                                if (bool1)
                                {
                                    errorId = 8;
                                    errorMsg = "List index out of range.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                valueStack[valueStackSize - 1] = listImpl1.items[i];
                                break;
                            case 163:
                                dictImpl1 = (DictImpl)left.internalValue;
                                i = (int)right.internalValue;
                                if (dictImpl1.keyType == 3)
                                {
                                    if (dictImpl1.intKeyLookup.ContainsKey(i))
                                    {
                                        bool1 = false;
                                        valueStack[valueStackSize - 1] = dictImpl1.values[dictImpl1.intKeyLookup[i]];
                                    }
                                    else
                                    {
                                        bool1 = true;
                                    }
                                }
                                else
                                {
                                    bool1 = true;
                                }
                                if (bool1)
                                {
                                    errorId = 6;
                                    errorMsg = "Dictionary does not have that key";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                break;
                            case 165:
                                dictImpl1 = (DictImpl)left.internalValue;
                                stringImpl1 = (StringImpl)right.internalValue;
                                if (stringImpl1.isBuilder)
                                {
                                    stringFlatten(stringImpl1);
                                }
                                str1 = stringImpl1.nativeStr;
                                if (dictImpl1.keyType == 5)
                                {
                                    if (dictImpl1.strKeyLookup.ContainsKey(str1))
                                    {
                                        bool1 = false;
                                        valueStack[valueStackSize - 1] = dictImpl1.values[dictImpl1.strKeyLookup[str1]];
                                    }
                                    else
                                    {
                                        bool1 = true;
                                    }
                                }
                                else
                                {
                                    bool1 = true;
                                }
                                if (bool1)
                                {
                                    errorId = 6;
                                    errorMsg = "Dictionary does not have that key";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                break;
                            default:
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                switch (left.type)
                                {
                                    case 5:
                                        errorMsg = "Strings must be indexed with an integer.";
                                        break;
                                    case 9:
                                        errorMsg = "Lists must be indexed with an integer.";
                                        break;
                                    default:
                                        errorMsg = "Cannot index into this type";
                                        break;
                                }
                                errorId = 4;
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 23:
                        // OP_INT_INCR;
                        i = valueStackSize - 1;
                        value = valueStack[i];
                        if (value.type != 3)
                        {
                            errorId = 9;
                            errorMsg = "Cannot increment/decrement non-integer";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        valueStack[i] = buildInteger(globalValues, row.firstArg + (int)value.internalValue);
                        break;
                    case 24:
                        // OP_JUMP;
                        pc += row.firstArg;
                        break;
                    case 25:
                        // OP_MATH_FLOOR;
                        value = valueStack[valueStackSize - 1];
                        if (value.type == 4)
                        {
                            float1 = (double)value.internalValue;
                            int1 = (int) System.Math.Floor(float1);
                            if (int1 > float1)
                            {
                                int1 -= 1;
                            }
                            valueStack[valueStackSize - 1] = buildInteger(globalValues, int1);
                        }
                        else if (value.type == 3)
                        {
                        }
                        else
                        {
                            errorId = 4;
                            errorMsg = "floor can only take in a numeric value.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 26:
                        // OP_NEGATIVE_SIGN;
                        value = valueStack[valueStackSize - 1];
                        switch (value.type)
                        {
                            case 3:
                                i = -(int)value.internalValue;
                                if (i < 1200 && -i > 1200)
                                {
                                    if (i < 0)
                                    {
                                        value = globalValues.posIntegers[i];
                                    }
                                    else
                                    {
                                        value = globalValues.posIntegers[i];
                                    }
                                }
                                else
                                {
                                    value = new Value(3, i);
                                }
                                break;
                            case 4:
                                float1 = (double)value.internalValue;
                                if (float1 != 0)
                                {
                                    value = new Value(4, -float1);
                                }
                                break;
                            default:
                                errorId = 4;
                                errorMsg = "Cannot apply a negative sign to this type.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        valueStack[valueStackSize - 1] = value;
                        break;
                    case 27:
                        // OP_POP;
                        valueStackSize -= 1;
                        break;
                    case 28:
                        // OP_POP_AND_JUMP_IF_FALSE;
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        if (!(bool)value.internalValue)
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 29:
                        // OP_POP_AND_JUMP_IF_TRUE;
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        if ((bool)value.internalValue)
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 30:
                        // OP_POP_IF_FALSE_OR_JUMP;
                        value = valueStack[valueStackSize - 1];
                        if (value.type == 2 && !(bool)value.internalValue)
                        {
                            valueStackSize -= 1;
                        }
                        else
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 31:
                        // OP_POP_IF_NULL_OR_JUMP;
                        value = valueStack[valueStackSize - 1];
                        if (value.type == 1)
                        {
                            valueStackSize -= 1;
                        }
                        else
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 32:
                        // OP_POP_IF_TRUE_OR_JUMP;
                        value = valueStack[valueStackSize - 1];
                        if (value.type == 2 && (bool)value.internalValue)
                        {
                            valueStackSize -= 1;
                        }
                        else
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 33:
                        // OP_PUSH_ARG;
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = frame.args[row.firstArg];
                        valueStackSize += 1;
                        break;
                    case 34:
                        // OP_PUSH_ARG_IF_PRESENT;
                        if (row.firstArg < frame.argc)
                        {
                            if (valueStackSize == valueStackCapacity)
                            {
                                valueStack = increaseValueStackCapacity(task);
                                valueStackCapacity = valueStack.Length;
                            }
                            valueStack[valueStackSize] = frame.args[row.firstArg];
                            valueStackSize += 1;
                            pc += row.secondArg;
                        }
                        break;
                    case 35:
                        // OP_PUSH_BASE_CTOR;
                        value = ec.classes[row.firstArg].ctor;
                        fp = FunctionPointer_cloneWithNewType((FunctionPointer)value.internalValue, 6);
                        row.valueCache = new Value(11, fp);
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 36:
                        // OP_PUSH_BOOL;
                        row.valueCache = globalValues.falseValue;
                        if (row.firstArg == 1)
                        {
                            row.valueCache = globalValues.trueValue;
                        }
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 37:
                        // OP_PUSH_CLASS_REF;
                        classDef = ec.classes[row.firstArg];
                        if (classDef.staticInitialized)
                        {
                            row.valueCache = classDef.classRef;
                            row.op = 44;
                            pc -= 1;
                        }
                        else
                        {
                            classDef.staticInitialized = true;
                            if (classDef.staticCtorFuncId > 0)
                            {
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                fn = ec.functions[classDef.staticCtorFuncId];
                                pc = fn.pc - 1;
                                frame = new StackFrame(frame, pc, new Value[0], 0, valueStackSize, valueStackSize, new Dictionary<string, Value>(), classDef.classRef, true, null, false, fn);
                                locals = frame.locals;
                                task.stack = frame;
                            }
                            else
                            {
                                pc -= 1;
                            }
                        }
                        break;
                    case 38:
                        // OP_PUSH_FLOAT;
                        if (row.args.Length == 1)
                        {
                            i = row.firstArg;
                            if (i >= 0 && i <= 4)
                            {
                                value = globalValues.floatsBy4x[i];
                            }
                            else
                            {
                                value = buildFloat(i * 0.25);
                            }
                        }
                        else
                        {
                            value = buildFloat(double.Parse(row.stringArg));
                        }
                        row.valueCache = value;
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 39:
                        // OP_PUSH_FUNC_PTR;
                        row.valueCache = ec.functionsAsValues[row.firstArg];
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 40:
                        // OP_PUSH_INT;
                        row.valueCache = buildInteger(globalValues, row.firstArg);
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 63:
                        // OP_PUSH_LAMBDA;
                        value = ec.statelessLambdasAsValues[row.firstArg];
                        if (value == null)
                        {
                            frame.pc = pc;
                            frame.valueStackSize = valueStackSize;
                            task.stack = frame;
                            return ExRes_HardCrash(task, "TODO: lambda closures");
                        }
                        else
                        {
                            row.valueCache = value;
                            row.op = 44;
                            pc -= 1;
                        }
                        break;
                    case 41:
                        // OP_PUSH_NULL;
                        row.valueCache = globalValues.nullValue;
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 42:
                        // OP_PUSH_STRING;
                        row.valueCache = buildString(globalValues, row.stringArg, true);
                        row.op = 44;
                        pc -= 1;
                        break;
                    case 43:
                        // OP_PUSH_THIS;
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = frame.context;
                        valueStackSize += 1;
                        break;
                    case 44:
                        // OP_PUSH_VALUE;
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = row.valueCache;
                        valueStackSize += 1;
                        break;
                    case 45:
                        // OP_PUSH_VAR;
                        name = row.stringArg;
                        if (locals.ContainsKey(name))
                        {
                            if (valueStackSize == valueStackCapacity)
                            {
                                valueStack = increaseValueStackCapacity(task);
                                valueStackCapacity = valueStack.Length;
                            }
                            valueStack[valueStackSize] = locals[name];
                            valueStackSize += 1;
                        }
                        else
                        {
                            errorId = 1;
                            errorMsg = string.Join("", new string[] { "The variable '", row.stringArg, "' has not been assigned a value." });
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        break;
                    case 46:
                        // OP_RETURN;
                        if (ec.tryDescriptors[pc] != null)
                        {
                            frame.pc = pc;
                            frame.valueStackSize = valueStackSize;
                            task.stack = frame;
                            return ExRes_HardCrash(task, "TODO: returns from within try/catch/finally blocks");
                        }
                        if (frame.useContextAsReturnValue)
                        {
                            valueStack[valueStackSize - 1] = frame.context;
                        }
                        nextFrame = frame.previous;
                        frame.previous = task.framePool;
                        task.framePool = frame;
                        frame.context = null;
                        frame = nextFrame;
                        task.stack = frame;
                        if (frame == null)
                        {
                            return ExRes_Done(task);
                        }
                        locals = frame.locals;
                        pc = frame.pc;
                        break;
                    case 47:
                        // OP_SLICE;
                        i = row.firstArg;
                        valueStackSize -= 1;
                        if ((i & 4) > 0)
                        {
                            int3 = (int)valueStack[valueStackSize].internalValue;
                            if (int3 == 0)
                            {
                                errorId = 4;
                                errorMsg = "Cannot use 0 as a step distance for a slice.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                            }
                            valueStackSize -= 1;
                        }
                        else
                        {
                            int3 = 1;
                        }
                        if ((i & 2) > 0)
                        {
                            int2 = (int)valueStack[valueStackSize].internalValue;
                            valueStackSize -= 1;
                            bool2 = true;
                        }
                        else
                        {
                            bool2 = false;
                        }
                        if ((i & 1) > 0)
                        {
                            int1 = (int)valueStack[valueStackSize].internalValue;
                            valueStackSize -= 1;
                            bool1 = true;
                        }
                        else
                        {
                            bool1 = false;
                        }
                        value = valueStack[valueStackSize];
                        if (value.type == 5)
                        {
                            bool3 = true;
                            stringImpl1 = (StringImpl)value.internalValue;
                            if (stringImpl1.isBuilder)
                            {
                                stringFlatten(stringImpl1);
                            }
                            sz = stringImpl1.length;
                        }
                        else if (value.type == 9)
                        {
                            bool3 = false;
                            listImpl1 = (ListImpl)value.internalValue;
                            sz = listImpl1.length;
                        }
                        else
                        {
                            errorId = 4;
                            errorMsg = "Slicing can only be performed on strings or lists.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        if (bool1)
                        {
                            if (int1 < 0)
                            {
                                int1 += sz;
                            }
                            if (int1 < 0 || int1 >= sz)
                            {
                                errorId = 8;
                                errorMsg = "Start index is out of range.";
                                return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                            }
                        }
                        if (bool2 && int2 < 0)
                        {
                            int2 += sz;
                        }
                        if (bool1)
                        {
                            if (bool2)
                            {
                                if (int3 > 0)
                                {
                                    if (int1 <= int2)
                                    {
                                        leftType = int1;
                                        rightType = int2;
                                        if (rightType > sz)
                                        {
                                            rightType = sz;
                                        }
                                    }
                                    else
                                    {
                                        leftType = 0;
                                        rightType = 0;
                                    }
                                }
                                else if (int1 <= int2)
                                {
                                    leftType = 0;
                                    rightType = 0;
                                }
                                else
                                {
                                    leftType = int1;
                                    rightType = int2;
                                    if (rightType < -1)
                                    {
                                        rightType = -1;
                                    }
                                }
                            }
                            else if (int3 > 0)
                            {
                                leftType = int1;
                                rightType = sz;
                            }
                            else
                            {
                                leftType = int1;
                                rightType = -1;
                            }
                        }
                        else if (bool2)
                        {
                            if (int3 > 0)
                            {
                                leftType = 0;
                                rightType = int2;
                                if (rightType > sz)
                                {
                                    rightType = sz;
                                }
                            }
                            else
                            {
                                leftType = sz - 1;
                                rightType = int2;
                                if (rightType < -1)
                                {
                                    rightType = -1;
                                }
                            }
                        }
                        else if (int3 > 0)
                        {
                            leftType = 0;
                            rightType = sz;
                        }
                        else
                        {
                            leftType = sz - 1;
                            rightType = -1;
                        }
                        if (bool3)
                        {
                            intList = new List<int>();
                            intArray1 = stringImpl1.uChars;
                            if (int3 > 0)
                            {
                                i = leftType;
                                while (i < rightType)
                                {
                                    intList.Add(intArray1[i]);
                                    i += int3;
                                }
                            }
                            else
                            {
                                i = leftType;
                                while (i > rightType)
                                {
                                    intList.Add(intArray1[i]);
                                    i += int3;
                                }
                            }
                            output = createStringFromUnicodeArray(globalValues, intList.ToArray(), false);
                        }
                        else
                        {
                            valueList = new List<Value>();
                            valueArr = listImpl1.items;
                            if (int3 > 0)
                            {
                                i = leftType;
                                while (i < rightType)
                                {
                                    valueList.Add(valueArr[i]);
                                    i += int3;
                                }
                            }
                            else
                            {
                                i = leftType;
                                while (i > rightType)
                                {
                                    valueList.Add(valueArr[i]);
                                    i += int3;
                                }
                            }
                            sz = valueList.Count;
                            output = new Value(9, new ListImpl(ec.nextRefId, sz, sz, valueList.ToArray()));
                        }
                        valueStack[valueStackSize] = output;
                        valueStackSize += 1;
                        break;
                    case 48:
                        // OP_SPECIAL_ACTION;
                        output = globalValues.nullValue;
                        switch (row.firstArg)
                        {
                            case 16:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                if (value.type != 9)
                                {
                                    errorId = 4;
                                    errorMsg = "List expected";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                intArray1 = convertListToByteArray((ListImpl)value.internalValue);
                                if (intArray1 == null)
                                {
                                    errorId = 4;
                                    errorMsg = "Byte list includes non-byte values";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                bool1 = (bool)valueStack[valueStackSize + 1].internalValue;
                                str1 = buildBase64String(intArray1);
                                if (bool1)
                                {
                                    str1 = str1.Replace("+", "-").Replace("/", "_");
                                }
                                output = buildString(globalValues, str1, false);
                                break;
                            case 17:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type != 5)
                                {
                                    errorId = 4;
                                    errorMsg = "String expected";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                intArray1 = System.Convert.FromBase64String(stringUtil_getFlatValue(value)).Select(b => (int)b).ToArray();
                                if (intArray1 == null)
                                {
                                    errorId = 4;
                                    errorMsg = "String contains non-base64 characters.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                output = buildIntegerListValue(ec, intArray1);
                                break;
                            case 6:
                                valueStackSize -= 2;
                                left = valueStack[valueStackSize];
                                right = valueStack[valueStackSize + 1];
                                leftType = left.type;
                                rightType = right.type;
                                if (leftType != rightType)
                                {
                                    if (leftType == 3 && rightType == 4)
                                    {
                                        float1 = (int)left.internalValue - (double)right.internalValue;
                                    }
                                    else if (leftType == 4 && rightType == 3)
                                    {
                                        float1 = (double)left.internalValue - (int)right.internalValue;
                                    }
                                    else
                                    {
                                        float1 = 0.0 + leftType - rightType;
                                    }
                                }
                                else if (leftType == 3)
                                {
                                    float1 = 0.0 + (int)left.internalValue - (int)right.internalValue;
                                }
                                else if (leftType == 4)
                                {
                                    float1 = (double)left.internalValue - (double)right.internalValue;
                                }
                                else if (leftType == 5)
                                {
                                    stringImpl1 = (StringImpl)left.internalValue;
                                    stringImpl2 = (StringImpl)right.internalValue;
                                    if (stringImpl1.isBuilder)
                                    {
                                        stringFlatten(stringImpl1);
                                    }
                                    if (stringImpl2.isBuilder)
                                    {
                                        stringFlatten(stringImpl2);
                                    }
                                    sz = stringImpl1.length;
                                    int2 = stringImpl2.length;
                                    if (int2 < sz)
                                    {
                                        sz = int2;
                                    }
                                    int3 = 0;
                                    i = 0;
                                    while (i < sz)
                                    {
                                        int1 = stringImpl1.uChars[i];
                                        int2 = stringImpl2.uChars[i];
                                        if (int1 != int2)
                                        {
                                            int3 = int1 - int2;
                                            i += sz;
                                        }
                                        i += 1;
                                    }
                                    if (int3 == 0)
                                    {
                                        int3 = stringImpl2.length - stringImpl1.length;
                                    }
                                    float1 = 0.0 + int3;
                                }
                                else
                                {
                                    float1 = 0.0;
                                }
                                if (float1 == 0)
                                {
                                    output = globalValues.intZero;
                                }
                                else if (float1 < 0)
                                {
                                    output = globalValues.negIntegers[1];
                                }
                                else
                                {
                                    output = globalValues.intOne;
                                }
                                break;
                            case 20:
                                valueStackSize -= 2;
                                str1 = stringUtil_getFlatValue(valueStack[valueStackSize]);
                                valueArr = ((ListImpl)valueStack[valueStackSize + 1].internalValue).items;
                                output = json_util_parse(ec, str1, intBuffer16);
                                i = 0;
                                while (i < 3)
                                {
                                    valueArr[i] = buildInteger(globalValues, intBuffer16[i]);
                                    i += 1;
                                }
                                break;
                            case 21:
                                valueStackSize -= 3;
                                str1 = json_util_serialize(globalValues, valueStack[valueStackSize], (bool)valueStack[valueStackSize + 1].internalValue, (ListImpl)valueStack[valueStackSize + 2].internalValue);
                                output = buildString(globalValues, str1, false);
                                break;
                            case 8:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                output = buildFloat(System.Math.Acos(float1));
                                break;
                            case 9:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                output = buildFloat(System.Math.Asin(float1));
                                break;
                            case 10:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                right = valueStack[valueStackSize + 1];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                if (right.type == 1)
                                {
                                    float2 = 1.0;
                                }
                                else if (right.type == 4)
                                {
                                    float2 = (double)right.internalValue;
                                }
                                else
                                {
                                    float2 = 0.0 + (int)right.internalValue;
                                }
                                output = buildFloat(System.Math.Atan2(float1, float2));
                                break;
                            case 11:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                output = buildFloat(System.Math.Cos(float1));
                                break;
                            case 12:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                i = (int)valueStack[valueStackSize + 1].internalValue;
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                if (float1 <= 0)
                                {
                                    output = VALUE_NULL;
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        bool1 = true;
                                        float2 = 10.0;
                                    }
                                    else if (i > 0)
                                    {
                                        bool1 = true;
                                        float2 = 2.0;
                                    }
                                    else
                                    {
                                        bool1 = false;
                                        float2 = 2.718281828459;
                                    }
                                    float3 = System.Math.Log(float1) / System.Math.Log(float2);
                                    if (bool1)
                                    {
                                        int1 = (int)(float1 + 0.0000000000015);
                                        if ((int)float1 != int1 && System.Math.Pow(float2, int1) == float1)
                                        {
                                            float3 = 0.0 + int1;
                                        }
                                    }
                                    output = buildFloat(float3);
                                }
                                break;
                            case 13:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                output = buildFloat(System.Math.Sin(float1));
                                break;
                            case 14:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                if (value.type == 4)
                                {
                                    float1 = (double)value.internalValue;
                                }
                                else
                                {
                                    float1 = 0.0 + (int)value.internalValue;
                                }
                                output = buildFloat(System.Math.Tan(float1));
                                break;
                            case 15:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                output = VALUE_NULL;
                                if (value.type == 5)
                                {
                                    PST_ParseFloat(stringUtil_getFlatValue(value), floatBuffer16);
                                    if (floatBuffer16[0] > 0)
                                    {
                                        int1 = (int) System.Math.Floor(floatBuffer16[1]);
                                        output = buildInteger(globalValues, int1);
                                    }
                                }
                                break;
                            case 25:
                                valueStackSize -= 1;
                                value = valueStack[valueStackSize];
                                output = VALUE_NULL;
                                if (value.type == 5)
                                {
                                    PST_ParseFloat(stringUtil_getFlatValue(value), floatBuffer16);
                                    output = VALUE_NULL;
                                    if (floatBuffer16[0] > 0)
                                    {
                                        float1 = floatBuffer16[1];
                                        if (float1 == 0.0)
                                        {
                                            output = globalValues.floatsBy4x[0];
                                        }
                                        else if (float1 == 1.0)
                                        {
                                            output = globalValues.floatsBy4x[4];
                                        }
                                        else if (float1 == 0.5)
                                        {
                                            output = globalValues.floatsBy4x[2];
                                        }
                                        else
                                        {
                                            output = buildFloat(float1);
                                        }
                                    }
                                }
                                break;
                            case 7:
                                output = buildFloat(PST_Random.NextDouble());
                                break;
                            case 27:
                                // res_list;
                                valueStackSize -= 1;
                                resourceLookup = (System.Collections.Generic.Dictionary<string, EmbeddedResource>)valueStack[valueStackSize].internalValue;
                                output = buildListFromStringArray(ec, resourceLookup.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray());
                                break;
                            case 28:
                                str1 = frame.previous.functionInfo.moduleId;
                                if (ec.embeddedResources.ContainsKey(str1))
                                {
                                    output = new Value(14, ec.embeddedResources[str1]);
                                }
                                else
                                {
                                    output = new Value(14, new Dictionary<string, EmbeddedResource>());
                                }
                                break;
                            case 29:
                                // res_read;
                                valueStackSize -= 4;
                                resourceLookup = (System.Collections.Generic.Dictionary<string, EmbeddedResource>)valueStack[valueStackSize].internalValue;
                                str1 = stringUtil_getFlatValue(valueStack[valueStackSize + 1]);
                                str2 = stringUtil_getFlatValue(valueStack[valueStackSize + 2]);
                                listImpl1 = (ListImpl)valueStack[valueStackSize + 3].internalValue;
                                if (!resourceLookup.ContainsKey(str1))
                                {
                                    output = buildString(globalValues, "Resource not found: " + str1, false);
                                }
                                else
                                {
                                    resource = resourceLookup[str1];
                                    if (str2 == "Y")
                                    {
                                        strOut = null;
                                        if (resource.type == 1)
                                        {
                                            strOut = "TEXT";
                                        }
                                        if (resource.type == 3)
                                        {
                                            strOut = "IMAGE";
                                        }
                                        if (resource.type == 2)
                                        {
                                            strOut = "BINARY";
                                        }
                                        List_add(listImpl1, buildString(globalValues, strOut, true));
                                        output = VALUE_NULL;
                                    }
                                    else if (str2 == "T")
                                    {
                                        if (resource.type == 1)
                                        {
                                            List_add(listImpl1, buildString(globalValues, (string)resource.payload, false));
                                            output = VALUE_NULL;
                                        }
                                        else
                                        {
                                            output = buildString(globalValues, "Resource is not a text file: " + str1, false);
                                        }
                                    }
                                    else if (str2 == "B")
                                    {
                                        if (resource.type == 2)
                                        {
                                            intArray1 = (int[])resource.payload;
                                            sz = intArray1.Length;
                                            i = 0;
                                            while (i < sz)
                                            {
                                                List_add(listImpl1, globalValues.posIntegers[intArray1[i]]);
                                                i += 1;
                                            }
                                            output = VALUE_NULL;
                                        }
                                        else if (resource.type == 3)
                                        {
                                            output = buildString(globalValues, "Resource is an image and is not available as raw binary data: " + str1, false);
                                        }
                                    }
                                    else if (str2 == "I")
                                    {
                                        output = buildString(globalValues, "NOT IMPLEMENTED: image resource loading: " + str1, false);
                                    }
                                    else
                                    {
                                        output = buildString(globalValues, "ERR", false);
                                    }
                                }
                                break;
                            case 3:
                                valueStackSize -= 1;
                                output = Sort_end((SortState)valueStack[valueStackSize].internalValue);
                                break;
                            case 4:
                                valueStackSize -= 2;
                                output = VALUE_FALSE;
                                if (Sort_getNextCmp(valueStack[valueStackSize], valueStack[valueStackSize + 1]))
                                {
                                    output = VALUE_TRUE;
                                }
                                break;
                            case 5:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                bool1 = (bool)valueStack[valueStackSize + 1].internalValue;
                                output = Sort_proceedWithCmpResult(value, bool1);
                                break;
                            case 2:
                                valueStackSize -= 2;
                                output = Sort_start(valueStack[valueStackSize], valueStack[valueStackSize + 1]);
                                break;
                            case 23:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                str1 = stringUtil_getFlatValue(valueStack[valueStackSize + 1]);
                                if (value.type != 9)
                                {
                                    errorId = 4;
                                    errorMsg = "List expected";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                intArray1 = convertListToByteArray((ListImpl)value.internalValue);
                                if (intArray1 == null)
                                {
                                    errorId = 4;
                                    errorMsg = "Byte list includes non-byte values";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                bool1 = true;
                                if (str1 == "utf8")
                                {
                                    str1 = System.Text.Encoding.UTF8.GetString((intArray1).Select(v => (byte)v).ToArray());
                                    if (str1 == null)
                                    {
                                        bool1 = false;
                                    }
                                }
                                else
                                {
                                    errorId = 4;
                                    errorMsg = "Encoding not implemented.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                if (bool1)
                                {
                                    output = buildString(globalValues, str1, false);
                                }
                                else
                                {
                                    output = VALUE_NULL;
                                }
                                str1 = null;
                                break;
                            case 22:
                                valueStackSize -= 1;
                                str1 = stringUtil_getFlatValue(valueStack[valueStackSize]);
                                if (str1 == "utf8")
                                {
                                    output = VALUE_TRUE;
                                }
                                else
                                {
                                    output = VALUE_FALSE;
                                }
                                break;
                            case 24:
                                valueStackSize -= 2;
                                str1 = stringUtil_getFlatValue(valueStack[valueStackSize]);
                                str2 = stringUtil_getFlatValue(valueStack[valueStackSize + 1]);
                                if (str2 == "utf8")
                                {
                                    intArray1 = PST_stringToUtf8Bytes(str1);
                                }
                                else
                                {
                                    errorId = 4;
                                    errorMsg = "Encoding not implemented.";
                                    return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                                }
                                output = buildIntegerListValue(ec, intArray1);
                                intArray1 = null;
                                str1 = null;
                                break;
                            case 1:
                                float1 = PST_CurrentTime;
                                if (row.secondArg == 1)
                                {
                                    output = buildFloat(float1);
                                }
                                else
                                {
                                    output = buildInteger(globalValues, (int) System.Math.Floor(float1));
                                }
                                break;
                            case 26:
                                valueStackSize -= 2;
                                value = valueStack[valueStackSize];
                                stringUtil_getFlatValue(value);
                                intArray1 = ((StringImpl)value.internalValue).uChars;
                                listImpl1 = (ListImpl)valueStack[valueStackSize + 1].internalValue;
                                xmlUtil_parse(ec, intArray1, listImpl1.items);
                                output = VALUE_NULL;
                                break;
                            default:
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                return ExRes_HardCrash(task, "ACTION NOT IMPLEMENTED");
                        }
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = output;
                        valueStackSize += 1;
                        break;
                    case 49:
                        // OP_STACK_DO_SI_DO_4A;
                        i = valueStackSize - 4;
                        j = i + 2;
                        value = valueStack[i];
                        valueStack[i] = valueStack[j];
                        valueStack[j] = value;
                        value = valueStack[i + 1];
                        valueStack[i + 1] = valueStack[j + 1];
                        valueStack[j + 1] = value;
                        break;
                    case 50:
                        // OP_STACK_DO_SI_DUP_1;
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        i = valueStackSize - 2;
                        valueStackSize += 1;
                        valueStack[i + 2] = valueStack[i];
                        valueStack[i] = valueStack[i + 1];
                        break;
                    case 51:
                        // OP_STACK_DO_SI_DUP_2;
                        i = valueStackSize - 3;
                        value = valueStack[i];
                        valueStack[i] = valueStack[i + 1];
                        valueStack[i + 1] = valueStack[i + 2];
                        valueStack[i + 2] = value;
                        break;
                    case 52:
                        // OP_STACK_DUPLICATE;
                        if (valueStackSize == valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = valueStack[valueStackSize - 1];
                        valueStackSize += 1;
                        break;
                    case 53:
                        // OP_STACK_DUPLICATE_2;
                        if (valueStackSize + 2 >= valueStackCapacity)
                        {
                            valueStack = increaseValueStackCapacity(task);
                            valueStackCapacity = valueStack.Length;
                        }
                        valueStack[valueStackSize] = valueStack[valueStackSize - 2];
                        valueStack[valueStackSize + 1] = valueStack[valueStackSize - 1];
                        valueStackSize += 2;
                        break;
                    case 54:
                        // OP_SWITCH_ADD;
                        if (row.firstArg < 0)
                        {
                            i = 1;
                            while (i < row.args.Length)
                            {
                                switchIntLookup[row.args[i + 1]] = row.args[i];
                                i += 2;
                            }
                        }
                        else
                        {
                            switchStrLookup[row.stringArg] = row.firstArg;
                        }
                        break;
                    case 55:
                        // OP_SWITCH_BUILD;
                        if (row.secondArg == 1)
                        {
                            switchIntLookup = new Dictionary<int, int>();
                            switchStrLookup = null;
                        }
                        else
                        {
                            switchIntLookup = null;
                            switchStrLookup = new Dictionary<string, int>();
                        }
                        break;
                    case 56:
                        // OP_SWITCH_FINALIZE;
                        pc -= row.args[0];
                        row = byteCode[pc];
                        if (switchIntLookup != null)
                        {
                            ec.switchIntLookupsByPc[pc] = switchIntLookup;
                            row.op = 57;
                        }
                        else
                        {
                            ec.switchStrLookupsByPc[pc] = switchStrLookup;
                            row.op = 58;
                        }
                        pc -= 1;
                        break;
                    case 57:
                        // OP_SWITCH_INT;
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        i = (int)value.internalValue;
                        switchIntLookup = ec.switchIntLookupsByPc[pc];
                        if (switchIntLookup.ContainsKey(i))
                        {
                            pc += switchIntLookup[i];
                        }
                        else
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 58:
                        // OP_SWITCH_STRING;
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        stringImpl1 = (StringImpl)value.internalValue;
                        if (stringImpl1.isBuilder)
                        {
                            stringFlatten(stringImpl1);
                        }
                        str1 = stringImpl1.nativeStr;
                        switchStrLookup = ec.switchStrLookupsByPc[pc];
                        if (switchStrLookup.ContainsKey(str1))
                        {
                            pc += switchStrLookup[str1];
                        }
                        else
                        {
                            pc += row.firstArg;
                        }
                        break;
                    case 59:
                        // OP_THROW;
                        valueStackSize -= 1;
                        value = valueStack[valueStackSize];
                        bool1 = false;
                        if (value.type == 12)
                        {
                            instance1 = (Instance)value.internalValue;
                            int1 = 0;
                            i = 1;
                            while (i < ec.classes.Length && int1 == 0)
                            {
                                if (ec.classes[i].name == "Exception")
                                {
                                    int1 = i;
                                }
                                i += 1;
                            }
                            classDef = instance1.classDef;
                            while (classDef != null)
                            {
                                if (classDef.id == int1)
                                {
                                    bool1 = true;
                                }
                                classDef = classDef.parent;
                            }
                        }
                        if (!bool1)
                        {
                            errorId = 4;
                            errorMsg = "Only Exception instances can be thrown.";
                            return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
                        }
                        frame.pc = pc;
                        frame.valueStackSize = valueStackSize;
                        task.stack = frame;
                        instance1.members[instance1.classDef.nameToOffset["trace"]] = generateStackTrace(task);
                        if (bubbleException(task, value))
                        {
                            return ExRes_HardCrash(task, getExceptionMessage(value, true));
                        }
                        frame = task.stack;
                        pc = frame.pc;
                        locals = frame.locals;
                        break;
                    case 61:
                        // OP_TRY_CATCH_ROUTER;
                        value = frame.bubblingValue;
                        frame.bubblingValueUncaught = exceptionCatcherRouteException(value, row.args, intBuffer16);
                        pc += intBuffer16[0];
                        if (!frame.bubblingValueUncaught)
                        {
                            if (valueStackSize == valueStackCapacity)
                            {
                                valueStack = increaseValueStackCapacity(task);
                                valueStackCapacity = valueStack.Length;
                            }
                            valueStack[valueStackSize] = value;
                            valueStackSize += 1;
                            frame.bubblingValue = null;
                        }
                        break;
                    case 60:
                        // OP_TRY_FINALLY_END;
                        if (frame.bubblingValue != null)
                        {
                            value = frame.bubblingValue;
                            if (frame.bubblingValueUncaught)
                            {
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                task.stack = frame;
                                if (bubbleException(task, value))
                                {
                                    return ExRes_HardCrash(task, getExceptionMessage(value, true));
                                }
                                frame = task.stack;
                                pc = frame.pc;
                                locals = frame.locals;
                            }
                        }
                        break;
                    case 65:
                        // OP_TYPEOF;
                        switch (valueStack[valueStackSize - 1].type)
                        {
                            case 1:
                                str1 = "null";
                                break;
                            case 3:
                                str1 = "int";
                                break;
                            case 4:
                                str1 = "float";
                                break;
                            case 2:
                                str1 = "bool";
                                break;
                            case 5:
                                str1 = "string";
                                break;
                            case 11:
                                str1 = "function";
                                break;
                            case 9:
                                str1 = "list";
                                break;
                            case 10:
                                str1 = "dict";
                                break;
                            case 12:
                                str1 = "object";
                                break;
                            case 14:
                                str1 = "native";
                                break;
                            default:
                                frame.pc = pc;
                                frame.valueStackSize = valueStackSize;
                                task.stack = frame;
                                return ExRes_HardCrash(task, "Unknown type?");
                        }
                        valueStack[valueStackSize - 1] = buildString(globalValues, str1, true);
                        break;
                    default:
                        frame.pc = pc;
                        frame.valueStackSize = valueStackSize;
                        task.stack = frame;
                        return ExRes_HardCrash(task, "INVALID OP CODE: " + row.op.ToString());
                }
                pc += 1;
            }
        }

        public static SortTask Sort_buildTaskList(int start, int length, Value[] items, Value[] output, System.Collections.Generic.List<SortTask> tasks)
        {
            if (length == 1)
            {
                SortNode node = new SortNode(items[start], output[start], null);
                SortTask task = new SortTask(true, null, null, node, node, null, false, null);
                tasks.Add(task);
                return task;
            }
            if (length == 2)
            {
                SortNode left = new SortNode(items[start], output[start], null);
                SortNode right = new SortNode(items[start + 1], output[start + 1], null);
                SortTask task2 = new SortTask(false, left, right, null, null, null, false, null);
                tasks.Add(task2);
                return task2;
            }
            int half = length >> 1;
            SortTask leftTask = Sort_buildTaskList(start, half, items, output, tasks);
            SortTask rightTask = Sort_buildTaskList(start + half, length - half, items, output, tasks);
            SortTask taskMerge = new SortTask(false, null, null, null, null, null, false, null);
            leftTask.feedsTo = taskMerge;
            leftTask.feedsToLeft = true;
            rightTask.feedsTo = taskMerge;
            tasks.Add(taskMerge);
            return taskMerge;
        }

        public static Value Sort_end(SortState state)
        {
            Value arr = state.copyBackBuffer;
            Value[] items = ((ListImpl)arr.internalValue).items;
            SortNode walker = state.output;
            int i = 0;
            while (walker != null)
            {
                items[i] = walker.outputValue;
                walker = walker.next;
                i += 1;
            }
            return arr;
        }

        public static bool Sort_getNextCmp(Value sortStateValue, Value pairValue)
        {
            Value[] pair = ((ListImpl)pairValue.internalValue).items;
            SortState o = (SortState)sortStateValue.internalValue;
            SortTask task = o.taskQueue;
            bool keepRunning = true;
            while (keepRunning)
            {
                if (task.isMerged)
                {
                    if (task.feedsTo == null)
                    {
                        o.output = task.mergedHead;
                        keepRunning = false;
                        return false;
                    }
                    if (task.feedsToLeft)
                    {
                        task.feedsTo.left = task.mergedHead;
                    }
                    else
                    {
                        task.feedsTo.right = task.mergedHead;
                    }
                    task = task.next;
                    o.taskQueue = task;
                    keepRunning = task != null;
                }
                else
                {
                    if (task.left != null && task.right != null)
                    {
                        pair[0] = task.left.value;
                        pair[1] = task.right.value;
                        return true;
                    }
                    if (task.left == null && task.right == null)
                    {
                        task.isMerged = true;
                    }
                    else
                    {
                        SortNode mergeWinner = null;
                        if (task.left == null)
                        {
                            mergeWinner = task.right;
                            task.right = mergeWinner.next;
                            mergeWinner.next = null;
                        }
                        else
                        {
                            mergeWinner = task.left;
                            task.left = mergeWinner.next;
                            mergeWinner.next = null;
                        }
                        if (task.mergedHead == null)
                        {
                            task.mergedHead = mergeWinner;
                            task.mergedTail = mergeWinner;
                        }
                        else
                        {
                            task.mergedTail.next = mergeWinner;
                            task.mergedTail = mergeWinner;
                        }
                    }
                }
            }
            return false;
        }

        public static Value Sort_proceedWithCmpResult(Value stateVal, bool isSwap)
        {
            SortState state = (SortState)stateVal.internalValue;
            SortTask task = state.taskQueue;
            SortNode winner = null;
            if (isSwap)
            {
                winner = task.right;
                task.right = winner.next;
            }
            else
            {
                winner = task.left;
                task.left = winner.next;
            }
            winner.next = null;
            if (task.mergedHead == null)
            {
                task.mergedHead = winner;
            }
            else
            {
                task.mergedTail.next = winner;
            }
            task.mergedTail = winner;
            return stateVal;
        }

        public static Value Sort_start(Value valueList, Value mirrorList)
        {
            if (mirrorList.type == 1)
            {
                mirrorList = valueList;
            }
            ListImpl values = (ListImpl)valueList.internalValue;
            int sz = values.length;
            Value[] items = values.items;
            Value[] mirror = ((ListImpl)mirrorList.internalValue).items;
            SortState o = new SortState(null, null, null, values.length < 2, null, mirrorList);
            if (o.isDone)
            {
                if (items.Length == 1)
                {
                    o.output = new SortNode(items[0], mirror[0], null);
                }
            }
            else
            {
                System.Collections.Generic.List<SortTask> tasks = new List<SortTask>();
                Sort_buildTaskList(0, sz, items, mirror, tasks);
                int i = 1;
                while (i < tasks.Count)
                {
                    tasks[i - 1].next = tasks[i];
                    i += 1;
                }
                o.taskQueue = tasks[0];
            }
            return new Value(14, o);
        }

        public static void stringFlatten(StringImpl s)
        {
            int[] ucharsBuilder = new int[s.length];
            int ucharOffset = 0;
            System.Collections.Generic.List<string> strBuilder = new List<string>();
            System.Collections.Generic.List<StringImpl> q = new List<StringImpl>();
            q.Add(s);
            StringImpl current = null;
            int i = 0;
            int currentLen = 0;
            int[] currentUChars = null;
            while (q.Count > 0)
            {
                current = PST_ListPop(q);
                if (current.isBuilder)
                {
                    q.Add(current.right);
                    q.Add(current.left);
                }
                else
                {
                    currentUChars = current.uChars;
                    currentLen = current.length;
                    i = 0;
                    while (i < currentLen)
                    {
                        ucharsBuilder[i + ucharOffset] = currentUChars[i];
                        i += 1;
                    }
                    ucharOffset += currentLen;
                    strBuilder.Add(current.nativeStr);
                }
            }
            s.isBuilder = false;
            s.left = null;
            s.right = null;
            s.nativeStr = string.Join("", strBuilder);
            s.uChars = ucharsBuilder;
        }

        public static Value stringUtil_changeCase(Value orig, bool isUpper)
        {
            bool changes = false;
            StringImpl si = (StringImpl)orig.internalValue;
            if (si.isBuilder)
            {
                stringFlatten(si);
            }
            string s = si.nativeStr;
            string s2 = null;
            if (isUpper)
            {
                s2 = s.ToUpper();
            }
            else
            {
                s2 = s.ToLower();
            }
            if (s2 == s)
            {
                return orig;
            }
            return new Value(5, buildAsciiStringImpl(s2));
        }

        public static string stringUtil_getFlatValue(Value val)
        {
            StringImpl si = (StringImpl)val.internalValue;
            if (si.isBuilder)
            {
                stringFlatten(si);
            }
            return si.nativeStr;
        }

        public static Value[] stringUtil_split(GlobalValues g, Value str, string sep)
        {
            StringImpl si = (StringImpl)str.internalValue;
            if (si.isBuilder)
            {
                stringFlatten(si);
            }
            string[] values = PST_StringSplit(si.nativeStr, sep);
            int sz = values.Length;
            Value[] o = new Value[sz];
            int i = 0;
            while (i < sz)
            {
                o[i] = buildString(g, values[i], false);
                i += 1;
            }
            return o;
        }

        public static Value stringUtil_trim(Value str, bool front, bool back)
        {
            StringImpl strimpl = (StringImpl)str.internalValue;
            int start = 0;
            int length = strimpl.length;
            if (length == 0)
            {
                return str;
            }
            if (strimpl.isBuilder)
            {
                stringFlatten(strimpl);
            }
            int c = 0;
            int[] uchars = strimpl.uChars;
            while (back && length > 0)
            {
                c = uchars[length - 1];
                switch (c)
                {
                    case 9:
                        length -= 1;
                        break;
                    case 10:
                        length -= 1;
                        break;
                    case 13:
                        length -= 1;
                        break;
                    case 32:
                        length -= 1;
                        break;
                    default:
                        back = false;
                        break;
                }
            }
            while (front && length > 0)
            {
                c = uchars[start];
                switch (c)
                {
                    case 9:
                        length -= 1;
                        start += 1;
                        break;
                    case 10:
                        length -= 1;
                        start += 1;
                        break;
                    case 13:
                        length -= 1;
                        start += 1;
                        break;
                    case 32:
                        length -= 1;
                        start += 1;
                        break;
                    default:
                        front = false;
                        break;
                }
            }
            if (length == strimpl.length)
            {
                return str;
            }
            int[] newChars = new int[length];
            int i = 0;
            while (i < length)
            {
                newChars[i] = uchars[i + start];
                i += 1;
            }
            StringImpl o = new StringImpl(length, false, newChars, strimpl.nativeStr.Substring(start, length), null, null);
            return new Value(5, o);
        }

        public static ExecutionResult ThrowError(ExecutionTask task, StackFrame frame, int pc, int valueStackSize, int errId, string msg)
        {
            frame.pc = pc;
            frame.valueStackSize = valueStackSize;
            task.stack = frame;
            return ThrowErrorImpl(task, errId, msg);
        }

        public static ExecutionResult ThrowErrorImpl(ExecutionTask task, int errId, string msg)
        {
            StackFrame frame = task.stack;
            if (frame.valueStackSize + 5 < task.valueStack.Length)
            {
                increaseValueStackCapacity(task);
            }
            ExecutionContext ec = task.execCtx;
            GlobalValues g = ec.globalValues;
            FunctionInfo throwFunc = ec.functions[ec.significantFunctions["thrw"]];
            Value[] args = new Value[2];
            args[0] = buildInteger(g, errId);
            args[1] = buildString(g, msg, false);
            task.stack = new StackFrame(frame, throwFunc.pc, args, 2, frame.valueStackSize, frame.valueStackSize, new Dictionary<string, Value>(), null, false, null, false, throwFunc);
            return new ExecutionResult(5, task, 0, null, null);
        }

        public static int tryGetNameId(System.Collections.Generic.Dictionary<string, int> names, string s)
        {
            if (names.ContainsKey(s))
            {
                return names[s];
            }
            return -1;
        }

        public static Value[] valueArrayIncreaseCapacity(Value[] oldArr)
        {
            int oldCapacity = oldArr.Length;
            int newCapacity = oldCapacity * 2;
            if (newCapacity < 8)
            {
                newCapacity = 8;
            }
            Value[] newArr = new Value[newCapacity];
            int i = 0;
            while (i < oldCapacity)
            {
                newArr[i] = oldArr[i];
                i += 1;
            }
            return newArr;
        }

        public static string valueToHumanString(Value value)
        {
            switch (value.type)
            {
                case 1:
                    return "null";
                case 2:
                    bool b = (bool)value.internalValue;
                    if (b)
                    {
                        return "true";
                    }
                    return "false";
                case 3:
                    return ((int)value.internalValue).ToString();
                case 4:
                    string floatStr = PST_FloatToString((double)value.internalValue);
                    if (!floatStr.Contains("."))
                    {
                        floatStr = floatStr + ".0";
                    }
                    return floatStr;
                case 5:
                    StringImpl strImpl = (StringImpl)value.internalValue;
                    if (strImpl.isBuilder)
                    {
                        stringFlatten(strImpl);
                    }
                    return strImpl.nativeStr;
                case 12:
                    Instance inst = (Instance)value.internalValue;
                    return string.Join("", new string[] { "Instance<", inst.classDef.name, ":", inst.id.ToString(), ">" });
                case 9:
                    ListImpl list = (ListImpl)value.internalValue;
                    return string.Join("", new string[] { "List[size=", list.length.ToString(), "]" });
                default:
                    break;
            }
            return "TODO: to string for type: " + value.type.ToString();
        }

        public static Value xml_convertCharsToString(GlobalValues globals, System.Collections.Generic.List<int> chars)
        {
            xml_performEntitySwaps(chars);
            int j = 0;
            int i = 0;
            while (i < chars.Count)
            {
                int c = chars[i];
                if (c != -1)
                {
                    chars[j] = c;
                    j += 1;
                }
                i += 1;
            }
            while (chars.Count > j)
            {
                PST_ListPop(chars);
            }
            int[] charArr = chars.ToArray();
            return createStringFromUnicodeArraySegment(globals, charArr, 0, charArr.Length);
        }

        public static bool xml_ensureMore(XmlParseContext ctx)
        {
            if (xml_hasMore(ctx))
            {
                return true;
            }
            xml_setError(ctx, "Unexpected end of XML document.");
            return false;
        }

        public static int xml_getEntity(System.Collections.Generic.List<int> chars, int start, int end)
        {
            int length = end - start;
            int i = 0;
            int digit = 0;
            if (chars[start] == 35 && length > 2)
            {
                int value = 0;
                if (chars[start + 1] == 120)
                {
                    if (length > 2 && length <= 6)
                    {
                        i = 2;
                        while (i < length)
                        {
                            digit = hexToInt(chars[start + i]);
                            if (digit == -1)
                            {
                                return -1;
                            }
                            value = value * 16 + digit;
                            i += 1;
                        }
                    }
                }
                else if (length > 1 && length <= 5)
                {
                    i = 1;
                    while (i < length)
                    {
                        digit = decToInt(chars[start + i]);
                        if (digit == -1)
                        {
                            return -1;
                        }
                        value = value * 10 + digit;
                        i += 1;
                    }
                }
                return value;
            }
            if (length > 4)
            {
                return -1;
            }
            int flatVal = 0;
            i = 0;
            while (i < 4)
            {
                flatVal = flatVal << 2;
                if (i < length)
                {
                    int c = chars[start + i];
                    if (c >= 65 && c <= 90)
                    {
                        c += 32;
                    }
                    if (c > 127)
                    {
                        return -1;
                    }
                    flatVal = flatVal | c;
                }
                i += 1;
            }
            switch (flatVal)
            {
                case 8000:
                    return 60;
                case 8128:
                    return 62;
                case 8144:
                    return 38;
                case 8188:
                    return 34;
                case 8191:
                    return 39;
            }
            return -1;
        }

        public static bool xml_hasMore(XmlParseContext ctx)
        {
            return ctx.index < ctx.length;
        }

        public static int xml_isValidNameChar(int c)
        {
            if (c < 128)
            {
                if (c >= 65 && c <= 90)
                {
                    return 1;
                }
                if (c >= 97 && c <= 122)
                {
                    return 1;
                }
                if (c == 58)
                {
                    return 1;
                }
                if (c == 45)
                {
                    return 2;
                }
                if (c == 46)
                {
                    return 2;
                }
                if (c >= 48 && c <= 57)
                {
                    return 2;
                }
            }
            else
            {
                if (c == 183)
                {
                    return 2;
                }
                if (c >= 768 && c <= 879)
                {
                    return 2;
                }
                if (c >= 8255 && c <= 8256)
                {
                    return 2;
                }
                if (c >= 192 && c <= 214)
                {
                    return 1;
                }
                if (c >= 216 && c <= 246)
                {
                    return 1;
                }
                if (c >= 248 && c <= 767)
                {
                    return 1;
                }
                if (c >= 880 && c <= 893)
                {
                    return 1;
                }
                if (c >= 895 && c <= 8191)
                {
                    return 1;
                }
                if (c >= 8204 && c <= 8205)
                {
                    return 1;
                }
                if (c >= 8304 && c <= 8591)
                {
                    return 1;
                }
                if (c >= 11264 && c <= 12271)
                {
                    return 1;
                }
                if (c >= 12289 && c <= 55295)
                {
                    return 1;
                }
                if (c >= 63744 && c <= 64975)
                {
                    return 1;
                }
                if (c >= 65008 && c <= 65533)
                {
                    return 1;
                }
                if (c >= 65536 && c <= 983039)
                {
                    return 1;
                }
            }
            return 0;
        }

        public static int xml_peekChar(XmlParseContext ctx)
        {
            if (ctx.index >= ctx.length)
            {
                return -1;
            }
            return ctx.chars[ctx.index];
        }

        public static void xml_performEntitySwaps(System.Collections.Generic.List<int> chars)
        {
            int i = 0;
            while (i < chars.Count)
            {
                if (chars[i] == 38)
                {
                    int j = 0;
                    while (j < 10)
                    {
                        if (i + j < chars.Count && chars[i + j] == 59)
                        {
                            int end = i + j;
                            int newChar = xml_getEntity(chars, i + 1, end);
                            if (newChar != -1)
                            {
                                int k = i;
                                while (k < end)
                                {
                                    chars[k] = -1;
                                    k += 1;
                                }
                                chars[end] = newChar;
                            }
                            j = 10;
                        }
                        j++;
                    }
                }
                i++;
            }
        }

        public static int xml_popChar(XmlParseContext ctx)
        {
            int val = -1;
            if (ctx.index < ctx.length)
            {
                val = ctx.chars[ctx.index];
                if (val == 10)
                {
                    ctx.line += 1;
                    ctx.col = 0;
                }
                else
                {
                    ctx.col += 1;
                }
                ctx.index += 1;
            }
            return val;
        }

        public static bool xml_popCloseTag(XmlParseContext ctx, int tagNameStart, int tagNameEnd)
        {
            if (xml_popChar(ctx) != 60)
            {
                return false;
            }
            if (xml_popChar(ctx) != 47)
            {
                return false;
            }
            int length = tagNameEnd - tagNameStart;
            int i = 0;
            while (i < length)
            {
                if (xml_popChar(ctx) != ctx.chars[tagNameStart + i])
                {
                    return false;
                }
                i += 1;
            }
            xml_skipWhitespace(ctx);
            return xml_popChar(ctx) == 62;
        }

        public static int xml_popElement(XmlParseContext ctx)
        {
            if (60 != xml_popChar(ctx))
            {
                xml_setError(ctx, "Expected '<' here");
                return 0;
            }
            xml_skipWhitespace(ctx);
            int tagNameStart = ctx.index;
            Value tagName = xml_popWord(ctx);
            int tagNameEnd = ctx.index;
            xml_skipWhitespace(ctx);
            if (ctx.hasError || !xml_ensureMore(ctx))
            {
                return 0;
            }
            if (tagName == null)
            {
                xml_setError(ctx, "Invalid use of '<' character.");
                return 0;
            }
            ctx.buffer.Add(ctx.globals.intOne);
            ctx.buffer.Add(tagName);
            int attributeSizeIndex = ctx.buffer.Count;
            ctx.buffer.Add(ctx.globals.intZero);
            int attributeCount = 0;
            while (xml_hasMore(ctx) && (xml_peekChar(ctx) != 62 && xml_peekChar(ctx) != 47))
            {
                Value attrName = xml_popWord(ctx);
                xml_skipWhitespace(ctx);
                if (ctx.hasError || !xml_ensureMore(ctx))
                {
                    return 0;
                }
                if (xml_popChar(ctx) != 61)
                {
                    xml_setError(ctx, "Expected '=' here");
                    return 0;
                }
                xml_skipWhitespace(ctx);
                if (ctx.hasError || !xml_ensureMore(ctx))
                {
                    return 0;
                }
                Value attrValue = xml_popQuotedValue(ctx);
                xml_skipWhitespace(ctx);
                if (ctx.hasError || !xml_ensureMore(ctx))
                {
                    return 0;
                }
                attributeCount += 1;
                ctx.buffer.Add(attrName);
                ctx.buffer.Add(attrValue);
            }
            ctx.buffer[attributeSizeIndex] = buildInteger(ctx.globals, attributeCount);
            int childCountIndex = ctx.buffer.Count;
            ctx.buffer.Add(ctx.globals.intZero);
            if (xml_peekChar(ctx) == 47)
            {
                xml_popChar(ctx);
                if (xml_popChar(ctx) != 62)
                {
                    xml_setError(ctx, "Expected '>' here.");
                    return 0;
                }
                xml_skipWhitespace(ctx);
                return 0;
            }
            if (xml_popChar(ctx) != 62)
            {
                if (!xml_ensureMore(ctx))
                {
                    return 0;
                }
                xml_setError(ctx, "Expected '>' here.");
                return 0;
            }
            bool insideElement = true;
            int childCount = 0;
            while (insideElement && ctx.index < ctx.length)
            {
                if (xml_peekChar(ctx) == 60)
                {
                    if (xml_tryPopCloseTagFor(ctx, tagNameStart, tagNameEnd))
                    {
                        insideElement = false;
                    }
                    else
                    {
                        childCount += 1;
                        xml_popElement(ctx);
                        if (ctx.hasError)
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    int textStart = ctx.index;
                    xml_popTextValue(ctx);
                    int textEnd = ctx.index;
                    System.Collections.Generic.List<int> textSpanChars = new List<int>();
                    int i = textStart;
                    while (i < textEnd)
                    {
                        textSpanChars.Add(ctx.chars[i]);
                        i += 1;
                    }
                    Value textVal = xml_convertCharsToString(ctx.globals, textSpanChars);
                    ctx.buffer.Add(textVal);
                    childCount += 1;
                }
            }
            ctx.buffer[childCountIndex] = buildInteger(ctx.globals, childCount);
            return 0;
        }

        public static Value xml_popQuotedValue(XmlParseContext ctx)
        {
            if (!xml_ensureMore(ctx))
            {
                return null;
            }
            if (xml_peekChar(ctx) != 34)
            {
                Value word = xml_popWord(ctx);
                if (!ctx.hasError)
                {
                    xml_skipWhitespace(ctx);
                }
                return word;
            }
            xml_popChar(ctx);
            int indexStart = ctx.index;
            System.Collections.Generic.List<int> chars = new List<int>();
            while (ctx.index < ctx.length && xml_peekChar(ctx) != 34)
            {
                chars.Add(xml_popChar(ctx));
            }
            if (!xml_ensureMore(ctx))
            {
                return null;
            }
            xml_popChar(ctx);
            return xml_convertCharsToString(ctx.globals, chars);
        }

        public static int xml_popTextValue(XmlParseContext ctx)
        {
            int indexStart = ctx.index;
            while (ctx.index < ctx.length && ctx.chars[ctx.index] != 60)
            {
                xml_popChar(ctx);
            }
            return 0;
        }

        public static Value xml_popWord(XmlParseContext ctx)
        {
            if (!xml_hasMore(ctx))
            {
                xml_setError(ctx, "Unexpected end of XML document.");
                return null;
            }
            int startIndex = ctx.index;
            int validity = xml_isValidNameChar(xml_popChar(ctx));
            if (validity == 0)
            {
                xml_setError(ctx, "Unexpected character.");
                return null;
            }
            if (validity == 2)
            {
                xml_setError(ctx, "Character is not valid for the first character of a name.");
                return null;
            }
            while (ctx.index < ctx.length && validity > 0)
            {
                validity = xml_isValidNameChar(xml_peekChar(ctx));
                if (validity > 0)
                {
                    xml_popChar(ctx);
                }
            }
            int length = ctx.index - startIndex;
            return createStringFromUnicodeArraySegment(ctx.globals, ctx.chars, startIndex, length);
        }

        public static int xml_setError(XmlParseContext ctx, string msg)
        {
            ctx.hasError = true;
            ctx.errorMessage = msg;
            return 0;
        }

        public static int xml_skipVersionHeaderIfPresent(XmlParseContext ctx)
        {
            return 0;
        }

        public static int xml_skipWhitespace(XmlParseContext ctx)
        {
            while (ctx.index < ctx.length)
            {
                int c = ctx.chars[ctx.index];
                if (c != 32 && (c != 10 && (c != 13 && c != 9)))
                {
                    return 0;
                }
                if (c == 10)
                {
                    ctx.col = 0;
                    ctx.line += 1;
                }
                else
                {
                    ctx.col += 1;
                }
                ctx.index += 1;
            }
            return 0;
        }

        public static bool xml_tryPopCloseTagFor(XmlParseContext ctx, int tagNameStart, int tagNameEnd)
        {
            int tagNameLen = tagNameEnd - tagNameStart;
            int col = ctx.col;
            int line = ctx.line;
            int index = ctx.index;
            if (xml_popCloseTag(ctx, tagNameStart, tagNameEnd))
            {
                return true;
            }
            ctx.col = col;
            ctx.line = line;
            ctx.index = index;
            return false;
        }

        public static int xmlUtil_parse(ExecutionContext ec, int[] chars, Value[] dataOut)
        {
            GlobalValues globals = ec.globalValues;
            dataOut[0] = globals.intZero;
            dataOut[1] = buildString(globals, "XML Parsing is not implemented yet.", false);
            dataOut[2] = globals.intZero;
            dataOut[3] = globals.intZero;
            XmlParseContext ctx = new XmlParseContext(1, 1, false, "", 0, chars, chars.Length, new List<Value>(), globals);
            xml_skipWhitespace(ctx);
            if (!ctx.hasError)
            {
                xml_skipVersionHeaderIfPresent(ctx);
            }
            xml_skipWhitespace(ctx);
            if (!ctx.hasError)
            {
                xml_popElement(ctx);
            }
            xml_skipWhitespace(ctx);
            if (!ctx.hasError && xml_hasMore(ctx))
            {
                ctx.hasError = true;
                ctx.errorMessage = "Unexpected data at end of root element.";
            }
            if (ctx.hasError)
            {
                dataOut[1] = buildString(globals, ctx.errorMessage, false);
                dataOut[2] = buildInteger(globals, ctx.line);
                dataOut[3] = buildInteger(globals, ctx.col);
            }
            else
            {
                dataOut[0] = globals.intOne;
                dataOut[1] = buildList(ec, ctx.buffer.ToArray(), true, -1);
            }
            return 0;
        }
    }
}
