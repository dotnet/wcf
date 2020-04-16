// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using SessionIdleManager = System.ServiceModel.Channels.ServiceChannel.SessionIdleManager;

namespace System.ServiceModel.Dispatcher
{
    internal class ListenerHandler : CommunicationObject
    {
        private static Action<object> s_initiateChannelPump = new Action<object>(ListenerHandler.InitiateChannelPump);
        private static AsyncCallback s_waitCallback = Fx.ThunkCallback(new AsyncCallback(ListenerHandler.WaitCallback));

        private readonly ChannelDispatcher _channelDispatcher;
        private ListenerChannel _channel;
        private SessionIdleManager _idleManager;
        private bool _acceptedNull;
        private bool _doneAccepting;
        private EndpointDispatcherTable _endpoints;
        private readonly IListenerBinder _listenerBinder;
        private IDefaultCommunicationTimeouts _timeouts;

        internal ListenerHandler(IListenerBinder listenerBinder, ChannelDispatcher channelDispatcher, IDefaultCommunicationTimeouts timeouts)
        {
            _listenerBinder = listenerBinder;
            if (!((_listenerBinder != null)))
            {
                Fx.Assert("ListenerHandler.ctor: (this.listenerBinder != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("listenerBinder");
            }

            _channelDispatcher = channelDispatcher;
            if (!((_channelDispatcher != null)))
            {
                Fx.Assert("ListenerHandler.ctor: (this.channelDispatcher != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelDispatcher");
            }

            _timeouts = timeouts;

            _endpoints = channelDispatcher.EndpointDispatcherTable;
        }

        internal ChannelDispatcher ChannelDispatcher
        {
            get { return _channelDispatcher; }
        }

        internal ListenerChannel Channel
        {
            get { return _channel; }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return ServiceDefaults.CloseTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return ServiceDefaults.OpenTimeout; }
        }

        internal EndpointDispatcherTable Endpoints
        {
            get { return _endpoints; }
            set { _endpoints = value; }
        }

        new internal object ThisLock
        {
            get { return base.ThisLock; }
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            this.OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            this.OnOpen(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _channelDispatcher.Channels.IncrementActivityCount();
            NewChannelPump();
        }

        internal void NewChannelPump()
        {
            ActionItem.Schedule(ListenerHandler.s_initiateChannelPump, this);
        }

        private static void InitiateChannelPump(object state)
        {
            ListenerHandler listenerHandler = state as ListenerHandler;
            listenerHandler.ChannelPump();
        }

        private void ChannelPump()
        {
            IChannelListener listener = _listenerBinder.Listener;

            for (; ; )
            {
                if (_acceptedNull || (listener.State == CommunicationState.Faulted))
                {
                    this.DoneAccepting();
                    break;
                }

                this.Dispatch();
            }
        }

        private static void WaitCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            };

            ListenerHandler listenerHandler = (ListenerHandler)result.AsyncState;
            IChannelListener listener = listenerHandler._listenerBinder.Listener;
            listenerHandler.Dispatch();
        }

        private void AbortChannels()
        {
            IChannel[] channels = _channelDispatcher.Channels.ToArray();
            for (int index = 0; index < channels.Length; index++)
            {
                channels[index].Abort();
            }
        }

        private void CloseChannel(IChannel channel, TimeSpan timeout)
        {
            try
            {
                if (channel.State != CommunicationState.Closing && channel.State != CommunicationState.Closed)
                {
                    CloseChannelState state = new CloseChannelState(this, channel);
                    if (channel is ISessionChannel<IDuplexSession>)
                    {
                        IDuplexSession duplexSession = ((ISessionChannel<IDuplexSession>)channel).Session;
                        IAsyncResult result = duplexSession.BeginCloseOutputSession(timeout, Fx.ThunkCallback(new AsyncCallback(CloseOutputSessionCallback)), state);
                        if (result.CompletedSynchronously)
                            duplexSession.EndCloseOutputSession(result);
                    }
                    else
                    {
                        IAsyncResult result = channel.BeginClose(timeout, Fx.ThunkCallback(new AsyncCallback(CloseChannelCallback)), state);
                        if (result.CompletedSynchronously)
                            channel.EndClose(result);
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                this.HandleError(e);

                if (channel is ISessionChannel<IDuplexSession>)
                {
                    channel.Abort();
                }
            }
        }

        private static void CloseChannelCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            CloseChannelState state = (CloseChannelState)result.AsyncState;
            try
            {
                state.Channel.EndClose(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                state.ListenerHandler.HandleError(e);
            }
        }

        public void CloseInput(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            // Close all datagram channels
            IChannel[] channels = _channelDispatcher.Channels.ToArray();
            for (int index = 0; index < channels.Length; index++)
            {
                IChannel channel = channels[index];
                if (!this.IsSessionChannel(channel))
                {
                    try
                    {
                        channel.Close(timeoutHelper.RemainingTime());
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        this.HandleError(e);
                    }
                }
            }
        }

        private static void CloseOutputSessionCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            CloseChannelState state = (CloseChannelState)result.AsyncState;
            try
            {
                ((ISessionChannel<IDuplexSession>)state.Channel).Session.EndCloseOutputSession(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                state.ListenerHandler.HandleError(e);
                state.Channel.Abort();
            }
        }

        private void CloseChannels(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            IChannel[] channels = _channelDispatcher.Channels.ToArray();
            for (int index = 0; index < channels.Length; index++)
                CloseChannel(channels[index], timeoutHelper.RemainingTime());
        }

        private void Dispatch()
        {
            ListenerChannel channel = _channel;
            SessionIdleManager idleManager = _idleManager;
            _channel = null;
            _idleManager = null;

            try
            {
                if (channel != null)
                {
                    ChannelHandler handler = new ChannelHandler(_listenerBinder.MessageVersion, channel.Binder, this, idleManager);

                    if (!channel.Binder.HasSession)
                    {
                        _channelDispatcher.Channels.Add(channel.Binder.Channel);
                    }

                    if (channel.Binder is DuplexChannelBinder)
                    {
                        DuplexChannelBinder duplexChannelBinder = channel.Binder as DuplexChannelBinder;
                        duplexChannelBinder.ChannelHandler = handler;
                        duplexChannelBinder.DefaultCloseTimeout = this.DefaultCloseTimeout;

                        if (_timeouts == null)
                            duplexChannelBinder.DefaultSendTimeout = ServiceDefaults.SendTimeout;
                        else
                            duplexChannelBinder.DefaultSendTimeout = _timeouts.SendTimeout;
                    }

                    ChannelHandler.Register(handler);
                    channel = null;
                    idleManager = null;
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                this.HandleError(e);
            }
            finally
            {
                if (channel != null)
                {
                    channel.Binder.Channel.Abort();
                    if (idleManager != null)
                    {
                        idleManager.CancelTimer();
                    }
                }
            }
        }

        private void AcceptedNull()
        {
            _acceptedNull = true;
        }

        private void DoneAccepting()
        {
            lock (this.ThisLock)
            {
                if (!_doneAccepting)
                {
                    _doneAccepting = true;
                    _channelDispatcher.Channels.DecrementActivityCount();
                }
            }
        }

        private bool IsSessionChannel(IChannel channel)
        {
            return (channel is ISessionChannel<IDuplexSession> ||
                    channel is ISessionChannel<IInputSession> ||
                    channel is ISessionChannel<IOutputSession>);
        }

        private void CancelPendingIdleManager()
        {
            SessionIdleManager idleManager = _idleManager;
            if (idleManager != null)
            {
                idleManager.CancelTimer();
            }
        }

        protected override void OnAbort()
        {
            // if there's an idle manager that has not been transferred to the channel handler, cancel it
            CancelPendingIdleManager();

            // Start aborting incoming channels
            _channelDispatcher.Channels.CloseInput();

            // Abort existing channels
            this.AbortChannels();

            // Wait for channels to finish aborting
            _channelDispatcher.Channels.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // if there's an idle manager that has not been cancelled, cancel it
            CancelPendingIdleManager();

            // Start aborting incoming channels
            _channelDispatcher.Channels.CloseInput();

            // Start closing existing channels
            this.CloseChannels(timeoutHelper.RemainingTime());

            // Wait for channels to finish closing
            return _channelDispatcher.Channels.BeginClose(timeoutHelper.RemainingTime(), callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // if there's an idle manager that has not been cancelled, cancel it
            CancelPendingIdleManager();

            // Start aborting incoming channels
            _channelDispatcher.Channels.CloseInput();

            // Start closing existing channels
            this.CloseChannels(timeoutHelper.RemainingTime());

            // Wait for channels to finish closing
            _channelDispatcher.Channels.Close(timeoutHelper.RemainingTime());
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _channelDispatcher.Channels.EndClose(result);
        }

        private bool HandleError(Exception e)
        {
            return _channelDispatcher.HandleError(e);
        }

        internal class CloseChannelState
        {
            private ListenerHandler _listenerHandler;
            private IChannel _channel;

            internal CloseChannelState(ListenerHandler listenerHandler, IChannel channel)
            {
                _listenerHandler = listenerHandler;
                _channel = channel;
            }

            internal ListenerHandler ListenerHandler
            {
                get { return _listenerHandler; }
            }

            internal IChannel Channel
            {
                get { return _channel; }
            }
        }
    }

    internal class ListenerChannel
    {
        private IChannelBinder _binder;

        public ListenerChannel(IChannelBinder binder)
        {
            _binder = binder;
        }

        public IChannelBinder Binder
        {
            get { return _binder; }
        }
    }
}
