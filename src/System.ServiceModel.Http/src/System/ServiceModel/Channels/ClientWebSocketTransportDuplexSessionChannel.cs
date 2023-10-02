// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.WebSockets;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Security.Tokens;
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
                    var clientWebSocket = new ClientWebSocket();
                    await ConfigureClientWebSocketAsync(clientWebSocket, helper.RemainingTime());
                    await clientWebSocket.ConnectAsync(Via, await helper.GetCancellationTokenAsync());
                    ValidateWebSocketConnection(clientWebSocket);
                    WebSocket = clientWebSocket;
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

        private void ValidateWebSocketConnection(ClientWebSocket clientWebSocket)
        {
            string requested = WebSocketSettings.SubProtocol;
            string obtained = clientWebSocket.SubProtocol;
            if (!(requested == null ? string.IsNullOrWhiteSpace(obtained) : requested.Equals(obtained, StringComparison.OrdinalIgnoreCase)))
            {
                clientWebSocket.Dispose();
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(SR.WebSocketInvalidProtocolNotInClientList, obtained, requested)));
            }
        }

        private async Task ConfigureClientWebSocketAsync(ClientWebSocket clientWebSocket, TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            ChannelParameterCollection channelParameterCollection = new ChannelParameterCollection();
            if (HttpChannelFactory<IDuplexSessionChannel>.MapIdentity(RemoteAddress, _channelFactory.AuthenticationScheme))
            {
                clientWebSocket.Options.SetRequestHeader("Host", HttpTransportSecurityHelpers.GetIdentityHostHeader(RemoteAddress));
            }

            (_webRequestTokenProvider, _webRequestProxyTokenProvider) =
                await _channelFactory.CreateAndOpenTokenProvidersAsync(
                    RemoteAddress,
                    Via,
                    channelParameterCollection,
                    helper.RemainingTime());

            SecurityTokenContainer clientCertificateToken = null;
            if (_channelFactory is HttpsChannelFactory<IDuplexSessionChannel> httpsChannelFactory && httpsChannelFactory.RequireClientCertificate)
            {
                SecurityTokenProvider certificateProvider = await httpsChannelFactory.CreateAndOpenCertificateTokenProviderAsync(RemoteAddress, Via, channelParameterCollection, helper.RemainingTime());
                clientCertificateToken = await httpsChannelFactory.GetCertificateSecurityTokenAsync(certificateProvider, RemoteAddress, Via, channelParameterCollection, helper);
                if (clientCertificateToken != null)
                {
                    X509SecurityToken x509Token = (X509SecurityToken)clientCertificateToken.Token;
                    clientWebSocket.Options.ClientCertificates.Add(x509Token.Certificate);
                }

                if (httpsChannelFactory.WebSocketCertificateCallback != null)
                {
                    clientWebSocket.Options.RemoteCertificateValidationCallback = httpsChannelFactory.WebSocketCertificateCallback;
                }
            }

            if (WebSocketSettings.SubProtocol != null)
            {
                clientWebSocket.Options.AddSubProtocol(WebSocketSettings.SubProtocol);
            }

            // These headers were added for WCF specific handshake to avoid encoder or transfermode mismatch between client and server.
            // For BinaryMessageEncoder, since we are using a sessionful channel for websocket, the encoder is actually different when
            // we are using Buffered or Stramed transfermode. So we need an extra header to identify the transfermode we are using, just
            // to make people a little bit easier to diagnose these mismatch issues.
            if (_channelFactory.MessageVersion != MessageVersion.None)
            {
                clientWebSocket.Options.SetRequestHeader(WebSocketTransportSettings.SoapContentTypeHeader, _channelFactory.WebSocketSoapContentType);

                if (_channelFactory.MessageEncoderFactory is BinaryMessageEncoderFactory)
                {
                    clientWebSocket.Options.SetRequestHeader(WebSocketTransportSettings.BinaryEncoderTransferModeHeader, _channelFactory.TransferMode.ToString());
                }
            }

            (NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel) =
                await HttpChannelUtilities.GetCredentialAsync(_channelFactory.AuthenticationScheme, _webRequestTokenProvider, timeout);

            if (_channelFactory.Proxy != null)
            {
                clientWebSocket.Options.Proxy = _channelFactory.Proxy;
            }
            else if (_channelFactory.ProxyFactory != null)
            {
                clientWebSocket.Options.Proxy = await _channelFactory.ProxyFactory.CreateWebProxyAsync(
                    authenticationLevel,
                    impersonationLevel,
                    _webRequestProxyTokenProvider,
                    helper.RemainingTime());
            }

            if (credential == CredentialCache.DefaultCredentials || credential == null)
            {
                if (_channelFactory.AuthenticationScheme != AuthenticationSchemes.Anonymous)
                {
                    clientWebSocket.Options.UseDefaultCredentials = true;
                }
            }
            else
            {
                clientWebSocket.Options.UseDefaultCredentials = false;
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

                clientWebSocket.Options.Credentials = credentials;
            }

            if (_channelFactory.AllowCookies)
            {
                var cookieContainerManager = _channelFactory.GetHttpCookieContainerManager();
                clientWebSocket.Options.Cookies = cookieContainerManager.CookieContainer;
            }

            clientWebSocket.Options.KeepAliveInterval = _channelFactory.WebSocketSettings.KeepAliveInterval;
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

