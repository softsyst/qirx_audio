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

using softsyst.qirx.audiodecoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using softsyst.Generic.Logger;
using softsyst.Generic;
using softsyst.qirx.DAB;
using softsyst.qirx.Interfaces;

namespace softsyst.qirx.Audio
{
    /// <summary>
    /// UDP interface buffer types
    /// </summary>
    internal enum BufferType
    {
        BUFFER_NIL = 0,

        /// <summary>
        /// AAC stream
        /// </summary>
        BUFFER_AAC,

        /// <summary>
        /// DAB stream, ONE_FRAME type or first frame of TWO_FRAME type
        /// </summary>
        BUFFER_MP2_1,

        /// <summary>
        /// DAB stream, 2nd frame of TWO_FRAME type
        /// </summary>
        BUFFER_MP2_2,

        /// <summary>
        /// Command
        /// </summary>
        BUFFER_COMMAND,
    }

    class mp4
    {
        logging<mp4> logger = new logging<mp4>(logging2.log);

        public dabAACInfo AACInfo { get; private set; } = new dabAACInfo();
        const int AACHeaderLength = 7; //without crc

        /// <summary>
        /// libfaad handle
        /// </summary>
        IntPtr hDecoder = (IntPtr)0;

        /// <summary>
        /// C# to libfaad link
        /// </summary>
        aacDecoder AACDecoder;

        /// <summary>
        /// C# to kjmp link
        /// </summary>
        dabDecoderDAB DABDecoder;

        /// <summary>
        /// Audio interface
        /// </summary>
        internal IAudioSink waudio { get; private set; }

        IMsgBox _msgBox = MsgBoxFactory.Create();

        /// <summary>
        /// udp streaming , own endpoint
        /// </summary>
        UdpClient udp = new UdpClient(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8767));

        /// <summary>
        /// remote IP endpoint for udp streaming, default port can be overridden in the config file qirx.config
        /// </summary>
        IPEndPoint ip = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8766);

        /// <summary>
        /// Set by the receiving function
        /// </summary>
        IPEndPoint remoteIp;

        /// <summary>
        /// udp command, own endpoint
        /// </summary>
        UdpClient udpCmd = new UdpClient(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8769));

        /// <summary>
        /// remote IP endpoint for udp command, default port can be overridden in the config file qirx.config
        /// </summary>
        IPEndPoint ipCmd = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8768);

        /// <summary>
        /// Set by the command receiving function
        /// </summary>
        IPEndPoint remoteIpCmd;

        /// <summary>
        /// Flag used from main
        /// </summary>
        internal bool terminateCmdThread = false;

        /// <summary>
        /// Audio sampling rate
        /// </summary>
        int sample_rate;

        /// <summary>
        /// Audio channels
        /// </summary>
        byte channels;

        int init_result = -1;
        ConstSizeBuffer audioOutBuf = new ConstSizeBuffer(10000);

        /// <summary>
        /// Libfaad and audio states, to be returned to host
        /// </summary>
        byte[] AudioStateArray;


        bool _initialized = false;
        internal bool Initialized { get { return _initialized; } set { _initialized = value; } }

        /// <summary>
        /// UDP interface buffer types
        /// </summary>
        enum AudioMode
        {
            MODE_NIL = 0,

            /// <summary>
            /// AAC stream
            /// </summary>
            MODE_AAC,

            /// <summary>
            /// MP2 stream
            /// </summary>
            MODE_MP2,

            /// <summary>
            /// UDP only stream
            /// </summary>
            MODE_UDP_ONLY,
        }
        /// <summary>
        /// Audio Commands
        /// </summary>
        enum AudioCommands {
            NIL = 0,

            /// <summary>
            /// AAC or MP2
            /// </summary>
            MODE,

            /// <summary>
            /// Mute the audio
            /// </summary>
            MUTE,

            /// <summary>
            /// Set UDP audio streaming on/off
            /// </summary>
            UDP,

            /// <summary>
            /// Set the udp port for audio streaming
            /// </summary>
            UDPPort
        }



        public mp4()
        {
            init();
        }

        private bool init()
        {
            if (udp != null)
            {
                udp.Connect(ip);
                logger.Info(string.Format("\nUDP connected to: {0}:{1}", ip.Address, ip.Port));
                Console.WriteLine(string.Format("\nUDP connected to: {0}:{1}", ip.Address, ip.Port));
            }
            //hDecoder = AACDecoder.open();
            //if (hDecoder == (IntPtr)0)
            //{
            //    //_msgBox.Show("Open AACDecoder failed");
            //    return false;
            //}

            if (udpCmd != null)
            {
                udpCmd.Connect(ipCmd);
                logger.Info(string.Format("UDPCmd connected to: {0}:{1}", ipCmd.Address, ipCmd.Port));
                Console.WriteLine(string.Format("UDPCmd connected to: {0}:{1}", ipCmd.Address, ipCmd.Port));
            }
            return true;
        }

        /// <summary>
        /// Command receiving thread
        /// </summary>
        internal void UDPCmdReceiveSync()
        {
            Console.WriteLine("Command thread entered.");
            udpCmd.Client.ReceiveTimeout = 200;
            while (true)
            {
                try
                {
                    byte[] buf = udpCmd.Receive(ref remoteIpCmd);

                    if (terminateCmdThread)
                        break;
                    if (buf == null || buf.Length == 0)
                        continue;

                    BufferType buftype = parseHeader(buf);

                    if (buftype == BufferType.BUFFER_COMMAND)
                    {
                        byte[] response = new byte[1];
                        if (processCmd(buf))
                            response[0] = 1;
                        else
                            response[0] = 0xff;
                        udpCmd.Send(response, response.Length);
                    }
                }
                catch (Exception )
                {
                    if (terminateCmdThread)
                        break;
                }
            }
            if (udpCmd != null)
            {
                udpCmd.Close();
                udpCmd = null;
                Console.WriteLine("UDPCmd closed.");
            }
        }

        private bool processCmd(byte[] buf)
        {
            if (waudio == null)
                return false;

            try
            {
                if (buf.Length >= 7)
                {
                    AudioCommands cmd = (AudioCommands)buf[2];
                    byte[] param = new byte[2];
                    int par = BitConverter.ToInt32(buf, 3);

                    switch (cmd)
                    {
                        case AudioCommands.MODE:
                            AudioMode mode = (AudioMode)par;
                            if (!initializeMode(mode))
                            {
                                Console.WriteLine($"Mode {mode} setting error ");
                                return false;
                            }
                            else
                                Console.WriteLine($"Mode {mode} set ");
                            break;
                        case AudioCommands.MUTE:
                            if (Mode == AudioMode.MODE_NIL)
                                return false;
                            waudio.Mute = par == 1 ? true : false;
                            string onoff = waudio.Mute ? "on" : "off";
                            Console.WriteLine($"Audio Mute {onoff} ");
                            break;
                        case AudioCommands.UDP:
                            if (Mode == AudioMode.MODE_NIL)
                                return false;
                            waudio.UDP = par == 1 ? true : false;
                            onoff = waudio.UDP ? "on" : "off";
                            Console.WriteLine($"UDP PCM16 output {onoff} ");
                            break;
                        case AudioCommands.UDPPort:
                            if (Mode == AudioMode.MODE_NIL)
                                return false;
                            waudio.UDP_Port = par;
                            Console.WriteLine($"UDP PCM16 output port set: {par} ");
                            break;
                        default:
                            if (Mode == AudioMode.MODE_NIL)
                                return false;
                            break;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Audio strem receiving thread fct
        /// </summary>
        public void UDPReceiveSync()
        {
            Console.WriteLine("Stream Receive thread entered.");
            while (true)
            {
                try
                {
                    byte[] buf = udp.Receive(ref remoteIp);

                    BufferType buftype = parseHeader(buf);

                    if (buftype == BufferType.BUFFER_AAC)
                    {
                        if (Mode != AudioMode.MODE_AAC)
                        {
                            initializeMode(AudioMode.MODE_AAC);
                        }

                        byte[] pcm16 = null;
                        if (processBuffer(buf, out pcm16))
                        {
                            AudioStateArray = collectAudioStates();

                            if (pcm16 != null && AudioStateArray != null)
                            {
                                var concat = new byte[AudioStateArray.Length + pcm16.Length];
                                AudioStateArray.CopyTo(concat, 0);
                                pcm16.CopyTo(concat, AudioStateArray.Length);

                                if (udp.Send(concat, concat.Length) != concat.Length)
                                {
                                    reset();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            reset();
                            break;
                        }
                    }

                    else if (buftype == BufferType.BUFFER_MP2_1 || buftype == BufferType.BUFFER_MP2_2 )
                    {
                        if (Mode != AudioMode.MODE_MP2)
                        {
                            initializeMode(AudioMode.MODE_MP2);
                        }

                        byte[] pcm16 = null;
                        if (DABDecoder.decode(buf, out pcm16, buftype))
                        {
                            AudioStateArray = DABDecoder.collectAudioStates(pcm16 != null);

                            if (pcm16 != null && AudioStateArray != null)
                            {
                                var concat = new byte[AudioStateArray.Length + pcm16.Length];
                                AudioStateArray.CopyTo(concat, 0);
                                pcm16.CopyTo(concat, AudioStateArray.Length);

                                if (udp.Send(concat, concat.Length) != concat.Length)
                                {
                                    reset();
                                    break;
                                }
                            }
                            else if (pcm16 == null)
                            {
                                udp.Send(AudioStateArray, AudioStateArray.Length);
                            }
                        }
                        }
                }
                catch (Exception )
                {
                    reset();
                    break;
                }
            }
        }

        private void reset()
        {
            clear();
            _initialized = false;
        }

        /// <summary>
        /// Process audio stream
        /// </summary>
        /// <param name="rxBuf">AAC coded audio buffer</param>
        /// <param name="pcm16">PCM16 decoded audio buffer</param>
        /// <returns></returns>
        private bool processBuffer(byte[] rxBuf, out byte[] pcm16)
        {
            pcm16 = null; //this one receives the decoded bytes
            // this is a complete aac frame with 960 samples
            if (!_initialized)
            {
                if (!initialize(rxBuf))
                    return false;
            }

            //next does not include the header (initialized with asc), works with all bitrates
            int bytesConsumed;
            int decoderObjectType = 0;
            int decoderChannels = 0;
            int decoderSamplingRate = 0;
            int startIx = 0;

            int error = AACDecoder.decode(hDecoder, rxBuf, startIx, out pcm16, out bytesConsumed,
                out decoderSamplingRate, out decoderChannels, out decoderObjectType);
            AACInfo.DecoderSamplingRate = decoderSamplingRate;
            AACInfo.DecoderChannels = decoderChannels;
            AACInfo.DecoderObjectType = (AAC_ObjectTypes)decoderObjectType;
            AACInfo.PCMBitDepth = 16;

            // construct buffers of always 10000 bytes and send it to the audio
            if (error != 0)
            {
                string s = AACDecoder.getErrorMessage((byte)error);
                logger.Error(s);
                return false;
            }
            else
            {
                byte[] pcm16Buf = audioOutBuf.createBuffer(pcm16);
                if (pcm16Buf != null)
                    waudio.AddSamples(pcm16Buf, 0, pcm16Buf.Length);
                //else is a normal operating case indicating that the const size buffer is not yet ready
            }
            return true;
        }

        AudioMode Mode { get; set; } = AudioMode.MODE_NIL;

        private bool initializeMode (AudioMode mode)
        {
            if (mode == AudioMode.MODE_AAC)
            {
                Mode = AudioMode.MODE_AAC;
                AACDecoder = new aacDecoder();
                hDecoder = AACDecoder.open();
                if (hDecoder != (IntPtr)0)
                    return true;

            }
            else if (mode == AudioMode.MODE_MP2)
            {
                Mode = AudioMode.MODE_MP2;
                DABDecoder = new dabDecoderDAB();
                return true;
            }

            Mode = AudioMode.MODE_NIL;
            return false;
        }

        /// <summary>
        /// Init AAC
        /// </summary>
        /// <param name="rxBuf"></param>
        /// <returns></returns>
        private bool initialize(byte[] rxBuf)
        {
            //parse header and initialize the decoder
            init_result = AACDecoder.init(hDecoder,
                                                rxBuf,
                                                ref sample_rate,
                                                ref channels);
            AACInfo.DecoderSamplingRate = sample_rate;
            AACInfo.DecoderChannels = channels;
            if (init_result != 0)
            {
                init_result *= -1;
                string err = AACDecoder.getErrorMessage((byte)init_result);
                byte[] b = System.Text.Encoding.ASCII.GetBytes(err);
                if (b == null)
                    throw new Exception("BP");
                //err = UnsafeCommon.UnsafeAsciiBytesToString(b, 0);

                string s = string.Format("Error initializing decoder library: {0}", err);
                AACDecoder.Close(hDecoder);
                hDecoder = (IntPtr)0;
                _initialized = false;
                logger.Error(s);
                return false;
            }

            if (waudio != null)
                clear();
            waudio = softsyst.qirx.Audio.wavAudio.create(eAudioSink.NAudio, "MP4", 0); 
            if (waudio == null)
            {
                logger.Error("Cannot create audio sink MP4");
                return false;
            }

            if (channels > 2)
            {
                logger.Warning($"{channels} audio channels detected. Reduced to two");
                channels = 2;
            }
            waudio.Init(sample_rate, 16, channels);
            waudio.Start();
            _initialized = true;
            logger.Debug("AAC and waudio initialized");
            
            return true;
        }

        void fillAudioStateList(int val, List<byte> bytes)
        {
            byte[] by = BitConverter.GetBytes(val);
            foreach (byte b in by)
                bytes.Add(b);
        }


        byte[] collectAudioStates()
        {
            List<byte> bytes = new List<byte>();

            fillAudioStateList(waudio.SamplingRate, bytes);
            fillAudioStateList(waudio.BitsPerSample, bytes);
            fillAudioStateList(waudio.Channels, bytes);
            fillAudioStateList(waudio.Mute?1:0, bytes);
            fillAudioStateList(waudio.UDP ? 1:0, bytes);
            fillAudioStateList(waudio.UDP_Port, bytes);

            fillAudioStateList(AACInfo.DecoderSamplingRate, bytes);
            fillAudioStateList(AACInfo.DecoderChannels, bytes);
            fillAudioStateList((int)AACInfo.DecoderObjectType, bytes);
            return bytes.ToArray();
        }

        /// <summary>
        ///dont close the decoder, only the waudio
        /// </summary>
        internal void clear()
        {
            if (waudio != null)
                waudio.Stop();
            waudio = null;
            if (udp != null)
                udp.Close();
            udp = null;
            Console.WriteLine("UDP closed");
        }


        /**
        //https://wiki.multimedia.cx/index.php?title=MPEG-4_Audio
        There are 13 supported frequencies:

        0: 96000 Hz
        1: 88200 Hz
        2: 64000 Hz
        3: 48000 Hz
        4: 44100 Hz
        5: 32000 Hz
        6: 24000 Hz
        7: 22050 Hz
        8: 16000 Hz
        9: 12000 Hz
        10: 11025 Hz
        11: 8000 Hz
        12: 7350 Hz
        13: Reserved
        14: Reserved
        15: frequency is written explictly
         * **/

        //https://wiki.multimedia.cx/index.php?title=ADTS
        //ADTS Header Structure
        //AAAAAAAA AAAABCCD EEFFFFGH HHIJKLMM MMMMMMMM MMMOOOOO OOOOOOPP(QQQQQQQQ QQQQQQQQ)

        //Header consists of 7 or 9 bytes(without or with CRC).
        //Letter Length(bits)   Description
        //A 	12 	syncword 0xFFF, all bits must be 1
        //B 	1 	MPEG Version: 0 for MPEG-4, 1 for MPEG-2
        //C 	2 	Layer: always 0
        //D 	1 	protection absent, Warning, set to 1 if there is no CRC and 0 if there is CRC
        //E 	2 	profile, the MPEG-4 Audio Object Type minus 1
        //F 	4 	MPEG-4 Sampling Frequency Index(15 is forbidden)
        //G 	1 	private bit, guaranteed never to be used by MPEG, set to 0 when encoding, ignore when decoding
        //H 	3 	MPEG-4 Channel Configuration(in the case of 0, the channel configuration is sent via an inband PCE)
        //I 	1 	originality, set to 0 when encoding, ignore when decoding
        //J 	1 	home, set to 0 when encoding, ignore when decoding
        //K 	1 	copyrighted id bit, the next bit of a centrally registered copyright identifier, set to 0 when encoding, ignore when decoding
        //L 	1 	copyright id start, signals that this frame's copyright id bit is the first bit of the copyright id, set to 0 when encoding, ignore when decoding
        //M 	13 	frame length, this value must include 7 or 9 bytes of header length: FrameLength = (ProtectionAbsent == 1 ? 7 : 9) + size(AACFrame)
        //O 	11 	Buffer fullness
        //P 	2 	Number of AAC frames(RDBs) in ADTS frame minus 1, for maximum compatibility always use 1 AAC frame per ADTS frame
        //Q 	16 	CRC if protection absent is 0         

        // 
        private BufferType parseHeader(byte[] buf)
        {
            if (buf[0] == 0xff && buf[1] == 0xf1)
                return BufferType.BUFFER_AAC;

            if (buf[0] == 0xff && (buf[1] & 0xf0) == 0xf0)
                return BufferType.BUFFER_MP2_1;

            if (buf[0] == 0x55 && buf[1] == 0xaa)
                return BufferType.BUFFER_COMMAND;

            if (Mode == AudioMode.MODE_MP2) // then second frame of TWO_FRAMES MP2 service
                return BufferType.BUFFER_MP2_2;

            return BufferType.BUFFER_NIL;
        }
    }
}
