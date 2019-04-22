// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using SessionIdleManager = System.ServiceModel.Channels.ServiceChannel.SessionIdleManager;

namespace System.ServiceModel.Dispatcher
{
    internal class ListenerHandler : CommunicationObject
    {
        private static Action<object> s_initiateChannelPump = new Action<object>(ListenerHandler.InitiateChannelPump);
        private static AsyncCallback s_waitCallback = Fx.ThunkCallback(new AsyncCallback(ListenerHandler.WaitCallback));
        private SessionIdleManager _idleManager;
        private bool _acceptedNull;
        private bool _doneAccepting;
        private readonly IListenerBinder _listenerBinder;
        private IDefaultCommunicationTimeouts _timeouts;

        internal ListenerHandler(IListenerBinder listenerBinder, ChannelDispatcher channelDispatcher, IDefaultCommunicationTimeouts timeouts)
        {
            _listenerBinder = listenerBinder;
            if (!((_listenerBinder != null)))
            {
                Fx.Assert("ListenerHandler.ctor: (this.listenerBinder != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(listenerBinder));
            }

            ChannelDispatcher = channelDispatcher;
            if (!((ChannelDispatcher != null)))
            {
                Fx.Assert("ListenerHandler.ctor: (this.channelDispatcher != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelDispatcher));
            }

            _timeouts = timeouts;

            Endpoints = channelDispatcher.EndpointDispatcherTable;
        }

        internal ChannelDispatcher ChannelDispatcher { get; }

        internal ListenerChannel Channel { get; private set; }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return ServiceDefaults.CloseTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return ServiceDefaults.OpenTimeout; }
        }

        internal EndpointDispatcherTable Endpoints { get; set; }

        new internal object ThisLock
        {
            get { return base.ThisLock; }
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            OnOpen(timeout);
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
            ChannelDispatcher.Channels.IncrementActivityCount();
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
                    DoneAccepting();
                    break;
                }

                Dispatch();
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
            IChannel[] channels = ChannelDispatcher.Channels.ToArray();
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
                        {
                            duplexSession.EndCloseOutputSession(result);
                        }
                    }
                    else
                    {
                        IAsyncResult result = channel.BeginClose(timeout, Fx.ThunkCallback(new AsyncCallback(CloseChannelCallback)), state);
                        if (result.CompletedSynchronously)
                        {
                            channel.EndClose(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                HandleError(e);

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
            IChannel[] channels = ChannelDispatcher.Channels.ToArray();
            for (int index = 0; index < channels.Length; index++)
            {
                IChannel channel = channels[index];
                if (!IsSessionChannel(channel))
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
                        HandleError(e);
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
            IChannel[] channels = ChannelDispatcher.Channels.ToArray();
            for (int index = 0; index < channels.Length; index++)
            {
                CloseChannel(channels[index], timeoutHelper.RemainingTime());
            }
        }

        private void Dispatch()
        {
            ListenerChannel channel = Channel;
            SessionIdleManager idleManager = _idleManager;
            Channel = null;
            _idleManager = null;

            try
            {
                if (channel != null)
                {
                    ChannelHandler handler = new ChannelHandler(_listenerBinder.MessageVersion, channel.Binder, this, idleManager);

                    if (!channel.Binder.HasSession)
                    {
                        ChannelDispatcher.Channels.Add(channel.Binder.Channel);
                    }

                    if (channel.Binder is DuplexChannelBinder)
                    {
                        DuplexChannelBinder duplexChannelBinder = channel.Binder as DuplexChannelBinder;
                        duplexChannelBinder.ChannelHandler = handler;
                        duplexChannelBinder.DefaultCloseTimeout = DefaultCloseTimeout;

                        if (_timeouts == null)
                        {
                            duplexChannelBinder.DefaultSendTimeout = ServiceDefaults.SendTimeout;
                        }
                        else
                        {
                            duplexChannelBinder.DefaultSendTimeout = _timeouts.SendTimeout;
                        }
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
                HandleError(e);
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
            lock (ThisLock)
            {
                if (!_doneAccepting)
                {
                    _doneAccepting = true;
                    ChannelDispatcher.Channels.DecrementActivityCount();
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
            ChannelDispatcher.Channels.CloseInput();

            // Abort existing channels
            AbortChannels();

            // Wait for channels to finish aborting
            ChannelDispatcher.Channels.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // if there's an idle manager that has not been cancelled, cancel it
            CancelPendingIdleManager();

            // Start aborting incoming channels
            ChannelDispatcher.Channels.CloseInput();

            // Start closing existing channels
            CloseChannels(timeoutHelper.RemainingTime());

            // Wait for channels to finish closing
            return ChannelDispatcher.Channels.BeginClose(timeoutHelper.RemainingTime(), callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            // if there's an idle manager that has not been cancelled, cancel it
            CancelPendingIdleManager();

            // Start aborting incoming channels
            ChannelDispatcher.Channels.CloseInput();

            // Start closing existing channels
            CloseChannels(timeoutHelper.RemainingTime());

            // Wait for channels to finish closing
            ChannelDispatcher.Channels.Close(timeoutHelper.RemainingTime());
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChannelDispatcher.Channels.EndClose(result);
        }

        private bool HandleError(Exception e)
        {
            return ChannelDispatcher.HandleError(e);
        }

        internal class CloseChannelState
        {
            private IChannel _channel;

            internal CloseChannelState(ListenerHandler listenerHandler, IChannel channel)
            {
                ListenerHandler = listenerHandler;
                _channel = channel;
            }

            internal ListenerHandler ListenerHandler { get; }

            internal IChannel Channel
            {
                get { return _channel; }
            }
        }
    }

    internal class ListenerChannel
    {
        public ListenerChannel(IChannelBinder binder)
        {
            Binder = binder;
        }

        public IChannelBinder Binder { get; }
    }
}
