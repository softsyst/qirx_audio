#region COPYRIGHT: Public Domain
using System;
using System.Collections.Generic;
using System.Text;

namespace softsyst.Generic
{
    /// <summary>
    /// Static class to Dump a byte array to a string
    /// </summary>
    public static class HexUtil
    {
        /// <summary>
        /// Creates a hex dump of a byte array.
        /// Useful for dumping protocol streams
        /// </summary>
        /// <param name="bytes">Byte array to dump</param>
        /// <param name="length">Length of the array to read</param>
        /// <param name="bytesPerLine">Bytes per line to output in representation</param>
        /// <returns>Hexadecimal representation</returns>
        public static string BytesToHex(byte[] bytes, int length, int bytesPerLine)
        {
            bool even = false;
            if ((bytesPerLine % 2) == 0)    //even bytesPerLine: insert separator in the middle
                even = true;
            if (bytes == null || length == 0)
                return "";
            StringBuilder buffer = new StringBuilder("");

            for (int offset = 0; offset < length; offset += bytesPerLine)
            {
                buffer.AppendFormat("{0:X8}   ", offset);
                int numBytes = Math.Min(bytesPerLine, length - offset);

                // append all bytes in the line in hex
                for (int i = 0; i < numBytes; i++)
                {
                    if (even && (i == (bytesPerLine / 2)))
                        buffer.Append("- ");
                    buffer.AppendFormat("{0:X2} ", bytes[offset + i]);
                }
                buffer.Append(new string(' ', ((bytesPerLine - numBytes) * 3) + 3));

                for (int i = 0; i < numBytes; i++)
                {
                    buffer.Append(ByteToChar(bytes, offset + i));
                }

                // new line
                buffer.Append(Environment.NewLine);
            }
            return buffer.ToString();
        }


        /// <summary>
        /// Takes a byte from an array and returns
        /// its character representation.
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <param name="length">Length to take into account</param>
        /// <returns>Hex string</returns>
        public static string BytesToHex(byte[] bytes, int length)
        {
            if (bytes == null || bytes.Length == 0)
                return "";
            StringBuilder buffer = new StringBuilder("");

            for (int i = 0; i < length; i++)
            {
                // append all bytes in the string in hex
                buffer.AppendFormat("{0:X2} ", bytes[i]);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Converts a byte to a char
        /// </summary>
        /// <param name="b">Byte array</param>
        /// <param name="offset">Index into the byte array</param>
        /// <returns>char representation</returns>
        /// <remarks>If it's a visible ASCII character, returns it
        ///otherwise returns '.' </remarks>
        static char ByteToChar(byte[] b, int offset)
        {
            byte theByte = b[offset];
            if (theByte < 32 || theByte > 127)
                return '.';
            char[] chars = Encoding.ASCII.GetChars(b, offset, 1);
            return chars[0];
        }
    } // class HexUtil
}
#endregion

