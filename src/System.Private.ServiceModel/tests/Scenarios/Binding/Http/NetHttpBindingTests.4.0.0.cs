// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public class Binding_Http_NetHttpBindingTests : ConditionalWcfTest
{
    [WcfTheory]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String(NetHttpMessageEncoding messageEncoding)
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetHttpBinding binding = new NetHttpBinding();
            binding.MessageEncoding = messageEncoding;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_NetHttp + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding)));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.NotNull(result);
            Assert.Equal(testString, result);

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
