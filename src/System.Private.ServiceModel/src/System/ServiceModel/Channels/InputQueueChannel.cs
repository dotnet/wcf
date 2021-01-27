// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    abstract class InputQueueChannel<TDisposable> : ChannelBase where TDisposable : class, IDisposable
    {
        InputQueue<TDisposable> _inputQueue;

        protected InputQueueChannel(ChannelManagerBase channelManager)
            : base(channelManager)
        {
            _inputQueue = TraceUtility.CreateInputQueue<TDisposable>();
        }

        public int InternalPendingItems => _inputQueue.PendingCount;

        public int PendingItems
        {
            get
            {
                ThrowIfDisposedOrNotOpen();
                return InternalPendingItems;
            }
        }

        public void EnqueueAndDispatch(TDisposable item)
        {
            EnqueueAndDispatch(item, null);
        }

        public void EnqueueAndDispatch(TDisposable item, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            OnEnqueueItem(item);

            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            _inputQueue.EnqueueAndDispatch(item, dequeuedCallback, canDispatchOnThisThread);
        }

        public void EnqueueAndDispatch(Exception exception, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            _inputQueue.EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
        }

        public void EnqueueAndDispatch(TDisposable item, Action dequeuedCallback)
        {
            OnEnqueueItem(item);

            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            _inputQueue.EnqueueAndDispatch(item, dequeuedCallback);
        }

        public bool EnqueueWithoutDispatch(Exception exception, Action dequeuedCallback)
        {
            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            return _inputQueue.EnqueueWithoutDispatch(exception, dequeuedCallback);
        }

        public bool EnqueueWithoutDispatch(TDisposable item, Action dequeuedCallback)
        {
            OnEnqueueItem(item);

            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            return _inputQueue.EnqueueWithoutDispatch(item, dequeuedCallback);
        }

        public void Dispatch()
        {
            // NOTE: don't need to check IsDisposed here: InputQueue will handle dispose
            _inputQueue.Dispatch();
        }

        public void Shutdown()
        {
            _inputQueue.Shutdown();
        }

        protected override void OnFaulted()
        {
            base.OnFaulted();
            _inputQueue.Shutdown(() => GetPendingException());
        }

        protected virtual void OnEnqueueItem(TDisposable item)
        {
        }

        protected async Task<(bool dequeued, TDisposable item)> DequeueAsync(TimeSpan timeout)
        {
            ThrowIfNotOpened();
            (bool dequeued, TDisposable item) = await _inputQueue.TryDequeueAsync(timeout);

            if (item == null)
            {
                ThrowIfFaulted();
                ThrowIfAborted();
            }

            return (dequeued, item);
        }

        protected async Task<bool> WaitForItemAsync(TimeSpan timeout)
        {
            ThrowIfNotOpened();
            bool dequeued = await _inputQueue.WaitForItemAsync(timeout);

            ThrowIfFaulted();
            ThrowIfAborted();

            return dequeued;
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            _inputQueue.Shutdown(() => GetPendingException());
        }

        protected override void OnAbort()
        {
            _inputQueue.Close();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _inputQueue.Close();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            _inputQueue.Close();
            return Task.CompletedTask;
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            _inputQueue.Close();
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }
    }
}
