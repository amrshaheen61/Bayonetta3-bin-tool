﻿using Helper;
using System.Text;

namespace Bayonetta3_bin_tool.Core
{
    public static class Cipher
    {
        public static int SetEncodeString(this IStream stream, string inputString)
        {
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(inputString + "\0");
            byte[] encodedBytes = new byte[utf16Bytes.Length];

            for (int i = 0; i < utf16Bytes.Length; i++)
            {
                encodedBytes[i] = (byte)((utf16Bytes[i] + 0x26) % 0x100);
            }
            stream.SetBytes(encodedBytes);
            return encodedBytes.Length;
        }

        public static string GetEncodeString(this IStream stream, int lenght)
        {
            var utf16Bytes = stream.GetBytes(lenght);

            for (int i = 0; i < utf16Bytes.Length; i++)
            {
                utf16Bytes[i] = (byte)((utf16Bytes[i] - 0x26) % 0x100);
            }

            return Encoding.Unicode.GetString(utf16Bytes).TrimEnd('\0');
        }
    }
}
