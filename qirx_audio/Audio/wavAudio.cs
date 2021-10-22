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
using NAudio.Wave;
using softsyst.Generic.Logger;
using softsyst.qirx.Interfaces;

namespace softsyst.qirx.Audio
{
    /// <summary>
    /// Handles the NAudio interface for direct output and recording
    /// Accessed by the application via the IAudioSink interface
    /// </summary>
    public class wavAudio : audioBase 
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// NAudio elements
        /// </summary>
        internal WaveOut wavOut;
        internal BufferedWaveProvider wavBufProvider;
        internal WaveFormat format;

        /// <summary>
        /// Protected Constructor, called from the "create" virtual constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rxNumber"></param>
        internal wavAudio(string name, int rxNumber): base(name, rxNumber)
        {
            try
            {
                logger.Debug(string.Format("WavAudio constructed, name {0}, rx number {1}",
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
            if (wavOut != null)
                wavOut.Play();
        }


        /// <summary>
        /// Writes pcm samples into an audio output channel
        /// </summary>
        /// <param name="pcm16"></param>
        protected override void writeSamples(byte[] pcm16)
        {
            if (wavBufProvider != null)
                wavBufProvider.AddSamples(pcm16, 0, pcm16.Length);
        }

        /// <summary>
        /// Thread processing the audio queue
        /// </summary>
        protected override void doWork()
        {
            //logger.Debug("Entered worker thread " + thrdWorker.Name + " function, ...");
            format = new WaveFormat(samplingRate, bitsPerSample, channels);
            try
            {
                wavOut = new WaveOut();
                wavBufProvider = new BufferedWaveProvider(format);
                wavBufProvider.BufferLength = 512000;
                //wavBufProvider.DiscardOnBufferOverflow = true;
                //logger.Debug(string.Format("... Creating and Initializing  wavOut {0} with Buffer length {1}...", threadName, wavBufProvider.BufferLength));
                wavOut.Init(wavBufProvider);
            }
            catch(Exception e)
            {
                logger?.Error( e.Message);
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
                //if (wavWriter != null)
                //    wavWriter.Dispose();
                if (wavOut != null)
                    wavOut.Dispose();
                //if (wavBufProvider != null)
                //    wavBufProvider.Dispose(); //not existing
                base.Dispose(disposing);
                logger.Info("WAV Audio disposed");
            }
            catch (Exception e)
            {
                logger?.Error(e.Message);
            }
        }
    }
}