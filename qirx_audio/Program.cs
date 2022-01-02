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
using System.Reflection;
using System.Threading;
using softsyst.Generic;

namespace softsyst.qirx.Audio
{
    class Program
    {
        static ImportResolver _impResolve;
        //static private char inChar = ' ';
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine("QIRX AAC stream player V2.2");
            Console.WriteLine("Copyright (c) Clem Schmidt, 2019-2021, qirx.softsyst.com\n");

            Console.WriteLine("libfaad2 is Copyright (C) 2003-2005 M. Bakker, Nero AG, http://www.nero.com\n");
            Console.WriteLine("NAudio is an open source .NET audio library written by Mark Heath (mark.heath@gmail.com)");
            Console.WriteLine("For more information, visit http://github.com/naudio/NAudio");
            Console.WriteLine("PortAudio Portable Real-Time Audio Library, Copyright (c) 1999-2011 Ross Bencina and Phil Burk");
            Console.WriteLine("PortAudio P/Invoke interfacing based on work by Atsushi Eno, https://github.com/atsushieno/portaudio-sharp");

            Console.WriteLine("\n The following UDP Addresses and ports are used:");
            Console.WriteLine("\tAAC Streams: \tRemote endpoint:\t 127.0.0.1:8766");
            Console.WriteLine("\tAAC Streams: \tOwn endpoint:\t\t 127.0.0.1:8767");
            Console.WriteLine("\tCommands: \tRemote endpoint:\t 127.0.0.1:8768");
            Console.WriteLine("\tCommands: \tOwn endpoint: \t\t 127.0.0.1:8769\n");

            _impResolve = new ImportResolver(assembly);
            if (_impResolve != null)
                Console.WriteLine($"\tImport Resolver for assembly {assembly} created.\n");

            // Endless loop, only terminated by closing the exe.
            // The two threads are created on startup and/or selection of a service,
            // one for the commands, and one for the audio stream to be decoded.
            while(true)
            {

                mp4 MP4 = new mp4();

                Thread threadCmd= new Thread(MP4.UDPCmdReceiveSync);
                threadCmd.Start();

                Thread threadRx = new Thread(MP4.UDPReceiveSync);
                threadRx.Start();

                //// TODO, threads run still forever.
                //while (inChar != 'X')
                //{
                //    string inp = Console.ReadLine();
                //    inChar = inp[0];
                //}
                //MP4.terminateCmdThread = true;

                threadRx.Join();
                threadCmd.Join();
            }
        }
    }
}
