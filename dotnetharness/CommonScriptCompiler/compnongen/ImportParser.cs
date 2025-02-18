using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class ImportParser
    {
        private static ImportStatement createBuiltinImport(TokenStream tokens)
        {
            List<Token> builtinName = new List<Token>()
            {
                FunctionWrapper.createFakeToken(tokens, (int) TokenType.NAME, "{BUILTIN}", 0, 0)
            };

            return FunctionWrapper.ImportStatement_new(
                FunctionWrapper.createFakeToken(tokens, (int) TokenType.KEYWORD, "import", 0, 0),
                builtinName,
                FunctionWrapper.createFakeToken(tokens, (int) TokenType.PUNCTUATION, "*", 0, 0));
        }

        public static ImportStatement[] AdvanceThroughImports(TokenStream tokens, bool isCoreBuiltin)
        {
            List<ImportStatement> output = new List<ImportStatement>();
            if (!isCoreBuiltin) output.Add(createBuiltinImport(tokens));

            while (FunctionWrapper.Tokens_hasMore(tokens) && FunctionWrapper.Tokens_isNext(tokens, "import"))
            {
                Token importToken = FunctionWrapper.Tokens_popKeyword(tokens, "import");
                List<Token> tokenChain = new List<Token>() {
                    FunctionWrapper.Tokens_popName(tokens, "module name"),
                };

                while (FunctionWrapper.Tokens_popIfPresent(tokens, "."))
                {
                    tokenChain.Add(FunctionWrapper.Tokens_popName(tokens, "module name"));
                }
                Token importTargetName = null;
                if (FunctionWrapper.Tokens_popIfPresent(tokens, "->"))
                {
                    if (FunctionWrapper.Tokens_isNext(tokens, "*"))
                    {
                        importTargetName = FunctionWrapper.Tokens_pop(tokens);
                    }
                    else
                    {
                        importTargetName = FunctionWrapper.Tokens_popName(tokens, "import target variable");
                    }
                }

                FunctionWrapper.Tokens_popExpected(tokens, ";");

                output.Add(FunctionWrapper.ImportStatement_new(importToken, tokenChain, importTargetName));
            }

            return output.ToArray();
        }
    }
}
