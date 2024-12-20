﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Web;
#else
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
#endif
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WcfTestCommon;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace WcfService
{
    [ServiceContract]
    public interface ITestHost
    {
        [OperationContract]
        [WebGet(UriTemplate = "ClientCert?asPem={asPem}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream ClientCert(bool asPem);

        [OperationContract]
        [WebGet(UriTemplate = "Crl", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Crl();

        [OperationContract]
        [WebGet(UriTemplate = "PeerCert?asPem={asPem}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream PeerCert(bool asPem);

        [OperationContract]
        [WebGet(UriTemplate = "MachineCert?asPem={asPem}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream MachineCert(bool asPem);

        [OperationContract]
        [WebGet(UriTemplate = "RootCert?asPem={asPem}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream RootCert(bool asPem);

        [OperationContract]
        [WebGet(UriTemplate = "Fqdn", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Fqdn();

        [OperationContract]
        [WebGet(UriTemplate = "Ping", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Ping();

        [OperationContract]
        [WebGet(UriTemplate = "State", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream State();

#if NET
        [OperationContract]
        [WebGet(UriTemplate = "Shutdown", BodyStyle = WebMessageBodyStyle.Bare)]
        void Shutdown();
#endif
    }

    public class TestHost : ITestHost
    {
        public Stream ClientCert(bool asPem)
        {
            X509Certificate2 clientCert = CertificateFromSubject(StoreName.My, StoreLocation.LocalMachine, "WCF Client Certificate");
            byte[] response;

            if (clientCert == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                response = Encoding.UTF8.GetBytes("Client certificate not found on system");
            }
            else
            {
                if (asPem)
                {
                    WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-client-cert.crt\"";
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pem-file";
                    response = Encoding.ASCII.GetBytes(GetCertificateAsPem(clientCert));
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-client-cert.pfx\"";
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs12";
                    response = clientCert.Export(X509ContentType.Pfx, "test");
                }
            }

            return new MemoryStream(response);
        }

        public Stream Crl()
        {
            // The test.crl is generated by the cert util tool and will not expire until the root cert expires.
            // We should investigate if we can generate it in the run time. This is not a blocking issue.
            string downloadFilePath = @"c:\\WCFTest\\test.crl";
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs7-crl";
            WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-crl.crl\"";

            return File.OpenRead(downloadFilePath);
        }

        public Stream RootCert(bool asPem)
        {
            X509Certificate2 rootCert = CertificateFromSubject(StoreName.Root, StoreLocation.LocalMachine, "DO_NOT_TRUST_WcfBridgeRootCA");
            byte[] response;

            if (rootCert == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                response = Encoding.UTF8.GetBytes("Root certificate not found on system");
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-root-cert.crt\"";

                if (asPem)
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pem-file";
                    response = Encoding.ASCII.GetBytes(GetCertificateAsPem(rootCert));
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-x509-ca-cert";
                    response = rootCert.RawData;
                }
            }

            return new MemoryStream(response);
        }

        public Stream Fqdn()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(Dns.GetHostEntry("127.0.0.1").HostName));
        }

        public Stream PeerCert(bool asPem)
        {
            X509Certificate2 peerCert = CertificateFromFriendlyName(StoreName.TrustedPeople, StoreLocation.LocalMachine, "WCF Bridge - UserPeerTrustCertificateResource");

            byte[] response;

            if (peerCert == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                response = Encoding.UTF8.GetBytes("Peer certificate not found on system");
            }
            else
            {
                if (asPem)
                {
                    WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-peer-cert.crt\"";
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pem-file";
                    response = Encoding.ASCII.GetBytes(GetCertificateAsPem(peerCert));
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-peer-cert.pfx\"";
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pkcs12";
                    response = peerCert.Export(X509ContentType.Pfx, "test");
                }
            }

            return new MemoryStream(response);
        }

        public Stream MachineCert(bool asPem)
        {
            X509Certificate2 machineCert = CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager");
            byte[] response;

            if (machineCert == null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                response = Encoding.UTF8.GetBytes("Machine certificate not found on system");
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = "attachment; filename=\"wcf-machine-cert.crt\"";

                if (asPem)
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-pem-file";
                    response = Encoding.ASCII.GetBytes(GetCertificateAsPem(machineCert));
                }
                else
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-x509-user-cert";
                    response = machineCert.RawData;
                }
            }

            return new MemoryStream(response);
        }

        public Stream Ping()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes("Service has started"));
        }

        public Stream State()
        {
            return new MemoryStream();
        }

        // All certificates have installed on the server machine, including client cert by the cert util tool.
        public static X509Certificate2 CertificateFromSubject(StoreName name, StoreLocation location, string subjectName)
        {
            X509Store store = null;

            try
            {
                store = CertificateHelper.GetX509Store(name, location);

                X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, validOnly: true);
                return foundCertificates.Count == 0 ? null : foundCertificates[0];
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }
        }

        public static X509Certificate2 CertificateFromFriendlyName(StoreName name, StoreLocation location, string friendlyName)
        {
            X509Store store = null;

            try
            {
                store = CertificateHelper.GetX509Store(name, location);

                X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByIssuerName, "DO_NOT_TRUST_WcfBridgeRootCA", false);
                string friendlyNameHash = CertificateGenerator.HashFriendlyNameToString(friendlyName);
                foreach (X509Certificate2 cert in foundCertificates)
                {
                    // Search by serial number in Linux/MacOS
                    if (cert.FriendlyName == friendlyName || cert.SerialNumber == friendlyNameHash)
                    {
                        return cert;
                    }
                }
                return null;
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
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

        public void Shutdown()
        {
            Environment.Exit(0);
        }
    }
}
