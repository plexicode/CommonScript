﻿using System.Collections.Generic;

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
                "@public function floor(n) { return $math_floor(n); }",
                "@public function getUnixTime() { return $unix_time(0); }",
                "@public function getUnixTimeFloat() { return $unix_time(1); }",
                
                "function map(a, f) { o = []; s = a.length; for (i = 0; i < s; i++) o.add(f(a[i])); return o; }",
                "function filter(a, f) { o = []; s = a.length; for (i = 0; i < s; i++) { v = a[i]; if (f(v) == true) o.add(v); } return o; }",

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
                
                // TODO: there seems to be a parsing problem with **
                // "@public function sqrt(val) { return val ** .5; }",
            ]) },
        };
    }
}
