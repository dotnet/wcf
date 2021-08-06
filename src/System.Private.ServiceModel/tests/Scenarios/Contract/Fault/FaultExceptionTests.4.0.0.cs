// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static partial class FaultExceptionTests
{
    [WcfFact]
    [OuterLoop]
    public static void FaultException_Throws_WithFaultDetail()
    {
        string faultMsg = "Test Fault Exception";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        EndpointAddress endpointAddress = null;

        FaultException<FaultDetail> exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.TestFault(faultMsg);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(String.Equals(exception.Detail.Message, faultMsg), String.Format("Expected Fault Message: {0}, actual: {1}", faultMsg, exception.Detail.Message));
    }

    [WcfFact]
    [OuterLoop]
    public static void UnexpectedException_Throws_FaultException()
    {
        string faultMsg = "This is a test fault msg";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        EndpointAddress endpointAddress = null;

        FaultException<ExceptionDetail> exception = Assert.Throws<FaultException<ExceptionDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.ThrowInvalidOperationException(faultMsg);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(String.Equals(exception.Detail.Message, faultMsg), String.Format("Expected Fault Message: {0}, actual: {1}", faultMsg, exception.Detail.Message));
    }

    [WcfFact]
    [OuterLoop]
    public static void FaultException_Throws_With_Int()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        BasicHttpBinding binding = null;

        int expectedFaultCode = 5;  // arbitrary integer choice
        FaultException<int> thrownException = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            thrownException = Assert.Throws<FaultException<int>>(() =>
           {
               serviceProxy.TestFaultInt(expectedFaultCode);
           });

            // *** VALIDATE *** \\
            Assert.Equal(expectedFaultCode, thrownException.Detail);

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
    public static void FaultException_MultipleFaultContracts_Throws_WithFaultDetail()
    {
        string faultMsg = "Test Fault Exception";
        BasicHttpBinding binding;
        ChannelFactory<IWcfService> factory;
        IWcfService serviceProxy;

        // *** VALIDATE *** \\
        var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
        // *** SETUP *** \\
        binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
            {
                serviceProxy.TestFaults(faultMsg, true);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
                factory.Close();
            }
            finally
            {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.Equal(faultMsg, exception.Detail.Message);
    }

    [WcfFact]
    [OuterLoop]
    public static void FaultException_MultipleFaultContracts_Throws_WithFaultDetail2()
    {
        string faultMsg = "Test Fault Exception";
        BasicHttpBinding binding;
        ChannelFactory<IWcfService> factory;
        IWcfService serviceProxy;

        // *** VALIDATE *** \\
        var exception = Assert.Throws<FaultException<FaultDetail2>>(() =>
        {
        // *** SETUP *** \\
        binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
            {
                serviceProxy.TestFaults(faultMsg, false);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
                factory.Close();
            }
            finally
            {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.Equal(faultMsg, exception.Detail.Message);
    }

    [WcfFact]
    [OuterLoop]
    public static void FaultException_FaultContractAndKnownType_Throws_WithFaultDetail()
    {
        string faultMsg = "Test Fault Exception";
        BasicHttpBinding binding;
        ChannelFactory<IWcfService> factory;
        IWcfService serviceProxy;

        // *** VALIDATE *** \\
        var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
        // *** SETUP *** \\
        binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
            {
                serviceProxy.TestFaultWithKnownType(faultMsg, null);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
                factory.Close();
            }
            finally
            {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.Equal(faultMsg, exception.Detail.Message);
    }

    [WcfFact]
    [OuterLoop]
    public static void FaultException_FaultContractAndKnownType_Echo()
    {
        BasicHttpBinding binding;
        ChannelFactory<IWcfService> factory;
        IWcfService serviceProxy;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
        {
            var input = new object[] { new FaultDetail(), new KnownTypeA() };
            var response = serviceProxy.TestFaultWithKnownType(null, input);

            // *** VALIDATE *** \\
            Assert.True(input.Length == response.Length, String.Format("Expected {0} response items but actual was {1}", input.Length, response.Length));
            Assert.True(response[0] != null, "Expected response item to be FaultDetail, but actual was null");
            Assert.True(response[0].GetType() == typeof(FaultDetail), String.Format("Expected response item to be FaultDetail but actual was {0}", response[0].GetType()));
            Assert.True(response[1] != null, "Expected response item to be KnownTypeA, but actual was null");
            Assert.True(response[1].GetType() == typeof(KnownTypeA), String.Format("Expected response item to be FaultDetail2 but actual was {0}", response[1].GetType()));

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
