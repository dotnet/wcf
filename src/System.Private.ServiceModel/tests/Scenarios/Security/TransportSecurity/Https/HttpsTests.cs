// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IdentityModel.Selectors;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using Infrastructure.Common;
using Xunit;

public class HttpsTests : ConditionalWcfTest
{
    // Client: CustomBinding set MessageVersion to Soap11
    // Server: BasicHttpsBinding default value is Soap11
    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(1123, PlatformID.AnyUnix)]
    public static void CrossBinding_Soap11_EchoString()
    {
        string variationDetails = "Client:: CustomBinding/MessageVersion=Soap11\nServer:: BasicHttpsBinding/DefaultValues";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Https_DefaultBinding_Address));
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
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Server_Accepts_Certificates))]
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
    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(1123, PlatformID.AnyUnix)]
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
    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(1123, PlatformID.AnyUnix)]
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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
#if FEATURE_NETNATIVE
    [ActiveIssue(959)] // Server certificate validation not supported in NET Native
#else
    [ActiveIssue(959, PlatformID.AnyUnix)] // Server certificate validation not supported on Linux and OS X
#endif
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

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_DefaultBinding_Address));
            clientCertThumb = ServiceUtilHelper.LocalCertThumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = factory.Credentials.ServiceCertificate.Authentication;
            //factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
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

    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed), nameof(Server_Accepts_Certificates))]
    [ActiveIssue(960, PlatformID.AnyUnix)]
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
            clientCertThumb = ServiceUtilHelper.LocalCertThumbprint;

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
}
