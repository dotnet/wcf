// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Transactions;
using System.Threading.Tasks;
using System.Transactions;
using FxResources.System.ServiceModel.Primitives;

namespace System.ServiceModel.Channels
{
    internal interface ITransactionChannel
    {
        void WriteTransactionDataToMessage(Message message, MessageDirection direction);
        void ReadIssuedTokens(Message message, MessageDirection direction);
    }

    internal abstract class TransactionChannel<TChannel>
        : LayeredChannel<TChannel>, ITransactionChannel where TChannel : class, IChannel
    {
        private ITransactionChannelManager _factory;

        protected TransactionChannel(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            _factory = (ITransactionChannelManager)channelManager;

            if (_factory.TransactionProtocol == TransactionProtocol.OleTransactions)
            {
                Formatter = TransactionFormatter.OleTxFormatter;
            }
            else if (_factory.TransactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004)
            {
                Formatter = TransactionFormatter.WsatFormatter10;
            }
            else if (_factory.TransactionProtocol == TransactionProtocol.WSAtomicTransaction11)
            {
                Formatter = TransactionFormatter.WsatFormatter11;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentException(SRP.SFxBadTransactionProtocols));
            }
        }

        internal TransactionFormatter Formatter { get; }

        internal TransactionProtocol Protocol => _factory.TransactionProtocol;

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(FaultConverter))
            {
                return (T)(object)new TransactionChannelFaultConverter<TChannel>(this);
            }

            return base.GetProperty<T>();
        }

        public T GetInnerProperty<T>() where T : class
        {
            return base.InnerChannel.GetProperty<T>();
        }

        private ICollection<RequestSecurityTokenResponse> GetIssuedTokens(Message message)
        {
            return IssuedTokensHeader.ExtractIssuances(message, _factory.StandardsManager, message.Version.Envelope.UltimateDestinationActorValues, null);
        }

        private void FaultOnMessage(Message message, string reason, string codeString)
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(codeString, FaultCodeConstants.Namespaces.Transactions);
            FaultException fault = new FaultException(reason, code, FaultCodeConstants.Actions.Transactions);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(fault);
        }

        public void ReadIssuedTokens(Message message, MessageDirection direction)
        {
            TransactionFlowOption option = _factory.FlowIssuedTokens;

            ICollection<RequestSecurityTokenResponse> issuances = GetIssuedTokens(message);

            if (issuances != null && issuances.Count != 0)
            {
                if (option == TransactionFlowOption.NotAllowed)
                {
                    FaultOnMessage(message, SRP.IssuedTokenFlowNotAllowed, FaultCodeConstants.Codes.IssuedTokenFlowNotAllowed);
                }

                foreach (RequestSecurityTokenResponse rstr in issuances)
                {
                    TransactionFlowProperty.Ensure(message).IssuedTokens.Add(rstr);
                }
            }
        }

        private void ReadTransactionFromMessage(Message message, TransactionFlowOption txFlowOption)
        {
            TransactionInfo transactionInfo = null;
            try
            {
                transactionInfo = Formatter.ReadTransaction(message);
            }
            catch (TransactionException e)
            {
                DiagnosticUtility.TraceHandledException(e, TraceEventType.Error);
                FaultOnMessage(message, SRP.Format(SRP.SFxTransactionDeserializationFailed, e.Message), FaultCodeConstants.Codes.TransactionHeaderMalformed);
            }

            if (transactionInfo != null)
            {
                TransactionMessageProperty.Set(transactionInfo, message);
            }
            else if (txFlowOption == TransactionFlowOption.Mandatory)
            {
                FaultOnMessage(message, SRP.SFxTransactionFlowRequired, FaultCodeConstants.Codes.TransactionHeaderMissing);
            }
        }

        public virtual void ReadTransactionDataFromMessage(Message message, MessageDirection direction)
        {
            ReadIssuedTokens(message, direction);

            TransactionFlowOption txFlowOption = _factory.GetTransaction(direction, message.Headers.Action);
            if (TransactionFlowOptionHelper.AllowedOrRequired(txFlowOption))
            {
                ReadTransactionFromMessage(message, txFlowOption);
            }
        }

        public void WriteTransactionDataToMessage(Message message, MessageDirection direction)
        {
            TransactionFlowOption txFlowOption = _factory.GetTransaction(direction, message.Headers.Action);
            if (TransactionFlowOptionHelper.AllowedOrRequired(txFlowOption))
            {
                WriteTransactionToMessage(message, txFlowOption);
            }

            if (TransactionFlowOptionHelper.AllowedOrRequired(_factory.FlowIssuedTokens))
            {
                WriteIssuedTokens(message, direction);
            }
        }

        private void WriteTransactionToMessage(Message message, TransactionFlowOption txFlowOption)
        {
            Transaction transaction = TransactionFlowProperty.TryGetTransaction(message);

            if (transaction != null)
            {
                try
                {
                    Formatter.WriteTransaction(transaction, message);
                }
                catch (TransactionException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(e.Message, e));
                }
            }
            else if (txFlowOption == TransactionFlowOption.Mandatory)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SRP.SFxTransactionFlowRequired));
            }
        }

        private void WriteIssuedTokens(Message message, MessageDirection direction)
        {
            ICollection<RequestSecurityTokenResponse> issuances = TransactionFlowProperty.TryGetIssuedTokens(message);
            if (issuances != null)
            {
                IssuedTokensHeader header = new IssuedTokensHeader(issuances, _factory.StandardsManager);
                message.Headers.Add(header);
            }
        }
    }

    internal class TransactionOutputChannelGeneric<TChannel> : TransactionChannel<TChannel>, IOutputChannel, IAsyncOutputChannel where TChannel : class, IOutputChannel
    {
        public TransactionOutputChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel)
        {
        }

        public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;

        public Uri Via => InnerChannel.Via;

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            InnerChannel.Send(message, timeoutHelper.RemainingTime());
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            return InnerChannel.BeginSend(message, timeoutHelper.RemainingTime(), callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            InnerChannel.EndSend(result);
        }

        public Task SendAsync(Message message)
        {
            return SendAsync(message, DefaultSendTimeout);
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            if (InnerChannel is IAsyncOutputChannel asyncChannel)
            {
                return asyncChannel.SendAsync(message, timeoutHelper.RemainingTime());
            }
            else
            {
                return Task.Factory.FromAsync(InnerChannel.BeginSend, InnerChannel.EndSend, message, timeoutHelper.RemainingTime(), null);
            }
        }
    }

    internal class TransactionRequestChannelGeneric<TChannel> : TransactionChannel<TChannel>, IRequestChannel, IAsyncRequestChannel where TChannel : class, IRequestChannel
    {
        public TransactionRequestChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel)
        {
        }

        public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;

        public Uri Via => InnerChannel.Via;

        public Message Request(Message message)
        {
            return Request(message, DefaultSendTimeout);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            Message reply = InnerChannel.Request(message, timeoutHelper.RemainingTime());
            if (reply != null)
                ReadIssuedTokens(reply, MessageDirection.Output);
            return reply;
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            return BeginRequest(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            return InnerChannel.BeginRequest(message, timeoutHelper.RemainingTime(), callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            Message reply = InnerChannel.EndRequest(result);
            if (reply != null)
                ReadIssuedTokens(reply, MessageDirection.Output);
            return reply;
        }

        public Task<Message> RequestAsync(Message message)
        {
            return RequestAsync(message, DefaultSendTimeout);
        }

        public async Task<Message> RequestAsync(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, MessageDirection.Input);
            Message reply;
            if (InnerChannel is IAsyncRequestChannel asyncChannel)
            {
                reply = await asyncChannel.RequestAsync(message, timeoutHelper.RemainingTime());
            }
            else
            {
                reply = await Task.Factory.FromAsync(
                    InnerChannel.BeginRequest, InnerChannel.EndRequest, message, timeoutHelper.RemainingTime(), null);
            }
            if (reply != null)
                ReadIssuedTokens(reply, MessageDirection.Output);
            return reply;
        }
    }

    internal class TransactionDuplexChannelGeneric<TChannel> : TransactionReceiveChannelGeneric<TChannel>, IDuplexChannel, IAsyncDuplexChannel where TChannel : class, IDuplexChannel
    {
        private MessageDirection _sendMessageDirection;

        public TransactionDuplexChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel, MessageDirection direction)
            : base(channelManager, innerChannel, direction)
        {
            if (direction == MessageDirection.Input)
            {
                _sendMessageDirection = MessageDirection.Output;
            }
            else
            {
                _sendMessageDirection = MessageDirection.Input;
            }
        }

        public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;

        public Uri Via => InnerChannel.Via;

        public override void ReadTransactionDataFromMessage(Message message, MessageDirection direction)
        {
            try
            {
                base.ReadTransactionDataFromMessage(message, direction);
            }
            catch (FaultException fault)
            {
                Message reply = Message.CreateMessage(message.Version, fault.CreateMessageFault(), fault.Action);

                RequestReplyCorrelator.AddressReply(reply, message);
                RequestReplyCorrelator.PrepareReply(reply, message.Headers.MessageId);

                try
                {
                    Send(reply);
                }
                finally
                {
                    reply.Close();
                }

                throw;
            }
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, _sendMessageDirection);
            InnerChannel.Send(message, timeoutHelper.RemainingTime());
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, _sendMessageDirection);
            return InnerChannel.BeginSend(message, timeoutHelper.RemainingTime(), callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            InnerChannel.EndSend(result);
        }

        public Task SendAsync(Message message)
        {
            return SendAsync(message, DefaultSendTimeout);
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            WriteTransactionDataToMessage(message, _sendMessageDirection);
            if (InnerChannel is IAsyncOutputChannel asyncChannel)
            {
                return asyncChannel.SendAsync(message, timeoutHelper.RemainingTime());
            }
            else
            {
                return Task.Factory.FromAsync(InnerChannel.BeginSend, InnerChannel.EndSend, message, timeoutHelper.RemainingTime(), null);
            }
        }
    }

    internal class TransactionReceiveChannelGeneric<TChannel> : TransactionChannel<TChannel>, IInputChannel, IAsyncInputChannel
    where TChannel : class, IInputChannel
    {
        private MessageDirection _receiveMessageDirection;

        public TransactionReceiveChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel, MessageDirection direction)
            : base(channelManager, innerChannel)
        {
            _receiveMessageDirection = direction;
        }

        public EndpointAddress LocalAddress => InnerChannel.LocalAddress;

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            return InputChannel.HelpReceive(this, timeout);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InputChannel.HelpReceiveAsync(this, timeout).ToApm(callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginTryReceive(timeout, callback, state);
        }

        public virtual bool EndTryReceive(IAsyncResult asyncResult, out Message message)
        {
            if (!InnerChannel.EndTryReceive(asyncResult, out message))
            {
                return false;
            }

            if (message != null)
            {
                ReadTransactionDataFromMessage(message, _receiveMessageDirection);
            }

            return true;
        }

        public virtual bool TryReceive(TimeSpan timeout, out Message message)
        {
            if (!InnerChannel.TryReceive(timeout, out message))
            {
                return false;
            }

            if (message != null)
            {
                ReadTransactionDataFromMessage(message, _receiveMessageDirection);
            }

            return true;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return InnerChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return InnerChannel.EndWaitForMessage(result);
        }

        public Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            if (InnerChannel is IAsyncInputChannel asyncInputChannel)
            {
                return asyncInputChannel.WaitForMessageAsync(timeout);
            }
            else
            {
                return Task.Factory.FromAsync(BeginWaitForMessage, EndWaitForMessage, timeout, null);
            }
        }

        public Task<Message> ReceiveAsync()
        {
            return ReceiveAsync(DefaultReceiveTimeout);
        }

        public Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            if (InnerChannel is IAsyncInputChannel asyncChannel)
            {
                return InputChannel.HelpReceiveAsync(asyncChannel, timeout);
            }
            else
            {
                return Task.Factory.FromAsync(BeginReceive, EndReceive, timeout, null);
            }
        }

        public Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
        {
            if (InnerChannel is IAsyncInputChannel asyncChannel)
            {
                return asyncChannel.TryReceiveAsync(timeout);
            }
            else
            {
                return TaskHelpers.FromAsync<TimeSpan, bool, Message>(BeginTryReceive, EndTryReceive, timeout, null);
            }
        }
    }

    internal class TransactionOutputDuplexChannelGeneric<TChannel> : TransactionDuplexChannelGeneric<TChannel> where TChannel : class, IDuplexChannel
    {
        public TransactionOutputDuplexChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel, MessageDirection.Output)
        {
        }
    }
}
