using System;
using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal static class Tokenizer
    {
        private enum TokenizerMode
        {
            READY,
            WORD,
            STRING,
            COMMENT,
        }

        private static Dictionary<char, bool> alphanumerics = null;
        private static Dictionary<char, bool> numeric = null;
        private static Dictionary<char, bool> whitespace = null;
        private static Dictionary<string, bool> keywords = null;
        private static Dictionary<string, bool> multicharTokens = null;

        static Tokenizer()
        {
            Tokenizer.alphanumerics = new Dictionary<char, bool>();
            Tokenizer.numeric = new Dictionary<char, bool>();
            Tokenizer.whitespace = new Dictionary<char, bool>();
            Tokenizer.keywords = new Dictionary<string, bool>();
            Tokenizer.multicharTokens = new Dictionary<string, bool>();

            for (int i = 0; i < 26; i++)
            {
                Tokenizer.alphanumerics[(char)('a' + i)] = true;
                Tokenizer.alphanumerics[(char)('A' + i)] = true;
                Tokenizer.alphanumerics['_'] = true;
                if (i < 10)
                {
                    Tokenizer.alphanumerics[(char)('0' + i)] = true;
                    Tokenizer.numeric[(char)('0' + i)] = true;
                }
            }

            string whitespace = " \r\n\t";
            for (int i = 0; i < whitespace.Length; i++)
            {
                Tokenizer.whitespace[whitespace[i]] = true;
            }

            string[] keywords = new string[] {
                "function", "class", "field", "property", "constructor", "const", "enum",
                "base", "this",
                "null", "false", "true", "new",
                "is", "typeof",
                "if", "else", "for", "while", "do", "break", "continue", "switch", "case", "default", "yield", "return",
                "throw", "try", "catch", "finally",
                "import", "namespace",
                "public", "static", "readonly", "abstract",
            };
            foreach (string keyword in keywords)
            {
                Tokenizer.keywords[keyword] = true;
            }

            string[] mcharTokens = "++ -- && || ** == != <= >= => -> << >> >>> ?? += -= *= %= /= <<= >>= >>>= |= &= ^= **=".Split(' ');
            foreach (string token in mcharTokens)
            {
                Tokenizer.multicharTokens[token] = true;
            }
        }

        public static TokenStream Tokenize(string file, string code)
        {
            string trimmedCode = code.Replace("\r\n", "\n").TrimEnd() + "\n\n\n\n\n"; // ample buffer ending for substring for multi-char tokens
            int length = trimmedCode.Length;
            int[] lines = new int[length];
            int[] cols = new int[length];
            int line = 1;
            int col = 1;
            for (int i = 0; i < length; i++)
            {
                lines[i] = line;
                cols[i] = col;
                if (trimmedCode[i] == '\n')
                {
                    line++;
                    col = 1;
                }
                else
                {
                    col++;
                }
            }

            TokenizerMode mode = TokenizerMode.READY;
            int tokenStart = 0;
            char tokenSubtype = ' ';
            char c;
            string tokenVal;
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < length; i++)
            {
                c = trimmedCode[i];
                switch (mode)
                {
                    case TokenizerMode.READY:
                        if (Tokenizer.whitespace.ContainsKey(c))
                        {
                            // do nothing 
                        }
                        else if (Tokenizer.alphanumerics.ContainsKey(c))
                        {
                            tokenStart = i;
                            mode = TokenizerMode.WORD;
                        }
                        else if (c == '"' || c == '\'')
                        {
                            tokenStart = i;
                            mode = TokenizerMode.STRING;
                            tokenSubtype = c;
                        }
                        else if (c == '/' && (trimmedCode[i + 1] == '/' || trimmedCode[i + 1] == '*'))
                        {
                            mode = TokenizerMode.COMMENT;
                            tokenSubtype = trimmedCode[i + 1];
                            i++;
                        }
                        else
                        {
                            if (c == '>' && trimmedCode.Substring(i, 4) == ">>>=")
                            {
                                tokenVal = ">>>=";
                            }
                            else
                            {
                                tokenVal = trimmedCode.Substring(i, 3);
                                if (!Tokenizer.multicharTokens.ContainsKey(tokenVal))
                                {
                                    tokenVal = trimmedCode.Substring(i, 2);
                                    if (!Tokenizer.multicharTokens.ContainsKey(tokenVal))
                                    {
                                        tokenVal = c + "";
                                    }
                                }

                            }

                            tokens.Add(new Token(tokenVal, (int) TokenType.PUNCTUATION, file, lines[i], cols[i]));
                            i += tokenVal.Length - 1;
                        }

                        break;

                    case TokenizerMode.WORD:
                        if (!Tokenizer.alphanumerics.ContainsKey(c))
                        {
                            tokenVal = trimmedCode.Substring(tokenStart, i - tokenStart);
                            int ttype = Tokenizer.keywords.ContainsKey(tokenVal)
                                ? (int) TokenType.KEYWORD
                                : Tokenizer.numeric.ContainsKey(tokenVal[0])
                                    ? tokenVal.ToLower().StartsWith("0x")
                                        ? (int) TokenType.HEX_INTEGER
                                        : (int) TokenType.INTEGER
                                    : (int) TokenType.NAME;
                            tokens.Add(new Token(tokenVal, ttype, file, lines[tokenStart], cols[tokenStart]));
                            i--;
                            mode = TokenizerMode.READY;
                        }
                        break;

                    case TokenizerMode.COMMENT:
                        if (tokenSubtype == '*')
                        {
                            if (c == '*' && trimmedCode[i + 1] == '/')
                            {
                                i++;
                                mode = TokenizerMode.READY;
                            }
                        }
                        else if (tokenSubtype == '/')
                        {
                            if (c == '\n')
                            {
                                mode = TokenizerMode.READY;
                            }
                        }
                        break;

                    case TokenizerMode.STRING:
                        if (c == '\\')
                        {
                            i++; // verify character escape sequences later
                        }
                        else if (c == tokenSubtype)
                        {
                            tokenVal = trimmedCode.Substring(tokenStart, i - tokenStart + 1);
                            tokens.Add(new Token(tokenVal, (int) TokenType.STRING, file, lines[tokenStart], cols[tokenStart]));
                            mode = TokenizerMode.READY;
                        }
                        break;
                }
            }

            if (mode != TokenizerMode.READY) throw new Exception("Unexpected end of file");

            for (int i = 0; i < tokens.Count; i++)
            {
                Token current = tokens[i];
                if (current != null && current.Value == ".")
                {
                    Token left = i > 0 ? tokens[i - 1] : null;
                    Token right = i + 1 < tokens.Count ? tokens[i + 1] : null;

                    if (left != null &&
                        left.Type == (int) TokenType.INTEGER &&
                        left.Line == current.Line &&
                        left.Col + left.Value.Length == current.Col)
                    {
                        left.Value += ".";
                        left.Type = (int) TokenType.FLOAT;
                        tokens[i - 1] = null;
                        tokens[i] = left;
                        current = left;
                    }

                    if (right != null &&
                        right.Type == (int) TokenType.INTEGER &&
                        right.Line == current.Line &&
                        current.Col + current.Value.Length == right.Col)
                    {
                        current.Value += right.Value;
                        current.Type = (int) TokenType.FLOAT;
                        tokens[i + 1] = null;
                    }
                }

                current = tokens[i];
                if (i + 1 < tokens.Count && current != null && current.Value == "@")
                {
                    Token next = tokens[i + 1];
                    if (next != null &&
                        next.Line == current.Line &&
                        next.Col == current.Col + 1 &&
                        next.Type == (int) TokenType.KEYWORD)
                    {
                        current.Value += next.Value;
                        current.Type = (int) TokenType.ANNOTATION;
                        tokens[i + 1] = null;
                    }
                }
            }

            List<Token> output = new List<Token>();
            foreach (Token t in tokens)
            {
                if (t != null)
                {
                    output.Add(t);
                }
            }

            return new TokenStream(file, output.ToArray());
        }
    }
}
