using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class AbstractEntity
    {
        public Token firstToken;
        public int type;
        public object specificData;
        public string simpleName;
        public Token nameToken;
        public string fqName;
        public System.Collections.Generic.Dictionary<string, Token> annotations;
        public bool isStatic;
        public AbstractEntity nestParent;
        public FileContext fileContext;
        public int serializationIndex;

        public AbstractEntity(Token firstToken, int type, object specificData, string simpleName, Token nameToken, string fqName, System.Collections.Generic.Dictionary<string, Token> annotations, bool isStatic, AbstractEntity nestParent, FileContext fileContext, int serializationIndex)
        {
            this.firstToken = firstToken;
            this.type = type;
            this.specificData = specificData;
            this.simpleName = simpleName;
            this.nameToken = nameToken;
            this.fqName = fqName;
            this.annotations = annotations;
            this.isStatic = isStatic;
            this.nestParent = nestParent;
            this.fileContext = fileContext;
            this.serializationIndex = serializationIndex;
        }
    }
}
