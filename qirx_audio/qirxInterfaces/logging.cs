using log4net;
using softsyst.Generic.Logger.Interface;
using System.Reflection;
/**
** ** QIRX - Software Defined Radio - AAC Audio  
** Copyright (C) 2017 Clem Schmidt, softsyst GmbH, http://www.softsyst.com
** All Rights Reserved
**  
** This module is NOT free software
** Unauthorized distribution and copy prohibited
**
**/
using System;

namespace softsyst.Generic.Logger
{
    class instantiator
    {
        public string AssemblyPath { get; private set; }
        /// <summary>
        /// It is assumed the assembly resides in the exe's directory
        /// </summary>
        /// <param name="assemblyName"></param>
        public instantiator(string assemblyName)
        {
            AssemblyPath = System.AppDomain.CurrentDomain.BaseDirectory + assemblyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="typename">Fully qualified type string</param>
        /// <returns></returns>
        public object instance<T>(string typename)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(AssemblyPath);
                Type ty = assembly.GetType(typename);
                object o = Activator.CreateInstance(ty);
                if (o is T)
                    return o;
                return o;
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
    // Log wrapper for static classes
    public static class logging2
    {
        static public ILogger log;
        static public void logInfo(ILog logger, string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Info(txt);
        }
        static public void logDebug(ILog logger, string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Debug(txt);
        }
        static public void logWarning(ILog logger, string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Warn(txt);
        }
        static public void logError(ILog logger, string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Error(txt);
        }
        static public bool LogAvailable { get { return log != null; } }

        private const string loggingAssemblyName = "softsyst.Logger.dll";
        private const string loggingAssemblyType = "softsyst.Generic.Logger.Log";

        /// <summary>
        /// Instantiate the logging dll if present
        /// </summary>
        public static void createLogging()
        {
            if (logging2.log != null) //already avail
                return;
            try
            {
                instantiator inst = new instantiator(loggingAssemblyName);
                logging2.log = (ILogger)inst.instance<ILogger>(loggingAssemblyType);
            }
            catch //(Exception e)
            {
                logging2.log = null;
            }
        }

            /// <summary>
            /// Configuration of the log
            /// </summary>
            public static bool configureLog(string udpAddress = "127.0.0.1", int udpPort = 878, 
                string directory = @"C:/tmp/logging", string logFileNamePrefix = @"logDefault_",
                int maxLogFileSizeKB = 10000,
                bool fileLogging = true)
            {
                if (logging2.log == null)
                    return false;
                try
                {
                    logging2.log.SetUdpListenerEndpooint(udpAddress, udpPort);
                    logging2.log.SetLogLevelForUdpListeners(log4net.Core.Level.All);

                    // configuration.fileLogging == "yes";
                    if (fileLogging)
                    {
                        string logDir = directory;
                        logDir += logFileNamePrefix;

                        logging2.log.AddLogFileListener(logDir, maxLogFileSizeKB, 10);
                        logging2.log.SetLogLevelForAllFileListeners(log4net.Core.Level.All);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

    }
    /// <summary>
    /// Log Wrapper for dynamic classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class logging<T>
    {
        ILogger log;
        public ILog logger;
        public logging(ILogger logg)
        {
            if (logg == null)
                return;
            this.log = logg;
            logger = (ILog)log.Init(typeof(T));
        }

        public void Info(string txt)
        {
            if (log == null || logger == null) 
                return;
            logger.Info(txt);
        }

        public void Debug(string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Debug(txt);
        }

        public void Warning(string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Warn(txt);
        }

        public void Error(string txt)
        {
            if (log == null || logger == null)
                return;
            logger.Error(txt);
        }

        public void Exception(string txt, Exception ex)
        {
            if (log == null || logger == null)
                return;
            logger.Error("Exception: " + txt + "  " + ex.Message);
        }

        public void Exception(Exception ex)
        {
            if (log == null || logger == null)
                return;
            log.Exception(ex);
        }

        public bool LogAvailable { get { return log != null; } }
    }
}
