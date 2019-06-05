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

namespace softsyst.qirx.Interfaces
{
    public enum eAACchannelMode { AAC_CHANNEL_MODE_NULL, AAC_CHANNEL_MODE_MONO, AAC_CHANNEL_MODE_STEREO };
    public enum eAACCodingMode { AAC_CODING_MODE_NULL, AAC_CODING_MODE_ASC, AAC_CODING_MODE_DTS };
    public enum AudioRecordingType { WAV, AAC };
    public enum MpegSurround { NotUsed = 0, FiveDotOne, SevenDotOne, Res, Res2, Res3, Re4s, Other }
    //public string[] MpegSurroundTexts =
    //{
    //    "No", "5.1", "7.1", "Reserved", "Reserved", "Reserved", "Reserved", "Other"
    //};

    //https://wiki.multimedia.cx/index.php?title=MPEG-4_Audio
    public enum AAC_ObjectTypes { Null = 0, AAC_Main, AAC_LC, AAC_SSR, AAC_LTP, AAC_SBR }

}
