// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class RequestChannel : ChannelBase, IRequestChannel, IAsyncRequestChannel
    {
        private bool _manualAddressing;
        private List<IRequestBase> _outstandingRequests = new List<IRequestBase>();
        private EndpointAddress _to;
        private Uri _via;
        private TaskCompletionSource<object> _closedTcs;

        private bool _closed;

        protected RequestChannel(ChannelManagerBase channelFactory, EndpointAddress to, Uri via, bool manualAddressing)
            : base(channelFactory)
        {
            if (!manualAddressing)
            {
                if (to == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("to");
                }
            }

            _manualAddressing = manualAddressing;
            _to = to;
            _via = via;
        }

        protected bool ManualAddressing
        {
            get
            {
                return _manualAddressing;
            }
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return _to;
            }
        }

        public Uri Via
        {
            get
            {
                return _via;
            }
        }

        protected void AbortPendingRequests()
        {
            IRequestBase[] requestsToAbort = CopyPendingRequests(false);

            if (requestsToAbort != null)
            {
                foreach (IRequestBase request in requestsToAbort)
                {
                    request.Abort(this);
                }
            }
        }

        protected IAsyncResult BeginWaitForPendingRequests(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return WaitForPendingRequestsAsync(timeout).ToApm(callback, state);
        }

        protected void EndWaitForPendingRequests(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private void FinishClose()
        {
            lock (_outstandingRequests)
            {
                if (!_closed)
                {
                    _closed = true;
                    if (_closedTcs != null)
                    {
                        _closedTcs.TrySetResult(null);
                        _closedTcs = null;
                    }
                }
            }
        }

        private IRequestBase[] SetupWaitForPendingRequests()
        {
            return this.CopyPendingRequests(true);
        }

        protected void WaitForPendingRequests(TimeSpan timeout)
        {
            WaitForPendingRequestsAsync(timeout).Wait();
        }

        internal protected async Task WaitForPendingRequestsAsync(TimeSpan timeout)
        {
            IRequestBase[] pendingRequests = SetupWaitForPendingRequests();
            if (pendingRequests != null)
            {
                if (!await _closedTcs.Task.AwaitWithTimeout(timeout))
                {
                    foreach (IRequestBase request in pendingRequests)
                    {
                        request.Abort(this);
                    }
                }
            }
            FinishClose();
        }

        private IRequestBase[] CopyPendingRequests(bool createTcsIfNecessary)
        {
            IRequestBase[] requests = null;

            lock (_outstandingRequests)
            {
                if (_outstandingRequests.Count > 0)
                {
                    requests = new IRequestBase[_outstandingRequests.Count];
                    _outstandingRequests.CopyTo(requests);
                    _outstandingRequests.Clear();

                    if (createTcsIfNecessary && _closedTcs == null)
                    {
                        _closedTcs = new TaskCompletionSource<object>();
                    }
                }
            }

            return requests;
        }

        protected void FaultPendingRequests()
        {
            IRequestBase[] requestsToFault = CopyPendingRequests(false);

            if (requestsToFault != null)
            {
                foreach (IRequestBase request in requestsToFault)
                {
                    request.Fault(this);
                }
            }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IRequestChannel))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return default(T);
        }

        protected override void OnAbort()
        {
            AbortPendingRequests();
        }

        private void ReleaseRequest(IRequestBase request)
        {
            if (request != null)
            {
                // Synchronization of OnReleaseRequest is the 
                // responsibility of the concrete implementation of request.
                request.OnReleaseRequest();
            }

            lock (_outstandingRequests)
            {
                // Remove supports the connection having been removed, so don't need extra Contains() check,
                // even though this may have been removed by Abort()
                _outstandingRequests.Remove(request);
                if (_outstandingRequests.Count == 0)
                {
                    if (!_closed && _closedTcs != null)
                    {
                        _closedTcs.TrySetResult(null);
                        _closedTcs = null;
                    }
                }
            }
        }

        private void TrackRequest(IRequestBase request)
        {
            lock (_outstandingRequests)
            {
                ThrowIfDisposedOrNotOpen(); // make sure that we haven't already snapshot our collection
                _outstandingRequests.Add(request);
            }
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            return this.BeginRequest(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return RequestAsync(message, timeout).ToApm(callback, state);
        }

        protected abstract IAsyncRequest CreateAsyncRequest(Message message);

        public Message EndRequest(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public Message Request(Message message)
        {
            return this.Request(message, this.DefaultSendTimeout);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            return RequestAsyncInternal(message, timeout).WaitForCompletion();
        }

        public Task<Message> RequestAsync(Message message)
        {
            return RequestAsync(message, this.DefaultSendTimeout);
        }

        private async Task<Message> RequestAsyncInternal(Message message, TimeSpan timeout)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            return await RequestAsync(message, timeout);
        }

        public async Task<Message> RequestAsync(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }

            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("timeout", timeout, SRServiceModel.SFxTimeoutOutOfRange0));

            ThrowIfDisposedOrNotOpen();

            AddHeadersTo(message);
            IAsyncRequest request = CreateAsyncRequest(message);
            TrackRequest(request);
            try
            {
                Message reply;
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                TimeSpan savedTimeout = timeoutHelper.RemainingTime();
                try
                {
                    await request.SendRequestAsync(message, timeoutHelper);
                }
                catch (TimeoutException timeoutException)
                {
                    throw TraceUtility.ThrowHelperError(new TimeoutException(string.Format(SRServiceModel.RequestChannelSendTimedOut, savedTimeout),
                        timeoutException), message);
                }

                savedTimeout = timeoutHelper.RemainingTime();

                try
                {
                    reply = await request.ReceiveReplyAsync(timeoutHelper);
                }
                catch (TimeoutException timeoutException)
                {
                    throw TraceUtility.ThrowHelperError(new TimeoutException(string.Format(SRServiceModel.RequestChannelWaitForReplyTimedOut, savedTimeout),
                        timeoutException), message);
                }

                return reply;
            }
            finally
            {
                ReleaseRequest(request);
            }
        }

        protected virtual void AddHeadersTo(Message message)
        {
            if (!_manualAddressing && _to != null)
            {
                _to.ApplyTo(message);
            }
        }
    }

    public interface IRequestBase
    {
        void Abort(RequestChannel requestChannel);
        void Fault(RequestChannel requestChannel);
        void OnReleaseRequest();
    }

    public interface IRequest : IRequestBase
    {
        void SendRequest(Message message, TimeSpan timeout);
        Message WaitForReply(TimeSpan timeout);
    }

    public interface IAsyncRequest : IRequestBase
    {
        Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper);
        Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper);
    }
}
