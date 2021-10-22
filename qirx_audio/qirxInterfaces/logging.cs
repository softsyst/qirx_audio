/**
** QIRX - Software Defined Radio 
** Copyright (C) 2017-2021 Clem Schmidt, softsyst GmbH, http://qirx.softsyst.com
** All Rights Reserved
**  
** This module is NOT free software
** Unauthorized distribution and copy prohibited
**
**/
using NLog;
using System;
using System.Text;

namespace softsyst.Generic.Logger
{
    // Log wrapper 
    public static class logging2
    {
        private static string MakeNewLogFilePath(string dir, string fileNamePrefix)
        {
            string fname = string.Format("{0}-{1:yyyy-MM-dd--HH-mm-ss}.txt", fileNamePrefix, DateTime.Now);
            return dir + fname;
        }

        public static bool SetNLogLevel(string lev)
        {
            try
            {
                LogLevel level = LogLevel.FromString(lev);
                SetNlogLogLevel(level);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void ShutdownLogger()
        {
            LogManager.Shutdown();
        }
        /// <summary>
        /// Reconfigures the NLog logging level.
        /// </summary>
        /// <param name="level">The <see cref="LogLevel" /> to be set.</param>
        ///  https://gist.github.com/pmullins/f21c3d83e96b9fd8a720
        
        private static void SetNlogLogLevel(LogLevel level)
        {
           // 'Trace'  0
           // 'Debug'  1
           // 'Info'   2
           // 'Warn'   3
           // 'Error'  4
           // 'Fatal'  5
           // 'Off'    6
       
            // Uncomment these to enable NLog logging. NLog exceptions are swallowed by default.
            ////NLog.Common.InternalLogger.LogFile = @"C:\Temp\nlog.debug.log";
            ////NLog.Common.InternalLogger.LogLevel = LogLevel.Debug;

            if (level == LogLevel.Off)
            {
                LogManager.DisableLogging();
            }
            else
            {
                if (!LogManager.IsLoggingEnabled())
                {
                    LogManager.EnableLogging();
                }

                foreach (var rule in LogManager.Configuration.LoggingRules)
                {
                    // Iterate over all levels up to and including the target, (re)enabling them.
                    for (int i =0; i < level.Ordinal; i++)
                    {
                        rule.DisableLoggingForLevel(LogLevel.FromOrdinal(i));
                    }
                    // Iterate over all levels up to and including the target, (re)enabling them.
                    for (int i = level.Ordinal; i <= 5; i++)
                    {
                        rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                    }
                }
            }

            LogManager.ReconfigExistingLoggers();
        }
        /// <summary>
        /// Configuration of the log
        /// </summary>
        public static bool configureLog(string level = "Info", string udpAddress = "127.0.0.1", int udpPort = 878,
            string directory = @"C:/tmp/logging/", string logFileNamePrefix = @"logDefault_",
            int maxLogFileSizeKB = 10000,
            bool fileLogging = true)
        {
            try
            {
                var config = new NLog.Config.LoggingConfiguration();
                NLog.Targets.FileTarget logfile = null;

                string fpath = "Unassigned";
                if (fileLogging)
                {
                    string logDir = directory;
                    logDir += logFileNamePrefix;
                    fpath = MakeNewLogFilePath(logDir, logFileNamePrefix);
                    logfile = new NLog.Targets.FileTarget("logfile") { FileName = fpath };
                    logfile.Layout = "${log4jxmlevent}";
                    logfile.KeepFileOpen = true;
                    logfile.ArchiveAboveSize = maxLogFileSizeKB * 1000;
                    logfile.ArchiveEvery = NLog.Targets.FileArchivePeriod.Day;
                }
                NLog.Targets.NetworkTarget network = new("network");
                string udp = $"udp://{udpAddress}:{udpPort}";
                network.Address = udp;
                network.Layout = "${log4jxmlevent:includeCallSite=true:includeNLogData=true}";
                network.NewLine = false;
                network.MaxMessageSize = 65000;
                network.Encoding = new UTF8Encoding();

                // Targets where to log to: File and Network. Omit console currently.
                //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                // Rules for mapping loggers to targets            
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, network);

                // Apply config           
                NLog.LogManager.Configuration = config;

                logging2.SetNLogLevel(level);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
