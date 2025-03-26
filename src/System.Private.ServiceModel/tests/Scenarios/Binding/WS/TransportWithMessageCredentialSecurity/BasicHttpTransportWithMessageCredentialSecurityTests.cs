using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public class BasicHttpTransportWithMessageCredentialSecurityTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void BasicHttps_SecModeTransWithMessCred_CertClientCredential_Succeeds()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.BasicHttps_SecModeTransWithMessCred_ClientCredTypeCert));
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

    [WcfTheory]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    [InlineData(TransferMode.Buffered)]
    [InlineData(TransferMode.Streamed)]
    public static void BasicHttps_SecModeTransWithMessCred_UserNameClientCredential_Succeeds(TransferMode transferMode)
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        string username = null;
        string password = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.TransferMode = transferMode;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.BasicHttps_SecModeTransWithMessCred_ClientCredTypeUserName + $"/{Enum.GetName(typeof(TransferMode), transferMode)}"));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            username = Guid.NewGuid().ToString("n").Substring(0, 8);
            char[] usernameArr = username.ToCharArray();
            Array.Reverse(usernameArr);
            password = new string(usernameArr);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;

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
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void Https_SecModeTransWithMessCred_UserNameClientCredential_Succeeds()
    {
        string testString = "Hello";
        string result = null;
        string username = null;
        string password = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_SecModeTransWithMessCred_ClientCredTypeUserName));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            username = Guid.NewGuid().ToString("n").Substring(0, 8);
            char[] usernameArr = username.ToCharArray();
            Array.Reverse(usernameArr);
            password = new string(usernameArr);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            result = serviceProxy.Echo(testString);

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

    [WcfTheory]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    [InlineData(true)]
    [InlineData(false)]
    public static void Https_InvalidClientCredential_EnableUnsecuredResponse_DifferentException(bool enableUnsecuredResponse)
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        string username = null;
        string password = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        TransferMode transferMode = TransferMode.Buffered;
        try
        {
            // *** SETUP *** \\
            TextMessageEncodingBindingElement textEncoding = new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.Soap11 };
            HttpsTransportBindingElement httpsTransport = new HttpsTransportBindingElement() { TransferMode = transferMode };
            TransportSecurityBindingElement sec = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            sec.EnableUnsecuredResponse = enableUnsecuredResponse;
            CustomBinding customBinding = new CustomBinding(sec, textEncoding, httpsTransport);            
            endpointAddress = new EndpointAddress(new Uri(Endpoints.BasicHttps_SecModeTransWithMessCred_ClientCredTypeUserName + $"/{Enum.GetName(typeof(TransferMode), transferMode)}"));
            factory = new ChannelFactory<IWcfService>(customBinding, endpointAddress);
            username = Guid.NewGuid().ToString("n").Substring(0, 8);
            char[] usernameArr = username.ToCharArray();
            Array.Reverse(usernameArr);
            password = new string(usernameArr);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password + "1";//invalid password

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Fail("should throw exception earlier");

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            if (enableUnsecuredResponse)
            {
                Assert.True(ex is System.ServiceModel.Security.SecurityAccessDeniedException);
            }
            else
            {
                Assert.True(ex is System.ServiceModel.Security.MessageSecurityException);
            }
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
