using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ModuleResource
    {
        public string path;
        public int type;
        public string textPayload;
        public int[] binaryPayload;
        public ImageResource imagePayload;

        public ModuleResource(string path, int type, string textPayload, int[] binaryPayload, ImageResource imagePayload)
        {
            this.path = path;
            this.type = type;
            this.textPayload = textPayload;
            this.binaryPayload = binaryPayload;
            this.imagePayload = imagePayload;
        }
    }
}
