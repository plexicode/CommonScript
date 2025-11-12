using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class ImageResource
    {
        public bool isJpeg;
        public int width;
        public int height;
        public object handle;

        public ImageResource(bool isJpeg, int width, int height, object handle)
        {
            this.isJpeg = isJpeg;
            this.width = width;
            this.height = height;
            this.handle = handle;
        }
    }
}
