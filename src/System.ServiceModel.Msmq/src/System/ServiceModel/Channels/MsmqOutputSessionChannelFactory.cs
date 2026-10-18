// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    // Send-side channel factory for NetMsmqBinding session channels. Shares the
    // whole send pipeline with the datagram factory; the only difference is the
    // channel type it hands back, which exposes IOutputSession.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputSessionChannelFactory : MsmqOutputChannelFactoryBase<IOutputSessionChannel>
    {
        internal MsmqOutputSessionChannelFactory(MsmqTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context)
        {
        }

        protected override IOutputSessionChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            return new MsmqOutputSessionChannel(this, address, via ?? address.Uri);
        }
    }
}
