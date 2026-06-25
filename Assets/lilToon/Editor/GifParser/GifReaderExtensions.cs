using System.IO;

namespace jp.lilxyzw.gifparser
{
    internal static class GifReaderExtensions
    {
        public static ByteColor[] ReadColorTable(
            this BinaryReader reader,
            int colorCount)
        {
            ByteColor[] table =
                new ByteColor[colorCount];

            for (int i = 0; i < colorCount; i++)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();

                table[i] = new ByteColor(r, g, b, 255);
            }

            return table;
        }

        public static byte[] ReadSubBlocks(
            this BinaryReader reader)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int size = reader.ReadByte();

                    if (size == 0)
                        break;

                    byte[] block = reader.ReadBytes(size);

                    ms.Write(block, 0, block.Length);
                }

                return ms.ToArray();
            }
        }
    }
}
