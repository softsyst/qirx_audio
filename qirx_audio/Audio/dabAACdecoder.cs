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

//#define AAC_TEST

using softsyst.Generic.Logger;
using softsyst.qirx.PInvoke;
using System;
using System.Runtime.InteropServices;
using System.IO;


namespace softsyst.qirx.audiodecoder
{
    /// <summary>
    /// Wrapper to the libfaad
    /// </summary>
    /// <remarks>Decoupling of the pinvoked calls enable the rest of the SW.</remarks>
    public unsafe class aacDecoder : IDisposable
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;
        /// <summary>

        PInvoke_libfaad2 pfaad = new PInvoke_libfaad2();

        int maxOutBufferLen = 10000;
        IntPtr pMem;

        unsafe public IntPtr open()
        {
            try
            {
                pMem = Marshal.AllocCoTaskMem(maxOutBufferLen);
                return libfaadCalls.NeAACDecOpen();
            }
			catch (Exception ex)
            {
				logger?.Error (ex.Message);
                return (IntPtr)0;
            }       
        }

        unsafe public void Close(IntPtr hDecoder)
        {
            try
            {
                libfaadCalls.NeAACDecClose(hDecoder );
                Marshal.FreeCoTaskMem((IntPtr)pMem);
#if AAC_TEST
                if (fs != null)
                    fs.Close();
                fs = null;
#endif
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
                throw;
            }       
        }

#if AAC_TEST
        FileStream fs;
        string aactestpath = @"D:\RawTestFile.aac";

#endif

        public unsafe int init(
                        IntPtr hDecoder,
                        byte[] buffer,
                        ref Int32 sampleRate,
                        ref byte channels
                        )
        {
#if AAC_TEST
            fs = new FileStream(aactestpath, FileMode.Create);
#endif

            try
            {
                fixed (byte* p = &buffer[0])
                {
                    int result = libfaadCalls.NeAACDecInit(hDecoder,
                                        p,
                                        buffer.Length,
                                        ref sampleRate,
                                        ref channels,
                                        1);             //1: 960 bytes frame length, mandatory for DAB
                    if (result != 0)
                    {
                        string s = string.Format("Error initializing decoder library: {0}", "");
                                           // NeAACDecGetErrorMessage(init_result));
                    }
                    PInvoke_libfaad2.configuration* pcfg = (PInvoke_libfaad2.configuration *)libfaadCalls.NeAACDecGetCurrentConfiguration(hDecoder);
                    byte b = pcfg->defObjectType;
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        public unsafe int init2(
                        IntPtr hDecoder,
                        byte[] buffer,
                        ref Int32 sampleRate,
                        ref byte channels
                        )
        {
#if AAC_TEST
            fs = new FileStream(aactestpath, FileMode.Create);
#endif

            try
            {
                fixed (byte* p = &buffer[2]) //index 2 because of header id bytes 0xff, 0xee
                {
                    int result = libfaadCalls.NeAACDecInit2(hDecoder,
                                        p,
                                        buffer.Length,
                                        ref sampleRate,
                                        ref channels);
                    if (result != 0)
                    {
                        string s = string.Format("Error initializing decoder library: {0}", "");
                                           // NeAACDecGetErrorMessage(init_result));
                    }
                    PInvoke_libfaad2.configuration* pcfg = (PInvoke_libfaad2.configuration *)libfaadCalls.NeAACDecGetCurrentConfiguration(hDecoder);
                    byte b = pcfg->defObjectType;
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        public unsafe int initDRM(
                        IntPtr hDecoder,
                        Int32 sampleRate,
                        byte channels
                        )
        {
            try
            {
                    int result = libfaadCalls.NeAACDecInitDRM(hDecoder,
                                        sampleRate,
                                        channels);
                    if (result != 0)
                    {
                        string s = string.Format("Error initializing decoder library: {0}", "");
                                           // NeAACDecGetErrorMessage(init_result));
                    }
                    PInvoke_libfaad2.configuration* pcfg = (PInvoke_libfaad2.configuration *)libfaadCalls.NeAACDecGetCurrentConfiguration(hDecoder);
                    byte b = pcfg->defObjectType;
                    
                    return result;
                }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        public unsafe string getErrorMessage(byte errCode)
        {
            int len = 256;
            byte[] arr = new byte[len];
            string result;
            fixed (Byte* pp = &arr[0])
            {
                Byte* p = libfaadCalls.NeAACDecGetErrorMessage(errCode);
                if (p == (Byte*)0)
                {
                    return string.Format("NeAACDecGetErrorMessage failed. Error Code was {0}", errCode);
                }
                for (int i = 0; i < len; i++)
                    pp[i] = p[i];
                result = System.Text.Encoding.Default.GetString(arr, 0, len);
            }
            return result;
        }

        /// <summary>
        /// AAC Decoder using the libfaad2 dll
        /// </summary>
        /// <param name="hDecoder">libfaad2 handle</param>
        /// <param name="buffer">Input to the decoder, including leading bytes</param>
        /// <param name="startIx">Start index into the input buffer. Here start the raw encoded bytes. 
        /// The leading information is the au_start prefix. Both, header and encoded data are provided by the RS decoder</param>
        /// <param name="decoded">Decoded output buffer</param>
        /// <param name="bytesConsumed">Information from the decoder</param>
        /// <returns></returns>
        public unsafe int decode(IntPtr hDecoder, byte[] buffer, int startIx, out byte[] decoded, 
           out int samplingRate, out int channels, out int objectType)
        {
            samplingRate = channels = objectType = 0;
#if AAC_TEST
            if (fs != null)
            {
                fs.Write(buffer, 0, buffer.Length);
            }
#endif
            int retval = -1;

            decoded = null;
            try
            {
                fixed (Byte* p = &buffer[startIx])
                {
                    fixed (IntPtr* ppMem = &pMem)
                    {
                        c_libraryCalls.memset(pMem, 0, maxOutBufferLen);
                        //This version allows for providing own managed memory as the output buffer
                        //This is important for avoiding memory leaks.
                        //That memory is once allocated in the constructor of always sufficient size, and released on disposing.
                        Byte* pMemAlias = libfaadCalls.NeAACDecDecode2(hDecoder,
                                                  ref pfaad.hinfo, p, buffer.Length-startIx,
                                                  (Byte**)ppMem, maxOutBufferLen);
                        samplingRate = (int)pfaad.hinfo.samplereate;
                        channels = (int)pfaad.hinfo.channels;
                        objectType = (int)pfaad.hinfo.object_type;

                        Byte* pp;
                        if (pMemAlias == (Byte*)0)
                            pp = libfaadCalls.NeAACDecGetErrorMessage(pfaad.hinfo.error);

                        //samples are -per default - in Int16 units, not in byte units
                        if (pfaad.hinfo.error == 0 && pfaad.hinfo.samples != 0)
                        {
                            uint len = (uint)pfaad.hinfo.samples * 2;
                            decoded = new byte[len];

                            fixed (Byte* pA = decoded)
                            {
                                c_libraryCalls.memcpy((IntPtr)pA, (IntPtr)pMemAlias, len);
                            }
                            retval = 0;
                        }
                        else
                            retval = pfaad.hinfo.error;

                        return retval;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                //throw;
            }
            return -1;
        }


        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
			logger.Debug ("Disposing");
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
#if AAC_TEST
                    if (fs != null)
                        fs.Close();
                    fs = null;
#endif
                    // Dispose managed resources.
                    //clear();
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                //if (hDecoder != (IntPtr)0)
                //    NeAACDecClose(hDecoder);
                //hDecoder = (IntPtr)0;

            }
            disposed = true;
        }
    }
}
