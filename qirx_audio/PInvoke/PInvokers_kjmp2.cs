/**
** ** QIRX - Software Defined Radio - AAC Audio  
** Copyright(C) 2017 Clem Schmidt, softsyst GmbH, http://www.softsyst.com
**
** This program is free software; you can redistribute it and/or modify
** it under the terms of the GNU General Public License as published by
** the Free Software Foundation; either version 2 of the License, or
** (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
** GNU General Public License for more details.
** 
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software
** Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
**
**/

using System;
using System.Runtime.InteropServices;

namespace softsyst.qirx.PInvoke
{
    static public class kjmp2Calls
    {
#if LINUX
        [DllImport("kjmp2.so", EntryPoint = "mp2_initialize", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("kjmp2.dll", EntryPoint = "mp2_initialize", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static int mp2_initialize();


#if LINUX
        [DllImport("kjmp2.so", EntryPoint = "mp2_getSampleRate", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("kjmp2.dll", EntryPoint = "mp2_getSampleRate", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static int mp2_getSampleRate(Byte* buf);

#if LINUX
        [DllImport("kjmp2.so", EntryPoint = "mp2_decode", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#else
        [DllImport("kjmp2.dll", EntryPoint = "mp2_decode", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
#endif
        extern unsafe public static int mp2_decode(Byte* buf, int len, Byte* pcm);

    }
}
