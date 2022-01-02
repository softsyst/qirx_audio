using softsyst.Generic.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static softsyst.Generic.Sys.PlatformDetector;

namespace softsyst.Generic
{
    public class ImportResolver
    {
        private static readonly NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger(); // = null;// new logging<configFile>(logging2.log);

        private static Platform _platform;

        public ImportResolver(Assembly assembly )
        {
            _platform = PlatformDetector.detectPlatform();

            // Register the import resolver before calling the imported function.
            // Only one import resolver can be set for a given assembly.
            NativeLibrary.SetDllImportResolver(assembly, DllImportResolver);
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            try
            {
                switch (assembly.GetName().Name)
                {
                    case "qirx_audio":
                        if (libraryName == "libportaudio.so.2")
                        {
                            // On Linux, load the default library.
                            if (_platform == Platform.WINDOWS)
                            {
                                return NativeLibrary.Load("libportaudio64bit", assembly, searchPath);
                            }
                        }
                        else if (libraryName == "libc")
                        {
                            // On Linux, load the default library.
                            if (_platform == Platform.WINDOWS)
                            {
                                return NativeLibrary.Load("msvcrt", assembly, searchPath);
                            }
                        }
                        else if (libraryName == "libfaad.so")
                        {
                            // On Linux, load the default library.
                            if (_platform == Platform.WINDOWS)
                            {
                                return NativeLibrary.Load("libfaad2", assembly, searchPath);
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                logger?.Error($"Cannot load library {libraryName}: {e.Message}");
            }
            // Otherwise, fallback to default import resolver.
            return IntPtr.Zero;
        }
    }
}
