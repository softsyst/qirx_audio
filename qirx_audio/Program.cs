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
using System.Threading;

namespace softsyst.qirx.Audio
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("QIRX AAC stream player V1.0");
            Console.WriteLine("Copyright (c) softsyst GmbH and Clem Schmidt, 2019, www.softsyst.com\n");

            Console.WriteLine("libfaad2 is Copyright (C) 2003-2005 M. Bakker, Nero AG, http://www.nero.com\n");
            Console.WriteLine("NAudio is an open source .NET audio library written by Mark Heath (mark.heath@gmail.com)");
            Console.WriteLine("For more information, visit http://github.com/naudio/NAudio");

            Console.WriteLine("\n The following UDP Addresses and ports are used:");
            Console.WriteLine("\tAAC Streams: \tRemote endpoint:\t 127.0.0.1:8766");
            Console.WriteLine("\tAAC Streams: \tOwn endpoint:\t\t 127.0.0.1:8767");
            Console.WriteLine("\tCommands: \tRemote endpoint:\t 127.0.0.1:8768");
            Console.WriteLine("\tCommands: \tOwn endpoint: \t\t 127.0.0.1:8769\n");


            while(true)
            {

                mp4 MP4 = new mp4();

                Thread threadCmd= new Thread(MP4.UDPCmdReceiveSync);
                threadCmd.Start();

                Thread threadRx = new Thread(MP4.UDPReceiveSync);
                threadRx.Start();
                threadRx.Join();
                MP4.terminateCmdThread = true;
                threadCmd.Join();
            }
        }
    }
}
