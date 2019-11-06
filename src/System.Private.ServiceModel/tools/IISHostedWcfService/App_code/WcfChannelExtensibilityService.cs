// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Samples.MessageInterceptor;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfChannelExtensiblityService : IWcfChannelExtensibilityContract
    {
        public void ReportWindSpeed(int speed)
        {
            // All the real work for this test is done in DroppingServerInterceptor.OnReceive
        }
    }

    internal class DroppingServerElement : InterceptingElement
    {
        protected override ChannelMessageInterceptor CreateMessageInterceptor()
        {
            return new DroppingServerInterceptor();
        }
    }

    public class DroppingServerInterceptor : ChannelMessageInterceptor
    {
        public DroppingServerInterceptor() { }

        public override void OnReceive(ref Message msg)
        {
            // Verify the additional Header inserted by the ChannelMessageInterceptor exists
            if (msg.Headers.FindHeader("ByPass", "urn:InterceptorNamespace") > 0)
            {
                return;
            }
            // Drop incoming Message if the Message does not have the special header
            msg = null;
        }

        public override ChannelMessageInterceptor Clone()
        {
            return new DroppingServerInterceptor();
        }
    }
}
