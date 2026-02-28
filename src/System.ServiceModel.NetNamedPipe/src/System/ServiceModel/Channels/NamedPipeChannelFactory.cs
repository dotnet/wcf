// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    internal class NamedPipeChannelFactory<TChannel> : NetFramingTransportChannelFactory<TChannel>, IPipeTransportFactorySettings
    {
        public NamedPipeChannelFactory(NamedPipeTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context,
            GetConnectionGroupName(bindingElement),
            bindingElement.ConnectionPoolSettings.IdleTimeout,
            bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint)
        {
            if (bindingElement.PipeSettings != null)
            {
                PipeSettings = bindingElement.PipeSettings.Clone();
            }
        }

        public override string Scheme => Uri.UriSchemeNetPipe;

        public NamedPipeSettings PipeSettings
        {
            get; private set;
        }

        private static string GetConnectionGroupName(NamedPipeTransportBindingElement bindingElement)
        {
            return bindingElement.ConnectionPoolSettings.GroupName + bindingElement.PipeSettings.ApplicationContainerSettings.GetConnectionGroupSuffix();
        }

        public override IConnectionInitiator GetConnectionInitiator() => new PipeConnectionInitiator(ConnectionBufferSize, this);

        protected override string GetConnectionPoolKey(EndpointAddress address, Uri via) => PipeConnectionInitiator.GetPipeName(via, this);

        protected override bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement) => upgradeBindingElement is not SslStreamSecurityBindingElement;
    }
}
