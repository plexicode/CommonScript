using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler.Internal
{
    internal static class FunctionWrapper
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private static Dictionary<string, System.Func<object[], object>> PST_ExtCallbacks = new Dictionary<string, System.Func<object[], object>>();

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

        private static readonly string[] PST_SplitSep = new string[1];

        private static string[] PST_StringSplit(string value, string sep)
        {
            if (sep.Length == 1) return value.Split(sep[0]);
            if (sep.Length == 0) return value.ToCharArray().Select<char, string>(c => "" + c).ToArray();
            PST_SplitSep[0] = sep;
            return value.Split(PST_SplitSep, System.StringSplitOptions.None);
        }

        public static void PST_RegisterExtensibleCallback(string name, System.Func<object[], object> func)
        {
            PST_ExtCallbacks[name] = func;
        }

        public static object _Errors_ThrowImpl(int type, Token t, string s1, string s2)
        {
            object[] args = new object[3];
            args[0] = type;
            if (type == 1)
            {
                args[1] = t;
                args[2] = s1;
            }
            else if (type == 2)
            {
                args[1] = s1;
                args[2] = s2;
            }
            else if (type == 3)
            {
                args[1] = s1;
            }
            return PST_ExtCallbacks.ContainsKey("throwParserException") ? PST_ExtCallbacks["throwParserException"].Invoke(args) : null;
        }

        public static ByteCodeBuffer ByteCodeBuffer_from2(ByteCodeBuffer left, ByteCodeBuffer right)
        {
            return new ByteCodeBuffer(left.length + right.length, false, left, right, null, left.first, right.last);
        }

        public static ByteCodeBuffer ByteCodeBuffer_fromRow(ByteCodeRow row)
        {
            return new ByteCodeBuffer(1, true, null, null, row, row, row);
        }

        public static ByteCodeRow ByteCodeRow_new(int opCode, Token token, string stringArg, int[] args)
        {
            return new ByteCodeRow(opCode, stringArg, token, args, 0, 0, null);
        }

        public static ByteCodeBuffer convertToBuffer(ByteCodeRow[] flatRows)
        {
            ByteCodeBuffer buf = null;
            int length = flatRows.Length;
            int i = 0;
            while (i < length)
            {
                buf = join2(buf, ByteCodeBuffer_fromRow(flatRows[i]));
                i += 1;
            }
            return buf;
        }

        public static ByteCodeBuffer create0(int opCode, Token token, string stringArg)
        {
            return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, new int[0]));
        }

        public static ByteCodeBuffer create1(int opCode, Token token, string stringArg, int arg1)
        {
            int[] args = new int[1];
            args[0] = arg1;
            return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create2(int opCode, Token token, string stringArg, int arg1, int arg2)
        {
            int[] args = new int[2];
            args[0] = arg1;
            args[1] = arg2;
            return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static ByteCodeBuffer create3(int opCode, Token token, string stringArg, int arg1, int arg2, int arg3)
        {
            int[] args = new int[3];
            args[0] = arg1;
            args[1] = arg2;
            args[2] = arg3;
            return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
        }

        public static void Errors_Throw(Token token, string msg)
        {
            _Errors_ThrowImpl(1, token, msg, "");
        }

        public static void Errors_ThrowEof(string fileName, string msg)
        {
            _Errors_ThrowImpl(2, null, fileName, msg);
        }

        public static void Errors_ThrowGeneralError(string msg)
        {
            _Errors_ThrowImpl(3, null, msg, "");
        }

        public static void Errors_ThrowNotImplemented(Token token, string optionalMsg)
        {
            if (optionalMsg == null)
            {
                optionalMsg = "";
            }
            Errors_Throw(token, ("***NOT IMPLEMENTED*** " + optionalMsg).Trim());
        }

        public static object fail(string msg)
        {
            object[] failArgs = new object[1];
            failArgs[0] = msg;
            return PST_ExtCallbacks.ContainsKey("hardCrash") ? PST_ExtCallbacks["hardCrash"].Invoke(failArgs) : null;
        }

        public static ByteCodeRow[] flatten(ByteCodeBuffer buffer)
        {
            if (buffer == null)
            {
                return new ByteCodeRow[0];
            }
            System.Collections.Generic.List<ByteCodeBuffer> q = new List<ByteCodeBuffer>();
            q.Add(buffer);
            System.Collections.Generic.List<ByteCodeRow> output = new List<ByteCodeRow>();
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

        public static string GEN_BUILTINS_base64()
        {
            return "@5base64ToBytes(b64str) { @4$b64_to_bytes(b64str); }\n@5base64ToUtf8String(b64str) { @4$txt_bytes_to_string($b64_to_bytes(b64str),'utf8'); }\n@5bytesToBase64(bytes, web=false) { @4$b64_from_bytes(bytes, web == true); }\n@5stringToBase64(str, web=false) { @4$b64_from_bytes($txt_string_to_bytes(str,'utf8'), web == true); }";
        }

        public static string GEN_BUILTINS_builtins()
        {
            return "@5print(o) { @4$io_stdout(o); }\n@5tryParseInt(s) { @4$parse_int(s); }\n@5tryParseFloat(s) { @4$parse_float(s); }\n@5floor(n) { @4$math_floor(n); }\n@5getUnixTime() { @4$unix_time(0); }\n@5getUnixTimeFloat() { @4$unix_time(1); }\n@5delayInvoke(fn, sec) { $delay_invoke(fn, 1.0*sec); }\n@5sleep(sec) { $sleep(1.0*sec); }\n@1map(a, f) {\no = [];\ns = a.length;\nfor (i = 0; i < s; i++) o.add(f(a[i]));\n@4o;\n}\n@1filter(a, f) {\no = [];\ns = a.length;\nfor (i = 0; i < s; i++) {\nv = a[i];\nif (f(v) == true) o.add(v);\n}\n@4o;\n}\n@1kvpMap(d, f) {\nk=d.keys();\ns=k.length;\nif (s>0) {\nv=d.values();\no=[];\nfor (i=0;i<s;i++){\nk[i]=f(k[i],v[i]);\n}\n}\n@4k;\n}\n@1reduce(a, f, c = reduce) {\ns = a.length;\nif (c == reduce) {\nif (s < 1) thrw(2, \"List must have at least one item.\");\nc = a[0];\ni = 1;\n} else\ni = 0;\nwhile (i < s) {\nc = f(c, a[i]);\ni++;\n}\n@4c;\n}\n@3Exception { field message; field trace = []; constructor(msg = null) { this.message = (msg ?? '') + ''; } }\n@3Fatal@6@2\n@3FieldNotFound@6@2\n@3InvalidArgument@6@2\n@3Invocation@6@2\n@3KeyNotFound@6@2\n@3NotImplemented@6@2\n@3NullReference@6@2\n@3OutOfRange@6@2\n@3StackOverflowException : FatalException@2\n@3Type@6@2\n@3ZeroDivisor@6@2\n@3ImmutableData@6@2\n@1thrw(n, m) {\nswitch (n) {\ncase 1: throw new FatalException(m);\ncase 2: throw new StackOverflowException(m);\ncase 3: throw new FieldNotFoundException(m);\ncase 4: throw new InvalidArgumentException(m);\ncase 5: throw new InvocationException(m);\ncase 6: throw new KeyNotFoundException(m);\ncase 7: throw new NullReferenceException(m);\ncase 8: throw new OutOfRangeException(m);\ncase 9: throw new TypeException(m);\ncase 10: throw new ZeroDivisorException(m);\ncase 11: throw new ImmutableDataException(m);\n}\nthrow new Exception(m);\n}\n@1sortK(arr, keyFn) {\nkeys = [];\nfor (i = 0; i < arr.length; i++) {\nkeys.add(keyFn(arr[i]));\n}\n@4sort(keys, sort, arr);\n}\n@1sort(arr, cmp=sort, mirror=null) {\nis_sys_cmp = cmp == sort;\no = $sort_start(arr, mirror);\np = [0, 0];\nwhile ($sort_get_next_cmp(o, p)) {\n$sort_proceed(o, (is_sys_cmp ? $cmp(p[0], p[1]) : cmp(p[0], p[1])) > 0);\n}\n@4$sort_end(o);\n}";
        }

        public static string GEN_BUILTINS_json()
        {
            return "@3JsonParse@6 {\nconstructor(l, c) : base('JSON parse error on line ' + l + ', col ' + c) {}\n}\n@3JsonSerialization@6 {\nconstructor() : base('Value contained un-serialiazable value.') {}\n}\n@5jsonParse(str) { @4parseImpl(str, true); }\n@5tryJsonParse(str) { @4parseImpl(str, false); }\n@1parseImpl(str, f) {\ne = [0, 0, 0];\nv = $json_parse(str + '', e);\nif (e[0] < 1) @4v;\nif (f) throw new JsonParseException(e[1], e[2]);\n@4null;\n}\n@5jsonSerialize(obj, pretty = false, tab = 2) {\np = pretty == true;\ns = $json_serialize(obj, p);\nif (s == null) throw new JsonSerializationException();\n@4(p && tab != '\\t') ? s.replace('\\t', ' ' * tab) : s;\n}";
        }

        public static string GEN_BUILTINS_math()
        {
            return "@5sin(r) { @4$math_sin(r - 0); }\n@5cos(r) { @4$math_cos(r - 0); }\n@5tan(r) { @4$math_tan(r - 0); }\n@5arcsin(x) { @4$math_arcsin(x - 0); }\n@5arccos(x) { @4$math_arccos(x - 0); }\n@5arctan(yOrVal, x = null) { @4$math_arctan(yOrVal, x == null ? null : (x - 0)); }\n@5log10(val) { @4$math_log(val - 0, 0); }\n@5log2(val) { @4$math_log(val - 0, 1); }\n@5ln(val) { @4$math_log(val - 0, -1); }\n@5abs(val) { @4val < 0 ? -val : val; }\n@5sign(val) { @4val == 0 ? 0 : val < 0 ? -1 : 1; }\n@5sqrt(val) { @4val ** .5; }";
        }

        public static string GEN_BUILTINS_random()
        {
            return "@5randomFloat() { @4$random_float(); }\n@5randomBool() { @4$random_float() < .5; }\n@5randomInt(a, b = null) {\nif (b == null) {\nb = a;\na = 0;\n}\nd = b - a;\nif (d <= 0) throw new InvalidArgumentException(\"Range must be a positive number.\");\n@4a + $math_floor($random_float() * d);\n}";
        }

        public static string GEN_BUILTINS_textencoding()
        {
            return "@3TextEncoding@6 {\nconstructor(m) : base(m) { }\n}\n@1validEnc(e) {\ne = (e ?? '') + '';\nt = e.lower().replace('-', '');\nif ($txt_is_valid_enc(t)) @4t;\nthrow new TextEncodingException(\"'\" + e + \"' is not a valid encoding.\");\n}\n@5bytesToText(arr, e='utf8') {\nn = validEnc(e);\ns = $txt_bytes_to_string(arr, n);\nif (s == null) throw new TextEncodingException(\"Invalid byte values for encoding: '\" + n + \"'\");\n@4s;\n}\n@5textToBytes(s, e = 'utf8') {\n@4$txt_string_to_bytes((s ?? '') + '', validEnc(e));\n}";
        }

        public static string GEN_BUILTINS_xml()
        {
            return "@3XmlNode{}\n@3XmlElement:XmlNode{\nfield name;\nfield attributes;\nfield children = [];\nconstructor() : base() { }\n}\n@3XmlText:XmlNode {\nfield value;\nconstructor() : base() { }\n}\n@3XmlParse@6 {\nfield line;\nfield col;\nfield err;\nconstructor(e, l, c) : base('XML Parse Error: ' + e) {\nthis.err = e;\nthis.line = l;\nthis.col = c;\n}\n}\n@5parseXml(s) {\no = [0, 0, 0, 0];\n$xml_parse(s + '', o);\nif(o[0] == 0)\nthrow new XmlParseException(o[1], o[2], o[3]);\n@4_build([0], o[1]);\n}\n@1_build(\ni,\nb\n) {\nif (b[i[0]] == 1) {\ni[0]++;\no = new XmlElement();\no.name = b[i[0]++];\nc = b[i[0]++];\na = { };\nwhile (c --> 0) {\nk = b[i[0]++];\nv = b[i[0]++];\na[k] = v;\n}\no.attributes = a;\nc = b[i[0]++];\nwhile (c --> 0)\no.children.add(_build(i, b));\n} else {\no = new XmlText();\no.value = b[i[0]++];\n}\n@4o;\n}";
        }

        public static string GetBuiltin(string moduleName)
        {
            string code = GetBuiltinRawStoredString(moduleName);
            if (code == null)
            {
                fail(moduleName);
                return null;
            }
            return code.Replace("@1", "function ").Replace("@2", " { constructor(m=null):base(m){} }").Replace("@3", "@public class ").Replace("@4", "return ").Replace("@5", "@public function ").Replace("@6", "Exception : Exception");
        }

        public static string GetBuiltinRawStoredString(string m)
        {
            if (m == "builtins")
            {
                return GEN_BUILTINS_builtins();
            }
            if (m == "base64")
            {
                return GEN_BUILTINS_base64();
            }
            if (m == "json")
            {
                return GEN_BUILTINS_json();
            }
            if (m == "math")
            {
                return GEN_BUILTINS_math();
            }
            if (m == "random")
            {
                return GEN_BUILTINS_random();
            }
            if (m == "textencoding")
            {
                return GEN_BUILTINS_textencoding();
            }
            if (m == "xml")
            {
                return GEN_BUILTINS_xml();
            }
            return null;
        }

        public static ByteCodeBuffer join2(ByteCodeBuffer a, ByteCodeBuffer b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            return ByteCodeBuffer_from2(a, b);
        }

        public static ByteCodeBuffer join3(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c)
        {
            return join2(a, join2(b, c));
        }

        public static ByteCodeBuffer join4(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c, ByteCodeBuffer d)
        {
            return join2(join2(a, b), join2(c, d));
        }

        public static ByteCodeBuffer join5(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c, ByteCodeBuffer d, ByteCodeBuffer e)
        {
            return join2(join2(a, b), join3(c, d, e));
        }

        public static ByteCodeBuffer join6(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c, ByteCodeBuffer d, ByteCodeBuffer e, ByteCodeBuffer f)
        {
            return join3(join2(a, b), join2(c, d), join2(e, f));
        }

        public static ByteCodeBuffer join7(ByteCodeBuffer a, ByteCodeBuffer b, ByteCodeBuffer c, ByteCodeBuffer d, ByteCodeBuffer e, ByteCodeBuffer f, ByteCodeBuffer g)
        {
            return join4(join2(a, b), join2(c, d), join2(e, f), g);
        }

        public static StaticContext StaticContext_new()
        {
            return new StaticContext(TokenizerStaticContext_new());
        }

        public static StringSet StringSet_add(StringSet s, string item)
        {
            s.items[item] = true;
            return s;
        }

        public static StringSet StringSet_fromArray(string[] items)
        {
            StringSet s = StringSet_new();
            int i = 0;
            while (i < items.Length)
            {
                s.items[items[i]] = true;
                i += 1;
            }
            return s;
        }

        public static StringSet StringSet_fromList(System.Collections.Generic.List<string> items)
        {
            StringSet s = StringSet_new();
            int i = 0;
            while (i < items.Count)
            {
                s.items[items[i]] = true;
                i += 1;
            }
            return s;
        }

        public static bool StringSet_has(StringSet s, string item)
        {
            return s.items.ContainsKey(item);
        }

        public static StringSet StringSet_new()
        {
            return new StringSet(new Dictionary<string, bool>());
        }

        public static int[] StringToUnicodeArray(string strVal)
        {
            int[] bytes = PST_stringToUtf8Bytes(strVal);
            int[] output = new int[bytes.Length];
            int length = bytes.Length;
            int c = 0;
            int i = 0;
            int j = 0;
            int val = 0;
            while (i < length)
            {
                c = bytes[i];
                if (c < 128)
                {
                    output[j] = c;
                    i += 1;
                }
                else if ((c & 224) == 192)
                {
                    output[j] = (c & 31) << 6 | (bytes[i + 1] & 63);
                    i += 2;
                }
                else if ((c & 240) == 224)
                {
                    output[j] = (c & 15) << 12 | (bytes[i + 1] & 63) << 6 | (bytes[i + 2] & 63);
                    i += 3;
                }
                else if ((c & 240) == 224)
                {
                    output[j] = (c & 7) << 18 | (bytes[i + 1] & 63) << 12 | (bytes[i + 2] & 63) << 6 | (bytes[i + 3] & 63);
                    i += 4;
                }
                else
                {
                    return null;
                }
                j += 1;
            }
            if (j < length)
            {
                int[] trimmedOutput = new int[j];
                while (j > 0)
                {
                    j -= 1;
                    trimmedOutput[j] = output[j];
                }
                output = trimmedOutput;
            }
            return output;
        }

        public static string Token_getFingerprint(Token t)
        {
            if (t.Fingerprint == null)
            {
                t.Fingerprint = string.Join("", new string[] { t.File, ",", t.Line.ToString(), ",", t.Col.ToString() });
            }
            return t.Fingerprint;
        }

        public static Token Token_new(string value, int type, string file, int line, int col)
        {
            return new Token(value, file, type, line, col, null);
        }

        public static Token[] Tokenize(string file, string content, StaticContext ctx)
        {
            string contentPadded = (content.Replace("\r\n", "\n").Replace("\r", "\n")) + "\n\n\n\n\n";
            int[] chars = StringToUnicodeArray(contentPadded);
            int length = chars.Length;
            TokenizerStaticContext tokenizerCtx = ctx.tokenizerCtx;
            int[] lines = new int[length];
            int[] cols = new int[length];
            int i = 0;
            int j = 0;
            int k = 0;
            int line = 1;
            int col = 1;
            int c = 0;
            i = 0;
            while (i < length)
            {
                c = chars[i];
                lines[i] = line;
                cols[i] = col;
                if (c == 10)
                {
                    line += 1;
                    col = 1;
                }
                else
                {
                    col += 1;
                }
                i += 1;
            }
            int mode = 1;
            int tokenStart = 0;
            int tokenSubtype = 0;
            string tokenVal = "";
            System.Collections.Generic.List<Token> tokens = new List<Token>();
            i = 0;
            while (i < length)
            {
                c = chars[i];
                switch (mode)
                {
                    case 1:
                        if (tokenizerCtx.whitespace.ContainsKey(c))
                        {
                        }
                        else if (tokenizerCtx.alphanumerics.ContainsKey(c))
                        {
                            mode = 3;
                            tokenStart = i;
                        }
                        else if (c == 34 || c == 39)
                        {
                            mode = 2;
                            tokenStart = i;
                            tokenSubtype = c;
                        }
                        else if (c == 47 && (chars[i + 1] == 47 || chars[i + 1] == 42))
                        {
                            mode = 4;
                            tokenSubtype = chars[i + 1];
                            i += 1;
                        }
                        else
                        {
                            tokenVal = null;
                            if (c == 62 && chars[i + 1] == c && chars[i + 2] == c && chars[i + 3] == 61)
                            {
                                tokenVal = ">>>=";
                            }
                            else if (tokenizerCtx.multicharTokensByFirstChar.ContainsKey(c))
                            {
                                System.Collections.Generic.List<int[]> mcharCandidates = tokenizerCtx.multicharTokensByFirstChar[c];
                                j = 0;
                                while (j < mcharCandidates.Count && tokenVal == null)
                                {
                                    int[] mcharCandidate = mcharCandidates[j];
                                    bool isMatch = true;
                                    int mSize = mcharCandidate.Length;
                                    k = 1;
                                    while (k < mSize)
                                    {
                                        if (mcharCandidate[k] != chars[i + k])
                                        {
                                            isMatch = false;
                                            k += mSize;
                                        }
                                        k += 1;
                                    }
                                    if (isMatch)
                                    {
                                        tokenVal = UnicodeArrayToString(mcharCandidate);
                                    }
                                    j += 1;
                                }
                            }
                            if (tokenVal == null)
                            {
                                tokenVal = (char)c + "";
                            }
                            tokens.Add(Token_new(tokenVal, 3, file, lines[i], cols[i]));
                            i += tokenVal.Length - 1;
                        }
                        break;
                    case 3:
                        if (!tokenizerCtx.alphanumerics.ContainsKey(c))
                        {
                            int tokenLen = i - tokenStart;
                            tokenVal = UnicodeArrayToString_slice(chars, tokenStart, tokenLen);
                            int firstChar = chars[tokenStart];
                            int tokenType = 2;
                            if (tokenizerCtx.numerics.ContainsKey(firstChar))
                            {
                                if (firstChar == 48 && tokenLen > 2 && (chars[tokenStart + 1] == 120 || chars[tokenStart + 1] == 88))
                                {
                                    tokenType = 6;
                                }
                                else
                                {
                                    tokenType = 5;
                                }
                            }
                            else if (StringSet_has(tokenizerCtx.keywords, tokenVal))
                            {
                                tokenType = 1;
                            }
                            tokens.Add(Token_new(tokenVal, tokenType, file, lines[tokenStart], cols[tokenStart]));
                            i -= 1;
                            mode = 1;
                        }
                        break;
                    case 4:
                        if (c == 10 && tokenSubtype == 47)
                        {
                            mode = 1;
                        }
                        else if (c == 42 && tokenSubtype == c && chars[i + 1] == 47)
                        {
                            mode = 1;
                            i += 1;
                        }
                        break;
                    case 2:
                        if (c == tokenSubtype)
                        {
                            tokenVal = UnicodeArrayToString_slice(chars, tokenStart, i - tokenStart + 1);
                            mode = 1;
                            tokens.Add(Token_new(tokenVal, 4, file, lines[tokenStart], cols[tokenStart]));
                        }
                        else if (c == 92)
                        {
                            i += 1;
                        }
                        break;
                    default:
                        return null;
                }
                i += 1;
            }
            if (mode != 1)
            {
                if (mode == 2)
                {
                    Errors_ThrowEof(file, "Unclosed string.");
                }
                Errors_ThrowEof(file, "Unclosed comment.");
            }
            i = 0;
            while (i < tokens.Count)
            {
                Token current = tokens[i];
                if (current != null)
                {
                    Token left = null;
                    Token right = null;
                    if (i > 0)
                    {
                        left = tokens[i - 1];
                    }
                    if (i + 1 < tokens.Count)
                    {
                        right = tokens[i + 1];
                    }
                    if (left != null && (left.Line != current.Line || left.Col + left.Value.Length != current.Col))
                    {
                        left = null;
                    }
                    if (right != null && (right.Line != current.Line || current.Col + current.Value.Length != right.Col))
                    {
                        right = null;
                    }
                    if (current.Value == "@" && right != null && (right.Type == 2 || right.Type == 1))
                    {
                        current.Value += right.Value;
                        current.Type = 8;
                        tokens[i + 1] = null;
                    }
                    else if (current.Value == ".")
                    {
                        if (left != null && left.Type == 5)
                        {
                            left.Value += ".";
                            left.Type = 7;
                            tokens[i] = left;
                            tokens[i - 1] = null;
                            current = left;
                            left = null;
                        }
                        if (right != null && right.Type == 5)
                        {
                            current.Value += right.Value;
                            tokens[i + 1] = null;
                            current.Type = 7;
                        }
                    }
                }
                i += 1;
            }
            System.Collections.Generic.List<Token> output = new List<Token>();
            i = 0;
            while (i < tokens.Count)
            {
                if (tokens[i] != null)
                {
                    output.Add(tokens[i]);
                }
                i += 1;
            }
            return output.ToArray();
        }

        public static TokenizerStaticContext TokenizerStaticContext_new()
        {
            TokenizerStaticContext ctx = new TokenizerStaticContext(new Dictionary<int, bool>(), new Dictionary<int, bool>(), new Dictionary<int, bool>(), null, new Dictionary<int, System.Collections.Generic.List<int[]>>());
            int i = 0;
            i = 0;
            while (i < 10)
            {
                ctx.numerics[48 + i] = true;
                ctx.alphanumerics[48 + i] = true;
                i += 1;
            }
            i = 0;
            while (i < 26)
            {
                ctx.alphanumerics[65 + i] = true;
                ctx.alphanumerics[97 + i] = true;
                i += 1;
            }
            ctx.alphanumerics[95] = true;
            string ws = " \r\n\t";
            i = 0;
            while (i < ws.Length)
            {
                ctx.whitespace[(int)ws[i]] = true;
                i += 1;
            }
            ctx.keywords = StringSet_fromArray(PST_StringSplit("function class field property constructor const enum\nbase this\nnull false true new\nis typeof\nif else for while do break continue switch case default yield return\nthrow try catch finally\nimport namespace\npublic static readonly abstract".Trim().Replace("\n", " "), " "));
            string[] mcharTokens = PST_StringSplit(">>>= >>> <<= >>= **= ++ -- && || ** == != <= >= => -> << >> ?? += -= *= %= /= |= &= ^=", " ");
            i = 0;
            while (i < mcharTokens.Length)
            {
                string mcharTok = mcharTokens[i];
                int[] uchars = new int[mcharTok.Length];
                int j = 0;
                while (j < uchars.Length)
                {
                    uchars[j] = (int)mcharTok[j];
                    j += 1;
                }
                int firstChar = uchars[0];
                if (!ctx.multicharTokensByFirstChar.ContainsKey(firstChar))
                {
                    ctx.multicharTokensByFirstChar[firstChar] = new List<int[]>();
                }
                ctx.multicharTokensByFirstChar[firstChar].Add(uchars);
                i += 1;
            }
            return ctx;
        }

        public static string UnicodeArrayToString(int[] chars)
        {
            return UnicodeArrayToString_slice(chars, 0, chars.Length);
        }

        public static string UnicodeArrayToString_slice(int[] chars, int start, int length)
        {
            System.Collections.Generic.List<string> sb = new List<string>();
            int end = start + length;
            int i = start;
            while (i < end)
            {
                sb.Add(((char)chars[i]).ToString());
                i += 1;
            }
            return string.Join("", sb);
        }
    }
}
