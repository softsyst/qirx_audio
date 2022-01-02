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
using System.Text;
using System.IO;
using softsyst.Generic;
using softsyst.Generic.Logger;
using softsyst.qirx.Interfaces;

namespace softsyst.qirx.configuration
{
    public abstract class configFile
    {
        private readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        private static OperatingSystem os = Environment.OSVersion;
        protected static IMsgBox _msgBox = MsgBoxFactory.Create();
        //private static string configFileName = "qirx.config";

        public static string ConfigFilePath {get; private set;}

        // overwritten when starting the main window
        public static int InitialDisplayUpdateRateMs { get; set; } = 500; 

        /// <summary>
        /// After this method either the config file is present or the app is terminated
        /// </summary>
        /// <param name="create">Call with true only</param>
        public static bool createOrVerifyConfigFile(bool create = true)
        {
            return false;
            /*
            string path = System.AppDomain.CurrentDomain.BaseDirectory + configFileName;
            if (!File.Exists(path))
            {
                //try to create a default config file
                string dlgResult = _msgBox.Show("Configuration file " + path + " was not found. Do you want to have a default config file be created?", "QIRX Configuration", "YesNo", "Question");
                if (dlgResult == "No")
                {
                    _msgBox.Show("No Configuration File present. Application cannot continue.", "QIRX Configuration",
                        "OK", "Error");
                    Environment.Exit(-1);
                }
                if (create)
                {
                    if (!createConfigFile(path))
                    {
                        _msgBox.Show("Cannot create config file. Please check if directory is writeable. Application cannot continue.",
                            "QIRX Config File Creator", "OK", "Error");
                        Environment.Exit(-1);
                    }
                    
                    return createOrVerifyConfigFile(false);
                }
                return false;
            }
            ConfigFilePath = path;
            return true;
            */
        }

        // Deprecated to be removed
        public configFile(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigFilePath))
                {
                    string sError = "Configuration File Error: No configuration file present.";
                    throw new Exception(sError);
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
                if (_msgBox != null)
                    _msgBox.Show(ex.Message, "QIRX Configuration Error", "OK", "Error");
            }
        }

        public configFile()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigFilePath))
                {
                    if (createOrVerifyConfigFile(false) == false) // do not try to create a new file here
                    {
                        string sError = "Configuration File Error: No configuration file present.";
                        throw new Exception(sError);
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex.Message);
                if (_msgBox != null)
                    _msgBox.Show(ex.Message, "QIRX Configuration Error", "OK", "Error");
            }
        }

        public virtual void readConfiguration()
        {
            logger.Info(string.Format("Reading Configuration for {0} to {1}",
                this.GetType().ToString(), ConfigFilePath));
        }

        public virtual void writeConfiguration()
        {
            logger.Info(string.Format("Writing Configuration for {0} to {1}",
                this.GetType().ToString(), ConfigFilePath));
        }

        //static ILog ilogger = LogManager.GetLogger(typeof(configFile));
        public static bool createConfigFile(string path)
        {
            try
            {
                //logging2.logInfo(ilogger, "Creating config file: " + path );
                byte[] bytes = new ASCIIEncoding().GetBytes(configFileTemplate);

                using (FileStream f = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    f.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                if (_msgBox != null)
                    _msgBox.Show("Error on creating config file: " + ex.Message, "QIRX Configuration Error", "OK", "Stop");
                //logging2.logError(ilogger, ex.Message );
                return false;
            }
            return true;
        }

        public static string configFileTemplate
        {
            get
            {
                string s = string.Empty;
                s = "<?xml version = \"1.0\" encoding = \"utf-8\" ?>\n";
                s += "<configuration>\n";
                //s += "   <rxDevices>\n";
                //s += "       <rxDevice>\n";
                //s += "         <TunerType value = \"R820\" />\n";
                //s += "         <!--Remote tcpip server address>-->\n";
                //s += "         <Socket IPaddress = \"127.0.0.1\" port = \"1234\" />\n";
                //s += "         <CenterFrequency value = \"222064000\" />\n";
                //s += "         <SamplingRate value = \"2048000\" />\n";
                //s += "         <BitWidth value = \"8\" />\n";
                //s += "         <DeviceIndex value = \"0\" />\n";
                //s += "         <AGC value = \"on\" />\n";
                //s += "         <ppm value = \"0\" />\n";
                //s += "         <DeltaHz value = \"0\" />\n";
                //s += "         <GainIndex value = \"0\" />\n";
                //s += "         <Bandwidth value = \"1200000\" />\n";
                //s += "         <FrontendReceiver polling = \"false\" />\n";
                //s += "        </rxDevice>\n";
                //s += "   </rxDevices>\n";
                //s += "      <application allowMoreThanOneInstance = \"true\" />\n";
                //s += "      <starter autostart = \"yes\" autostop = \"no\">\n";
                //s += "           <RemoteStarterPort value = \"12345\" />\n";
                //s += "           <IQProvider_exe value = \"rtl_tcp\" />\n";
                //s += "           <CommandLine value = \" -d 0 -a 127.0.0.1 -p 1234 \" />\n";
                //s += "       </starter>\n";
                //s += "       <Directories>\n";
                //s += "           <wavOut>\n";
                //s += "             <DAB value = \"C:/tmp/\" maxSize=\"0\" />\n";
                //s += "             <FM value = \"C:/tmp/\" maxSize=\"0\" />\n";
                //s += "             <AM value = \"C:/tmp/\" maxSize=\"0\" />\n";
                //s += "           </wavOut>\n";

                //    s += "           <rawOut value = \"C:/tmp/\" maxSize=\"0\" />\n";
                //    s += "           <logging value = \"C:/qirxLogging/\"/>\n";
                //    s += "           <TIILogger value = \"C:/tmp/\"/>\n";
                //    s += "           <Persistence value = \"C:/tmp/\"/>\n";

                //s += "      </Directories>\n";
                //s += "      <Algorithm moveFrequencyParallel=\"yes\" />\n";
                //s += "      <Display>\n";
                //s += "          <UpdateIntervalMs value = \"500\"/>\n";
                //s += "      </Display>\n";
                //s += "      <Spectra>\n";
                //s += "	        <SpectrumFrontend Freeze=\"no\" Gradient=\"yes\" SmoothingLevel=\"5\"/>\n";
                //s += "	        <SpectrumCIR Freeze=\"no\" Gradient=\"yes\" SmoothingLevel=\"3\"  ScaleType=\"1\"/>\n";
                //s += "      </Spectra>\n";
                //s += "      <Rx1>\n";
                //s += "         <Startup started = \"yes\" spectrum = \"NF\" frequency = \"222064000\" step = \"0\" modulation = \"DAB\">\n";
                //s += "         </Startup>\n";
                //s += "      </Rx1>\n";
                //s += "      <Rx2>\n";
                //s += "        <Startup started = \"no\" spectrum = \"HF\" frequency = \"222064000\" step = \"25000\" modulation = \"NFM\" />\n";
                //s += "      </Rx2>\n";
                //s += "      <DAB>\n";
                //s += "          <SNR DeltaSound_dB = \"5\" />\n";
                //s += "          <Ensemble channel = \"11D\" service = \"BR-KLASSIK      \" udpIPAddress=\"255.255.255.255\" udpPort = \"8765\" muted = \"no\"/>\n";
                //s += "          <Ensembles value=\"NN,5A,5B,5C,5D,6A,6B,6C,6D,7A,7B,7C,7D,8A,8B,8C,8D,9A,9B,9C,9D,10A,10B,10C,10D,10N,11A,11B,11C,11D,11N,12A,12B,12C,12D,12N,13A,13B,13C,13D,13E,13F\"/>\n";
                //s += "          <GUI SelecteTabIndex = \"1\" />\n";
                //s += "          <TII Threshold = \"0.2\" />\n";
                //s += "      	<Scanner AutoMode=\"yes\" Forever=\"yes\" DelayTime=\"15\" MinLevel=\"7\" Ensembles=\"5A,5B,5C,5D,6A,6B,6C,6D,7A,7B,7C,7D,8A,8B,8C,8D,9A,9B,9C,9D,10A,10B,10C,10D,10N,11A,11B,11C,11D,11N,12A,12B,12C,12D,12N,13A,13B,13C,13D,13E,13F\"/>\n";
                //s += "          <AudioRecorder Type=\"WAV\" />\n";
                //s += "      </DAB>\n";
                s += "      <Logging>\n";
                s += "           <FileNamePrefix value = \"qirx.log\"/>\n";
                s += "           <MaxFileSizeKB value = \"10000\"/>\n";
                s += "           <UDP IPAddress=\"255.255.255.255\" Port=\"878\" />\n";
                s += "      </Logging>\n";
                s += "      </configuration>\n";
                return s;
            }
        }
    }
}
