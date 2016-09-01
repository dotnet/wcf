﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// 

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Infrastructure.Common;

using Xunit;

public static partial class WsHttpsTests
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [OuterLoop]
    public static void CreateUserNameOverTransportBindingElement_Round_Trips()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        CustomBinding binding;

        try
        {
            // *** SETUP *** \\
            var securityBindingElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            securityBindingElement.IncludeTimestamp = false;
            securityBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            binding = new CustomBinding(
                            new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), 
                            securityBindingElement,
                            new HttpsTransportBindingElement());

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap11_Address));
            factory.Credentials.UserName.UserName = "someUser";
            factory.Credentials.UserName.Password = "somePassword";

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(result == testString, string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

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
