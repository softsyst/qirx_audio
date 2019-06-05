using System;
using System.Collections.Generic;
using System.Reflection;

namespace softsyst.Generic.Assemblies
{
    public static class AssembliesHelper
    {
        /// <summary>
        /// Loads an assembly object array
        /// </summary>
        /// <returns>List of Assembly objects</returns>
        public static Assembly[] listLoadedAssemblies()
        {
            List<Assembly> assyList = new List<Assembly>();
            try
            {
                AppDomain curdom = AppDomain.CurrentDomain;
                Assembly[] assems = curdom.GetAssemblies();
                foreach (Assembly assm in assems)
                    assyList.Add(assm);
            }
            catch
            {
            }
            return assyList.ToArray();
        }

        public static string AssemblyName(Assembly assy)
        {
            string fullName = assy.FullName;
            string[] parts = fullName.Split(',');
            if (parts.Length > 0)
                return parts[0];
            return string.Empty;
        }

        public static string AssemblyVersion(Assembly assy)
        {
            try
            {
                string fullName = assy.FullName;
                string[] parts = fullName.Split(',');
                if (parts.Length > 1)
                {
                    string v = parts[1].Substring(parts[1].IndexOf('=')+1);
                    return v;
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public static string AssemblyCopyright(Assembly assy)
        {
            try
            {
                object[] atts = assy.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (atts.Length == 0)
                    return string.Empty;
                return ((AssemblyCopyrightAttribute)atts[0]).Copyright;
            }
            catch
            {
            }
            return string.Empty;
        }

        public static string AssemblyDescription(Assembly assy)
        {
            try
            {
                object[] atts = assy.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (atts.Length == 0)
                    return string.Empty;
                return ((AssemblyDescriptionAttribute)atts[0]).Description;
            }
            catch
            {
            }
            return string.Empty;
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        public static string AssembVersion
        {
            get
            {
                string assVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                assVersion += " BW Test";
                //assVersion += " Beta";
                //if (TIIavailable)
                //    assVersion += " TII ";

                return assVersion;
            }
        }
        public static string AssemblyProductVersion
        {
            get
            {
                string assVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                //Delete the last (fourth) number to be compatible with Explorer
                string[] versionParts = assVersion.Split('.');
                if (versionParts.Length < 4)
                    return assVersion;

                int ix = assVersion.LastIndexOf('.');
                string productVersion = assVersion.Remove(ix);
                return productVersion;
            }
        }

        public static string AssembDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssembCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
    }
}
