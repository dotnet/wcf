// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]

    internal class NamedPipeChannelFactory<TChannel> : NetFramingTransportChannelFactory<TChannel>
    {
        public NamedPipeChannelFactory(NamedPipeTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context,
            GetConnectionGroupName(bindingElement),
            bindingElement.ConnectionPoolSettings.IdleTimeout,
            bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)
        {
        }

        public override string Scheme
        {
            get { return Uri.UriSchemeNetPipe; }
        }

        private static string GetConnectionGroupName(NamedPipeTransportBindingElement bindingElement)
        {
            return bindingElement.ConnectionPoolSettings.GroupName;
        }

        public override IConnectionInitiator GetConnectionInitiator()
        {
            return new PipeConnectionInitiator(ConnectionBufferSize);
        }

        protected override string GetConnectionPoolKey(EndpointAddress address, Uri via)
        {
            return PipeConnectionInitiator.GetPipeName(via);
        }

        protected override bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement)
        {
            return upgradeBindingElement is not SslStreamSecurityBindingElement;
        }
    }
}
