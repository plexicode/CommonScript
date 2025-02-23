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

        private static string PST_FloatToString(double value)
        {
            string output = value.ToString();
            if (output[0] == '.') output = "0" + output;
            if (!output.Contains('.')) output += ".0";
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

        public static void PST_ParseFloat(string strValue, double[] output)
        {
            double num = 0.0;
            output[0] = double.TryParse(strValue, out num) ? 1 : -1;
            output[1] = num;
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

        public static int[] bsbFlatten(ByteStringBuilder sbs)
        {
            System.Collections.Generic.List<ByteStringBuilder> q = new List<ByteStringBuilder>();
            q.Add(sbs);
            System.Collections.Generic.List<int> output = new List<int>();
            while (q.Count > 0)
            {
                int lastIndex = q.Count - 1;
                ByteStringBuilder current = q[lastIndex];
                q.RemoveAt(lastIndex);
                if (current.isLeaf)
                {
                    int[] currentBytes = current.bytes;
                    int length = currentBytes.Length;
                    int i = 0;
                    while (i < length)
                    {
                        output.Add(currentBytes[i]);
                        i += 1;
                    }
                }
                else
                {
                    q.Add(current.right);
                    q.Add(current.left);
                }
            }
            return output.ToArray();
        }

        public static ByteStringBuilder bsbFrom4Bytes(int a, int b, int c, int d)
        {
            int[] arr = new int[4];
            arr[0] = a;
            arr[1] = b;
            arr[2] = c;
            arr[3] = d;
            return bsbFromBytes(arr);
        }

        public static ByteStringBuilder bsbFromBytes(int[] bytes)
        {
            return new ByteStringBuilder(true, bytes.Length, bytes, null, null);
        }

        public static ByteStringBuilder bsbFromInt(int value)
        {
            int[] buf = null;
            if (value >= 0 && value < 128)
            {
                buf = new int[1];
                buf[0] = (int)value;
            }
            else if (value == -2147483648)
            {
                buf = new int[1];
                buf[0] = 192;
            }
            else
            {
                int firstByte = 128;
                bool isNegative = value < 0;
                if (isNegative)
                {
                    value *= -1;
                    firstByte |= 16;
                }
                if (value < 256)
                {
                    firstByte |= 1;
                    buf = new int[2];
                    buf[0] = firstByte;
                    buf[1] = (int)(value & 255);
                }
                else if (value <= 65535)
                {
                    firstByte |= 2;
                    buf = new int[3];
                    buf[0] = firstByte;
                    buf[1] = (int)(value >> 8 & 255);
                    buf[2] = (int)(value & 255);
                }
                else if (value <= 16777215)
                {
                    firstByte |= 3;
                    buf = new int[4];
                    buf[0] = firstByte;
                    buf[1] = (int)(value >> 16 & 255);
                    buf[2] = (int)(value >> 8 & 255);
                    buf[3] = (int)(value & 255);
                }
                else if (value <= 2147483647)
                {
                    firstByte |= 4;
                    buf = new int[5];
                    buf[0] = firstByte;
                    buf[1] = (int)(value >> 24 & 255);
                    buf[2] = (int)(value >> 16 & 255);
                    buf[3] = (int)(value >> 8 & 255);
                    buf[4] = (int)(value & 255);
                }
                else
                {
                    fail("Not implemented");
                }
            }
            return new ByteStringBuilder(true, buf.Length, buf, null, null);
        }

        public static ByteStringBuilder bsbFromLenString(string value)
        {
            ByteStringBuilder payload = bsbFromUtf8String(value);
            return bsbJoin2(bsbFromInt(payload.length), payload);
        }

        public static ByteStringBuilder bsbFromUtf8String(string value)
        {
            int[] bytes = PST_stringToUtf8Bytes(value);
            return new ByteStringBuilder(true, bytes.Length, bytes, null, null);
        }

        public static ByteStringBuilder bsbJoin2(ByteStringBuilder a, ByteStringBuilder b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            return new ByteStringBuilder(false, a.length + b.length, null, a, b);
        }

        public static ByteStringBuilder bsbJoin3(ByteStringBuilder a, ByteStringBuilder b, ByteStringBuilder c)
        {
            return bsbJoin2(bsbJoin2(a, b), c);
        }

        public static ByteStringBuilder bsbJoin4(ByteStringBuilder a, ByteStringBuilder b, ByteStringBuilder c, ByteStringBuilder d)
        {
            return bsbJoin2(bsbJoin2(a, b), bsbJoin2(c, d));
        }

        public static ByteStringBuilder bsbJoin5(ByteStringBuilder a, ByteStringBuilder b, ByteStringBuilder c, ByteStringBuilder d, ByteStringBuilder e)
        {
            return bsbJoin3(bsbJoin2(a, b), bsbJoin2(c, d), e);
        }

        public static ByteStringBuilder bsbJoin8(ByteStringBuilder a, ByteStringBuilder b, ByteStringBuilder c, ByteStringBuilder d, ByteStringBuilder e, ByteStringBuilder f, ByteStringBuilder g, ByteStringBuilder h)
        {
            return bsbJoin2(bsbJoin4(a, b, c, d), bsbJoin4(e, f, g, h));
        }

        public static BundleClassInfo BundleClassInfo_new(int classId, int parentId, string name, int ctorId, int staticCtorId, System.Collections.Generic.Dictionary<string, int> methodsToId, string[] newDirectMembersByNextOffsets, System.Collections.Generic.List<string> staticMethods, System.Collections.Generic.List<string> staticFields)
        {
            return new BundleClassInfo(classId, parentId, name, ctorId, staticCtorId, methodsToId, newDirectMembersByNextOffsets, staticMethods, staticFields);
        }

        public static BundleEnumInfo BundleEnumInfo_createFromEntity(EnumEntity e)
        {
            int sz = e.memberValues.Length;
            string[] names = new string[sz];
            int[] values = new int[sz];
            int i = 0;
            while (i < sz)
            {
                names[i] = e.memberNameTokens[i].Value;
                values[i] = e.memberValues[i].intVal;
                i += 1;
            }
            return new BundleEnumInfo(e.baseData.serializationIndex, names, values);
        }

        public static BundleFunctionInfo BundleFunctionInfo_new(System.Collections.Generic.List<ByteCodeRow> code, int argcMin, int argcMax, string name)
        {
            return new BundleFunctionInfo(code.ToArray(), argcMin, argcMax, name);
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
            ClassEntity cd = new ClassEntity(null, null, null, null, null, new Dictionary<string, AbstractEntity>(), null);
            cd.baseData = AbstractEntity_new(classToken, 1, cd);
            cd.baseData.nameToken = nameToken;
            cd.baseData.simpleName = nameToken.Value;
            cd.baseData.fqName = fqName;
            return cd;
        }

        public static int ClassSorter_calcDepth(ClassEntity cls, System.Collections.Generic.Dictionary<string, int> depthByName)
        {
            string fqName = cls.baseData.fqName;
            if (depthByName.ContainsKey(fqName))
            {
                return depthByName[fqName];
            }
            if (cls.baseClassEntity == null)
            {
                depthByName[fqName] = 1;
                return 1;
            }
            int depth = ClassSorter_calcDepth(cls.baseClassEntity, depthByName) + 1;
            depthByName[fqName] = depth;
            return depth;
        }

        public static CompilationBundle CompilationBundle_new()
        {
            CompilationBundle b = new CompilationBundle(null, null, null, null, null, null, null, 0, 0);
            b.byteCodeById = new List<ByteCodeRow[]>();
            b.byteCodeById.Add(null);
            b.functionById = new List<BundleFunctionInfo>();
            b.functionById.Add(null);
            b.classById = new List<BundleClassInfo>();
            b.classById.Add(null);
            b.enumById = new List<BundleEnumInfo>();
            b.enumById.Add(null);
            b.lambdaById = new List<BundleFunctionInfo>();
            b.lambdaById.Add(null);
            return b;
        }

        public static void CompiledModule_AddLambdas(CompiledModule m, System.Collections.Generic.List<FunctionEntity> lambdas)
        {
            m.lambdaEntities = new List<AbstractEntity>();
            int i = 0;
            while (i < lambdas.Count)
            {
                m.lambdaEntities.Add(lambdas[i].baseData);
                i += 1;
            }
        }

        public static void CompiledModule_InitializeLookups(CompiledModule m, System.Collections.Generic.Dictionary<string, AbstractEntity> rootEntities, System.Collections.Generic.Dictionary<string, AbstractEntity> flatEntities)
        {
            m.nestedEntities = rootEntities;
            m.flattenedEntities = flatEntities;
            m.entitiesNoEnumParents = new Dictionary<string, AbstractEntity>();
            string[] fqNames = m.flattenedEntities.Keys.ToArray();
            int i = 0;
            while (i < fqNames.Length)
            {
                string fqName = fqNames[i];
                AbstractEntity entity = m.flattenedEntities[fqName];
                if (entity.type == 4)
                {
                    Token[] enumMemberNameTokens = ((EnumEntity)entity.specificData).memberNameTokens;
                    int j = 0;
                    while (j < enumMemberNameTokens.Length)
                    {
                        Token enumMem = enumMemberNameTokens[j];
                        m.entitiesNoEnumParents[string.Join("", new string[] { fqName, ".", enumMem.Value })] = entity;
                        j += 1;
                    }
                }
                else
                {
                    m.entitiesNoEnumParents[fqName] = entity;
                }
                i += 1;
            }
        }

        public static CompiledModule CompiledModule_new(string id)
        {
            return new CompiledModule(id, new Dictionary<string, string>(), null, null, null, null);
        }

        public static string[] CompilerContext_CalculateCompilationOrder(CompilerContext compiler)
        {
            System.Collections.Generic.Dictionary<string, int> recurseState = new Dictionary<string, int>();
            System.Collections.Generic.List<string> order = new List<string>();
            System.Collections.Generic.List<string> queue = new List<string>();
            queue.Add(compiler.rootId);
            while (queue.Count > 0)
            {
                int last = queue.Count - 1;
                string currentId = queue[last];
                queue.RemoveAt(last);
                int currentRecurseState = 0;
                if (recurseState.ContainsKey(currentId))
                {
                    currentRecurseState = recurseState[currentId];
                }
                if (currentRecurseState == 2)
                {
                }
                else
                {
                    System.Collections.Generic.List<string> deps = compiler.depIdsByModuleId[currentId];
                    System.Collections.Generic.List<string> newDeps = new List<string>();
                    int i = 0;
                    while (i < deps.Count)
                    {
                        string depId = deps[i];
                        if (recurseState.ContainsKey(depId))
                        {
                            if (recurseState[depId] == 2)
                            {
                            }
                            else if (recurseState[depId] == 1)
                            {
                                fail(string.Join("", new string[] { "There is a cyclical dependency involving ", depId, " and ", currentId }));
                            }
                        }
                        else
                        {
                            newDeps.Add(depId);
                        }
                        i += 1;
                    }
                    if (newDeps.Count == 0)
                    {
                        recurseState[currentId] = 2;
                        order.Add(currentId);
                    }
                    else
                    {
                        recurseState[currentId] = 1;
                        queue.Add(currentId);
                        i = 0;
                        while (i < newDeps.Count)
                        {
                            queue.Add(newDeps[i]);
                            i += 1;
                        }
                    }
                }
            }
            return order.ToArray();
        }

        public static CompilerContext CompilerContext_new(string rootId, string flavorId, string extensionVersionId, string[] extensionNames)
        {
            CompilerContext ctx = new CompilerContext(StaticContext_new(), rootId, new Dictionary<string, System.Collections.Generic.List<string>>(), new Dictionary<string, System.Collections.Generic.List<FileContext>>(), null, new Dictionary<string, bool>(), null, extensionVersionId, flavorId, new List<string>());
            int i = 0;
            while (i < extensionNames.Length)
            {
                ctx.extensionNames.Add(extensionNames[i]);
                i += 1;
            }
            ctx.unfulfilledDependencies[rootId] = true;
            System.Collections.Generic.Dictionary<string, string> builtinFiles = new Dictionary<string, string>();
            builtinFiles["builtins.script"] = GetSourceForBuiltinModule("builtins");
            PUBLIC_SupplyFilesForModule(ctx, "{BUILTIN}", builtinFiles, true, true);
            return ctx;
        }

        public static ConstEntity ConstEntity_new(Token constToken, Token nameToken, Expression constValue)
        {
            ConstEntity c = new ConstEntity(constValue, null);
            c.baseData = AbstractEntity_new(constToken, 2, c);
            c.baseData.nameToken = nameToken;
            c.baseData.simpleName = nameToken.Value;
            return c;
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

        public static EnumEntity EnumEntity_new(Token enumToken, Token nameToken, Token[] memberNames, Expression[] memberValues)
        {
            EnumEntity e = new EnumEntity(memberNames, memberValues, null);
            e.baseData = AbstractEntity_new(enumToken, 4, e);
            e.baseData.nameToken = nameToken;
            e.baseData.simpleName = nameToken.Value;
            if (memberNames.Length == 0)
            {
                Errors_Throw(enumToken, "This enum definition is empty.");
            }
            System.Collections.Generic.Dictionary<string, bool> collisionCheck = new Dictionary<string, bool>();
            bool isImplicit = memberValues[0] == null;
            int i = 0;
            while (i < memberNames.Length)
            {
                Token name = memberNames[i];
                if (collisionCheck.ContainsKey(name.Value))
                {
                    Errors_Throw(name, "This enum value name collides with a previous definition.");
                }
                bool valueIsImplicit = memberValues[i] == null;
                if (valueIsImplicit != isImplicit)
                {
                    Errors_Throw(enumToken, "This enum definition defines values for some but not all members. Mixed implicit/explicit definitions are not allowed.");
                }
                if (isImplicit)
                {
                    e.memberValues[i] = Expression_createIntegerConstant(null, i + 1);
                }
                i += 1;
            }
            return e;
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

        public static int[] ExportUtil_exportBundle(string flavorId, string extVersionId, CompilationBundle bundle)
        {
            int i = 0;
            int j = 0;
            ByteStringBuilder flavor = bsbFromLenString(flavorId);
            ByteStringBuilder version = bsbFromLenString(extVersionId);
            int commonScriptMajor = 0;
            int commonScriptMinor = 1;
            int commonScriptPatch = 0;
            ByteStringBuilder header = bsbJoin4(bsbFromUtf8String("PXCS"), bsbFrom4Bytes(0, commonScriptMajor, commonScriptMinor, commonScriptPatch), flavor, version);
            ByteStringBuilder metadata = bsbJoin3(bsbFromUtf8String("MTD"), bsbFromInt(bundle.mainFunctionId), bsbFromInt(bundle.builtInCount));
            ByteStringBuilder tokenData = bsbJoin2(bsbFromUtf8String("TOK"), bsbFromInt(bundle.tokensById.Length - 1));
            System.Collections.Generic.Dictionary<string, int> fileNameToOffset = new Dictionary<string, int>();
            ByteStringBuilder tokenFileNames = null;
            i = 1;
            while (i < bundle.tokensById.Length)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                if (!fileNameToOffset.ContainsKey(filename))
                {
                    fileNameToOffset[filename] = fileNameToOffset.Count;
                    tokenFileNames = bsbJoin2(tokenFileNames, bsbFromLenString(filename));
                }
                i += 1;
            }
            tokenData = bsbJoin3(tokenData, bsbFromInt(fileNameToOffset.Count), tokenFileNames);
            i = 1;
            while (i < bundle.tokensById.Length)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                int fileOffset = fileNameToOffset[filename];
                tokenData = bsbJoin4(tokenData, bsbFromInt(fileOffset), bsbFromInt(tok.Line), bsbFromInt(tok.Col));
                i += 1;
            }
            ByteStringBuilder stringData = bsbJoin2(bsbFromUtf8String("STR"), bsbFromInt(bundle.stringById.Length - 1));
            i = 1;
            while (i < bundle.stringById.Length)
            {
                string val = bundle.stringById[i];
                stringData = bsbJoin2(stringData, bsbFromLenString(val));
                i += 1;
            }
            ByteStringBuilder entityAcc = null;
            i = 1;
            while (i < bundle.functionById.Count)
            {
                BundleFunctionInfo fn = bundle.functionById[i];
                entityAcc = bsbJoin2(entityAcc, bsbJoin5(bsbFromInt(fn.argcMin), bsbFromInt(fn.argcMax), bsbFromLenString(fn.name), bsbFromInt(fn.code.Length), ExportUtil_exportCode(fn.code)));
                i += 1;
            }
            i = 1;
            while (i < bundle.lambdaById.Count)
            {
                BundleFunctionInfo fn = bundle.lambdaById[i];
                entityAcc = bsbJoin2(entityAcc, bsbJoin4(bsbFromInt(fn.argcMin), bsbFromInt(fn.argcMax), bsbFromInt(fn.code.Length), ExportUtil_exportCode(fn.code)));
                i += 1;
            }
            i = 1;
            while (i < bundle.enumById.Count)
            {
                BundleEnumInfo bei = bundle.enumById[i];
                int memberCount = bei.names.Length;
                entityAcc = bsbJoin2(entityAcc, bsbFromInt(memberCount));
                j = 0;
                while (j < memberCount)
                {
                    entityAcc = bsbJoin3(entityAcc, bsbFromLenString(bei.names[j]), bsbFromInt(bei.values[j]));
                    j++;
                }
                i += 1;
            }
            i = 1;
            while (i < bundle.classById.Count)
            {
                BundleClassInfo bci = bundle.classById[i];
                ByteStringBuilder classInfo = bsbJoin8(bsbFromLenString(bci.name), bsbFromInt(bci.parentId), bsbFromInt(bci.ctorId), bsbFromInt(bci.staticCtorId), bsbFromInt(bci.newDirectMembersByNextOffsets.Length), bsbFromInt(bci.methodsToId.Count), bsbFromInt(bci.staticFields.Count), bsbFromInt(bci.staticMethods.Count));
                j = 0;
                while (j < bci.newDirectMembersByNextOffsets.Length)
                {
                    string memberName = bci.newDirectMembersByNextOffsets[j];
                    bool isMethod = bci.methodsToId.ContainsKey(memberName);
                    int info = 0;
                    if (isMethod)
                    {
                        info = 1;
                    }
                    classInfo = bsbJoin3(classInfo, bsbFromLenString(memberName), bsbFromInt(info));
                    j += 1;
                }
                ByteStringBuilder methodInfo = null;
                string[] methodNames = bci.methodsToId.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                j = 0;
                while (j < methodNames.Length)
                {
                    string methodName = methodNames[j];
                    methodInfo = bsbJoin3(methodInfo, bsbFromLenString(methodName), bsbFromInt(bci.methodsToId[methodName]));
                    j += 1;
                }
                ByteStringBuilder staticFields = null;
                string[] fieldNames = bci.staticFields.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                j = 0;
                while (j < fieldNames.Length)
                {
                    string staticField = fieldNames[j];
                    staticFields = bsbJoin2(staticFields, bsbFromLenString(staticField));
                    j += 1;
                }
                ByteStringBuilder staticMethods = null;
                string[] staticMethodNames = bci.staticMethods.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                j = 0;
                while (j < staticMethodNames.Length)
                {
                    string staticMethod = staticMethodNames[j];
                    int funcId = bci.methodsToId[staticMethod];
                    staticMethods = bsbJoin3(staticMethods, bsbFromLenString(staticMethod), bsbFromInt(funcId));
                    j += 1;
                }
                entityAcc = bsbJoin5(entityAcc, classInfo, methodInfo, staticFields, staticMethods);
                i += 1;
            }
            ByteStringBuilder entityHeader = bsbJoin5(bsbFromUtf8String("ENT"), bsbFromInt(bundle.functionById.Count - 1), bsbFromInt(bundle.enumById.Count - 1), bsbFromInt(bundle.classById.Count - 1), bsbFromInt(bundle.lambdaById.Count - 1));
            ByteStringBuilder entityData = bsbJoin2(entityHeader, entityAcc);
            ByteStringBuilder full = bsbJoin5(header, metadata, stringData, tokenData, entityData);
            return bsbFlatten(full);
        }

        public static ByteStringBuilder ExportUtil_exportCode(ByteCodeRow[] rows)
        {
            ByteStringBuilder bsb = null;
            int i = 0;
            while (i < rows.Length)
            {
                ByteCodeRow row = rows[i];
                int[] args = row.args;
                int argsLen = args.Length;
                int flags = argsLen * 4;
                ByteStringBuilder bsbStringArg = null;
                ByteStringBuilder bsbToken = null;
                if (row.stringArg != null)
                {
                    flags += 1;
                    bsbStringArg = bsbFromInt(row.stringId);
                }
                if (row.token != null)
                {
                    flags += 2;
                    bsbToken = bsbFromInt(row.tokenId);
                }
                ByteStringBuilder bsbRow = bsbJoin4(bsbFromInt(row.opCode), bsbFromInt(flags), bsbStringArg, bsbToken);
                int j = 0;
                while (j < argsLen)
                {
                    bsbRow = bsbJoin2(bsbRow, bsbFromInt(args[j]));
                    j += 1;
                }
                bsb = bsbJoin2(bsb, bsbRow);
                i += 1;
            }
            return bsb;
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

        public static FieldEntity FieldEntity_new(Token fieldToken, Token nameToken, Token equalToken, Expression defaultValueOrNull)
        {
            FieldEntity fe = new FieldEntity(defaultValueOrNull, equalToken, null);
            fe.baseData = AbstractEntity_new(fieldToken, 5, fe);
            fe.baseData.nameToken = nameToken;
            fe.baseData.simpleName = nameToken.Value;
            return fe;
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

        public static ByteCodeBuffer finalizeBreakContinue(ByteCodeBuffer originalBuffer, int additionalBreakOffset, bool allowContinue, int additionalContinueOffset)
        {
            int i = 0;
            ByteCodeRow[] rows = flatten(originalBuffer);
            i = 0;
            while (i < rows.Length)
            {
                int op = rows[i].opCode;
                if (op == -1 || op == -2)
                {
                    int additionalOffset = additionalBreakOffset;
                    if (op == -2)
                    {
                        if (!allowContinue)
                        {
                            fail("");
                        }
                        additionalOffset = additionalContinueOffset;
                    }
                    rows[i].opCode = 24;
                    int offsetToEnd = rows.Length - i - 1 + additionalOffset;
                    rows[i].args = new int[1];
                    rows[i].args[0] = offsetToEnd;
                }
                i += 1;
            }
            ByteCodeBuffer output = null;
            i = 0;
            while (i < rows.Length)
            {
                output = join2(output, ByteCodeBuffer_fromRow(rows[i]));
                i += 1;
            }
            return output;
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

        public static AbstractEntity[] FlattenEntities(StaticContext staticCtx, System.Collections.Generic.Dictionary<string, AbstractEntity> rootEntities)
        {
            System.Collections.Generic.List<AbstractEntity> output = new List<AbstractEntity>();
            System.Collections.Generic.List<AbstractEntity> queue = new List<AbstractEntity>();
            AbstractEntity[] arrTemp = rootEntities.Values.ToArray();
            int j = 0;
            while (j < arrTemp.Length)
            {
                queue.Add(arrTemp[j]);
                j += 1;
            }
            int i = 0;
            while (i < queue.Count)
            {
                AbstractEntity entity = queue[i];
                output.Add(entity);
                AbstractEntity[] lookupMembers = Entity_getMemberLookup(staticCtx, entity).Values.ToArray();
                j = 0;
                while (j < lookupMembers.Length)
                {
                    AbstractEntity mem = lookupMembers[j];
                    queue.Add(mem);
                    j += 1;
                }
                i += 1;
            }
            return output.ToArray();
        }

        public static string FloatToStringWorkaround(double val)
        {
            string str = PST_FloatToString(val);
            str = str.ToLower();
            if (str.Contains("e") && str.EndsWith(".0"))
            {
                str = str.Substring(0, str.Length - 2).Replace("e", ".0e");
            }
            return str;
        }

        public static FunctionEntity FunctionEntity_BuildConstructor(Token ctorToken, System.Collections.Generic.List<Token> args, System.Collections.Generic.List<Expression> argDefaultValues, System.Collections.Generic.List<Expression> baseArgs, System.Collections.Generic.List<Statement> code, bool isStatic)
        {
            FunctionEntity fle = FunctionEntity_new(ctorToken, 3, args, argDefaultValues, code);
            if (isStatic)
            {
                fle.FunctionSubtype = 5;
                fle.baseData.simpleName = "@cctor";
            }
            else
            {
                fle.FunctionSubtype = 3;
                fle.baseData.simpleName = "@ctor";
            }
            if (baseArgs != null)
            {
                fle.baseCtorArgValues = baseArgs.ToArray();
            }
            fle.baseData.isStatic = isStatic;
            return fle;
        }

        public static FunctionEntity FunctionEntity_BuildLambda(FileContext ctx, Token firstToken, System.Collections.Generic.List<Token> argNames, System.Collections.Generic.List<Expression> argDefaultValues, System.Collections.Generic.List<Statement> code)
        {
            FunctionEntity fle = FunctionEntity_new(firstToken, 10, argNames, argDefaultValues, code);
            fle.FunctionSubtype = 6;
            fle.baseData.fileContext = ctx;
            return fle;
        }

        public static FunctionEntity FunctionEntity_BuildMethodOrStandalone(Token funcToken, Token nameToken, System.Collections.Generic.List<Token> args, System.Collections.Generic.List<Expression> argValues, System.Collections.Generic.List<Statement> code, bool isStatic, ClassEntity classParent)
        {
            bool isMethod = classParent != null;
            FunctionEntity fle = FunctionEntity_new(funcToken, 6, args, argValues, code);
            if (isMethod)
            {
                if (isStatic)
                {
                    fle.FunctionSubtype = 4;
                }
                else
                {
                    fle.FunctionSubtype = 2;
                }
            }
            else
            {
                fle.FunctionSubtype = 1;
            }
            fle.baseData.nameToken = nameToken;
            fle.baseData.simpleName = nameToken.Value;
            return fle;
        }

        public static FunctionEntity FunctionEntity_new(Token firstToken, int type, System.Collections.Generic.List<Token> argNames, System.Collections.Generic.List<Expression> argDefaultValues, System.Collections.Generic.List<Statement> code)
        {
            FunctionEntity fn = new FunctionEntity(argNames.ToArray(), argDefaultValues.ToArray(), null, null, code.ToArray(), null, -1);
            fn.baseData = AbstractEntity_new(firstToken, type, fn);
            return fn;
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

        public static string GetSourceForBuiltinModule(string moduleName)
        {
            string code = GetBuiltinRawStoredString(moduleName);
            if (code == null)
            {
                fail(moduleName);
                return null;
            }
            return code.Replace("@1", "function ").Replace("@2", " { constructor(m=null):base(m){} }").Replace("@3", "@public class ").Replace("@4", "return ").Replace("@5", "@public function ").Replace("@6", "Exception : Exception");
        }

        public static ImportStatement[] ImportParser_AdvanceThroughImports(TokenStream tokens, bool isCoreBuiltin)
        {
            System.Collections.Generic.List<ImportStatement> output = new List<ImportStatement>();
            if (!isCoreBuiltin)
            {
                output.Add(ImportParser_createBuiltinImport(tokens));
            }
            while (Tokens_hasMore(tokens) && Tokens_isNext(tokens, "import"))
            {
                Token importToken = Tokens_popKeyword(tokens, "import");
                System.Collections.Generic.List<Token> tokenChain = new List<Token>();
                tokenChain.Add(Tokens_popName(tokens, "module name"));
                while (Tokens_popIfPresent(tokens, "."))
                {
                    tokenChain.Add(Tokens_popName(tokens, "module name"));
                }
                Token importTargetName = null;
                if (Tokens_popIfPresent(tokens, "->"))
                {
                    if (Tokens_isNext(tokens, "*"))
                    {
                        importTargetName = Tokens_pop(tokens);
                    }
                    else
                    {
                        importTargetName = Tokens_popName(tokens, "import target variable");
                    }
                }
                Tokens_popExpected(tokens, ";");
                output.Add(ImportStatement_new(importToken, tokenChain, importTargetName));
            }
            return output.ToArray();
        }

        public static ImportStatement ImportParser_createBuiltinImport(TokenStream tokens)
        {
            System.Collections.Generic.List<Token> builtinName = new List<Token>();
            builtinName.Add(createFakeToken(tokens, 2, "{BUILTIN}", 0, 0));
            return ImportStatement_new(createFakeToken(tokens, 1, "import", 0, 0), builtinName, createFakeToken(tokens, 3, "*", 0, 0));
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

        public static bool IsBuiltInModule(string moduleId)
        {
            return moduleId != "builtins" && GetBuiltinRawStoredString(moduleId) != null;
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

        public static string[] OrderStringsByDescendingFrequencyUsingLookup(System.Collections.Generic.Dictionary<string, int> frequencyLookupByKey)
        {
            int total = 0;
            int i = 0;
            string[] values = frequencyLookupByKey.Keys.ToArray();
            i = 0;
            while (i < values.Length)
            {
                total += frequencyLookupByKey[values[i]];
                i += 1;
            }
            int padSize = total.ToString().Length + 1;
            System.Collections.Generic.Dictionary<string, string> valueByLexicalSortKey = new Dictionary<string, string>();
            i = 0;
            while (i < values.Length)
            {
                string value = values[i];
                string key = string.Join("", new string[] { PadIntegerToSize(total - frequencyLookupByKey[value], padSize), ":", value });
                valueByLexicalSortKey[key] = value;
                i += 1;
            }
            string[] keys = valueByLexicalSortKey.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
            System.Collections.Generic.List<string> output = new List<string>();
            i = 0;
            while (i < keys.Length)
            {
                output.Add(valueByLexicalSortKey[keys[i]]);
                i += 1;
            }
            return output.ToArray();
        }

        public static string PadIntegerToSize(int n, int size)
        {
            string o = n.ToString();
            while (o.Length < size)
            {
                o = "0" + o;
            }
            return o;
        }

        public static void PUBLIC_EnsureDependenciesFulfilled(object compObj)
        {
            CompilerContext compiler = (CompilerContext)compObj;
            if (compiler.unfulfilledDependencies.Count > 0)
            {
                fail("Not all dependencies are fulfilled.");
            }
        }

        public static string PUBLIC_GetNextRequiredModuleId(object compObj)
        {
            CompilerContext compiler = (CompilerContext)compObj;
            while (true)
            {
                if (compiler.unfulfilledDependencies.Count == 0)
                {
                    return null;
                }
                string[] unfulfilledDependencies = compiler.unfulfilledDependencies.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                string nextKey = unfulfilledDependencies[0];
                if (!IsBuiltInModule(nextKey))
                {
                    return nextKey;
                }
                System.Collections.Generic.Dictionary<string, string> builtinFiles = new Dictionary<string, string>();
                builtinFiles[nextKey + ".script"] = GetSourceForBuiltinModule(nextKey);
                PUBLIC_SupplyFilesForModule(compiler, nextKey, builtinFiles, false, true);
            }
        }

        public static void PUBLIC_SupplyFilesForModule(object compObj, string moduleId, System.Collections.Generic.Dictionary<string, string> fileLookup, bool isCoreBuiltin, bool isBuiltInLib)
        {
            int i = 0;
            int j = 0;
            CompilerContext compiler = (CompilerContext)compObj;
            compiler.depIdsByModuleId[moduleId] = new List<string>();
            System.Collections.Generic.List<FileContext> files = new List<FileContext>();
            System.Collections.Generic.Dictionary<string, ImportStatement> imports = new Dictionary<string, ImportStatement>();
            string[] fileNamesOrdered = fileLookup.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
            i = 0;
            while (i < fileNamesOrdered.Length)
            {
                string path = fileNamesOrdered[i];
                FileContext fileCtx = FileContext_new(compiler.staticCtx, path, fileLookup[path]);
                fileCtx.isCoreBuiltin = isCoreBuiltin;
                fileCtx.isBuiltInLib = isBuiltInLib;
                files.Add(fileCtx);
                fileCtx.imports = ImportParser_AdvanceThroughImports(fileCtx.tokens, isCoreBuiltin);
                FileContext_initializeImportLookup(fileCtx);
                j = 0;
                while (j < fileCtx.imports.Length)
                {
                    ImportStatement impStmnt = fileCtx.imports[j];
                    imports[impStmnt.flatName] = impStmnt;
                    j += 1;
                }
                i += 1;
            }
            compiler.filesByModuleId[moduleId] = files;
            if (compiler.unfulfilledDependencies.ContainsKey(moduleId))
            {
                compiler.unfulfilledDependencies.Remove(moduleId);
            }
            System.Collections.Generic.List<string> allDeps = new List<string>();
            string[] importedIds = imports.Keys.ToArray();
            i = 0;
            while (i < importedIds.Length)
            {
                allDeps.Add(importedIds[i]);
                i += 1;
            }
            compiler.depIdsByModuleId[moduleId] = allDeps;
            i = 0;
            while (i < allDeps.Count)
            {
                string depId = allDeps[i];
                if (!compiler.filesByModuleId.ContainsKey(depId))
                {
                    compiler.unfulfilledDependencies[depId] = true;
                }
                i += 1;
            }
        }

        public static Resolver Resolver_new(StaticContext staticCtx, System.Collections.Generic.Dictionary<string, AbstractEntity> rootEntities, System.Collections.Generic.List<string> extensionNames)
        {
            Resolver r = new Resolver(staticCtx, rootEntities, new Dictionary<string, AbstractEntity>(), new Dictionary<string, AbstractEntity>(), new Dictionary<string, AbstractEntity>(), new Dictionary<string, AbstractEntity>(), new List<FunctionEntity>(), null, null, null, 0, StringSet_fromList(extensionNames));
            r.entityList = FlattenEntities(staticCtx, rootEntities);
            int i = 0;
            while (i < r.entityList.Length)
            {
                AbstractEntity tle = r.entityList[i];
                r.flattenedEntities[tle.fqName] = tle;
                r.flattenedEntitiesAndEnumValues[tle.fqName] = tle;
                if (tle.type == 4)
                {
                    EnumEntity enumDef = (EnumEntity)tle.specificData;
                    int j = 0;
                    while (j < enumDef.memberNameTokens.Length)
                    {
                        string fqName = string.Join("", new string[] { enumDef.baseData.fqName, ".", enumDef.memberNameTokens[j].Value });
                        r.enumsByMemberFqName[fqName] = enumDef.baseData;
                        r.flattenedEntitiesAndEnumValues[fqName] = enumDef.baseData;
                        r.flattenedEntitiesNoEnumParents[fqName] = enumDef.baseData;
                        j++;
                    }
                }
                else
                {
                    r.flattenedEntitiesNoEnumParents[tle.fqName] = tle;
                }
                i += 1;
            }
            return r;
        }

        public static ByteCodeBuffer serializeAssignField(StaticContext staticCtx, Statement assignField, string baseOp)
        {
            Expression df = assignField.assignTarget;
            string fieldName = df.strVal;
            ByteCodeBuffer bufVal = serializeExpression(staticCtx, assignField.assignValue);
            ByteCodeBuffer bufRoot = serializeExpression(staticCtx, df.root);
            if (baseOp != "=")
            {
                ByteCodeBuffer incrBuf = join2(bufRoot, create0(52, null, null));
                incrBuf = join2(incrBuf, create0(15, df.opToken, fieldName));
                incrBuf = join3(incrBuf, bufVal, create0(4, assignField.assignOp, baseOp));
                return join2(incrBuf, create1(1, assignField.assignOp, fieldName, 1));
            }
            return join3(bufVal, bufRoot, create0(1, assignField.assignOp, fieldName));
        }

        public static ByteCodeBuffer serializeAssignIndex(StaticContext staticCtx, Statement assignIndex, string baseOp)
        {
            Token bracketToken = assignIndex.assignTarget.opToken;
            ByteCodeBuffer bufVal = serializeExpression(staticCtx, assignIndex.assignValue);
            ByteCodeBuffer bufIndex = serializeExpression(staticCtx, assignIndex.assignTarget.right);
            ByteCodeBuffer bufRoot = serializeExpression(staticCtx, assignIndex.assignTarget.root);
            if (baseOp != "=")
            {
                ByteCodeBuffer incrBuf = join3(bufRoot, bufIndex, create0(53, null, null));
                incrBuf = join2(incrBuf, create0(22, bracketToken, null));
                incrBuf = join3(incrBuf, bufVal, create0(4, assignIndex.assignOp, baseOp));
                return join2(incrBuf, create1(2, assignIndex.assignOp, null, 1));
            }
            return join4(bufVal, bufRoot, bufIndex, create0(2, assignIndex.assignOp, null));
        }

        public static ByteCodeBuffer serializeAssignVariable(StaticContext staticCtx, Statement assignVar, string baseOp)
        {
            ByteCodeBuffer bufVal = serializeExpression(staticCtx, assignVar.assignValue);
            ByteCodeBuffer bufVar = serializeExpression(staticCtx, assignVar.assignTarget);
            if (baseOp != "=")
            {
                bufVal = join3(bufVar, bufVal, create0(4, assignVar.assignOp, baseOp));
            }
            return join2(bufVal, create0(3, assignVar.assignOp, assignVar.assignTarget.strVal));
        }

        public static ByteCodeBuffer serializeBaseCtorReference(Expression baseCtor)
        {
            AbstractEntity baseClass = baseCtor.entityPtr;
            return create1(35, baseCtor.firstToken, null, baseClass.serializationIndex);
        }

        public static ByteCodeBuffer serializeBinaryOp(StaticContext staticCtx, Expression binOp)
        {
            Token opToken = binOp.opToken;
            string op = opToken.Value;
            Expression left = binOp.left;
            Expression right = binOp.right;
            ByteCodeBuffer leftBuf = serializeExpression(staticCtx, left);
            ByteCodeBuffer rightBuf = serializeExpression(staticCtx, right);
            if (op == "??")
            {
                return join3(leftBuf, create1(31, opToken, null, rightBuf.length), rightBuf);
            }
            if (op == "&&" || op == "||")
            {
                leftBuf = ByteCodeUtil_ensureBooleanExpression(left.firstToken, leftBuf);
                rightBuf = ByteCodeUtil_ensureBooleanExpression(right.firstToken, rightBuf);
                int opCode = 30;
                if (op == "&&")
                {
                    opCode = 32;
                }
                return join3(leftBuf, create1(opCode, null, null, rightBuf.length), rightBuf);
            }
            return join3(leftBuf, rightBuf, create0(4, opToken, op));
        }

        public static ByteCodeBuffer serializeBitwiseNot(StaticContext staticCtx, Expression bwn)
        {
            return join2(ByteCodeUtil_ensureIntegerExpression(bwn.root.firstToken, serializeExpression(staticCtx, bwn.root)), create0(10, bwn.firstToken, null));
        }

        public static ByteCodeBuffer serializeBoolConst(Expression bc)
        {
            int boolVal = 0;
            if (bc.boolVal)
            {
                boolVal = 1;
            }
            return create1(36, bc.firstToken, null, boolVal);
        }

        public static ByteCodeBuffer serializeBooleanNot(StaticContext staticCtx, Expression bn)
        {
            return join2(serializeExpression(staticCtx, bn.root), create0(11, bn.firstToken, null));
        }

        public static ByteCodeBuffer serializeBreak(Statement br)
        {
            return create0(-1, br.firstToken, null);
        }

        public static ByteCodeBuffer serializeClassReference(Expression classRef)
        {
            return create1(37, classRef.firstToken, null, classRef.entityPtr.serializationIndex);
        }

        public static ByteCodeBuffer serializeCodeBlock(StaticContext staticCtx, Statement[] block)
        {
            ByteCodeBuffer buf = null;
            int i = 0;
            while (i < block.Length)
            {
                buf = join2(buf, serializeStatement(staticCtx, block[i]));
                i += 1;
            }
            return buf;
        }

        public static ByteCodeBuffer serializeConstructorInvocation(StaticContext staticCtx, Expression ctorInvoke)
        {
            AbstractEntity classDef = ctorInvoke.entityPtr;
            ByteCodeBuffer buf = null;
            int argc = ctorInvoke.args.Length;
            int i = 0;
            while (i < argc)
            {
                buf = join2(buf, serializeExpression(staticCtx, ctorInvoke.args[i]));
                i += 1;
            }
            return join3(create1(14, ctorInvoke.firstToken, null, classDef.serializationIndex), buf, create1(21, ctorInvoke.opToken, null, argc));
        }

        public static ByteCodeBuffer serializeContinue(Statement cont)
        {
            return create0(-2, cont.firstToken, null);
        }

        public static ByteCodeBuffer serializeDictionaryDefinition(StaticContext staticCtx, Expression dictDef)
        {
            int sz = dictDef.keys.Length;
            ByteCodeBuffer buf = null;
            int i = 0;
            while (i < sz)
            {
                buf = join3(buf, serializeExpression(staticCtx, dictDef.keys[i]), serializeExpression(staticCtx, dictDef.values[i]));
                i += 1;
            }
            return join2(buf, create1(13, dictDef.firstToken, null, sz));
        }

        public static ByteCodeBuffer serializeDotField(StaticContext staticCtx, Expression df)
        {
            return join2(serializeExpression(staticCtx, df.root), create0(15, df.opToken, df.strVal));
        }

        public static ByteCodeBuffer serializeDoWhileLoop(StaticContext staticCtx, Statement doWhileLoop)
        {
            ByteCodeBuffer body = serializeCodeBlock(staticCtx, doWhileLoop.code);
            ByteCodeBuffer condition = ByteCodeUtil_ensureBooleanExpression(doWhileLoop.condition.firstToken, serializeExpression(staticCtx, doWhileLoop.condition));
            return join4(finalizeBreakContinue(body, condition.length + 2, true, 0), condition, create1(28, null, null, 1), create1(24, null, null, -(2 + condition.length + body.length)));
        }

        public static ByteCodeBuffer serializeExpression(StaticContext staticCtx, Expression expr)
        {
            switch (expr.type)
            {
                case 15:
                    fail("");
                    return null;
                case 2:
                    return serializeBaseCtorReference(expr);
                case 3:
                    return serializeBinaryOp(staticCtx, expr);
                case 4:
                    return serializeBitwiseNot(staticCtx, expr);
                case 5:
                    return serializeBoolConst(expr);
                case 6:
                    return serializeBooleanNot(staticCtx, expr);
                case 7:
                    return serializeClassReference(expr);
                case 8:
                    return serializeConstructorInvocation(staticCtx, expr);
                case 10:
                    return serializeDictionaryDefinition(staticCtx, expr);
                case 11:
                    return serializeDotField(staticCtx, expr);
                case 14:
                    return serializeExtensionInvocation(staticCtx, expr);
                case 16:
                    return serializeFloatConstant(expr);
                case 17:
                    return serializeFunctionInvocation(staticCtx, expr);
                case 18:
                    return serializeFunctionReference(expr);
                case 20:
                    return serializeIndex(staticCtx, expr);
                case 21:
                    return serializeInlineIncrement(staticCtx, expr);
                case 22:
                    return serializeIntegerConstant(expr);
                case 23:
                    return serializeLambda(expr);
                case 24:
                    return serializeListDefinition(staticCtx, expr);
                case 25:
                    return serializeNegativeSign(staticCtx, expr);
                case 26:
                    return serializeNullConstant(expr);
                case 27:
                    return serializeSlice(staticCtx, expr);
                case 28:
                    return serializeStringConstant(expr);
                case 29:
                    return serializeTernary(staticCtx, expr);
                case 30:
                    return serializeThis(expr);
                case 33:
                    return serializeTypeOf(staticCtx, expr);
                case 31:
                    return serializeVariable(expr);
                default:
                    fail("Not implemented");
                    return null;
            }
        }

        public static ByteCodeBuffer serializeExpressionStatement(StaticContext staticCtx, Statement exprStmnt)
        {
            return join2(serializeExpression(staticCtx, exprStmnt.expression), create0(27, null, null));
        }

        public static ByteCodeBuffer serializeExtensionInvocation(StaticContext staticCtx, Expression extInvoke)
        {
            if (SpecialActionUtil_IsSpecialActionAndNotExtension(staticCtx.specialActionUtil, extInvoke.strVal))
            {
                return serializeSpecialAction(staticCtx, extInvoke);
            }
            ByteCodeBuffer buf = null;
            int argc = extInvoke.args.Length;
            int i = 0;
            while (i < argc)
            {
                buf = join2(buf, serializeExpression(staticCtx, extInvoke.args[i]));
                i++;
            }
            return join2(buf, create1(20, extInvoke.firstToken, extInvoke.strVal, argc));
        }

        public static ByteCodeBuffer serializeFloatConstant(Expression floatConst)
        {
            double val = floatConst.floatVal;
            if (val * 4 % 1 == 0)
            {
                return create1(38, null, null, (int)(val * 4));
            }
            return create0(38, null, FloatToStringWorkaround(val));
        }

        public static ByteCodeBuffer serializeForEachLoop(StaticContext staticCtx, Statement forEachLoop)
        {
            string loopExpr = "@fe" + forEachLoop.autoId.ToString();
            string iteratorVar = "@fi" + forEachLoop.autoId.ToString();
            ByteCodeBuffer setup = join5(serializeExpression(staticCtx, forEachLoop.expression), create0(66, forEachLoop.expression.firstToken, null), create0(3, null, loopExpr), create1(40, null, null, 0), create0(3, null, iteratorVar));
            ByteCodeBuffer bufBody = serializeCodeBlock(staticCtx, forEachLoop.code);
            ByteCodeBuffer increment = join4(create0(45, null, iteratorVar), create1(40, null, null, 1), create0(4, null, "+"), create0(3, null, iteratorVar));
            ByteCodeBuffer doAssign = join4(create0(45, null, loopExpr), create0(45, null, iteratorVar), create0(22, null, null), create0(3, forEachLoop.varToken, forEachLoop.varToken.Value));
            ByteCodeBuffer lengthCheck = join5(create0(45, null, iteratorVar), create0(45, null, loopExpr), create0(15, null, "length"), create0(4, null, ">="), create1(29, null, null, doAssign.length + bufBody.length + increment.length + 1));
            bufBody = finalizeBreakContinue(bufBody, 5, true, 0);
            int reverseJumpDistance = -1 - increment.length - bufBody.length - doAssign.length - lengthCheck.length;
            ByteCodeBuffer fullCode = join6(setup, lengthCheck, doAssign, bufBody, increment, create1(24, null, null, reverseJumpDistance));
            return fullCode;
        }

        public static ByteCodeBuffer serializeForLoop(StaticContext staticCtx, Statement forLoop)
        {
            Expression condition = forLoop.condition;
            Statement[] code = forLoop.code;
            Statement[] init = forLoop.forInit;
            Statement[] step = forLoop.forStep;
            ByteCodeBuffer bufInit = serializeCodeBlock(staticCtx, init);
            ByteCodeBuffer bufStep = serializeCodeBlock(staticCtx, step);
            ByteCodeBuffer bufBody = serializeCodeBlock(staticCtx, code);
            ByteCodeBuffer bufCondition = serializeExpression(staticCtx, condition);
            bufCondition = ByteCodeUtil_ensureBooleanExpression(condition.firstToken, bufCondition);
            int stepSize = 0;
            int bodySize = 0;
            int conditionSize = bufCondition.length;
            if (bufStep != null)
            {
                stepSize = bufStep.length;
            }
            if (bufBody != null)
            {
                bodySize = bufBody.length;
            }
            return join6(bufInit, bufCondition, create1(28, null, null, bodySize + stepSize + 1), finalizeBreakContinue(bufBody, bufStep.length + 1, true, 0), bufStep, create1(24, null, null, -(1 + bodySize + stepSize + 1 + conditionSize)));
        }

        public static ByteCodeBuffer serializeFunctionInvocation(StaticContext staticCtx, Expression funcInvoke)
        {
            ByteCodeBuffer buf = serializeExpression(staticCtx, funcInvoke.root);
            int argc = funcInvoke.args.Length;
            int i = 0;
            while (i < argc)
            {
                buf = join2(buf, serializeExpression(staticCtx, funcInvoke.args[i]));
                i += 1;
            }
            return join2(buf, create1(21, funcInvoke.opToken, null, argc));
        }

        public static ByteCodeBuffer serializeFunctionReference(Expression funcRef)
        {
            AbstractEntity funcDef = (AbstractEntity)funcRef.entityPtr;
            int index = funcDef.serializationIndex;
            if (index == -1)
            {
                fail("");
            }
            return create1(39, funcRef.firstToken, null, index);
        }

        public static ByteCodeBuffer serializeIfStatement(StaticContext staticCtx, Statement ifStmnt)
        {
            Expression condition = ifStmnt.condition;
            Statement[] ifCode = ifStmnt.code;
            Statement[] elseCode = ifStmnt.elseCode;
            ByteCodeBuffer buf = serializeExpression(staticCtx, condition);
            ByteCodeBuffer bufTrue = serializeCodeBlock(staticCtx, ifCode);
            ByteCodeBuffer bufFalse = serializeCodeBlock(staticCtx, elseCode);
            buf = ByteCodeUtil_ensureBooleanExpression(condition.firstToken, buf);
            int trueSize = 0;
            int falseSize = 0;
            if (bufTrue != null)
            {
                trueSize = bufTrue.length;
            }
            if (bufFalse != null)
            {
                falseSize = bufFalse.length;
            }
            if (trueSize + falseSize == 0)
            {
                return join2(buf, create0(27, null, null));
            }
            if (falseSize == 0)
            {
                return join3(buf, create1(28, null, null, trueSize), bufTrue);
            }
            if (trueSize == 0)
            {
                return join3(buf, create1(29, null, null, falseSize), bufFalse);
            }
            return join5(buf, create1(28, null, null, trueSize + 1), bufTrue, create1(24, null, null, falseSize), bufFalse);
        }

        public static ByteCodeBuffer serializeIndex(StaticContext staticCtx, Expression index)
        {
            return join3(serializeExpression(staticCtx, index.root), serializeExpression(staticCtx, index.right), create0(22, index.opToken, null));
        }

        public static ByteCodeBuffer serializeInlineIncrement(StaticContext staticCtx, Expression ii)
        {
            switch (ii.root.type)
            {
                case 31:
                    return serializeInlineIncrementVar(staticCtx, ii);
                case 20:
                    return serializeInlineIncrementIndex(staticCtx, ii);
                case 11:
                    return serializeInlineIncrementDotField(staticCtx, ii);
            }
            fail("");
            return null;
        }

        public static ByteCodeBuffer serializeInlineIncrementDotField(StaticContext staticCtx, Expression ii)
        {
            bool isPrefix = ii.boolVal;
            ByteCodeBuffer root = serializeExpression(staticCtx, ii.root.root);
            int incrAmt = 1;
            if (ii.opToken.Value == "--")
            {
                incrAmt = -1;
            }
            ByteCodeBuffer buf = root;
            buf = join2(buf, create0(52, null, null));
            buf = join2(buf, create0(15, ii.root.opToken, ii.root.strVal));
            if (isPrefix)
            {
                buf = join2(buf, create1(23, ii.opToken, null, incrAmt));
                buf = join2(buf, create0(50, null, null));
            }
            else
            {
                buf = join2(buf, create0(52, null, null));
                buf = join2(buf, create1(23, ii.opToken, null, incrAmt));
                buf = join2(buf, create0(51, null, null));
            }
            buf = join2(buf, create0(1, ii.opToken, ii.root.strVal));
            return buf;
        }

        public static ByteCodeBuffer serializeInlineIncrementIndex(StaticContext staticCtx, Expression ii)
        {
            bool isPrefix = ii.boolVal;
            ByteCodeBuffer root = serializeExpression(staticCtx, ii.root.root);
            ByteCodeBuffer index = serializeExpression(staticCtx, ii.root.right);
            int incrAmt = 1;
            if (ii.opToken.Value == "--")
            {
                incrAmt = -1;
            }
            ByteCodeBuffer buf = join2(root, index);
            buf = join2(buf, create0(53, null, null));
            buf = join2(buf, create0(22, ii.root.opToken, null));
            if (isPrefix)
            {
                buf = join2(buf, create1(23, ii.opToken, null, incrAmt));
                buf = join2(buf, create0(52, null, null));
            }
            else
            {
                buf = join2(buf, create0(52, null, null));
                buf = join2(buf, create1(23, ii.opToken, null, incrAmt));
            }
            buf = join2(buf, create0(49, null, null));
            buf = join2(buf, create0(2, ii.root.opToken, null));
            return buf;
        }

        public static ByteCodeBuffer serializeInlineIncrementVar(StaticContext staticCtx, Expression ii)
        {
            bool isPrefix = ii.boolVal;
            int incrAmt = 1;
            if (ii.opToken.Value == "--")
            {
                incrAmt = -1;
            }
            if (isPrefix)
            {
                return join4(serializeExpression(staticCtx, ii.root), create1(23, ii.opToken, null, incrAmt), create0(52, null, null), create0(3, null, ii.root.strVal));
            }
            else
            {
                return join4(serializeExpression(staticCtx, ii.root), create0(52, null, null), create1(23, ii.opToken, null, incrAmt), create0(3, null, ii.root.strVal));
            }
        }

        public static ByteCodeBuffer serializeIntegerConstant(Expression intConst)
        {
            return create1(40, intConst.firstToken, null, intConst.intVal);
        }

        public static ByteCodeBuffer serializeLambda(Expression lambda)
        {
            FunctionEntity lambdaEntity = (FunctionEntity)lambda.entityPtr.specificData;
            return create1(63, lambda.firstToken, null, lambdaEntity.baseData.serializationIndex);
        }

        public static ByteCodeBuffer serializeListDefinition(StaticContext staticCtx, Expression listDef)
        {
            ByteCodeBuffer buf = null;
            int sz = listDef.values.Length;
            int i = 0;
            while (i < sz)
            {
                buf = join2(buf, serializeExpression(staticCtx, listDef.values[i]));
                i += 1;
            }
            return join2(buf, create1(12, listDef.firstToken, null, sz));
        }

        public static ByteCodeBuffer serializeNegativeSign(StaticContext staticCtx, Expression negSign)
        {
            return join2(serializeExpression(staticCtx, negSign.root), create0(26, negSign.opToken, null));
        }

        public static ByteCodeBuffer serializeNullConstant(Expression nullConst)
        {
            return create0(41, nullConst.firstToken, null);
        }

        public static ByteCodeBuffer serializeReturn(StaticContext staticCtx, Statement ret)
        {
            ByteCodeBuffer buf = null;
            if (ret.expression == null)
            {
                buf = create0(41, null, null);
            }
            else
            {
                buf = serializeExpression(staticCtx, ret.expression);
            }
            return join2(buf, create0(46, ret.firstToken, null));
        }

        public static ByteCodeBuffer serializeSlice(StaticContext staticCtx, Expression slice)
        {
            Expression start = slice.args[0];
            Expression end = slice.args[1];
            Expression step = slice.args[2];
            int sliceMask = 0;
            ByteCodeBuffer rootBuf = serializeExpression(staticCtx, slice.root);
            ByteCodeBuffer startBuf = null;
            ByteCodeBuffer endBuf = null;
            ByteCodeBuffer stepBuf = null;
            if (start != null)
            {
                sliceMask += 1;
                startBuf = ByteCodeUtil_ensureIntegerExpression(start.firstToken, serializeExpression(staticCtx, start));
            }
            if (end != null)
            {
                sliceMask += 2;
                endBuf = ByteCodeUtil_ensureIntegerExpression(end.firstToken, serializeExpression(staticCtx, end));
            }
            if (step != null)
            {
                sliceMask += 4;
                stepBuf = ByteCodeUtil_ensureIntegerExpression(step.firstToken, serializeExpression(staticCtx, step));
            }
            return join5(rootBuf, startBuf, endBuf, stepBuf, create1(47, slice.opToken, null, sliceMask));
        }

        public static ByteCodeBuffer serializeSpecialAction(StaticContext staticCtx, Expression action)
        {
            ByteCodeBuffer argBuffer = null;
            int argc = action.args.Length;
            int i = 0;
            while (i < argc)
            {
                argBuffer = join2(argBuffer, serializeExpression(staticCtx, action.args[i]));
                i++;
            }
            if (action.strVal == "math_floor")
            {
                return join2(argBuffer, create0(25, action.firstToken, null));
            }
            if (action.strVal == "unix_time")
            {
                return create2(48, null, null, 1, action.args[0].intVal);
            }
            int specialActionOpCode = SpecialActionUtil_GetSpecialActionOpCode(staticCtx.specialActionUtil, action.strVal);
            ByteCodeBuffer actionBuf = create1(48, null, null, specialActionOpCode);
            return join2(argBuffer, actionBuf);
        }

        public static ByteCodeBuffer serializeStatement(StaticContext staticCtx, Statement stmnt)
        {
            switch (stmnt.type)
            {
                case 1:
                    string op = stmnt.assignOp.Value;
                    if (op != "=")
                    {
                        op = op.Substring(0, op.Length - 1);
                    }
                    switch (stmnt.assignTarget.type)
                    {
                        case 31:
                            return serializeAssignVariable(staticCtx, stmnt, op);
                        case 20:
                            return serializeAssignIndex(staticCtx, stmnt, op);
                        case 11:
                            return serializeAssignField(staticCtx, stmnt, op);
                    }
                    fail("");
                    return null;
                case 2:
                    return serializeBreak(stmnt);
                case 3:
                    return serializeContinue(stmnt);
                case 4:
                    return serializeDoWhileLoop(staticCtx, stmnt);
                case 5:
                    return serializeExpressionStatement(staticCtx, stmnt);
                case 6:
                    return serializeForLoop(staticCtx, stmnt);
                case 7:
                    return serializeForEachLoop(staticCtx, stmnt);
                case 8:
                    return serializeIfStatement(staticCtx, stmnt);
                case 9:
                    return serializeReturn(staticCtx, stmnt);
                case 10:
                    return serializeSwitchStatement(staticCtx, stmnt);
                case 11:
                    return serializeThrowStatement(staticCtx, stmnt);
                case 12:
                    return serializeTryStatement(staticCtx, stmnt);
                case 13:
                    return serializeWhileLoop(staticCtx, stmnt);
                default:
                    fail("");
                    return null;
            }
        }

        public static ByteCodeBuffer serializeStringConstant(Expression strConst)
        {
            return create0(42, strConst.firstToken, strConst.strVal);
        }

        public static ByteCodeBuffer serializeSwitchStatement(StaticContext staticCtx, Statement switchStmnt)
        {
            int i = 0;
            int j = 0;
            if (switchStmnt.switchChunks.Length == 0)
            {
                return join3(serializeExpression(staticCtx, switchStmnt.condition), create0(17, switchStmnt.condition.firstToken, null), create0(27, null, null));
            }
            int conditionTypeEnsuranceOpCode = 17;
            Expression firstCaseExpr = switchStmnt.switchChunks[0].Cases[0];
            if (firstCaseExpr != null)
            {
                if (firstCaseExpr.type == 22)
                {
                    conditionTypeEnsuranceOpCode = 18;
                }
                else if (firstCaseExpr.type == 28)
                {
                    conditionTypeEnsuranceOpCode = 19;
                }
                else
                {
                    fail("");
                }
            }
            ByteCodeBuffer condBuf = join2(serializeExpression(staticCtx, switchStmnt.condition), create0(conditionTypeEnsuranceOpCode, switchStmnt.condition.firstToken, null));
            ByteCodeBuffer caseBuf = null;
            int currentJumpOffset = 0;
            bool hasDefault = false;
            System.Collections.Generic.Dictionary<string, int> stringJumpOffset = new Dictionary<string, int>();
            System.Collections.Generic.Dictionary<int, int> intJumpOffset = new Dictionary<int, int>();
            int defaultJumpOffset = -1;
            i = 0;
            while (i < switchStmnt.switchChunks.Length)
            {
                SwitchChunk chunk = switchStmnt.switchChunks[i];
                j = 0;
                while (j < chunk.Cases.Count)
                {
                    Expression expr = chunk.Cases[j];
                    if (expr == null)
                    {
                        defaultJumpOffset = currentJumpOffset;
                        hasDefault = true;
                    }
                    else if (conditionTypeEnsuranceOpCode == 18)
                    {
                        intJumpOffset[expr.intVal] = currentJumpOffset;
                    }
                    else if (conditionTypeEnsuranceOpCode == 19)
                    {
                        stringJumpOffset[expr.strVal] = currentJumpOffset;
                    }
                    else
                    {
                        fail("");
                    }
                    j += 1;
                }
                ByteCodeBuffer chunkBuf = serializeCodeBlock(staticCtx, chunk.Code.ToArray());
                currentJumpOffset += chunkBuf.length;
                caseBuf = join2(caseBuf, chunkBuf);
                i += 1;
            }
            if (!hasDefault)
            {
                defaultJumpOffset = currentJumpOffset;
            }
            caseBuf = finalizeBreakContinue(caseBuf, 0, false, 0);
            ByteCodeBuffer jumpBuf = null;
            if (conditionTypeEnsuranceOpCode == 18)
            {
                System.Collections.Generic.List<int> jumpArgs = new List<int>();
                jumpArgs.Add(-1);
                int[] intKeys = intJumpOffset.Keys.ToArray().OrderBy<int, int>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                i = 0;
                while (i < intKeys.Length)
                {
                    int k = intKeys[i];
                    jumpArgs.Add(intJumpOffset[k] + 2);
                    jumpArgs.Add(k);
                    i += 1;
                }
                ByteCodeBuffer lookup = create0(54, null, null);
                lookup.row.args = jumpArgs.ToArray();
                jumpBuf = join3(create2(55, null, null, defaultJumpOffset + 2, 1), lookup, create1(56, null, null, 2));
            }
            else if (conditionTypeEnsuranceOpCode == 19)
            {
                string[] keys = stringJumpOffset.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
                jumpBuf = create2(55, null, null, defaultJumpOffset + keys.Length + 1, 2);
                i = 0;
                while (i < keys.Length)
                {
                    string key = keys[i];
                    jumpBuf = join2(jumpBuf, create1(54, null, key, stringJumpOffset[key] + keys.Length + 1));
                    i += 1;
                }
                jumpBuf = join2(jumpBuf, create1(56, null, null, jumpBuf.length));
            }
            else
            {
                jumpBuf = create0(27, null, null);
            }
            return join3(condBuf, jumpBuf, caseBuf);
        }

        public static ByteCodeBuffer serializeTernary(StaticContext staticCtx, Expression ternaryExpression)
        {
            ByteCodeBuffer condBuf = serializeExpression(staticCtx, ternaryExpression.root);
            ByteCodeBuffer leftBuf = serializeExpression(staticCtx, ternaryExpression.left);
            ByteCodeBuffer rightBuf = serializeExpression(staticCtx, ternaryExpression.right);
            condBuf = ByteCodeUtil_ensureBooleanExpression(ternaryExpression.opToken, condBuf);
            return join5(condBuf, create1(28, null, null, leftBuf.length + 1), leftBuf, create1(24, null, null, rightBuf.length), rightBuf);
        }

        public static ByteCodeBuffer serializeThis(Expression thisKeyword)
        {
            return create0(43, thisKeyword.firstToken, null);
        }

        public static ByteCodeBuffer serializeThrowStatement(StaticContext staticCtx, Statement thrw)
        {
            return join2(serializeExpression(staticCtx, thrw.expression), create0(59, thrw.firstToken, null));
        }

        public static ByteCodeBuffer serializeTryStatement(StaticContext staticCtx, Statement tryStmnt)
        {
            int i = 0;
            ByteCodeBuffer tryBuf = serializeCodeBlock(staticCtx, tryStmnt.code);
            System.Collections.Generic.List<ByteCodeBuffer> catchBufs = new List<ByteCodeBuffer>();
            i = 0;
            while (i < tryStmnt.catchChunks.Length)
            {
                CatchChunk cc = tryStmnt.catchChunks[i];
                ByteCodeBuffer catchBuf = serializeCodeBlock(staticCtx, cc.Code);
                if (cc.exceptionVarName == null)
                {
                    catchBuf = join2(create0(27, null, null), catchBuf);
                }
                else
                {
                    catchBuf = join2(create0(3, cc.exceptionVarName, cc.exceptionVarName.Value), catchBuf);
                }
                catchBufs.Add(catchBuf);
                i += 1;
            }
            ByteCodeBuffer finallyBuf = join2(serializeCodeBlock(staticCtx, tryStmnt.finallyCode), create0(60, null, null));
            int jumpOffset = 0;
            i = catchBufs.Count - 2;
            while (i >= 0)
            {
                jumpOffset += catchBufs[i + 1].length;
                catchBufs[i] = join2(catchBufs[i], create1(24, null, null, jumpOffset));
                i -= 1;
            }
            jumpOffset = 0;
            System.Collections.Generic.List<int> catchRouterArgs = new List<int>();
            i = 0;
            while (i < catchBufs.Count)
            {
                if (i > 0)
                {
                    catchRouterArgs.Add(0);
                }
                catchRouterArgs.Add(jumpOffset);
                CatchChunk cc = tryStmnt.catchChunks[i];
                int j = 0;
                while (j < cc.ClassDefinitions.Length)
                {
                    ClassEntity cdef = cc.ClassDefinitions[j];
                    catchRouterArgs.Add(cdef.baseData.serializationIndex);
                    j += 1;
                }
                jumpOffset += catchBufs[i].length;
                i += 1;
            }
            catchRouterArgs.Insert(0, jumpOffset);
            ByteCodeBuffer catchRouterBuf = ByteCodeBuffer_fromRow(ByteCodeRow_new(61, null, null, catchRouterArgs.ToArray()));
            ByteCodeBuffer routeAndCatches = catchRouterBuf;
            i = 0;
            while (i < catchBufs.Count)
            {
                ByteCodeBuffer catchBuf = catchBufs[i];
                routeAndCatches = join2(routeAndCatches, catchBuf);
                i += 1;
            }
            tryBuf = join2(tryBuf, create1(24, null, null, routeAndCatches.length));
            int[] tryCatchInfo = new int[4];
            tryCatchInfo[0] = 0;
            tryCatchInfo[1] = tryBuf.length;
            tryCatchInfo[2] = routeAndCatches.length;
            tryCatchInfo[3] = finallyBuf.length;
            tryBuf.first.tryCatchInfo = tryCatchInfo;
            return join3(tryBuf, routeAndCatches, finallyBuf);
        }

        public static ByteCodeBuffer serializeTypeOf(StaticContext staticCtx, Expression typeOfExpr)
        {
            ByteCodeBuffer root = serializeExpression(staticCtx, typeOfExpr.root);
            return join2(root, create0(65, typeOfExpr.firstToken, null));
        }

        public static ByteCodeBuffer serializeVariable(Expression v)
        {
            if (v.strVal == "print")
            {
                fail("");
            }
            return create0(45, v.firstToken, v.strVal);
        }

        public static ByteCodeBuffer serializeWhileLoop(StaticContext staticCtx, Statement whileLoop)
        {
            ByteCodeBuffer condBuf = serializeExpression(staticCtx, whileLoop.condition);
            condBuf = ByteCodeUtil_ensureBooleanExpression(whileLoop.condition.firstToken, condBuf);
            ByteCodeBuffer loopBody = serializeCodeBlock(staticCtx, whileLoop.code);
            return join4(condBuf, create1(28, null, null, loopBody.length + 1), finalizeBreakContinue(loopBody, 1, true, -loopBody.length - 1 - condBuf.length), create1(24, null, null, -(loopBody.length + condBuf.length + 1 + 1)));
        }

        public static ClassEntity[] SortClassesInDeterministicDependencyOrder(ClassEntity[] unorderedClasses)
        {
            int i = 0;
            System.Collections.Generic.Dictionary<string, int> classDepthByFqName = new Dictionary<string, int>();
            System.Collections.Generic.Dictionary<string, ClassEntity> classByLexicalKey = new Dictionary<string, ClassEntity>();
            ClassEntity cls = null;
            int padSize = unorderedClasses.Length.ToString().Length + 1;
            i = 0;
            while (i < unorderedClasses.Length)
            {
                cls = unorderedClasses[i];
                int depth = ClassSorter_calcDepth(cls, classDepthByFqName);
                string key = string.Join("", new string[] { PadIntegerToSize(depth, padSize), ":", cls.baseData.fqName });
                classByLexicalKey[key] = cls;
                i += 1;
            }
            string[] keys = classByLexicalKey.Keys.ToArray().OrderBy<string, string>(_PST_GEN_arg => _PST_GEN_arg).ToArray();
            ClassEntity[] output = new ClassEntity[keys.Length];
            i = 0;
            while (i < keys.Length)
            {
                output[i] = classByLexicalKey[keys[i]];
                i += 1;
            }
            return output;
        }

        public static int SpecialActionUtil_GetSpecialActionArgc(SpecialActionUtil sau, string name)
        {
            return sau.SPECIAL_ACTION_ARGC[name];
        }

        public static int SpecialActionUtil_GetSpecialActionOpCode(SpecialActionUtil sau, string name)
        {
            return sau.SPECIAL_ACTION_BY_FUNC_NAME[name];
        }

        public static bool SpecialActionUtil_IsSpecialActionAndNotExtension(SpecialActionUtil sau, string name)
        {
            return sau.SPECIAL_ACTION_BY_FUNC_NAME.ContainsKey(name);
        }

        public static SpecialActionUtil SpecialActionUtil_new()
        {
            System.Collections.Generic.Dictionary<string, int> idByName = new Dictionary<string, int>();
            idByName["b64_from_bytes"] = 16;
            idByName["b64_to_bytes"] = 17;
            idByName["cmp"] = 6;
            idByName["json_parse"] = 20;
            idByName["json_serialize"] = 21;
            idByName["math_arccos"] = 8;
            idByName["math_arcsin"] = 9;
            idByName["math_arctan"] = 10;
            idByName["math_cos"] = 11;
            idByName["math_floor"] = -1;
            idByName["math_log"] = 12;
            idByName["math_sin"] = 13;
            idByName["math_tan"] = 14;
            idByName["parse_float"] = 25;
            idByName["parse_int"] = 15;
            idByName["random_float"] = 7;
            idByName["sort_end"] = 3;
            idByName["sort_get_next_cmp"] = 4;
            idByName["sort_proceed"] = 5;
            idByName["sort_start"] = 2;
            idByName["txt_bytes_to_string"] = 23;
            idByName["txt_is_valid_enc"] = 22;
            idByName["txt_string_to_bytes"] = 24;
            idByName["unix_time"] = 1;
            idByName["xml_parse"] = 26;
            System.Collections.Generic.Dictionary<string, int> argcByName = new Dictionary<string, int>();
            argcByName["b64_from_bytes"] = 2;
            argcByName["b64_to_bytes"] = 1;
            argcByName["cmp"] = 2;
            argcByName["json_parse"] = 2;
            argcByName["json_serialize"] = 2;
            argcByName["math_arccos"] = 1;
            argcByName["math_arcsin"] = 1;
            argcByName["math_arctan"] = 2;
            argcByName["math_cos"] = 1;
            argcByName["math_floor"] = 1;
            argcByName["math_log"] = 2;
            argcByName["math_sin"] = 1;
            argcByName["math_tan"] = 1;
            argcByName["parse_float"] = 1;
            argcByName["parse_int"] = 1;
            argcByName["random_float"] = 0;
            argcByName["sort_end"] = 1;
            argcByName["sort_get_next_cmp"] = 2;
            argcByName["sort_proceed"] = 2;
            argcByName["sort_start"] = 2;
            argcByName["txt_bytes_to_string"] = 2;
            argcByName["txt_is_valid_enc"] = 1;
            argcByName["txt_string_to_bytes"] = 2;
            argcByName["unix_time"] = 1;
            argcByName["xml_parse"] = 2;
            return new SpecialActionUtil(idByName, argcByName);
        }

        public static Statement Statement_createAssignment(Expression targetExpr, Token assignOp, Expression valueExpr)
        {
            Statement assign = Statement_new(targetExpr.firstToken, 1);
            assign.assignTarget = targetExpr;
            assign.assignValue = valueExpr;
            assign.assignOp = assignOp;
            return assign;
        }

        public static Statement Statement_createBreakContinue(Token breakContinueToken)
        {
            int type = 3;
            if (breakContinueToken.Value == "break")
            {
                type = 2;
            }
            return Statement_new(breakContinueToken, type);
        }

        public static Statement Statement_createDoWhile(Token doToken, Statement[] code, Token whileToken, Expression condition)
        {
            Statement doWhile = Statement_new(doToken, 4);
            doWhile.condition = condition;
            doWhile.code = code;
            doWhile.assignOp = whileToken;
            return doWhile;
        }

        public static Statement Statement_createExpressionAsStatement(Expression expr)
        {
            Statement wrapper = Statement_new(expr.firstToken, 5);
            wrapper.expression = expr;
            return wrapper;
        }

        public static Statement Statement_createForEachLoop(Token forToken, Token varName, Expression listExpr, Statement[] code)
        {
            Statement forEachLoop = Statement_new(forToken, 7);
            forEachLoop.varToken = varName;
            forEachLoop.expression = listExpr;
            forEachLoop.code = code;
            return forEachLoop;
        }

        public static Statement Statement_createForLoop(Token forToken, Statement[] init, Expression condition, Statement[] step, Statement[] code)
        {
            Statement forLoop = Statement_new(forToken, 6);
            forLoop.condition = condition;
            forLoop.forInit = init;
            forLoop.forStep = step;
            forLoop.code = code;
            return forLoop;
        }

        public static Statement Statement_createIfStatement(Token ifToken, Expression condition, Statement[] ifCode, Statement[] elseCode)
        {
            Statement ifStatement = Statement_new(ifToken, 8);
            ifStatement.condition = condition;
            ifStatement.code = ifCode;
            ifStatement.elseCode = elseCode;
            return ifStatement;
        }

        public static Statement Statement_createReturn(Token returnToken, Expression expr)
        {
            Statement ret = Statement_new(returnToken, 9);
            ret.expression = expr;
            return ret;
        }

        public static Statement Statement_createSwitchStatement(Token switchToken, Expression condition, SwitchChunk[] chunks)
        {
            Statement swtStmnt = Statement_new(switchToken, 10);
            swtStmnt.condition = condition;
            swtStmnt.switchChunks = chunks;
            return swtStmnt;
        }

        public static Statement Statement_createThrow(Token throwToken, Expression value)
        {
            Statement throwStmnt = Statement_new(throwToken, 11);
            throwStmnt.expression = value;
            return throwStmnt;
        }

        public static Statement Statement_createTry(Token tryToken, Statement[] tryCode, CatchChunk[] catches, Token finallyToken, Statement[] finallyCode)
        {
            Statement tryStmnt = Statement_new(tryToken, 12);
            tryStmnt.code = tryCode;
            tryStmnt.catchChunks = catches;
            tryStmnt.finallyCode = finallyCode;
            tryStmnt.finallyToken = finallyToken;
            return tryStmnt;
        }

        public static Statement Statement_createWhileLoop(Token whileToken, Expression condition, Statement[] code)
        {
            Statement whileLoop = Statement_new(whileToken, 13);
            whileLoop.condition = condition;
            whileLoop.code = code;
            return whileLoop;
        }

        public static Statement Statement_new(Token firstToken, int type)
        {
            return new Statement(firstToken, type, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0);
        }

        public static StaticContext StaticContext_new()
        {
            return new StaticContext(TokenizerStaticContext_new(), new Dictionary<string, AbstractEntity>(), SpecialActionUtil_new(), StringSet_fromArray(PST_StringSplit("public static", " ")));
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

        public static double TryParseFloat(Token throwToken, string rawValue)
        {
            double[] o = new double[2];
            PST_ParseFloat(rawValue, o);
            if (o[0] > 0)
            {
                return o[1];
            }
            Errors_Throw(throwToken, "Invalid float constant");
            return 0.0;
        }

        public static int TryParseInteger(Token throwToken, string rawValue, bool isHex)
        {
            int output = 0;
            int start = 0;
            int baseMultiplier = 10;
            int[] chars = PST_stringToUtf8Bytes(rawValue.ToLower());
            if (isHex)
            {
                start = 2;
                baseMultiplier = 16;
            }
            int i = start;
            while (i < chars.Length)
            {
                int d = chars[i];
                int digitVal = 0;
                if (d >= 48 && d <= 57)
                {
                    digitVal = d - 48;
                }
                else if (isHex && d >= 97 && d <= 102)
                {
                    digitVal = d - 97 + 10;
                }
                else
                {
                    if (isHex)
                    {
                        Errors_Throw(throwToken, "Invalid hexadecimal constant.");
                    }
                    Errors_Throw(throwToken, "Invalid integer constant");
                }
                output = output * baseMultiplier + digitVal;
                i++;
            }
            return output;
        }

        public static string TryParseString(Token throwToken, string rawValue)
        {
            System.Collections.Generic.List<string> output = new List<string>();
            int length = rawValue.Length - 1;
            string c = "";
            int i = 1;
            while (i < length)
            {
                c = rawValue.Substring(i, 1);
                if (c == "\\")
                {
                    i += 1;
                    if (i == length)
                    {
                        Errors_Throw(throwToken, "Invalid backslash in string constant.");
                    }
                    c = rawValue.Substring(i, 1);
                    if (c == "n")
                    {
                        c = "\n";
                    }
                    else if (c == "r")
                    {
                        c = "\r";
                    }
                    else if (c == "'" || c == "\"" || c == "\\")
                    {
                    }
                    else if (c == "t")
                    {
                        c = "\t";
                    }
                    else
                    {
                        Errors_Throw(throwToken, string.Join("", new string[] { "Unrecognized string escape sequence: '\\", c, "'" }));
                    }
                }
                output.Add(c);
                i += 1;
            }
            return string.Join("", output);
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
