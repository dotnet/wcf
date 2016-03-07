// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class TcpTransportSecurityStreamedResource : TcpResource
    {
        protected override string Address { get { return "tcp-transport-security-streamed"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.Transport)
            {
                PortSharingEnabled = false,
                TransferMode = TransferMode.Streamed
            };
        }
    }
}
