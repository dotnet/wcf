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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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
}
