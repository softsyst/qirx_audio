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

//#define LINUX


using System;
using System.Runtime.InteropServices;


namespace softsyst.qirx.PInvoke
{
    static public class libfaadCalls
    {
#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecOpen", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecOpen", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern public static IntPtr NeAACDecOpen();


#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecClose", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecClose", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern public static void NeAACDecClose(IntPtr hDecoder);


#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecInit2", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecInit2", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static byte NeAACDecInit2(
            IntPtr hDecoder,
            byte* pBuffer,
            Int32 SizeOfDecoderSpecificInfo,
            ref Int32 samplerate,
            ref byte channels
            );


#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecInitDRM", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecInitDRM", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static int NeAACDecInitDRM(
            IntPtr hDecoder,
            Int32 samplerate,
            byte channels
            );

#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecInit", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecInit", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static int NeAACDecInit(
            IntPtr hDecoder,
            byte* pBuffer,
            Int32 buffer_size,
            ref Int32 samplerate,
            ref byte channels,
            byte shortFrameLength   // 1: 960 byte frames
            );



#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecGetErrorMessage", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecGetErrorMessage", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static Byte* NeAACDecGetErrorMessage(byte errcode);



#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecDecode", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecDecode", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static Byte* NeAACDecDecode(
            IntPtr hDecoder,
            ref PInvoke_libfaad2.frameInfo frameInfo,
            Byte* buffer,
            Int32 buffer_size
            );


#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecDecode2", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecDecode2", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static Byte* NeAACDecDecode2(
            IntPtr hDecoder,
            ref PInvoke_libfaad2.frameInfo frameInfo,
            Byte* buffer,
            Int32 buffer_size,
            Byte** pOutbuffer,
            Int32 out_buffer_size
            );



#if LINUX
        [DllImport("libfaad.so", EntryPoint = "NeAACDecGetCurrentConfiguration", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("libfaad2.dll", EntryPoint = "NeAACDecGetCurrentConfiguration", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static Byte* NeAACDecGetCurrentConfiguration(IntPtr hDecoder);
    }


    public class PInvoke_libfaad2
    {
        //        long init_result = NeAACDecInit2 (aacHandle,
        //                                                asc,
        //                                                sizeof (asc),
        //                                                &sample_rate,
        //                                                &channels);
        //            NeAACDecClose (aacHandle);
        //    outBuffer = (Int16 *)NeAACDecDecode (aacHandle,
        //                                            &hInfo, buffer, bufferLength);
        //                                NeAACDecGetErrorMessage (-init_result));
        //    NeAACDecFrameInfo	hInfo;
        public frameInfo hinfo = new frameInfo();
        public configuration config = new configuration();
        //PInvoke_libfaad2()
        //{

        //}
        [StructLayout(LayoutKind.Sequential)]
        unsafe public struct frameInfo
        {
#if LINUX
            public UInt64 bytesconsumed;
            public UInt64 samples;
#else
            public UInt32 bytesconsumed;
            public UInt32 samples;
#endif
            public byte channels;
            public byte error;
            public UInt32 samplereate;

            /* SBR: 0: off, 1: on; upsample, 2: on; downsampled, 3: off; upsampled */
            public byte sbr;

            /* MPEG-4 ObjectType */
            public byte object_type;

            /* AAC header type; MP4 will be signalled as RAW also */
            public byte header_type;

            /* multichannel configuration */
            public byte num_front_channels;
            public byte num_side_channels;
            public byte num_back_channels;
            public byte num_lfe_channels;
            public fixed byte channel_position[64];

            /* PS: 0: off, 1: on */
            byte ps;
        }

        [StructLayout(LayoutKind.Sequential)]
        unsafe public struct configuration
        {
            public byte defObjectType;
            public int defSampleRate;
            public byte outputFormat;
            public byte downMatrix;
            public byte useOldADTSFormat;
            public byte dontUpSampleImplicitSBR;
        }
    }
}
