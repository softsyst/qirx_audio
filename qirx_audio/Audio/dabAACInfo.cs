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
using softsyst.Generic;
using softsyst.qirx.Interfaces;
using softsyst.Generic.Logger;

namespace softsyst.qirx.DAB
{
    public class dabAACInfo : IAACInfo
    {

        public eAACchannelMode aacChannelMode { get; internal set; }
        public eAACCodingMode aacCodingMode { get; internal set; }
        public bool PS { get; internal set; }
        public MpegSurround Surround { get; internal set; }
        public bool SBR { get; internal set; }
        public int AudioUnits { get; internal set; }
        public int DecoderSamplingRate { get; internal set; }
        public int DecoderChannels { get; internal set; }
        public AAC_ObjectTypes DecoderObjectType { get; internal set; }
        public int PCMBitDepth { get; internal set; }
    }
}
