// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel.Diagnostics;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading;

namespace System.ServiceModel.Dispatcher
{
    internal class BufferedReceiveBinder : IChannelBinder
    {
        private static Action<object> s_tryReceive = new Action<object>(BufferedReceiveBinder.TryReceive);
        private static AsyncCallback s_tryReceiveCallback = Fx.ThunkCallback(new AsyncCallback(TryReceiveCallback));

        private IChannelBinder _channelBinder;
        private InputQueue<RequestContextWrapper> _inputQueue;
        [Fx.Tag.SynchronizationObject(Blocking = true, Kind = Fx.Tag.SynchronizationKind.InterlockedNoSpin)]

        private int _pendingOperationSemaphore;

        public BufferedReceiveBinder(IChannelBinder channelBinder)
        {
            _channelBinder = channelBinder;
            _inputQueue = new InputQueue<RequestContextWrapper>();
        }

        public IChannel Channel
        {
            get { return _channelBinder.Channel; }
        }

        public bool HasSession
        {
            get { return _channelBinder.HasSession; }
        }

        public Uri ListenUri
        {
            get { return _channelBinder.ListenUri; }
        }

        public EndpointAddress LocalAddress
        {
            get { return _channelBinder.LocalAddress; }
        }

        public EndpointAddress RemoteAddress
        {
            get { return _channelBinder.RemoteAddress; }
        }

        public void Abort()
        {
            _inputQueue.Close();
            _channelBinder.Abort();
        }

        public void CloseAfterFault(TimeSpan timeout)
        {
            _inputQueue.Close();
            _channelBinder.CloseAfterFault(timeout);
        }

        // Locking:
        // Only 1 channelBinder operation call should be active at any given time. All future calls
        // will wait on the inputQueue. The semaphore is always released right before the Dispatch on the inputQueue.
        // This protects a new call racing with an existing operation that is just about to fully complete.

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            if (Interlocked.CompareExchange(ref _pendingOperationSemaphore, 1, 0) == 0)
            {
                ActionItem.Schedule(s_tryReceive, this);
            }

            RequestContextWrapper wrapper;
            bool success = _inputQueue.Dequeue(timeout, out wrapper);

            if (success && wrapper != null)
            {
                requestContext = wrapper.RequestContext;
            }
            else
            {
                requestContext = null;
            }

            return success;
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (Interlocked.CompareExchange(ref _pendingOperationSemaphore, 1, 0) == 0)
            {
                IAsyncResult result = _channelBinder.BeginTryReceive(timeout, s_tryReceiveCallback, this);
                if (result.CompletedSynchronously)
                {
                    HandleEndTryReceive(result);
                }
            }

            return _inputQueue.BeginDequeue(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            RequestContextWrapper wrapper;
            bool success = _inputQueue.EndDequeue(result, out wrapper);

            if (success && wrapper != null)
            {
                requestContext = wrapper.RequestContext;
            }
            else
            {
                requestContext = null;
            }
            return success;
        }

        public RequestContext CreateRequestContext(Message message)
        {
            return _channelBinder.CreateRequestContext(message);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            _channelBinder.Send(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelBinder.BeginSend(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _channelBinder.EndSend(result);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            return _channelBinder.Request(message, timeout);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelBinder.BeginRequest(message, timeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return _channelBinder.EndRequest(result);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return _channelBinder.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelBinder.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return _channelBinder.EndWaitForMessage(result);
        }

        internal void InjectRequest(RequestContext requestContext)
        {
            // Reuse the existing requestContext
            _inputQueue.EnqueueAndDispatch(new RequestContextWrapper(requestContext));
        }

        //
        // TryReceive threads
        //

        private static void TryReceive(object state)
        {
            BufferedReceiveBinder binder = (BufferedReceiveBinder)state;

            RequestContext requestContext;
            bool requiresDispatch = false;
            try
            {
                if (binder._channelBinder.TryReceive(TimeSpan.MaxValue, out requestContext))
                {
                    requiresDispatch = binder._inputQueue.EnqueueWithoutDispatch(new RequestContextWrapper(requestContext), null);
                }
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }

                requiresDispatch = binder._inputQueue.EnqueueWithoutDispatch(exception, null);
            }
            finally
            {
                Interlocked.Exchange(ref binder._pendingOperationSemaphore, 0);
                if (requiresDispatch)
                {
                    binder._inputQueue.Dispatch();
                }
            }
        }

        private static void TryReceiveCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            HandleEndTryReceive(result);
        }

        private static void HandleEndTryReceive(IAsyncResult result)
        {
            BufferedReceiveBinder binder = (BufferedReceiveBinder)result.AsyncState;

            RequestContext requestContext;
            bool requiresDispatch = false;
            try
            {
                if (binder._channelBinder.EndTryReceive(result, out requestContext))
                {
                    requiresDispatch = binder._inputQueue.EnqueueWithoutDispatch(new RequestContextWrapper(requestContext), null);
                }
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }

                requiresDispatch = binder._inputQueue.EnqueueWithoutDispatch(exception, null);
            }
            finally
            {
                Interlocked.Exchange(ref binder._pendingOperationSemaphore, 0);
                if (requiresDispatch)
                {
                    binder._inputQueue.Dispatch();
                }
            }
        }

        // A RequestContext may be 'null' (some pieces of ChannelHandler depend on this) but the InputQueue
        // will not allow null items to be enqueued. Wrap the RequestContexts in another object to
        // facilitate this semantic
        private class RequestContextWrapper
        {
            public RequestContextWrapper(RequestContext requestContext)
            {
                this.RequestContext = requestContext;
            }

            public RequestContext RequestContext
            {
                get;
                private set;
            }
        }
    }
}
