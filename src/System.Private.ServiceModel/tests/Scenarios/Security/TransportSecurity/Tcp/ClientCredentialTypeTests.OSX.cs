// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using Xunit;
using System.ServiceModel;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.IO;
using System.ServiceModel.Security;

public partial class Tcp_ClientCredentialTypeTests : ConditionalWcfTest
{
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(OSXPeer_Certificate_Installed),
           nameof(SSL_Available))]
    [OuterLoop]
    public static void NetTcp_SecModeTrans_CertValMode_OSXCustomStore_Succeeds_In_CustomStore()
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            endpointAddress = new EndpointAddress(
                                new Uri(Endpoints.NetTcp_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new OSXPeerCertificateValidator();

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    public class OSXPeerCertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            Exception exception;
            if (!TryValidate(certificate, out exception))
                throw exception;
        }

        static bool StoreContainsCertificate(X509Store store, X509Certificate2 certificate)
        {
            X509Certificate2Collection certificates = null;
            try
            {
                certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                return certificates.Count > 0;
            }
            finally
            {
                ResetAllCertificates(certificates);
            }
        }

        // This is a workaround, Since store.Certificates returns a full collection
        // of certs in store.  These are holding native resources.
        internal static void ResetAllCertificates(X509Certificate2Collection certificates)
        {
            if (certificates != null)
            {
                for (int i = 0; i < certificates.Count; ++i)
                {
                    ResetCertificate(certificates[i]);
                }
            }
        }

        internal static void ResetCertificate(X509Certificate2 certificate)
        {
            certificate.Dispose();
        }

        internal bool TryValidate(X509Certificate2 certificate, out Exception exception)
        {
            // Checklist
            // 1) time validity of cert
            // 2) in local trusted key chain (in place of Trusted People store)
            // 3) not in disallowed store

            DateTime now = DateTime.Now;

            if (now > certificate.NotAfter || now < certificate.NotBefore)
            {
                exception = new Exception($"The X.509 certificate ({GetCertificateId(certificate)}) usage time is invalid.  " +
                    $"The usage time '{now}' does not fall between NotBefore time '{certificate.NotBefore}' and NotAfter time '{certificate.NotAfter}'.");
                return false;
            }

            if (!File.Exists(ServiceUtilHelper.OSXCustomKeychainFilePath))
            {
                // The certificate can't be in a non-existent keychain file
                exception = new Exception($"The X.509 certificate {GetCertificateId(certificate)} is not in the keychain file {ServiceUtilHelper.OSXCustomKeychainFilePath}.");
                return false;
            }

            X509Store store;
            using (SafeKeychainHandle keychain = SafeKeychainHandle.Open(ServiceUtilHelper.OSXCustomKeychainFilePath, ServiceUtilHelper.OSXCustomKeychainPassword))
            {
                using (store = new X509Store(keychain.DangerousGetHandle()))
                {
                    if (!StoreContainsCertificate(store, certificate))
                    {
                        exception = new Exception($"The X.509 certificate {GetCertificateId(certificate)} is not in the keychain file {ServiceUtilHelper.OSXCustomKeychainFilePath}.");
                        return false;
                    }
                }
            }

            using (store = new X509Store(StoreName.Disallowed, StoreLocation.CurrentUser))
            {
                if (StoreContainsCertificate(store, certificate))
                {
                    exception = new Exception($"The {GetCertificateId(certificate)} X.509 certificate is in an untrusted certificate store.");
                    return false;
                }
            }

            exception = null;
            return true;
        }

        internal static string GetCertificateId(X509Certificate2 certificate)
        {
            StringBuilder str = new StringBuilder(256);
            AppendCertificateIdentityName(str, certificate);
            return str.ToString();
        }

        internal static void AppendCertificateIdentityName(StringBuilder str, X509Certificate2 certificate)
        {
            string value = certificate.SubjectName.Name;
            if (String.IsNullOrEmpty(value))
            {
                value = certificate.GetNameInfo(X509NameType.DnsName, false);
                if (String.IsNullOrEmpty(value))
                {
                    value = certificate.GetNameInfo(X509NameType.SimpleName, false);
                    if (String.IsNullOrEmpty(value))
                    {
                        value = certificate.GetNameInfo(X509NameType.EmailName, false);
                        if (String.IsNullOrEmpty(value))
                        {
                            value = certificate.GetNameInfo(X509NameType.UpnName, false);
                        }
                    }
                }
            }
            // Same format as X509Identity
            str.Append(string.IsNullOrEmpty(value) ? "<x509>" : value);
            str.Append("; ");
            str.Append(certificate.Thumbprint);
        }
    }
}
