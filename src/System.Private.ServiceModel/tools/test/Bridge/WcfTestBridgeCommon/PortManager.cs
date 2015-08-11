// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    // This class exists to create and delete firewall rules to
    // manage which ports are open on behalf of the Bridge.
    public class PortManager
    {
        private static HashSet<int> s_inPorts = new HashSet<int>();
        private static HashSet<int> s_outPorts = new HashSet<int>();
        private static object s_portLock = new object();

        public static void OpenPortInFirewall(int port)
        {
            OpenPortInFirewall(port, "in", s_inPorts);
            OpenPortInFirewall(port, "out", s_outPorts);
        }

        private static void OpenPortInFirewall(int port, string direction, HashSet<int> ports)
        {
            lock (s_portLock)
            {
                if (ports.Contains(port))
                {
                    return;
                }
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "netsh";
                processStartInfo.Arguments = String.Format("advfirewall firewall add rule name = \"Bridge auto-open {0} {1}\" protocol = TCP dir ={0} localport = {1} action = allow", direction, port);

                Process process = Process.Start(processStartInfo);
                process.WaitForExit();

                ports.Add(port);
            }
        }

        public static void ClosePortInFirewall(int port)
        {
            ClosePortInFirewall(port, "in", s_inPorts);
            ClosePortInFirewall(port, "out", s_outPorts);
        }

        private static void ClosePortInFirewall(int port, string direction, HashSet<int> ports)
        {
            lock (s_portLock)
            {
                if (!ports.Contains(port))
                {
                    return;
                }
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "netsh";
                processStartInfo.Arguments = String.Format("advfirewall firewall delete rule name = \"Bridge auto-open {0} {1}\" protocol = TCP dir ={0} localport = {1}", direction, port);

                Process process = Process.Start(processStartInfo);
                process.WaitForExit();

                ports.Remove(port);
            }
        }

        public static void CloseAllOpenedPortsInFireWall()
        {
            int[] ports = s_inPorts.ToArray();
            foreach (int p in ports)
            {
                ClosePortInFirewall(p);
            }

        }
    }
}

