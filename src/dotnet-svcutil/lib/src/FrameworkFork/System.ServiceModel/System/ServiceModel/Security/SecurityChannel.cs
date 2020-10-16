// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    internal abstract class SecurityChannel<TChannel> :
        LayeredChannel<TChannel>
        where TChannel : class, IChannel
    {
        private SecurityProtocol _securityProtocol;

        protected SecurityChannel(ChannelManagerBase channelManager, TChannel innerChannel)
            : this(channelManager, innerChannel, null)
        {
        }

        protected SecurityChannel(ChannelManagerBase channelManager, TChannel innerChannel, SecurityProtocol securityProtocol)
            : base(channelManager, innerChannel)
        {
            _securityProtocol = securityProtocol;
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(FaultConverter))
            {
                return new SecurityChannelFaultConverter(this.InnerChannel) as T;
            }

            return base.GetProperty<T>();
        }

        public SecurityProtocol SecurityProtocol
        {
            get
            {
                return _securityProtocol;
            }
            protected set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _securityProtocol = value;
            }
        }

        protected override void OnAbort()
        {
            if (_securityProtocol != null)
            {
                _securityProtocol.Close(true, TimeSpan.Zero);
            }

            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, this.BeginCloseSecurityProtocol, this.EndCloseSecurityProtocol,
                base.OnBeginClose, base.OnEndClose);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        private IAsyncResult BeginCloseSecurityProtocol(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityChannel async path");
        }

        private void EndCloseSecurityProtocol(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityChannel async path");
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_securityProtocol != null)
            {
                _securityProtocol.Close(false, timeoutHelper.RemainingTime());
            }
            base.OnClose(timeoutHelper.RemainingTime());
        }

        protected void ThrowIfDisposedOrNotOpen(Message message)
        {
            ThrowIfDisposedOrNotOpen();
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
        }

        private class NullSecurityProtocolCloseAsyncResult : CompletedAsyncResult
        {
            public NullSecurityProtocolCloseAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            new public static void End(IAsyncResult result)
            {
                AsyncResult.End<NullSecurityProtocolCloseAsyncResult>(result);
            }
        }
    }
}
