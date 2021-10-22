using Commons.Media.PortAudio;
using System;

namespace softsyst.qirx.Audio
{
    public class PortAudio : audioBase
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Protected Constructor, called from the "create" virtual constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rxNumber"></param>
        internal PortAudio(string name, int rxNumber) : base(name, rxNumber)
        {
            try
            {
                logger?.Debug(string.Format("PortAudio constructed, name {0}, rx number {1}",
                    new object[] { name, rxNumber }));
            }
            catch (Exception ex)
            {
                logger?.Error("Constructor: ", ex.Message);
            }
        }
        /// <summary>
        /// Starts the wav player
        /// </summary>
        protected override void play()
        {
            UDP = false;
        }

        /// <summary>
        /// The portAudio stream
        /// </summary>
        private IntPtr _stream = (IntPtr)0;
        PaStreamParameters _paStreamParams;


        unsafe public override void Init(int samplingRate, int bitsPerSample, int channels)
        {

            try
            {
                PaErrorCode err = PortAudioInterop.Pa_Initialize();
                if (err != PaErrorCode.paNoError)
                    throw new Exception($"PortAudio Initialize failed with {err}");

                PaSampleFormat fmt;
                if (bitsPerSample == 8) fmt = PaSampleFormat.Int8;
                else if (bitsPerSample == 16) fmt = PaSampleFormat.Int16;
                else if (bitsPerSample == 32) fmt = PaSampleFormat.Float32;
                else fmt = PaSampleFormat.Int16;

                int devix = PortAudioInterop.Pa_GetDefaultOutputDevice();
                PaStreamParameters paStreamParams = new();

                paStreamParams.device = devix;
                paStreamParams.channelCount = channels;
                paStreamParams.sampleFormat = fmt;
                paStreamParams.suggestedLatency = 0;
                paStreamParams.hostApiSpecificStreamInfo = (IntPtr)0;

                
                int* p = (int*)&paStreamParams;
                // the next doesn't work. Marshalling not possible
                //CppInstancePtr hdl = Factory.ToNative<PaStreamParameters>(paStreamParams);

                err = PortAudioInterop.Pa_IsFormatSupported((IntPtr)0, p, samplingRate);
                if (err != PaErrorCode.paNoError)
                    throw new Exception($"PortAudio Init with samplingRate {samplingRate}, bitsPerSample {bitsPerSample}, channels {channels} failed with {err}");

                _paStreamParams = paStreamParams;

                base.Init(samplingRate, bitsPerSample, channels);

                logger?.Info ($"PortAudio parameter check with samplingRate {samplingRate}, bitsPerSample {bitsPerSample}, channels {channels} succeeded with {err}");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Writes pcm samples into an audio output channel
        /// </summary>
        /// <param name="pcm16"></param>
        protected override void writeSamples(byte[] pcm16)
        {
            // byte -> ushort
            uint frames = (uint) ((pcm16.Length / 2) / _paStreamParams.channelCount);
            unsafe 
            {
                fixed (byte* p = &pcm16[0])
                {
                    if (_stream != (IntPtr)0)
                        PortAudioInterop.Pa_WriteStream(_stream, p, frames);
                }
            }
        }

        /// <summary> 
        /// Thread processing the audio queue
        /// </summary>
        protected override void doWork()
        {
            logger?.Debug("Entered worker thread " + thrdWorker.Name + " function, ...");

            try
            {
                //PaErrorCode err = PortAudioInterop.Pa_OpenDefaultStream(out _stream, 0, 2, (IntPtr)PaSampleFormat.Int16,
                PaErrorCode err = PortAudioInterop.Pa_OpenDefaultStream(out _stream, 0,
                    _paStreamParams.channelCount,
                    (IntPtr)_paStreamParams.sampleFormat,
                    samplingRate, (IntPtr)0, null, (IntPtr)0);

                if (err != PaErrorCode.paNoError)
                    throw new Exception(err.ToString());

                err = PortAudioInterop.Pa_StartStream(_stream);

                if (err != PaErrorCode.paNoError)
                    throw new Exception(err.ToString());

            }
            catch (Exception e)
            {
                logger?.Error($"Entered PortAudio worker thread \"{ thrdWorker.Name}\" failed with {e.Message}");
                throw;
            }
            base.doWork();
        }

        /// <summary>
        /// Implements IDisposable
        /// </summary>
        public override void Dispose(bool disposing)
        {
            try
            {
                if (_stream != (IntPtr)0)
                    PortAudioInterop.Pa_AbortStream(_stream);
                PortAudioInterop.Pa_Terminate();

                base.Dispose(disposing);

                logger?.Info("portAudio disposed");
            }
            catch (Exception e)
            {
                logger?.Error(e.Message);
            }
        }
    }
}
