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

using softsyst.Generic;
using softsyst.Generic.SafeCollections;
using softsyst.qirx.configuration;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace softsyst.qirx.Audio
{
    public enum eAudioSink { UDPonly, NAudio, PortAudio};
    /// <summary>
    /// Base class with common parts for the NAudio out (windows)
    /// and the udpOnly out (Linux)
    /// </summary>
    public class audioBase :  IAudioSink, IDisposable
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        enum eWaitHandle { WAIT_QUEUE = 0, WAIT_TERMINATE = 1 }

        /// <summary>
        /// Audio out worker thread
        /// </summary>
        protected Thread thrdWorker;

        /// <summary>
        /// PCM samples Q
        /// </summary>
        SafeQueue<byte[]> samplesQ;

        /// <summary>
        /// Handles for the audio out thread control
        /// </summary>
        EventWaitHandle evtTerminate;
        EventWaitHandle qEvent;
        EventWaitHandle[] eventHandles = null;

        /// <summary>
        /// The sampling rate in Hz
        /// </summary>
        protected int samplingRate;
        public int SamplingRate { get { return samplingRate; } }

        /// <summary>
        /// Number of bits per sample
        /// </summary>
        protected int bitsPerSample;
        public int BitsPerSample { get { return bitsPerSample; } }

        /// <summary>
        /// Number of audio channels
        /// </summary>
        protected int channels;
        public int Channels { get { return channels; } }

        /// <summary>
        /// The rx number this demodulator is attached to
        /// Counting from 1; 0 means invalid number
        /// </summary>
        internal int rxNumber { get; set; }

        /// <summary>
        /// Thread name for debugging
        /// </summary>
        protected string threadName;

        /// <summary>
        /// Maximum size of the wav file, currently not used
        /// </summary>
        protected long fileMaxSize = 0;

        /// <summary>
        /// Modulation string, e.g. "DAB"
        /// </summary>
        internal string modulation;

        /// <summary>
        /// Directory for the wav file
        /// </summary>
        internal string wavOutDir;

        /// <summary>
        /// Persisted configuration, read from qirx.config
        /// </summary>
        protected configFileWavOut cfg;


        /// <summary>
        /// Flag to indicate if audio out is silent
        /// </summary>
        /// <remarks>Does not affect the file out!</remarks>
        bool muted = false;
        public virtual bool Mute
        {
            get { return muted; }
            set { muted = value; }
        }

        /// <summary>
        /// udp streaming class
        /// </summary>
        UdpClient udp = new UdpClient();

        /// <summary>
        /// IP endpoint for udp streaming, default port can be overridden in the config file qirx.config
        /// </summary>
        IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 8765);

        /// <summary>
        /// UDP audio streaming port
        /// </summary>
        private int udpPort { get; set; }
        public int UDP_Port { get { return udpPort; } set { udpPort = value; } }

        /// <summary>
        /// UDP streaming IP Address
        /// </summary>
        IPAddress udpIPAddress;

        /// <summary>
        /// Flag if udp streaming is active
        /// </summary>
        bool udpFlag = false;
        public bool UDP
        {
            get { return udpFlag; }
            set { udpFlag = value; }
        }

        /// <summary>
        /// Virtual Constructor
        /// </summary>
        /// <param name="audioSinkType">Selector for the audio output type</param>
        /// <param name="name">Name</param>
        /// <param name="rxNumber">number of this sink</param>
        /// <returns></returns>
        public static IAudioSink create(eAudioSink audioSinkType, string name, int rxNumber)
        {
            if (audioSinkType == eAudioSink.NAudio)
            {
                return new wavAudio(name, rxNumber);
            }
            if (audioSinkType == eAudioSink.PortAudio)
            {
                return new PortAudio(name, rxNumber);
            }
            else if (audioSinkType == eAudioSink.UDPonly)
            {
                return new audioBase(name, rxNumber);
            }
            else
                return null;
        }

        /// <summary>
        /// Protected Constructor, called from the "create" virtual constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rxNumber"></param>
        protected audioBase(string name, int rxNumber)
        {
            try
            {
                this.rxNumber = rxNumber;
                threadName = name;
                evtTerminate = new ManualResetEvent(false);
                qEvent = new ManualResetEvent(false);
                eventHandles = new EventWaitHandle[2] { qEvent, evtTerminate };

                if (name.Contains("MP4") || name.Contains("MP2"))
                {
                    modulation = "DAB";
                }
                else if (name.Contains("FM"))
                    modulation = "FM";
                else if (name.Contains("AM"))
                    modulation = "AM";
                else
                    modulation = "UNKNOWN";

                if (modulation != "UNKNOWN")
                {
                    cfg = new configFileWavOut( modulation);
                    readConfiguration();
                }


                logger.Debug(string.Format("Audio out class constructed, name {0}, rx number {1}",
                    new object[] { name, rxNumber }));
            }
            catch (Exception ex)
            {
                logger.Error("Constructor: ", ex.Message);
            }
        }

        /// <summary>
        /// Initialisation of the audio sink
        /// </summary>
        /// <param name="samplingRate">Audio sampling rate</param>
        /// <param name="bitsPerSample">Audio bits per sample</param>
        /// <param name="channels">No of audio channels</param>
        public virtual void Init(int samplingRate, int bitsPerSample, int channels)
        {
            this.samplingRate = samplingRate;
            this.bitsPerSample = bitsPerSample;
            this.channels = channels;
            try
            {
                logger.Debug
                    (string.Format("Audio initialised for DDC with modulation {0}, samplingRate {1}, bitsPerSample: {2}, Channels:{3} ",
                              new object[] { modulation, samplingRate, bitsPerSample, channels }));
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
            }
        }

        /// <summary>
        /// Reads the qirx.config
        /// </summary>
        protected virtual void readConfiguration()
        {
            try
            {
                // if no config file available, the default values apply,
                // in particular mute == false
                cfg.readConfiguration();
                wavOutDir = cfg.wavOutDir;
                udpPort = cfg.udpPort;
                udpIPAddress = cfg.udpIPAddress;
                muted = cfg.Muted;

                if (string.IsNullOrEmpty(wavOutDir))
                {
                    wavOutDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                fileMaxSize = 0;
                if (!long.TryParse(cfg.sFileMaxSize, out fileMaxSize))
                    fileMaxSize = 0;

            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
            }
        }

        /// <summary>
        /// Writes the qirx.config
        /// </summary>
        protected virtual void writeConfiguration()
        {
            string s = muted ? "yes" : "no";
            if (cfg != null)
            {
                cfg.sMuted = s;
                cfg.writeConfiguration();
            }
        }

        /// <summary>
        /// Starts the audio
        /// </summary>
        public void Start()
        {
            thrdWorker = new Thread(doWork);
            thrdWorker.Name = "WavOut " + threadName;
            thrdWorker.Priority = ThreadPriority.Highest;
            logger.Debug("Starting the wavOut thread " + threadName);
            thrdWorker.Start();
        }

        /// <summary>
        /// Stops the audio
        /// </summary>
        public void Stop()
        {
            if (thrdWorker != null && thrdWorker.ThreadState != ThreadState.Unstarted)
            {
                if (evtTerminate != null)
                    evtTerminate.Set();
                if (!thrdWorker.Join(500))
                {
                    //thrdWorker.Abort();
                    logger?.Warn("Worker thread " + thrdWorker.Name + " did not terminate after 0.5 sec");
                }
                else
                    logger?.Debug("Worker thread " + thrdWorker.Name + "  terminated normally");

                writeConfiguration();

            }
        }

        /// <summary>
        /// Feeds the samples queue and signals to the thread
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void AddSamples(byte[] buffer, int offset, int count)
        {
            try
            {
                if (samplesQ != null)
                {
                    samplesQ.Enqueue(buffer);
                    qEvent.Set();
                }
            }
            catch (Exception e)
            {
                logger.Error("Add samples error: " + e.Message);
            }
        }

        // Virtual Methods without being implemented in the base file

        /// <summary>
        /// Starts the audio output
        /// </summary>
        protected virtual void play() { }

        /// <summary>
        /// Writes samples into the audio out channel
        /// </summary>
        /// <param name="pcm16"></param>
        protected virtual void writeSamples(byte[] pcm16) { }

        /// <summary>
        /// Thread processing the audio queue
        /// </summary>
        protected virtual void doWork()
        {
            if (udp != null)
            {
                udp.Connect(ip);
                ip.Port = udpPort;
                ip.Address = udpIPAddress;

                logger.Info(string.Format("UDP connected to: {0}:{1}", ip.Address, ip.Port));
            }
            samplesQ = new SafeQueue<byte[]>((uint)Thread.CurrentThread.ManagedThreadId); // q ident

            logger.Debug(string.Format("Starting wavOut {0}, entering endless loop", threadName));

            play(); //virtual

            while (true)
            {
                eWaitHandle handle = (eWaitHandle)WaitHandle.WaitAny(eventHandles);
                if (handle == eWaitHandle.WAIT_TERMINATE)
                    break;
                //else if (handle == eWaitHandle.WAIT_QUEUE) //no other possibilty left

                byte[] buf;
                while (samplesQ.TryDequeue(out buf))
                {
                    try
                    {
                        {
                            int count = buf.Length;
                            //Possible UDP Consumers
                            //http://gqrx.dk/doc/streaming-audio-over-udp
                            //nc -l -u -p 8765 | aplay -r 48000 -f S16_LE -t raw -c 1
                            //vlc --demux=rawaud --rawaud-channels=2 --rawaud-samplerate=48000 udp://@:8765
                            if (udpFlag)
                                udp.Send(buf, buf.Length);

                            if (muted)
                            {
                                Array.Clear(buf, 0, buf.Length);
                            }
                            writeSamples(buf);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.Error(ex.Message);
                    }
                }
                qEvent.Reset();
            }
            Dispose();
            logger.Debug(string.Format("Terminating thread {0} ... ", threadName));
        }
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// IDispose satisfaction
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    logger?.Debug(string.Format("Disposing {0} ... ", threadName));

                    if (samplesQ != null)
                        samplesQ.Clear();
                    qEvent.Close();
                    evtTerminate.Close();
                    qEvent = null;
                    evtTerminate = null;
                    eventHandles = null;
                    if (udp != null)
                    {
                        udp.Close();
                        udp = null;
                        logger?.Debug("udp Disposed");
                    }
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                logger?.Error(e.Message);
            }
        }
    }
}