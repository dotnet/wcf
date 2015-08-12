// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NetFwTypeLib;
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
        private static string s_PortNameFormat = "Bridge auto-rule {0}";
        private static object s_portLock = new object();
        private static Dictionary<int, string> s_AddedRulesByPort = new Dictionary<int, string>();
        private static bool s_registeredForProcessExit = false;

        private static void RegisterForProcessExit()
        {
            lock (s_portLock)
            {
                if (!s_registeredForProcessExit)
                {
                    AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                        {
                            CloseAllOpenedPortsInFireWall();
                        };
                    s_registeredForProcessExit = true;
                }
            }
        }

        private static void AddNewFirewallRule(int port)
        {
            lock (s_portLock)
            {
                if (s_AddedRulesByPort.ContainsKey(port))
                {
                    return;
                }

                // If we add any rules, register to delete them at process exit.
                RegisterForProcessExit();

                // Create the FwPolicy2 object.
                Type netFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);
                INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(netFwPolicy2Type);

                // Get the Rules object
                INetFwRules rulesObject = fwPolicy2.Rules;

                int CurrentProfiles = fwPolicy2.CurrentProfileTypes;

                // Create a Rule Object.
                Type netFwRuleType = Type.GetTypeFromProgID("HNetCfg.FWRule", false);
                INetFwRule newRule = (INetFwRule)Activator.CreateInstance(netFwRuleType);

                newRule.Name = String.Format(s_PortNameFormat, port);
                newRule.Description = String.Format("Rule added for Bridge use of port {0}", port);
                newRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                newRule.LocalPorts = port.ToString();
                newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                newRule.Enabled = true;
                newRule.Profiles = CurrentProfiles;
                newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;

                // Add a new rule
                rulesObject.Add(newRule);

                Trace.WriteLine(String.Format("Added firewall rule {0}", newRule.Name),
                                typeof(PortManager).Name);

                s_AddedRulesByPort[port] = newRule.Name;
            }
        }

        private static void CloseAllOpenedPortsInFireWall()
        {
            Type NetFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(NetFwPolicy2Type);

            // Get the Rules object
            INetFwRules RulesObject = fwPolicy2.Rules;

            lock (s_portLock)
            {
                foreach (var pair in s_AddedRulesByPort)
                {
                    RulesObject.Remove(pair.Value);
                    Trace.WriteLine(String.Format("Removed firewall rule {0}", pair.Value),
                                    typeof(PortManager).Name);
                }

                s_AddedRulesByPort.Clear();
            }
        }

        public static void OpenPortInFirewall(int port)
        {
            AddNewFirewallRule(port);
        }
    }
}

