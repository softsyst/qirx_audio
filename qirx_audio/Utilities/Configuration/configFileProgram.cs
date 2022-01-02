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
using softsyst.Generic.XML;
using softsyst.Generic.IO;
using softsyst.Generic.Logger;

namespace softsyst.qirx.configuration
{
    public class configFileProgram : configFile
    {

        public string PersistenceDirectory { get; set; }
        public string loggingDirectory { get; set; }
        public string logFileNamePrefix { get; set; }
        public int maxLogFileSizeKB { get; set; }
        public string loggerUdpAddress;
        public int loggerUdpPort;
        public bool allowMoreThanOneInstance { get; private set; }
        public int displayUpdateIntervalMs { get; set; }



        public override void readConfiguration()
        {
            try
            {
                loadLoggingSettings();
                loadOther();
            }
            catch (Exception ex)
            {
                if (_msgBox != null)
                    _msgBox.Show(ex.Message, "QIRX Configuration Read Program Settings Error", "OK", "Error");
            }
        }

        public configFileProgram(string path) : base(path) { }
        public configFileProgram() : base() { }

        public override void writeConfiguration()
        {
            try
            {
                base.writeConfiguration();
                XMLHelper.writeSingleXMLNodeAttrib(ConfigFilePath, "./Display/UpdateIntervalMs", "value", displayUpdateIntervalMs.ToString());
            }
            catch (Exception )
            {
                //logger.Error("Error writing configuration: " + e.Message);
            }
        }
        private void loadLoggingSettings()
        {
            loggingDirectory = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Directories/logging", "value");
            loggingDirectory = FilesPathHelper.PathAddBackslash(loggingDirectory );
            logFileNamePrefix = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Logging/FileNamePrefix", "value");
            string s = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Logging/MaxFileSizeKB", "value");
            int i = 0;
            if (int.TryParse(s, out i))
                maxLogFileSizeKB = i;
            else
                maxLogFileSizeKB = 2000;

            try
            {
                loggerUdpAddress = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Logging/UDP", "IPAddress");
                i = 0;
                s = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Logging/UDP", "Port");
                if (int.TryParse(s, out i))
                    loggerUdpPort = i;
                else
                    loggerUdpPort = 878; //log4view default
            }
            catch (Exception)
            {
                loggerUdpAddress = "Error";
                loggerUdpPort = 0;
            }
        }
        private void loadOther()
        {
            //logger.Info("Loading Other Settings..");
            allowMoreThanOneInstance = false;
            string s = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./application", "allowMoreThanOneInstance");

            bool b;
            if (bool.TryParse(s, out b))
                allowMoreThanOneInstance = b;

            PersistenceDirectory = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Directories/Persistence", "value");
            PersistenceDirectory = FilesPathHelper.PathAddBackslash(PersistenceDirectory);

            s = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Display/UpdateIntervalMs", "value");
            int i;
            if (int.TryParse(s, out i))
                displayUpdateIntervalMs = i;
            else
                displayUpdateIntervalMs = 500;

        }
    }
}
