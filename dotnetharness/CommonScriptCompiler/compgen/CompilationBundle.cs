using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class CompilationBundle
    {
        public string[] stringById;
        public System.Collections.Generic.List<ByteCodeRow[]> byteCodeById;
        public System.Collections.Generic.List<BundleFunctionInfo> functionById;
        public System.Collections.Generic.List<BundleFunctionInfo> lambdaById;
        public System.Collections.Generic.List<BundleClassInfo> classById;
        public System.Collections.Generic.List<BundleEnumInfo> enumById;
        public Token[] tokensById;
        public int mainFunctionId;
        public int builtInCount;

        public CompilationBundle(string[] stringById, System.Collections.Generic.List<ByteCodeRow[]> byteCodeById, System.Collections.Generic.List<BundleFunctionInfo> functionById, System.Collections.Generic.List<BundleFunctionInfo> lambdaById, System.Collections.Generic.List<BundleClassInfo> classById, System.Collections.Generic.List<BundleEnumInfo> enumById, Token[] tokensById, int mainFunctionId, int builtInCount)
        {
            this.stringById = stringById;
            this.byteCodeById = byteCodeById;
            this.functionById = functionById;
            this.lambdaById = lambdaById;
            this.classById = classById;
            this.enumById = enumById;
            this.tokensById = tokensById;
            this.mainFunctionId = mainFunctionId;
            this.builtInCount = builtInCount;
        }
    }
}
