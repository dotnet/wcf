// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading;

namespace System.ServiceModel
{
    internal delegate void InstanceContextEmptyCallback(InstanceContext instanceContext);

    internal class ServiceChannelManager : LifetimeManager
    {
        private int _activityCount;
        private ICommunicationWaiter _activityWaiter;
        private int _activityWaiterCount;
        private InstanceContextEmptyCallback _emptyCallback;
        private IChannel _firstIncomingChannel;
        private ChannelCollection _incomingChannels;
        private ChannelCollection _outgoingChannels;
        private InstanceContext _instanceContext;

        public ServiceChannelManager(InstanceContext instanceContext)
            : this(instanceContext, null)
        {
        }

        public ServiceChannelManager(InstanceContext instanceContext, InstanceContextEmptyCallback emptyCallback)
            : base(instanceContext.ThisLock)
        {
            _instanceContext = instanceContext;
            _emptyCallback = emptyCallback;
        }

        public int ActivityCount
        {
            get { return _activityCount; }
        }

        public ICollection<IChannel> IncomingChannels
        {
            get
            {
                this.EnsureIncomingChannelCollection();
                return (ICollection<IChannel>)_incomingChannels;
            }
        }

        public ICollection<IChannel> OutgoingChannels
        {
            get
            {
                if (_outgoingChannels == null)
                {
                    lock (this.ThisLock)
                    {
                        if (_outgoingChannels == null)
                            _outgoingChannels = new ChannelCollection(this, this.ThisLock);
                    }
                }
                return _outgoingChannels;
            }
        }

        public bool IsBusy
        {
            get
            {
                if (this.ActivityCount > 0)
                    return true;

                if (base.BusyCount > 0)
                    return true;

                ICollection<IChannel> outgoing = _outgoingChannels;
                if ((outgoing != null) && (outgoing.Count > 0))
                    return true;

                return false;
            }
        }

        public void AddIncomingChannel(IChannel channel)
        {
            bool added = false;

            lock (this.ThisLock)
            {
                if (this.State == LifetimeState.Opened)
                {
                    if (_firstIncomingChannel == null)
                    {
                        if (_incomingChannels == null)
                        {
                            _firstIncomingChannel = channel;
                            this.ChannelAdded(channel);
                        }
                        else
                        {
                            if (_incomingChannels.Contains(channel))
                                return;
                            _incomingChannels.Add(channel);
                        }
                    }
                    else
                    {
                        this.EnsureIncomingChannelCollection();
                        if (_incomingChannels.Contains(channel))
                            return;
                        _incomingChannels.Add(channel);
                    }
                    added = true;
                }
            }

            if (!added)
            {
                channel.Abort();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
            }
        }

        public IAsyncResult BeginCloseInput(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseCommunicationAsyncResult closeResult = null;

            lock (this.ThisLock)
            {
                if (_activityCount > 0)
                {
                    closeResult = new CloseCommunicationAsyncResult(timeout, callback, state, this.ThisLock);

                    if (!(_activityWaiter == null))
                    {
                        Fx.Assert("ServiceChannelManager.BeginCloseInput: (this.activityWaiter == null)");
                    }
                    _activityWaiter = closeResult;
                    Interlocked.Increment(ref _activityWaiterCount);
                }
            }

            if (closeResult != null)
                return closeResult;
            else
                return new CompletedAsyncResult(callback, state);
        }

        private void ChannelAdded(IChannel channel)
        {
            base.IncrementBusyCount();
            channel.Closed += this.OnChannelClosed;
        }

        private void ChannelRemoved(IChannel channel)
        {
            channel.Closed -= this.OnChannelClosed;
            base.DecrementBusyCount();
        }


        public void CloseInput(TimeSpan timeout)
        {
            SyncCommunicationWaiter activityWaiter = null;

            lock (this.ThisLock)
            {
                if (_activityCount > 0)
                {
                    activityWaiter = new SyncCommunicationWaiter(this.ThisLock);
                    if (!(_activityWaiter == null))
                    {
                        Fx.Assert("ServiceChannelManager.CloseInput: (this.activityWaiter == null)");
                    }
                    _activityWaiter = activityWaiter;
                    Interlocked.Increment(ref _activityWaiterCount);
                }
            }

            if (activityWaiter != null)
            {
                CommunicationWaitResult result = activityWaiter.Wait(timeout, false);
                if (Interlocked.Decrement(ref _activityWaiterCount) == 0)
                {
                    activityWaiter.Dispose();
                    _activityWaiter = null;
                }

                switch (result)
                {
                    case CommunicationWaitResult.Expired:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRServiceModel.SfxCloseTimedOutWaitingForDispatchToComplete));
                    case CommunicationWaitResult.Aborted:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
                }
            }
        }

        public void DecrementActivityCount()
        {
            ICommunicationWaiter activityWaiter = null;
            bool empty = false;

            lock (this.ThisLock)
            {
                if (!(_activityCount > 0))
                {
                    Fx.Assert("ServiceChannelManager.DecrementActivityCount: (this.activityCount > 0)");
                }
                if (--_activityCount == 0)
                {
                    if (_activityWaiter != null)
                    {
                        activityWaiter = _activityWaiter;
                        Interlocked.Increment(ref _activityWaiterCount);
                    }
                    if (this.BusyCount == 0)
                        empty = true;
                }
            }

            if (activityWaiter != null)
            {
                activityWaiter.Signal();
                if (Interlocked.Decrement(ref _activityWaiterCount) == 0)
                {
                    activityWaiter.Dispose();
                    _activityWaiter = null;
                }
            }

            if (empty && this.State == LifetimeState.Opened)
                OnEmpty();
        }

        public void EndCloseInput(IAsyncResult result)
        {
            if (result is CloseCommunicationAsyncResult)
            {
                CloseCommunicationAsyncResult.End(result);
                if (Interlocked.Decrement(ref _activityWaiterCount) == 0)
                {
                    _activityWaiter.Dispose();
                    _activityWaiter = null;
                }
            }
            else
                CompletedAsyncResult.End(result);
        }

        private void EnsureIncomingChannelCollection()
        {
            lock (this.ThisLock)
            {
                if (_incomingChannels == null)
                {
                    _incomingChannels = new ChannelCollection(this, this.ThisLock);
                    if (_firstIncomingChannel != null)
                    {
                        _incomingChannels.Add(_firstIncomingChannel);
                        this.ChannelRemoved(_firstIncomingChannel); // Adding to collection called ChannelAdded, so call ChannelRemoved to balance
                        _firstIncomingChannel = null;
                    }
                }
            }
        }

        public void IncrementActivityCount()
        {
            lock (this.ThisLock)
            {
                if (this.State == LifetimeState.Closed)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
                _activityCount++;
            }
        }

        protected override void IncrementBusyCount()
        {
            base.IncrementBusyCount();
        }

        protected override void OnAbort()
        {
            IChannel[] channels = this.SnapshotChannels();
            for (int index = 0; index < channels.Length; index++)
                channels[index].Abort();

            ICommunicationWaiter activityWaiter = null;

            lock (this.ThisLock)
            {
                if (_activityWaiter != null)
                {
                    activityWaiter = _activityWaiter;
                    Interlocked.Increment(ref _activityWaiterCount);
                }
            }

            if (activityWaiter != null)
            {
                activityWaiter.Signal();
                if (Interlocked.Decrement(ref _activityWaiterCount) == 0)
                {
                    activityWaiter.Dispose();
                    _activityWaiter = null;
                }
            }

            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, BeginCloseInput, EndCloseInput, OnBeginCloseContinue, OnEndCloseContinue);
        }

        private IAsyncResult OnBeginCloseContinue(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            return base.OnBeginClose(timeoutHelper.RemainingTime(), callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            this.CloseInput(timeoutHelper.RemainingTime());

            base.OnClose(timeoutHelper.RemainingTime());
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        private void OnEndCloseContinue(IAsyncResult result)
        {
            base.OnEndClose(result);
        }

        protected override void OnEmpty()
        {
            if (_emptyCallback != null)
                _emptyCallback(_instanceContext);
        }

        private void OnChannelClosed(object sender, EventArgs args)
        {
            this.RemoveChannel((IChannel)sender);
        }

        public bool RemoveChannel(IChannel channel)
        {
            lock (this.ThisLock)
            {
                if (_firstIncomingChannel == channel)
                {
                    _firstIncomingChannel = null;
                    this.ChannelRemoved(channel);
                    return true;
                }
                else if (_incomingChannels != null && _incomingChannels.Contains(channel))
                {
                    _incomingChannels.Remove(channel);
                    return true;
                }
                else if (_outgoingChannels != null && _outgoingChannels.Contains(channel))
                {
                    _outgoingChannels.Remove(channel);
                    return true;
                }
            }

            return false;
        }

        public IChannel[] SnapshotChannels()
        {
            lock (this.ThisLock)
            {
                int outgoingCount = (_outgoingChannels != null ? _outgoingChannels.Count : 0);

                if (_firstIncomingChannel != null)
                {
                    IChannel[] channels = new IChannel[1 + outgoingCount];
                    channels[0] = _firstIncomingChannel;
                    if (outgoingCount > 0)
                        _outgoingChannels.CopyTo(channels, 1);
                    return channels;
                }

                if (_incomingChannels != null)
                {
                    IChannel[] channels = new IChannel[_incomingChannels.Count + outgoingCount];
                    _incomingChannels.CopyTo(channels, 0);
                    if (outgoingCount > 0)
                        _outgoingChannels.CopyTo(channels, _incomingChannels.Count);
                    return channels;
                }

                if (outgoingCount > 0)
                {
                    IChannel[] channels = new IChannel[outgoingCount];
                    _outgoingChannels.CopyTo(channels, 0);
                    return channels;
                }
            }
            return Array.Empty<IChannel>();
        }

        internal class ChannelCollection : ICollection<IChannel>
        {
            private ServiceChannelManager _channelManager;
            private object _syncRoot;
            private HashSet<IChannel> _hashSet = new HashSet<IChannel>();

            public bool IsReadOnly
            {
                get { return false; }
            }

            public int Count
            {
                get
                {
                    lock (_syncRoot)
                    {
                        return _hashSet.Count;
                    }
                }
            }

            public ChannelCollection(ServiceChannelManager channelManager, object syncRoot)
            {
                if (syncRoot == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("syncRoot"));

                _channelManager = channelManager;
                _syncRoot = syncRoot;
            }

            public void Add(IChannel channel)
            {
                lock (_syncRoot)
                {
                    if (_hashSet.Add(channel))
                    {
                        _channelManager.ChannelAdded(channel);
                    }
                }
            }

            public void Clear()
            {
                lock (_syncRoot)
                {
                    foreach (IChannel channel in _hashSet)
                        _channelManager.ChannelRemoved(channel);
                    _hashSet.Clear();
                }
            }

            public bool Contains(IChannel channel)
            {
                lock (_syncRoot)
                {
                    if (channel != null)
                    {
                        return _hashSet.Contains(channel);
                    }
                    return false;
                }
            }

            public void CopyTo(IChannel[] array, int arrayIndex)
            {
                lock (_syncRoot)
                {
                    _hashSet.CopyTo(array, arrayIndex);
                }
            }

            public bool Remove(IChannel channel)
            {
                lock (_syncRoot)
                {
                    bool ret = false;
                    if (channel != null)
                    {
                        ret = _hashSet.Remove(channel);
                        if (ret)
                        {
                            _channelManager.ChannelRemoved(channel);
                        }
                    }
                    return ret;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                lock (_syncRoot)
                {
                    return _hashSet.GetEnumerator();
                }
            }

            IEnumerator<IChannel> IEnumerable<IChannel>.GetEnumerator()
            {
                lock (_syncRoot)
                {
                    return _hashSet.GetEnumerator();
                }
            }
        }
    }
}
