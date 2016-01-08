// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WcfTestBridgeCommon;

namespace WcfService.CertificateResources
{
    // Base class for all Certificate Resources
    // Provides some helpers and a cache for all certificates created by this instance of the Bridge
    public abstract class CertificateResource : IResource
    {
        protected const string certificateKeyName = "certificate";
        protected const string crlKeyName = "crl";
        protected const string crlUriKeyName = "crlUri";
        protected const string revokedCertificatesKeyName = "revokedCertificates";
        protected const string revokeSerialNumberKeyName = "revoke";
        protected const string subjectKeyName = "subject";
        protected const string subjectAlternativeNamesKeyName = "subjectAlternativeNames";
        protected const string subjectsKeyName = "subjects";
        protected const string thumbprintKeyName = "thumbprint";
        protected const string thumbprintsKeyName = "thumbprints";
        protected const string isLocalKeyName = "isLocal";
        protected const string exportAsPemKeyName = "exportAsPem";

        protected static string s_localHostname; 
        
        // Cache for certs created via CertificateResources
        // key: subject CN, value: X509Certificate2
        protected static Dictionary<string, X509Certificate2> s_createdCertsBySubject = new Dictionary<string, X509Certificate2>();
        // key: cert thumbprint, value: X509Certificate2
        protected static Dictionary<string, X509Certificate2> s_createdCertsByThumbprint = new Dictionary<string, X509Certificate2>();
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
                s_createdCertsBySubject.Clear();
                s_createdCertsByThumbprint.Clear();
            }
        }

        public static string GetCertificateAsPem(X509Certificate2 certificate)
        {
            string base64String = Convert.ToBase64String(certificate.RawData);

            const string header = "-----BEGIN CERTIFICATE-----\n";
            const string footer = "-----END CERTIFICATE-----";

            StringBuilder builder = new StringBuilder(base64String.Length + header.Length + footer.Length);

            int base64StringIndex = 0; 
            builder.Append(header);
            while (base64StringIndex < base64String.Length)
            {
                int charactersToAppend = Math.Min(64, base64String.Length - base64StringIndex);

                builder.Append(base64String, base64StringIndex, charactersToAppend);

                // PEM dictates that this must be a \n, not OS-dependent 
                builder.Append('\n');

                base64StringIndex += charactersToAppend;
            }

            builder.Append(footer);

            return builder.ToString();
        }
    }
}
