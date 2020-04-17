// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class TransportDuplexSessionChannel : TransportOutputChannel, IDuplexSessionChannel
    {
        private BufferManager _bufferManager;
        private IDuplexSession _duplexSession;
        private bool _isInputSessionClosed;
        private bool _isOutputSessionClosed;
        private MessageEncoder _messageEncoder;
        private SynchronizedMessageSource _messageSource;
        private SecurityMessageProperty _remoteSecurity;
        private EndpointAddress _localAddress;
        private SemaphoreSlim _sendLock;
        private Uri _localVia;
        private static Action<object> s_onWriteComplete = new Action<object>(OnWriteComplete);

        protected TransportDuplexSessionChannel(
                  ChannelManagerBase manager,
                  ITransportFactorySettings settings,
                  EndpointAddress localAddress,
                  Uri localVia,
                  EndpointAddress remoteAddresss,
                  Uri via)
                : base(manager, remoteAddresss, via, settings.ManualAddressing, settings.MessageVersion)
        {
            _localAddress = localAddress;
            _localVia = localVia;
            _bufferManager = settings.BufferManager;
            _sendLock = new SemaphoreSlim(1);
            _messageEncoder = settings.MessageEncoderFactory.CreateSessionEncoder();
            this.Session = new ConnectionDuplexSession(this);
        }

        public EndpointAddress LocalAddress
        {
            get { return _localAddress; }
        }

        public SecurityMessageProperty RemoteSecurity
        {
            get { return _remoteSecurity; }
            protected set { _remoteSecurity = value; }
        }

        public IDuplexSession Session
        {
            get { return _duplexSession; }
            protected set { _duplexSession = value; }
        }

        protected BufferManager BufferManager
        {
            get
            {
                return _bufferManager;
            }
        }

        protected MessageEncoder MessageEncoder
        {
            get { return _messageEncoder; }
            set { _messageEncoder = value; }
        }

        internal SynchronizedMessageSource MessageSource
        {
            get { return _messageSource; }
        }

        protected abstract bool IsStreamedOutput { get; }

        public Message Receive()
        {
            return this.Receive(this.DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message = null;
            if (DoneReceivingInCurrentState())
            {
                return null;
            }

            bool shouldFault = true;
            try
            {
                message = _messageSource.Receive(timeout);
                this.OnReceiveMessage(message);
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
                        message = null;
                    }

                    this.Fault();
                }
            }
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
                message = await _messageSource.ReceiveAsync(timeout);
                this.OnReceiveMessage(message);
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
                        message = null;
                    }

                    this.Fault();
                }
            }
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.ReceiveAsync(timeout).ToApm(callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.ReceiveAsync(timeout).ToApm(callback, state);
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

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            try
            {
                message = this.Receive(timeout);
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
                bool success = await _messageSource.WaitForMessageAsync(timeout);
                shouldFault = !success; // need to fault if we've timed out because we're now toast
                return success;
            }
            finally
            {
                if (shouldFault)
                {
                    this.Fault();
                }
            }
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            if (DoneReceivingInCurrentState())
            {
                return true;
            }

            bool shouldFault = true;
            try
            {
                bool success = _messageSource.WaitForMessage(timeout);
                shouldFault = !success; // need to fault if we've timed out because we're now toast
                return success;
            }
            finally
            {
                if (shouldFault)
                {
                    this.Fault();
                }
            }
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.WaitForMessageAsync(timeout).ToApm(callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return result.ToApmEnd<bool>();
        }

        protected void SetMessageSource(IMessageSource messageSource)
        {
            _messageSource = new SynchronizedMessageSource(messageSource);
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
            if (!await _sendLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.CloseTimeout(string.Format(SRServiceModel.CloseTimedOut, timeout));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                                string.Format(SRServiceModel.CloseTimedOut, timeout),
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
                    await this.CloseOutputSessionCoreAsync(timeout);
                    this.OnOutputSessionClosed(ref timeoutHelper);
                    shouldFault = false;
                }
                finally
                {
                    if (shouldFault)
                    {
                        this.Fault();
                    }
                }
            }
            finally
            {
                _sendLock.Release();
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
            if (!_sendLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.CloseTimeout(string.Format(SRServiceModel.CloseTimedOut, timeout));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                                string.Format(SRServiceModel.CloseTimedOut, timeout),
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
                    this.CloseOutputSessionCore(timeout);
                    this.OnOutputSessionClosed(ref timeoutHelper);
                    shouldFault = false;
                }
                finally
                {
                    if (shouldFault)
                    {
                        this.Fault();
                    }
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        // used to return cached connection to the pool/reader pool
        protected abstract void ReturnConnectionIfNecessary(bool abort, TimeSpan timeout);

        protected override void OnAbort()
        {
            this.ReturnConnectionIfNecessary(true, TimeSpan.Zero);
        }

        protected override void OnFaulted()
        {
            base.OnFaulted();
            this.ReturnConnectionIfNecessary(true, TimeSpan.Zero);
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await this.CloseOutputSessionAsync(timeoutHelper.RemainingTime());

            // close input session if necessary
            if (!_isInputSessionClosed)
            {
                await this.EnsureInputClosedAsync(timeoutHelper.RemainingTime());
                this.OnInputSessionClosed();
            }

            this.CompleteClose(timeoutHelper.RemainingTime());
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            this.CloseOutputSession(timeoutHelper.RemainingTime());

            // close input session if necessary
            if (!_isInputSessionClosed)
            {
                this.EnsureInputClosed(timeoutHelper.RemainingTime());
                this.OnInputSessionClosed();
            }

            this.CompleteClose(timeoutHelper.RemainingTime());
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
                this.OnInputSessionClosed();
            }
            else
            {
                this.PrepareMessage(message);
            }
        }

        protected void ApplyChannelBinding(Message message)
        {
            //ChannelBindingUtility.TryAddToMessage(this.channelBindingToken, message, false);
        }

        protected virtual void PrepareMessage(Message message)
        {
            message.Properties.Via = _localVia;

            this.ApplyChannelBinding(message);

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
                        this.LocalAddress != null && this.LocalAddress.Uri != null ? this.LocalAddress.Uri.AbsoluteUri : string.Empty,
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
            this.ThrowIfDisposedOrNotOpen();

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!await _sendLock.WaitAsync(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                            string.Format(SRServiceModel.SendToViaTimedOut, Via, timeout),
                                            TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            byte[] buffer = null;

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                this.ThrowIfDisposedOrNotOpen();
                this.ThrowIfOutputSessionClosed();

                bool success = false;
                try
                {
                    this.ApplyChannelBinding(message);

                    var tcs = new TaskCompletionSource<bool>(this);

                    AsyncCompletionResult completionResult;
                    if (this.IsStreamedOutput)
                    {
                        completionResult = this.StartWritingStreamedMessage(message, timeoutHelper.RemainingTime(), s_onWriteComplete, this);
                    }
                    else
                    {
                        bool allowOutputBatching;
                        ArraySegment<byte> messageData;
                        allowOutputBatching = message.Properties.AllowOutputBatching;
                        messageData = this.EncodeMessage(message);

                        buffer = messageData.Array;
                        completionResult = this.StartWritingBufferedMessage(
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

                    this.FinishWritingMessage();

                    success = true;
                    if (WcfEventSource.Instance.MessageSentByTransportIsEnabled())
                    {
                        EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                        WcfEventSource.Instance.MessageSentByTransport(eventTraceActivity, this.RemoteAddress.Uri.AbsoluteUri);
                    }
                }
                finally
                {
                    if (!success)
                    {
                        this.Fault();
                    }
                }
            }
            finally
            {
                _sendLock.Release();
            }
            if (buffer != null)
            {
                _bufferManager.ReturnBuffer(buffer);
            }
        }

        private static void OnWriteComplete(object state)
        {
            if (state == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("state");
            }

            var tcs = state as TaskCompletionSource<bool>;
            if (tcs == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("state", SRServiceModel.SPS_InvalidAsyncResult);
            }

            tcs.TrySetResult(true);
        }


        protected override void OnSend(Message message, TimeSpan timeout)
        {
            this.ThrowIfDisposedOrNotOpen();

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // If timeout == TimeSpan.MaxValue, then we want to pass Timeout.Infinite as 
            // SemaphoreSlim doesn't accept timeouts > Int32.MaxValue.
            // Using TimeoutHelper.RemainingTime() would yield a value less than TimeSpan.MaxValue
            // and would result in the value Int32.MaxValue so we must use the original timeout specified.
            if (!_sendLock.Wait(TimeoutHelper.ToMilliseconds(timeout)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(
                                            string.Format(SRServiceModel.SendToViaTimedOut, Via, timeout),
                                            TimeoutHelper.CreateEnterTimedOutException(timeout)));
            }

            try
            {
                // check again in case the previous send faulted while we were waiting for the lock
                this.ThrowIfDisposedOrNotOpen();
                this.ThrowIfOutputSessionClosed();

                bool success = false;
                try
                {
                    this.ApplyChannelBinding(message);

                    this.OnSendCore(message, timeoutHelper.RemainingTime());
                    success = true;
                    if (WcfEventSource.Instance.MessageSentByTransportIsEnabled())
                    {
                        EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                        WcfEventSource.Instance.MessageSentByTransport(eventTraceActivity, this.RemoteAddress.Uri.AbsoluteUri);
                    }
                }
                finally
                {
                    if (!success)
                    {
                        this.Fault();
                    }
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        // cleanup after the framing handshake has completed
        protected abstract void CompleteClose(TimeSpan timeout);

        // must be called under sendLock 
        private void ThrowIfOutputSessionClosed()
        {
            if (_isOutputSessionClosed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SendCannotBeCalledAfterCloseOutputSession));
            }
        }

        private async Task EnsureInputClosedAsync(TimeSpan timeout)
        {
            Message message = await this.MessageSource.ReceiveAsync(timeout);
            if (message != null)
            {
                using (message)
                {
                    ProtocolException error = ProtocolException.ReceiveShutdownReturnedNonNull(message);
                    throw TraceUtility.ThrowHelperError(error, message);
                }
            }
        }

        private void EnsureInputClosed(TimeSpan timeout)
        {
            Message message = this.MessageSource.Receive(timeout);
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
                this.ReturnConnectionIfNecessary(false, timeoutHelper.RemainingTime());
            }
        }

        public class ConnectionDuplexSession : IDuplexSession
        {
            private static UriGenerator s_uriGenerator;
            private TransportDuplexSessionChannel _channel;
            private string _id;

            public ConnectionDuplexSession(TransportDuplexSessionChannel channel)
                : base()
            {
                _channel = channel;
            }

            public string Id
            {
                get
                {
                    if (_id == null)
                    {
                        lock (_channel)
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

            public TransportDuplexSessionChannel Channel
            {
                get { return _channel; }
            }

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
                return this.BeginCloseOutputSession(_channel.DefaultCloseTimeout, callback, state);
            }

            public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _channel.CloseOutputSessionAsync(timeout).ToApm(callback, state);
            }

            public void EndCloseOutputSession(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            public void CloseOutputSession()
            {
                this.CloseOutputSession(_channel.DefaultCloseTimeout);
            }

            public void CloseOutputSession(TimeSpan timeout)
            {
                _channel.CloseOutputSession(timeout);
            }
        }
    }
}
