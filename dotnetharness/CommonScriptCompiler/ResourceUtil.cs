using System.Linq;

namespace CommonScript.Compiler;

public enum ResourceType
{
    BINARY,
    TEXT,
    IMAGE,
}

public static class ResourceUtil
{
    private static byte[] PNG_PREFIX = [0x89, (byte)'P', (byte)'N', (byte)'G'];
    private static byte[] JPEG_PREFIX = [0xFF, 0xD8];
    private static byte[] UTF8_PREFIX = [0xEF, 0xBB, 0xBF];

    private static bool IsPrefixMatch(byte[] payload, byte[] prefix)
    {
        if (payload.Length < prefix.Length) return false;
        for (int i = 0; i < prefix.Length; i++)
        {
            if (payload[i] != prefix[i]) return false;
        }

        return true;
    }

    private static bool IsValidUtf8(byte[] payload)
    {
        int len = payload.Length;
        byte current;
        int followupSequenceSize;
        int start = 0;
        if (len >= 3 && IsPrefixMatch(payload, UTF8_PREFIX)) start = 3;
        for (int i = start; i < len; i++)
        {
            current = payload[i];
            if ((current & 0x80) == 0) followupSequenceSize = 0;
            else if ((current & 0xE0) == 0xC0) followupSequenceSize = 1;
            else if ((current & 0xF0) == 0xE0) followupSequenceSize = 2;
            else if ((current & 0xF8) == 0xF0) followupSequenceSize = 3;
            else return false;

            while (followupSequenceSize-- > 0)
            {
                i++;
                if (i >= len) return false;
                if ((payload[i] & 0xC0) != 0x80) return false;
            }
        }

        return true;
    }

    public static ResourceType CategorizeResource(string filePath, byte[] bytes)
    {
        string[] pathParts = filePath.Split('/');
        string name = pathParts.LastOrDefault();
        int dot = name.LastIndexOf('.');
        string ext = null;
        if (dot != -1)
        {
            ext = name.Substring(dot + 1).ToUpperInvariant();
        }

        if (IsValidUtf8(bytes)) return ResourceType.TEXT;

        switch (ext ?? "")
        {
            case "PNG":
                return IsPrefixMatch(bytes, PNG_PREFIX) ? ResourceType.IMAGE : ResourceType.BINARY;

            case "JPEG":
            case "JPG":
                return IsPrefixMatch(bytes, JPEG_PREFIX) ? ResourceType.IMAGE : ResourceType.BINARY;
        }

        return ResourceType.BINARY;
    }
}
