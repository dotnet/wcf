using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class Binding_Http_WSHttpBindingTests
{
    [WcfTheory]
    [InlineData(WSMessageEncoding.Text)]
    [InlineData(WSMessageEncoding.Mtom)]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String(WSMessageEncoding messageEncoding)
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        WSHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None);
            binding.MessageEncoding = messageEncoding;

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.WSHttpBindingBaseAddress + Enum.GetName(typeof(WSMessageEncoding), messageEncoding)));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(result == testString, String.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

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

