// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Tools.ServiceModel.Svcutil.Metadata;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CmdCredentialsProvider : IHttpCredentialsProvider, IClientCertificateProvider, IServerCertificateValidationProvider
    {
        #region IHttpCredentialProvider

        private bool _authMessageShown;

        public NetworkCredential GetCredentials(Uri serviceUri, WebException webException)
        {
            ShowAuthenticationConsent();

            string username = null;
            while (string.IsNullOrWhiteSpace(username))
            {
                username = ReadUserInput(SR.UsernamePrompt);
                Console.WriteLine();
            }
            username = username.Trim();

            var password = ReadUserInput(SR.PasswordPrompt, isPassword: true);
            Console.WriteLine();

            return new NetworkCredential(username, password);
        }

        private void ShowAuthenticationConsent()
        {
            if (!_authMessageShown)
            {
                _authMessageShown = true;

                Console.WriteLine();
                Console.WriteLine(SR.WrnUserBasicCredentialsInClearText);
                PromptEnterOrEscape(throwOnEscape: true);
            }
        }

        #endregion

        #region IClientCertificateProvider
        private const string OidClientAuthValue = "1.3.6.1.5.5.7.3.2";

        private X509Certificate2Collection _certificates;
        private X509Certificate2Collection Certificates
        {
            get
            {
                if (_certificates == null)
                {
                    _certificates = GetCertificates();
                }
                return _certificates;
            }
        }

        internal bool AcceptCert { get; set; }

        public X509Certificate2Collection GetCertificates()
        {
            X509Certificate2Collection certs = new X509Certificate2Collection();
            X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                certificateStore.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                foreach (X509Certificate2 certificate in certificateStore.Certificates)
                {
                    if (certificate.HasPrivateKey)
                    {
                        bool digitalSignatureUsage = false;
                        bool clientAuthEnhancedUsage = false;
                        bool enhancedKeyUsageSupported = false;

                        foreach (X509Extension extension in certificate.Extensions)
                        {
                            X509KeyUsageExtension keyUsage = extension as X509KeyUsageExtension;
                            if (keyUsage != null)
                            {
                                digitalSignatureUsage = (keyUsage.KeyUsages & X509KeyUsageFlags.DigitalSignature) != 0;
                            }
                            else
                            {
                                X509EnhancedKeyUsageExtension enhancedKeyUsage = extension as X509EnhancedKeyUsageExtension;
                                if (enhancedKeyUsage != null && enhancedKeyUsage.EnhancedKeyUsages != null)
                                {
                                    enhancedKeyUsageSupported = true;
                                    foreach (var oid in enhancedKeyUsage.EnhancedKeyUsages)
                                    {
                                        if (oid.Value == OidClientAuthValue)
                                        {
                                            clientAuthEnhancedUsage = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (digitalSignatureUsage && (!enhancedKeyUsageSupported || clientAuthEnhancedUsage))
                        {
                            certs.Add(certificate);
                        }
                    }
                }
            }
            finally
            {
                certificateStore.Dispose();
            }

            return certs;
        }

        public X509Certificate GetCertificate(Uri serviceUri)
        {
            X509Certificate2 cert = null;
            if (this.Certificates.Count > 0)
            {
                cert = this.Certificates.Count > 1 ? SelectCertificateFromCollection(this.Certificates, serviceUri) : this.Certificates[0];
            }
            return cert;
        }

        private Dictionary<string, X509Certificate> _validatedClientCerts = new Dictionary<string, X509Certificate>();

        private X509Certificate2 SelectCertificateFromCollection(X509Certificate2Collection selectedCerts, Uri serviceUri)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, SR.CertificateSelectionMessageFormat, serviceUri.Authority));
            if (!AcceptCert)
            {
                PromptEnterOrEscape(throwOnEscape: true);
            }

            var candidateCerts = new List<X509Certificate2>();
            int counter = 1;
            foreach (var cert in selectedCerts)
            {
                var certhash = cert.GetCertHashString();
                if (!_validatedClientCerts.Keys.Contains(certhash))
                {
                    candidateCerts.Add(cert);
                    var certId = counter++ + ".";
                    Console.WriteLine(FormatCertificate(cert, certId));
                }
            }

            string idxString; ;
            int idx = 0;
            do
            {
                idxString = ReadUserInput(SR.CertificateIndexPrompt);
                Console.WriteLine();
            }
            while (!int.TryParse(idxString, out idx) || idx < 1 || idx > candidateCerts.Count);

            var selectedCert = candidateCerts[idx - 1];
            _validatedClientCerts[selectedCert.GetCertHashString()] = selectedCert;

            return selectedCert;
        }

        #endregion

        #region IServerCertificateValidationProvider

        private Uri _serviceUri;

        public void BeforeServerCertificateValidation(Uri serviceUri)
        {
#if NETCORE10
            // NOOP
#else
            System.Diagnostics.Debug.Assert(_serviceUri == null, "provider already started for the specified service URI");
            _serviceUri = serviceUri;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(this.ValidateServerCertificate);
#endif
        }

        public void AfterServerCertificateValidation(Uri serviceUri)
        {
#if NETCORE10
            // NOOP
#else
            System.Diagnostics.Debug.Assert(_serviceUri == serviceUri, "provider not statrted for the specified service URI");
            _serviceUri = null;
            ServicePointManager.ServerCertificateValidationCallback -= new RemoteCertificateValidationCallback(this.ValidateServerCertificate);
#endif
        }


        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;

            HttpWebRequest request = sender as HttpWebRequest;
            if (request != null && _serviceUri != null && _serviceUri.Authority == request.RequestUri.Authority)
            {
                result = sslPolicyErrors == SslPolicyErrors.None ? true : PromptUserOnInvalidCert(cert, sslPolicyErrors);
            }

            return result;
        }

        private Dictionary<string, bool> _validatedServerCerts = new Dictionary<string, bool>();

        private bool PromptUserOnInvalidCert(X509Certificate cert, SslPolicyErrors sslPolicyErrors)
        {
            var certhash = cert.GetCertHashString();

            if (!_validatedServerCerts.Keys.Contains(certhash))
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, SR.ErrServerCertFailedValidationFormat, sslPolicyErrors, FormatCertificate(cert)));
                _validatedServerCerts[certhash] = AcceptCert ? true : PromptEnterOrEscape(throwOnEscape: false);
            }

            return _validatedServerCerts[certhash];
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new CmdCredentialsProvider() { AcceptCert = AcceptCert };            
        }

        #endregion

        #region common functions

        private static string FormatCertificate(X509Certificate cert, string certId = null)
        {
            var separator = "--------------------------------------------------------";
            return separator + Environment.NewLine + certId + cert + separator;
        }

        private static bool PromptEnterOrEscape(bool throwOnEscape)
        {
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.WriteLine(SR.EnterOrEscapeMessage);
                keyInfo = Console.ReadKey(intercept: true);
            }
            while (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Escape);

            Console.WriteLine();
            if (keyInfo.Key == ConsoleKey.Escape && throwOnEscape)
            {
                throw new OperationCanceledException();
            }

            return keyInfo.Key == ConsoleKey.Enter;
        }

        public static string ReadUserInput(string prompt, bool isPassword = false)
        {
            ConsoleKeyInfo keyInfo;
            StringBuilder userInput = new StringBuilder();

            Console.Write(prompt);

            do
            {
                keyInfo = System.Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    throw new OperationCanceledException();
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && userInput.Length > 0)
                {
                    Console.Write("\b \b");
                    userInput = userInput.Remove(userInput.Length - 1, 1);
                }
                else if (!Char.IsControl(keyInfo.KeyChar))
                {
                    userInput.Append(keyInfo.KeyChar);
                    System.Console.Write(isPassword ? '*' : keyInfo.KeyChar);
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);

            return userInput.ToString();
        }

        #endregion
    }

    internal static class CertificateExtensions
    {
        public static string GetCertHashString(this X509Certificate cert)
        {
            return Encoding.Unicode.GetString(cert.GetCertHash());
        }
    }
}
