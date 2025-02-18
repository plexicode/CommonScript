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

        public static AbstractEntity AbstractEntity_new(Token firstToken, int type, object specificData)
        {
            return new AbstractEntity(firstToken, type, specificData, null, null, null, null, false, null, null, -1);
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

        public static ByteCodeBuffer ByteCodeUtil_ensureBooleanExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null)
            {
                fail("invalid operation");
                return null;
            }
            ByteCodeRow last = buf.last;
            if (last.opCode == 4)
            {
                string op = last.stringArg;
                if (op == "||" || op == "&&" || op == "==" || op == "!=" || op == "<" || op == ">" || op == "<=" || op == ">=")
                {
                    return buf;
                }
            }
            if (last.opCode == 16)
            {
                return buf;
            }
            if (last.opCode == 36)
            {
                return buf;
            }
            if (last.opCode == 11)
            {
                return buf;
            }
            return join2(buf, create0(16, throwToken, null));
        }

        public static ByteCodeBuffer ByteCodeUtil_ensureIntegerExpression(Token throwToken, ByteCodeBuffer buf)
        {
            if (buf == null)
            {
                fail("invalid operation");
                return null;
            }
            ByteCodeRow last = buf.last;
            if (last.opCode == 40)
            {
                return buf;
            }
            if (last.opCode == 10)
            {
                return buf;
            }
            return join2(buf, create0(18, throwToken, null));
        }

        public static CatchChunk CatchChunk_new(Statement[] catchCode, System.Collections.Generic.List<Token[]> exceptionClassNamesRaw, Token exceptionVarToken)
        {
            return new CatchChunk(exceptionClassNamesRaw.ToArray(), null, exceptionVarToken, catchCode, false);
        }

        public static ClassEntity ClassEntity_new(Token classToken, Token nameToken, string fqName)
        {
            ClassEntity cd = new ClassEntity(null, null, null, null, 0, null, new Dictionary<string, AbstractEntity>(), null);
            cd.baseData = AbstractEntity_new(classToken, 1, cd);
            cd.baseData.nameToken = nameToken;
            cd.baseData.simpleName = nameToken.Value;
            cd.baseData.fqName = fqName;
            return cd;
        }

        public static CompiledModule CompiledModule_new(string id)
        {
            return new CompiledModule(id, new Dictionary<string, string>(), null, null, null, null);
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

        public static Token createFakeToken(TokenStream tokens, int type, string value, int line, int col)
        {
            return Token_new(value, type, tokens.file, line, col);
        }

        public static string[] DotField_getVariableRootedDottedChain(Expression outermostDotField, string errorMessage)
        {
            System.Collections.Generic.List<string> chain = new List<string>();
            chain.Add(outermostDotField.strVal);
            Expression walker = outermostDotField.root;
            while (walker != null)
            {
                chain.Add(walker.strVal);
                if (walker.type == 11)
                {
                    walker = walker.root;
                }
                else if (walker.type == 31)
                {
                    walker = null;
                }
                else if (errorMessage != null)
                {
                    Errors_Throw(walker.firstToken, errorMessage);
                }
                else
                {
                    return null;
                }
            }
            chain.Reverse();
            return chain.ToArray();
        }

        public static System.Collections.Generic.Dictionary<string, AbstractEntity> Entity_getMemberLookup(StaticContext staticCtx, AbstractEntity entity)
        {
            if (entity.type == 1)
            {
                return ((ClassEntity)entity.specificData).classMembers;
            }
            if (entity.type == 7)
            {
                return ((NamespaceEntity)entity.specificData).nestedMembers;
            }
            if (entity.type == 9)
            {
                return ((ModuleWrapperEntity)entity.specificData).publicMembers;
            }
            return staticCtx.emptyLookup;
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

        public static Expression Expression_cloneWithNewToken(Token token, Expression expr)
        {
            return new Expression(token, expr.type, expr.root, expr.left, expr.right, expr.opToken, expr.boolVal, expr.strVal, expr.intVal, expr.floatVal, expr.entityPtr, expr.importPtr, expr.args, expr.keys, expr.values, expr.argNames, expr.nestedCode);
        }

        public static Expression Expression_createBaseCtorReference(Token token)
        {
            return Expression_new(token, 2);
        }

        public static Expression Expression_createBaseReference(Token token)
        {
            return Expression_new(token, 1);
        }

        public static Expression Expression_createBinaryOp(Expression left, Token op, Expression right)
        {
            Expression pair = Expression_new(left.firstToken, 3);
            pair.opToken = op;
            pair.left = left;
            pair.right = right;
            return pair;
        }

        public static Expression Expression_createBoolConstant(Token token, bool val)
        {
            Expression expr = Expression_new(token, 5);
            expr.boolVal = val;
            return expr;
        }

        public static Expression Expression_createBracketIndex(Expression root, Token bracketToken, Expression index)
        {
            Expression bracketIndex = Expression_new(root.firstToken, 20);
            bracketIndex.root = root;
            bracketIndex.opToken = bracketToken;
            bracketIndex.right = index;
            return bracketIndex;
        }

        public static Expression Expression_createClassReference(Token firstToken, AbstractEntity classDef)
        {
            Expression classRef = Expression_new(firstToken, 7);
            classRef.entityPtr = classDef;
            classRef.boolVal = false;
            return classRef;
        }

        public static Expression Expression_createConstructorInvocation(Token firstToken, AbstractEntity classDef, Token invokeToken, Expression[] args)
        {
            Expression ctorInvoke = Expression_new(firstToken, 8);
            ctorInvoke.entityPtr = classDef;
            ctorInvoke.args = args;
            ctorInvoke.opToken = invokeToken;
            return ctorInvoke;
        }

        public static Expression Expression_createConstructorReference(Token newToken, Expression nameChain)
        {
            Expression ctor = Expression_new(newToken, 9);
            ctor.root = nameChain;
            return ctor;
        }

        public static Expression Expression_createDictionaryDefinition(Token openDict, Expression[] keys, Expression[] values)
        {
            Expression expr = Expression_new(openDict, 10);
            expr.keys = keys;
            expr.values = values;
            int i = 0;
            while (i < keys.Length)
            {
                Expression key = keys[i];
                if (key.type != 28 && key.type != 22)
                {
                    Errors_Throw(key.firstToken, "Only string and integer constants can be used as dictionary keys");
                }
                i += 1;
            }
            return expr;
        }

        public static Expression Expression_createDotField(Expression root, Token dotToken, string name)
        {
            Expression df = Expression_new(root.firstToken, 11);
            df.root = root;
            df.opToken = dotToken;
            df.strVal = name;
            return df;
        }

        public static Expression Expression_createEnumConstant(Token firstToken, AbstractEntity enumDef, string name, int value)
        {
            Expression enumConst = Expression_new(firstToken, 12);
            enumConst.entityPtr = enumDef;
            enumConst.strVal = name;
            enumConst.intVal = value;
            return enumConst;
        }

        public static Expression Expression_createEnumReference(Token firstToken, AbstractEntity enumDef)
        {
            Expression enumRef = Expression_new(firstToken, 13);
            enumRef.entityPtr = enumDef;
            return enumRef;
        }

        public static Expression Expression_createExtensionInvocation(Token firstToken, string name, Expression[] args)
        {
            Expression extInvoke = Expression_new(firstToken, 14);
            extInvoke.strVal = name;
            extInvoke.args = args;
            return extInvoke;
        }

        public static Expression Expression_createExtensionReference(Token prefixToken, string name)
        {
            Expression extRef = Expression_new(prefixToken, 15);
            extRef.strVal = name;
            return extRef;
        }

        public static Expression Expression_createFloatConstant(Token token, double val)
        {
            Expression expr = Expression_new(token, 16);
            expr.floatVal = val;
            return expr;
        }

        public static Expression Expression_createFunctionInvocation(Expression root, Token parenToken, Expression[] args)
        {
            Expression funcInvoke = Expression_new(root.firstToken, 17);
            funcInvoke.root = root;
            funcInvoke.opToken = parenToken;
            funcInvoke.args = args;
            return funcInvoke;
        }

        public static Expression Expression_createFunctionReference(Token firstToken, string name, AbstractEntity funcDef)
        {
            Expression funcRef = Expression_new(firstToken, 18);
            funcRef.strVal = name;
            funcRef.entityPtr = funcDef;
            return funcRef;
        }

        public static Expression Expression_createImportReference(Token firstToken, ImportStatement importStatement)
        {
            Expression impRef = Expression_new(firstToken, 19);
            impRef.importPtr = importStatement;
            return impRef;
        }

        public static Expression Expression_createInlineIncrement(Token firstToken, Expression root, Token incrementOp, bool isPrefix)
        {
            Expression expr = Expression_new(firstToken, 21);
            expr.opToken = incrementOp;
            expr.root = root;
            expr.boolVal = isPrefix;
            switch (root.type)
            {
                case 11:
                    break;
                case 20:
                    break;
                case 31:
                    break;
                default:
                    Errors_Throw(incrementOp, string.Join("", new string[] { "The '", incrementOp.Value, "' operator is not allowed on this type of expression." }));
                    break;
            }
            return expr;
        }

        public static Expression Expression_createIntegerConstant(Token token, int val)
        {
            Expression expr = Expression_new(token, 22);
            expr.intVal = val;
            return expr;
        }

        public static Expression Expression_createLambda(Token firstToken, Token[] argNameTokens, Expression[] argDefaultValues, Token arrowToken, Statement[] code)
        {
            Expression expr = Expression_new(firstToken, 23);
            expr.argNames = argNameTokens;
            expr.values = argDefaultValues;
            expr.opToken = arrowToken;
            expr.nestedCode = code;
            return expr;
        }

        public static Expression Expression_createListDefinition(Token openList, Expression[] items)
        {
            Expression expr = Expression_new(openList, 24);
            expr.values = items;
            return expr;
        }

        public static Expression Expression_createNamespaceReference(Token firstToken, AbstractEntity nsDef)
        {
            Expression nsRef = Expression_new(firstToken, 32);
            nsRef.entityPtr = nsDef;
            return nsRef;
        }

        public static Expression Expression_createNegatePrefix(Token opToken, Expression root)
        {
            int t = 25;
            if (opToken.Value == "!")
            {
                t = 6;
            }
            if (opToken.Value == "~")
            {
                t = 4;
            }
            Expression expr = Expression_new(opToken, t);
            expr.opToken = opToken;
            expr.root = root;
            return expr;
        }

        public static Expression Expression_createNullConstant(Token token)
        {
            return Expression_new(token, 26);
        }

        public static Expression Expression_createSliceExpression(Expression rootExpression, Token bracketToken, Expression start, Expression end, Expression step)
        {
            Expression sliceExpr = Expression_new(rootExpression.firstToken, 27);
            sliceExpr.root = rootExpression;
            sliceExpr.opToken = bracketToken;
            Expression[] args = new Expression[3];
            args[0] = start;
            args[1] = end;
            args[2] = step;
            sliceExpr.args = args;
            return sliceExpr;
        }

        public static Expression Expression_createStringConstant(Token token, string val)
        {
            Expression expr = Expression_new(token, 28);
            expr.strVal = val;
            return expr;
        }

        public static Expression Expression_createTernary(Expression condition, Token op, Expression trueValue, Expression falseValue)
        {
            Expression ternary = Expression_new(condition.firstToken, 29);
            ternary.root = condition;
            ternary.opToken = op;
            ternary.left = trueValue;
            ternary.right = falseValue;
            return ternary;
        }

        public static Expression Expression_createThisReference(Token token)
        {
            return Expression_new(token, 30);
        }

        public static Expression Expression_createTypeof(Token typeofToken, Expression root)
        {
            Expression typeofExpr = Expression_new(typeofToken, 33);
            typeofExpr.root = root;
            return typeofExpr;
        }

        public static Expression Expression_createVariable(Token token, string varName)
        {
            Expression expr = Expression_new(token, 31);
            expr.strVal = varName;
            return expr;
        }

        public static Expression Expression_new(Token firstToken, int type)
        {
            return new Expression(firstToken, type, null, null, null, null, false, null, 0, 0.0, null, null, null, null, null, null, null);
        }

        public static object fail(string msg)
        {
            object[] failArgs = new object[1];
            failArgs[0] = msg;
            return PST_ExtCallbacks.ContainsKey("hardCrash") ? PST_ExtCallbacks["hardCrash"].Invoke(failArgs) : null;
        }

        public static void FileContext_initializeImportLookup(FileContext fileCtx)
        {
            fileCtx.importsByVar = new Dictionary<string, ImportStatement>();
            int i = 0;
            while (i < fileCtx.imports.Length)
            {
                ImportStatement imp = fileCtx.imports[i];
                string varName = null;
                if (imp.importTargetVariableName != null)
                {
                    varName = imp.importTargetVariableName.Value;
                }
                else
                {
                    if (imp.flatName.Contains("."))
                    {
                        Errors_Throw(imp.importToken, "Dot-delimited import paths must use an alias.");
                    }
                    varName = imp.flatName;
                }
                if (varName != "*" && fileCtx.importsByVar.ContainsKey(varName))
                {
                    Errors_Throw(imp.importTargetVariableName, string.Join("", new string[] { "There are multiple imports loaded as the variable '", varName, "'" }));
                }
                fileCtx.importsByVar[varName] = imp;
                i += 1;
            }
        }

        public static FileContext FileContext_new(StaticContext staticCtx, string path, string content)
        {
            return new FileContext(staticCtx, path, content, TokenStream_new(path, Tokenize(path, content, staticCtx)), null, null, false, false, null);
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

        public static ImportStatement ImportStatement_new(Token importToken, System.Collections.Generic.List<Token> tokenChain, Token targetVarName)
        {
            System.Collections.Generic.List<string> flatName = new List<string>();
            int i = 0;
            while (i < tokenChain.Count)
            {
                flatName.Add(tokenChain[i].Value);
                i += 1;
            }
            return new ImportStatement(importToken, tokenChain.ToArray(), string.Join(".", flatName), targetVarName, targetVarName != null && targetVarName.Value == "*", null);
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

        public static ModuleWrapperEntity ModuleWrapperEntity_new(Token token, ImportStatement imp)
        {
            System.Collections.Generic.Dictionary<string, AbstractEntity> modEnts = imp.compiledModuleRef.nestedEntities;
            ModuleWrapperEntity mw = new ModuleWrapperEntity(modEnts, null);
            mw.baseData = AbstractEntity_new(token, 9, mw);
            return mw;
        }

        public static NamespaceEntity NamespaceEntity_new(Token nsToken, Token nameToken, string fqName)
        {
            NamespaceEntity ns = new NamespaceEntity(new Dictionary<string, AbstractEntity>(), null);
            ns.baseData = AbstractEntity_new(nsToken, 7, ns);
            ns.baseData.nameToken = nameToken;
            ns.baseData.simpleName = nameToken.Value;
            ns.baseData.fqName = fqName;
            return ns;
        }

        public static Statement Statement_new(Token firstToken, int type)
        {
            return new Statement(firstToken, type, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0);
        }

        public static StaticContext StaticContext_new()
        {
            return new StaticContext(TokenizerStaticContext_new(), new Dictionary<string, AbstractEntity>());
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

        public static SwitchChunk SwitchChunk_new()
        {
            return new SwitchChunk(new List<Token>(), new List<Expression>(), new List<Statement>());
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

        public static bool Tokens_doesNextInclude5(TokenStream tokens, string val1, string val2, string val3, string val4, string val5)
        {
            string next = Tokens_peekValue(tokens);
            return next == val1 || next == val2 || next == val3 || next == val4 || next == val5;
        }

        public static bool Tokens_doesNextInclulde2(TokenStream tokens, string val1, string val2)
        {
            return Tokens_doesNextInclulde4(tokens, val1, val2, null, null);
        }

        public static bool Tokens_doesNextInclulde3(TokenStream tokens, string val1, string val2, string val3)
        {
            return Tokens_doesNextInclulde4(tokens, val1, val2, val3, null);
        }

        public static bool Tokens_doesNextInclulde4(TokenStream tokens, string val1, string val2, string val3, string val4)
        {
            string next = Tokens_peekValue(tokens);
            return next == val1 || next == val2 || next == val3 || next == val4;
        }

        public static void Tokens_ensureMore(TokenStream tokens)
        {
            if (!Tokens_hasMore(tokens))
            {
                Errors_ThrowEof(tokens.file, "Unexpected end of file");
            }
        }

        public static bool Tokens_hasMore(TokenStream tokens)
        {
            return tokens.index < tokens.length;
        }

        public static bool Tokens_isNext(TokenStream tokens, string value)
        {
            return Tokens_peekValue(tokens) == value;
        }

        public static bool Tokens_isSequenceNext2(TokenStream tokens, string val1, string val2)
        {
            return Tokens_isSequenceNext4(tokens, val1, val2, null, null);
        }

        public static bool Tokens_isSequenceNext3(TokenStream tokens, string val1, string val2, string val3)
        {
            return Tokens_isSequenceNext4(tokens, val1, val2, val3, null);
        }

        public static bool Tokens_isSequenceNext4(TokenStream tokens, string val1, string val2, string val3, string val4)
        {
            if (val1 != null && tokens.index < tokens.length && tokens.tokens[tokens.index].Value != val1)
            {
                return false;
            }
            if (val2 != null && tokens.index + 1 < tokens.length && tokens.tokens[tokens.index + 1].Value != val2)
            {
                return false;
            }
            if (val3 != null && tokens.index + 2 < tokens.length && tokens.tokens[tokens.index + 2].Value != val3)
            {
                return false;
            }
            if (val4 != null && tokens.index + 3 < tokens.length && tokens.tokens[tokens.index + 3].Value != val4)
            {
                return false;
            }
            return true;
        }

        public static Token Tokens_peek(TokenStream tokens)
        {
            if (tokens.index >= tokens.length)
            {
                return null;
            }
            return tokens.tokens[tokens.index];
        }

        public static Token Tokens_peekAhead(TokenStream tokens, int distance)
        {
            if (tokens.index + distance < tokens.length)
            {
                return tokens.tokens[tokens.index + distance];
            }
            return null;
        }

        public static int Tokens_peekType(TokenStream tokens)
        {
            if (!Tokens_hasMore(tokens))
            {
                return 9;
            }
            return Tokens_peek(tokens).Type;
        }

        public static string Tokens_peekValue(TokenStream tokens)
        {
            Token t = Tokens_peek(tokens);
            if (t == null)
            {
                return null;
            }
            return t.Value;
        }

        public static string Tokens_peekValueNonNull(TokenStream tokens)
        {
            string v = Tokens_peekValue(tokens);
            if (v == null)
            {
                return "";
            }
            return v;
        }

        public static Token Tokens_pop(TokenStream tokens)
        {
            if (tokens.index >= tokens.length)
            {
                return null;
            }
            Token t = tokens.tokens[tokens.index];
            tokens.index += 1;
            return t;
        }

        public static Token Tokens_popExpected(TokenStream tokens, string value)
        {
            Token output = Tokens_pop(tokens);
            if (output == null)
            {
                Tokens_ensureMore(tokens);
            }
            if (output.Value != value)
            {
                Errors_Throw(output, string.Join("", new string[] { "Expected '", value, "' but found '", output.Value, "' instead." }));
            }
            if (output.Type == 1)
            {
                fail("Use popKeyword instead.");
            }
            return output;
        }

        public static bool Tokens_popIfPresent(TokenStream tokens, string value)
        {
            if (Tokens_peekValue(tokens) == value)
            {
                tokens.index += 1;
                return true;
            }
            return false;
        }

        public static Token Tokens_popKeyword(TokenStream tokens, string value)
        {
            Token next = Tokens_pop(tokens);
            if (next == null)
            {
                Tokens_ensureMore(tokens);
            }
            if (next.Value != value || next.Type != 1)
            {
                Errors_Throw(next, string.Join("", new string[] { "Expected '", value, "' keyword but found '", next.Value, "' instead." }));
            }
            return next;
        }

        public static Token Tokens_popName(TokenStream tokens, string purposeForErrorMessage)
        {
            Token t = Tokens_pop(tokens);
            if (t == null)
            {
                Tokens_ensureMore(tokens);
            }
            if (t.Type != 2)
            {
                Errors_Throw(t, string.Join("", new string[] { "Expected ", purposeForErrorMessage, " but found '", t.Value, "' instead." }));
            }
            return t;
        }

        public static TokenStream TokenStream_new(string file, Token[] tokens)
        {
            return new TokenStream(0, tokens.Length, tokens, file);
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
