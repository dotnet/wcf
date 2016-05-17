// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Samples.MessageInterceptor;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfChannelExtensiblityService : IWcfChannelExtensibilityContract
    {
        public void ReportWindSpeed(int speed)
        {
            if (speed >= 64)
            {
                Console.WriteLine("Dangerous wind detected! Reported speed (" + speed + ") is greater than 64 kph.");
            }
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
        private int _messagesSinceLastReport = 0;
        private readonly int _reportPeriod = 5;

        public DroppingServerInterceptor() { }

        public override void OnReceive(ref Message msg)
        {
            if (msg.Headers.FindHeader("ByPass", "urn:InterceptorNamespace") > 0)
            {
                if (++_messagesSinceLastReport == _reportPeriod)
                {
                    Console.WriteLine(_reportPeriod + " wind speed reports have been received.");
                }
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
