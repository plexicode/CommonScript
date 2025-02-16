using System;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class TokenStream
    {
        private int index = 0;
        private int length;
        private Token[] tokens;
        private string file;

        public TokenStream(string file, Token[] tokens)
        {
            this.file = file;
            this.tokens = tokens;
            this.length = tokens.Length;
        }

        public string GetFile() { return this.file; }

        public Token PeekAhead(int distance)
        {
            if (this.index + distance < this.length)
            {
                return this.tokens[this.index + distance];
            }
            return null;
        }

        public Token pop()
        {
            if (this.index >= this.length) return null;
            Token t = this.tokens[this.index];
            this.index++;
            return t;
        }

        public Token peek()
        {
            if (this.index >= this.length) return null;
            return this.tokens[this.index];
        }

        public string peekValue()
        {
            Token t = this.peek();
            if (t == null) return null;
            return t.Value;
        }

        public string peekValueNonNull()
        {
            return this.peekValue() ?? "";
        }

        public bool hasMore()
        {
            return this.index < this.length;
        }

        public void ensureMore()
        {
            if (!this.hasMore())
            {
                Errors.ThrowEof(this.file, "Unexpected end of file");
            }
        }

        public bool isSequenceNext2(string val1, string val2)
        {
            return isSequenceNext4(val1, val2, null, null);
        }

        public bool isSequenceNext3(string val1, string val2, string val3)
        {
            return isSequenceNext4(val1, val2, val3, null);
        }

        public bool isSequenceNext4(string val1, string val2, string val3, string val4)
        {
            if (val1 != null && this.index < this.length && this.tokens[this.index].Value != val1) return false;
            if (val2 != null && this.index + 1 < this.length && this.tokens[this.index + 1].Value != val2) return false;
            if (val3 != null && this.index + 2 < this.length && this.tokens[this.index + 2].Value != val3) return false;
            if (val4 != null && this.index + 3 < this.length && this.tokens[this.index + 3].Value != val4) return false;
            return true;
        }

        public bool isNext(string value)
        {
            return this.peekValue() == value;
        }

        public bool doesNextInclulde2(string val1, string val2)
        {
            return this.doesNextInclulde4(val1, val2, null, null);
        }

        public bool doesNextInclulde3(string val1, string val2, string val3)
        {
            return this.doesNextInclulde4(val1, val2, val3, null);
        }

        public bool doesNextInclulde4(string val1, string val2, string val3, string val4)
        {
            string next = this.peekValue();
            return next == val1 || next == val2 || next == val3 || next == val4;
        }

        public bool doesNextInclude5(string val1, string val2, string val3, string val4, string val5)
        {
            string next = this.peekValue();
            return next == val1 || next == val2 || next == val3 || next == val4 || next == val5;
        }

        public Token popKeyword(string value)
        {
            Token next = this.pop();
            if (next == null) this.ensureMore(); // throw
            if (next.Value != value || next.Type != (int) TokenType.KEYWORD)
            {
                Errors.ThrowError(next, "Expected '" + value + "' keyword but found '" + next.Value + "' instead.");
            }
            return next;
        }

        public bool popIfPresent(string value)
        {
            if (this.peekValue() == value)
            {
                this.index++;
                return true;
            }
            return false;
        }

        public Token popName(string purposeForErrorMessage)
        {
            Token t = this.pop();
            if (t == null) this.ensureMore(); // throw 
            if (t.Type != (int) TokenType.NAME) Errors.ThrowError(t, "Expected " + purposeForErrorMessage + " but found '" + t.Value + "' instead.");
            return t;
        }

        public Token popExpected(string value)
        {
            Token output = this.pop();
            if (output == null) this.ensureMore(); // throw 
            if (output.Value != value) Errors.ThrowError(output, "Expected '" + value + "' but found '" + output.Value + "' instead.");
            
            // this is an internal assert and not a user error. Feel free to remove later.
            if (output.Type == (int) TokenType.KEYWORD) throw new Exception("Use popKeyword instead.");
            
            return output;
        }

        public int peekType()
        {
            if (!this.hasMore()) return (int) TokenType.EOF;
            return this.peek().Type;
        }
    }
}
