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
                TokenUtil.createFakeToken(tokens, (int) TokenType.NAME, "{BUILTIN}", 0, 0)
            };

            return FunctionWrapper.ImportStatement_new(
                TokenUtil.createFakeToken(tokens, (int) TokenType.KEYWORD, "import", 0, 0),
                builtinName,
                TokenUtil.createFakeToken(tokens, (int) TokenType.PUNCTUATION, "*", 0, 0));
        }

        public static ImportStatement[] AdvanceThroughImports(TokenStream tokens, bool isCoreBuiltin)
        {
            List<ImportStatement> output = new List<ImportStatement>();
            if (!isCoreBuiltin) output.Add(createBuiltinImport(tokens));

            while (TokenStreamUtil.Tokens_hasMore(tokens) && TokenStreamUtil.Tokens_isNext(tokens, "import"))
            {
                Token importToken = TokenStreamUtil.Tokens_popKeyword(tokens, "import");
                List<Token> tokenChain = new List<Token>() {
                    TokenStreamUtil.Tokens_popName(tokens, "module name"),
                };

                while (TokenStreamUtil.Tokens_popIfPresent(tokens, "."))
                {
                    tokenChain.Add(TokenStreamUtil.Tokens_popName(tokens, "module name"));
                }
                Token importTargetName = null;
                if (TokenStreamUtil.Tokens_popIfPresent(tokens, "->"))
                {
                    if (TokenStreamUtil.Tokens_isNext(tokens, "*"))
                    {
                        importTargetName = TokenStreamUtil.Tokens_pop(tokens);
                    }
                    else
                    {
                        importTargetName = TokenStreamUtil.Tokens_popName(tokens, "import target variable");
                    }
                }

                TokenStreamUtil.Tokens_popExpected(tokens, ";");

                output.Add(FunctionWrapper.ImportStatement_new(importToken, tokenChain, importTargetName));
            }

            return output.ToArray();
        }
    }
}
