// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Text;
using Xunit;

public static class FaultExceptionTests
{
    [Fact]
    [OuterLoop]
    public static void FaultException_Throws_WithFaultDetail()
    {
        string faultMsg = "Test Fault Exception";
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                serviceProxy.TestFault(faultMsg);
            }
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(FaultException<FaultDetail>))
            {
                string error = string.Format("Expected exception: {0}, actual: {1}\r\n{2}",
                                             "FaultException<FaultDetail>", e.GetType(), e.ToString());
                if (e.InnerException != null)
                    error += String.Format("\r\nInnerException:\r\n{0}", e.InnerException.ToString());
                errorBuilder.AppendLine(error);
            }
            else
            {
                FaultException<FaultDetail> faultException = (FaultException<FaultDetail>)(e);
                string actualFaultMsg = ((FaultDetail)(faultException.Detail)).Message;
                if (actualFaultMsg != faultMsg)
                {
                    errorBuilder.AppendLine(string.Format("Expected Fault Message: {0}, actual: {1}", faultMsg, actualFaultMsg));
                }
            }

            Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: FaultException_Throws_WithFaultDetail FAILED with the following errors: {0}", errorBuilder));
            return;
        }

        Assert.True(false, "Expected FaultException<FaultDetail> exception, but no exception thrown.");
    }

    [Fact]
    [OuterLoop]
    public static void UnexpectedException_Throws_FaultException()
    {
        string faultMsg = "This is a test fault msg";
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                serviceProxy.ThrowInvalidOperationException(faultMsg);
            }
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(FaultException<ExceptionDetail>))
            {
                errorBuilder.AppendLine(string.Format("Expected exception: {0}, actual: {1}", "FaultException<ExceptionDetail>", e.GetType()));
            }
            else
            {
                FaultException<ExceptionDetail> faultException = (FaultException<ExceptionDetail>)(e);
                string actualFaultMsg = ((ExceptionDetail)(faultException.Detail)).Message;
                if (actualFaultMsg != faultMsg)
                {
                    errorBuilder.AppendLine(string.Format("Expected Fault Message: {0}, actual: {1}", faultMsg, actualFaultMsg));
                }
            }

            Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: UnexpectedException_Throws_FaultException FAILED with the following errors: {0}", errorBuilder));
            return;
        }

        Assert.True(false, "Expected FaultException<FaultDetail> exception, but no exception thrown.");
    }

    [Fact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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

    [Fact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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

    [Fact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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

    [Fact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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

    [Fact]
    [OuterLoop]
    public static void FaultException_FaultContractAndKnownType_Echo()
    {
        BasicHttpBinding binding;
        ChannelFactory<IWcfService> factory;
        IWcfService serviceProxy;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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
