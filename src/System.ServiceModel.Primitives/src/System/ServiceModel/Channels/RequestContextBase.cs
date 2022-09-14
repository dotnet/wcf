// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;

namespace System.ServiceModel.Channels
{
    internal abstract class RequestContextBase : RequestContext
    {
        private TimeSpan _defaultCloseTimeout;
        private CommunicationState _state = CommunicationState.Opened;
        private Message _requestMessage;
        private Exception _requestMessageException;
        private bool _replySent;
        private bool _aborted;

        protected RequestContextBase(Message requestMessage, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        {
            DefaultSendTimeout = defaultSendTimeout;
            _defaultCloseTimeout = defaultCloseTimeout;
            _requestMessage = requestMessage;
        }

        public void ReInitialize(Message requestMessage)
        {
            _state = CommunicationState.Opened;
            _requestMessageException = null;
            _replySent = false;
            ReplyInitiated = false;
            _aborted = false;
            _requestMessage = requestMessage;
        }

        public override Message RequestMessage
        {
            get
            {
                if (_requestMessageException != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_requestMessageException);
                }

                return _requestMessage;
            }
        }

        protected void SetRequestMessage(Message requestMessage)
        {
            Fx.Assert(_requestMessageException == null, "Cannot have both a requestMessage and a requestException.");
            _requestMessage = requestMessage;
        }

        protected void SetRequestMessage(Exception requestMessageException)
        {
            Fx.Assert(_requestMessage == null, "Cannot have both a requestMessage and a requestException.");
            _requestMessageException = requestMessageException;
        }

        protected bool ReplyInitiated { get; private set; }

        protected object ThisLock { get; } = new object();

        public bool Aborted
        {
            get
            {
                return _aborted;
            }
        }

        public TimeSpan DefaultCloseTimeout
        {
            get { return _defaultCloseTimeout; }
        }

        public TimeSpan DefaultSendTimeout { get; }

        public override void Abort()
        {
            lock (ThisLock)
            {
                if (_state == CommunicationState.Closed)
                {
                    return;
                }

                _state = CommunicationState.Closing;

                _aborted = true;
            }

            try
            {
                OnAbort();
            }
            finally
            {
                _state = CommunicationState.Closed;
            }
        }

        public override void Close()
        {
            Close(_defaultCloseTimeout);
        }

        public override void Close(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.ValueMustBeNonNegative));
            }

            bool sendAck = false;
            lock (ThisLock)
            {
                if (_state != CommunicationState.Opened)
                {
                    return;
                }

                if (TryInitiateReply())
                {
                    sendAck = true;
                }

                _state = CommunicationState.Closing;
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool throwing = true;

            try
            {
                if (sendAck)
                {
                    OnReply(null, timeoutHelper.RemainingTime());
                }

                OnClose(timeoutHelper.RemainingTime());
                _state = CommunicationState.Closed;
                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    Abort();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
            {
                return;
            }

            if (_replySent)
            {
                Close();
            }
            else
            {
                Abort();
            }
        }

        protected abstract void OnAbort();
        protected abstract void OnClose(TimeSpan timeout);
        protected abstract void OnReply(Message message, TimeSpan timeout);
        protected abstract IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        protected abstract void OnEndReply(IAsyncResult result);

        protected void ThrowIfInvalidReply()
        {
            if (_state == CommunicationState.Closed || _state == CommunicationState.Closing)
            {
                if (_aborted)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationObjectAbortedException(SRP.RequestContextAborted));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
                }
            }

            if (ReplyInitiated)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ReplyAlreadySent));
            }
        }

        /// <summary>
        /// Attempts to initiate the reply. If a reply is not initiated already (and the object is opened), 
        /// then it initiates the reply and returns true. Otherwise, it returns false.
        /// </summary>
        protected bool TryInitiateReply()
        {
            lock (ThisLock)
            {
                if ((_state != CommunicationState.Opened) || ReplyInitiated)
                {
                    return false;
                }
                else
                {
                    ReplyInitiated = true;
                    return true;
                }
            }
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            return BeginReply(message, DefaultSendTimeout, callback, state);
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            // "null" is a valid reply (signals a 202-style "ack"), so we don't have a null-check here
            lock (ThisLock)
            {
                ThrowIfInvalidReply();
                ReplyInitiated = true;
            }

            return OnBeginReply(message, timeout, callback, state);
        }

        public override void EndReply(IAsyncResult result)
        {
            OnEndReply(result);
            _replySent = true;
        }

        public override void Reply(Message message)
        {
            Reply(message, DefaultSendTimeout);
        }

        public override void Reply(Message message, TimeSpan timeout)
        {
            // "null" is a valid reply (signals a 202-style "ack"), so we don't have a null-check here
            lock (ThisLock)
            {
                ThrowIfInvalidReply();
                ReplyInitiated = true;
            }

            OnReply(message, timeout);
            _replySent = true;
        }

        // This method is designed for WebSocket only, and will only be used once the WebSocket response was sent.
        // For WebSocket, we never call HttpRequestContext.Reply to send the response back. 
        // Instead we call AcceptWebSocket directly. So we need to set the replyInitiated and 
        // replySent boolean to be true once the response was sent successfully. Otherwise when we 
        // are disposing the HttpRequestContext, we will see a bunch of warnings in trace log.
        protected void SetReplySent()
        {
            lock (ThisLock)
            {
                ThrowIfInvalidReply();
                ReplyInitiated = true;
            }

            _replySent = true;
        }
    }

    internal class RequestContextMessageProperty : IDisposable
    {
        private RequestContext _context;
        private object _thisLock = new object();

        public RequestContextMessageProperty(RequestContext context)
        {
            _context = context;
        }

        public static string Name
        {
            get { return "requestContext"; }
        }

        void IDisposable.Dispose()
        {
            bool success = false;
            RequestContext thisContext;

            lock (_thisLock)
            {
                if (_context == null)
                {
                    return;
                }

                thisContext = _context;
                _context = null;
            }

            try
            {
                thisContext.Close();
                success = true;
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException e)
            {
                if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                {
                    WcfEventSource.Instance.CloseTimeout(e.Message);
                }
            }
            finally
            {
                if (!success)
                {
                    thisContext.Abort();
                }
            }
        }
    }
}
