// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if FEATURE_NETNATIVE
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

using RTApiInformation = Windows.Foundation.Metadata.ApiInformation;
using RTHttpBaseProtocolFilter = Windows.Web.Http.Filters.HttpBaseProtocolFilter;
using RTHttpCacheReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior;
using RTHttpCacheWriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior;
using RTHttpCookieUsageBehavior = Windows.Web.Http.Filters.HttpCookieUsageBehavior;
using RTPasswordCredential = Windows.Security.Credentials.PasswordCredential;

namespace System.ServiceModel.Channels
{
    internal class RTHttpClientHandler : DelegatingHandler
    {
        private static readonly Lazy<bool> s_RTCookieUsageBehaviorSupported =
            new Lazy<bool>(InitRTCookieUsageBehaviorSupported);

        //#region Fields

        private readonly RTHttpBaseProtocolFilter _rtFilter;
        private readonly RTHttpHandlerToFilter _handlerToFilter;

        private volatile bool _operationStarted;
        private volatile bool _disposed;

        private CookieContainer _cookieContainer;
        private bool _useCookies;
        private DecompressionMethods _automaticDecompression;
        private X509Certificate2Collection _clientCertificates;


        //#endregion Fields

        //#region Properties

        public bool UseCookies
        {
            get { return _useCookies; }
            set
            {
                CheckDisposedOrStarted();
                _useCookies = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!UseCookies)
                {
                    throw new InvalidOperationException("net_http_invalid_enable_first, UseCookies, true");
                }
                CheckDisposedOrStarted();
                _cookieContainer = value;
            }
        }

        public X509Certificate2Collection ClientCertificates
        {
            get { return _clientCertificates; }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get { return _automaticDecompression; }
            set
            {
                CheckDisposedOrStarted();

                // Automatic decompression is implemented downstack.
                // HBPF will decompress both gzip and deflate, we will set
                // accept-encoding for one, the other, or both passed in here.
                _rtFilter.AutomaticDecompression = (value != DecompressionMethods.None);
                _automaticDecompression = value;
            }
        }

        public bool PreAuthenticate
        {
            get { return true; }
            set
            {
                if (value != PreAuthenticate)
                {
                    throw new PlatformNotSupportedException(String.Format(CultureInfo.InvariantCulture,
                        "net_http_value_not_supported, {0}, PreAuthenticate", value));
                }
                CheckDisposedOrStarted();
            }
        }

        public bool UseDefaultCredentials
        {
            get { return Credentials == null; }
            set
            {
                CheckDisposedOrStarted();
                if (value)
                {
                    // System managed
                    _rtFilter.ServerCredential = null;
                }
                else if (_rtFilter.ServerCredential == null)
                {
                    // The only way to disable default credentials is to provide credentials.
                    // Do not overwrite credentials if they were already assigned.
                    _rtFilter.ServerCredential = new RTPasswordCredential();
                }
            }
        }

        public ICredentials Credentials
        {
            get
            {
                RTPasswordCredential rtCreds = _rtFilter.ServerCredential;
                if (rtCreds == null)
                {
                    return null;
                }

                NetworkCredential creds = new NetworkCredential(rtCreds.UserName, rtCreds.Password);
                return creds;
            }
            set
            {
                if (value == null)
                {
                    CheckDisposedOrStarted();
                    _rtFilter.ServerCredential = null;
                }
                else if (value == CredentialCache.DefaultCredentials)
                {
                    CheckDisposedOrStarted();
                    // System managed
                    _rtFilter.ServerCredential = null;
                }
                else if (value is NetworkCredential)
                {
                    CheckDisposedOrStarted();
                    _rtFilter.ServerCredential = RTPasswordCredentialFromNetworkCredential((NetworkCredential)value);
                }
                else
                {
                    throw new PlatformNotSupportedException(string.Format(CultureInfo.InvariantCulture,
                        "net_http_value_not_supported, {0}, Credentials", value));
                }
            }
        }

        private bool RTCookieUsageBehaviorSupported
        {
            get
            {
                return s_RTCookieUsageBehaviorSupported.Value;
            }
        }

        public RTHttpClientHandler()
        {
            _rtFilter = new RTHttpBaseProtocolFilter();
            _handlerToFilter = new RTHttpHandlerToFilter(_rtFilter);
            InnerHandler = _handlerToFilter;

            // TODO: Fix up client certificate options
            _clientCertificates = new X509Certificate2Collection();

            InitRTCookieUsageBehavior();

            _useCookies = true; // deal with cookies by default.
            _cookieContainer = new CookieContainer(); // default container used for dealing with auto-cookies.

            // Managed at this layer for granularity, but uses the desktop default.
            _rtFilter.AutomaticDecompression = false;
            _automaticDecompression = DecompressionMethods.None;

            // We don't support using the UI model in HttpBaseProtocolFilter() especially for auto-handling 401 responses.
            _rtFilter.AllowUI = false;

            // The .NET Desktop System.Net Http APIs (based on HttpWebRequest/HttpClient) uses no caching by default.
            // To preserve app-compat, we turn off caching (as much as possible) in the WinRT HttpClient APIs.
            _rtFilter.CacheControl.ReadBehavior = RTHttpCacheReadBehavior.MostRecent;
            _rtFilter.CacheControl.WriteBehavior = RTHttpCacheWriteBehavior.NoCache;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                try
                {
                    _rtFilter.Dispose();
                }
                catch (InvalidComObjectException)
                {
                    // We'll ignore this error since it can happen when Dispose() is called from an object's finalizer
                    // and the WinRT object (rtFilter) has already been disposed by the .NET Native runtime.
                }
            }

            base.Dispose(disposing);
        }

        private void ConfigureRequest(HttpRequestMessage request)
        {
            ApplyRequestCookies(request);

            ApplyDecompressionSettings(request);

            ApplyClientCertificateSettings();
        }

        // Taken from System.Net.CookieModule.OnSendingHeaders
        private void ApplyRequestCookies(HttpRequestMessage request)
        {
            if (UseCookies)
            {
                string cookieHeader = CookieContainer.GetCookieHeader(request.RequestUri);
                if (!string.IsNullOrWhiteSpace(cookieHeader))
                {
                    bool success = request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.Cookie, cookieHeader);
                    Debug.Assert(success);
                }
            }
        }

        private void ApplyDecompressionSettings(HttpRequestMessage request)
        {
            // Decompression: Add the Gzip and Deflate headers if not already present.
            ApplyDecompressionSetting(request, DecompressionMethods.GZip, "gzip");
            ApplyDecompressionSetting(request, DecompressionMethods.Deflate, "deflate");
        }

        private void ApplyDecompressionSetting(HttpRequestMessage request, DecompressionMethods method, string methodName)
        {
            if ((AutomaticDecompression & method) == method)
            {
                bool found = false;
                foreach (StringWithQualityHeaderValue encoding in request.Headers.AcceptEncoding)
                {
                    if (methodName.Equals(encoding.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(methodName));
                }
            }
        }

        private void ApplyClientCertificateSettings()
        {
            if (ClientCertificates.Count > 0)
            {
                bool foundCertificate = false;
                var firstCertificate = ClientCertificates[0];
                foreach (var extension in firstCertificate.Extensions)
                {
                    var attachmentExtension = extension as X509CertificateInitiatorClientCredential.X509UwpCertificateAttachmentExtension;
                    if (attachmentExtension != null && attachmentExtension.AttachedCertificate != null)
                    {
                        _rtFilter.ClientCertificate = attachmentExtension.AttachedCertificate;
                        foundCertificate = true;
                        break;
                    }
                }
                Contract.Assert(foundCertificate, "We shouldn't be able to have an X509Certificate2 which doesn't have an attached UWP Certificate");
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CheckDisposed();
            SetOperationStarted();

            HttpResponseMessage response;
            try
            {
                ConfigureRequest(request);

                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Convert back to the expected exception type
                throw new HttpRequestException("net_http_client_execution_error", ex);
            }

            ProcessResponse(response);
            return response;
        }

        private void ProcessResponse(HttpResponseMessage response)
        {
            ProcessResponseCookies(response);
        }

        // Taken from System.Net.CookieModule.OnReceivedHeaders
        private void ProcessResponseCookies(HttpResponseMessage response)
        {
            if (UseCookies)
            {
                IEnumerable<string> values;
                if (response.Headers.TryGetValues(HttpKnownHeaderNames.SetCookie, out values))
                {
                    foreach (string cookieString in values)
                    {
                        if (!string.IsNullOrWhiteSpace(cookieString))
                        {
                            try
                            {
                                // Parse the cookies so that we can filter some of them out
                                CookieContainer helper = new CookieContainer();
                                helper.SetCookies(response.RequestMessage.RequestUri, cookieString);
                                CookieCollection cookies = helper.GetCookies(response.RequestMessage.RequestUri);
                                foreach (Cookie cookie in cookies)
                                {
                                    // We don't want to put HttpOnly cookies in the CookieContainer if the system
                                    // doesn't support the RTHttpBaseProtocolFilter CookieUsageBehavior property.
                                    // Prior to supporting that, the WinRT HttpClient could not turn off cookie
                                    // processing. So, it would always be storing all cookies in its internal container.
                                    // Putting HttpOnly cookies in the .NET CookieContainer would cause problems later
                                    // when the .NET layer tried to add them on outgoing requests and conflicted with
                                    // the WinRT nternal cookie processing.
                                    //
                                    // With support for WinRT CookieUsageBehavior, cookie processing is turned off
                                    // within the WinRT layer. This allows us to process cookies using only the .NET
                                    // layer. So, we need to add all applicable cookies that are received to the
                                    // CookieContainer.
                                    if (RTCookieUsageBehaviorSupported || !cookie.HttpOnly)
                                    {
                                        CookieContainer.Add(response.RequestMessage.RequestUri, cookie);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        private void SetOperationStarted()
        {
            if (!_operationStarted)
            {
                _operationStarted = true;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_operationStarted)
            {
                throw new InvalidOperationException("net_http_operation_started");
            }
        }

        private RTPasswordCredential RTPasswordCredentialFromNetworkCredential(NetworkCredential creds)
        {
            // RTPasswordCredential doesn't allow assigning string.Empty values, but those are the default values.
            RTPasswordCredential rtCreds = new RTPasswordCredential();
            if (!string.IsNullOrEmpty(creds.UserName))
            {
                if (!string.IsNullOrEmpty(creds.Domain))
                {
                    rtCreds.UserName = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", creds.Domain, creds.UserName);
                }
                else
                {
                    rtCreds.UserName = creds.UserName;
                }
            }

            if (!string.IsNullOrEmpty(creds.Password))
            {
                rtCreds.Password = creds.Password;
            }

            return rtCreds;
        }

        private static bool InitRTCookieUsageBehaviorSupported()
        {
            return RTApiInformation.IsPropertyPresent(
                "Windows.Web.Http.Filters.HttpBaseProtocolFilter",
                "CookieUsageBehavior");
        }

        //// Regardless of whether we're running on a machine that supports this WinRT API, we still might not be able
        //// to call the API. This is due to the calling app being compiled against an older Windows 10 Tools SDK. Since
        //// this library was compiled against the newer SDK, having these new API calls in this class will cause JIT
        //// failures in CoreCLR which generate a MissingMethodException before the code actually runs. So, we need
        //// these helper methods and try/catch handling.

        private void InitRTCookieUsageBehavior()
        {
            try
            {
                InitRTCookieUsageBehaviorHelper();
            }
            catch (MissingMethodException)
            {
                Debug.WriteLine("HttpClientHandler.InitRTCookieUsageBehavior: MissingMethodException");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitRTCookieUsageBehaviorHelper()
        {
            // Always turn off WinRT cookie processing if the WinRT API supports turning it off.
            // Use .NET CookieContainer handling only.
            if (RTCookieUsageBehaviorSupported)
            {
                _rtFilter.CookieUsageBehavior = RTHttpCookieUsageBehavior.NoCookies;
            }
        }

        internal static class HttpKnownHeaderNames
        {
            public const string Cookie = "Cookie";
            public const string SetCookie = "Set-Cookie";
        }

    }
}
#endif //FEATURE_NETNATIVE
