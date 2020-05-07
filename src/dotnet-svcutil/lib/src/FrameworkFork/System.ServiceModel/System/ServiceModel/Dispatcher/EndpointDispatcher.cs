// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    public class EndpointDispatcher
    {
        private ChannelDispatcher _channelDispatcher;
        private ServiceChannel _datagramChannel;
        private int _filterPriority;
        private Uri _listenUri;
        private EndpointAddress _originalAddress = null;

        public ChannelDispatcher ChannelDispatcher
        {
            get { return _channelDispatcher; }
        }

        public string ContractName
        {
            get { return String.Empty; }
        }

        public string ContractNamespace
        {
            get { return String.Empty; }
        }

        internal ServiceChannel DatagramChannel
        {
            get { return _datagramChannel; }
            set { _datagramChannel = value; }
        }

        public DispatchRuntime DispatchRuntime
        {
            get { return null; }
        }

        public EndpointAddress EndpointAddress
        {
            get
            {
                if (_channelDispatcher == null)
                {
                    return _originalAddress;
                }

                if ((_originalAddress != null) && (_originalAddress.Identity != null))
                {
                    return _originalAddress;
                }

                IChannelListener listener = _channelDispatcher.Listener;
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

        public int FilterPriority
        {
            get { return _filterPriority; }
            set { _filterPriority = value; }
        }

        internal void Attach(ChannelDispatcher channelDispatcher)
        {
            if (channelDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelDispatcher");
            }

            if (_channelDispatcher != null)
            {
                Exception error = new InvalidOperationException(SRServiceModel.SFxEndpointDispatcherMultipleChannelDispatcher0);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }

            _channelDispatcher = channelDispatcher;
            _listenUri = channelDispatcher.Listener.Uri;
        }

        internal void Detach(ChannelDispatcher channelDispatcher)
        {
            if (channelDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelDispatcher");
            }

            if (_channelDispatcher != channelDispatcher)
            {
                Exception error = new InvalidOperationException(SRServiceModel.SFxEndpointDispatcherDifferentChannelDispatcher0);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }

            _channelDispatcher = null;
        }
    }
}
