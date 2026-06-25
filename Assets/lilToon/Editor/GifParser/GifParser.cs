using System;
using System.IO;

namespace jp.lilxyzw.gifparser
{
    internal static class GifParser
    {
        public static GifFile Parse(byte[] data)
        {
            using (MemoryStream stream =
                new MemoryStream(data))
            using (BinaryReader reader =
                new BinaryReader(stream))
            {
                GifFile gif = new GifFile();

                ParseHeader(reader);

                ParseLogicalScreenDescriptor(
                    reader,
                    gif);

                GraphicControlState gc =
                    new GraphicControlState();

                while (stream.Position < stream.Length)
                {
                    byte introducer =
                        reader.ReadByte();

                    switch (introducer)
                    {
                        case 0x21:
                            ParseExtension(
                                reader,
                                gc);
                            break;

                        case 0x2C:
                            GifFrame frame =
                                ParseImage(
                                    reader,
                                    gc);

                            gif.Frames.Add(frame);

                            gc.Reset();
                            break;

                        case 0x3B:
                            return gif;

                        default:
                            throw new Exception(
                                $"Unknown block: 0x{introducer:X2}");
                    }
                }

                return gif;
            }
        }
        
        static void ParseHeader(
            BinaryReader reader)
        {
            string signature =
                new string(
                    reader.ReadChars(6));

            if (signature != "GIF87a" &&
                signature != "GIF89a")
            {
                throw new Exception(
                    "Not a GIF file.");
            }
        }
        
        static void ParseLogicalScreenDescriptor(
            BinaryReader reader,
            GifFile gif)
        {
            gif.Width = reader.ReadUInt16();
            gif.Height = reader.ReadUInt16();

            byte packed =
                reader.ReadByte();

            bool hasGlobalColorTable =
                (packed & 0x80) != 0;

            int colorTableSize =
                1 << ((packed & 0x07) + 1);

            reader.ReadByte();
            reader.ReadByte();

            if (hasGlobalColorTable)
            {
                gif.GlobalColorTable =
                    reader.ReadColorTable(
                        colorTableSize);
            }
        }

        sealed class GraphicControlState
        {
            public int DelayTime;
            public int DisposalMethod;
            public int TransparentIndex = -1;

            public void Reset()
            {
                DelayTime = 0;
                DisposalMethod = 0;
                TransparentIndex = -1;
            }
        }

        static void ParseGraphicControlExtension(
            BinaryReader reader,
            GraphicControlState gc)
        {
            reader.ReadByte();

            byte packed =
                reader.ReadByte();

            ushort delay =
                reader.ReadUInt16();

            byte transparentIndex =
                reader.ReadByte();

            reader.ReadByte();

            gc.DelayTime = delay;

            gc.DisposalMethod =
                (packed >> 2) & 0x07;

            bool transparent =
                (packed & 0x01) != 0;

            gc.TransparentIndex =
                transparent
                    ? transparentIndex
                    : -1;
        }

        static void ParseExtension(
            BinaryReader reader,
            GraphicControlState gc)
        {
            byte label =
                reader.ReadByte();

            switch (label)
            {
                case 0xF9:
                    ParseGraphicControlExtension(
                        reader,
                        gc);
                    break;

                default:
                    reader.ReadSubBlocks();
                    break;
            }
        }

        static GifFrame ParseImage(
            BinaryReader reader,
            GraphicControlState gc)
        {
            GifFrame frame =
                new GifFrame();

            frame.Left =
                reader.ReadUInt16();

            frame.Top =
                reader.ReadUInt16();

            frame.Width =
                reader.ReadUInt16();

            frame.Height =
                reader.ReadUInt16();

            byte packed =
                reader.ReadByte();

            bool hasLocalColorTable =
                (packed & 0x80) != 0;

            frame.Interlaced =
                (packed & 0x40) != 0;

            int localColorCount =
                1 << ((packed & 0x07) + 1);

            if (hasLocalColorTable)
            {
                frame.LocalColorTable =
                    reader.ReadColorTable(
                        localColorCount);
            }

            frame.LzwMinimumCodeSize =
                reader.ReadByte();

            frame.CompressedImageData =
                reader.ReadSubBlocks();

            frame.DelayTime =
                gc.DelayTime;

            frame.DisposalMethod =
                gc.DisposalMethod;

            frame.TransparentIndex =
                gc.TransparentIndex;

            return frame;
        }
    }
}
