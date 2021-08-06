// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static partial class TextEncodingTests
{
    [WcfFact]
    [OuterLoop]
    public static void TextMessageEncoder_WrongContentTypeResponse_Throws_ProtocolException()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testContentType = "text/blah";
        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Assert.Throws<ProtocolException>(() => { serviceProxy.ReturnContentType(testContentType); });

            // *** VALIDATE *** \\

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void TextMessageEncoder_QuotedCharSet_In_Response_ContentType()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService channel = null;
        string testQuotedCharSetContentType = "text/xml; param1 = value1; charset=\"utf-8\"; param2 = value2; param3 = \"value3\"";

        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            channel = factory.CreateChannel();

            // *** EXECUTE *** \\
            channel.ReturnContentType(testQuotedCharSetContentType);
        }
        finally
        {
            // *** CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel, factory);
        }
    }
}
