// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    // Send-side channel factory for NetMsmqBinding datagram channels: binary
    // encoded SOAP messages dispatched into a single queue through the native
    // mqrt.dll send path.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputChannelFactory : MsmqOutputChannelFactoryBase<IOutputChannel>
    {
        internal MsmqOutputChannelFactory(MsmqTransportBindingElement bindingElement, BindingContext context)
            : base(bindingElement, context)
        {
        }

        protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            return new MsmqOutputChannel(this, address, via ?? address.Uri);
        }
    }
}
