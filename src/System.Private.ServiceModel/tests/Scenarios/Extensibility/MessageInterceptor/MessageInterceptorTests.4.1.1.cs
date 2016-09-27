// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public partial class MessageInterceptorTests : ConditionalWcfTest
{
    public class MessageModifier : ChannelMessageInterceptor
    {
        int send = 0;

        public override void OnSend(ref Message msg)
        {
            send++;
            if (send % 2 == 0)
            {
                // Add extra header so that when the message arrives on service side, the message won't be dropped
                msg.Headers.Add(MessageHeader.CreateHeader("ByPass", "urn:InterceptorNamespace", "ByPassPassword"));
            }
        }

        public override ChannelMessageInterceptor Clone()
        {
            return new MessageModifier();
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void CustomBinding_Message_Interceptor()
    {
        ChannelFactory<IWcfChannelExtensibilityContract> factory = null;
        IWcfChannelExtensibilityContract serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(
                new InterceptingBindingElement(new MessageModifier()),
                new HttpTransportBindingElement());

            factory = new ChannelFactory<IWcfChannelExtensibilityContract>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_ChannelExtensibility));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE & VALIDATE *** \\
            int[] windSpeeds = new int[] { 100, 90, 80, 70, 60, 50, 40, 30, 20, 10 };
            for (int i = 0; i < 10; i++)
            {
                serviceProxy.ReportWindSpeed(windSpeeds[i]);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}