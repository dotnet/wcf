// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            thrownException = Assert.Throws <FaultException<int>>(() =>
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
}
