using System;
using System.Text;

namespace CommonScript.Compiler;

public class ImageResource
{
    internal byte[] Data { get; set; }
    internal bool IsValid { get; set; } = false;
    internal int Width { get; set; } = -1;
    internal int Height { get; set; } = -1;
    internal string Format { get; set; } = "";

    public ImageResource(byte[] data)
    {
        this.Data = data;
        this.Initialize();
    }

    private void Initialize()
    {
        if (this.IsMatch(0, [0x89]))
        {
            this.InitializePng();
        }
        else
        {
            this.InitializeJpeg();
        }
    }

    private static byte[] StringToUtf8Bytes(string s)
    {
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    private int GetSignedInt32LE(int offset, int fallback)
    {
        if (offset + 4 > this.Data.Length) return fallback;
        long result = 0;
        for (int i = 0; i < 4; i++)
        {
            long current = 0xFFFF & this.Data[i + offset];
            result = (result << 8) | this.Data[i + offset];
        }

        return result > 0x7FFFFFF ? (int)-result : (int)result;
    }

    private bool IsMatch(int offset, byte[] arr)
    {
        if (offset + arr.Length > this.Data.Length) return false;
        for (int i = 0; i < arr.Length; i++)
        {
            if (this.Data[offset + i] != arr[i]) return false;
        }

        return true;
    }

    private string GetAsciiString(int offset, int size)
    {
        StringBuilder sb = new StringBuilder();
        if (offset + size > this.Data.Length) return null;
        for (int i = 0; i < size; i++)
        {
            byte b = this.Data[offset + i];
            if (b < 32 || b >= 127) return null;
            sb.Append((char)b);
        }

        return sb.ToString();
    }

    private void InitializePng()
    {
        byte[] firstBytes =
        [
            0x89,
            ..StringToUtf8Bytes("PNG"),
            0x0D, 0x0A, 0x1A, 0x0A,
        ];
        if (!this.IsMatch(0, firstBytes)) return;

        int chunkSize = this.GetSignedInt32LE(8, -1);
        string chunkName = this.GetAsciiString(12, 4);
        if (chunkSize <= 0 || chunkName == null) return;

        this.Width = this.GetSignedInt32LE(16, -1);
        this.Height = this.GetSignedInt32LE(20, -1);
        if (this.Width <= 0 || this.Height <= 0) return;

        this.Format = "PNG";
        this.IsValid = true;
    }

    private void InitializeJpeg()
    {
        throw new NotImplementedException();
        this.Format = "JPEG";
        this.IsValid = true;
    }

    internal object ConvertToInternal()
    {
        return CommonScript.Compiler.Internal.FunctionWrapper.PUBLIC_buildVerifiedImageResourceDescriptor(
            this.Format,
            this.Width,
            this.Height,
            this.Data);
    }
}
