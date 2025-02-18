﻿using System;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class TokenStreamUtil
    {
        public static Token Tokens_peekAhead(TokenStream tokens, int distance)
        {
            if (tokens.index + distance < tokens.length)
            {
                return tokens.tokens[tokens.index + distance];
            }
            return null;
        }

        public static Token Tokens_pop(TokenStream tokens)
        {
            if (tokens.index >= tokens.length) return null;
            Token t = tokens.tokens[tokens.index];
            tokens.index++;
            return t;
        }

        public static Token Tokens_peek(TokenStream tokens)
        {
            if (tokens.index >= tokens.length) return null;
            return tokens.tokens[tokens.index];
        }

        public static string Tokens_peekValue(TokenStream tokens)
        {
            Token t = Tokens_peek(tokens);
            if (t == null) return null;
            return t.Value;
        }

        public static string Tokens_peekValueNonNull(TokenStream tokens)
        {
            return Tokens_peekValue(tokens) ?? "";
        }

        public static bool Tokens_hasMore(TokenStream tokens)
        {
            return tokens.index < tokens.length;
        }

        public static void Tokens_ensureMore(TokenStream tokens)
        {
            if (!Tokens_hasMore(tokens))
            {
                FunctionWrapper.Errors_ThrowEof(tokens.file, "Unexpected end of file");
            }
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
            if (val1 != null && tokens.index < tokens.length && tokens.tokens[tokens.index].Value != val1) return false;
            if (val2 != null && tokens.index + 1 < tokens.length && tokens.tokens[tokens.index + 1].Value != val2) return false;
            if (val3 != null && tokens.index + 2 < tokens.length && tokens.tokens[tokens.index + 2].Value != val3) return false;
            if (val4 != null && tokens.index + 3 < tokens.length && tokens.tokens[tokens.index + 3].Value != val4) return false;
            return true;
        }

        public static bool Tokens_isNext(TokenStream tokens, string value)
        {
            return Tokens_peekValue(tokens) == value;
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

        public static bool Tokens_doesNextInclude5(TokenStream tokens, string val1, string val2, string val3, string val4, string val5)
        {
            string next = Tokens_peekValue(tokens);
            return next == val1 || next == val2 || next == val3 || next == val4 || next == val5;
        }

        public static Token Tokens_popKeyword(TokenStream tokens, string value)
        {
            Token next = Tokens_pop(tokens);
            if (next == null) Tokens_ensureMore(tokens); // throw
            if (next.Value != value || next.Type != (int) TokenType.KEYWORD)
            {
                FunctionWrapper.Errors_Throw(next, "Expected '" + value + "' keyword but found '" + next.Value + "' instead.");
            }
            return next;
        }

        public static bool Tokens_popIfPresent(TokenStream tokens, string value)
        {
            if (Tokens_peekValue(tokens) == value)
            {
                tokens.index++;
                return true;
            }
            return false;
        }

        public static Token Tokens_popName(TokenStream tokens, string purposeForErrorMessage)
        {
            Token t = Tokens_pop(tokens);
            if (t == null) Tokens_ensureMore(tokens); // throw 
            if (t.Type != (int) TokenType.NAME) FunctionWrapper.Errors_Throw(t, "Expected " + purposeForErrorMessage + " but found '" + t.Value + "' instead.");
            return t;
        }

        public static Token Tokens_popExpected(TokenStream tokens, string value)
        {
            Token output = Tokens_pop(tokens);
            if (output == null) Tokens_ensureMore(tokens); // throw 
            if (output.Value != value) FunctionWrapper.Errors_Throw(output, "Expected '" + value + "' but found '" + output.Value + "' instead.");
            
            // this is an internal assert and not a user error. Feel free to remove later.
            if (output.Type == (int) TokenType.KEYWORD) throw new Exception("Use popKeyword instead.");
            
            return output;
        }

        public static int Tokens_peekType(TokenStream tokens)
        {
            if (!Tokens_hasMore(tokens)) return (int) TokenType.EOF;
            return Tokens_peek(tokens).Type;
        }
    }
}
