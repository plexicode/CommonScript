using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class CompilationBundle
    {
        public List<string> nameById;
        public Dictionary<string, int> nameToId;
        public string[] stringById = null;
        public List<ByteCodeRow[]> byteCodeById;
        public List<BundleFunctionInfo> functionById;
        public List<BundleClassInfo> classById;
        public List<BundleEnumInfo> enumById;
        public Token[] tokensById = null;
        public int mainFunctionId = 0;
        public int builtInCount = 0;

        public CompilationBundle()
        {
            byteCodeById = new List<ByteCodeRow[]>() { null };
            functionById = new List<BundleFunctionInfo>() { null };
            classById = new List<BundleClassInfo>() { null };
            enumById = new List<BundleEnumInfo>() { null };
        }
    }

    internal class BundleClassInfo
    {
        public int id;
        public int parentId;
        public string name;
        public int ctorId;
        public int staticCtorId;
        public Dictionary<string, int> methodsToId;
        public string[] newDirectMembersByNextOffsets;
        public List<string> staticMethods;
        public List<string> staticFields;
    }

    internal class BundleFunctionInfo
    {
        public ByteCodeRow[] code;
        public int argcMin;
        public int argcMax;
        public string name;
    }

    internal class BundleEnumInfo
    {
        public int enumId;
        public string[] names;
        public int[] values;

        public static BundleEnumInfo createFromEntity(EnumEntity e)
        {
            List<string> names = new List<string>();
            List<int> values = new List<int>();
            for (int i = 0; i < e.memberValues.Length; i++)
            {
                names.Add(e.memberNameTokens[i].Value);
                values.Add(e.memberValues[i].intVal);
            }

            return new BundleEnumInfo()
            {
                enumId = e.serializationIndex,
                names = names.ToArray(),
                values = values.ToArray(),
            };
        }
    }
}
