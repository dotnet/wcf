//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

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

        private bool authMessageShown;

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
            if (!authMessageShown)
            {
                authMessageShown = true;

                Console.WriteLine();
                Console.WriteLine(SR.WrnUserBasicCredentialsInClearText);
                PromptEnterOrEscape(throwOnEscape: true);
            }
        }

        #endregion

        #region IClientCertificateProvider
        private const string OidClientAuthValue = "1.3.6.1.5.5.7.3.2";

        private X509Certificate2Collection certificates;
        private X509Certificate2Collection Certificates
        {
            get
            {
                if (this.certificates == null)
                {
                    this.certificates = GetCertificates();
                }
                return this.certificates;
            }
        }

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

        Dictionary<string, X509Certificate> validatedClientCerts = new Dictionary<string, X509Certificate>();

        private X509Certificate2 SelectCertificateFromCollection(X509Certificate2Collection selectedCerts, Uri serviceUri)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, SR.CertificateSelectionMessageFormat, serviceUri.Authority));
            PromptEnterOrEscape(throwOnEscape: true);

            var candidateCerts = new List<X509Certificate2>();
            int counter = 1;
            foreach (var cert in selectedCerts)
            {
                var certhash = cert.GetCertHashString();
                if (!validatedClientCerts.Keys.Contains(certhash))
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
            validatedClientCerts[selectedCert.GetCertHashString()] = selectedCert;

            return selectedCert;
        }

        #endregion

        #region IServerCertificateValidationProvider

        private Uri serviceUri;

        public void BeforeServerCertificateValidation(Uri serviceUri)
        {
#if NETCORE10
            // NOOP
#else
            System.Diagnostics.Debug.Assert(this.serviceUri == null, "provider already started for the specified service URI");
            this.serviceUri = serviceUri;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(this.ValidateServerCertificate);
#endif
        }

        public void AfterServerCertificateValidation(Uri serviceUri)
        {
#if NETCORE10
            // NOOP
#else
            System.Diagnostics.Debug.Assert(this.serviceUri == serviceUri, "provider not statrted for the specified service URI");
            this.serviceUri = null;
            ServicePointManager.ServerCertificateValidationCallback -= new RemoteCertificateValidationCallback(this.ValidateServerCertificate);
#endif
        }


        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;

            HttpWebRequest request = sender as HttpWebRequest;
            if (request != null && this.serviceUri != null && this.serviceUri.Authority == request.RequestUri.Authority)
            {
                result = sslPolicyErrors == SslPolicyErrors.None ? true : PromptUserOnInvalidCert(cert, sslPolicyErrors);
            }

            return result;
        }

        Dictionary<string, bool> validatedServerCerts = new Dictionary<string, bool>();

        private bool PromptUserOnInvalidCert(X509Certificate cert, SslPolicyErrors sslPolicyErrors)
        {
            var certhash = cert.GetCertHashString();

            if (!validatedServerCerts.Keys.Contains(certhash))
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, SR.ErrServerCertFailedValidationFormat, sslPolicyErrors, FormatCertificate(cert)));
                validatedServerCerts[certhash] = PromptEnterOrEscape(throwOnEscape: false);
            }

            return validatedServerCerts[certhash];
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new CmdCredentialsProvider();
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
            var userInput = string.Empty;

            Console.Write(prompt);

            do
            {
                keyInfo = System.Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    throw new OperationCanceledException();
                }

                if (!Char.IsControl(keyInfo.KeyChar))
                {
                    userInput += keyInfo.KeyChar;
                    System.Console.Write(isPassword ? '*' : keyInfo.KeyChar);
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);

            return userInput;
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
