// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.WebSockets;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ClientWebSocketTransportDuplexSessionChannel : WebSocketTransportDuplexSessionChannel
    {
        private HttpChannelFactory<IDuplexSessionChannel> _channelFactory;
        private SecurityTokenProviderContainer _webRequestTokenProvider;
        private SecurityTokenProviderContainer _webRequestProxyTokenProvider;
        private volatile bool _cleanupStarted;

        public ClientWebSocketTransportDuplexSessionChannel(HttpChannelFactory<IDuplexSessionChannel> channelFactory, EndpointAddress remoteAddress, Uri via)
            : base(channelFactory, remoteAddress, via)
        {
            Contract.Assert(channelFactory != null, "connection factory must be set");
            _channelFactory = channelFactory;
        }

        protected override bool IsStreamedOutput
        {
            get { return TransferModeHelper.IsRequestStreamed(TransferMode); }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginOpen(this, timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            CommunicationObjectInternal.OnOpen(this, timeout);
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            bool disposeInvoker = false;
            (HttpMessageInvoker invoker, disposeInvoker) = await SetupInvoker(helper.RemainingTime());
            HttpResponseMessage response = null;
            bool disposeResponse = false;
            bool success = false;
            try
            {
                if (WcfEventSource.Instance.WebSocketConnectionRequestSendStartIsEnabled())
                {
                    WcfEventSource.Instance.WebSocketConnectionRequestSendStart(
                        EventTraceActivity,
                        RemoteAddress != null ? RemoteAddress.ToString() : string.Empty);
                }

                try
                {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Via) { Version = HttpVersion.Version11 };

                                // These headers were added for WCF specific handshake to avoid encoder or transfermode mismatch between client and server.
                                // For BinaryMessageEncoder, since we are using a sessionful channel for websocket, the encoder is actually different when
                                // we are using Buffered or Stramed transfermode. So we need an extra header to identify the transfermode we are using, just
                                // to make people a little bit easier to diagnose these mismatch issues.
                                if (_channelFactory.MessageVersion != MessageVersion.None)
                                {
                                    request.Headers.TryAddWithoutValidation(WebSocketTransportSettings.SoapContentTypeHeader, _channelFactory.WebSocketSoapContentType);

                                    if (_channelFactory.MessageEncoderFactory is BinaryMessageEncoderFactory)
                                    {
                                        request.Headers.TryAddWithoutValidation(WebSocketTransportSettings.BinaryEncoderTransferModeHeader, _channelFactory.TransferMode.ToString());
                                    }
                                }

                                string secValue = AddWebSocketHeaders(request);

                                Task<HttpResponseMessage> sendTask = invoker is HttpClient client
                                    ? client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                    : invoker.SendAsync(request, CancellationToken.None);
                                response = await sendTask.ConfigureAwait(false);

                                ValidateResponse(response, secValue);
                                break;
                            }
                            catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.ExtendedConnectNotSupported || ex.Data.Contains("HTTP2_ENABLED"))
                            {
                            }
                        }

                        // The SecWebSocketProtocol header is optional.  We should only get it with a non-empty value if we requested subprotocols,
                        // and then it must only be one of the ones we requested.  If we got a subprotocol other than one we requested (or if we
                        // already got one in a previous header), fail. Otherwise, track which one we got.
                        string subprotocol = null;
                        if (response.Headers.TryGetValues(HttpKnownHeaderNames.SecWebSocketProtocol, out IEnumerable<string> subprotocolEnumerableValues))
                        {
                            Debug.Assert(subprotocolEnumerableValues is string[]);
                            string[] subprotocolArray = (string[])subprotocolEnumerableValues;
                            if (subprotocolArray.Length > 0 && !string.IsNullOrEmpty(subprotocolArray[0]))
                            {
                                if (WebSocketSettings.SubProtocol is not null)
                                {
                                    if (WebSocketSettings.SubProtocol.Equals(subprotocolArray[0], StringComparison.OrdinalIgnoreCase))
                                    {
                                        subprotocol = WebSocketSettings.SubProtocol;
                                    }
                                }

                                if (subprotocol == null)
                                {
                                    throw new WebSocketException(
                                    WebSocketError.UnsupportedProtocol,
                                        SR.Format(SR.net_WebSockets_AcceptUnsupportedProtocol, WebSocketSettings.SubProtocol, string.Join(", ", subprotocolArray)));
                                }
                            }
                        }

                        // Get the response stream and wrap it in a web socket.
                        Stream connectedStream = response.Content.ReadAsStream();
                        Debug.Assert(connectedStream.CanWrite);
                        Debug.Assert(connectedStream.CanRead);
                        WebSocket = WebSocket.CreateFromStream(connectedStream, new WebSocketCreationOptions
                        {
                            IsServer = false,
                            SubProtocol = subprotocol,
                            KeepAliveInterval = this.WebSocketSettings.KeepAliveInterval,
                            //???KeepAliveTimeout = options.KeepAliveTimeout 
                            //???DangerousDeflateOptions = negotiatedDeflateOptions
                        });
                    }
                    catch (Exception exc)
                    {
                        Abort();
                        disposeResponse = true;

                        if (exc is WebSocketException || exc is OperationCanceledException)
                        {
                            throw;
                        }

                        throw new WebSocketException(WebSocketError.Faulted, SR.net_webstatus_ConnectFailure, exc);
                    }
                    finally
                    {
                        if (response is not null)
                        {
                            if (disposeResponse)
                            {
                                response.Dispose();
                            }
                        }

                        // Disposing the invoker will not affect any active stream wrapped in the WebSocket.
                        if (disposeInvoker)
                        {
                            invoker?.Dispose();
                        }
                    }
                }
                finally
                {
                    if (WebSocket != null && _cleanupStarted)
                    {
                        WebSocket.Abort();
                        CommunicationObjectAbortedException communicationObjectAbortedException = new CommunicationObjectAbortedException(
                            new WebSocketException(WebSocketError.ConnectionClosedPrematurely).Message);
                        FxTrace.Exception.AsWarning(communicationObjectAbortedException);
                        throw communicationObjectAbortedException;
                    }
                }

                bool inputUseStreaming = TransferModeHelper.IsResponseStreamed(TransferMode);

                SetMessageSource(new WebSocketMessageSource(
                    this,
                    WebSocket,
                    inputUseStreaming,
                    this));

                success = true;

                if (WcfEventSource.Instance.WebSocketConnectionRequestSendStopIsEnabled())
                {
                    WcfEventSource.Instance.WebSocketConnectionRequestSendStop(
                        EventTraceActivity,
                        WebSocket != null ? WebSocket.GetHashCode() : -1);
                }
            }
            catch (WebSocketException ex)
            {
                if (WcfEventSource.Instance.WebSocketConnectionFailedIsEnabled())
                {
                    WcfEventSource.Instance.WebSocketConnectionFailed(EventTraceActivity, ex.Message);
                }

                TryConvertAndThrow(ex);
            }
            finally
            {
                CleanupTokenProviders();
                if (!success)
                {
                    CleanupOnError();
                }
            }
        }

        private void ValidateResponse(HttpResponseMessage response, string secValue)
        {
            Debug.Assert(response.Version == HttpVersion.Version11 || response.Version == HttpVersion.Version20);

            if (response.Version == HttpVersion.Version11)
            {
                if (response.StatusCode != HttpStatusCode.SwitchingProtocols)
                {
                    throw new WebSocketException(WebSocketError.NotAWebSocket, SR.Format(SR.net_WebSockets_ConnectStatusExpected, (int)response.StatusCode, (int)HttpStatusCode.SwitchingProtocols));
                }

                Debug.Assert(secValue != null);

                // The Connection, Upgrade, and SecWebSocketAccept headers are required and with specific values.
                ValidateHeader(response.Headers, HttpKnownHeaderNames.Connection, "Upgrade");
                ValidateHeader(response.Headers, HttpKnownHeaderNames.Upgrade, "websocket");
                ValidateHeader(response.Headers, HttpKnownHeaderNames.SecWebSocketAccept, secValue);
            }

            if (response.Content is null)
            {
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
            }
        }

        private static void ValidateHeader(HttpHeaders headers, string name, string expectedValue)
        {
            if (headers.NonValidated.TryGetValues(name, out HeaderStringValues hsv))
            {
                if (hsv.Count == 1)
                {
                    foreach (string value in hsv)
                    {
                        if (string.Equals(value, expectedValue, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                        break;
                    }
                }

                throw new WebSocketException(WebSocketError.HeaderError, SR.Format(SR.net_WebSockets_InvalidResponseHeader, name, hsv));
            }

            throw new WebSocketException(WebSocketError.Faulted, SR.Format(SR.net_WebSockets_MissingResponseHeader, name));
        }

        private async Task<(HttpMessageInvoker invoker, bool disposeInvoker)> SetupInvoker(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            ChannelParameterCollection channelParameterCollection = new ChannelParameterCollection();
            bool disposeInvoker = true;
            var handler = new SocketsHttpHandler();
            handler.PooledConnectionLifetime = TimeSpan.Zero;
            if (_channelFactory.AllowCookies)
            {
                var cookieContainerManager = _channelFactory.GetHttpCookieContainerManager();
                handler.CookieContainer = cookieContainerManager.CookieContainer;
                handler.UseCookies = cookieContainerManager.CookieContainer != null;
            }

            //configure handler.SslOptions
            SecurityTokenContainer clientCertificateToken = null;
            if (_channelFactory is HttpsChannelFactory<IDuplexSessionChannel> httpsChannelFactory)
            {
                if (httpsChannelFactory.RequireClientCertificate)
                {
                    SecurityTokenProvider certificateProvider = await httpsChannelFactory.CreateAndOpenCertificateTokenProviderAsync(RemoteAddress, Via, channelParameterCollection, helper.RemainingTime());
                    clientCertificateToken = await httpsChannelFactory.GetCertificateSecurityTokenAsync(certificateProvider, RemoteAddress, Via, channelParameterCollection, helper);
                    if (clientCertificateToken != null)
                    {
                        X509SecurityToken x509Token = (X509SecurityToken)clientCertificateToken.Token;
                        Debug.Assert(handler.SslOptions.ClientCertificates == null);
                        handler.SslOptions.ClientCertificates = new X509Certificate2Collection
                        {
                            x509Token.Certificate
                        };
                    }
                }
                //Fix for issue #5729: Removed the httpsChannelFactory.RequireClientCertificate condition from the following if statement.
                if (httpsChannelFactory.WebSocketCertificateCallback != null)
                {
                    handler.SslOptions.RemoteCertificateValidationCallback = httpsChannelFactory.WebSocketCertificateCallback;
                }
            }

            //configure handler.Proxy
            (NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel) =
                await HttpChannelUtilities.GetCredentialAsync(_channelFactory.AuthenticationScheme, _webRequestTokenProvider, timeout);
            if (_channelFactory.Proxy != null)
            {
                handler.Proxy = _channelFactory.Proxy;
            }
            else if (_channelFactory.ProxyFactory != null)
            {
                handler.Proxy = await _channelFactory.ProxyFactory.CreateWebProxyAsync(
                    authenticationLevel,
                    impersonationLevel,
                    _webRequestProxyTokenProvider,
                    helper.RemainingTime());
            }
            else
            {
                handler.UseProxy = false;
            }

            //configure handler.Credentials
            if (credential == CredentialCache.DefaultCredentials || credential == null)
            {
                if (_channelFactory.AuthenticationScheme != AuthenticationSchemes.Anonymous)
                {
                    handler.Credentials = CredentialCache.DefaultCredentials;
                }
            }
            else
            {
                CredentialCache credentials = new CredentialCache();
                Uri credentialCacheUriPrefix = _channelFactory.GetCredentialCacheUriPrefix(Via);
                if (_channelFactory.AuthenticationScheme == AuthenticationSchemes.IntegratedWindowsAuthentication)
                {
                    credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Negotiate),
                        credential);
                    credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(AuthenticationSchemes.Ntlm),
                        credential);
                }
                else
                {
                    credentials.Add(credentialCacheUriPrefix, AuthenticationSchemesHelper.ToString(_channelFactory.AuthenticationScheme),
                        credential);
                }

                handler.Credentials = credentials;
            }

            return (new HttpMessageInvoker(handler), disposeInvoker);
        }

        private string AddWebSocketHeaders(HttpRequestMessage request)
        {
            // always exact because we handle downgrade here
            request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
            string secValue = null;

            if (request.Version == HttpVersion.Version11)
            {
                // Create the security key and expected response, then build all of the request headers
                KeyValuePair<string, string> secKeyAndSecWebSocketAccept = CreateSecKeyAndSecWebSocketAccept();
                secValue = secKeyAndSecWebSocketAccept.Value;
                request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.Connection, HttpKnownHeaderNames.Upgrade);
                request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.Upgrade, "websocket");
                request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.SecWebSocketKey, secKeyAndSecWebSocketAccept.Key);
            }
            else if (request.Version == HttpVersion.Version20)
            {
                request.Headers.Protocol = "websocket";
            }

            request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.SecWebSocketVersion, "13");

            if (WebSocketSettings.SubProtocol != null)
            {
                request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.SecWebSocketProtocol, WebSocketSettings.SubProtocol);
            }

            return secValue;
        }

        /// <summary>
        /// Creates a pair of a security key for sending in the Sec-WebSocket-Key header and
        /// the associated response we expect to receive as the Sec-WebSocket-Accept header value.
        /// </summary>
        /// <returns>A key-value pair of the request header security key and expected response header value.</returns>
        [SuppressMessage("Microsoft.Security", "CA5350", Justification = "Required by RFC6455")]
        private static KeyValuePair<string, string> CreateSecKeyAndSecWebSocketAccept()
        {
            // GUID appended by the server as part of the security key response.  Defined in the RFC.
            ReadOnlySpan<byte> wsServerGuidBytes = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"u8;

            Span<byte> bytes = stackalloc byte[24 /* Base64 guid length */ + wsServerGuidBytes.Length];

            // Base64-encode a new Guid's bytes to get the security key
            bool success = Guid.NewGuid().TryWriteBytes(bytes);
            Debug.Assert(success);
            string secKey = Convert.ToBase64String(bytes.Slice(0, 16 /*sizeof(Guid)*/));

            // Get the corresponding ASCII bytes for seckey+wsServerGuidBytes
            int encodedSecKeyLength = Encoding.ASCII.GetBytes(secKey, bytes);
            wsServerGuidBytes.CopyTo(bytes.Slice(encodedSecKeyLength));

            // Hash the seckey+wsServerGuidBytes bytes
            System.Security.Cryptography.SHA1.TryHashData(bytes, bytes, out int bytesWritten);
            Debug.Assert(bytesWritten == 20 /* SHA1 hash length */);

            // Return the security key + the base64 encoded hashed bytes
            return new KeyValuePair<string, string>(
                secKey,
                Convert.ToBase64String(bytes.Slice(0, bytesWritten)));
        }

        protected override void OnCleanup()
        {
            _cleanupStarted = true;
            base.OnCleanup();
        }

        private static void TryConvertAndThrow(WebSocketException ex)
        {
            switch (ex.WebSocketErrorCode)
            {
                //case WebSocketError.Success:
                //case WebSocketError.InvalidMessageType:
                //case WebSocketError.Faulted:
                //case WebSocketError.NativeError:
                //case WebSocketError.NotAWebSocket:
                case WebSocketError.UnsupportedVersion:
                    throw FxTrace.Exception.AsError(new CommunicationException(SR.Format(SR.WebSocketVersionMismatchFromServer, ""), ex));
                case WebSocketError.UnsupportedProtocol:
                    throw FxTrace.Exception.AsError(new CommunicationException(SR.Format(SR.WebSocketSubProtocolMismatchFromServer, ""), ex));
                //case WebSocketError.HeaderError:
                //case WebSocketError.ConnectionClosedPrematurely:
                //case WebSocketError.InvalidState:
                default:
                    throw FxTrace.Exception.AsError(new CommunicationException(ex.Message, ex));
            }
        }

        private void CleanupOnError()
        {
            Cleanup();
        }

        private void CleanupTokenProviders()
        {
            if (_webRequestTokenProvider != null)
            {
                _webRequestTokenProvider.Abort();
                _webRequestTokenProvider = null;
            }

            if (_webRequestProxyTokenProvider != null)
            {
                _webRequestProxyTokenProvider.Abort();
                _webRequestProxyTokenProvider = null;
            }
        }
    }
}
