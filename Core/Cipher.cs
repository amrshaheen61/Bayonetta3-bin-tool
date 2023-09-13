using Helper;
using System.Text;

namespace Bayonetta3_bin_tool.Core
{
    public static class Cipher
    {
        public static int SetEncodeString(this IStream stream, string inputString)
        {
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(ReplaceBreaklines(inputString,true) + "\0");
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

            return ReplaceBreaklines(Encoding.Unicode.GetString(utf16Bytes).TrimEnd('\0'));
        }

        private static string ReplaceBreaklines(string StringValue, bool Back = false)
        {
            if (!Back)
            {
                StringValue = StringValue.Replace("\r\n", "<cf>");
                StringValue = StringValue.Replace("\r", "<cr>");
                StringValue = StringValue.Replace("\n", "<lf>");
            }
            else
            {
                StringValue = StringValue.Replace("<cf>", "\r\n");
                StringValue = StringValue.Replace("<cr>", "\r");
                StringValue = StringValue.Replace("<lf>", "\n");
            }

            return StringValue;
        }
    }
}
