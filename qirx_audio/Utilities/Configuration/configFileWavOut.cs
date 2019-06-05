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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using softsyst.Generic.IO;
using softsyst.Generic.Logger;
using softsyst.qirx.Interfaces;
using softsyst.Generic.XML;
using softsyst.Generic;

namespace softsyst.qirx.configuration
{
    public class configFileWavOut : configFile
    {
        logging<configFileWavOut> logger = new logging<configFileWavOut>(logging2.log);

        string modulation;
        //Read Configuration
        public string wavOutDir { get; private set; }
        public string sFileMaxSize { get; private set; }
        public string sUdpPort { get; private set; }
        public int udpPort { get; private set; }
        public string sUdpIPAddress { get; private set; }
        public IPAddress udpIPAddress { get; private set; }
        public bool Muted { get; private set; }
        //Write Configuration
        public string sMuted { get; set; }

        public configFileWavOut(string modulation) : base()
        {
            this.modulation = modulation;
        }

        public override void readConfiguration()
        {
            string xPathDAB = "./DAB/Ensemble";
            try
            {
                sUdpPort = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, xPathDAB, "udpPort");
                int port = 0;
                if (!int.TryParse(sUdpPort, out port))
                    throw new Exception("Invalid udp Port");

                sUdpIPAddress = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, xPathDAB, "udpIPAddress");
                IPAddress addr = null;
                if (!IPAddress.TryParse(sUdpIPAddress, out addr))
                    throw new Exception("Invalid udp IPAddress");
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                sUdpPort = "8765"; //default
                sUdpIPAddress = "255.255.255.255"; //default
            }
            udpPort = int.Parse(sUdpPort);
            udpIPAddress  = IPAddress.Parse(sUdpIPAddress);

            try
            {
                wavOutDir = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Directories/wavOut/" + modulation, "value");
                wavOutDir = FilesPathHelper.PathAddBackslash(wavOutDir);

                sFileMaxSize = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, "./Directories/wavOut/" + modulation, "maxSize");
                Muted = XMLHelper.readSingleXMLNodeAttrib(ConfigFilePath, xPathDAB, "muted") == "yes";
            }
            catch (Exception ex)
            {
                _msgBox.Show(ex.Message, "QIRX Configuration Audio Out File Settings Error", "OK", "Error");
            }
        }

        public override void writeConfiguration()
        {
            ////ignore in file mode
            //if (frontendConstants.rxMode == eRxMode.RX_MODE_FILE)
            //    return;

            try
            {
                string xPathDAB = "./DAB/Ensemble";
                XMLHelper.writeSingleXMLNodeAttrib(ConfigFilePath, xPathDAB, "muted", sMuted);
            }
            catch (Exception ex)
            {
                _msgBox.Show(ex.Message, "QIRX Save DAB Configuration Error", "OK", "Error");
            }
        }
    }
}
