// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

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
                return new SecurityChannelFaultConverter(InnerChannel) as T;
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
                _securityProtocol = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            }
        }

        protected override void OnAbort()
        {
            if (_securityProtocol != null)
            {
                _securityProtocol.CloseAsync(true, TimeSpan.Zero).GetAwaiter().GetResult();
            }

            base.OnAbort();
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_securityProtocol != null)
            {
                await _securityProtocol.CloseAsync(false, timeoutHelper.RemainingTime());
            }

            await base.OnCloseAsync(timeoutHelper.RemainingTime());
        }

        protected void ThrowIfDisposedOrNotOpen(Message message)
        {
            ThrowIfDisposedOrNotOpen();
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
        }
    }
}
