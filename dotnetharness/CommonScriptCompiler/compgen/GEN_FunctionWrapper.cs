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

        public static void PST_RegisterExtensibleCallback(string name, System.Func<object[], object> func)
        {
            PST_ExtCallbacks[name] = func;
        }

        public static object fail(string msg)
        {
            object[] failArgs = new object[1];
            failArgs[0] = msg;
            return PST_ExtCallbacks.ContainsKey("hardCrash") ? PST_ExtCallbacks["hardCrash"].Invoke(failArgs) : null;
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
            return "@3XmlNode{}\n@3XmlElement:XmlNode{\nfield name;\nfield attributes;\nfield children = [];\nconstructor() : base() { }\n}\n@3XmlText:XmlNode {\nfield value;\nconstructor() : base() { }\n}\n@3XmlParse@6{\nfield line;\nfield col;\nfield err;\nconstructor(e, l, c) : base('XML Parse Error: ' + e) {\nthis.err = e;\nthis.line = l;\nthis.col = c;\n}\n}\n@5parseXml(s){\no = [0, 0, 0, 0];\n$xml_parse(s + '', o);\nif(o[0] == 0)\nthrow new XmlParseException(o[1], o[2], o[3]);\n@4_build([0], o[1]);\n}\n@1_build(\ni,\nb,\n) {\nif (b[i[0]] == 1) {\ni[0]++;\no = new XmlElement();\no.name = b[i[0]++];\nc = b[i[0]++];\na = { };\nwhile (c --> 0) {\nk = b[i[0]++];\nv = b[i[0]++];\na[k] = v;\n}\no.attributes = a;\nc = b[i[0]++];\nwhile (c --> 0)\no.children.add(_build(i, b));\n} else {\no = new XmlText();\no.value = b[i[0]++];\n}\n@4o;\n}";
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
    }
}
