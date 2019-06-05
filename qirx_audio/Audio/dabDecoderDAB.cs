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
using System.Collections.Generic;
using softsyst.qirx.audiodecoder;
using softsyst.Generic.Logger;
using softsyst.Generic;

namespace softsyst.qirx.Audio
{
    /// <summary>
    /// DAB decoder class (in  contrast to DAB plus)
    /// </summary>
    public class dabDecoderDAB : IDisposable
    {
        logging<dabDecoderDAB> logger = new logging<dabDecoderDAB>(logging2.log);
        enum eFrames { ONE_FRAME, TWO_FRAMES };
        int bitrate;

        eFrames noOfFrames;

        /// <summary>
        /// Audio interface
        /// </summary>
        internal IAudioSink waudio { get; private set; }

        public int sampleRate { get; private set; }

        ConstSizeBuffer audioOutBuf = new ConstSizeBuffer(10000);

        public dabDecoderDAB()
        {
        }
        internal void clear()
        {
            audioOutBuf.clear();
            if (waudio != null)
                waudio.Stop();
            waudio = null;
            frame = null;
        }

        private void fillAudioStateList(int val, List<byte> bytes)
        {
            byte[] by = BitConverter.GetBytes(val);
            foreach (byte b in by)
                bytes.Add(b);
        }

        internal byte[] collectAudioStates(bool frameComplete)
        {
            List<byte> bytes = new List<byte>();

            fillAudioStateList(waudio.SamplingRate, bytes);
            fillAudioStateList(waudio.BitsPerSample, bytes);
            fillAudioStateList(waudio.Channels, bytes);
            fillAudioStateList(waudio.Mute ? 1 : 0, bytes);
            fillAudioStateList(waudio.UDP ? 1 : 0, bytes);
            fillAudioStateList(waudio.UDP_Port, bytes);
            fillAudioStateList(frameComplete ? 1 : 0, bytes);

            return bytes.ToArray();
        }

        byte[] frame;
        internal bool decode (byte[] inbytes, out byte[] pcm16, BufferType bufferType)
        {
            pcm16 = null;

            try
            {
                if (frame == null || 
                    (bufferType == BufferType.BUFFER_MP2_2 && noOfFrames == eFrames.ONE_FRAME))      //starting phase of the service
                {
                    if (!setup(inbytes)) //then wait for second frame, sets noOfFrames
                        return false;
                }
                switch (noOfFrames)
                {
                    case eFrames.ONE_FRAME:
                        Buffer.BlockCopy(inbytes, 0, frame, 0, inbytes.Length);
                        pcm16 = mp2Decoder.instance.decode(frame);
                        if (pcm16 != null && waudio != null)
                        {
                            byte[] pcm16Buf = audioOutBuf.createBuffer(pcm16);
                            if (pcm16Buf != null)
                                waudio.AddSamples(pcm16Buf, 0, pcm16Buf.Length);
                            //else is a normal operating case indicating that the const size buffer is not yet ready
                        }
                        break;

                    case eFrames.TWO_FRAMES :
                        if (isFirstBlock(inbytes))
                            Buffer.BlockCopy(inbytes, 0, frame, 0, inbytes.Length);
                        else
                        {
                            Buffer.BlockCopy(inbytes, 0, frame, inbytes.Length, inbytes.Length);
                            pcm16 = mp2Decoder.instance.decode(frame);
                            if (pcm16 != null && waudio != null)
                            {
                                byte[] pcm16Buf = audioOutBuf.createBuffer(pcm16);
                                if (pcm16Buf != null)
                                    waudio.AddSamples(pcm16Buf, 0, pcm16Buf.Length);
                                //else is a normal operating case indicating that the const size buffer is not yet ready
                            }
                        }
                        break;
                }
                return true; 
            }
            catch (Exception e)
            {
                logger.Exception(e);
            }
            return false;
        }

        bool isFirstBlock (byte[] inbytes)
        {
            if (inbytes[0] == 0xff && inbytes[1] == 0xf4) // then first block
                return true;
            return false;
        }

        bool setup(byte[] inbytes)
        {
            if (frame == null)
            {
                sampleRate = mp2Decoder.instance.getSampleRate(inbytes);
                if (sampleRate == 0) // wait for the next frame
                    return false;
                if (sampleRate == 24000)
                {
                    noOfFrames = eFrames.TWO_FRAMES;
                    frame = new byte[inbytes.Length * 2];
                }
                else
                {
                    noOfFrames = eFrames.ONE_FRAME;
                    frame = new byte[inbytes.Length];
                }
                if (waudio != null)
                    clear();

                waudio = wavAudio.create(eAudioSink.NAudio, "MP2", 0);
                if (waudio == null)
                {
                    logger.Error("Cannot create audio sink MP2");
                    return false;
                }

                waudio.Init(sampleRate, 16, 2);
                waudio.Start();
                return true;
            }
            return true;
        }

        bool disposed = false;

        public void Dispose()
        {
            Dispose(disposed);
        }

        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    clear();
                }
            }
            disposed = true;
        }
    }
}
