﻿using System.Collections.Generic;
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

                output.Add(FunctionWrapper.ImportStatement_new(importToken, tokenChain, importTargetName));
            }

            return output.ToArray();
        }
    }
}
