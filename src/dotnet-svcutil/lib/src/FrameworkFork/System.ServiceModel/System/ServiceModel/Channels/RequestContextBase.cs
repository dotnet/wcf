// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Channels
{
    internal abstract class RequestContextBase : RequestContext
    {
        private TimeSpan _defaultSendTimeout;
        private TimeSpan _defaultCloseTimeout;
        private CommunicationState _state = CommunicationState.Opened;
        private Message _requestMessage;
        private Exception _requestMessageException;
        private bool _replySent;
        private bool _replyInitiated;
        private bool _aborted;
        private object _thisLock = new object();

        protected RequestContextBase(Message requestMessage, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        {
            _defaultSendTimeout = defaultSendTimeout;
            _defaultCloseTimeout = defaultCloseTimeout;
            _requestMessage = requestMessage;
        }

        public void ReInitialize(Message requestMessage)
        {
            _state = CommunicationState.Opened;
            _requestMessageException = null;
            _replySent = false;
            _replyInitiated = false;
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

        protected bool ReplyInitiated
        {
            get { return _replyInitiated; }
        }

        protected object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

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

        public TimeSpan DefaultSendTimeout
        {
            get { return _defaultSendTimeout; }
        }

        public override void Abort()
        {
            lock (ThisLock)
            {
                if (_state == CommunicationState.Closed)
                    return;

                _state = CommunicationState.Closing;

                _aborted = true;
            }

            try
            {
                this.OnAbort();
            }
            finally
            {
                _state = CommunicationState.Closed;
            }
        }

        public override void Close()
        {
            this.Close(_defaultCloseTimeout);
        }

        public override void Close(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", timeout, SRServiceModel.ValueMustBeNonNegative));
            }

            bool sendAck = false;
            lock (ThisLock)
            {
                if (_state != CommunicationState.Opened)
                    return;

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
                    this.Abort();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (_replySent)
            {
                this.Close();
            }
            else
            {
                this.Abort();
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationObjectAbortedException(SRServiceModel.RequestContextAborted));
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }

            if (_replyInitiated)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ReplyAlreadySent));
        }

        /// <summary>
        /// Attempts to initiate the reply. If a reply is not initiated already (and the object is opened), 
        /// then it initiates the reply and returns true. Otherwise, it returns false.
        /// </summary>
        protected bool TryInitiateReply()
        {
            lock (_thisLock)
            {
                if ((_state != CommunicationState.Opened) || _replyInitiated)
                {
                    return false;
                }
                else
                {
                    _replyInitiated = true;
                    return true;
                }
            }
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            return this.BeginReply(message, _defaultSendTimeout, callback, state);
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            // "null" is a valid reply (signals a 202-style "ack"), so we don't have a null-check here
            lock (_thisLock)
            {
                this.ThrowIfInvalidReply();
                _replyInitiated = true;
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
            this.Reply(message, _defaultSendTimeout);
        }

        public override void Reply(Message message, TimeSpan timeout)
        {
            // "null" is a valid reply (signals a 202-style "ack"), so we don't have a null-check here
            lock (_thisLock)
            {
                this.ThrowIfInvalidReply();
                _replyInitiated = true;
            }

            this.OnReply(message, timeout);
            _replySent = true;
        }

        // This method is designed for WebSocket only, and will only be used once the WebSocket response was sent.
        // For WebSocket, we never call HttpRequestContext.Reply to send the response back. 
        // Instead we call AcceptWebSocket directly. So we need to set the replyInitiated and 
        // replySent boolean to be true once the response was sent successfully. Otherwise when we 
        // are disposing the HttpRequestContext, we will see a bunch of warnings in trace log.
        protected void SetReplySent()
        {
            lock (_thisLock)
            {
                this.ThrowIfInvalidReply();
                _replyInitiated = true;
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
                    return;
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
