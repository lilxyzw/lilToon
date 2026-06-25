using System.Collections.Generic;

namespace jp.lilxyzw.gifparser
{
    internal sealed class GifFile
    {
        public int Width;
        public int Height;

        public ByteColor[] GlobalColorTable;

        public readonly List<GifFrame> Frames =
            new List<GifFrame>();
    }

    internal sealed class GifFrame
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;

        public bool Interlaced;

        public ByteColor[] LocalColorTable;

        public byte[] CompressedImageData;

        public byte LzwMinimumCodeSize;

        public int DelayTime;

        public int DisposalMethod;

        public int TransparentIndex = -1;
    }

    internal struct ByteColor
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public ByteColor(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    internal sealed class GifBitmapFrame
    {
        public int Width;
        public int Height;

        public ByteColor[] Pixels;

        public int DelayTime;

        public ByteColor GetPixel(int x, int y) => Pixels[x + y * Width];
    }
}
