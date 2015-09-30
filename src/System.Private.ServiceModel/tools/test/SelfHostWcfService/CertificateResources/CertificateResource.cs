// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    public abstract class CertificateResource : IResource
    {
        protected const string certificateResourceString = "certificate";
        protected const string crlResourceString = "crl";
        protected const string subjectResourceString = "subject";
        protected const string subjectssResourceString = "subjects";
        protected const string thumbprintResourceString = "thumbprint";
        protected const string thumbprintsResourceString = "thumbprints";
        protected const string isLocalResourceString = "isLocal"; 

        protected static string s_localHostname; 
        
        // key: subject CN
        protected static Dictionary<string, X509Certificate2> s_createdCerts = new Dictionary<string, X509Certificate2>();
        protected static object s_certificateResourceLock = new object();

        public abstract ResourceResponse Get(ResourceRequestContext context);
        public abstract ResourceResponse Put(ResourceRequestContext context);

        static CertificateResource()
        {
            CertificateManager.BridgeConfigurationChanged += (object s, EventArgs args) => OnBridgeConfigurationChanged(s, args);
            s_localHostname = Dns.GetHostEntry("127.0.0.1").HostName;
        }

        protected bool IsLocalMachineResource(string subject)
        {
            return string.Compare("127.0.0.1", subject, StringComparison.OrdinalIgnoreCase) == 0 
                || string.Compare("localhost", subject, StringComparison.OrdinalIgnoreCase) == 0 
                || string.Compare(subject, s_localHostname,StringComparison.OrdinalIgnoreCase) == 0 
                || string.Compare(subject, s_localHostname.Split('.')[0], StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static void OnBridgeConfigurationChanged(object sender, EventArgs args)
        {
            lock (s_certificateResourceLock)
            {
                s_createdCerts.Clear();
            }
        }
    }
}
