using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonScript.Compiler
{
    internal class ByteStringBuilder
    {
        public bool isLeaf;
        public int length;
        public byte[] bytes;
        public ByteStringBuilder left;
        public ByteStringBuilder right;
    }

    internal static class Exporter
    {
        public static byte[] exportBundle(string flavorId, string extVersionId, CompilationBundle bundle)
        {
            ByteStringBuilder flavor = bsbFromLenString(flavorId);
            ByteStringBuilder version = bsbFromLenString(extVersionId);
            byte commonScriptMajor = 0;
            byte commonScriptMinor = 1;
            byte commonScriptPatch = 0;

            ByteStringBuilder header = bsbJoin4(
                bsbFromUtf8String("PXCS"),
                bsbFromBytes(new byte[] {
                    0,
                    commonScriptMajor,
                    commonScriptMinor,
                    commonScriptPatch,
                }),
                flavor,
                version);

            // TODO: xor-down a sha256 into a short checksum of the remaining chunks

            ByteStringBuilder metadata = bsbJoin3(
                bsbFromUtf8String("MTD"),
                bsbFromInt(bundle.mainFunctionId),
                bsbFromInt(bundle.builtInCount)
            );

            ByteStringBuilder tokenData = bsbJoin2(
                bsbFromUtf8String("TOK"),
                bsbFromInt(bundle.tokensById.Length - 1));
            Dictionary<string, int> fileNameToOffset = new Dictionary<string, int>();
            ByteStringBuilder tokenFileNames = null;
            for (int i = 1; i < bundle.tokensById.Length; i++)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                if (!fileNameToOffset.ContainsKey(filename))
                {
                    fileNameToOffset[filename] = fileNameToOffset.Count;
                    tokenFileNames = bsbJoin2(tokenFileNames, bsbFromLenString(filename));
                }
            }
            tokenData = bsbJoin3(tokenData, bsbFromInt(fileNameToOffset.Count), tokenFileNames);
            for (int i = 1; i < bundle.tokensById.Length; i++)
            {
                Token tok = bundle.tokensById[i];
                string filename = tok.File;
                int fileOffset = fileNameToOffset[filename];
                tokenData = bsbJoin4(tokenData, bsbFromInt(fileOffset), bsbFromInt(tok.Line), bsbFromInt(tok.Col));
            }

            ByteStringBuilder stringData = bsbJoin2(
                bsbFromUtf8String("STR"),
                bsbFromInt(bundle.stringById.Length - 1));
            for (int i = 1; i < bundle.stringById.Length; i++)
            {
                string val = bundle.stringById[i];
                stringData = bsbJoin2(stringData, bsbFromLenString(val));
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
                entityAcc = bsbJoin2(entityAcc, bsbJoin5(
                    bsbFromInt(fn.argcMin),
                    bsbFromInt(fn.argcMax),
                    bsbFromLenString(fn.name),
                    bsbFromInt(fn.code.Length),
                    exportCode(fn.code))
                );
            }

            for (int i = 1; i < bundle.enumById.Count; i++)
            {
                BundleEnumInfo bei = bundle.enumById[i];
                int memberCount = bei.names.Length;
                entityAcc = bsbJoin2(entityAcc, bsbFromInt(memberCount));
                for (int j = 0; j < memberCount; j++)
                {
                    entityAcc = bsbJoin3(
                        entityAcc,
                        bsbFromLenString(bei.names[j]),
                        bsbFromInt(bei.values[j]));
                }
            }

            for (int i = 1; i < bundle.classById.Count; i++)
            {
                BundleClassInfo bci = bundle.classById[i];
                ByteStringBuilder classInfo = bsbJoin8(
                    bsbFromLenString(bci.name),
                    bsbFromInt(bci.parentId),
                    bsbFromInt(bci.ctorId),
                    bsbFromInt(bci.staticCtorId),
                    bsbFromInt(bci.newDirectMembersByNextOffsets.Length),
                    bsbFromInt(bci.methodsToId.Count),
                    bsbFromInt(bci.staticFields.Count),
                    bsbFromInt(bci.staticMethods.Count));

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
                    classInfo = bsbJoin3(
                        classInfo,
                        bsbFromLenString(memberName),
                        bsbFromInt(info));
                }

                ByteStringBuilder methodInfo = null;
                string[] methodNames = bci.methodsToId.Keys.OrderBy(k => k).ToArray();
                foreach (string methodName in methodNames)
                {
                    methodInfo = bsbJoin3(
                        methodInfo,
                        bsbFromLenString(methodName),
                        bsbFromInt(bci.methodsToId[methodName]));
                }

                ByteStringBuilder staticFields = null;
                foreach (string staticField in bci.staticFields.OrderBy(k => k))
                {
                    staticFields = bsbJoin2(
                        staticFields,
                        bsbFromLenString(staticField));
                }

                ByteStringBuilder staticMethods = null;
                foreach (string staticMethod in bci.staticMethods.OrderBy(k => k))
                {
                    int funcId = bci.methodsToId[staticMethod];
                    staticMethods = bsbJoin3(
                        staticMethods,
                        bsbFromLenString(staticMethod),
                        bsbFromInt(funcId));
                }

                entityAcc = bsbJoin5(
                    entityAcc,
                    classInfo,
                    methodInfo,
                    staticFields,
                    staticMethods);
            }

            ByteStringBuilder entityHeader = bsbJoin4(
                bsbFromUtf8String("ENT"),
                bsbFromInt(bundle.functionById.Count - 1),
                bsbFromInt(bundle.enumById.Count - 1),
                bsbFromInt(bundle.classById.Count - 1));

            ByteStringBuilder entityData = bsbJoin2(entityHeader, entityAcc);

            ByteStringBuilder full = bsbJoin5(
                header, metadata, stringData, tokenData, entityData);

            return bsbFlatten(full);
        }

        public static ByteStringBuilder exportCode(ByteCodeRow[] rows)
        {
            ByteStringBuilder bsb = null;
            foreach (ByteCodeRow row in rows)
            {
                ByteStringBuilder bsbRow = bsbJoin4(
                    bsbFromInt(row.opCode),
                    bsbFromInt(row.args.Length * 4 + (row.stringArg != null ? 1 : 0) + (row.token != null ? 2 : 0)),
                    row.stringArg != null ? bsbFromInt(row.stringId) : null,
                    row.token != null ? bsbFromInt(row.tokenId) : null
                );
                for (int i = 0; i < row.args.Length; i++)
                {
                    bsbRow = bsbJoin2(bsbRow, bsbFromInt(row.args[i]));
                }
                bsb = bsbJoin2(bsb, bsbRow);
            }
            return bsb;
        }

        public static ByteStringBuilder bsbFromLenString(string value)
        {
            ByteStringBuilder payload = bsbFromUtf8String(value);
            return bsbJoin2(bsbFromInt(payload.length), payload);
        }

        public static ByteStringBuilder bsbFromUtf8String(string value)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            return new ByteStringBuilder()
            {
                isLeaf = true,
                bytes = bytes,
                length = bytes.Length,
                left = null,
                right = null,
            };
        }

        public static ByteStringBuilder bsbFromBytes(byte[] bytes)
        {
            return new ByteStringBuilder()
            {
                isLeaf = true,
                bytes = bytes,
                length = bytes.Length,
                left = null,
                right = null,
            };
        }

        public static ByteStringBuilder bsbFromByte(int value)
        {
            byte[] buf = new byte[1];
            buf[0] = (byte)(value & 0xFF);
            return new ByteStringBuilder()
            {
                isLeaf = true,
                bytes = buf,
                length = 1,
                left = null,
                right = null,
            };
        }

        /*
            0xxxxxxx --> 7 bit positive integer
            11000000 --> -2^31
            11100000 --> -2^63
            100syyyy --> y-byte positive integer followed by y-bytes to be converted into an integer. Then if s is 1, flip the sign.
        */
        public static ByteStringBuilder bsbFromInt(long value)
        {
            byte[] buf;
            if (value >= 0 && value < 128)
            {
                buf = new byte[1];
                buf[0] = (byte)value;
            }
            else if (value == int.MinValue)
            {
                buf = new byte[1];
                buf[0] = 0xC0;
            }
            else if (value == int.MinValue)
            {
                buf = new byte[1];
                buf[0] = 0xE0;
            }
            else
            {
                byte firstByte = 0x80;


                bool isNegative = value < 0;
                if (isNegative)
                {
                    value *= -1;
                    firstByte |= 0x10;
                }

                if (value < 256)
                {
                    firstByte |= 0x01;
                    buf = new byte[2];
                    buf[0] = firstByte;
                    buf[1] = (byte)(value & 0xFF);
                }
                else if (value <= 0xFFFF)
                {
                    firstByte |= 0x02;
                    buf = new byte[3];
                    buf[0] = firstByte;
                    buf[1] = (byte)((value >> 8) & 0xFF);
                    buf[2] = (byte)(value & 0xFF);
                }
                else if (value <= 0xFFFFFF)
                {
                    firstByte |= 0x03;
                    buf = new byte[4];
                    buf[0] = firstByte;
                    buf[1] = (byte)((value >> 16) & 0xFF);
                    buf[2] = (byte)((value >> 8) & 0xFF);
                    buf[3] = (byte)(value & 0xFF);
                }
                else if (value <= 0xFFFFFFFF)
                {
                    firstByte |= 0x04;
                    buf = new byte[5];
                    buf[0] = firstByte;
                    buf[1] = (byte)((value >> 24) & 0xFF);
                    buf[2] = (byte)((value >> 16) & 0xFF);
                    buf[3] = (byte)((value >> 8) & 0xFF);
                    buf[4] = (byte)(value & 0xFF);
                }
                else if (value <= 0xFFFFFFFFFFL)
                {
                    // TODO: etc.
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }

            }

            return new ByteStringBuilder()
            {
                length = buf.Length,
                bytes = buf,
                isLeaf = true,
                left = null,
                right = null,
            };
        }

        public static ByteStringBuilder bsbJoin2(ByteStringBuilder a, ByteStringBuilder b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return new ByteStringBuilder()
            {
                isLeaf = false,
                bytes = null,
                length = a.length + b.length,
                left = a,
                right = b,
            };
        }

        public static ByteStringBuilder bsbJoin3(
            ByteStringBuilder a,
            ByteStringBuilder b,
            ByteStringBuilder c)
        {
            return bsbJoin4(a, b, c, null);
        }

        public static ByteStringBuilder bsbJoin4(
            ByteStringBuilder a,
            ByteStringBuilder b,
            ByteStringBuilder c,
            ByteStringBuilder d)
        {
            return bsbJoin2(bsbJoin2(a, b), bsbJoin2(c, d));
        }

        public static ByteStringBuilder bsbJoin5(
            ByteStringBuilder a,
            ByteStringBuilder b,
            ByteStringBuilder c,
            ByteStringBuilder d,
            ByteStringBuilder e)
        {
            return bsbJoin3(bsbJoin2(a, b), bsbJoin2(c, d), e);
        }

        public static ByteStringBuilder bsbJoin6(
            ByteStringBuilder a,
            ByteStringBuilder b,
            ByteStringBuilder c,
            ByteStringBuilder d,
            ByteStringBuilder e,
            ByteStringBuilder f)
        {
            return bsbJoin3(bsbJoin2(a, b), bsbJoin2(c, d), bsbJoin2(e, f));
        }

        public static ByteStringBuilder bsbJoin8(
            ByteStringBuilder a,
            ByteStringBuilder b,
            ByteStringBuilder c,
            ByteStringBuilder d,
            ByteStringBuilder e,
            ByteStringBuilder f,
            ByteStringBuilder g,
            ByteStringBuilder h)
        {
            return bsbJoin2(bsbJoin4(a, b, c, d), bsbJoin4(e, f, g, h));
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
                    output.AddRange(current.bytes);
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
