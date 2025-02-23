using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class Exporter
    {
        public static byte[] exportBundle(string flavorId, string extVersionId, CompilationBundle bundle)
        {
            ByteStringBuilder flavor = FunctionWrapper.bsbFromLenString(flavorId);
            ByteStringBuilder version = FunctionWrapper.bsbFromLenString(extVersionId);
            byte commonScriptMajor = 0;
            byte commonScriptMinor = 1;
            byte commonScriptPatch = 0;

            ByteStringBuilder header = FunctionWrapper.bsbJoin4(
                FunctionWrapper.bsbFromUtf8String("PXCS"),
                FunctionWrapper.bsbFromBytes(new int[] {
                    0,
                    commonScriptMajor,
                    commonScriptMinor,
                    commonScriptPatch,
                }),
                flavor,
                version);

            // TODO: xor-down a sha256 into a short checksum of the remaining chunks

            ByteStringBuilder metadata = FunctionWrapper.bsbJoin3(
                FunctionWrapper.bsbFromUtf8String("MTD"),
                FunctionWrapper.bsbFromInt(bundle.mainFunctionId),
                FunctionWrapper.bsbFromInt(bundle.builtInCount)
            );

            ByteStringBuilder tokenData = FunctionWrapper.bsbJoin2(
                FunctionWrapper.bsbFromUtf8String("TOK"),
                FunctionWrapper.bsbFromInt(bundle.tokensById.Length - 1));
            Dictionary<string, int> fileNameToOffset = new Dictionary<string, int>();
            ByteStringBuilder tokenFileNames = null;
            for (int i = 1; i < bundle.tokensById.Length; i++)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                if (!fileNameToOffset.ContainsKey(filename))
                {
                    fileNameToOffset[filename] = fileNameToOffset.Count;
                    tokenFileNames = FunctionWrapper.bsbJoin2(tokenFileNames, FunctionWrapper.bsbFromLenString(filename));
                }
            }
            tokenData = FunctionWrapper.bsbJoin3(tokenData, FunctionWrapper.bsbFromInt(fileNameToOffset.Count), tokenFileNames);
            for (int i = 1; i < bundle.tokensById.Length; i++)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                int fileOffset = fileNameToOffset[filename];
                tokenData = FunctionWrapper.bsbJoin4(tokenData, FunctionWrapper.bsbFromInt(fileOffset), FunctionWrapper.bsbFromInt(tok.Line), FunctionWrapper.bsbFromInt(tok.Col));
            }

            ByteStringBuilder stringData = FunctionWrapper.bsbJoin2(
                FunctionWrapper.bsbFromUtf8String("STR"),
                FunctionWrapper.bsbFromInt(bundle.stringById.Length - 1));
            for (int i = 1; i < bundle.stringById.Length; i++)
            {
                string val = bundle.stringById[i];
                stringData = FunctionWrapper.bsbJoin2(stringData, FunctionWrapper.bsbFromLenString(val));
            }

            /*
            ByteStringBuilder nameData = bsbJoin2(
                bsbFromUtf8String("NAM"), 
                bsbFromInt(bundle.namesById.Length - 1));
            for (int i = 1; i < bundle.namesById.Length; i++)
            {
                string name = bundle.stringById[i];
                nameData = bsbJoin2(nameData, bsbFromLenString(name));
            }*/

            ByteStringBuilder entityAcc = null;

            for (int i = 1; i < bundle.functionById.Count; i++)
            {
                BundleFunctionInfo fn = bundle.functionById[i];
                entityAcc = FunctionWrapper.bsbJoin2(entityAcc, FunctionWrapper.bsbJoin5(
                    FunctionWrapper.bsbFromInt(fn.argcMin),
                    FunctionWrapper.bsbFromInt(fn.argcMax),
                    FunctionWrapper.bsbFromLenString(fn.name),
                    FunctionWrapper.bsbFromInt(fn.code.Length),
                    exportCode(fn.code))
                );
            }

            for (int i = 1; i < bundle.lambdaById.Count; i++)
            {
                BundleFunctionInfo fn = bundle.lambdaById[i];
                entityAcc = FunctionWrapper.bsbJoin2(entityAcc, FunctionWrapper.bsbJoin4(
                    FunctionWrapper.bsbFromInt(fn.argcMin),
                    FunctionWrapper.bsbFromInt(fn.argcMax),
                    FunctionWrapper.bsbFromInt(fn.code.Length),
                    exportCode(fn.code))
                );
            }

            for (int i = 1; i < bundle.enumById.Count; i++)
            {
                BundleEnumInfo bei = bundle.enumById[i];
                int memberCount = bei.names.Length;
                entityAcc = FunctionWrapper.bsbJoin2(entityAcc, FunctionWrapper.bsbFromInt(memberCount));
                for (int j = 0; j < memberCount; j++)
                {
                    entityAcc = FunctionWrapper.bsbJoin3(
                        entityAcc,
                        FunctionWrapper.bsbFromLenString(bei.names[j]),
                        FunctionWrapper.bsbFromInt(bei.values[j]));
                }
            }

            for (int i = 1; i < bundle.classById.Count; i++)
            {
                BundleClassInfo bci = bundle.classById[i];
                ByteStringBuilder classInfo = FunctionWrapper.bsbJoin8(
                    FunctionWrapper.bsbFromLenString(bci.name),
                    FunctionWrapper.bsbFromInt(bci.parentId),
                    FunctionWrapper.bsbFromInt(bci.ctorId),
                    FunctionWrapper.bsbFromInt(bci.staticCtorId),
                    FunctionWrapper.bsbFromInt(bci.newDirectMembersByNextOffsets.Length),
                    FunctionWrapper.bsbFromInt(bci.methodsToId.Count),
                    FunctionWrapper.bsbFromInt(bci.staticFields.Count),
                    FunctionWrapper.bsbFromInt(bci.staticMethods.Count));

                for (int j = 0; j < bci.newDirectMembersByNextOffsets.Length; j++)
                {
                    // TODO: encode function ID here instead of below
                    // This also means that you'll have to indicate that a string does not contribute to the new direct member count

                    string memberName = bci.newDirectMembersByNextOffsets[j];
                    bool isMethod = bci.methodsToId.ContainsKey(memberName);
                    int info = isMethod ? 1 : 0;
                    if (isMethod)
                    {
                        // TODO: put the function ID here once you have a way to indicate overridden methods
                    }
                    else
                    {
                        // TODO: put the field initial value here. For now, just set all this in the constructor.
                    }
                    classInfo = FunctionWrapper.bsbJoin3(
                        classInfo,
                        FunctionWrapper.bsbFromLenString(memberName),
                        FunctionWrapper.bsbFromInt(info));
                }

                ByteStringBuilder methodInfo = null;
                string[] methodNames = bci.methodsToId.Keys.OrderBy(k => k).ToArray();
                foreach (string methodName in methodNames)
                {
                    methodInfo = FunctionWrapper.bsbJoin3(
                        methodInfo,
                        FunctionWrapper.bsbFromLenString(methodName),
                        FunctionWrapper.bsbFromInt(bci.methodsToId[methodName]));
                }

                ByteStringBuilder staticFields = null;
                foreach (string staticField in bci.staticFields.OrderBy(k => k))
                {
                    staticFields = FunctionWrapper.bsbJoin2(
                        staticFields,
                        FunctionWrapper.bsbFromLenString(staticField));
                }

                ByteStringBuilder staticMethods = null;
                foreach (string staticMethod in bci.staticMethods.OrderBy(k => k))
                {
                    int funcId = bci.methodsToId[staticMethod];
                    staticMethods = FunctionWrapper.bsbJoin3(
                        staticMethods,
                        FunctionWrapper.bsbFromLenString(staticMethod),
                        FunctionWrapper.bsbFromInt(funcId));
                }

                entityAcc = FunctionWrapper.bsbJoin5(
                    entityAcc,
                    classInfo,
                    methodInfo,
                    staticFields,
                    staticMethods);
            }

            ByteStringBuilder entityHeader = FunctionWrapper.bsbJoin5(
                FunctionWrapper.bsbFromUtf8String("ENT"),
                FunctionWrapper.bsbFromInt(bundle.functionById.Count - 1),
                FunctionWrapper.bsbFromInt(bundle.enumById.Count - 1),
                FunctionWrapper.bsbFromInt(bundle.classById.Count - 1),
                FunctionWrapper.bsbFromInt(bundle.lambdaById.Count - 1));

            ByteStringBuilder entityData = FunctionWrapper.bsbJoin2(entityHeader, entityAcc);

            ByteStringBuilder full = FunctionWrapper.bsbJoin5(
                header, metadata, stringData, tokenData, entityData);

            return bsbFlatten(full);
        }

        public static ByteStringBuilder exportCode(ByteCodeRow[] rows)
        {
            ByteStringBuilder bsb = null;
            foreach (ByteCodeRow row in rows)
            {
                ByteStringBuilder bsbRow = FunctionWrapper.bsbJoin4(
                    FunctionWrapper.bsbFromInt(row.opCode),
                    FunctionWrapper.bsbFromInt(row.args.Length * 4 + (row.stringArg != null ? 1 : 0) + (row.token != null ? 2 : 0)),
                    row.stringArg != null ? FunctionWrapper.bsbFromInt(row.stringId) : null,
                    row.token != null ? FunctionWrapper.bsbFromInt(row.tokenId) : null
                );
                for (int i = 0; i < row.args.Length; i++)
                {
                    bsbRow = FunctionWrapper.bsbJoin2(bsbRow, FunctionWrapper.bsbFromInt(row.args[i]));
                }
                bsb = FunctionWrapper.bsbJoin2(bsb, bsbRow);
            }
            return bsb;
        }

        public static byte[] bsbFlatten(ByteStringBuilder sbs)
        {
            List<ByteStringBuilder> q = new List<ByteStringBuilder>() { sbs };
            List<byte> output = new List<byte>();
            while (q.Count > 0)
            {
                ByteStringBuilder current = q[q.Count - 1];
                q.RemoveAt(q.Count - 1);
                if (current.isLeaf)
                {
                    int[] currentBytes = current.bytes;
                    int len = currentBytes.Length;
                    for (int i = 0; i < len; i++)
                    {
                        output.Add((byte)currentBytes[i]);
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
    }
}
