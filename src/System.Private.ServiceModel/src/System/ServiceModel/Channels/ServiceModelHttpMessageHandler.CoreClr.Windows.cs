// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.ServiceModel.Channels
{
#if FEATURE_CORECLR && TARGETS_WINDOWS
    public partial class ServiceModelHttpMessageHandler
    {
        private readonly WinHttpHandler _innerHandler;

        public ServiceModelHttpMessageHandler()
        {
            _innerHandler = new WinHttpHandler();
            // WCF doesn't care about the granular WinHttpHandler timeout properties and the default value is 30 seconds
            // so we need to set them to infinite and allow the HttpClient.Timeout property to have precedence.
            _innerHandler.ReceiveHeadersTimeout = Timeout.InfiniteTimeSpan;
            _innerHandler.ReceiveDataTimeout = Timeout.InfiniteTimeSpan;
            _innerHandler.SendTimeout = Timeout.InfiniteTimeSpan;

            InnerHandler = _innerHandler;
        }

        public ICredentials Credentials
        {
            get { return _innerHandler.ServerCredentials; }
            set { _innerHandler.ServerCredentials = value; }
        }

        public bool UseCookies
        {
            get { return (_innerHandler.CookieUsePolicy == CookieUsePolicy.UseSpecifiedCookieContainer); }
            set { _innerHandler.CookieUsePolicy = value ? CookieUsePolicy.UseSpecifiedCookieContainer : CookieUsePolicy.IgnoreCookies; }
        }

        public bool UseDefaultCredentials
        {
            // WinHttpHandler doesn't have a separate UseDefaultCredentials property.  There
            // is just a ServerCredentials property.  So, we need to map the behavior.
            //
            // This property only affect .ServerCredentials and not .DefaultProxyCredentials.

            get { return (_innerHandler.ServerCredentials == CredentialCache.DefaultCredentials); }

            set
            {
                if (value)
                {
                    _innerHandler.ServerCredentials = CredentialCache.DefaultCredentials;
                }
                else
                {
                    if (_innerHandler.ServerCredentials == CredentialCache.DefaultCredentials)
                    {
                        // Only clear out the ServerCredentials property if it was a DefaultCredentials.
                        _innerHandler.ServerCredentials = null;
                    }
                }
            }
        }

        public bool UseProxy { get; set; }

        public bool CheckCertificateRevocationList
        {
            get { return false; }
            set { throw ExceptionHelper.PlatformNotSupported("CheckCertificateRevocationList not yet support"); }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { return _innerHandler.ClientCertificates; }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>
            ServerCertificateValidationCallback
        {
            get { return _innerHandler.ServerCertificateValidationCallback; }
            set { _innerHandler.ServerCertificateValidationCallback = value; }
        }

        public bool SupportsProxy => true;

        public IWebProxy Proxy
        {
            get { return _innerHandler.Proxy; }
            set { _innerHandler.Proxy = value; }
        }

        public bool SupportsClientCertificates => true;

        public bool SupportsCertificateValidationCallback => true;
    }
#endif
}
