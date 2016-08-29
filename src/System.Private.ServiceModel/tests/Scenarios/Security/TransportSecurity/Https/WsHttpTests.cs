// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Infrastructure.Common;

using Xunit;
using System.Threading.Tasks;

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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [OuterLoop]
    public static void CreateUserNameOverTransportBindingElement_Round_Trips_Async()
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

            // *** EXECUTE *** \\
            // Async factory open is part of the code under test
            Task t = Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None);
            t.GetAwaiter().GetResult();

            serviceProxy = factory.CreateChannel();

            Task<string> echoTask = serviceProxy.EchoWithTimeoutAsync(testString, ScenarioTestHelpers.TestTimeout);
            string result = echoTask.GetAwaiter().GetResult();

            // *** VALIDATE *** \\

            Assert.True(result == testString, string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

            // Async factory close is also code under test
            t = Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);
            t.GetAwaiter().GetResult();

            // Async proxy close is also code under test
            t = Task.Factory.FromAsync(((ICommunicationObject)serviceProxy).BeginClose, ((ICommunicationObject)serviceProxy).EndClose, TaskCreationOptions.None);
            t.GetAwaiter().GetResult();

            // *** CLEANUP *** \\
            // cleanup was all part of code under test
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

}
