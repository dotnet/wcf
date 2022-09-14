// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
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
        private StreamUpgradeProvider _upgrade;
        private ChannelBinding _channelBindingToken;

        public StreamedFramingRequestChannel(ChannelManagerBase factory, IConnectionOrientedTransportChannelFactorySettings settings,
            EndpointAddress remoteAddress, Uri via, IConnectionInitiator connectionInitiator, ConnectionPool connectionPool)
            : base(factory, remoteAddress, via, settings.ManualAddressing)
        {
            _settings = settings;
            _connectionInitiator = connectionInitiator;
            _connectionPool = connectionPool;

            _messageEncoder = settings.MessageEncoderFactory.Encoder;
            _upgrade = settings.Upgrade;
        }

        private Memory<byte> Preamble { get; set; }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override void OnOpen(TimeSpan timeout) { }

        protected internal override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override void OnOpened()
        {
            // setup our preamble which we'll use for all connections we establish
            EncodedVia encodedVia = new EncodedVia(Via.AbsoluteUri);
            EncodedContentType encodedContentType = EncodedContentType.Create(_settings.MessageEncoderFactory.Encoder.ContentType);
            int startSize = ClientSingletonEncoder.ModeBytes.Length + ClientSingletonEncoder.CalcStartSize(encodedVia, encodedContentType);
            int preambleEndOffset = 0;
            if (_upgrade == null)
            {
                preambleEndOffset = startSize;
                startSize += SessionEncoder.PreambleEndBytes.Length;
            }
            Preamble = Fx.AllocateByteArray(startSize);
            ClientSingletonEncoder.ModeBytes.CopyTo(Preamble);
            ClientSingletonEncoder.EncodeStart(Preamble.Slice(ClientSingletonEncoder.ModeBytes.Length), encodedVia, encodedContentType);
            if (preambleEndOffset > 0)
            {
                ClientSingletonEncoder.PreambleEndBytes.CopyTo(Preamble.Slice(preambleEndOffset));
            }

            // and then transition to the Opened state
            base.OnOpened();
        }

        protected override IAsyncRequest CreateAsyncRequest(Message message) => new StreamedConnectionPoolHelper.StreamedFramingAsyncRequest(this);

        internal async Task<(IConnection connection, SecurityMessageProperty remoteSecurity)> SendPreambleAsync(IConnection connection, TimeoutHelper timeoutHelper, ClientFramingDecoder decoder)
        {
            SecurityMessageProperty remoteSecurity = null;
            await connection.WriteAsync(Preamble, true, timeoutHelper.RemainingTime());

            if (_upgrade != null)
            {
                IStreamUpgradeChannelBindingProvider channelBindingProvider = _upgrade.GetProperty<IStreamUpgradeChannelBindingProvider>();
                StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(RemoteAddress, Via);

                bool upgradeInitiated;
                (upgradeInitiated, connection)= await ConnectionUpgradeHelper.InitiateUpgradeAsync(upgradeInitiator, connection, decoder, this, timeoutHelper.RemainingTime());
                if (!upgradeInitiated)
                {
                    await ConnectionUpgradeHelper.DecodeFramingFaultAsync(decoder, connection, Via, _messageEncoder.ContentType, timeoutHelper.RemainingTime());
                }

                if (channelBindingProvider != null && channelBindingProvider.IsChannelBindingSupportEnabled)
                {
                    _channelBindingToken = channelBindingProvider.GetChannelBinding(upgradeInitiator, ChannelBindingKind.Endpoint);
                }

                remoteSecurity = StreamSecurityUpgradeInitiator.GetRemoteSecurity(upgradeInitiator);
                await connection.WriteAsync(ClientSingletonEncoder.PreambleEndBytes, true, timeoutHelper.RemainingTime());
            }

            byte[] ackBuffer = new byte[1];
            int ackBytesRead = await connection.ReadAsync(ackBuffer, timeoutHelper.RemainingTime());

            if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, decoder, Via))
            {
                await ConnectionUpgradeHelper.DecodeFramingFaultAsync(decoder, connection, Via,
                    _messageEncoder.ContentType, timeoutHelper.RemainingTime());
            }

            return (connection, remoteSecurity);
        }

        protected override void OnClose(TimeSpan timeout) => base.WaitForPendingRequests(timeout);

        protected internal override Task OnCloseAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override void OnClosed()
        {
            base.OnClosed();

            // clean up the CBT after transitioning to the closed state
            ChannelBindingUtility.Dispose(ref _channelBindingToken);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => WaitForPendingRequestsAsync(timeout).ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        internal class StreamedConnectionPoolHelper : ConnectionPoolHelper
        {
            private StreamedFramingRequestChannel _channel;
            private SecurityMessageProperty _remoteSecurity;

            public StreamedConnectionPoolHelper(StreamedFramingRequestChannel channel)
                : base(channel._connectionPool, channel._connectionInitiator, channel.Via)
            {
                _channel = channel;
            }

            public ClientSingletonDecoder Decoder { get; private set; }

            public SecurityMessageProperty RemoteSecurity
            {
                get { return _remoteSecurity; }
            }

            protected override TimeoutException CreateNewConnectionTimeoutException(TimeSpan timeout, TimeoutException innerException) => new TimeoutException(SR.Format(SR.RequestTimedOutEstablishingTransportSession,
                        timeout, _channel.Via.AbsoluteUri), innerException);

            protected override async Task<IConnection> AcceptPooledConnectionAsync(IConnection connection, TimeoutHelper timeoutHelper)
            {
                Decoder = new ClientSingletonDecoder(0);
                (connection, _remoteSecurity) = await _channel.SendPreambleAsync(connection, timeoutHelper, Decoder);
                return connection;
            }

            private class ClientSingletonConnectionReader : SingletonConnectionReader
            {
                private StreamedConnectionPoolHelper _connectionPoolHelper;

                public ClientSingletonConnectionReader(IConnection connection, StreamedConnectionPoolHelper connectionPoolHelper,
                    IConnectionOrientedTransportFactorySettings settings)
                    : base(connection, connectionPoolHelper.RemoteSecurity, settings, null)
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

                protected override void OnClose(TimeSpan timeout) => _connectionPoolHelper.Close(timeout);
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

                public void Abort(RequestChannel requestChannel) => Cleanup();

                public void Fault(RequestChannel requestChannel) => Cleanup();

                private void Cleanup() => _connectionPoolHelper.Abort();


                public void OnReleaseRequest()
                {
                }

                public async Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper)
                {
                    try
                    {
                        _connectionReader = new ClientSingletonConnectionReader(_connection, _connectionPoolHelper, _channel._settings);
                        _connectionReader.DoneSending(TimeSpan.Zero);
                        Message message = await _connectionReader.ReceiveAsync(timeoutHelper);
                        if (message != null)
                        {
                            ChannelBindingUtility.TryAddToMessage(_channel._channelBindingToken, message, false);
                        }

                        return message;
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
