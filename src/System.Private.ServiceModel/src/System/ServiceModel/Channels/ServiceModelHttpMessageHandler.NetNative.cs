// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
#if FEATURE_NETNATIVE
    public partial class ServiceModelHttpMessageHandler
    {
        private RTHttpClientHandler _innerHandler;

        public ServiceModelHttpMessageHandler()
        {
            _innerHandler = new RTHttpClientHandler();
            InnerHandler = _innerHandler;
        }

        public ICredentials Credentials
        {
            get { return _innerHandler.Credentials; }
            set { _innerHandler.Credentials = value; }
        }

        public bool UseCookies
        {
            get { return _innerHandler.UseCookies; }
            set { _innerHandler.UseCookies = value; }
        }

        public bool UseDefaultCredentials
        {
            get { return _innerHandler.UseDefaultCredentials; }
            set { _innerHandler.UseDefaultCredentials = value; }
        }

        public bool UseProxy
        {
            get { return true; }
            set { /* RTHttpClient can only use the system defined proxy so this is a no-op */ }
        }

        public bool CheckCertificateRevocationList
        {
            get { return false; }
            set { throw ExceptionHelper.PlatformNotSupported("CheckCertificateRevocationList not yet support"); }
        }

        public X509Certificate2Collection ClientCertificates
        {
            get { return _innerHandler.ClientCertificates; }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>
            ServerCertificateValidationCallback
        {
            get { return null; }
            set { throw ExceptionHelper.PlatformNotSupported("Certificate validation not supported yet"); }
        }

        public bool SupportsProxy
        {
            get { return false; /* Only uses wininet configured proxy */ }
        }

        public bool SupportsClientCertificates
        {
            get { return true; } 
        }

        public bool SupportsCertificateValidationCallback
        {
            get { return false; }
        }
    }
#endif
}
