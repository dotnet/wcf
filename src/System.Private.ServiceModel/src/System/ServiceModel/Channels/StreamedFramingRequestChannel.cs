// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels.ConnectionHelpers;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class StreamedFramingRequestChannel : RequestChannel
    {
        internal IConnectionInitiator _connectionInitiator;
        internal ConnectionPool _connectionPool;
        private MessageEncoder _messageEncoder;
        private IConnectionOrientedTransportFactorySettings _settings;
        private byte[] _startBytes;
        private StreamUpgradeProvider _upgrade;
        private ChannelBinding _channelBindingToken;

        public StreamedFramingRequestChannel(ChannelManagerBase factory, IConnectionOrientedTransportChannelFactorySettings settings,
            EndpointAddress remoteAddresss, Uri via, IConnectionInitiator connectionInitiator, ConnectionPool connectionPool)
            : base(factory, remoteAddresss, via, settings.ManualAddressing)
        {
            _settings = settings;
            _connectionInitiator = connectionInitiator;
            _connectionPool = connectionPool;

            _messageEncoder = settings.MessageEncoderFactory.Encoder;
            _upgrade = settings.Upgrade;
        }

        private byte[] Preamble
        {
            get { return _startBytes; }
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return TaskHelpers.CompletedTask();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override void OnOpened()
        {
            // setup our preamble which we'll use for all connections we establish
            EncodedVia encodedVia = new EncodedVia(this.Via.AbsoluteUri);
            EncodedContentType encodedContentType = EncodedContentType.Create(_settings.MessageEncoderFactory.Encoder.ContentType);
            int startSize = ClientSingletonEncoder.ModeBytes.Length + ClientSingletonEncoder.CalcStartSize(encodedVia, encodedContentType);
            int preambleEndOffset = 0;
            if (_upgrade == null)
            {
                preambleEndOffset = startSize;
                startSize += ClientDuplexEncoder.PreambleEndBytes.Length;
            }
            _startBytes = Fx.AllocateByteArray(startSize);
            Buffer.BlockCopy(ClientSingletonEncoder.ModeBytes, 0, _startBytes, 0, ClientSingletonEncoder.ModeBytes.Length);
            ClientSingletonEncoder.EncodeStart(_startBytes, ClientSingletonEncoder.ModeBytes.Length, encodedVia, encodedContentType);
            if (preambleEndOffset > 0)
            {
                Buffer.BlockCopy(ClientSingletonEncoder.PreambleEndBytes, 0, _startBytes, preambleEndOffset, ClientSingletonEncoder.PreambleEndBytes.Length);
            }

            // and then transition to the Opened state
            base.OnOpened();
        }

        protected override IAsyncRequest CreateAsyncRequest(Message message)
        {
            return new StreamedConnectionPoolHelper.StreamedFramingAsyncRequest(this);
        }

        internal IConnection SendPreamble(IConnection connection, ref TimeoutHelper timeoutHelper,
            ClientFramingDecoder decoder, out SecurityMessageProperty remoteSecurity)
        {
            connection.Write(Preamble, 0, Preamble.Length, true, timeoutHelper.RemainingTime());

            if (_upgrade != null)
            {
                IStreamUpgradeChannelBindingProvider channelBindingProvider = _upgrade.GetProperty<IStreamUpgradeChannelBindingProvider>();

                StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(this.RemoteAddress, this.Via);

                if (!ConnectionUpgradeHelper.InitiateUpgrade(upgradeInitiator, ref connection, decoder,
                    this, ref timeoutHelper))
                {
                    ConnectionUpgradeHelper.DecodeFramingFault(decoder, connection, Via, _messageEncoder.ContentType, ref timeoutHelper);
                }

#if FEATURE_CORECLR // ExtendedProtection
                if (channelBindingProvider != null && channelBindingProvider.IsChannelBindingSupportEnabled)
                {
                    _channelBindingToken = channelBindingProvider.GetChannelBinding(upgradeInitiator, ChannelBindingKind.Endpoint);
                }
#endif // FEATURE_CORECLR // ExtendedProtection

                remoteSecurity = StreamSecurityUpgradeInitiator.GetRemoteSecurity(upgradeInitiator);

                connection.Write(ClientSingletonEncoder.PreambleEndBytes, 0,
                    ClientSingletonEncoder.PreambleEndBytes.Length, true, timeoutHelper.RemainingTime());
            }
            else
            {
                remoteSecurity = null;
            }

            // read ACK
            byte[] ackBuffer = new byte[1];
            int ackBytesRead = connection.Read(ackBuffer, 0, ackBuffer.Length, timeoutHelper.RemainingTime());
            if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, decoder, this.Via))
            {
                ConnectionUpgradeHelper.DecodeFramingFault(decoder, connection, Via, _messageEncoder.ContentType, ref timeoutHelper);
            }

            return connection;
        }

        internal async Task<IConnection> SendPreambleAsync(IConnection connection, TimeoutHelper timeoutHelper, ClientFramingDecoder decoder)
        {
            await connection.WriteAsync(Preamble, 0, Preamble.Length, true, timeoutHelper.RemainingTime());

            if (_upgrade != null)
            {
                StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(this.RemoteAddress, this.Via);

                await upgradeInitiator.OpenAsync(timeoutHelper.RemainingTime());
                var connectionWrapper = new OutWrapper<IConnection>();
                connectionWrapper.Value = connection;
                bool upgradeInitiated = await ConnectionUpgradeHelper.InitiateUpgradeAsync(upgradeInitiator, connectionWrapper, decoder, this, timeoutHelper.RemainingTime());
                connection = connectionWrapper.Value;
                if (!upgradeInitiated)
                {
                    await ConnectionUpgradeHelper.DecodeFramingFaultAsync(decoder, connection, this.Via, _messageEncoder.ContentType, timeoutHelper.RemainingTime());
                }

                await upgradeInitiator.CloseAsync(timeoutHelper.RemainingTime());

                await connection.WriteAsync(ClientSingletonEncoder.PreambleEndBytes, 0, ClientSingletonEncoder.PreambleEndBytes.Length, true, timeoutHelper.RemainingTime());
            }

            byte[] ackBuffer = new byte[1];
            int ackBytesRead = await connection.ReadAsync(ackBuffer, 0, ackBuffer.Length, timeoutHelper.RemainingTime());

            if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, decoder, Via))
            {
                await ConnectionUpgradeHelper.DecodeFramingFaultAsync(decoder, connection, Via,
                    _messageEncoder.ContentType, timeoutHelper.RemainingTime());
            }

            return connection;
        }

        protected override void OnClose(TimeSpan timeout)
        {
            base.WaitForPendingRequests(timeout);
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            // clean up the CBT after transitioning to the closed state
            ChannelBindingUtility.Dispose(ref _channelBindingToken);
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return base.WaitForPendingRequestsAsync(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        internal class StreamedConnectionPoolHelper : ConnectionPoolHelper
        {
            private StreamedFramingRequestChannel _channel;
            private ClientSingletonDecoder _decoder;
            private SecurityMessageProperty _remoteSecurity;

            public StreamedConnectionPoolHelper(StreamedFramingRequestChannel channel)
                : base(channel._connectionPool, channel._connectionInitiator, channel.Via)
            {
                _channel = channel;
            }

            public ClientSingletonDecoder Decoder
            {
                get { return _decoder; }
            }

            public SecurityMessageProperty RemoteSecurity
            {
                get { return _remoteSecurity; }
            }

            protected override TimeoutException CreateNewConnectionTimeoutException(TimeSpan timeout, TimeoutException innerException)
            {
                return new TimeoutException(SR.Format(SR.RequestTimedOutEstablishingTransportSession,
                        timeout, _channel.Via.AbsoluteUri), innerException);
            }

            protected override IConnection AcceptPooledConnection(IConnection connection, ref TimeoutHelper timeoutHelper)
            {
                _decoder = new ClientSingletonDecoder(0);
                return _channel.SendPreamble(connection, ref timeoutHelper, _decoder, out _remoteSecurity);
            }

            protected override Task<IConnection> AcceptPooledConnectionAsync(IConnection connection, ref TimeoutHelper timeoutHelper)
            {
                _decoder = new ClientSingletonDecoder(0);
                return _channel.SendPreambleAsync(connection, timeoutHelper, _decoder);
            }

            private class ClientSingletonConnectionReader : SingletonConnectionReader
            {
                private StreamedConnectionPoolHelper _connectionPoolHelper;

                public ClientSingletonConnectionReader(IConnection connection, StreamedConnectionPoolHelper connectionPoolHelper,
                    IConnectionOrientedTransportFactorySettings settings)
                    : base(connection, 0, 0, connectionPoolHelper.RemoteSecurity, settings, null)
                {
                    Contract.Assert(connectionPoolHelper != null);
                    _connectionPoolHelper = connectionPoolHelper;
                }

                protected override long StreamPosition
                {
                    get { return _connectionPoolHelper.Decoder.StreamPosition; }
                }

                protected override bool DecodeBytes(byte[] buffer, ref int offset, ref int size, ref bool isAtEof)
                {
                    while (size > 0)
                    {
                        int bytesRead = _connectionPoolHelper.Decoder.Decode(buffer, offset, size);
                        if (bytesRead > 0)
                        {
                            offset += bytesRead;
                            size -= bytesRead;
                        }

                        switch (_connectionPoolHelper.Decoder.CurrentState)
                        {
                            case ClientFramingDecoderState.EnvelopeStart:
                                // we're at the envelope
                                return true;

                            case ClientFramingDecoderState.End:
                                isAtEof = true;
                                return false;
                        }
                    }

                    return false;
                }

                protected override void OnClose(TimeSpan timeout)
                {
                    _connectionPoolHelper.Close(timeout);
                }
            }


            internal class StreamedFramingAsyncRequest : IAsyncRequest
            {
                private StreamedFramingRequestChannel _channel;
                private IConnection _connection;
                private StreamedConnectionPoolHelper _connectionPoolHelper;
                private Message _message;
                private TimeoutHelper _timeoutHelper;
                private ClientSingletonConnectionReader _connectionReader;

                public StreamedFramingAsyncRequest(StreamedFramingRequestChannel channel)
                {
                    _channel = channel;
                    _connectionPoolHelper = new StreamedConnectionPoolHelper(channel);
                }

                public async Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper)
                {
                    _timeoutHelper = timeoutHelper;
                    _message = message;

                    bool success = false;
                    try
                    {
                        try
                        {
                            _connection = await _connectionPoolHelper.EstablishConnectionAsync(timeoutHelper.RemainingTime());

                            ChannelBindingUtility.TryAddToMessage(_channel._channelBindingToken, _message, false);
                            await StreamingConnectionHelper.WriteMessageAsync(_message, _connection, true, _channel._settings, timeoutHelper);
                        }
                        catch (TimeoutException exception)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                new TimeoutException(SR.Format(SR.TimeoutOnRequest, timeoutHelper.RemainingTime()), exception));
                        }

                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            Cleanup();
                        }
                    }
                }

                public void Abort(RequestChannel requestChannel)
                {
                    Cleanup();
                }

                public void Fault(RequestChannel requestChannel)
                {
                    Cleanup();
                }

                private void Cleanup()
                {
                    _connectionPoolHelper.Abort();
                }


                public void OnReleaseRequest()
                {
                }

                public async Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper)
                {
                    try
                    {
                        _connectionReader = new ClientSingletonConnectionReader(_connection, _connectionPoolHelper, _channel._settings);
                        _connectionReader.DoneSending(TimeSpan.Zero);
                        return await _connectionReader.ReceiveAsync(timeoutHelper);
                    }
                    catch (OperationCanceledException)
                    {
                        var cancelToken = _timeoutHelper.GetCancellationToken();
                        if (cancelToken.IsCancellationRequested)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(
                                SR.RequestChannelWaitForReplyTimedOut, timeoutHelper.OriginalTimeout)));
                        }
                        else
                        {
                            // Cancellation came from somewhere other than timeoutCts and needs to be handled differently.
                            throw;
                        }
                    }
                }
            }
        }
    }
}
