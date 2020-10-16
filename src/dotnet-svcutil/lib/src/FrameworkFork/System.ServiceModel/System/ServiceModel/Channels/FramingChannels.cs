// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime;
using System.ServiceModel.Security;
using System.ServiceModel.Channels.ConnectionHelpers;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class FramingDuplexSessionChannel : TransportDuplexSessionChannel
    {
        private static EndpointAddress s_anonymousEndpointAddress = new EndpointAddress(EndpointAddress.AnonymousUri, new AddressHeader[0]);
        private IConnection _connection;
        private bool _exposeConnectionProperty;

        private FramingDuplexSessionChannel(ChannelManagerBase manager, IConnectionOrientedTransportFactorySettings settings,
            EndpointAddress localAddress, Uri localVia, EndpointAddress remoteAddresss, Uri via, bool exposeConnectionProperty)
            : base(manager, settings, localAddress, localVia, remoteAddresss, via)
        {
            _exposeConnectionProperty = exposeConnectionProperty;
        }

        protected FramingDuplexSessionChannel(ChannelManagerBase factory, IConnectionOrientedTransportFactorySettings settings,
            EndpointAddress remoteAddresss, Uri via, bool exposeConnectionProperty)
            : this(factory, settings, s_anonymousEndpointAddress, settings.MessageVersion.Addressing == AddressingVersion.None ? null : new Uri("http://www.w3.org/2005/08/addressing/anonymous"),
            remoteAddresss, via, exposeConnectionProperty)
        {
            this.Session = FramingConnectionDuplexSession.CreateSession(this, settings.Upgrade);
        }

        protected IConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        protected override bool IsStreamedOutput
        {
            get { return false; }
        }

        protected override void CloseOutputSessionCore(TimeSpan timeout)
        {
            Connection.Write(SessionEncoder.EndBytes, 0, SessionEncoder.EndBytes.Length, true, timeout);
        }

        protected override Task CloseOutputSessionCoreAsync(TimeSpan timeout)
        {
            return Connection.WriteAsync(SessionEncoder.EndBytes, 0, SessionEncoder.EndBytes.Length, true, timeout);
        }

        protected override void CompleteClose(TimeSpan timeout)
        {
            this.ReturnConnectionIfNecessary(false, timeout);
        }

        protected override void PrepareMessage(Message message)
        {
            if (_exposeConnectionProperty)
            {
                message.Properties[ConnectionMessageProperty.Name] = _connection;
            }
            base.PrepareMessage(message);
        }

        protected override void OnSendCore(Message message, TimeSpan timeout)
        {
            bool allowOutputBatching;
            ArraySegment<byte> messageData;

            allowOutputBatching = message.Properties.AllowOutputBatching;
            messageData = this.EncodeMessage(message);

            this.Connection.Write(messageData.Array, messageData.Offset, messageData.Count, !allowOutputBatching,
                timeout, this.BufferManager);
        }

        protected override AsyncCompletionResult BeginCloseOutput(TimeSpan timeout, Action<object> callback, object state)
        {
            return this.Connection.BeginWrite(SessionEncoder.EndBytes, 0, SessionEncoder.EndBytes.Length,
                    true, timeout, callback, state);
        }

        protected override void FinishWritingMessage()
        {
            this.Connection.EndWrite();
        }

        protected override AsyncCompletionResult StartWritingBufferedMessage(Message message, ArraySegment<byte> messageData, bool allowOutputBatching, TimeSpan timeout, Action<object> callback, object state)
        {
            return this.Connection.BeginWrite(messageData.Array, messageData.Offset, messageData.Count,
                    !allowOutputBatching, timeout, callback, state);
        }

        protected override AsyncCompletionResult StartWritingStreamedMessage(Message message, TimeSpan timeout, Action<object> callback, object state)
        {
            Contract.Assert(false, "Streamed output should never be called in this channel.");
            throw new InvalidOperationException();
        }

        protected override ArraySegment<byte> EncodeMessage(Message message)
        {
            ArraySegment<byte> messageData = MessageEncoder.WriteMessage(message,
                int.MaxValue, this.BufferManager, SessionEncoder.MaxMessageFrameSize);

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
                            SecurityMessageProperty security = this.Channel.RemoteSecurity;
                            if (security != null && security.ServiceSecurityContext != null &&
                                security.ServiceSecurityContext.IdentityClaim != null &&
                                security.ServiceSecurityContext.PrimaryIdentity != null)
                            {
                                _remoteIdentity = EndpointIdentity.CreateIdentity(
                                    security.ServiceSecurityContext.IdentityClaim);
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
        private bool _flowIdentity;

        public ClientFramingDuplexSessionChannel(ChannelManagerBase factory, IConnectionOrientedTransportChannelFactorySettings settings,
            EndpointAddress remoteAddresss, Uri via, IConnectionInitiator connectionInitiator, ConnectionPool connectionPool,
            bool exposeConnectionProperty, bool flowIdentity)
            : base(factory, settings, remoteAddresss, via, exposeConnectionProperty)
        {
            _settings = settings;
            this.MessageEncoder = settings.MessageEncoderFactory.CreateSessionEncoder();
            _upgrade = settings.Upgrade;
            _flowIdentity = flowIdentity;
            _connectionPoolHelper = new DuplexConnectionPoolHelper(this, connectionPool, connectionInitiator);
        }

        private ArraySegment<byte> CreatePreamble()
        {
            EncodedVia encodedVia = new EncodedVia(this.Via.AbsoluteUri);
            EncodedContentType encodedContentType = EncodedContentType.Create(this.MessageEncoder.ContentType);

            // calculate preamble length
            int startSize = ClientDuplexEncoder.ModeBytes.Length + SessionEncoder.CalcStartSize(encodedVia, encodedContentType);
            int preambleEndOffset = 0;
            if (_upgrade == null)
            {
                preambleEndOffset = startSize;
                startSize += ClientDuplexEncoder.PreambleEndBytes.Length;
            }

            byte[] startBytes = Fx.AllocateByteArray(startSize);
            Buffer.BlockCopy(ClientDuplexEncoder.ModeBytes, 0, startBytes, 0, ClientDuplexEncoder.ModeBytes.Length);
            SessionEncoder.EncodeStart(startBytes, ClientDuplexEncoder.ModeBytes.Length, encodedVia, encodedContentType);
            if (preambleEndOffset > 0)
            {
                Buffer.BlockCopy(ClientDuplexEncoder.PreambleEndBytes, 0, startBytes, preambleEndOffset, ClientDuplexEncoder.PreambleEndBytes.Length);
            }

            return new ArraySegment<byte>(startBytes, 0, startSize);
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

        private async Task<IConnection> SendPreambleAsync(IConnection connection, ArraySegment<byte> preamble, TimeSpan timeout)
        {
            var timeoutHelper = new TimeoutHelper(timeout);

            // initialize a new decoder
            _decoder = new ClientDuplexDecoder(0);
            byte[] ackBuffer = new byte[1];
            await connection.WriteAsync(preamble.Array, preamble.Offset, preamble.Count, true, timeoutHelper.RemainingTime());

            if (_upgrade != null)
            {
                StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(this.RemoteAddress, this.Via);

                await upgradeInitiator.OpenAsync(timeoutHelper.RemainingTime());
                var connectionWrapper = new OutWrapper<IConnection>();
                connectionWrapper.Value = connection;
                bool upgradeInitiated = await ConnectionUpgradeHelper.InitiateUpgradeAsync(upgradeInitiator, connectionWrapper, _decoder, this, timeoutHelper.RemainingTime());
                connection = connectionWrapper.Value;
                if (!upgradeInitiated)
                {
                    await ConnectionUpgradeHelper.DecodeFramingFaultAsync(_decoder, connection, this.Via, MessageEncoder.ContentType, timeoutHelper.RemainingTime());
                }

                SetRemoteSecurity(upgradeInitiator);
                await upgradeInitiator.CloseAsync(timeoutHelper.RemainingTime());

                await connection.WriteAsync(ClientDuplexEncoder.PreambleEndBytes, 0, ClientDuplexEncoder.PreambleEndBytes.Length, true, timeoutHelper.RemainingTime());
            }

            int ackBytesRead = await connection.ReadAsync(ackBuffer, 0, ackBuffer.Length, timeoutHelper.RemainingTime());

            if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, _decoder, Via))
            {
                await ConnectionUpgradeHelper.DecodeFramingFaultAsync(_decoder, connection, Via,
                    MessageEncoder.ContentType, timeoutHelper.RemainingTime());
            }

            return connection;
        }


        private IConnection SendPreamble(IConnection connection, ArraySegment<byte> preamble, ref TimeoutHelper timeoutHelper)
        {
            // initialize a new decoder
            _decoder = new ClientDuplexDecoder(0);
            byte[] ackBuffer = new byte[1];
            connection.Write(preamble.Array, preamble.Offset, preamble.Count, true, timeoutHelper.RemainingTime());

            if (_upgrade != null)
            {
                StreamUpgradeInitiator upgradeInitiator = _upgrade.CreateUpgradeInitiator(this.RemoteAddress, this.Via);

                upgradeInitiator.Open(timeoutHelper.RemainingTime());
                if (!ConnectionUpgradeHelper.InitiateUpgrade(upgradeInitiator, ref connection, _decoder, this, ref timeoutHelper))
                {
                    ConnectionUpgradeHelper.DecodeFramingFault(_decoder, connection, this.Via, MessageEncoder.ContentType, ref timeoutHelper);
                }

                SetRemoteSecurity(upgradeInitiator);
                upgradeInitiator.Close(timeoutHelper.RemainingTime());
                connection.Write(ClientDuplexEncoder.PreambleEndBytes, 0, ClientDuplexEncoder.PreambleEndBytes.Length, true, timeoutHelper.RemainingTime());
            }

            // read ACK
            int ackBytesRead = connection.Read(ackBuffer, 0, ackBuffer.Length, timeoutHelper.RemainingTime());
            if (!ConnectionUpgradeHelper.ValidatePreambleResponse(ackBuffer, ackBytesRead, _decoder, Via))
            {
                ConnectionUpgradeHelper.DecodeFramingFault(_decoder, connection, Via,
                    MessageEncoder.ContentType, ref timeoutHelper);
            }

            return connection;
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            IConnection connection;
            try
            {
                connection = await _connectionPoolHelper.EstablishConnectionAsync(timeout);
            }
            catch (TimeoutException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(string.Format(SRServiceModel.TimeoutOnOpen, timeout), exception));
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
            IConnection connection;
            try
            {
                connection = _connectionPoolHelper.EstablishConnection(timeout);
            }
            catch (TimeoutException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(string.Format(SRServiceModel.TimeoutOnOpen, timeout), exception));
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
            base.SetMessageSource(new ClientDuplexConnectionReader(this, connection, _decoder, _settings, MessageEncoder));

            lock (ThisLock)
            {
                if (this.State != CommunicationState.Opening)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationObjectAbortedException(string.Format(SRServiceModel.DuplexChannelAbortedDuringOpen, this.Via)));
                }

                this.Connection = connection;
            }
        }

        private void SetRemoteSecurity(StreamUpgradeInitiator upgradeInitiator)
        {
            this.RemoteSecurity = StreamSecurityUpgradeInitiator.GetRemoteSecurity(upgradeInitiator);
        }

        protected override void PrepareMessage(Message message)
        {
            base.PrepareMessage(message);
        }

        internal class DuplexConnectionPoolHelper : ConnectionPoolHelper
        {
            private ClientFramingDuplexSessionChannel _channel;
            private ArraySegment<byte> _preamble;

            public DuplexConnectionPoolHelper(ClientFramingDuplexSessionChannel channel,
                ConnectionPool connectionPool, IConnectionInitiator connectionInitiator)
                : base(connectionPool, connectionInitiator, channel.Via)
            {
                _channel = channel;
                _preamble = channel.CreatePreamble();
            }

            protected override TimeoutException CreateNewConnectionTimeoutException(TimeSpan timeout, TimeoutException innerException)
            {
                return new TimeoutException(string.Format(SRServiceModel.OpenTimedOutEstablishingTransportSession,
                        timeout, _channel.Via.AbsoluteUri), innerException);
            }

            protected override IConnection AcceptPooledConnection(IConnection connection, ref TimeoutHelper timeoutHelper)
            {
                return _channel.SendPreamble(connection, _preamble, ref timeoutHelper);
            }

            protected override Task<IConnection> AcceptPooledConnectionAsync(IConnection connection, ref TimeoutHelper timeoutHelper)
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

            int size = await connection.ReadAsync(0,
                Math.Min(FaultStringDecoder.FaultSizeQuota, connection.AsyncReadBufferSize),
                timeoutHelper.RemainingTime());

            int offset = 0;
            while (size > 0)
            {
                int bytesDecoded = decoder.Decode(connection.AsyncReadBuffer, offset, size);
                offset += bytesDecoded;
                size -= bytesDecoded;

                if (decoder.CurrentState == ClientFramingDecoderState.Fault)
                {
                    ConnectionUtilities.CloseNoThrow(connection, timeoutHelper.RemainingTime());
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
                        size = await connection.ReadAsync(0,
                            Math.Min(FaultStringDecoder.FaultSizeQuota, connection.AsyncReadBufferSize),
                            timeoutHelper.RemainingTime());
                    }
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(decoder.CreatePrematureEOFException());
        }

        public static void DecodeFramingFault(ClientFramingDecoder decoder, IConnection connection,
            Uri via, string contentType, ref TimeoutHelper timeoutHelper)
        {
            ValidateReadingFaultString(decoder);

            int offset = 0;
            byte[] faultBuffer = Fx.AllocateByteArray(FaultStringDecoder.FaultSizeQuota);
            int size = connection.Read(faultBuffer, offset, faultBuffer.Length, timeoutHelper.RemainingTime());

            while (size > 0)
            {
                int bytesDecoded = decoder.Decode(faultBuffer, offset, size);
                offset += bytesDecoded;
                size -= bytesDecoded;

                if (decoder.CurrentState == ClientFramingDecoderState.Fault)
                {
                    ConnectionUtilities.CloseNoThrow(connection, timeoutHelper.RemainingTime());
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
                        size = connection.Read(faultBuffer, offset, faultBuffer.Length, timeoutHelper.RemainingTime());
                    }
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(decoder.CreatePrematureEOFException());
        }

        public static bool InitiateUpgrade(StreamUpgradeInitiator upgradeInitiator, ref IConnection connection,
            ClientFramingDecoder decoder, IDefaultCommunicationTimeouts defaultTimeouts, ref TimeoutHelper timeoutHelper)
        {
            string upgradeContentType = upgradeInitiator.GetNextUpgrade();

            while (upgradeContentType != null)
            {
                EncodedUpgrade encodedUpgrade = new EncodedUpgrade(upgradeContentType);
                // write upgrade request framing for synchronization
                connection.Write(encodedUpgrade.EncodedBytes, 0, encodedUpgrade.EncodedBytes.Length, true, timeoutHelper.RemainingTime());
                byte[] buffer = new byte[1];

                // read upgrade response framing 
                int size = connection.Read(buffer, 0, buffer.Length, timeoutHelper.RemainingTime());

                if (!ValidateUpgradeResponse(buffer, size, decoder)) // we have a problem
                {
                    return false;
                }

                // initiate wire upgrade
                ConnectionStream connectionStream = new ConnectionStream(connection, defaultTimeouts);
                Stream upgradedStream = upgradeInitiator.InitiateUpgrade(connectionStream);

                // and re-wrap connection
                connection = new StreamConnection(upgradedStream, connectionStream);

                upgradeContentType = upgradeInitiator.GetNextUpgrade();
            }

            return true;
        }

        public static async Task<bool> InitiateUpgradeAsync(StreamUpgradeInitiator upgradeInitiator, OutWrapper<IConnection> connectionWrapper,
            ClientFramingDecoder decoder, IDefaultCommunicationTimeouts defaultTimeouts, TimeSpan timeout)
        {
            IConnection connection = connectionWrapper.Value;
            string upgradeContentType = upgradeInitiator.GetNextUpgrade();

            while (upgradeContentType != null)
            {
                EncodedUpgrade encodedUpgrade = new EncodedUpgrade(upgradeContentType);
                // write upgrade request framing for synchronization
                await connection.WriteAsync(encodedUpgrade.EncodedBytes, 0, encodedUpgrade.EncodedBytes.Length, true, timeout);
                byte[] buffer = new byte[1];

                // read upgrade response framing 
                int size = await connection.ReadAsync(buffer, 0, buffer.Length, timeout);

                if (!ValidateUpgradeResponse(buffer, size, decoder)) // we have a problem
                {
                    return false;
                }

                // initiate wire upgrade
                ConnectionStream connectionStream = new ConnectionStream(connection, defaultTimeouts);
                Stream upgradedStream = await upgradeInitiator.InitiateUpgradeAsync(connectionStream);

                // and re-wrap connection
                connection = new StreamConnection(upgradedStream, connectionStream);
                connectionWrapper.Value = connection;

                upgradeContentType = upgradeInitiator.GetNextUpgrade();
            }

            return true;
        }

        private static void ValidateReadingFaultString(ClientFramingDecoder decoder)
        {
            if (decoder.CurrentState != ClientFramingDecoderState.ReadingFaultString)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new System.ServiceModel.Security.MessageSecurityException(
                    SRServiceModel.ServerRejectedUpgradeRequest));
            }
        }

        public static bool ValidatePreambleResponse(byte[] buffer, int count, ClientFramingDecoder decoder, Uri via)
        {
            if (count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(string.Format(SRServiceModel.ServerRejectedSessionPreamble, via),
                    decoder.CreatePrematureEOFException()));
            }

            // decode until the framing byte has been processed (it always will be)
            while (decoder.Decode(buffer, 0, count) == 0)
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRServiceModel.ServerRejectedUpgradeRequest, decoder.CreatePrematureEOFException()));
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
