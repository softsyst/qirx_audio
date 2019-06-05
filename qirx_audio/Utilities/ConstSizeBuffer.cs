/**
** ** QIRX - Software Defined Radio - AAC Audio  
** Copyright (C) 2017 Clem Schmidt, softsyst GmbH, http://www.softsyst.com
**  
** This program is free software; you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation; either version 2 of the License, or
** (at your option) any later version.
** 
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
** 
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software 
** Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
**
**/

using System;
namespace softsyst.Generic
{
    /// <summary>
    /// Creates a constant size byte output buffer from various smaller buffer inputs
    /// E.g. used to create a const size buffer for the udp audio output
    /// </summary>
    public class ConstSizeBuffer
    {
        /// <summary>
        /// Arbitrary, but always constant bytes of the output buffer
        /// </summary>
        /// <remarks>AAC decodes into an array of rather arbitrary value. It is known after
        /// decode only. To have constant length array, bytes must be collected across 
        /// more than one decode, usually two or three.</remarks>
        int CONST_BUFFER_LENGTH = 10000;

        /// <summary>
        /// Array sent to the audio sink
        /// </summary>
        byte[] constLengthBuf;

        /// <summary>
        /// Next free index into the pcm16Buf array
        /// </summary>
        int pix = 0;

        public ConstSizeBuffer(int bufferLength)
        {
            CONST_BUFFER_LENGTH = bufferLength;
            constLengthBuf = new byte[CONST_BUFFER_LENGTH];
        }

        public void clear()
        {
            Array.Clear(constLengthBuf, 0, constLengthBuf.Length);
            pix = 0;
        }

        /// <summary>
        /// Creates the output buffer from more than one input
        /// </summary>
        /// <returns>The complete buffer, or null if still incomplete.</returns>
        /// <param name="src">Source.</param>
        /// <remarks>Always consumes a full input buffer</remarks>
        /// <remarks>The input buffer MUST be smaller than the constant length buffer</remarks>
        public byte[] createBuffer(byte[] src)
        {
            if (src == null)
                return null;
            if (src.Length == 0 || src.Length >= CONST_BUFFER_LENGTH)
                throw new Exception(string.Format("Input buffer invalid length {0}", src.Length));

            byte[] returnBuffer = null;

            //pix is  the index of the next free byte in constLengthBuf
            //Length of bytes to copy from the src array
            //It is always smaller than constLengthBuf.Length
            int cpylen = Math.Min(src.Length, constLengthBuf.Length - pix);

            //Copy into the destination array..
            Buffer.BlockCopy(src, 0, constLengthBuf, pix, cpylen);

            //.. adjust the index into the source array..
            int srcOffset = cpylen % src.Length;

            //.. and do likewise for the destination array
            pix += cpylen;
            if (pix > constLengthBuf.Length)
            {
                pix = 0;
                throw new Exception("Buffer copy length error");
            }

            //check if we have a complete dest array
            if (pix == constLengthBuf.Length)
            {
                //Return the completed buffer to the caller
                returnBuffer = constLengthBuf;

                // Get a new buffer..
                constLengthBuf = new byte[CONST_BUFFER_LENGTH]; //usually 10000, i.e. 5000 shorts

                //..copy the rest of the source ..
                int rest = src.Length - cpylen;
                Buffer.BlockCopy(src, srcOffset, constLengthBuf, 0, rest);

                //...adjust the index of the destination array
                pix = rest;
            }
            //The source array is consumed,
            // Return either null or a completed buffer of constant length
            return returnBuffer;
        }
    }
}
