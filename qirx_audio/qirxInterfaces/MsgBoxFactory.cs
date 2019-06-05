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

using System.Reflection;
using System;
using System.IO;

namespace softsyst.qirx.Interfaces
{
    public static class MsgBoxFactory
    {
        public static string dllName = "PopupDialog.dll";

        public static IMsgBox _msgBox;

        /// <summary>
        /// Create the instance: first try to create from the resources
        /// </summary>
        /// <returns></returns>
        public static IMsgBox Create()
        {
            if (_msgBox != null)
                return _msgBox;
            // First try to load from an embedded resource
            try
            {
                Assembly entryAssy = Assembly.GetEntryAssembly();
                //Assembly executingAssembly = Assembly.GetExecutingAssembly();
                byte[] assemblyRawBytes;
                Assembly popup = null;

                using (Stream stream = entryAssy.GetManifestResourceStream(dllName))
                {
                    if (stream != null)
                    {
                        assemblyRawBytes = new byte[stream.Length];
                        stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                        popup = Assembly.Load(assemblyRawBytes);
                        Type T = popup.GetType("softsyst.qirx.PopupDialog.MsgBox");
                        _msgBox = (IMsgBox)Activator.CreateInstance(T);
                        return _msgBox;
                    }
                }
            }
            catch
            {
            }

            //Second try from a file
            try
            {
                string assemblyPath = System.AppDomain.CurrentDomain.BaseDirectory + dllName;
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                Type T = assembly.GetType("softsyst.qirx.PopupDialog.MsgBox");
                IMsgBox msgBox = (IMsgBox)Activator.CreateInstance(T);
                return msgBox;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}