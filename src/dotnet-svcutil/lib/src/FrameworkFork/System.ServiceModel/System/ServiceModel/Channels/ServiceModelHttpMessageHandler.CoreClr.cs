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
#if FEATURE_CORECLR && !TARGETS_WINDOWS
    public partial class ServiceModelHttpMessageHandler
    {
        private HttpClientHandler _innerHandler;

        public ServiceModelHttpMessageHandler()
        {
            _innerHandler = new HttpClientHandler();
            InnerHandler = _innerHandler;
        }

        public bool AllowAutoRedirect
        {
            get { return _innerHandler.AllowAutoRedirect; }
            set { _innerHandler.AllowAutoRedirect = value; }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get { return _innerHandler.ClientCertificateOptions; }
            set { _innerHandler.ClientCertificateOptions = value; }
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
            get { return _innerHandler.UseProxy; }
            set { _innerHandler.UseProxy = value; }
        }

        public bool CheckCertificateRevocationList
        {
            get { return false; }
            set { throw ExceptionHelper.PlatformNotSupported("CheckCertificateRevocationList not yet support"); }
        }

        public X509Certificate2Collection ClientCertificates
        {
            get { return null; }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>
            ServerCertificateValidationCallback
        {
            get { return null; }
            set { throw ExceptionHelper.PlatformNotSupported("Certificate validation not supported yet"); }
        }

        public virtual bool SupportsProxy
        {
            get { return _innerHandler.SupportsProxy; }
        }

        public virtual bool SupportsClientCertificates
        {
            get { return false; } 
        }
    }
#endif
}
