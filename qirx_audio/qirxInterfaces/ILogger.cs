/**
** ** QIRX - Software Defined Radio - AAC Audio  
** Copyright (C) 2018 Clem Schmidt, softsyst GmbH, http://www.softsyst.com
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

namespace softsyst.Generic.Logger.Interface
{
    public interface ILogger
    {
        void AddLogFileListener(String filePrefix, int maxLogFileSizeInKB, int maxLogFiles);
        void SetLogLevelForAllFileListeners(log4net.Core.Level levelToBeApplied);
        void SetLogLevelForUdpListeners(log4net.Core.Level levelToBeApplied);
        void SetUdpListenerEndpooint(string IPAddr, int port);
        object Init(Type logType);
        void Info(string txt);
        void Debug(string txt);
        void Warning(string txt);
        void Error(string txt);
        void Exception(Exception ex);
     }
}
