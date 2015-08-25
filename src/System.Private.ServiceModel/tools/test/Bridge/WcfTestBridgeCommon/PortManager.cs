// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    // This class exists to create and delete firewall rules to
    // manage which ports are open on behalf of the Bridge.
    public static class PortManager
    {
        // This prefix is used both to name rules and to discover existing
        // rules created by this class, so it must be unique
        private static string s_RuleNamePrefix = "Bridge WCF test port rule";
        private static object s_portLock = new object();
        private static bool s_registeredForProcessExit = false;
        private static INetFwPolicy2 s_netFwPolicy2;
        private static string s_remoteAddresses = "LocalSubnet";

        private static INetFwPolicy2 NetFwPolicy2
        {
            get
            {
                lock (s_portLock)
                {
                    if (s_netFwPolicy2 == null)
                    {
                        Type netFwPolicy2Type = Type.GetTypeFromProgID("HNetCfg.FwPolicy2", false);
                        s_netFwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(netFwPolicy2Type);
                    }
                }

                return s_netFwPolicy2;
            }
        }

        public static string RemoteAddresses
        {
            get
            {
                return s_remoteAddresses;
            }
            set
            {
                s_remoteAddresses = value;
            }
        }
 
        // We listen for ProcessExit so we can delete the firewall
        // rules we added while the Bridge was running.
        private static void RegisterForProcessExit()
        {
            lock (s_portLock)
            {
                if (!s_registeredForProcessExit)
                {
                    AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                    {
                        RemoveAllBridgeFirewallRules();
                    };
                    s_registeredForProcessExit = true;
                }
            }
        }

        // Searches all existing firewall rules with the given name
        private static INetFwRule FindRule(string name, int port)
        {
            lock (s_portLock)
            {
                // Match on our special naming pattern and port
                HashSet<string> ruleSet = new HashSet<string>();
                foreach (var r in NetFwPolicy2.Rules)
                {
                    INetFwRule rule = (INetFwRule)r;
                    if (String.Equals(name, rule.Name, StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(rule.LocalPorts, port.ToString(), StringComparison.Ordinal))
                    {
                        return rule;
                    }
                }

                return null;
            }
        }

        private static void AddFirewallRule(int port)
        {
            lock (s_portLock)
            {
                // If we add any rules, register to delete them at process exit.
                RegisterForProcessExit();

                // If we already created this rule, we don't create it again.
                string ruleName = String.Format("{0} {1}", s_RuleNamePrefix, port);
                if (FindRule(ruleName, port) != null)
                {
                    return;
                }

                INetFwRules rulesObject = NetFwPolicy2.Rules;
                int currentProfiles = NetFwPolicy2.CurrentProfileTypes;

                // Create a Rule Object.
                Type netFwRuleType = Type.GetTypeFromProgID("HNetCfg.FWRule", false);
                INetFwRule newRule = (INetFwRule)Activator.CreateInstance(netFwRuleType);

                try
                {
                    newRule.Name = ruleName;
                    newRule.Description = String.Format("Rule added for Bridge WCF test use of port {0}", port);
                    newRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                    newRule.LocalPorts = port.ToString();
                    newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                    newRule.Enabled = true;
                    newRule.Profiles = currentProfiles;
                    newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    newRule.RemoteAddresses = RemoteAddresses;

                    // Add a new rule
                    rulesObject.Add(newRule);

                    Trace.WriteLine(String.Format("Added firewall rule {0}", newRule.Name),
                                    typeof(PortManager).Name);
                }
                catch (Exception ex)
                {
                    string message = String.Format("Failed to add firewall rule name:{0}, port:{1}, remoteAddresses:{2}{3}{4}",
                                                    newRule.Name, port, RemoteAddresses, Environment.NewLine, ex.ToString());
                    Console.WriteLine(message);
                    Trace.TraceWarning(message);
                }
            }
        }

        public static void RemoveAllBridgeFirewallRules()
        {
            lock (s_portLock)
            {
                // Capture Bridge-specific rules into local list so we
                // don't mutate the rule list during enumeration.
                HashSet<string> ruleSet = new HashSet<string>();
                foreach (var r in NetFwPolicy2.Rules)
                {
                    INetFwRule rule = (INetFwRule)r;
                    string ruleName = rule.Name;
                    if (rule.Name.StartsWith(s_RuleNamePrefix, StringComparison.Ordinal) && !ruleSet.Contains(rule.Name))
                    {
                        ruleSet.Add(rule.Name);
                    }
                }

                foreach (string ruleName in ruleSet)
                {
                    try {
                        NetFwPolicy2.Rules.Remove(ruleName);
                        Console.WriteLine("Removed firewall rule '{0}'", ruleName);
                    }
                    catch (FileNotFoundException fnfe)
                    {
                        // This exception can happen when multiple processes
                        // are cleaning up the rules, and the rule has already
                        // been removed.
                        Console.WriteLine("Unable to remove rule '{0}' : {1}",
                                            ruleName, fnfe.Message);
                    }
                }
            }
        }

        public static void OpenPortInFirewall(int port)
        {
            AddFirewallRule(port);
        }
    }
}

