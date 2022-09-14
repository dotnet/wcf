// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal delegate Task OperationWithTimeoutAsyncCallback(TimeSpan timeout);

    internal sealed class Guard
    {
        private TaskCompletionSource<object> _tcs;
        private int _currentCount = 0;
        private int _maxCount;
        private bool _closed;
        private object _thisLock = new object();

        public Guard() : this(1) { }

        public Guard(int maxCount)
        {
            _maxCount = maxCount;
        }

        public void Abort()
        {
            _closed = true;
        }

        public async Task CloseAsync(TimeSpan timeout)
        {
            lock (_thisLock)
            {
                if (_closed)
                    return;

                _closed = true;

                if (_currentCount > 0)
                    _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            if (_tcs != null)
            {
                try
                {
                    if (!await _tcs.Task.AwaitWithTimeout(timeout))
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRP.Format(SRP.TimeoutOnOperation, timeout)));
                }
                finally
                {
                    lock (_thisLock)
                    {
                        _tcs.TrySetResult(null);
                        _tcs = null;
                    }
                }
            }
        }

        public bool Enter()
        {
            lock (_thisLock)
            {
                if (_closed)
                    return false;

                if (_currentCount == _maxCount)
                    return false;

                _currentCount++;
                return true;
            }
        }

        public void Exit()
        {
            lock (_thisLock)
            {
                _currentCount--;

                if (_currentCount < 0)
                {
                    throw Fx.AssertAndThrow("Exit can only be called after Enter.");
                }

                if (_currentCount == 0)
                {
                    if (_tcs != null)
                    {
                        Fx.AssertAndThrow(!_tcs.Task.IsCompleted, "TCS should not have already been completed");
                        _tcs.TrySetResult(null);
                    }
                }
            }
        }
    }

    internal class InterruptibleTimer
    {
        public delegate Task AsyncWaitCallback(object state);
        private WaitCallback _callback;
        private AsyncWaitCallback _asyncCallback;
        private bool _aborted = false;
        private TimeSpan _defaultInterval;
        private static Action<object> s_onTimerElapsed = new Action<object>(OnTimerElapsed);
        private static Func<object, Task> s_onTimerElapsedAsync = new Func<object, Task>(OnTimerElapsedAsync);
        private bool _set = false;
        private object _state;
        private IOThreadTimer _timer;
        private bool _isAsync;

        public InterruptibleTimer(TimeSpan defaultInterval, WaitCallback callback, object state) : this(defaultInterval, callback, null, state)
        {
            if (callback == null)
            {
                throw Fx.AssertAndThrow("Argument callback cannot be null.");
            }

            _isAsync = false;
        }

        public InterruptibleTimer(TimeSpan defaultInterval, AsyncWaitCallback callback, object state) : this(defaultInterval, null, callback, state)
        {
            if (callback == null)
            {
                throw Fx.AssertAndThrow("Argument callback cannot be null.");
            }

            _isAsync = true;
        }

        private InterruptibleTimer(TimeSpan defaultInterval, WaitCallback callback, AsyncWaitCallback asyncCallback, object state)
        {
            _defaultInterval = defaultInterval;
            _callback = callback;
            _asyncCallback = asyncCallback;
            _state = state;
        }


        private object ThisLock { get; } = new object();

        public void Abort()
        {
            lock (ThisLock)
            {
                _aborted = true;

                if (_set)
                {
                    _timer.Cancel();
                    _set = false;
                }
            }
        }

        public bool Cancel()
        {
            lock (ThisLock)
            {
                if (_aborted)
                {
                    return false;
                }

                if (_set)
                {
                    _timer.Cancel();
                    _set = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void OnTimerElapsed()
        {
            lock (ThisLock)
            {
                if (_aborted)
                    return;

                _set = false;
            }

            _callback(_state);
        }

        private Task OnTimerElapsedAsync()
        {
            lock (ThisLock)
            {
                if (_aborted)
                    return Task.CompletedTask;

                _set = false;
            }

            return _asyncCallback(_state);
        }

        private static void OnTimerElapsed(object state)
        {
            InterruptibleTimer interruptibleTimer = (InterruptibleTimer)state;
            interruptibleTimer.OnTimerElapsed();
        }

        private static Task OnTimerElapsedAsync(object state)
        {
            InterruptibleTimer interruptibleTimer = (InterruptibleTimer)state;
            return interruptibleTimer.OnTimerElapsedAsync();
        }

        public void Set()
        {
            Set(_defaultInterval);
        }

        public void Set(TimeSpan interval)
        {
            InternalSet(interval, false);
        }

        public void SetIfNotSet()
        {
            InternalSet(_defaultInterval, true);
        }

        private void InternalSet(TimeSpan interval, bool ifNotSet)
        {
            lock (ThisLock)
            {
                if (_aborted || (ifNotSet && _set))
                    return;

                if (_timer == null)
                {
                    if (_isAsync)
                        _timer = new IOThreadTimer(s_onTimerElapsedAsync, this, true);
                    else
                        _timer = new IOThreadTimer(s_onTimerElapsed, this, true);
                }

                _timer.Set(interval);
                _set = true;
            }
        }
    }

    internal class InterruptibleWaitObject
    {
        private bool _aborted = false;
        private CommunicationObject _communicationObject;
        private bool _set;
        private int _syncWaiters;
        private object _thisLock = new object();
        private bool _throwTimeoutByDefault = true;
        private TaskCompletionSource<object> _tcs;

        public InterruptibleWaitObject(bool signaled)
            : this(signaled, true)
        {
        }

        public InterruptibleWaitObject(bool signaled, bool throwTimeoutByDefault)
        {
            _set = signaled;
            _throwTimeoutByDefault = throwTimeoutByDefault;
        }

        public void Abort(CommunicationObject communicationObject)
        {
            if (communicationObject == null)
            {
                throw Fx.AssertAndThrow("Argument communicationObject cannot be null.");
            }

            lock (_thisLock)
            {
                if (_aborted)
                {
                    return;
                }

                _communicationObject = communicationObject;

                _aborted = true;
                InternalSet();
            }
        }

        public void Fault(CommunicationObject communicationObject)
        {
            if (communicationObject == null)
            {
                throw Fx.AssertAndThrow("Argument communicationObject cannot be null.");
            }

            lock (_thisLock)
            {
                if (_aborted)
                {
                    return;
                }

                _communicationObject = communicationObject;

                _aborted = false;
                InternalSet();
            }
        }

        private Exception GetException()
        {
            if (_communicationObject == null)
            {
                Fx.Assert("Caller is attempting to retrieve an exception from a null communicationObject.");
            }

            return _aborted
                ? _communicationObject.CreateAbortedException()
                : _communicationObject.GetTerminalException();
        }

        private void InternalSet()
        {
            lock (_thisLock)
            {
                _set = true;

                if (_tcs != null)
                {
                    _tcs.TrySetResult(null);
                }
            }
        }

        public void Reset()
        {
            lock (_thisLock)
            {
                _communicationObject = null;
                _aborted = false;
                _set = false;

                if (_tcs != null && _tcs.Task.IsCompleted)
                {
                    _tcs = new TaskCompletionSource<object>();
                }
            }
        }

        public void Set()
        {
            InternalSet();
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return WaitAsync(timeout, _throwTimeoutByDefault);
        }

        public async Task<bool> WaitAsync(TimeSpan timeout, bool throwTimeoutException)
        {
            lock (_thisLock)
            {
                if (_set)
                {
                    if (_communicationObject != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(GetException());
                    }

                    return true;
                }

                if (_tcs == null)
                {
                    _tcs = new TaskCompletionSource<object>();
                }

                _syncWaiters++;
            }

            try
            {
                if (!await _tcs.Task.AwaitWithTimeout(timeout))
                {
                    if (throwTimeoutException)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRP.Format(SRP.TimeoutOnOperation, timeout)));
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                lock (_thisLock)
                {
                    // Last one out turns off the light.
                    _syncWaiters--;
                    if (_syncWaiters == 0 && _tcs.Task.IsCompleted)
                    {
                        _tcs = null;
                    }
                }
            }

            if (_communicationObject != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(GetException());
            }

            return true;
        }
    }

    internal abstract class FaultHelper
    {
        protected FaultHelper()
        {
        }

        protected object ThisLock { get; } = new object();

        public abstract void Abort();

        public static bool AddressReply(Message message, Message faultMessage)
        {
            try
            {
                RequestReplyCorrelator.PrepareReply(faultMessage, message);
            }
            catch (MessageHeaderException exception)
            {
                // ---- it - we don't need to correlate the reply if the MessageId header is bad
                if (DiagnosticUtility.ShouldTraceInformation)
                    DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);
            }

            bool sendFault = true;
            try
            {
                sendFault = RequestReplyCorrelator.AddressReply(faultMessage, message);
            }
            catch (MessageHeaderException exception)
            {
                // ---- it - we don't need to address the reply if the addressing headers are bad
                if (DiagnosticUtility.ShouldTraceInformation)
                    DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);
            }

            return sendFault;
        }

        public abstract Task CloseAsync(TimeSpan timeout);
        public abstract Task SendFaultAsync(IReliableChannelBinder binder, RequestContext requestContext, Message faultMessage);
    }

    internal abstract class TypedFaultHelper<TState> : FaultHelper
    {
        private InterruptibleWaitObject _closeHandle;
        private TimeSpan _defaultCloseTimeout;
        private TimeSpan _defaultSendTimeout;
        private Dictionary<IReliableChannelBinder, TState> _faultList = new Dictionary<IReliableChannelBinder, TState>();

        protected TypedFaultHelper(TimeSpan defaultSendTimeout, TimeSpan defaultCloseTimeout)
        {
            _defaultSendTimeout = defaultSendTimeout;
            _defaultCloseTimeout = defaultCloseTimeout;
        }

        public override void Abort()
        {
            Dictionary<IReliableChannelBinder, TState> tempFaultList;
            InterruptibleWaitObject tempCloseHandle;

            lock (ThisLock)
            {
                tempFaultList = _faultList;
                _faultList = null;
                tempCloseHandle = _closeHandle;
            }

            if ((tempFaultList == null) || (tempFaultList.Count == 0))
            {
                if (tempCloseHandle != null)
                    tempCloseHandle.Set();
                return;
            }

            foreach (KeyValuePair<IReliableChannelBinder, TState> pair in tempFaultList)
            {
                AbortState(pair.Value, true);
                pair.Key.Abort();
            }

            if (tempCloseHandle != null)
                tempCloseHandle.Set();
        }

        private void AbortBinder(IReliableChannelBinder binder)
        {
            try
            {
                binder.Abort();
            }
            finally
            {
                RemoveBinder(binder);
            }
        }

        private async Task AsyncCloseBinder(IReliableChannelBinder binder)
        {
            try
            {
                try
                {
                    await binder.CloseAsync(_defaultCloseTimeout);
                }
                finally
                {
                    RemoveBinder(binder);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                binder.HandleException(e);
            }
        }

        protected abstract void AbortState(TState state, bool isOnAbortThread);

        private void AfterClose()
        {
            Abort();
        }

        private bool BeforeClose()
        {
            lock (ThisLock)
            {
                if ((_faultList == null) || (_faultList.Count == 0))
                    return true;

                _closeHandle = new InterruptibleWaitObject(false, false);
            }

            return false;
        }

        public override async Task CloseAsync(TimeSpan timeout)
        {
            if (BeforeClose())
                return;

            await _closeHandle.WaitAsync(timeout);
            AfterClose();
        }

        protected abstract Task SendFaultAsync(IReliableChannelBinder binder, TState state, TimeSpan timeout);

        protected abstract TState GetState(RequestContext requestContext, Message faultMessage);

        protected void RemoveBinder(IReliableChannelBinder binder)
        {
            InterruptibleWaitObject tempCloseHandle;

            lock (ThisLock)
            {
                if (_faultList == null)
                    return;

                _faultList.Remove(binder);
                if ((_closeHandle == null) || (_faultList.Count > 0))
                    return;

                // Close has been called.
                _faultList = null;
                tempCloseHandle = _closeHandle;
            }

            tempCloseHandle.Set();
        }

        protected async Task SendFaultAsync(IReliableChannelBinder binder, TState state)
        {
            bool throwing = true;

            try
            {
                await SendFaultAsync(binder, state, _defaultSendTimeout);
                await AsyncCloseBinder(binder);
                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    AbortState(state, false);
                    AbortBinder(binder);
                }
            }
        }

        public override async Task SendFaultAsync(IReliableChannelBinder binder, RequestContext requestContext, Message faultMessage)
        {
            try
            {
                bool abort = true;
                TState state = GetState(requestContext, faultMessage);

                lock (ThisLock)
                {
                    if (_faultList != null)
                    {
                        abort = false;
                        _faultList.Add(binder, state);
                    }
                }

                if (abort)
                {
                    AbortState(state, false);
                    binder.Abort();
                }

                await TaskHelpers.EnsureDefaultTaskScheduler();
                await SendFaultAsync(binder, state);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                binder.HandleException(e);
            }
        }
    }

    internal struct FaultState
    {
        public FaultState(RequestContext requestContext, Message faultMessage)
        {
            RequestContext = requestContext;
            FaultMessage = faultMessage;
        }

        public Message FaultMessage { get; }
        public RequestContext RequestContext { get; }
    }

    internal class ReplyFaultHelper : TypedFaultHelper<FaultState>
    {
        public ReplyFaultHelper(TimeSpan defaultSendTimeout, TimeSpan defaultCloseTimeout)
            : base(defaultSendTimeout, defaultCloseTimeout)
        {
        }

        protected override void AbortState(FaultState faultState, bool isOnAbortThread)
        {
            // if abort is true, the request could be in the middle of the encoding step, let the sending thread clean up.
            if (!isOnAbortThread)
            {
                faultState.FaultMessage.Close();
            }

            faultState.RequestContext.Abort();
        }

        protected override async Task SendFaultAsync(IReliableChannelBinder binder, FaultState faultState, TimeSpan timeout)
        {
            var context = faultState.RequestContext;
            await Task.Factory.FromAsync(context.BeginReply, context.EndReply, faultState.FaultMessage, timeout, null);
            faultState.FaultMessage.Close();
        }

        protected override FaultState GetState(RequestContext requestContext, Message faultMessage)
        {
            return new FaultState(requestContext, faultMessage);
        }

    }

    internal class SendFaultHelper : TypedFaultHelper<Message>
    {
        public SendFaultHelper(TimeSpan defaultSendTimeout, TimeSpan defaultCloseTimeout)
            : base(defaultSendTimeout, defaultCloseTimeout)
        {
        }

        protected override void AbortState(Message message, bool isOnAbortThread)
        {
            // if abort is true, the request could be in the middle of the encoding step, let the sending thread clean up.
            if (!isOnAbortThread)
            {
                message.Close();
            }
        }

        protected override async Task SendFaultAsync(IReliableChannelBinder binder, Message message, TimeSpan timeout)
        {
            await binder.SendAsync(message, timeout);
            message.Close();
        }

        protected override Message GetState(RequestContext requestContext, Message faultMessage)
        {
            return faultMessage;
        }
    }

    internal static class ReliableMessagingConstants
    {
        static public TimeSpan UnknownInitiationTime = TimeSpan.FromSeconds(2);
        static public TimeSpan RequestorIterationTime = TimeSpan.FromSeconds(10);
        static public TimeSpan RequestorReceiveTime = TimeSpan.FromSeconds(10);
        static public int MaxSequenceRanges = 128;
    }

    // This class and its derivates attempt to unify 3 similar request reply patterns.
    // 1. Straightforward R/R pattern
    // 2. R/R pattern with binder and exception semantics on Open (CreateSequence)
    // 3. TerminateSequence request - TerminateSequence response for R(Request|Reply)SC
    internal abstract class ReliableRequestor
    {
        private InterruptibleWaitObject abortHandle = new InterruptibleWaitObject(false, false);
        private IReliableChannelBinder binder;
        private bool isCreateSequence;
        private ActionHeader messageAction;
        private BodyWriter messageBody;
        private WsrmMessageHeader messageHeader;
        private UniqueId messageId;
        private MessageVersion messageVersion;
        private TimeSpan originalTimeout;
        private string timeoutString1Index;

        public IReliableChannelBinder Binder
        {
            protected get { return binder; }
            set { binder = value; }
        }

        public bool IsCreateSequence
        {
            protected get { return isCreateSequence; }
            set { isCreateSequence = value; }
        }

        public ActionHeader MessageAction
        {
            set { messageAction = value; }
        }

        public BodyWriter MessageBody
        {
            set { messageBody = value; }
        }

        public UniqueId MessageId
        {
            get { return messageId; }
        }

        public WsrmMessageHeader MessageHeader
        {
            get { return messageHeader; }
            set { messageHeader = value; }
        }

        public MessageVersion MessageVersion
        {
            set { messageVersion = value; }
        }

        public string TimeoutString1Index
        {
            set { timeoutString1Index = value; }
        }

        public void Abort(CommunicationObject communicationObject)
        {
            abortHandle.Abort(communicationObject);
        }

        private Message CreateRequestMessage()
        {
            Message request = Message.CreateMessage(messageVersion, messageAction, messageBody);
            request.Properties.AllowOutputBatching = false;

            if (messageHeader != null)
            {
                request.Headers.Insert(0, messageHeader);
            }

            if (messageId != null)
            {
                request.Headers.MessageId = messageId;
                RequestReplyCorrelator.PrepareRequest(request);

                EndpointAddress address = binder.LocalAddress;

                if (address == null)
                {
                    request.Headers.ReplyTo = null;
                }
                else if (messageVersion.Addressing == AddressingVersion.WSAddressingAugust2004)
                {
                    request.Headers.ReplyTo = address;
                }
                else if (messageVersion.Addressing == AddressingVersion.WSAddressing10)
                {
                    request.Headers.ReplyTo = address.IsAnonymous ? null : address;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ProtocolException(SRP.Format(SRP.AddressingVersionNotSupported, messageVersion.Addressing)));
                }
            }

            return request;
        }

        private Task<bool> EnsureChannelAsync()
        {
            if (IsCreateSequence)
            {
                IClientReliableChannelBinder clientBinder = (IClientReliableChannelBinder)binder;
                return clientBinder.EnsureChannelForRequestAsync();
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        public virtual void Fault(CommunicationObject communicationObject)
        {
            abortHandle.Fault(communicationObject);
        }

        public abstract WsrmMessageInfo GetInfo();

        private TimeSpan GetNextRequestTimeout(TimeSpan remainingTimeout, out TimeoutHelper iterationTimeout, out bool lastIteration)
        {
            iterationTimeout = new TimeoutHelper(ReliableMessagingConstants.RequestorIterationTime);
            lastIteration = remainingTimeout <= iterationTimeout.RemainingTime();
            return remainingTimeout;
        }

        private bool HandleException(Exception exception, bool lastIteration)
        {
            if (IsCreateSequence)
            {
                if (exception is QuotaExceededException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(exception.Message, exception));
                }

                if (!binder.IsHandleable(exception)
                    || exception is MessageSecurityException
                    || exception is SecurityNegotiationException
                    || exception is SecurityAccessDeniedException
                    || (binder.State != CommunicationState.Opened)
                    || lastIteration)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return binder.IsHandleable(exception);
            }
        }

        private void ThrowTimeoutException()
        {
            if (timeoutString1Index != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TimeoutException(SRP.Format(timeoutString1Index, originalTimeout)));
            }
        }

        protected abstract Task<Message> OnRequestAsync(Message request, TimeSpan timeout, bool last);

        public async Task<Message> RequestAsync(TimeSpan timeout)
        {
            originalTimeout = timeout;
            TimeoutHelper timeoutHelper = new TimeoutHelper(originalTimeout);
            TimeoutHelper iterationTimeoutHelper;
            bool lastIteration;

            while (true)
            {
                Message request = null;
                Message reply = null;
                bool requestCompleted = false;
                TimeSpan requestTimeout = GetNextRequestTimeout(timeoutHelper.RemainingTime(),
                    out iterationTimeoutHelper, out lastIteration);

                try
                {
                    if (await EnsureChannelAsync())
                    {
                        request = CreateRequestMessage();
                        reply = await OnRequestAsync(request, requestTimeout, lastIteration);
                        requestCompleted = true;
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e) || !HandleException(e, lastIteration))
                    {
                        throw;
                    }

                    DiagnosticUtility.TraceHandledException(e, TraceEventType.Information);
                }
                finally
                {
                    if (request != null)
                    {
                        request.Close();
                    }
                }

                if (requestCompleted)
                {
                    if (ValidateReply(reply))
                    {
                        return reply;
                    }
                }

                if (lastIteration)
                    break;

                await abortHandle.WaitAsync(iterationTimeoutHelper.RemainingTime());
            }

            ThrowTimeoutException();
            return null;
        }

        public abstract void SetInfo(WsrmMessageInfo info);

        public void SetRequestResponsePattern()
        {
            if (messageId != null)
            {
                throw Fx.AssertAndThrow("Initialize messageId only once.");
            }

            messageId = new UniqueId();
        }

        private bool ValidateReply(Message response)
        {
            if (messageId != null)
            {
                // r/r pattern requires a response
                return response != null;
            }
            else
            {
                return true;
            }
        }
    }

    internal sealed class RequestReliableRequestor : ReliableRequestor
    {
        private bool replied = false;
        private WsrmMessageInfo replyInfo;
        private object thisLock = new object();

        private IClientReliableChannelBinder ClientBinder
        {
            get { return (IClientReliableChannelBinder)Binder; }
        }

        private object ThisLock
        {
            get { return thisLock; }
        }

        public override WsrmMessageInfo GetInfo()
        {
            return replyInfo;
        }

        private Message GetReply(Message reply, bool last)
        {
            lock (ThisLock)
            {
                if (reply != null && replyInfo != null)
                {
                    replyInfo = null;
                }
                else if (reply == null && replyInfo != null)
                {
                    reply = replyInfo.Message;
                }

                if (reply != null || last)
                {
                    replied = true;
                }
            }

            return reply;
        }

        protected override async Task<Message> OnRequestAsync(Message request, TimeSpan timeout, bool last)
        {
            return GetReply(await ClientBinder.RequestAsync(request, timeout, MaskingMode.None), last);
        }

        public override void SetInfo(WsrmMessageInfo info)
        {
            lock (ThisLock)
            {
                if (!replied && replyInfo == null)
                {
                    replyInfo = info;
                }
            }
        }
    }

    internal sealed class SendReceiveReliableRequestor : ReliableRequestor
    {
        private bool timeoutIsSafe;

        public bool TimeoutIsSafe
        {
            set { timeoutIsSafe = value; }
        }

        public override WsrmMessageInfo GetInfo()
        {
            throw Fx.AssertAndThrow("Not Supported.");
        }

        private TimeSpan GetReceiveTimeout(TimeSpan timeoutRemaining)
        {
            if ((timeoutRemaining < ReliableMessagingConstants.RequestorReceiveTime) || !timeoutIsSafe)
            {
                return timeoutRemaining;
            }
            else
            {
                return ReliableMessagingConstants.RequestorReceiveTime;
            }
        }

        protected override async Task<Message> OnRequestAsync(Message request, TimeSpan timeout, bool last)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            await Binder.SendAsync(request, timeoutHelper.RemainingTime(), MaskingMode.None);
            TimeSpan receiveTimeout = GetReceiveTimeout(timeoutHelper.RemainingTime());

            RequestContext requestContext;
            (_, requestContext) = await Binder.TryReceiveAsync(receiveTimeout, MaskingMode.None);
            return requestContext?.RequestMessage;
        }

        public override void SetInfo(WsrmMessageInfo info)
        {
            throw Fx.AssertAndThrow("Not Supported.");
        }
    }

    internal sealed class SendWaitReliableRequestor : ReliableRequestor
    {
        private bool replied = false;
        private InterruptibleWaitObject replyHandle = new InterruptibleWaitObject(false, true);
        private WsrmMessageInfo replyInfo;
        private object thisLock = new object();

        private object ThisLock
        {
            get { return thisLock; }
        }

        public override void Fault(CommunicationObject communicationObject)
        {
            replied = true;
            replyHandle.Fault(communicationObject);
            base.Fault(communicationObject);
        }

        public override WsrmMessageInfo GetInfo()
        {
            return replyInfo;
        }

        private Message GetReply(bool last)
        {
            lock (ThisLock)
            {
                if (replyInfo != null)
                {
                    replied = true;
                    return replyInfo.Message;
                }
                else if (last)
                {
                    replied = true;
                }
            }

            return null;
        }

        private TimeSpan GetWaitTimeout(TimeSpan timeoutRemaining)
        {
            if ((timeoutRemaining < ReliableMessagingConstants.RequestorReceiveTime))
            {
                return timeoutRemaining;
            }
            else
            {
                return ReliableMessagingConstants.RequestorReceiveTime;
            }
        }

        protected override async Task<Message> OnRequestAsync(Message request, TimeSpan timeout, bool last)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await Binder.SendAsync(request, timeoutHelper.RemainingTime(), MaskingMode.None);
            TimeSpan waitTimeout = GetWaitTimeout(timeoutHelper.RemainingTime());
            await replyHandle.WaitAsync(waitTimeout);
            return GetReply(last);
        }

        public override void SetInfo(WsrmMessageInfo info)
        {
            lock (ThisLock)
            {
                if (replied || replyInfo != null)
                {
                    return;
                }

                replyInfo = info;
            }

            replyHandle.Set();
        }
    }

    internal abstract class WsrmIndex
    {
        private static WsrmFeb2005Index s_wsAddressingAug2004WSReliableMessagingFeb2005;
        private static WsrmFeb2005Index s_wsAddressing10WSReliableMessagingFeb2005;
        private static Wsrm11Index s_wsAddressingAug2004WSReliableMessaging11;
        private static Wsrm11Index s_wsAddressing10WSReliableMessaging11;

        internal static ActionHeader GetAckRequestedActionHeader(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            return GetActionHeader(addressingVersion, reliableMessagingVersion, WsrmFeb2005Strings.AckRequested);
        }

        protected abstract ActionHeader GetActionHeader(string element);

        private static ActionHeader GetActionHeader(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion, string element)
        {
            WsrmIndex cache = null;

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                {
                    if (s_wsAddressingAug2004WSReliableMessagingFeb2005 == null)
                    {
                        s_wsAddressingAug2004WSReliableMessagingFeb2005 = new WsrmFeb2005Index(addressingVersion);
                    }

                    cache = s_wsAddressingAug2004WSReliableMessagingFeb2005;
                }
                else if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    if (s_wsAddressing10WSReliableMessagingFeb2005 == null)
                    {
                        s_wsAddressing10WSReliableMessagingFeb2005 = new WsrmFeb2005Index(addressingVersion);
                    }

                    cache = s_wsAddressing10WSReliableMessagingFeb2005;
                }
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                {
                    if (s_wsAddressingAug2004WSReliableMessaging11 == null)
                    {
                        s_wsAddressingAug2004WSReliableMessaging11 = new Wsrm11Index(addressingVersion);
                    }

                    cache = s_wsAddressingAug2004WSReliableMessaging11;
                }
                else if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    if (s_wsAddressing10WSReliableMessaging11 == null)
                    {
                        s_wsAddressing10WSReliableMessaging11 = new Wsrm11Index(addressingVersion);
                    }

                    cache = s_wsAddressing10WSReliableMessaging11;
                }
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }

            if (cache == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ProtocolException(SRP.Format(SRP.AddressingVersionNotSupported, addressingVersion)));
            }

            return cache.GetActionHeader(element);
        }

        internal static ActionHeader GetCloseSequenceActionHeader(AddressingVersion addressingVersion)
        {
            return GetActionHeader(addressingVersion, ReliableMessagingVersion.WSReliableMessaging11, Wsrm11Strings.CloseSequence);
        }

        internal static ActionHeader GetCloseSequenceResponseActionHeader(AddressingVersion addressingVersion)
        {
            return GetActionHeader(addressingVersion, ReliableMessagingVersion.WSReliableMessaging11, Wsrm11Strings.CloseSequenceResponse);
        }

        internal static ActionHeader GetCreateSequenceActionHeader(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            return GetActionHeader(addressingVersion, reliableMessagingVersion, WsrmFeb2005Strings.CreateSequence);
        }

        internal static string GetCreateSequenceActionString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Strings.CreateSequenceAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.CreateSequenceAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static XmlDictionaryString GetCreateSequenceResponseAction(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return XD.WsrmFeb2005Dictionary.CreateSequenceResponseAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return DXD.Wsrm11Dictionary.CreateSequenceResponseAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static string GetCreateSequenceResponseActionString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Strings.CreateSequenceResponseAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.CreateSequenceResponseAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static string GetFaultActionString(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return addressingVersion.DefaultFaultAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.FaultAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static XmlDictionaryString GetNamespace(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return XD.WsrmFeb2005Dictionary.Namespace;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return DXD.Wsrm11Dictionary.Namespace;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static string GetNamespaceString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Strings.Namespace;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.Namespace;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static ActionHeader GetSequenceAcknowledgementActionHeader(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            return GetActionHeader(addressingVersion, reliableMessagingVersion, WsrmFeb2005Strings.SequenceAcknowledgement);
        }

        internal static string GetSequenceAcknowledgementActionString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Strings.SequenceAcknowledgementAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.SequenceAcknowledgementAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static MessagePartSpecification GetSignedReliabilityMessageParts(
            ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Index.SignedReliabilityMessageParts;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Index.SignedReliabilityMessageParts;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static ActionHeader GetTerminateSequenceActionHeader(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            return GetActionHeader(addressingVersion, reliableMessagingVersion, WsrmFeb2005Strings.TerminateSequence);
        }

        internal static string GetTerminateSequenceActionString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return WsrmFeb2005Strings.TerminateSequenceAction;
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.TerminateSequenceAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static string GetTerminateSequenceResponseActionString(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                return Wsrm11Strings.TerminateSequenceResponseAction;
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        internal static ActionHeader GetTerminateSequenceResponseActionHeader(AddressingVersion addressingVersion)
        {
            return GetActionHeader(addressingVersion, ReliableMessagingVersion.WSReliableMessaging11,
                Wsrm11Strings.TerminateSequenceResponse);
        }
    }

    internal class Wsrm11Index : WsrmIndex
    {
        private static MessagePartSpecification s_signedReliabilityMessageParts;
        private ActionHeader _ackRequestedActionHeader;
        private AddressingVersion _addressingVersion;
        private ActionHeader _closeSequenceActionHeader;
        private ActionHeader _closeSequenceResponseActionHeader;
        private ActionHeader _createSequenceActionHeader;
        private ActionHeader _sequenceAcknowledgementActionHeader;
        private ActionHeader _terminateSequenceActionHeader;
        private ActionHeader _terminateSequenceResponseActionHeader;

        internal Wsrm11Index(AddressingVersion addressingVersion)
        {
            _addressingVersion = addressingVersion;
        }

        internal static MessagePartSpecification SignedReliabilityMessageParts
        {
            get
            {
                if (s_signedReliabilityMessageParts == null)
                {
                    XmlQualifiedName[] wsrmMessageHeaders = new XmlQualifiedName[]
                    {
                        new XmlQualifiedName(WsrmFeb2005Strings.Sequence, Wsrm11Strings.Namespace),
                        new XmlQualifiedName(WsrmFeb2005Strings.SequenceAcknowledgement, Wsrm11Strings.Namespace),
                        new XmlQualifiedName(WsrmFeb2005Strings.AckRequested, Wsrm11Strings.Namespace),
                        new XmlQualifiedName(Wsrm11Strings.UsesSequenceSTR, Wsrm11Strings.Namespace),
                    };

                    MessagePartSpecification s = new MessagePartSpecification(wsrmMessageHeaders);
                    s.MakeReadOnly();
                    s_signedReliabilityMessageParts = s;
                }

                return s_signedReliabilityMessageParts;
            }
        }

        protected override ActionHeader GetActionHeader(string element)
        {
            Wsrm11Dictionary wsrm11Dictionary = DXD.Wsrm11Dictionary;
            if (element == WsrmFeb2005Strings.AckRequested)
            {
                if (_ackRequestedActionHeader == null)
                {
                    _ackRequestedActionHeader = ActionHeader.Create(wsrm11Dictionary.AckRequestedAction,
                        _addressingVersion);
                }

                return _ackRequestedActionHeader;
            }
            else if (element == WsrmFeb2005Strings.CreateSequence)
            {
                if (_createSequenceActionHeader == null)
                {
                    _createSequenceActionHeader = ActionHeader.Create(wsrm11Dictionary.CreateSequenceAction,
                        _addressingVersion);
                }

                return _createSequenceActionHeader;
            }
            else if (element == WsrmFeb2005Strings.SequenceAcknowledgement)
            {
                if (_sequenceAcknowledgementActionHeader == null)
                {
                    _sequenceAcknowledgementActionHeader =
                        ActionHeader.Create(wsrm11Dictionary.SequenceAcknowledgementAction,
                        _addressingVersion);
                }

                return _sequenceAcknowledgementActionHeader;
            }
            else if (element == WsrmFeb2005Strings.TerminateSequence)
            {
                if (_terminateSequenceActionHeader == null)
                {
                    _terminateSequenceActionHeader =
                        ActionHeader.Create(wsrm11Dictionary.TerminateSequenceAction, _addressingVersion);
                }

                return _terminateSequenceActionHeader;
            }
            else if (element == Wsrm11Strings.TerminateSequenceResponse)
            {
                if (_terminateSequenceResponseActionHeader == null)
                {
                    _terminateSequenceResponseActionHeader =
                        ActionHeader.Create(wsrm11Dictionary.TerminateSequenceResponseAction, _addressingVersion);
                }

                return _terminateSequenceResponseActionHeader;
            }
            else if (element == Wsrm11Strings.CloseSequence)
            {
                if (_closeSequenceActionHeader == null)
                {
                    _closeSequenceActionHeader =
                        ActionHeader.Create(wsrm11Dictionary.CloseSequenceAction, _addressingVersion);
                }

                return _closeSequenceActionHeader;
            }
            else if (element == Wsrm11Strings.CloseSequenceResponse)
            {
                if (_closeSequenceResponseActionHeader == null)
                {
                    _closeSequenceResponseActionHeader =
                        ActionHeader.Create(wsrm11Dictionary.CloseSequenceResponseAction, _addressingVersion);
                }

                return _closeSequenceResponseActionHeader;
            }
            else
            {
                throw Fx.AssertAndThrow("Element not supported.");
            }
        }
    }

    internal class WsrmFeb2005Index : WsrmIndex
    {
        private static MessagePartSpecification s_signedReliabilityMessageParts;
        private ActionHeader _ackRequestedActionHeader;
        private AddressingVersion _addressingVersion;
        private ActionHeader _createSequenceActionHeader;
        private ActionHeader _sequenceAcknowledgementActionHeader;
        private ActionHeader _terminateSequenceActionHeader;

        internal WsrmFeb2005Index(AddressingVersion addressingVersion)
        {
            _addressingVersion = addressingVersion;
        }

        internal static MessagePartSpecification SignedReliabilityMessageParts
        {
            get
            {
                if (s_signedReliabilityMessageParts == null)
                {
                    XmlQualifiedName[] wsrmMessageHeaders = new XmlQualifiedName[]
                    {
                        new XmlQualifiedName(WsrmFeb2005Strings.Sequence, WsrmFeb2005Strings.Namespace),
                        new XmlQualifiedName(WsrmFeb2005Strings.SequenceAcknowledgement, WsrmFeb2005Strings.Namespace),
                        new XmlQualifiedName(WsrmFeb2005Strings.AckRequested, WsrmFeb2005Strings.Namespace),
                    };

                    MessagePartSpecification s = new MessagePartSpecification(wsrmMessageHeaders);
                    s.MakeReadOnly();
                    s_signedReliabilityMessageParts = s;
                }

                return s_signedReliabilityMessageParts;
            }
        }

        protected override ActionHeader GetActionHeader(string element)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;

            if (element == WsrmFeb2005Strings.AckRequested)
            {
                if (_ackRequestedActionHeader == null)
                {
                    _ackRequestedActionHeader = ActionHeader.Create(wsrmFeb2005Dictionary.AckRequestedAction,
                        _addressingVersion);
                }

                return _ackRequestedActionHeader;
            }
            else if (element == WsrmFeb2005Strings.CreateSequence)
            {
                if (_createSequenceActionHeader == null)
                {
                    _createSequenceActionHeader =
                        ActionHeader.Create(wsrmFeb2005Dictionary.CreateSequenceAction, _addressingVersion);
                }

                return _createSequenceActionHeader;
            }
            else if (element == WsrmFeb2005Strings.SequenceAcknowledgement)
            {
                if (_sequenceAcknowledgementActionHeader == null)
                {
                    _sequenceAcknowledgementActionHeader =
                        ActionHeader.Create(wsrmFeb2005Dictionary.SequenceAcknowledgementAction,
                        _addressingVersion);
                }

                return _sequenceAcknowledgementActionHeader;
            }
            else if (element == WsrmFeb2005Strings.TerminateSequence)
            {
                if (_terminateSequenceActionHeader == null)
                {
                    _terminateSequenceActionHeader =
                        ActionHeader.Create(wsrmFeb2005Dictionary.TerminateSequenceAction, _addressingVersion);
                }

                return _terminateSequenceActionHeader;
            }
            else
            {
                throw Fx.AssertAndThrow("Element not supported.");
            }
        }
    }

    internal static class WsrmUtilities
    {
        public static TimeSpan CalculateKeepAliveInterval(TimeSpan inactivityTimeout, int maxRetryCount)
        {
            return Ticks.ToTimeSpan(Ticks.FromTimeSpan(inactivityTimeout) / 2 / maxRetryCount);
        }

        internal static UniqueId NextSequenceId()
        {
            return new UniqueId();
        }

        internal static void AddAcknowledgementHeader(ReliableMessagingVersion reliableMessagingVersion,
            Message message, UniqueId id, SequenceRangeCollection ranges, bool final)
        {
            WsrmUtilities.AddAcknowledgementHeader(reliableMessagingVersion, message, id, ranges, final, -1);
        }

        internal static void AddAcknowledgementHeader(ReliableMessagingVersion reliableMessagingVersion,
            Message message, UniqueId id, SequenceRangeCollection ranges, bool final, int bufferRemaining)
        {
            message.Headers.Insert(0,
                new WsrmAcknowledgmentHeader(reliableMessagingVersion, id, ranges, final, bufferRemaining));
        }

        internal static void AddAckRequestedHeader(ReliableMessagingVersion reliableMessagingVersion, Message message,
            UniqueId id)
        {
            message.Headers.Insert(0, new WsrmAckRequestedHeader(reliableMessagingVersion, id));
        }

        internal static void AddSequenceHeader(ReliableMessagingVersion reliableMessagingVersion, Message message,
            UniqueId id, Int64 sequenceNumber, bool isLast)
        {
            message.Headers.Insert(0,
                new WsrmSequencedMessageHeader(reliableMessagingVersion, id, sequenceNumber, isLast));
        }

        internal static void AssertWsrm11(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion != ReliableMessagingVersion.WSReliableMessaging11)
            {
                throw Fx.AssertAndThrow("WS-ReliableMessaging 1.1 required.");
            }
        }

        internal static Message CreateAcknowledgmentMessage(MessageVersion version,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId id, SequenceRangeCollection ranges, bool final,
            int bufferRemaining)
        {
            Message message = Message.CreateMessage(version,
                WsrmIndex.GetSequenceAcknowledgementActionHeader(version.Addressing, reliableMessagingVersion));

            WsrmUtilities.AddAcknowledgementHeader(reliableMessagingVersion, message, id, ranges, final,
                bufferRemaining);
            message.Properties.AllowOutputBatching = false;

            return message;
        }

        internal static Message CreateAckRequestedMessage(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId id)
        {
            Message message = Message.CreateMessage(messageVersion,
                WsrmIndex.GetAckRequestedActionHeader(messageVersion.Addressing, reliableMessagingVersion));

            WsrmUtilities.AddAckRequestedHeader(reliableMessagingVersion, message, id);
            message.Properties.AllowOutputBatching = false;

            return message;
        }

        internal static Message CreateCloseSequenceResponse(MessageVersion messageVersion, UniqueId messageId,
            UniqueId inputId)
        {
            CloseSequenceResponse response = new CloseSequenceResponse(inputId);

            Message message = Message.CreateMessage(messageVersion,
                WsrmIndex.GetCloseSequenceResponseActionHeader(messageVersion.Addressing), response);

            message.Headers.RelatesTo = messageId;
            return message;
        }

        internal static Message CreateCreateSequenceResponse(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, bool duplex, CreateSequenceInfo createSequenceInfo,
            bool ordered, UniqueId inputId, EndpointAddress acceptAcksTo)
        {
            CreateSequenceResponse response = new CreateSequenceResponse(messageVersion.Addressing, reliableMessagingVersion);
            response.Identifier = inputId;
            response.Expires = createSequenceInfo.Expires;
            response.Ordered = ordered;

            if (duplex)
                response.AcceptAcksTo = acceptAcksTo;

            Message responseMessage
                = Message.CreateMessage(messageVersion, ActionHeader.Create(
                WsrmIndex.GetCreateSequenceResponseAction(reliableMessagingVersion), messageVersion.Addressing), response);

            return responseMessage;
        }

        internal static Message CreateCSRefusedCommunicationFault(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, string reason)
        {
            return CreateCSRefusedFault(messageVersion, reliableMessagingVersion, false, null, reason);
        }

        internal static Message CreateCSRefusedProtocolFault(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, string reason)
        {
            return CreateCSRefusedFault(messageVersion, reliableMessagingVersion, true, null, reason);
        }

        internal static Message CreateCSRefusedServerTooBusyFault(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, string reason)
        {
            FaultCode subCode = new FaultCode(WsrmFeb2005Strings.ConnectionLimitReached,
                WsrmFeb2005Strings.NETNamespace);
            subCode = new FaultCode(WsrmFeb2005Strings.CreateSequenceRefused,
                WsrmIndex.GetNamespaceString(reliableMessagingVersion), subCode);
            return CreateCSRefusedFault(messageVersion, reliableMessagingVersion, false, subCode, reason);
        }

        private static Message CreateCSRefusedFault(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, bool isSenderFault, FaultCode subCode, string reason)
        {
            FaultCode code;

            if (messageVersion.Envelope == EnvelopeVersion.Soap11)
            {
                code = new FaultCode(WsrmFeb2005Strings.CreateSequenceRefused, WsrmIndex.GetNamespaceString(reliableMessagingVersion));
            }
            else if (messageVersion.Envelope == EnvelopeVersion.Soap12)
            {
                if (subCode == null)
                    subCode = new FaultCode(WsrmFeb2005Strings.CreateSequenceRefused, WsrmIndex.GetNamespaceString(reliableMessagingVersion), subCode);

                if (isSenderFault)
                    code = FaultCode.CreateSenderFaultCode(subCode);
                else
                    code = FaultCode.CreateReceiverFaultCode(subCode);
            }
            else
            {
                throw Fx.AssertAndThrow("Unsupported version.");
            }

            FaultReason faultReason = new FaultReason(SRP.Format(SRP.CSRefused, reason), CultureInfo.CurrentCulture);

            MessageFault fault = MessageFault.CreateFault(code, faultReason);
            string action = WsrmIndex.GetFaultActionString(messageVersion.Addressing, reliableMessagingVersion);
            return Message.CreateMessage(messageVersion, fault, action);
        }

        public static Exception CreateCSFaultException(MessageVersion version,
            ReliableMessagingVersion reliableMessagingVersion, Message message, IChannel innerChannel)
        {
            MessageFault fault = MessageFault.CreateFault(message, TransportDefaults.MaxRMFaultSize);
            FaultCode code = fault.Code;
            FaultCode subCode;

            if (version.Envelope == EnvelopeVersion.Soap11)
            {
                subCode = code;
            }
            else if (version.Envelope == EnvelopeVersion.Soap12)
            {
                subCode = code.SubCode;
            }
            else
            {
                throw Fx.AssertAndThrow("Unsupported version.");
            }

            if (subCode != null)
            {
                // CreateSequenceRefused
                if ((subCode.Namespace == WsrmIndex.GetNamespaceString(reliableMessagingVersion))
                    && (subCode.Name == WsrmFeb2005Strings.CreateSequenceRefused))
                {
                    string reason = FaultException.GetSafeReasonText(fault);

                    if (version.Envelope == EnvelopeVersion.Soap12)
                    {
                        FaultCode subSubCode = subCode.SubCode;
                        if ((subSubCode != null)
                            && (subSubCode.Namespace == WsrmFeb2005Strings.NETNamespace)
                            && (subSubCode.Name == WsrmFeb2005Strings.ConnectionLimitReached))
                        {
                            return new ServerTooBusyException(reason);
                        }

                        if (code.IsSenderFault)
                        {
                            return new ProtocolException(reason);
                        }
                    }

                    return new CommunicationException(reason);
                }
                else if ((subCode.Namespace == version.Addressing.Namespace)
                    && (subCode.Name == AddressingStrings.EndpointUnavailable))
                {
                    return new EndpointNotFoundException(FaultException.GetSafeReasonText(fault));
                }
            }

            FaultConverter faultConverter = innerChannel.GetProperty<FaultConverter>();
            if (faultConverter == null)
                faultConverter = FaultConverter.GetDefaultFaultConverter(version);

            Exception exception;
            if (faultConverter.TryCreateException(message, fault, out exception))
            {
                return exception;
            }
            else
            {
                return new ProtocolException(SRP.Format(SRP.UnrecognizedFaultReceivedOnOpen, fault.Code.Namespace, fault.Code.Name, FaultException.GetSafeReasonText(fault)));
            }
        }

        internal static Message CreateEndpointNotFoundFault(MessageVersion version, string reason)
        {
            FaultCode subCode = new FaultCode(AddressingStrings.EndpointUnavailable, version.Addressing.Namespace);
            FaultCode code;

            if (version.Envelope == EnvelopeVersion.Soap11)
            {
                code = subCode;
            }
            else if (version.Envelope == EnvelopeVersion.Soap12)
            {
                code = FaultCode.CreateSenderFaultCode(subCode);
            }
            else
            {
                throw Fx.AssertAndThrow("Unsupported version.");
            }

            FaultReason faultReason = new FaultReason(reason, CultureInfo.CurrentCulture);
            MessageFault fault = MessageFault.CreateFault(code, faultReason);
            return Message.CreateMessage(version, fault, version.Addressing.DefaultFaultAction);
        }

        internal static Message CreateTerminateMessage(MessageVersion version,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId id)
        {
            return CreateTerminateMessage(version, reliableMessagingVersion, id, -1);
        }

        internal static Message CreateTerminateMessage(MessageVersion version,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId id, Int64 last)
        {
            Message message = Message.CreateMessage(version,
                WsrmIndex.GetTerminateSequenceActionHeader(version.Addressing, reliableMessagingVersion),
                new TerminateSequence(reliableMessagingVersion, id, last));

            message.Properties.AllowOutputBatching = false;

            return message;
        }

        internal static Message CreateTerminateResponseMessage(MessageVersion version, UniqueId messageId, UniqueId sequenceId)
        {
            Message message = Message.CreateMessage(version,
                WsrmIndex.GetTerminateSequenceResponseActionHeader(version.Addressing),
                new TerminateSequenceResponse(sequenceId));

            message.Properties.AllowOutputBatching = false;
            message.Headers.RelatesTo = messageId;
            return message;
        }

        internal static UniqueId GetInputId(WsrmMessageInfo info)
        {
            if (info.TerminateSequenceInfo != null)
            {
                return info.TerminateSequenceInfo.Identifier;
            }

            if (info.SequencedMessageInfo != null)
            {
                return info.SequencedMessageInfo.SequenceID;
            }

            if (info.AckRequestedInfo != null)
            {
                return info.AckRequestedInfo.SequenceID;
            }

            if (info.WsrmHeaderFault != null && info.WsrmHeaderFault.FaultsInput)
            {
                return info.WsrmHeaderFault.SequenceID;
            }

            if (info.CloseSequenceInfo != null)
            {
                return info.CloseSequenceInfo.Identifier;
            }

            return null;
        }

        internal static UniqueId GetOutputId(ReliableMessagingVersion reliableMessagingVersion, WsrmMessageInfo info)
        {
            if (info.AcknowledgementInfo != null)
            {
                return info.AcknowledgementInfo.SequenceID;
            }

            if (info.WsrmHeaderFault != null && info.WsrmHeaderFault.FaultsOutput)
            {
                return info.WsrmHeaderFault.SequenceID;
            }

            if (info.TerminateSequenceResponseInfo != null)
            {
                return info.TerminateSequenceResponseInfo.Identifier;
            }

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (info.CloseSequenceInfo != null)
                {
                    return info.CloseSequenceInfo.Identifier;
                }

                if (info.CloseSequenceResponseInfo != null)
                {
                    return info.CloseSequenceResponseInfo.Identifier;
                }

                if (info.TerminateSequenceResponseInfo != null)
                {
                    return info.TerminateSequenceResponseInfo.Identifier;
                }
            }

            return null;
        }

        internal static bool IsWsrmAction(ReliableMessagingVersion reliableMessagingVersion, string action)
        {
            if (action == null)
                return false;
            return (action.StartsWith(WsrmIndex.GetNamespaceString(reliableMessagingVersion), StringComparison.Ordinal));
        }

        public static void ReadEmptyElement(XmlDictionaryReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                reader.ReadEndElement();
            }
        }

        public static UniqueId ReadIdentifier(XmlDictionaryReader reader,
            ReliableMessagingVersion reliableMessagingVersion)
        {
            reader.ReadStartElement(XD.WsrmFeb2005Dictionary.Identifier, WsrmIndex.GetNamespace(reliableMessagingVersion));
            UniqueId sequenceID = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();
            return sequenceID;
        }

        public static Int64 ReadSequenceNumber(XmlDictionaryReader reader)
        {
            return WsrmUtilities.ReadSequenceNumber(reader, false);
        }

        public static Int64 ReadSequenceNumber(XmlDictionaryReader reader, bool allowZero)
        {
            Int64 sequenceNumber = reader.ReadContentAsLong();

            if (sequenceNumber < 0 || (sequenceNumber == 0 && !allowZero))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                    SRP.Format(SRP.InvalidSequenceNumber, sequenceNumber)));
            }

            return sequenceNumber;
        }

        // Caller owns message.
        public static WsrmFault ValidateCloseSequenceResponse(ChannelReliableSession session, UniqueId messageId,
            WsrmMessageInfo info, Int64 last)
        {
            string exceptionString = null;
            string faultString = null;

            if (info.CloseSequenceResponseInfo == null)
            {
                exceptionString = SRP.Format(SRP.InvalidWsrmResponseSessionFaultedExceptionString,
                    Wsrm11Strings.CloseSequence, info.Action,
                    Wsrm11Strings.CloseSequenceResponseAction);
                faultString = SRP.Format(SRP.InvalidWsrmResponseSessionFaultedFaultString,
                    Wsrm11Strings.CloseSequence, info.Action,
                    Wsrm11Strings.CloseSequenceResponseAction);
            }
            else if (!object.Equals(messageId, info.CloseSequenceResponseInfo.RelatesTo))
            {
                exceptionString = SRP.Format(SRP.WsrmMessageWithWrongRelatesToExceptionString, Wsrm11Strings.CloseSequence);
                faultString = SRP.Format(SRP.WsrmMessageWithWrongRelatesToFaultString, Wsrm11Strings.CloseSequence);
            }
            else if (info.AcknowledgementInfo == null || !info.AcknowledgementInfo.Final)
            {
                exceptionString = SRP.MissingFinalAckExceptionString;
                faultString = SRP.SequenceTerminatedMissingFinalAck;
            }
            else
            {
                return ValidateFinalAck(session, info, last);
            }

            UniqueId sequenceId = session.OutputID;
            return SequenceTerminatedFault.CreateProtocolFault(sequenceId, faultString, exceptionString);
        }

        public static WsrmFault ValidateFinalAck(ChannelReliableSession session, WsrmMessageInfo info, Int64 last)
        {
            WsrmAcknowledgmentInfo ackInfo = info.AcknowledgementInfo;
            WsrmFault fault = ValidateFinalAckExists(session, ackInfo);

            if (fault != null)
            {
                return fault;
            }

            SequenceRangeCollection finalRanges = ackInfo.Ranges;

            if (last == 0)
            {
                if (finalRanges.Count == 0)
                {
                    return null;
                }
            }
            else
            {
                if ((finalRanges.Count == 1) && (finalRanges[0].Lower == 1) && (finalRanges[0].Upper == last))
                {
                    return null;
                }
            }

            return new InvalidAcknowledgementFault(session.OutputID, ackInfo.Ranges);
        }

        public static WsrmFault ValidateFinalAckExists(ChannelReliableSession session, WsrmAcknowledgmentInfo ackInfo)
        {
            if (ackInfo == null || !ackInfo.Final)
            {
                string exceptionString = SRP.MissingFinalAckExceptionString;
                string faultString = SRP.SequenceTerminatedMissingFinalAck;
                return SequenceTerminatedFault.CreateProtocolFault(session.OutputID, faultString, exceptionString);
            }

            return null;
        }

        // Caller owns message.
        public static WsrmFault ValidateTerminateSequenceResponse(ChannelReliableSession session, UniqueId messageId,
            WsrmMessageInfo info, Int64 last)
        {
            string exceptionString = null;
            string faultString = null;

            if (info.WsrmHeaderFault is UnknownSequenceFault)
            {
                return null;
            }
            else if (info.TerminateSequenceResponseInfo == null)
            {
                exceptionString = SRP.Format(SRP.InvalidWsrmResponseSessionFaultedExceptionString,
                    WsrmFeb2005Strings.TerminateSequence, info.Action,
                    Wsrm11Strings.TerminateSequenceResponseAction);
                faultString = SRP.Format(SRP.InvalidWsrmResponseSessionFaultedFaultString,
                    WsrmFeb2005Strings.TerminateSequence, info.Action,
                    Wsrm11Strings.TerminateSequenceResponseAction);
            }
            else if (!object.Equals(messageId, info.TerminateSequenceResponseInfo.RelatesTo))
            {
                exceptionString = SRP.Format(SRP.WsrmMessageWithWrongRelatesToExceptionString, WsrmFeb2005Strings.TerminateSequence);
                faultString = SRP.Format(SRP.WsrmMessageWithWrongRelatesToFaultString, WsrmFeb2005Strings.TerminateSequence);
            }
            else
            {
                return ValidateFinalAck(session, info, last);
            }

            UniqueId sequenceId = session.OutputID;
            return SequenceTerminatedFault.CreateProtocolFault(sequenceId, faultString, exceptionString);
        }

        // Checks that ReplyTo and RemoteAddress are equivalent. Will fault the session with SequenceTerminatedFault.
        // Meant to be used for CloseSequence and TerminateSequence in Wsrm 1.1.
        public static bool ValidateWsrmRequest(ChannelReliableSession session, WsrmRequestInfo info,
            IReliableChannelBinder binder, RequestContext context)
        {
            if (!(info is CloseSequenceInfo) && !(info is TerminateSequenceInfo))
            {
                throw Fx.AssertAndThrow("Method is meant for CloseSequence or TerminateSequence only.");
            }

            if (info.ReplyTo.Uri != binder.RemoteAddress.Uri)
            {
                string faultString = SRP.Format(SRP.WsrmRequestIncorrectReplyToFaultString, info.RequestName);
                string exceptionString = SRP.Format(SRP.WsrmRequestIncorrectReplyToExceptionString, info.RequestName);
                WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(session.InputID, faultString, exceptionString);
                session.OnLocalFault(fault.CreateException(), fault, context);
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void WriteIdentifier(XmlDictionaryWriter writer,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId sequenceId)
        {
            writer.WriteStartElement(WsrmFeb2005Strings.Prefix, XD.WsrmFeb2005Dictionary.Identifier,
                WsrmIndex.GetNamespace(reliableMessagingVersion));
            writer.WriteValue(sequenceId);
            writer.WriteEndElement();
        }
    }
}
