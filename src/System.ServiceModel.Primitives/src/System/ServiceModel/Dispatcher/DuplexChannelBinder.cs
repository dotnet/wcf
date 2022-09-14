// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    internal class DuplexChannelBinder : IChannelBinder
    {
        private IDuplexChannel _channel;
        private IRequestReplyCorrelator _correlator;
        private TimeSpan _defaultSendTimeout;
        private IdentityVerifier _identityVerifier;
        private int _pending;
        private bool _syncPumpEnabled;
        private List<IDuplexRequest> _requests;
        private List<ICorrelatorKey> _timedOutRequests;
        private ChannelHandler _channelHandler;
        private volatile bool _requestAborted;

        internal DuplexChannelBinder(IDuplexChannel channel, IRequestReplyCorrelator correlator)
        {
            Fx.Assert(channel != null, "caller must verify");
            Fx.Assert(correlator != null, "caller must verify");
            _channel = channel;
            _correlator = correlator;
            _channel.Faulted += new EventHandler(OnFaulted);
        }

        internal DuplexChannelBinder(IDuplexChannel channel, IRequestReplyCorrelator correlator, Uri listenUri)
            : this(channel, correlator)
        {
            ListenUri = listenUri;
        }

        internal DuplexChannelBinder(IDuplexSessionChannel channel, IRequestReplyCorrelator correlator, Uri listenUri)
            : this((IDuplexChannel)channel, correlator, listenUri)
        {
            HasSession = true;
        }

        internal DuplexChannelBinder(IDuplexSessionChannel channel, IRequestReplyCorrelator correlator, bool useActiveAutoClose)
            : this(useActiveAutoClose ? new AutoCloseDuplexSessionChannel(channel) : channel, correlator, null)
        {
        }

        public IChannel Channel
        {
            get { return _channel; }
        }

        public TimeSpan DefaultCloseTimeout { get; set; }

        internal ChannelHandler ChannelHandler
        {
            get
            {
                if (!(_channelHandler != null))
                {
                    Fx.Assert("DuplexChannelBinder.ChannelHandler: (channelHandler != null)");
                }
                return _channelHandler;
            }
            set
            {
                if (!(_channelHandler == null))
                {
                    Fx.Assert("DuplexChannelBinder.ChannelHandler: (channelHandler == null)");
                }
                _channelHandler = value;
            }
        }

        public TimeSpan DefaultSendTimeout
        {
            get { return _defaultSendTimeout; }
            set { _defaultSendTimeout = value; }
        }

        public bool HasSession { get; }

        internal IdentityVerifier IdentityVerifier
        {
            get
            {
                if (_identityVerifier == null)
                {
                    _identityVerifier = IdentityVerifier.CreateDefault();
                }

                return _identityVerifier;
            }
            set
            {
                _identityVerifier = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        public Uri ListenUri { get; }

        public EndpointAddress LocalAddress
        {
            get { return _channel.LocalAddress; }
        }

        private bool Pumping
        {
            get
            {
                if (_syncPumpEnabled)
                {
                    return true;
                }

                if (ChannelHandler != null && ChannelHandler.HasRegisterBeenCalled)
                {
                    return true;
                }

                return false;
            }
        }

        public EndpointAddress RemoteAddress
        {
            get { return _channel.RemoteAddress; }
        }

        private List<IDuplexRequest> Requests
        {
            get
            {
                lock (ThisLock)
                {
                    if (_requests == null)
                    {
                        _requests = new List<IDuplexRequest>();
                    }

                    return _requests;
                }
            }
        }

        private List<ICorrelatorKey> TimedOutRequests
        {
            get
            {
                lock (ThisLock)
                {
                    if (_timedOutRequests == null)
                    {
                        _timedOutRequests = new List<ICorrelatorKey>();
                    }
                    return _timedOutRequests;
                }
            }
        }

        private object ThisLock
        {
            get { return this; }
        }

        private void OnFaulted(object sender, EventArgs e)
        {
            //Some unhandled exception happened on the channel. 
            //So close all pending requests so the callbacks (in case of async)
            //on the requests are called.
            AbortRequests();
        }

        public void Abort()
        {
            _channel.Abort();
            AbortRequests();
        }

        public void CloseAfterFault(TimeSpan timeout)
        {
            _channel.Close(timeout);
            AbortRequests();
        }

        private void AbortRequests()
        {
            IDuplexRequest[] array = null;
            lock (ThisLock)
            {
                if (_requests != null)
                {
                    array = _requests.ToArray();

                    foreach (IDuplexRequest request in array)
                    {
                        request.Abort();
                    }
                }
                _requests = null;
                _requestAborted = true;
            }

            // Remove requests from the correlator since the channel might be either faulting or aborting,
            // We are not going to get a reply for these requests. If they are not removed from the correlator, this will cause a leak.
            // This operation does not have to be under the lock
            if (array != null && array.Length > 0)
            {
                RequestReplyCorrelator requestReplyCorrelator = _correlator as RequestReplyCorrelator;
                if (requestReplyCorrelator != null)
                {
                    foreach (IDuplexRequest request in array)
                    {
                        ICorrelatorKey keyedRequest = request as ICorrelatorKey;
                        if (keyedRequest != null)
                        {
                            requestReplyCorrelator.RemoveRequest(keyedRequest);
                        }
                    }
                }
            }

            //if there are any timed out requests, delete it from the correlator table
            DeleteTimedoutRequestsFromCorrelator();
        }

        private TimeoutException GetReceiveTimeoutException(TimeSpan timeout)
        {
            EndpointAddress address = _channel.RemoteAddress ?? _channel.LocalAddress;
            if (address != null)
            {
                return new TimeoutException(SRP.Format(SRP.SFxRequestTimedOut2, address, timeout));
            }
            else
            {
                return new TimeoutException(SRP.Format(SRP.SFxRequestTimedOut1, timeout));
            }
        }

        internal bool HandleRequestAsReply(Message message)
        {
            UniqueId relatesTo = null;
            try
            {
                relatesTo = message.Headers.RelatesTo;
            }
            catch (MessageHeaderException)
            {
                // ignore it
            }
            if (relatesTo == null)
            {
                return false;
            }
            else
            {
                return HandleRequestAsReplyCore(message);
            }
        }

        private bool HandleRequestAsReplyCore(Message message)
        {
            IDuplexRequest request = _correlator.Find<IDuplexRequest>(message, true);
            if (request != null)
            {
                request.GotReply(message);
                return true;
            }
            return false;
        }

        public void EnsurePumping()
        {
            lock (ThisLock)
            {
                if (!_syncPumpEnabled)
                {
                    if (!ChannelHandler.HasRegisterBeenCalled)
                    {
                        ChannelHandler.Register(ChannelHandler);
                    }
                }
            }
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (_channel.State == CommunicationState.Faulted)
            {
                return new ChannelFaultedAsyncResult(callback, state);
            }

            return _channel.BeginTryReceive(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            ChannelFaultedAsyncResult channelFaultedResult = result as ChannelFaultedAsyncResult;
            if (channelFaultedResult != null)
            {
                AbortRequests();
                requestContext = null;
                return true;
            }

            Message message;
            if (_channel.EndTryReceive(result, out message))
            {
                if (message != null)
                {
                    requestContext = new DuplexRequestContext(_channel, message, this);
                }
                else
                {
                    AbortRequests();
                    requestContext = null;
                }
                return true;
            }
            else
            {
                requestContext = null;
                return false;
            }
        }

        public RequestContext CreateRequestContext(Message message)
        {
            return new DuplexRequestContext(_channel, message, this);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginSend(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _channel.EndSend(result);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            _channel.Send(message, timeout);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            bool success = false;
            AsyncDuplexRequest duplexRequest = null;

            try
            {
                RequestReplyCorrelator.PrepareRequest(message);
                duplexRequest = new AsyncDuplexRequest(message, this, timeout, callback, state);

                lock (ThisLock)
                {
                    RequestStarting(message, duplexRequest);
                }

                IAsyncResult result = _channel.BeginSend(message, timeout, Fx.ThunkCallback(new AsyncCallback(SendCallback)), duplexRequest);

                if (result.CompletedSynchronously)
                {
                    duplexRequest.FinishedSend(result, true);
                }

                EnsurePumping();

                success = true;
                return duplexRequest;
            }
            finally
            {
                lock (ThisLock)
                {
                    if (success)
                    {
                        duplexRequest.EnableCompletion();
                    }
                    else
                    {
                        RequestCompleting(duplexRequest);
                    }
                }
            }
        }

        public Message EndRequest(IAsyncResult result)
        {
            AsyncDuplexRequest duplexRequest = result as AsyncDuplexRequest;

            if (duplexRequest == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SPS_InvalidAsyncResult));
            }

            return duplexRequest.End();
        }

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            if (_channel.State == CommunicationState.Faulted)
            {
                AbortRequests();
                requestContext = null;
                return true;
            }

            Message message;
            if (_channel.TryReceive(timeout, out message))
            {
                if (message != null)
                {
                    requestContext = new DuplexRequestContext(_channel, message, this);
                }
                else
                {
                    AbortRequests();
                    requestContext = null;
                }
                return true;
            }
            else
            {
                requestContext = null;
                return false;
            }
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            SyncDuplexRequest duplexRequest = null;
            bool optimized = false;

            RequestReplyCorrelator.PrepareRequest(message);

            lock (ThisLock)
            {
                if (!Pumping)
                {
                    optimized = true;
                    _syncPumpEnabled = true;
                }

                if (!optimized)
                {
                    duplexRequest = new SyncDuplexRequest(this);
                }

                RequestStarting(message, duplexRequest);
            }

            if (optimized)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                UniqueId messageId = message.Headers.MessageId;

                try
                {
                    _channel.Send(message, timeoutHelper.RemainingTime());
                    if (DiagnosticUtility.ShouldUseActivity &&
                        ServiceModelActivity.Current != null &&
                        ServiceModelActivity.Current.ActivityType == ActivityType.ProcessAction)
                    {
                        ServiceModelActivity.Current.Suspend();
                    }

                    for (; ; )
                    {
                        TimeSpan remaining = timeoutHelper.RemainingTime();
                        Message reply;

                        if (!_channel.TryReceive(remaining, out reply))
                        {
                            throw TraceUtility.ThrowHelperError(GetReceiveTimeoutException(timeout), message);
                        }

                        if (reply == null)
                        {
                            AbortRequests();
                            return null;
                        }

                        if (reply.Headers.RelatesTo == messageId)
                        {
                            return reply;
                        }
                        else if (!HandleRequestAsReply(reply))
                        {
                            // SFx drops a message here
                            reply.Close();
                        }
                    }
                }
                finally
                {
                    lock (ThisLock)
                    {
                        RequestCompleting(null);
                        _syncPumpEnabled = false;
                        if (_pending > 0)
                        {
                            EnsurePumping();
                        }
                    }
                }
            }
            else
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                _channel.Send(message, timeoutHelper.RemainingTime());
                EnsurePumping();
                return duplexRequest.WaitForReply(timeoutHelper.RemainingTime());
            }
        }

        private void RequestStarting(Message message, IDuplexRequest request)
        {
            if (request != null)
            {
                Requests.Add(request);
                if (!_requestAborted)
                {
                    _correlator.Add(message, request);
                }
            }
            _pending++;
        }

        private void RequestCompleting(IDuplexRequest request)
        {
            _pending--;
            if (_pending == 0)
            {
                _requests = null;
            }
            else if ((request != null) && (_requests != null))
            {
                _requests.Remove(request);
            }
        }

        // ASSUMPTION: caller holds ThisLock
        private void AddToTimedOutRequestList(ICorrelatorKey request)
        {
            Fx.Assert(request != null, "request cannot be null");
            TimedOutRequests.Add(request);
        }

        // ASSUMPTION: caller holds  ThisLock
        private void RemoveFromTimedOutRequestList(ICorrelatorKey request)
        {
            Fx.Assert(request != null, "request cannot be null");
            if (_timedOutRequests != null)
            {
                _timedOutRequests.Remove(request);
            }
        }

        private void DeleteTimedoutRequestsFromCorrelator()
        {
            ICorrelatorKey[] array = null;
            if (_timedOutRequests != null && _timedOutRequests.Count > 0)
            {
                lock (ThisLock)
                {
                    if (_timedOutRequests != null && _timedOutRequests.Count > 0)
                    {
                        array = _timedOutRequests.ToArray();
                        _timedOutRequests = null;
                    }
                }
            }

            // Remove requests from the correlator since the channel might be either faulting, aborting or closing 
            // We are not going to get a reply for these timed out requests. If they are not removed from the correlator, this will cause a leak.
            // This operation does not have to be under the lock
            if (array != null && array.Length > 0)
            {
                RequestReplyCorrelator requestReplyCorrelator = _correlator as RequestReplyCorrelator;
                if (requestReplyCorrelator != null)
                {
                    foreach (ICorrelatorKey request in array)
                    {
                        requestReplyCorrelator.RemoveRequest(request);
                    }
                }
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            AsyncDuplexRequest duplexRequest = result.AsyncState as AsyncDuplexRequest;
            if (!((duplexRequest != null)))
            {
                Fx.Assert("DuplexChannelBinder.RequestCallback: (duplexRequest != null)");
            }

            if (!result.CompletedSynchronously)
            {
                duplexRequest.FinishedSend(result, false);
            }
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return _channel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return _channel.EndWaitForMessage(result);
        }

        internal class DuplexRequestContext : RequestContextBase
        {
            private DuplexChannelBinder _binder;
            private IDuplexChannel _channel;

            internal DuplexRequestContext(IDuplexChannel channel, Message request, DuplexChannelBinder binder)
                : base(request, binder.DefaultCloseTimeout, binder.DefaultSendTimeout)
            {
                _channel = channel;
                _binder = binder;
            }

            protected override void OnAbort()
            {
            }

            protected override void OnClose(TimeSpan timeout)
            {
            }

            protected override void OnReply(Message message, TimeSpan timeout)
            {
                if (message != null)
                {
                    _channel.Send(message, timeout);
                }
            }

            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return new ReplyAsyncResult(this, message, timeout, callback, state);
            }

            protected override void OnEndReply(IAsyncResult result)
            {
                ReplyAsyncResult.End(result);
            }

            internal class ReplyAsyncResult : AsyncResult
            {
                private static AsyncCallback s_onSend;
                private DuplexRequestContext _context;

                public ReplyAsyncResult(DuplexRequestContext context, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    if (message != null)
                    {
                        if (s_onSend == null)
                        {
                            s_onSend = Fx.ThunkCallback(new AsyncCallback(OnSend));
                        }
                        _context = context;
                        IAsyncResult result = context._channel.BeginSend(message, timeout, s_onSend, this);
                        if (!result.CompletedSynchronously)
                        {
                            return;
                        }
                        context._channel.EndSend(result);
                    }

                    base.Complete(true);
                }

                public static void End(IAsyncResult result)
                {
                    AsyncResult.End<ReplyAsyncResult>(result);
                }

                private static void OnSend(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    Exception completionException = null;
                    ReplyAsyncResult thisPtr = (ReplyAsyncResult)result.AsyncState;
                    try
                    {
                        thisPtr._context._channel.EndSend(result);
                    }
                    catch (Exception exception)
                    {
                        if (Fx.IsFatal(exception))
                        {
                            throw;
                        }
                        completionException = exception;
                    }

                    thisPtr.Complete(false, completionException);
                }
            }
        }

        private interface IDuplexRequest
        {
            void Abort();
            void GotReply(Message reply);
        }

        internal class SyncDuplexRequest : IDuplexRequest, ICorrelatorKey
        {
            private Message _reply;
            private DuplexChannelBinder _parent;
            private ManualResetEvent _wait = new ManualResetEvent(false);
            private int _waitCount = 0;
            private RequestReplyCorrelator.Key _requestCorrelatorKey;

            internal SyncDuplexRequest(DuplexChannelBinder parent)
            {
                _parent = parent;
            }

            RequestReplyCorrelator.Key ICorrelatorKey.RequestCorrelatorKey
            {
                get
                {
                    return _requestCorrelatorKey;
                }
                set
                {
                    Fx.Assert(_requestCorrelatorKey == null, "RequestCorrelatorKey is already set for this request");
                    _requestCorrelatorKey = value;
                }
            }

            public void Abort()
            {
                _wait.Set();
            }

            internal Message WaitForReply(TimeSpan timeout)
            {
                try
                {
                    if (!TimeoutHelper.WaitOne(_wait, timeout))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_parent.GetReceiveTimeoutException(timeout));
                    }
                }
                finally
                {
                    CloseWaitHandle();
                }

                return _reply;
            }

            public void GotReply(Message reply)
            {
                lock (_parent.ThisLock)
                {
                    _parent.RequestCompleting(this);
                }
                _reply = reply;
                _wait.Set();
                CloseWaitHandle();
            }

            private void CloseWaitHandle()
            {
                if (Interlocked.Increment(ref _waitCount) == 2)
                {
                    _wait.Dispose();
                }
            }
        }

        internal class AsyncDuplexRequest : AsyncResult, IDuplexRequest, ICorrelatorKey
        {
            private static Action<object> s_timerCallback = new Action<object>(AsyncDuplexRequest.TimerCallback);

            private bool _aborted;
            private bool _enableComplete;
            private bool _gotReply;
            private Exception _sendException;
            private IAsyncResult _sendResult;
            private DuplexChannelBinder _parent;
            private Message _reply;
            private bool _timedOut;
            private TimeSpan _timeout;
            private Timer _timer;
            private ServiceModelActivity _activity;
            private RequestReplyCorrelator.Key _requestCorrelatorKey;

            internal AsyncDuplexRequest(Message message, DuplexChannelBinder parent, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                _parent = parent;
                _timeout = timeout;

                if (timeout != TimeSpan.MaxValue)
                {
                    _timer = new Timer(new TimerCallback(AsyncDuplexRequest.s_timerCallback), this, timeout, TimeSpan.FromMilliseconds(-1));
                }
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    _activity = TraceUtility.ExtractActivity(message);
                }
            }

            private bool IsDone
            {
                get
                {
                    if (!_enableComplete)
                    {
                        return false;
                    }

                    return (((_sendResult != null) && _gotReply) ||
                            (_sendException != null) ||
                            _timedOut ||
                            _aborted);
                }
            }

            RequestReplyCorrelator.Key ICorrelatorKey.RequestCorrelatorKey
            {
                get
                {
                    return _requestCorrelatorKey;
                }
                set
                {
                    Fx.Assert(_requestCorrelatorKey == null, "RequestCorrelatorKey is already set for this request");
                    _requestCorrelatorKey = value;
                }
            }

            public void Abort()
            {
                bool done;

                lock (_parent.ThisLock)
                {
                    bool wasDone = IsDone;
                    _aborted = true;
                    done = !wasDone && IsDone;
                }

                if (done)
                {
                    Done(false);
                }
            }

            private void Done(bool completedSynchronously)
            {
                // Make sure that we are acting on the Reply activity.
                ServiceModelActivity replyActivity = DiagnosticUtility.ShouldUseActivity ?
                    TraceUtility.ExtractActivity(_reply) : null;
                using (ServiceModelActivity.BoundOperation(replyActivity))
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    lock (_parent.ThisLock)
                    {
                        if (_timedOut)
                        {
                            // this needs to be saved in a list since we need to remove these from the correlator table when the channel aborts or closes
                            _parent.AddToTimedOutRequestList(this);
                        }
                        _parent.RequestCompleting(this);
                    }

                    if (_sendException != null)
                    {
                        Complete(completedSynchronously, _sendException);
                    }
                    else if (_timedOut)
                    {
                        Complete(completedSynchronously, _parent.GetReceiveTimeoutException(_timeout));
                    }
                    else
                    {
                        Complete(completedSynchronously);
                    }
                }
            }

            public void EnableCompletion()
            {
                bool done;

                lock (_parent.ThisLock)
                {
                    bool wasDone = IsDone;
                    _enableComplete = true;
                    done = !wasDone && IsDone;
                }

                if (done)
                {
                    Done(true);
                }
            }

            public void FinishedSend(IAsyncResult sendResult, bool completedSynchronously)
            {
                Exception sendException = null;

                try
                {
                    _parent._channel.EndSend(sendResult);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    sendException = e;
                }

                bool done;

                lock (_parent.ThisLock)
                {
                    bool wasDone = IsDone;
                    _sendResult = sendResult;
                    _sendException = sendException;
                    done = !wasDone && IsDone;
                }

                if (done)
                {
                    Done(completedSynchronously);
                }
            }

            internal Message End()
            {
                AsyncResult.End<AsyncDuplexRequest>(this);
                return _reply;
            }

            public void GotReply(Message reply)
            {
                bool done;

                ServiceModelActivity replyActivity = DiagnosticUtility.ShouldUseActivity ?
                    TraceUtility.ExtractActivity(reply) : null;

                using (ServiceModelActivity.BoundOperation(replyActivity))
                {
                    lock (_parent.ThisLock)
                    {
                        bool wasDone = IsDone;
                        _reply = reply;
                        _gotReply = true;
                        done = !wasDone && IsDone;
                        // we got reply on the channel after the request timed out, let's delete it from the pending timed out requests
                        // we don't neeed to hold on to it since it is now  removed from the correlator table
                        if (wasDone && _timedOut)
                        {
                            _parent.RemoveFromTimedOutRequestList(this);
                        }
                    }
                    if (replyActivity != null && DiagnosticUtility.ShouldUseActivity)
                    {
                        TraceUtility.SetActivity(reply, _activity);
                        if (DiagnosticUtility.ShouldUseActivity && _activity != null)
                        {
                            if (null != FxTrace.Trace)
                            {
                                FxTrace.Trace.TraceTransfer(_activity.Id);
                            }
                        }
                    }
                }
                if (DiagnosticUtility.ShouldUseActivity && replyActivity != null)
                {
                    replyActivity.Stop();
                }

                if (done)
                {
                    Done(false);
                }
            }

            private void TimedOut()
            {
                bool done;

                lock (_parent.ThisLock)
                {
                    bool wasDone = IsDone;
                    _timedOut = true;
                    done = !wasDone && IsDone;
                }

                if (done)
                {
                    Done(false);
                }
            }

            private static void TimerCallback(object state)
            {
                ((AsyncDuplexRequest)state).TimedOut();
            }
        }

        private class ChannelFaultedAsyncResult : CompletedAsyncResult
        {
            public ChannelFaultedAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }
        }

        // used to read-ahead by a single message and auto-close the session when we read null
        internal class AutoCloseDuplexSessionChannel : IDuplexSessionChannel
        {
            private static AsyncCallback s_receiveAsyncCallback;
            private static Action<object> s_receiveThreadSchedulerCallback;
            private static AsyncCallback s_closeInnerChannelCallback;
            private IDuplexSessionChannel _innerChannel;
            private InputQueue<Message> _pendingMessages;
            private Action _messageDequeuedCallback;
            private CloseState _closeState;

            public AutoCloseDuplexSessionChannel(IDuplexSessionChannel innerChannel)
            {
                _innerChannel = innerChannel;
                _pendingMessages = new InputQueue<Message>();
                _messageDequeuedCallback = new Action(StartBackgroundReceive); // kick off a new receive when a message is picked up
                _closeState = new CloseState();
            }

            private object ThisLock
            {
                get
                {
                    return this;
                }
            }

            public EndpointAddress LocalAddress
            {
                get { return _innerChannel.LocalAddress; }
            }

            public EndpointAddress RemoteAddress
            {
                get { return _innerChannel.RemoteAddress; }
            }

            public Uri Via
            {
                get { return _innerChannel.Via; }
            }

            public IDuplexSession Session
            {
                get { return _innerChannel.Session; }
            }

            public CommunicationState State
            {
                get { return _innerChannel.State; }
            }

            public event EventHandler Closing
            {
                add { _innerChannel.Closing += value; }
                remove { _innerChannel.Closing -= value; }
            }

            public event EventHandler Closed
            {
                add { _innerChannel.Closed += value; }
                remove { _innerChannel.Closed -= value; }
            }

            public event EventHandler Faulted
            {
                add { _innerChannel.Faulted += value; }
                remove { _innerChannel.Faulted -= value; }
            }

            public event EventHandler Opened
            {
                add { _innerChannel.Opened += value; }
                remove { _innerChannel.Opened -= value; }
            }

            public event EventHandler Opening
            {
                add { _innerChannel.Opening += value; }
                remove { _innerChannel.Opening -= value; }
            }

            private TimeSpan DefaultCloseTimeout
            {
                get
                {
                    IDefaultCommunicationTimeouts defaultTimeouts = _innerChannel as IDefaultCommunicationTimeouts;

                    if (defaultTimeouts != null)
                    {
                        return defaultTimeouts.CloseTimeout;
                    }
                    else
                    {
                        return ServiceDefaults.CloseTimeout;
                    }
                }
            }

            private TimeSpan DefaultReceiveTimeout
            {
                get
                {
                    IDefaultCommunicationTimeouts defaultTimeouts = _innerChannel as IDefaultCommunicationTimeouts;

                    if (defaultTimeouts != null)
                    {
                        return defaultTimeouts.ReceiveTimeout;
                    }
                    else
                    {
                        return ServiceDefaults.ReceiveTimeout;
                    }
                }
            }

            // kick off an async receive so that we notice when the server is trying to shutdown
            private void StartBackgroundReceive()
            {
                if (s_receiveAsyncCallback == null)
                {
                    s_receiveAsyncCallback = Fx.ThunkCallback(new AsyncCallback(ReceiveAsyncCallback));
                }

                IAsyncResult result = null;
                Exception exceptionFromBeginReceive = null;
                try
                {
                    result = _innerChannel.BeginReceive(TimeSpan.MaxValue, s_receiveAsyncCallback, this);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    exceptionFromBeginReceive = e;
                }

                if (exceptionFromBeginReceive != null)
                {
                    _pendingMessages.EnqueueAndDispatch(exceptionFromBeginReceive, _messageDequeuedCallback, false);
                }
                else if (result.CompletedSynchronously)
                {
                    if (s_receiveThreadSchedulerCallback == null)
                    {
                        s_receiveThreadSchedulerCallback = new Action<object>(ReceiveThreadSchedulerCallback);
                    }

                    //Deskcop: IOThreadScheduler.ScheduleCallbackLowPriNoFlow(receiveThreadSchedulerCallback, result);
                    Task.Factory.StartNew(s_receiveThreadSchedulerCallback, result, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                }
            }

            private static void ReceiveThreadSchedulerCallback(object state)
            {
                IAsyncResult result = (IAsyncResult)state;
                AutoCloseDuplexSessionChannel thisPtr = (AutoCloseDuplexSessionChannel)result.AsyncState;
                thisPtr.OnReceive(result);
            }

            private static void ReceiveAsyncCallback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }
                AutoCloseDuplexSessionChannel thisPtr = (AutoCloseDuplexSessionChannel)result.AsyncState;
                thisPtr.OnReceive(result);
            }

            private void OnReceive(IAsyncResult result)
            {
                Message message = null;
                Exception receiveException = null;
                try
                {
                    message = _innerChannel.EndReceive(result);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    receiveException = e;
                }

                if (receiveException != null)
                {
                    _pendingMessages.EnqueueAndDispatch(receiveException, _messageDequeuedCallback, true);
                }
                else
                {
                    if (message == null)
                    {
                        // we've hit end of session, time for auto-close to kick in
                        _pendingMessages.Shutdown();
                        CloseInnerChannel();
                    }
                    else
                    {
                        _pendingMessages.EnqueueAndDispatch(message, _messageDequeuedCallback, true);
                    }
                }
            }

            private void CloseInnerChannel()
            {
                lock (ThisLock)
                {
                    if (!_closeState.TryBackgroundClose() || State != CommunicationState.Opened)
                    {
                        return;
                    }
                }

                IAsyncResult result = null;
                Exception backgroundCloseException = null;
                try
                {
                    if (s_closeInnerChannelCallback == null)
                    {
                        s_closeInnerChannelCallback = Fx.ThunkCallback(new AsyncCallback(CloseInnerChannelCallback));
                    }
                    result = _innerChannel.BeginClose(s_closeInnerChannelCallback, this);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    _innerChannel.Abort();

                    backgroundCloseException = e;
                }

                if (backgroundCloseException != null)
                {
                    // stash away exception to throw out of user's Close()
                    _closeState.CaptureBackgroundException(backgroundCloseException);
                }
                else if (result.CompletedSynchronously)
                {
                    OnCloseInnerChannel(result);
                }
            }

            private static void CloseInnerChannelCallback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ((AutoCloseDuplexSessionChannel)result.AsyncState).OnCloseInnerChannel(result);
            }

            private void OnCloseInnerChannel(IAsyncResult result)
            {
                Exception backgroundCloseException = null;
                try
                {
                    _innerChannel.EndClose(result);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    _innerChannel.Abort();
                    backgroundCloseException = e;
                }

                if (backgroundCloseException != null)
                {
                    // stash away exception to throw out of user's Close()
                    _closeState.CaptureBackgroundException(backgroundCloseException);
                }
                else
                {
                    _closeState.FinishBackgroundClose();
                }
            }

            public Message Receive()
            {
                return Receive(DefaultReceiveTimeout);
            }

            public Message Receive(TimeSpan timeout)
            {
                return _pendingMessages.Dequeue(timeout);
            }

            public IAsyncResult BeginReceive(AsyncCallback callback, object state)
            {
                return BeginReceive(DefaultReceiveTimeout, callback, state);
            }

            public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _pendingMessages.BeginDequeue(timeout, callback, state);
            }

            public Message EndReceive(IAsyncResult result)
            {
                throw NotImplemented.ByDesign;
            }

            public bool TryReceive(TimeSpan timeout, out Message message)
            {
                return _pendingMessages.Dequeue(timeout, out message);
            }

            public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _pendingMessages.BeginDequeue(timeout, callback, state);
            }

            public bool EndTryReceive(IAsyncResult result, out Message message)
            {
                return _pendingMessages.EndDequeue(result, out message);
            }

            public bool WaitForMessage(TimeSpan timeout)
            {
                return _pendingMessages.WaitForItem(timeout);
            }

            public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _pendingMessages.BeginWaitForItem(timeout, callback, state);
            }

            public bool EndWaitForMessage(IAsyncResult result)
            {
                return _pendingMessages.EndWaitForItem(result);
            }

            public T GetProperty<T>() where T : class
            {
                return _innerChannel.GetProperty<T>();
            }

            public void Abort()
            {
                _innerChannel.Abort();
                Cleanup();
            }

            public void Close()
            {
                Close(DefaultCloseTimeout);
            }

            public void Close(TimeSpan timeout)
            {
                bool performChannelClose;
                lock (ThisLock)
                {
                    performChannelClose = _closeState.TryUserClose();
                }
                if (performChannelClose)
                {
                    _innerChannel.Close(timeout);
                }
                else
                {
                    _closeState.WaitForBackgroundClose(timeout);
                }
                Cleanup();
            }

            public IAsyncResult BeginClose(AsyncCallback callback, object state)
            {
                return BeginClose(DefaultCloseTimeout, callback, state);
            }

            public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                bool performChannelClose;
                lock (ThisLock)
                {
                    performChannelClose = _closeState.TryUserClose();
                }
                if (performChannelClose)
                {
                    return _innerChannel.BeginClose(timeout, callback, state);
                }
                else
                {
                    return _closeState.BeginWaitForBackgroundClose(timeout, callback, state);
                }
            }

            public void EndClose(IAsyncResult result)
            {
                // don't need to lock here since BeginClose is the sync-point
                if (_closeState.TryUserClose())
                {
                    _innerChannel.EndClose(result);
                }
                else
                {
                    _closeState.EndWaitForBackgroundClose(result);
                }
                Cleanup();
            }

            // called from both Abort and Close paths
            private void Cleanup()
            {
                _pendingMessages.Dispose();
            }

            public void Open()
            {
                _innerChannel.Open();
                StartBackgroundReceive();
            }

            public void Open(TimeSpan timeout)
            {
                _innerChannel.Open(timeout);
                StartBackgroundReceive();
            }

            public IAsyncResult BeginOpen(AsyncCallback callback, object state)
            {
                return _innerChannel.BeginOpen(callback, state);
            }

            public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _innerChannel.BeginOpen(timeout, callback, state);
            }

            public void EndOpen(IAsyncResult result)
            {
                _innerChannel.EndOpen(result);
                StartBackgroundReceive();
            }

            public void Send(Message message)
            {
                Send(message);
            }

            public void Send(Message message, TimeSpan timeout)
            {
                Send(message, timeout);
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            {
                return _innerChannel.BeginSend(message, callback, state);
            }

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _innerChannel.BeginSend(message, timeout, callback, state);
            }

            public void EndSend(IAsyncResult result)
            {
                _innerChannel.EndSend(result);
            }

            internal class CloseState
            {
                private bool _userClose;
                private InputQueue<object> _backgroundCloseData;

                public CloseState()
                {
                }

                public bool TryBackgroundClose()
                {
                    Fx.Assert(_backgroundCloseData == null, "can't try twice");
                    if (!_userClose)
                    {
                        _backgroundCloseData = new InputQueue<object>();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                public void FinishBackgroundClose()
                {
                    Fx.Assert(_backgroundCloseData != null, "Only callable from background close");
                    _backgroundCloseData.Close();
                }

                public bool TryUserClose()
                {
                    if (_backgroundCloseData == null)
                    {
                        _userClose = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                public void WaitForBackgroundClose(TimeSpan timeout)
                {
                    Fx.Assert(_backgroundCloseData != null, "Need to check background close first");
                    object dummy = _backgroundCloseData.Dequeue(timeout);
                    Fx.Assert(dummy == null, "we should get an exception or null");
                }

                public IAsyncResult BeginWaitForBackgroundClose(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    Fx.Assert(_backgroundCloseData != null, "Need to check background close first");
                    return _backgroundCloseData.BeginDequeue(timeout, callback, state);
                }

                public void EndWaitForBackgroundClose(IAsyncResult result)
                {
                    Fx.Assert(_backgroundCloseData != null, "Need to check background close first");
                    object dummy = _backgroundCloseData.EndDequeue(result);
                    Fx.Assert(dummy == null, "we should get an exception or null");
                }

                public void CaptureBackgroundException(Exception exception)
                {
                    _backgroundCloseData.EnqueueAndDispatch(exception, null, true);
                }
            }
        }
    }
}
