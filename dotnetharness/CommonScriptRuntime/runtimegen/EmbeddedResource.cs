using System.Collections.Generic;

namespace CommonScript.Runtime.Internal
{
    public class EmbeddedResource
    {
        public string moduleId;
        public string path;
        public int type;
        public object payload;
        public int currentState;

        public EmbeddedResource(string moduleId, string path, int type, object payload, int currentState)
        {
            this.moduleId = moduleId;
            this.path = path;
            this.type = type;
            this.payload = payload;
            this.currentState = currentState;
        }
    }
}
