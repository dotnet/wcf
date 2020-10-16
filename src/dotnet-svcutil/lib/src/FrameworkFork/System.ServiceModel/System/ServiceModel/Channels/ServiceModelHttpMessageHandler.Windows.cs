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
#if TARGETS_WINDOWS && !FEATURE_NETNATIVE
    public partial class ServiceModelHttpMessageHandler
    {
        private WinHttpHandler _innerHandler;
        private bool _useProxy;

        public ServiceModelHttpMessageHandler()
        {
            _innerHandler = new WinHttpHandler();
            InnerHandler = _innerHandler;
        }

        public bool AllowAutoRedirect
        {
            get { return _innerHandler.AutomaticRedirection; }
            set { _innerHandler.AutomaticRedirection = value; }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get { return _innerHandler.ClientCertificateOption; }
            set { _innerHandler.ClientCertificateOption = value; }
        }

        public ICredentials Credentials
        {
            get { return _innerHandler.ServerCredentials; }
            set { _innerHandler.ServerCredentials = value; }
        }

        public bool UseCookies
        {
            get { return (_innerHandler.CookieUsePolicy == CookieUsePolicy.UseSpecifiedCookieContainer); }
            set
            {
                _innerHandler.CookieUsePolicy = value
                    ? CookieUsePolicy.UseSpecifiedCookieContainer
                    : CookieUsePolicy.IgnoreCookies;
            }
        }

        public bool UseDefaultCredentials
        {
            // WinHttpHandler doesn't have a separate UseDefaultCredentials property.  There 
            // is just a ServerCredentials property.  So, we need to map the behavior. 
            get { return (_innerHandler.ServerCredentials == CredentialCache.DefaultCredentials); }
            set
            {
                if (value)
                {
                    _innerHandler.ServerCredentials = CredentialCache.DefaultCredentials;
                }
                else if (_innerHandler.ServerCredentials == CredentialCache.DefaultCredentials)
                {
                    // Only clear out the ServerCredentials property if it was a DefaultCredentials. 
                    _innerHandler.ServerCredentials = null;
                }
            }
        }

        public bool UseProxy
        {
            get { return _useProxy; }
            set { _useProxy = value; }
        }

        public bool CheckCertificateRevocationList
        {
            get { return _innerHandler.CheckCertificateRevocationList; }
            set { _innerHandler.CheckCertificateRevocationList = value; }
        }

        public X509Certificate2Collection ClientCertificates
        {
            get { return _innerHandler.ClientCertificates; }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>
            ServerCertificateValidationCallback
        {
            get { return _innerHandler.ServerCertificateValidationCallback; }
            set { _innerHandler.ServerCertificateValidationCallback = value; }
        }

        public virtual bool SupportsProxy
        {
            get { return true; }
        }


        public virtual bool SupportsClientCertificates
        {
            get { return true; }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get current value of WindowsProxyUsePolicy.  Only call its WinHttpHandler 
            // property setter if the value needs to change. 
            var oldProxyUsePolicy = _innerHandler.WindowsProxyUsePolicy;
            if (_useProxy)
            {
                if (_innerHandler.Proxy == null)
                {
                    if (oldProxyUsePolicy != WindowsProxyUsePolicy.UseWinInetProxy)
                    {
                        _innerHandler.WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseWinInetProxy;
                    }
                }
                else
                {
                    if (oldProxyUsePolicy != WindowsProxyUsePolicy.UseCustomProxy)
                    {
                        _innerHandler.WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseCustomProxy;
                    }
                }
            }
            else
            {
                if (oldProxyUsePolicy != WindowsProxyUsePolicy.DoNotUseProxy)
                {
                    _innerHandler.WindowsProxyUsePolicy = WindowsProxyUsePolicy.DoNotUseProxy;
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
#endif
}
