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

using softsyst.qirx.PInvoke;
using System;

namespace softsyst.qirx.audiodecoder
{
    /// <summary>
    /// Wrapper to the kjmp2.dll
    /// </summary>
    public unsafe class mp2Decoder  
    {

        const int KJMP_SAMPLES_PER_FRAME = 1152;

        /// <summary>
        /// Private for the singleton
        /// </summary>
        private mp2Decoder( )
        {
            kjmp2Calls.mp2_initialize();
        }

        /// <summary>
        /// The single instance
        /// </summary>
        static mp2Decoder decoder;

        /// <summary>
        /// Singleton
        /// </summary>
        static public mp2Decoder instance
        {
            get
            {
                if (decoder == null)
                    decoder = new mp2Decoder();
                return decoder;
            }
        }

        public int getSampleRate(byte[] buffer)
        {
            try
            {
                fixed (Byte* p = &buffer[0])
                {
                    return kjmp2Calls.mp2_getSampleRate(p);
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public byte[]  decode(byte[] buffer )
        {
            byte[] pcm_buf = new byte[KJMP_SAMPLES_PER_FRAME*4];

            try
            {
                fixed (Byte* p = &buffer[0])
                {
                    fixed(byte*pp = &pcm_buf[0])
                        kjmp2Calls.mp2_decode(p, buffer.Length, pp);
                }
            }
            catch (Exception)
            {
            }
            return pcm_buf;
        }
    }
}
