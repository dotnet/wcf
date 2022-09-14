// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal delegate Task SendAsyncHandler(MessageAttemptInfo attemptInfo, TimeSpan timeout, bool maskUnhandledException);
    internal delegate void ComponentFaultedHandler(Exception faultException, WsrmFault fault);
    internal delegate void ComponentExceptionHandler(Exception exception);
    internal delegate Task RetryHandler(MessageAttemptInfo attemptInfo);

    internal sealed class ReliableOutputConnection
    {
        private static Func<object, Task> s_sendRetries = new Func<object, Task>(SendRetries);

        private UniqueId _id;
        private ReliableMessagingVersion _reliableMessagingVersion;
        private Guard _sendGuard = new Guard(int.MaxValue);
        private SendAsyncHandler _sendAsyncHandler;
        private OperationWithTimeoutAsyncCallback _sendAckRequestedAsyncHandler;
        private TimeSpan _sendTimeout;
        private InterruptibleWaitObject _shutdownHandle = new InterruptibleWaitObject(false);
        private bool _terminated = false;

        public ReliableOutputConnection(UniqueId id,
            int maxTransferWindowSize,
            MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion,
            TimeSpan initialRtt,
            bool requestAcks,
            TimeSpan sendTimeout)
        {
            _id = id;
            MessageVersion = messageVersion;
            _reliableMessagingVersion = reliableMessagingVersion;
            _sendTimeout = sendTimeout;
            Strategy = new TransmissionStrategy(reliableMessagingVersion, initialRtt, maxTransferWindowSize, requestAcks, id);
            Strategy.RetryTimeoutElapsed = OnRetryTimeoutElapsed;
            Strategy.OnException = RaiseOnException;
        }

        public ComponentFaultedHandler Faulted;
        public ComponentExceptionHandler OnException;

        private MessageVersion MessageVersion { get; }
        public bool Closed { get; private set; } = false;
        public long Last => Strategy.Last;
        public SendAsyncHandler SendAsyncHandler { set => _sendAsyncHandler = value; }
        public OperationWithTimeoutAsyncCallback SendAckRequestedAsyncHandler { set => _sendAckRequestedAsyncHandler = value; }
        public TransmissionStrategy Strategy { get; }
        private object ThisLock { get; } = new object();

        public void Abort(ChannelBase channel)
        {
            _sendGuard.Abort();
            _shutdownHandle.Abort(channel);
            Strategy.Abort(channel);
        }

        private Task CompleteTransferAsync(TimeSpan timeout)
        {
            if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                Message message = Message.CreateMessage(MessageVersion, WsrmFeb2005Strings.LastMessageAction);
                message.Properties.AllowOutputBatching = false;

                // Return value ignored.
                return InternalAddMessageAsync(message, timeout, null, true);
            }
            else if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (Strategy.SetLast())
                {
                    _shutdownHandle.Set();
                }
                else
                {
                    return _sendAckRequestedAsyncHandler(timeout);
                }
            }
            else
            {
                throw Fx.AssertAndThrow("Unsupported version.");
            }

            return Task.CompletedTask;
        }

        public Task<bool> AddMessageAsync(Message message, TimeSpan timeout, object state)
        {
            return InternalAddMessageAsync(message, timeout, state, false);
        }

        public bool CheckForTermination()
        {
            return Strategy.DoneTransmitting;
        }

        public async Task CloseAsync(TimeSpan timeout)
        {
            bool completeTransfer = false;

            lock (ThisLock)
            {
                completeTransfer = !Closed;
                Closed = true;
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            if (completeTransfer)
            {
                await CompleteTransferAsync(timeoutHelper.RemainingTime());
            }

            await _shutdownHandle.WaitAsync(timeoutHelper.RemainingTime());
            await _sendGuard.CloseAsync(timeoutHelper.RemainingTime());
            Strategy.Close();
        }

        public void Fault(ChannelBase channel)
        {
            _sendGuard.Abort();
            _shutdownHandle.Fault(channel);
            Strategy.Fault(channel);
        }

        private async Task<bool> InternalAddMessageAsync(Message message, TimeSpan timeout, object state, bool isLast)
        {
            MessageAttemptInfo attemptInfo;
            TimeoutHelper helper = new TimeoutHelper(timeout);

            try
            {
                if (isLast)
                {
                    if (state != null)
                    {
                        throw Fx.AssertAndThrow("The isLast overload does not take a state.");
                    }

                    attemptInfo = await Strategy.AddLastAsync(message, helper.RemainingTime(), null);
                }
                else
                {
                    bool success;
                    (attemptInfo, success) = await Strategy.AddAsync(message, helper.RemainingTime(), state);
                    if (!success)
                    {
                        return false;
                    }
                }
            }
            catch (TimeoutException)
            {
                if (isLast)
                    RaiseFault(null, SequenceTerminatedFault.CreateCommunicationFault(_id, SRP.SequenceTerminatedAddLastToWindowTimedOut, null));
                // else - RM does not fault the channel based on a timeout exception trying to add a sequenced message to the window.

                throw;
            }
            catch (Exception e)
            {
                if (!Fx.IsFatal(e))
                    RaiseFault(null, SequenceTerminatedFault.CreateCommunicationFault(_id, SRP.SequenceTerminatedUnknownAddToWindowError, null));

                throw;
            }

            if (_sendGuard.Enter())
            {
                try
                {
                    await _sendAsyncHandler(attemptInfo, helper.RemainingTime(), false);
                }
                catch (QuotaExceededException)
                {
                    RaiseFault(null, SequenceTerminatedFault.CreateQuotaExceededFault(_id));
                    throw;
                }
                finally
                {
                    _sendGuard.Exit();
                }
            }

            return true;
        }

        public bool IsFinalAckConsistent(SequenceRangeCollection ranges)
        {
            return Strategy.IsFinalAckConsistent(ranges);
        }

        private async Task OnRetryTimeoutElapsed(MessageAttemptInfo attemptInfo)
        {
            if (_sendGuard.Enter())
            {
                try
                {
                    await _sendAsyncHandler(attemptInfo, _sendTimeout, true);
                }
                finally
                {
                    _sendGuard.Exit();
                }
            }
        }

        private void OnTransferComplete()
        {
            Strategy.DequeuePending();

            if (Strategy.DoneTransmitting)
                Terminate();
        }

        public void ProcessTransferred(long transferred, SequenceRangeCollection ranges, int quotaRemaining)
        {
            if (transferred < 0)
            {
                throw Fx.AssertAndThrow("Argument transferred must be a valid sequence number or 0 for protocol messages.");
            }

            bool invalidAck;

            // ignored, TransmissionStrategy is being used to keep track of what must be re-sent.
            // In the Request-Reply case this state may not align with acks.
            bool inconsistentAck;

            Strategy.ProcessAcknowledgement(ranges, out invalidAck, out inconsistentAck);
            invalidAck = (invalidAck || ((transferred != 0) && !ranges.Contains(transferred)));

            if (!invalidAck)
            {
                if ((transferred > 0) && Strategy.ProcessTransferred(transferred, quotaRemaining))
                {
                    ActionItem.Schedule(s_sendRetries, this);
                }
                else
                {
                    OnTransferComplete();
                }
            }
            else
            {
                WsrmFault fault = new InvalidAcknowledgementFault(_id, ranges);
                RaiseFault(fault.CreateException(), fault);
            }
        }

        public void ProcessTransferred(SequenceRangeCollection ranges, int quotaRemaining)
        {
            bool invalidAck;
            bool inconsistentAck;

            Strategy.ProcessAcknowledgement(ranges, out invalidAck, out inconsistentAck);

            if (!invalidAck && !inconsistentAck)
            {
                if (Strategy.ProcessTransferred(ranges, quotaRemaining))
                {
                    ActionItem.Schedule(s_sendRetries, this);
                }
                else
                {
                    OnTransferComplete();
                }
            }
            else
            {
                WsrmFault fault = new InvalidAcknowledgementFault(_id, ranges);
                RaiseFault(fault.CreateException(), fault);
            }
        }

        private void RaiseFault(Exception faultException, WsrmFault fault)
        {
            ComponentFaultedHandler handler = Faulted;

            if (handler != null)
                handler(faultException, fault);
        }

        private void RaiseOnException(Exception exception)
        {
            OnException?.Invoke(exception);
        }

        private async Task SendRetries()
        {
            try
            {
                while (true)
                {
                    if (_sendGuard.Enter())
                    {
                        try
                        {
                            MessageAttemptInfo attemptInfo = Strategy.GetMessageInfoForRetry(false);
                            if (attemptInfo.Message == null)
                            {
                                break;
                            }

                            await _sendAsyncHandler(attemptInfo, _sendTimeout, true);
                        }
                        finally
                        {
                            _sendGuard.Exit();
                        }

                        Strategy.DequeuePending();
                        OnTransferComplete();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                RaiseOnException(e);
            }
        }

        private static Task SendRetries(object state)
        {
            ReliableOutputConnection outputConnection = (ReliableOutputConnection)state;
            return outputConnection.SendRetries();
        }

        public void Terminate()
        {
            lock (ThisLock)
            {
                if (_terminated)
                    return;

                _terminated = true;
            }

            _shutdownHandle.Set();
        }
    }
}
