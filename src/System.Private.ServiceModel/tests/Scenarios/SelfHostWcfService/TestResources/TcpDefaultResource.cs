// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class TcpDefaultResource : TcpResource
    {
        protected override string Address { get { return "tcp-default"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding() { PortSharingEnabled = false };
        }
    }
}
