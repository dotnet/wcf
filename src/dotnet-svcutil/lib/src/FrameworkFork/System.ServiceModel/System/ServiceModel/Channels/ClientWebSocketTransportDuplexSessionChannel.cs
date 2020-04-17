// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ClientWebSocketTransportDuplexSessionChannel : WebSocketTransportDuplexSessionChannel
    {
        private readonly ClientWebSocketFactory _connectionFactory;
        private HttpChannelFactory<IDuplexSessionChannel> _channelFactory;
        private SecurityTokenProviderContainer _webRequestTokenProvider;
        private SecurityTokenProviderContainer _webRequestProxyTokenProvider;
        private volatile bool _cleanupStarted;
        private volatile bool _cleanupIdentity;

        public ClientWebSocketTransportDuplexSessionChannel(HttpChannelFactory<IDuplexSessionChannel> channelFactory, ClientWebSocketFactory connectionFactory, EndpointAddress remoteAddresss, Uri via)
            : base(channelFactory, remoteAddresss, via)
        {
            Contract.Assert(channelFactory != null, "connection factory must be set");
            _channelFactory = channelFactory;
            _connectionFactory = connectionFactory;
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

                ChannelParameterCollection channelParameterCollection = new ChannelParameterCollection();

                if (HttpChannelFactory<IDuplexSessionChannel>.MapIdentity(this.RemoteAddress, _channelFactory.AuthenticationScheme))
                {
                    lock (ThisLock)
                    {
                        _cleanupIdentity = HttpTransportSecurityHelpers.AddIdentityMapping(Via, RemoteAddress);
                    }
                }

                X509Certificate2 clientCertificate = null;
                HttpsChannelFactory<IDuplexSessionChannel> httpsChannelFactory = _channelFactory as HttpsChannelFactory<IDuplexSessionChannel>;
                if (httpsChannelFactory != null && httpsChannelFactory.RequireClientCertificate)
                {
                    var certificateProvider = httpsChannelFactory.CreateAndOpenCertificateTokenProvider(RemoteAddress, Via, channelParameterCollection, helper.RemainingTime());
                    var clientCertificateToken = httpsChannelFactory.GetCertificateSecurityToken(certificateProvider, RemoteAddress, Via, channelParameterCollection, ref helper);
                    var x509Token = (X509SecurityToken)clientCertificateToken.Token;
                    clientCertificate = x509Token.Certificate;
                }

                try
                {
                    WebSocket = await CreateWebSocketWithFactoryAsync(clientCertificate, helper);
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
                    throw FxTrace.Exception.AsError(new CommunicationException(string.Format(SRServiceModel.WebSocketVersionMismatchFromServer, ""), ex));
                case WebSocketError.UnsupportedProtocol:
                    throw FxTrace.Exception.AsError(new CommunicationException(string.Format(SRServiceModel.WebSocketSubProtocolMismatchFromServer, ""), ex));
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

        private async Task<WebSocket> CreateWebSocketWithFactoryAsync(X509Certificate2 certificate, TimeoutHelper timeoutHelper)
        {
            Contract.Assert(_connectionFactory != null, "Invalid call: CreateWebSocketWithFactory.");

            if (WcfEventSource.Instance.WebSocketCreateClientWebSocketWithFactoryIsEnabled())
            {
                WcfEventSource.Instance.WebSocketCreateClientWebSocketWithFactory(EventTraceActivity, _connectionFactory.GetType().FullName);
            }

            // Create the client WebSocket with the factory.
            WebSocket ws;
            try
            {
                if (certificate != null)
                {
                    throw ExceptionHelper.PlatformNotSupported("client certificates not supported yet");
                }
                var headers = new WebHeaderCollection();
                headers[WebSocketTransportSettings.SoapContentTypeHeader] = _channelFactory.WebSocketSoapContentType;
                if (_channelFactory.MessageEncoderFactory is BinaryMessageEncoderFactory)
                {
                    headers[WebSocketTransportSettings.BinaryEncoderTransferModeHeader] = _channelFactory.TransferMode.ToString();
                }

                var credentials = _channelFactory.GetCredentials();
                ws = await _connectionFactory.CreateWebSocketAsync(Via, headers, credentials, WebSocketSettings.Clone(), timeoutHelper);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.ClientWebSocketFactory_CreateWebSocketFailed, _connectionFactory.GetType().Name), e));
            }

            // The returned WebSocket should be valid (non-null), in an opened state and with the same SubProtocol that we requested.
            if (ws == null)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.ClientWebSocketFactory_InvalidWebSocket, _connectionFactory.GetType().Name)));
            }

            if (ws.State != WebSocketState.Open)
            {
                ws.Dispose();
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.ClientWebSocketFactory_InvalidWebSocket, _connectionFactory.GetType().Name)));
            }

            string requested = WebSocketSettings.SubProtocol;
            string obtained = ws.SubProtocol;
            if (!(requested == null ? string.IsNullOrWhiteSpace(obtained) : requested.Equals(obtained, StringComparison.OrdinalIgnoreCase)))
            {
                ws.Dispose();
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.ClientWebSocketFactory_InvalidSubProtocol, _connectionFactory.GetType().Name, obtained, requested)));
            }

            return ws;
        }
    }
}

