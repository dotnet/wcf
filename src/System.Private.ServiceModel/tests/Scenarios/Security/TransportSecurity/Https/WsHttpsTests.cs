// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
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
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = "Hello";
        CustomBinding binding;
        EndpointAddress endpointAddress = null;

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

            endpointAddress = new EndpointAddress(Endpoints.WsHttpTransSecUserName_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(binding, endpointAddress);
            factory.Credentials.UserName.UserName = "test1";
            factory.Credentials.UserName.Password = "Mytestpwd1";

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
    public static void CreateUserNameOverTransportBindingElement_Throws_Wrong_Credentials()
    {
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = "Hello";
        CustomBinding binding;
        EndpointAddress endpointAddress = null;
        FaultException faultException = null;

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

            endpointAddress = new EndpointAddress(Endpoints.WsHttpTransSecUserName_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(binding, endpointAddress);
            factory.Credentials.UserName.UserName = "nottest1";
            factory.Credentials.UserName.Password = "notMytestpwd1";

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            faultException = Assert.Throws<FaultException>(() =>
            {
                serviceProxy.Echo(testString);
            });

            // *** VALIDATE *** \\
            string expectedFaultCode = "InvalidSecurityToken";
            Assert.True(String.Equals(faultException.Code.Name, expectedFaultCode, StringComparison.OrdinalIgnoreCase),
                        String.Format("Expected FaultCode '{0}' but actual was '{1}'", expectedFaultCode, faultException.Code.Name));

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
    [Issue(1494)]
    public static void CreateUserNameOverTransportBindingElement_Round_Trips_Async()
    {
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = "Hello";
        CustomBinding binding;
        EndpointAddress endpointAddress = null;

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

            endpointAddress = new EndpointAddress(Endpoints.WsHttpTransSecUserName_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(binding, endpointAddress);
            factory.Credentials.UserName.UserName = "test1";
            factory.Credentials.UserName.Password = "Mytestpwd1";

            // *** EXECUTE *** \\
            // Async factory open is part of the code under test
            Task t = Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None);
            t.GetAwaiter().GetResult();

            serviceProxy = factory.CreateChannel();

            Task<string> echoTask = serviceProxy.EchoAsync(testString);
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
