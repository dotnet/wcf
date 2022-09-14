// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Numerics;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal static class HttpTransportSecurityHelpers
    {
        private static Dictionary<string, int> s_targetNameCounter = new Dictionary<string, int>();

        public static void AddIdentityMapping(EndpointAddress target, Message message)
        {
            var hostHeader = GetIdentityHostHeader(target);
            HttpRequestMessageProperty requestProperty;
            if (!message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out requestProperty))
            {
                requestProperty = new HttpRequestMessageProperty();
                message.Properties.Add(HttpRequestMessageProperty.Name, requestProperty);
            }

            requestProperty.Headers[HttpRequestHeader.Host] = hostHeader;
        }

        public static string GetIdentityHostHeader(EndpointAddress target)
        {
            EndpointIdentity identity = target.Identity;
            string value;
            if (identity != null && !(identity is X509CertificateEndpointIdentity))
            {
                value = SecurityUtils.GetSpnFromIdentity(identity, target);
            }
            else
            {
                value = SecurityUtils.GetSpnFromTarget(target);
            }

            // HttpClientHandler supports specifying the SPN via the HOST header. The service name is hard coded to "HTTP/". "HTTP/"
            // is an alias for the "HOST/" service name so we accept either but can't accept anything else.
            if (!(value.StartsWith("host/", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http/", StringComparison.OrdinalIgnoreCase)))
            {
                throw Fx.Exception.AsError(new InvalidOperationException(SR.OnlyDefaultSpnServiceSupported));
            }

            // The leading service name has been constrained to be either "HTTP/" or "HOST/" which are both 5 charactes long.
            // This needs to be removed to provide just the hostname part for the Host header.
            return value.Substring(5);
        }

        public static void AddServerCertIdentityValidation(HttpClientHandler httpClientHandler, EndpointAddress to)
        {
            X509CertificateEndpointIdentity remoteCertificateIdentity = to.Identity as X509CertificateEndpointIdentity;
            if (remoteCertificateIdentity != null)
            {
                // The following condition should have been validated when the channel was created.
                Fx.Assert(remoteCertificateIdentity.Certificates.Count <= 1,
                    "HTTPS server certificate identity contains multiple certificates");
                var rawData = remoteCertificateIdentity.Certificates[0].GetRawCertData();
                var thumbprint = remoteCertificateIdentity.Certificates[0].Thumbprint;
                bool identityValidator(HttpRequestMessage requestMessage, X509Certificate2 cert, X509Chain chain, SslPolicyErrors policyErrors)
                {
                    try
                    {
                        ValidateServerCertificate(cert, rawData, thumbprint);
                    }
                    catch (SecurityNegotiationException e)
                    {
                        DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                        return false;
                    }

                    return (policyErrors == SslPolicyErrors.None);
                }

                SetServerCertificateValidationCallback(httpClientHandler, identityValidator);
            }
        }

        public static void SetServerCertificateValidationCallback(HttpClientHandler handler, Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> validator)
        {
            handler.ServerCertificateCustomValidationCallback =
                ChainValidator(handler.ServerCertificateCustomValidationCallback, validator);
        }

        private static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ChainValidator(
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> previousValidator,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> validator)
        {
            if (previousValidator == null)
            {
                return validator;
            }

            bool chained(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                bool valid = validator(request, certificate, chain, sslPolicyErrors);
                if (valid)
                {
                    return previousValidator(request, certificate, chain, sslPolicyErrors);
                }

                return false;
            }

            return chained;
        }

        private static void ValidateServerCertificate(X509Certificate2 certificate, byte[] rawData, string thumbprint)
        {
            byte[] certRawData = certificate.GetRawCertData();
            bool valid = true;
            if (rawData.Length != certRawData.Length)
            {
                valid = false;
            }
            else
            {
                int i = 0;
                while (true)
                {
                    if ((i + Vector<byte>.Count) > certRawData.Length)
                    {
                        // Not enough bytes left to use vector
                        for (; i < certRawData.Length; i++)
                        {
                            if (certRawData[i] != rawData[i])
                            {
                                valid = false;
                                break;
                            }
                        }

                        break;
                    }

                    Vector<byte> certDataVec = new Vector<byte>(certRawData, i);
                    Vector<byte> rawDataVec = new Vector<byte>(rawData, i);
                    if (!certDataVec.Equals(rawDataVec))
                    {
                        valid = false;
                        break;
                    }

                    i += Vector<byte>.Count;
                }
            }
            if (!valid)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new SecurityNegotiationException(SR.Format(SR.HttpsServerCertThumbprintMismatch,
                    certificate.Subject, certificate.Thumbprint, thumbprint)));
            }
        }
    }
}
