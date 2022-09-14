// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class TransportDuplexSessionChannel : TransportOutputChannel, IDuplexSessionChannel, IAsyncDuplexSessionChannel
    {
        private bool _isInputSessionClosed;
        private bool _isOutputSessionClosed;
        private Uri _localVia;
        private static Action<object> s_onWriteComplete = new Action<object>(OnWriteComplete);

        protected TransportDuplexSessionChannel(
                  ChannelManagerBase manager,
                  ITransportFactorySettings settings,
                  EndpointAddress localAddress,
                  Uri localVia,
                  EndpointAddress remoteAddress,
                  Uri via)
                : base(manager, remoteAddress, via, settings.ManualAddressing, settings.MessageVersion)
        {
            LocalAddress = localAddress;
            _localVia = localVia;
            BufferManager = settings.BufferManager;
            SendLock = new SemaphoreSlim(1);
            MessageEncoder = settings.MessageEncoderFactory.CreateSessionEncoder();
            Session = new ConnectionDuplexSession(this);
        }

        public EndpointAddress LocalAddress { get; }

        public SecurityMessageProperty RemoteSecurity { get; protected set; }

        public IDuplexSession Session { get; protected set; }

        protected SemaphoreSlim SendLock { get; }

        protected BufferManager BufferManager { get; }

        protected MessageEncoder MessageEncoder { get; set; }

        internal SynchronizedMessageSource MessageSource { get; private set; }

        protected abstract bool IsStreamedOutput { get; }

        IAsyncDuplexSession ISessionChannel<IAsyncDuplexSession>.Session => Session as IAsyncDuplexSession;

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            return ReceiveAsync(timeout).GetAwaiter().GetResult();
        }

        public Task<Message> ReceiveAsync()
        {
            return ReceiveAsync(DefaultReceiveTimeout);
        }

        public async Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            Message message = null;
            if (DoneReceivingInCurrentState())
            {
                return null;
            }

            bool shouldFault = true;
            try
            {
                await TaskHelpers.EnsureDefaultTaskScheduler();
                message = await MessageSource.ReceiveAsync(timeout);
                OnReceiveMessage(message);
                shouldFault = false;
                return message;
            }
            finally
            {
                if (shouldFault)
                {
                    if (message != null)
                    {
                        message.Close();
                    }

                    Fault();
                }
            }
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ReceiveAsync(timeout).ToApm(callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ReceiveAsync(timeout).ToApm(callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            try
            {
                message = result.ToApmEnd<Message>();
                return true;
            }
            catch (TimeoutException e)
            {
                if (WcfEventSource.Instance.ReceiveTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.ReceiveTimeout(e.Message);
                }


                message = null;
                return false;
            }
        }

        public async Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
        {
            try
            {
                await TaskHelpers.EnsureDefaultTaskScheduler();
                return (true, await ReceiveAsync(timeout));
            }
            catch(TimeoutException e)
            {
                if (WcfEventSource.Instance.ReceiveTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.ReceiveTimeout(e.Message);
                }

                return (false, null);
            }
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            try
            {
                message = Receive(timeout);
                return true;
            }
            catch (TimeoutException e)
            {
                if (WcfEventSource.Instance.ReceiveTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.ReceiveTimeout(e.Message);
                }
                message = null;
                return false;
            }
        }

        public async Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            if (DoneReceivingInCurrentState())
            {
                return true;
            }

            bool shouldFault = true;
            try
            {
                await TaskHelpers.EnsureDefaultTaskScheduler();
                bool success = await MessageSource.WaitForMessageAsync(timeout);
                shouldFault = !success; // need to fault if we've timed out because we're now toast
                return success;
            }
            finally
            {
                if (shouldFault)
                {
                    Fault();
                }
            }
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return WaitForMessageAsync(timeout).GetAwaiter().GetResult();
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return WaitForMessageAsync(timeout).ToApm(callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return result.ToApmEnd<bool>();
        }

        protected void SetMessageSource(IMessageSource messageSource)
        {
            MessageSource = new SynchronizedMessageSource(messageSource);
        }

        protected abstract Task CloseOutputSessionCoreAsync(TimeSpan timeout);

        protected abstract void CloseOutputSessionCore(TimeSpan timeout);

        protected async Task CloseOutputSessionAsync(TimeSpan timeout)
        {
            ThrowIfNotOpened();
            ThrowIfFaulted();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!await SendLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.CloseTimeout(SRP.Format(SRP.CloseTimedOut, timeout));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                                SRP.Format(SRP.CloseTimedOut, timeout),
                                                TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                ThrowIfFaulted();

                // we're synchronized by sendLock here
                if (_isOutputSessionClosed)
                {
                    return;
                }

                _isOutputSessionClosed = true;
                bool shouldFault = true;
                try
                {
                    await CloseOutputSessionCoreAsync(timeout);
                    OnOutputSessionClosed(ref timeoutHelper);
                    shouldFault = false;
                }
                finally
                {
                    if (shouldFault)
                    {
                        Fault();
                    }
                }
            }
            finally
            {
                SendLock.Release();
            }
        }

        protected void CloseOutputSession(TimeSpan timeout)
        {
            ThrowIfNotOpened();
            ThrowIfFaulted();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!SendLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.CloseTimeout(SRP.Format(SRP.CloseTimedOut, timeout));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                                SRP.Format(SRP.CloseTimedOut, timeout),
                                                TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                ThrowIfFaulted();

                // we're synchronized by sendLock here
                if (_isOutputSessionClosed)
                {
                    return;
                }

                _isOutputSessionClosed = true;
                bool shouldFault = true;
                try
                {
                    CloseOutputSessionCore(timeout);
                    OnOutputSessionClosed(ref timeoutHelper);
                    shouldFault = false;
                }
                finally
                {
                    if (shouldFault)
                    {
                        Fault();
                    }
                }
            }
            finally
            {
                SendLock.Release();
            }
        }

        // used to return cached connection to the pool/reader pool
        protected abstract void ReturnConnectionIfNecessary(bool abort, TimeSpan timeout);

        protected override void OnAbort()
        {
            ReturnConnectionIfNecessary(true, TimeSpan.Zero);
        }

        protected override void OnFaulted()
        {
            base.OnFaulted();
            ReturnConnectionIfNecessary(true, TimeSpan.Zero);
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await CloseOutputSessionAsync(timeoutHelper.RemainingTime());

            // close input session if necessary
            if (!_isInputSessionClosed)
            {
                await EnsureInputClosedAsync(timeoutHelper.RemainingTime());
                OnInputSessionClosed();
            }

            CompleteClose(timeoutHelper.RemainingTime());
        }

        protected override void OnClose(TimeSpan timeout)
        {
            OnCloseAsync(timeout).GetAwaiter().GetResult();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            // clean up the CBT after transitioning to the closed state
            //ChannelBindingUtility.Dispose(ref this.channelBindingToken);
        }

        protected virtual void OnReceiveMessage(Message message)
        {
            if (message == null)
            {
                OnInputSessionClosed();
            }
            else
            {
                PrepareMessage(message);
            }
        }

        protected void ApplyChannelBinding(Message message)
        {
            //ChannelBindingUtility.TryAddToMessage(this.channelBindingToken, message, false);
        }

        protected virtual void PrepareMessage(Message message)
        {
            message.Properties.Via = _localVia;

            ApplyChannelBinding(message);

            if (FxTrace.Trace.IsEnd2EndActivityTracingEnabled)
            {
                EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                Guid relatedActivityId = EventTraceActivity.GetActivityIdFromThread();
                if (eventTraceActivity == null)
                {
                    eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                    EventTraceActivityHelper.TryAttachActivity(message, eventTraceActivity);
                }

                if (WcfEventSource.Instance.MessageReceivedByTransportIsEnabled())
                {
                    WcfEventSource.Instance.MessageReceivedByTransport(
                        eventTraceActivity,
                        LocalAddress != null && LocalAddress.Uri != null ? LocalAddress.Uri.AbsoluteUri : string.Empty,
                        relatedActivityId);
                }
            }
        }

        protected abstract AsyncCompletionResult StartWritingBufferedMessage(Message message, ArraySegment<byte> messageData, bool allowOutputBatching, TimeSpan timeout, Action<object> callback, object state);

        protected abstract AsyncCompletionResult BeginCloseOutput(TimeSpan timeout, Action<object> callback, object state);

        protected virtual void FinishWritingMessage()
        {
        }

        protected abstract ArraySegment<byte> EncodeMessage(Message message);

        protected abstract void OnSendCore(Message message, TimeSpan timeout);

        protected abstract AsyncCompletionResult StartWritingStreamedMessage(Message message, TimeSpan timeout, Action<object> callback, object state);

        protected override async Task OnSendAsync(Message message, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!await SendLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                            SRP.Format(SRP.SendToViaTimedOut, Via, timeout),
                                            TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            byte[] buffer = null;

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                ThrowIfDisposedOrNotOpen();
                ThrowIfOutputSessionClosed();

                bool success = false;
                try
                {
                    ApplyChannelBinding(message);

                    var tcs = new TaskCompletionSource<bool>(this);

                    AsyncCompletionResult completionResult;
                    if (IsStreamedOutput)
                    {
                        completionResult = StartWritingStreamedMessage(message, timeoutHelper.RemainingTime(), s_onWriteComplete, tcs);
                    }
                    else
                    {
                        bool allowOutputBatching;
                        ArraySegment<byte> messageData;
                        allowOutputBatching = message.Properties.AllowOutputBatching;
                        messageData = EncodeMessage(message);

                        buffer = messageData.Array;
                        completionResult = StartWritingBufferedMessage(
                                                                          message,
                                                                          messageData,
                                                                          allowOutputBatching,
                                                                          timeoutHelper.RemainingTime(),
                                                                          s_onWriteComplete,
                                                                          tcs);
                    }

                    if (completionResult == AsyncCompletionResult.Completed)
                    {
                        tcs.TrySetResult(true);
                    }

                    await tcs.Task;

                    FinishWritingMessage();

                    success = true;
                    if (WcfEventSource.Instance.MessageSentByTransportIsEnabled())
                    {
                        EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                        WcfEventSource.Instance.MessageSentByTransport(eventTraceActivity, RemoteAddress.Uri.AbsoluteUri);
                    }
                }
                finally
                {
                    if (!success)
                    {
                        Fault();
                    }
                }
            }
            finally
            {
                SendLock.Release();
            }
            if (buffer != null)
            {
                BufferManager.ReturnBuffer(buffer);
            }
        }

        private static void OnWriteComplete(object state)
        {
            if (state == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(state));
            }

            var tcs = state as TaskCompletionSource<bool>;
            if (tcs == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRP.SPS_InvalidAsyncResult);
            }

            tcs.TrySetResult(true);
        }


        protected override void OnSend(Message message, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!SendLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                            SRP.Format(SRP.SendToViaTimedOut, Via, timeout),
                                            TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                ThrowIfDisposedOrNotOpen();
                ThrowIfOutputSessionClosed();

                bool success = false;
                try
                {
                    ApplyChannelBinding(message);

                    OnSendCore(message, timeoutHelper.RemainingTime());
                    success = true;
                    if (WcfEventSource.Instance.MessageSentByTransportIsEnabled())
                    {
                        EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                        WcfEventSource.Instance.MessageSentByTransport(eventTraceActivity, RemoteAddress.Uri.AbsoluteUri);
                    }
                }
                finally
                {
                    if (!success)
                    {
                        Fault();
                    }
                }
            }
            finally
            {
                SendLock.Release();
            }
        }

        // cleanup after the framing handshake has completed
        protected abstract void CompleteClose(TimeSpan timeout);

        // must be called under sendLock 
        private void ThrowIfOutputSessionClosed()
        {
            if (_isOutputSessionClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SendCannotBeCalledAfterCloseOutputSession));
            }
        }

        private async Task EnsureInputClosedAsync(TimeSpan timeout)
        {
            Message message = await MessageSource.ReceiveAsync(timeout);
            if (message != null)
            {
                using (message)
                {
                    ProtocolException error = ProtocolException.ReceiveShutdownReturnedNonNull(message);
                    throw TraceUtility.ThrowHelperError(error, message);
                }
            }
        }

        private void OnInputSessionClosed()
        {
            lock (ThisLock)
            {
                if (_isInputSessionClosed)
                {
                    return;
                }

                _isInputSessionClosed = true;
            }
        }

        private void OnOutputSessionClosed(ref TimeoutHelper timeoutHelper)
        {
            bool releaseConnection = false;
            lock (ThisLock)
            {
                if (_isInputSessionClosed)
                {
                    // we're all done, release the connection
                    releaseConnection = true;
                }
            }

            if (releaseConnection)
            {
                ReturnConnectionIfNecessary(false, timeoutHelper.RemainingTime());
            }
        }

        public class ConnectionDuplexSession : IDuplexSession, IAsyncDuplexSession
        {
            private static UriGenerator s_uriGenerator;
            private string _id;

            public ConnectionDuplexSession(TransportDuplexSessionChannel channel)
                : base()
            {
                Channel = channel;
            }

            public string Id
            {
                get
                {
                    if (_id == null)
                    {
                        lock (Channel)
                        {
                            if (_id == null)
                            {
                                _id = UriGenerator.Next();
                            }
                        }
                    }

                    return _id;
                }
            }

            public TransportDuplexSessionChannel Channel { get; }

            private static UriGenerator UriGenerator
            {
                get
                {
                    if (s_uriGenerator == null)
                    {
                        s_uriGenerator = new UriGenerator();
                    }

                    return s_uriGenerator;
                }
            }

            public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
            {
                return BeginCloseOutputSession(Channel.DefaultCloseTimeout, callback, state);
            }

            public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return Channel.CloseOutputSessionAsync(timeout).ToApm(callback, state);
            }

            public void EndCloseOutputSession(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            public void CloseOutputSession()
            {
                CloseOutputSession(Channel.DefaultCloseTimeout);
            }

            public void CloseOutputSession(TimeSpan timeout)
            {
                Channel.CloseOutputSession(timeout);
            }

            public Task CloseOutputSessionAsync()
            {
                return CloseOutputSessionAsync(Channel.DefaultCloseTimeout);
            }

            public Task CloseOutputSessionAsync(TimeSpan timeout)
            {
                return Channel.CloseOutputSessionAsync(timeout);
            }
        }
    }
}
