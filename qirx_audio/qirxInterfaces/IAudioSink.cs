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


namespace softsyst.Generic
{
    public interface IAudioSink
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="samplingRate"></param>
        /// <param name="bitsPerSample"></param>
        /// <param name="channels"></param>
        void Init(int samplingRate, int bitsPerSample, int channels);

        /// <summary>
        /// Starts the audio thread. Does not start the recording
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the audio thread
        /// </summary>
        void Stop();

        /// <summary>
        /// Adds audio pcm samples into the queue
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <remarks>Byte buffer, although Int16 pcm samples are added, i.e. every two bytes belong together</remarks>
        void AddSamples(byte[] buffer, int offset, int count);

        /// <summary>
        /// Audio Sampling Rate
        /// </summary>
        int SamplingRate { get;} 

        /// <summary>
        /// Number of bits per audio sample
        /// </summary>
        int BitsPerSample { get; }

        /// <summary>
        /// Number of audio channels
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// Indication if muted, i.e. no audio to the selected device
        /// </summary>
        /// <remarks>UDP out is not affected</remarks>
        bool Mute { get; set; }

        /// <summary>
        /// Indication if the sound is streamed to UDP as a byte stream
        /// </summary>
        /// <remarks>Mute is not affected</remarks>
        /// <remarks>Format is PCM16, Port is read from the config file, default is 8765</remarks>
        bool UDP { get; set; }

        /// <summary>
        /// The udp prot for the audio broadcast
        /// </summary>
        /// <remarks>Port is read from the config file, default is 8765</remarks>
        int UDP_Port { get; set; }

   }
}
