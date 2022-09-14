// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public class EndpointDispatcher
    {
        private Uri _listenUri;
        private EndpointAddress _originalAddress = null;

        public ChannelDispatcher ChannelDispatcher { get; private set; }

        public string ContractName
        {
            get { return String.Empty; }
        }

        public string ContractNamespace
        {
            get { return String.Empty; }
        }

        internal ServiceChannel DatagramChannel { get; set; }

        public DispatchRuntime DispatchRuntime
        {
            get { return null; }
        }

        public EndpointAddress EndpointAddress
        {
            get
            {
                if (ChannelDispatcher == null)
                {
                    return _originalAddress;
                }

                if ((_originalAddress != null) && (_originalAddress.Identity != null))
                {
                    return _originalAddress;
                }

                IChannelListener listener = ChannelDispatcher.Listener;
                EndpointIdentity identity = listener.GetProperty<EndpointIdentity>();
                if ((_originalAddress != null) && (identity == null))
                {
                    return _originalAddress;
                }

                EndpointAddressBuilder builder;
                if (_originalAddress != null)
                {
                    builder = new EndpointAddressBuilder(_originalAddress);
                }
                else
                {
                    builder = new EndpointAddressBuilder();
                    builder.Uri = listener.Uri;
                }
                builder.Identity = identity;
                return builder.ToEndpointAddress();
            }
        }

        public int FilterPriority { get; set; }

        internal void Attach(ChannelDispatcher channelDispatcher)
        {
            if (ChannelDispatcher != null)
            {
                Exception error = new InvalidOperationException(SRP.SFxEndpointDispatcherMultipleChannelDispatcher0);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }

            ChannelDispatcher = channelDispatcher ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelDispatcher));
            _listenUri = channelDispatcher.Listener.Uri;
        }

        internal void Detach(ChannelDispatcher channelDispatcher)
        {
            if (channelDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelDispatcher));
            }

            if (ChannelDispatcher != channelDispatcher)
            {
                Exception error = new InvalidOperationException(SRP.SFxEndpointDispatcherDifferentChannelDispatcher0);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }

            ChannelDispatcher = null;
        }
    }
}
