using System;

namespace softsyst.Generic.Sys
{
    public sealed class PlatformDetector
    {
        public enum Platform { WINDOWS, UNIX, UNKNOWN };

        public static Platform detectPlatform()
        {
            PlatformID pid = Environment.OSVersion.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return Platform.WINDOWS;
                case PlatformID.Unix:
                    return Platform.UNIX;
                default:
                    return Platform.UNKNOWN;
            }
        }
    }
}
