using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class Expression
    {
        public Token firstToken;
        public int type;
        public Expression root;
        public Expression left;
        public Expression right;
        public Token opToken;
        public bool boolVal;
        public string strVal;
        public int intVal;
        public double floatVal;
        public AbstractEntity entityPtr;
        public ImportStatement importPtr;
        public Expression[] args;
        public Expression[] keys;
        public Expression[] values;
        public Token[] argNames;
        public Statement[] nestedCode;

        public Expression(Token firstToken, int type, Expression root, Expression left, Expression right, Token opToken, bool boolVal, string strVal, int intVal, double floatVal, AbstractEntity entityPtr, ImportStatement importPtr, Expression[] args, Expression[] keys, Expression[] values, Token[] argNames, Statement[] nestedCode)
        {
            this.firstToken = firstToken;
            this.type = type;
            this.root = root;
            this.left = left;
            this.right = right;
            this.opToken = opToken;
            this.boolVal = boolVal;
            this.strVal = strVal;
            this.intVal = intVal;
            this.floatVal = floatVal;
            this.entityPtr = entityPtr;
            this.importPtr = importPtr;
            this.args = args;
            this.keys = keys;
            this.values = values;
            this.argNames = argNames;
            this.nestedCode = nestedCode;
        }
    }
}
