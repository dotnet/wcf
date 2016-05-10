// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
