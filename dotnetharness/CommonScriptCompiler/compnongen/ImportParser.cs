using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class ImportParser
    {
        private static ImportStatement createBuiltinImport(TokenStream tokens)
        {
            List<Token> builtinName = new List<Token>()
            {
                Token.createFakeToken(tokens, TokenType.NAME, "{BUILTIN}", 0, 0)
            };

            return new ImportStatement(
                Token.createFakeToken(tokens, TokenType.KEYWORD, "import", 0, 0),
                builtinName,
                Token.createFakeToken(tokens, TokenType.PUNCTUATION, "*", 0, 0));
        }

        public static ImportStatement[] AdvanceThroughImports(TokenStream tokens, bool isCoreBuiltin)
        {
            List<ImportStatement> output = new List<ImportStatement>();
            if (!isCoreBuiltin) output.Add(createBuiltinImport(tokens));

            while (tokens.hasMore() && tokens.isNext("import"))
            {
                Token importToken = tokens.popKeyword("import");
                List<Token> tokenChain = new List<Token>() {
                    tokens.popName("module name"),
                };

                while (tokens.popIfPresent("."))
                {
                    tokenChain.Add(tokens.popName("module name"));
                }
                Token importTargetName = null;
                if (tokens.popIfPresent("->"))
                {
                    if (tokens.isNext("*"))
                    {
                        importTargetName = tokens.pop();
                    }
                    else
                    {
                        importTargetName = tokens.popName("import target variable");
                    }
                }

                tokens.popExpected(";");

                output.Add(new ImportStatement(importToken, tokenChain, importTargetName));
            }

            return output.ToArray();
        }
    }
}
