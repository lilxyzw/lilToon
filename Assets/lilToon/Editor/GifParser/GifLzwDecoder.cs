using System;
using System.Collections.Generic;

namespace jp.lilxyzw.gifparser
{
    internal static class GifLzwDecoder
    {
        private sealed class BitReader
        {
            private readonly byte[] _data;
            private int _bitPosition;

            public BitReader(byte[] data)
            {
                _data = data;
            }

            public int ReadBits(int count)
            {
                int value = 0;

                for (int i = 0; i < count; i++)
                {
                    int byteIndex = _bitPosition >> 3;

                    if (byteIndex >= _data.Length)
                        return -1;

                    int bitIndex = _bitPosition & 7;

                    int bit = (_data[byteIndex] >> bitIndex) & 1;

                    value |= bit << i;

                    _bitPosition++;
                }

                return value;
            }
        }

        public static byte[] Decode(
            byte[] compressedData,
            int minCodeSize,
            int expectedPixelCount)
        {
            int clearCode = 1 << minCodeSize;
            int endCode = clearCode + 1;

            int nextCode;

            int codeSize;
            int maxCode;

            List<byte> output = new List<byte>(expectedPixelCount);

            BitReader reader = new BitReader(compressedData);

            List<byte>[] dictionary = new List<byte>[4096];

            void ResetDictionary()
            {
                for (int i = 0; i < dictionary.Length; i++)
                    dictionary[i] = null;

                int count = 1 << minCodeSize;

                for (int i = 0; i < count; i++)
                {
                    dictionary[i] = new List<byte>(1)
                    {
                        (byte)i
                    };
                }

                nextCode = endCode + 1;

                codeSize = minCodeSize + 1;

                maxCode = (1 << codeSize) - 1;
            }

            ResetDictionary();

            int previousCode = -1;

            while (true)
            {
                int code = reader.ReadBits(codeSize);

                if (code < 0)
                    break;

                if (code == clearCode)
                {
                    ResetDictionary();
                    previousCode = -1;
                    continue;
                }

                if (code == endCode)
                {
                    break;
                }

                List<byte> entry;

                if (code < nextCode && dictionary[code] != null)
                {
                    entry = dictionary[code];
                }
                else if (code == nextCode && previousCode >= 0)
                {
                    List<byte> prev = dictionary[previousCode];

                    entry = new List<byte>(prev.Count + 1);

                    entry.AddRange(prev);
                    entry.Add(prev[0]);
                }
                else
                {
                    throw new Exception(
                        $"GIF LZW decode error. Invalid code {code}");
                }

                output.AddRange(entry);

                if (previousCode >= 0)
                {
                    List<byte> prev = dictionary[previousCode];

                    List<byte> newEntry =
                        new List<byte>(prev.Count + 1);

                    newEntry.AddRange(prev);
                    newEntry.Add(entry[0]);

                    if (nextCode < 4096)
                    {
                        dictionary[nextCode] = newEntry;

                        nextCode++;

                        if (nextCode > maxCode &&
                            codeSize < 12)
                        {
                            codeSize++;
                            maxCode = (1 << codeSize) - 1;
                        }
                    }
                }

                previousCode = code;

                if (output.Count >= expectedPixelCount)
                    break;
            }

            return output.ToArray();
        }
    }
}
