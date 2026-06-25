using System;
using System.Collections.Generic;

namespace jp.lilxyzw.gifparser
{
    internal static class GifFrameComposer
    {
        public static List<GifBitmapFrame> Compose(
            GifFile gif)
        {
            List<GifBitmapFrame> result =
                new List<GifBitmapFrame>();

            int canvasWidth =
                gif.Width;

            int canvasHeight =
                gif.Height;

            ByteColor transparent =
                new ByteColor(0, 0, 0, 0);

            ByteColor[] canvas =
                new ByteColor[
                    canvasWidth *
                    canvasHeight];

            for (int i = 0; i < canvas.Length; i++)
                canvas[i] = transparent;

            foreach (GifFrame frame in gif.Frames)
            {
                ByteColor[] beforeFrame =
                    (ByteColor[])canvas.Clone();

                RenderFrame(
                    gif,
                    frame,
                    canvas);

                GifBitmapFrame bitmap =
                    new GifBitmapFrame();

                bitmap.Width =
                    canvasWidth;

                bitmap.Height =
                    canvasHeight;

                bitmap.DelayTime =
                    frame.DelayTime;

                bitmap.Pixels =
                    (ByteColor[])canvas.Clone();

                result.Add(bitmap);

                ApplyDisposal(
                    gif,
                    frame,
                    canvas,
                    beforeFrame,
                    transparent);
            }

            return result;
        }

        static void RenderFrame(
            GifFile gif,
            GifFrame frame,
            ByteColor[] canvas)
        {
            ByteColor[] colorTable =
                frame.LocalColorTable ??
                gif.GlobalColorTable;

            byte[] indices =
                GifLzwDecoder.Decode(
                    frame.CompressedImageData,
                    frame.LzwMinimumCodeSize,
                    frame.Width * frame.Height);

            if (frame.Interlaced)
            {
                indices =
                    Deinterlace(
                        indices,
                        frame.Width,
                        frame.Height);
            }

            int src = 0;

            for (int y = 0; y < frame.Height; y++)
            {
                int dstY =
                    frame.Top + y;

                if (dstY < 0)
                    continue;

                if (dstY >= gif.Height)
                    continue;

                for (int x = 0; x < frame.Width; x++)
                {
                    int dstX =
                        frame.Left + x;

                    if (dstX < 0)
                    {
                        src++;
                        continue;
                    }

                    if (dstX >= gif.Width)
                    {
                        src++;
                        continue;
                    }

                    byte index =
                        indices[src++];

                    if (index ==
                        frame.TransparentIndex)
                    {
                        continue;
                    }

                    canvas[
                        dstY * gif.Width +
                        dstX] =
                        colorTable[index];
                }
            }
        }

        static void ApplyDisposal(
            GifFile gif,
            GifFrame frame,
            ByteColor[] canvas,
            ByteColor[] beforeFrame,
            ByteColor transparent)
        {
            switch (frame.DisposalMethod)
            {
                case 0:
                case 1:
                    return;

                case 2:
                    ClearFrameArea(
                        frame,
                        gif.Width,
                        gif.Height,
                        canvas,
                        transparent);
                    return;

                case 3:
                    System.Array.Copy(
                        beforeFrame,
                        canvas,
                        canvas.Length);
                    return;
            }
        }

        static void ClearFrameArea(
            GifFrame frame,
            int canvasWidth,
            int canvasHeight,
            ByteColor[] canvas,
            ByteColor clearColor)
        {
            int startX = Math.Max(frame.Left, 0);
            int startY = Math.Max(frame.Top, 0);

            int endX =
                Math.Min(
                    frame.Left + frame.Width,
                    canvasWidth);

            int endY =
                Math.Min(
                    frame.Top + frame.Height,
                    canvasHeight);

            for (int y = startY; y < endY; y++)
            {
                int row = y * canvasWidth;

                for (int x = startX; x < endX; x++)
                {
                    canvas[row + x] =
                        clearColor;
                }
            }
        }

        static byte[] Deinterlace(
            byte[] source,
            int width,
            int height)
        {
            byte[] output =
                new byte[source.Length];

            int src = 0;

            void CopyPass(
                int start,
                int step)
            {
                for (int y = start;
                    y < height;
                    y += step)
                {
                    int dst =
                        y * width;

                    for (int x = 0;
                        x < width;
                        x++)
                    {
                        output[dst + x] =
                            source[src++];
                    }
                }
            }

            CopyPass(0, 8);
            CopyPass(4, 8);
            CopyPass(2, 4);
            CopyPass(1, 2);

            return output;
        }
    }
}
