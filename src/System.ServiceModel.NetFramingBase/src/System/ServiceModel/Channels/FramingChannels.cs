// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.IdentityModel.Claims;
using System.Buffers;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Channels
{
    internal abstract class FramingDuplexSessionChannel : TransportDuplexSessionChannel
    {
        private static EndpointAddress s_anonymousEndpointAddress = new EndpointAddress(EndpointAddress.AnonymousUri, new AddressHeader[0]);

        private FramingDuplexSessionChannel(ChannelManagerBase manager, IConnectionOrientedTransportFactorySettings settings,
            EndpointAddress localAddress, Uri localVia, EndpointAddress remoteAddress, Uri via)
            : base(manager, settings, localAddress, localVia, remoteAddress, via)
        {
        }

        protected FramingDuplexSessionChannel(ChannelManagerBase factory, IConnectionOrientedTransportFactorySettings settings,
            EndpointAddress remoteAddress, Uri via)
            : this(factory, settings, s_anonymousEndpointAddress, settings.MessageVersion.Addressing == AddressingVersion.None ? null : new Uri("http://www.w3.org/2005/08/addressing/anonymous"),
            remoteAddress, via)
        {
            Session = FramingConnectionDuplexSession.CreateSession(this, settings.Upgrade);
        }

        protected IConnection Connection { get; set; }

        protected override bool IsStreamedOutput
        {
            get { return false; }
        }

        protected override ValueTask CloseOutputSessionCoreAsync(TimeSpan timeout)
        {
            return Connection.WriteAsync(SessionEncoder.EndBytes, true, timeout);
        }

        protected override void CompleteClose(TimeSpan timeout)
        {
            ReturnConnectionIfNecessary(false, timeout);
        }

        protected override async ValueTask OnSendCoreAsync(Message message, TimeSpan timeout)
        {
            bool allowOutputBatching;
            ArraySegment<byte> messageData;

            allowOutputBatching = message.Properties.AllowOutputBatching;
            messageData = EncodeMessage(message);

            await TaskHelpers.EnsureDefaultTaskScheduler();
            await Connection.WriteAsync(messageData, !allowOutputBatching, timeout);
            BufferManager.ReturnBuffer(messageData.Array);
        }

        protected override ValueTask CloseOutputAsync(TimeSpan timeout)
        {
            return Connection.WriteAsync(SessionEncoder.EndBytes, true, timeout);
        }

        protected override ValueTask StartWritingBufferedMessage(Message message, ArraySegment<byte> messageData, bool allowOutputBatching, TimeSpan timeout)
        {
            return Connection.WriteAsync(messageData, !allowOutputBatching, timeout);
        }

        protected override ValueTask StartWritingStreamedMessage(Message message, TimeSpan timeout)
        {
            Contract.Assert(false, "Streamed output should never be called in this channel.");
            return ValueTask.FromException(new InvalidOperationException());
        }

        protected override ArraySegment<byte> EncodeMessage(Message message)
        {
            ArraySegment<byte> messageData = MessageEncoder.WriteMessage(message,
                int.MaxValue, BufferManager, SessionEncoder.MaxMessageFrameSize);

            messageData = SessionEncoder.EncodeMessageFrame(messageData);

            return messageData;
        }

        internal class FramingConnectionDuplexSession : ConnectionDuplexSession
        {
            private FramingConnectionDuplexSession(FramingDuplexSessionChannel channel)
                : base(channel)
            {
            }

            public static FramingConnectionDuplexSession CreateSession(FramingDuplexSessionChannel channel,
                StreamUpgradeProvider upgrade)
            {
                StreamSecurityUpgradeProvider security = upgrade as StreamSecurityUpgradeProvider;
                if (security == null)
                {
                    return new FramingConnectionDuplexSession(channel);
                }
                else
                {
                    return new SecureConnectionDuplexSession(channel);
                }
            }
            private class SecureConnectionDuplexSession : FramingConnectionDuplexSession, ISecuritySession
            {
                private EndpointIdentity _remoteIdentity;

                public SecureConnectionDuplexSession(FramingDuplexSessionChannel channel)
                    : base(channel)
                {
                    // empty
                }

                EndpointIdentity ISecuritySession.RemoteIdentity
                {
                    get
                    {
                        if (_remoteIdentity == null)
                        {
                            SecurityMessageProperty security = Channel.RemoteSecurity;
                            if (security != null && security.ServiceSecurityContext != null)
                            {
                                Claim identityClaim = SecurityUtils.GetPrimaryIdentityClaim(security.ServiceSecurityContext.AuthorizationContext);
                                if (identityClaim != null && security.ServiceSecurityContext.PrimaryIdentity != null)
                                {
                                    _remoteIdentity = EndpointIdentity.CreateIdentity(identityClaim);
                                }
                            }
                        }

                        return _remoteIdentity;
                    }
                }
            }
        }
    }

    internal class ClientFramingDuplexSessionChannel : FramingDuplexSessionChannel
    {
        private IConnectionOrientedTransportChannelFactorySettings _settings;
        private ClientDuplexDecoder _decoder;
        private StreamUpgradeProvider _upgrade;
        private ConnectionPoolHelper _connectionPoolHelper;

        public ClientFramingDuplexSessionChannel(ChannelManagerBase factory, IConnectionOrientedTransportChannelFactorySettings settings,
            EndpointAddress remoteAddress, Uri via, IConnectionInitiator connectionInitiator, ConnectionPool connectionPool)
            : base(factory, settings, remoteAddress, via)
        {
            _settings = settings;
            MessageEncoder = settings.MessageEncoderFactory.CreateSessionEncoder();
            _upgrade = settings.Upgrade;
            _connectionPoolHelper = new DuplexConnectionPoolHelper(this, connectionPool, connectionInitiator);
        }

        private Memory<byte> CreatePreamble()
        {
            EncodedVia encodedVia = new EncodedVia(Via.AbsoluteUri);
            EncodedContentType encodedContentType = EncodedContentType.Create(MessageEncoder.ContentType);

            // calculate preamble length
            int startSize = ClientDuplexEncoder.ModeBytes.Length + SessionEncoder.CalcStartSize(encodedVia, encodedContentType);
            int preambleEndOffset = 0;
            if (_upgrade == null)
            {
                preambleEndOffset = startSize;
                startSize += SessionEncoder.PreambleEndBytes.Length;
            }

            Memory<byte> startBytes = Fx.AllocateByteArray(startSize);
            ClientDuplexEncoder.ModeBytes.CopyTo(startBytes);
            SessionEncoder.EncodeStart(startBytes.Slice(ClientDuplexEncoder.ModeBytes.Length), encodedVia, encodedContentType);
            if (preambleEndOffset > 0)
            {
                SessionEncoder.PreambleEndBytes.CopyTo(startBytes.Slice(preambleEndOffset));
            }

            return startBytes;
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public override T GetProperty<T>()
        {
            T result = base.GetProperty<T>();

            if (result == null && _upgrade != null)
            {
                result = _upgrade.GetProperty<T>();
            }

            return result;
        }

        private async Task<IConnection> SendPreambleAsync(IConnection connection, Memory<byte> preamble, TimeSpan timeout)
        {
            var timeoutHelper = new TimeoutHelper(timeout);

            // initialize a new decoder
            _decoder = new ClientDuplexDecoder(0);
            byte[] ackBuffer = new byte[1];

            if (!await SendLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                                SR.Format(SR.CloseTimedOut, timeout),
                                                TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                await connection.WriteAsync(preamble, true, timeoutHelper.RemainingTime());

                if (_upgrade != null)
                {
                    IStreamUpgradeChannelBindingProvider channelBindingProvider = _upgrade.GetProperty<IStreamUpgradeChannelBindingProvider>();
                    StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(RemoteAddress, Via);

                    await upgradeInitiator.OpenAsync(timeoutHelper.RemainingTime());
                    bool upgradeInitiated;
                    (upgradeInitiated, connection) = await ConnectionUpgradeHelper.InitiateUpgradeAsync(upgradeInitiator, connection, _decoder, this, timeoutHelper.RemainingTime());
                    if (!upgradeInitiated)
                    {
                        await ConnectionUpgradeHelper.DecodeFramingFaultAsync(_decoder, connection, Via, MessageEncoder.ContentType, timeoutHelper.RemainingTime());
                    }

                    if (channelBindingProvider != null && channelBindingProvider.IsChannelBindingSupportEnabled)
                    {
                        SetChannelBinding(channelBindingProvider.GetChannelBinding(upgradeInitiator, ChannelBindingKind.Endpoint));
                    }

                    SetRemoteSecurity(upgradeInitiator);
                    await upgradeInitiator.CloseAsync(timeoutHelper.RemainingTime());

                    await connection.WriteAsync(SessionEncoder.PreambleEndBytes, true, timeoutHelper.RemainingTime());
                }

                int ackBytesRead = await connection.ReadAsync(ackBuffer, timeoutHelper.RemainingTime());

                if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, _decoder, Via))
                {
                    await ConnectionUpgradeHelper.DecodeFramingFaultAsync(_decoder, connection, Via,
                        MessageEncoder.ContentType, timeoutHelper.RemainingTime());
                }

                return connection;
            }
            finally
            {
                SendLock.Release();
            }
        }

        internal protected override async Task OnOpenAsync(TimeSpan timeout)
        {
            IConnection connection;
            try
            {
                using (TaskHelpers.RunTaskContinuationsOnOurThreads())
                {
                    connection = await _connectionPoolHelper.EstablishConnectionAsync(timeout);
                }
            }
            catch (TimeoutException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SR.Format(SR.TimeoutOnOpen, timeout), exception));
            }

            bool connectionAccepted = false;
            try
            {
                AcceptConnection(connection);
                connectionAccepted = true;
            }
            finally
            {
                if (!connectionAccepted)
                {
                    _connectionPoolHelper.Abort();
                }
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            OnOpenAsync(timeout).GetAwaiter().GetResult();
        }

        protected override void ReturnConnectionIfNecessary(bool abort, TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (abort)
                {
                    _connectionPoolHelper.Abort();
                }
                else
                {
                    _connectionPoolHelper.Close(timeout);
                }
            }
        }

        private void AcceptConnection(IConnection connection)
        {
            SetMessageSource(new ClientDuplexConnectionReader(this, connection, _decoder, _settings, MessageEncoder));

            lock (ThisLock)
            {
                if (State != CommunicationState.Opening)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationObjectAbortedException(SR.Format(SR.DuplexChannelAbortedDuringOpen, Via)));
                }

                Connection = connection;
            }
        }

        private void SetRemoteSecurity(StreamUpgradeInitiator upgradeInitiator)
        {
            RemoteSecurity = StreamSecurityUpgradeInitiator.GetRemoteSecurity(upgradeInitiator);
        }

        protected override void PrepareMessage(Message message)
        {
            base.PrepareMessage(message);
        }

        internal class DuplexConnectionPoolHelper : ConnectionPoolHelper
        {
            private ClientFramingDuplexSessionChannel _channel;
            private Memory<byte> _preamble;

            public DuplexConnectionPoolHelper(ClientFramingDuplexSessionChannel channel,
                ConnectionPool connectionPool, IConnectionInitiator connectionInitiator)
                : base(connectionPool, connectionInitiator, channel.Via)
            {
                _channel = channel;
                _preamble = channel.CreatePreamble();
            }

            protected override TimeoutException CreateNewConnectionTimeoutException(TimeSpan timeout, TimeoutException innerException)
            {
                return new TimeoutException(SR.Format(SR.OpenTimedOutEstablishingTransportSession,
                        timeout, _channel.Via.AbsoluteUri), innerException);
            }

            protected override Task<IConnection> AcceptPooledConnectionAsync(IConnection connection, TimeoutHelper timeoutHelper)
            {
                return _channel.SendPreambleAsync(connection, _preamble, timeoutHelper.RemainingTime());
            }
        }
    }

    // used by StreamedFramingRequestChannel and ClientFramingDuplexSessionChannel
    internal class ConnectionUpgradeHelper
    {
        public static async Task DecodeFramingFaultAsync(ClientFramingDecoder decoder, IConnection connection,
            Uri via, string contentType, TimeSpan timeout)
        {
            var timeoutHelper = new TimeoutHelper(timeout);
            ValidateReadingFaultString(decoder);

            byte[] faultBuffer = ArrayPool<byte>.Shared.Rent(FaultStringDecoder.FaultSizeQuota);
            int size = await connection.ReadAsync(new Memory<byte>(faultBuffer,0,
                Math.Min(FaultStringDecoder.FaultSizeQuota, connection.ConnectionBufferSize)),
                timeoutHelper.RemainingTime());

            int offset = 0;
            while (size > 0)
            {
                int bytesDecoded = decoder.Decode(faultBuffer, offset, size);
                offset += bytesDecoded;
                size -= bytesDecoded;

                if (decoder.CurrentState == ClientFramingDecoderState.Fault)
                {
                    await ConnectionUtilities.CloseNoThrowAsync(connection, timeoutHelper.RemainingTime());
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        FaultStringDecoder.GetFaultException(decoder.Fault, via.ToString(), contentType));
                }
                else
                {
                    if (decoder.CurrentState != ClientFramingDecoderState.ReadingFaultString)
                    {
                        throw new Exception("invalid framing client state machine");
                    }
                    if (size == 0)
                    {
                        offset = 0;
                        size = await connection.ReadAsync(new Memory<byte>(faultBuffer, 0,
                            Math.Min(FaultStringDecoder.FaultSizeQuota, connection.ConnectionBufferSize)),
                            timeoutHelper.RemainingTime());
                    }
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(decoder.CreatePrematureEOFException());
        }

        public static async Task<(bool success, IConnection connection)> InitiateUpgradeAsync(
            StreamUpgradeInitiator upgradeInitiator, IConnection connection,
            ClientFramingDecoder decoder, IDefaultCommunicationTimeouts defaultTimeouts, TimeSpan timeout)
        {
            string upgradeContentType = upgradeInitiator.GetNextUpgrade();

            while (upgradeContentType != null)
            {
                EncodedUpgrade encodedUpgrade = new EncodedUpgrade(upgradeContentType);
                // write upgrade request framing for synchronization
                await connection.WriteAsync(encodedUpgrade.EncodedBytes, true, timeout);
                byte[] buffer = new byte[1];

                // read upgrade response framing 
                int size = await connection.ReadAsync(buffer, timeout);

                if (!ValidateUpgradeResponse(buffer, size, decoder)) // we have a problem
                {
                    return (false, connection);
                }

                // initiate wire upgrade
                ConnectionStream connectionStream = new ConnectionStream(connection, defaultTimeouts);
                Stream upgradedStream = await upgradeInitiator.InitiateUpgradeAsync(connectionStream);

                // and re-wrap connection
                connection = new StreamConnection(upgradedStream, connectionStream);
                upgradeContentType = upgradeInitiator.GetNextUpgrade();
            }

            return (true, connection);
        }

        private static void ValidateReadingFaultString(ClientFramingDecoder decoder)
        {
            if (decoder.CurrentState != ClientFramingDecoderState.ReadingFaultString)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                    SR.ServerRejectedUpgradeRequest));
            }
        }

        public static bool ValidatePreambleResponse(byte[] buffer, int count, ClientFramingDecoder decoder, Uri via)
        {
            if (count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SR.Format(SR.ServerRejectedSessionPreamble, via),
                    decoder.CreatePrematureEOFException()));
            }

            // decode until the framing byte has been processed (it always will be)
            while(decoder.Decode(buffer, 0, count) == 0)
            {
                // do nothing
            }

            if (decoder.CurrentState != ClientFramingDecoderState.Start) // we have a problem
            {
                return false;
            }

            return true;
        }

        private static bool ValidateUpgradeResponse(byte[] buffer, int count, ClientFramingDecoder decoder)
        {
            if (count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.ServerRejectedUpgradeRequest, decoder.CreatePrematureEOFException()));
            }

            // decode until the framing byte has been processed (it always will be)
            while (decoder.Decode(buffer, 0, count) == 0)
            {
                // do nothing
            }

            if (decoder.CurrentState != ClientFramingDecoderState.UpgradeResponse) // we have a problem
            {
                return false;
            }

            return true;
        }
    }
}
