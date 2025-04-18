// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class HttpsTests : ConditionalWcfTest
{
    [WcfTheory]
    [InlineData(WSMessageEncoding.Text)]
    [InlineData(WSMessageEncoding.Mtom)]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(Server_Accepts_Certificates),
           nameof(SSL_Available))]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String(WSMessageEncoding messageEncoding)
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding basicHttpsBinding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            basicHttpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            basicHttpsBinding.MessageEncoding = messageEncoding;

            string clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;
            factory = new ChannelFactory<IWcfService>(basicHttpsBinding, new EndpointAddress(Endpoints.Https_DefaultBinding_Address + Enum.GetName(typeof(WSMessageEncoding), messageEncoding)));
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);
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

    // Client: CustomBinding set MessageVersion to Soap11
    // Server: BasicHttpsBinding default value is Soap11
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [OuterLoop]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    public static void CrossBinding_Soap11_EchoString()
    {
        string variationDetails = "Client:: CustomBinding/MessageVersion=Soap11\nServer:: BasicHttpsBinding/DefaultValues";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Https_DefaultBinding_Address_Text));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }

    // Client and Server bindings setup exactly the same using default settings.
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void SameBinding_DefaultSettings_EchoString()
    {
        string variationDetails = "Client:: CustomBinding/DefaultValues\nServer:: CustomBinding/DefaultValues";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpsTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap12_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }

    // Client and Server bindings setup exactly the same using Soap11
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void SameBinding_Soap11_EchoString()
    {
        string variationDetails = "Client:: CustomBinding/MessageVersion=Soap11\nServer:: CustomBinding/MessageVersion=Soap11";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap11_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }

    // Client and Server bindings setup exactly the same using Soap12
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void SameBinding_Soap12_EchoString()
    {
        string variationDetails = "Client:: CustomBinding/MessageVersion=Soap12\nServer:: CustomBinding/MessageVersion=Soap12";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpsTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap12_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void ServerCertificateValidation_EchoString()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_DefaultBinding_Address_Text));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            MyX509CertificateValidator myX509CertificateValidator = new MyX509CertificateValidator(ScenarioTestHelpers.CertificateIssuerName);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CustomCertificateValidator = myX509CertificateValidator;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(myX509CertificateValidator.validateMethodWasCalled, "The Validate method of the X509CertificateValidator was NOT called.");
            Assert.Equal(testString, result);

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

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(SSL_Available))]
    [OuterLoop]
    public static async Task ServerCertificateValidationUsingIdentity_EchoString()
    {
        EndpointAddress endpointAddress = null;
        X509Certificate2 serviceCertificate = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());

            serviceCertificate = await ServiceUtilHelper.GetServiceMacineCertFromServerAsync();
            var identity = new X509CertificateEndpointIdentity(serviceCertificate);
            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_DefaultBinding_Address_Text), identity);

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void ServerCertificateValidationUsingIdentity_Throws_EchoString()
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());

            // This is intentionally the wrong certificate
            var identity = new X509CertificateEndpointIdentity(ServiceUtilHelper.ClientCertificate);
            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_DefaultBinding_Address_Text), identity);

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Assert.Throws<SecurityNegotiationException>(() => { _ = serviceProxy.Echo(testString); });

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

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Server_Accepts_Certificates),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void ClientCertificate_EchoString()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding basicHttpsBinding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            basicHttpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_ClientCertificateAuth_Address));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(basicHttpsBinding, endpointAddress);
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(Server_Accepts_Certificates),
           nameof(SSL_Available))]
    [OuterLoop]
    public static void HttpExpect100Continue_ClientCertificate_True()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding basicHttpsBinding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            basicHttpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_ClientCertificateAuth_Address));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(basicHttpsBinding, endpointAddress);
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Dictionary<string, string> requestHeaders = serviceProxy.GetRequestHttpHeaders();

            // *** VALIDATE *** \\
            bool expectHeaderSent = requestHeaders.TryGetValue("Expect", out var expectHeader);
            Assert.True(expectHeaderSent, "Expect header should have been sent but wasn't");
            Assert.Equal("100-Continue", expectHeader, StringComparer.OrdinalIgnoreCase);

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

    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(Server_Accepts_Certificates),
           nameof(SSL_Available),
           nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_RequestReply_CertificateCredentials()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress;
        NetHttpsBinding binding = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Buffered;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            var uriBuilder = new UriBuilder(Endpoints.WebSocketHttpsRequestReplyClientCertAuth_Address);
            uriBuilder.Scheme = "wss";
            endpointAddress = new EndpointAddress(uriBuilder.Uri);
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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

    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
       nameof(Client_Certificate_Installed),
       nameof(Server_Accepts_Certificates),
       nameof(SSL_Available),
       nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_ServerCertificateValidation()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress;
        NetHttpsBinding binding = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Buffered;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            var uriBuilder = new UriBuilder(Endpoints.WebSocketHttpsRequestReplyClientCertAuth_Address);
            uriBuilder.Scheme = "wss";
            endpointAddress = new EndpointAddress(uriBuilder.Uri);
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            MyX509CertificateValidator myX509CertificateValidator = new MyX509CertificateValidator(ScenarioTestHelpers.CertificateIssuerName);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CustomCertificateValidator = myX509CertificateValidator;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(myX509CertificateValidator.validateMethodWasCalled, "The Validate method of the X509CertificateValidator was NOT called.");
            Assert.Equal(testString, result);

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
