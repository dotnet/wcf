// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public static class ExpectedExceptionTests
{
    [Fact]
    [OuterLoop]
    public static void NotExistentHost_Throws_EndpointNotFoundException()
    {
        string nonExistentHost = "http://nonexisthost/WcfService/WindowsCommunicationFoundation";

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(10000);

        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(nonExistentHost)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                string response = serviceProxy.Echo("Hello");
            }
        });

        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(nonExistentHost), string.Format("Expected exception message to contain: '{0}'", nonExistentHost));
    }

    [Fact]
    [OuterLoop]
    [ActiveIssue(398)]
    public static void ServiceRestart_Throws_CommunicationException()
    {
        StringBuilder errorBuilder = new StringBuilder();
        string restartServiceAddress = "";

        BasicHttpBinding binding = new BasicHttpBinding();

        try
        {
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                restartServiceAddress = serviceProxy.GetRestartServiceEndpoint();
            }
        }
        catch (Exception e)
        {
            string error = String.Format("Unexpected exception thrown while calling the 'GetRestartServiceEndpoint' operation. {0}", e.ToString());
            if (e.InnerException != null)
                error += String.Format("\r\nInnerException:\r\n{0}", e.InnerException.ToString());
            errorBuilder.AppendLine(error);
        }

        if (errorBuilder.Length == 0)
        {
            // Get the Service host name and replace localhost with it
            UriBuilder builder = new UriBuilder(Endpoints.HttpBaseAddress_Basic);
            string hostName = builder.Uri.Host;
            restartServiceAddress = restartServiceAddress.Replace("[HOST]", hostName);
            //On .NET Native retail, exception message is stripped to include only parameter
            string expectExceptionMsg = restartServiceAddress;

            try
            {
                using (ChannelFactory<IWcfRestartService> factory = new ChannelFactory<IWcfRestartService>(binding, new EndpointAddress(restartServiceAddress)))
                {
                    // Get the last portion of the restart service url which is a Guid and convert it back to a Guid
                    // This is needed by the RestartService operation as a Dictionary key to get the ServiceHost
                    string uniqueIdentifier = restartServiceAddress.Substring(restartServiceAddress.LastIndexOf("/") + 1);
                    Guid guid = new Guid(uniqueIdentifier);

                    IWcfRestartService serviceProxy = factory.CreateChannel();
                    serviceProxy.RestartService(guid);
                }

                errorBuilder.AppendLine("Expected CommunicationException exception, but no exception thrown.");
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CommunicationException))
                {
                    if (e.Message.Contains(expectExceptionMsg))
                    {
                    }
                    else
                    {
                        errorBuilder.AppendLine(string.Format("Expected exception message contains: {0}, actual: {1}", expectExceptionMsg, e.Message));
                    }
                }
                else
                {
                    errorBuilder.AppendLine(string.Format("Expected exception: {0}, actual: {1}/n Exception was: {2}", "CommunicationException", e.GetType(), e.ToString()));
                }
            }
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ServiceRestart_Throws_CommunicationException FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void NonExistentAction_Throws_ActionNotSupportedException()
    {
        string exceptionMsg = "The message with Action 'http://tempuri.org/IWcfService/NotExistOnServer' cannot be processed at the receiver, due to a ContractFilter mismatch at the EndpointDispatcher. This may be because of either a contract mismatch (mismatched Actions between sender and receiver) or a binding/security mismatch between the sender and the receiver.  Check that sender and receiver have the same contract and the same binding (including security requirements, e.g. Message, Transport, None).";
        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                serviceProxy.NotExistOnServer();
            }
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(System.ServiceModel.ActionNotSupportedException))
            {
                Assert.True(false, string.Format("Expected exception: {0}, actual: {1}", "ActionNotSupportedException", e.GetType()));
            }

            if (e.Message != exceptionMsg)
            {
                Assert.True(false, string.Format("Expected Fault Message: {0}, actual: {1}", exceptionMsg, e.Message));
            }
            return;
        }

        Assert.True(false, "Expected ActionNotSupportedException exception, but no exception thrown.");
    }

    // SendTimeout is set to 5 seconds, the service waits 10 seconds to respond.
    // The client should throw a TimeoutException
    [Fact]
    [OuterLoop]
    [ActiveIssue(273)]
    public static void SendTimeout_For_Long_Running_Operation_Throws_TimeoutException()
    {
        TimeSpan serviceOperationTimeout = TimeSpan.FromMilliseconds(10000);
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(5000);
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));

        Stopwatch watch = new Stopwatch();
        try
        {
            var exception = Assert.Throws<TimeoutException>(() =>
            {
                IWcfService proxy = factory.CreateChannel();
                watch.Start();
                proxy.EchoWithTimeout("Hello", serviceOperationTimeout);
            });
        }
        finally
        {
            watch.Stop();
        }

        // want to assert that this completed in > 5 s as an upper bound since the SendTimeout is 5 sec
        // (usual case is around 5001-5005 ms) 
        Assert.InRange<long>(watch.ElapsedMilliseconds, 4985, 6000);
    }

    // SendTimeout is set to 0, this should trigger a TimeoutException before even attempting to call the service.
    [Fact]
    [OuterLoop]
    public static void SendTimeout_Zero_Throws_TimeoutException_Immediately()
    {
        TimeSpan serviceOperationTimeout = TimeSpan.FromMilliseconds(5000);
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(0);
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));

        Stopwatch watch = new Stopwatch();
        try
        {
            var exception = Assert.Throws<TimeoutException>(() =>
            {
                IWcfService proxy = factory.CreateChannel();
                watch.Start();
                proxy.EchoWithTimeout("Hello", serviceOperationTimeout);
            });
        }
        finally
        {
            watch.Stop();
        }

        // want to assert that this completed in < 2 s as an upper bound since the SendTimeout is 0 sec
        // (usual case is around 1 - 3 ms) 
        Assert.InRange<long>(watch.ElapsedMilliseconds, 0, 2000);
    }

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
    public static void UnknownUrl_Throws_EndpointNotFoundException()
    {
        // We need a running service host at the other end but mangle the endpoint suffix
        string notFoundUrl = Endpoints.HttpBaseAddress_Basic + "not-an-endpoint";

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(10000);

        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            try
            {
                using (
                    ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding,
                        new EndpointAddress(notFoundUrl)))
                {
                    IWcfService serviceProxy = factory.CreateChannel();
                    string response = serviceProxy.Echo("Hello");
                }
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationException ce)
            {
                if (ce.InnerException == null)
                    throw;

                if (ce.InnerException.GetType() == typeof (HttpRequestException))
                {
                    var httpReqExcep = ce.InnerException as HttpRequestException;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Received HttpRequestException with unknown error code ")
                        .AppendLine(ce.InnerException.HResult.ToString())
                        .AppendLine("Full details for HttpRequestException:")
                        .AppendLine(httpReqExcep.ToString());
                    throw new CommunicationException(sb.ToString());
                }

                throw;
            }
        });

        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(notFoundUrl), string.Format("Expected exception message to contain: '{0}'", notFoundUrl));
    }

    [Fact]
    [OuterLoop]
    public static void UnknownUrl_Throws_ProtocolException()
    {
        string protocolExceptionUri = Endpoints.HttpProtocolError_Address;

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(10000);

        using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(protocolExceptionUri)))
        {
            IWcfService serviceProxy = factory.CreateChannel();
            ProtocolException exception = Assert.Throws<ProtocolException>(() =>
            {
                string response = serviceProxy.Echo("Hello");
            });

            // On .Net Native retail, exception message is stripped to include only parameter
            Assert.True(exception.Message.Contains(protocolExceptionUri), string.Format("Expected exception message to contain '{0}'", protocolExceptionUri));
        }
    }

    [Fact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_DirectThrow()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback(true);
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
            {
                factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
                IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }

                // Not closing the factory as an exception will always be thrown prior to this point.
            });

            Assert.Equal("ServicePingFaultCallback", exception.Code.Name);
            Assert.Equal("Reason: Testing FaultException returned from Duplex Callback", exception.Reason.GetMatchingTranslation().Text);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_ReturnsFaultedTask()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
            {
                factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
                IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }

                // Not closing the factory as an exception will always be thrown prior to this point.
            });

            Assert.Equal("ServicePingFaultCallback", exception.Code.Name);
            Assert.Equal("Reason: Testing FaultException returned from Duplex Callback", exception.Reason.GetMatchingTranslation().Text);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    // Verify product throws MessageSecurityException when the Dns identity from the server does not match the expectation
    public static void TCP_ServiceCertExpired_Throw_MessageSecurityException()
    {
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ExpiredServerCertResource_Address));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var result = serviceProxy.Echo(testString);
            Assert.True(false, "Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.True(false, string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }
            string exceptionMessage = exception.Message;
            Assert.True(exceptionMessage.Contains(Endpoints.Tcp_ExpiredServerCertResource_HostName), string.Format("Expected message contains {0}, actual message: {1}", Endpoints.Tcp_ExpiredServerCertResource_HostName, exceptionMessage));
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }

    [Fact]
    [OuterLoop]
    public static void TCP_ServiceCertFailedCustomValidate_Throw_Exception()
    {
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address), new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
        factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new MyCertificateValidator();

        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var result = serviceProxy.Echo(testString);
        }
        catch (Exception e)
        {
            string message = e.Message;
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }
}

public class MyCertificateValidator : X509CertificateValidator
{
    public const string exceptionMsg = "Not issued by a trusted issuer";

    public override void Validate(X509Certificate2 certificate)
    {
        if (certificate.IssuerName.Name != "RandomOne")
        {
            throw new Exception(exceptionMsg);
        }
    }
}
