using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class BuiltinUtil
    {
        public static string GetBuiltinSource()
        {
            string template = "@public class #Exception:Exception { constructor(m=null):base(m){} }";

            string[] lines = [
                // TODO: o == null ? 'null' : (o + '')
                "@public function print(o) { return $io_stdout(o); }",
                "@public function tryParseInt(s) { return $parse_int(s); }",
                "@public function tryParseFloat(s) { return $parse_float(s); }",
                "@public function floor(n) { return $math_floor(n); }",
                "@public function getUnixTime() { return $unix_time(0); }",
                "@public function getUnixTimeFloat() { return $unix_time(1); }",

                "@public function delayInvoke(fn, sec) { $delay_invoke(fn, 1.0*sec); }",
                "@public function sleep(sec) { $sleep(1.0*sec); }",

                "function map(a, f) { o = []; s = a.length; for (i = 0; i < s; i++) o.add(f(a[i])); return o; }",
                "function filter(a, f) { o = []; s = a.length; for (i = 0; i < s; i++) { v = a[i]; if (f(v) == true) o.add(v); } return o; }",
                "function kvpMap(d, f) {",
                    "k=d.keys();",
                    "s=k.length;",
                    "if (s>0) {",
                        "v=d.values();",
                        "o=[];",
                        "for (i=0;i<s;i++){",
                            "k[i]=f(k[i],v[i]);",
                        "}",
                    "}",
                    "return k;",
                "}",

                // TODO: throw an error instead of return if the size is too small without an acc.
                // Because reduce is an internally-private reference, use it as its own singleton to indicate no value passed in. null is a valid default accumulator value.
                "function reduce(a, f, c = reduce) { s = a.length; if (c == reduce) { if (s < 1) thrw(2, 'List must have at least one item.'); c = a[0]; i = 1; } else i = 0; while (i < s) { c = f(c, a[i]); i++; } return c; }",

                "@public class Exception { field message; field trace = []; constructor(msg = null) { this.message = (msg ?? '') + ''; } }",
                template.Replace("#", "Fatal"),
                template.Replace("#", "FieldNotFound"),
                template.Replace("#", "InvalidArgument"),
                template.Replace("#", "Invocation"),
                template.Replace("#", "KeyNotFound"),
                template.Replace("#", "NotImplemented"),
                template.Replace("#", "NullReference"),
                template.Replace("#", "OutOfRange"),
                template.Replace("#", "StackOverflow").Replace("n:", "n:Fatal"),
                template.Replace("#", "Type"),
                template.Replace("#", "ZeroDivisor"),
                template.Replace("#", "ImmutableData"),

                "function thrw(n, m) {",
                  "switch (n) {",
                    "case 1: throw new FatalException(m);", // ideally this should be subclassed out to more specific exceptions if the need arises to throw this
                    "case 2: throw new StackOverflowException(m);",

                    "case 3: throw new FieldNotFoundException(m);",
                    "case 4: throw new InvalidArgumentException(m);",
                    "case 5: throw new InvocationException(m);",
                    "case 6: throw new KeyNotFoundException(m);",
                    "case 7: throw new NullReferenceException(m);",
                    "case 8: throw new OutOfRangeException(m);",
                    "case 9: throw new TypeException(m);",
                    "case 10: throw new ZeroDivisorException(m);",
                    "case 11: throw new ImmutableDataException(m);",
                  "}",
                  "throw new Exception(m);",
                "}",

                "function sortK(arr, keyFn) {",
                  "keys = [];",
                  "for (i = 0; i < arr.length; i++) {",
                    "keys.add(keyFn(arr[i]));",
                  "}",
                  "return sort(keys, sort, arr);",
                "}",

                "function sort(arr, cmp=sort, mirror=null) {",
                  "is_sys_cmp = cmp == sort;",
                  "o = $sort_start(arr, mirror);",
                  "p = [0, 0];",
                  "while ($sort_get_next_cmp(o, p)) {",
                    "$sort_proceed(o, (is_sys_cmp ? $cmp(p[0], p[1]) : cmp(p[0], p[1])) > 0);",
                  "}",
                  "return $sort_end(o);",
                "}",

                "",
            ];

            return string.Join("\n", lines);
        }

        public static string GetSourceFilesFor(string moduleId)
        {
            return BUILTIN_MODULES[moduleId];
        }

        public static bool IsBuiltInModule(string moduleId)
        {
            return BUILTIN_MODULES.ContainsKey(moduleId);
        }

        private static readonly Dictionary<string, string> BUILTIN_MODULES = new Dictionary<string, string>()
        {
            { "random", string.Join('\n', [
                "@public function randomFloat() { return $random_float(); }",
                "@public function randomInt(a, b = null) {",
                    "if (b == null) { b = a; a = 0; }",
                    "d = b - a;",
                    "if (d <= 0) throw new InvalidArgumentException('Range must be a positive number.');",
                    "return a + $math_floor($random_float() * d);",
                "}",
                "@public function randomBool() { return $random_float() < .5; }",
                "",
            ]) },
            { "math", string.Join('\n', [
                // Use n - 0 to force numeric validation. 
                // TODO: better quick argument validation.
                "@public function sin(r) { return $math_sin(r-0); }",
                "@public function cos(r) { return $math_cos(r-0); }",
                "@public function tan(r) { return $math_tan(r-0); }",
                "@public function arcsin(x) { return $math_arcsin(x-0); }",
                "@public function arccos(x) { return $math_arccos(x-0); }",
                "@public function arctan(yOrVal, x = null) { return $math_arctan(yOrVal, x == null ? null : (x-0)); }",
                "@public function log10(val) { return $math_log(val-0, 0); }",
                "@public function log2(val) { return $math_log(val-0, 1); }",
                "@public function ln(val) { return $math_log(val-0, -1); }",
                "@public function abs(val) { return val < 0 ? -val : val; }",
                "@public function sign(val) { return val == 0 ? 0 : val < 0 ? -1 : 1; }",
                "@public function sqrt(val) { return val ** .5; }",
                "",
            ]) },
            { "base64", string.Join('\n', [
                "@public function base64ToBytes(b64str) { return $b64_to_bytes(b64str); }",
                "@public function base64ToUtf8String(b64str) { return $txt_bytes_to_string($b64_to_bytes(b64str),'utf8'); }",
                "@public function bytesToBase64(bytes, web=false) { return $b64_from_bytes(bytes, web == true); }",
                "@public function stringToBase64(str, web=false) { return $b64_from_bytes($txt_string_to_bytes(str,'utf8'), web == true); }",
                "",
            ]) },
            { "json", string.Join('\n', [
                "@public class JsonParseException:Exception { constructor(l, c):base('JSON parse error on line ' + l + ', col ' + c){} }",
                "@public class JsonSerializationException:Exception { constructor():base('Value contained un-serialiazable valule.'){} }",
                "@public function jsonParse(str) { return parseImpl(str, true); }",
                "@public function tryJsonParse(str) { return parseImpl(str, false); }",
                "function parseImpl(str, f) {",
                    "e = [0, 0, 0];",
                    "v = $json_parse(str + '', e);",
                    "if (e[0]<1) return v;",
                    "if (f) throw new JsonParseException(e[1], e[2]);",
                    "return null;",
                "}",
                "@public function jsonSerialize(obj, pretty = false, tab = 2) {",
                    "p = pretty == true;",
                    "s = $json_serialize(obj, p);",
                    "if (s == null) throw new JsonSerializationException();",
                    "return (p && tab != '\t') ? s.replace('\t', ' ' * tab) : s;",
                "}",
                "",
            ]) },
            { "textencoding", string.Join('\n', [
                "@public class TextEncodingException:Exception { constructor(m):base(m){} }",
                "function validEnc(e) {",
                    "e = (e??'')+'';",
                    "t = e.lower().replace('-', '');",
                    "if ($txt_is_valid_enc(t)) return t;",
                    "throw new TextEncodingException(\"'\" + e + \"' is not a valid encoding.\");",
                "}",
                "@public function bytesToText(arr, e='utf8') {",
                    "n = validEnc(e);",
                    "s = $txt_bytes_to_string(arr, n);",
                    "if (s == null) throw new TextEncodingException(\"Invalid byte values for encoding: '\" + n + \"'\");",
                    "return s;",
                "}",
                "@public function textToBytes(s, e='utf8') {",
                    "return $txt_string_to_bytes((s??'')+'', validEnc(e));",
                "}",
                "",
            ]) },
        };
    }
}
