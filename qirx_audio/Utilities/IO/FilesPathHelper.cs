using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace softsyst.Generic.IO
{
    public sealed class FilesPathHelper
    {
        //https://stackoverflow.com/questions/20405965/how-to-ensure-there-is-trailing-directory-separator-in-paths/20406830
        public static string PathAddBackslash(string path)
        {
            if (string.IsNullOrEmpty( path) || string.IsNullOrWhiteSpace(path))
                return string.Empty;

            path = path.TrimEnd();

            if (PathEndsWithDirectorySeparator(path))
                return path;

            return path + GetDirectorySeparatorUsedInPath(path);

        }

        public static bool PathEndsWithDirectorySeparator(string path)
        {
            if (path.Length == 0)
                return false;

            char lastChar = path[path.Length - 1];
            return lastChar == Path.DirectorySeparatorChar
                || lastChar == Path.AltDirectorySeparatorChar;
        }

        public static char GetDirectorySeparatorUsedInPath(string path)
        {
            if (path.Contains(Path.AltDirectorySeparatorChar))
                return Path.AltDirectorySeparatorChar;

            return Path.DirectorySeparatorChar;
        }

        public static bool hasExeExtension(string path)
        {
            if (!Path.HasExtension(path))
                return false;
            string extension = Path.GetExtension(path);

            if (extension.ToLower() == "exe")
                return true;
            return false;
        }
    }
}
