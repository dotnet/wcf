// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public class SetupValidationTests : ConditionalWcfTest
{
    //[todo: arcade] Resolve getting these tests running in Arcade.
    // Had to re-name the test project to end in "Tests" for Arcade to pick it up and run the tests.
    // But two of the tests fail and one is skipped, this prevents all the rest of the scenario tests from running.
    //[WcfFact]
    //[Condition(nameof(Root_Certificate_Installed))]
    //[OuterLoop]
    //public static void Root_Certificate_Correctly_Installed()
    //{
    //    string testString = "Hello";
    //    CustomBinding binding = null;
    //    ChannelFactory<IWcfService> factory = null;
    //    IWcfService serviceProxy = null;

    //    try
    //    {
    //        // *** SETUP *** \\
    //        binding = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpsTransportBindingElement());
    //        factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap12_Address));
    //        serviceProxy = factory.CreateChannel();

    //        // *** EXECUTE *** \\
    //        string result = serviceProxy.Echo(testString);

    //        // *** VALIDATE *** \\
    //        Assert.NotNull(result);
    //        Assert.Equal(testString, result);

    //        // *** CLEANUP *** \\
    //        factory.Close();
    //        ((ICommunicationObject)serviceProxy).Close();
    //    }
    //    finally
    //    {
    //        // *** ENSURE CLEANUP *** \\
    //        ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
    //    }
    //}

    //[WcfFact]
    //[Issue(1886, OS = OSID.OSX)]
    //[Condition(nameof(Client_Certificate_Installed))]
    //[OuterLoop]
    //public static void Client_Certificate_Correctly_Installed()
    //{
    //    string clientCertThumb = null;
    //    EndpointAddress endpointAddress = null;
    //    string testString = "Hello";
    //    ChannelFactory<IWcfService> factory = null;
    //    IWcfService serviceProxy = null;

    //    try
    //    {
    //        // *** SETUP *** \\
    //        NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
    //        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

    //        endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_Address),
    //            new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
    //        clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

    //        factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
    //        factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
    //        factory.Credentials.ClientCertificate.SetCertificate(
    //            StoreLocation.CurrentUser,
    //            StoreName.My,
    //            X509FindType.FindByThumbprint,
    //            clientCertThumb);

    //        serviceProxy = factory.CreateChannel();

    //        // *** EXECUTE *** \\
    //        string result = serviceProxy.Echo(testString);

    //        // *** VALIDATE *** \\
    //        Assert.Equal(testString, result);

    //        // *** CLEANUP *** \\
    //        ((ICommunicationObject)serviceProxy).Close();
    //        factory.Close();
    //    }
    //    finally
    //    {
    //        // *** ENSURE CLEANUP *** \\
    //        ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
    //    }
    //}

    //[WcfFact]
    //[Issue(1945, OS = OSID.OSX)]
    //[Condition(nameof(Peer_Certificate_Installed))]
    //[OuterLoop]
    //public static void Peer_Certificate_Correctly_Installed()
    //{
    //    // *** SETUP *** \\
    //    InvalidOperationException exception = null;

    //    // *** EXECUTE *** \\
    //    try
    //    {
    //        DateTime now = DateTime.Now;
    //        X509Certificate2 certificate = ServiceUtilHelper.PeerCertificate;
    //        StoreName storeName = StoreName.TrustedPeople;
    //        StoreLocation storeLocation = StoreLocation.CurrentUser;

    //        Assert.True(certificate != null, "Certificate is null");
    //        Assert.True(now > certificate.NotBefore,
    //               String.Format("The current date {{0}} is earlier than NotBefore ({1})",
    //                             now,
    //                             certificate.NotBefore));

    //        Assert.True(now < certificate.NotAfter,
    //        String.Format("The current date {{0}} is later than NotAfter ({1})",
    //                     now,
    //                     certificate.NotAfter));

    //        using (X509Store store = new X509Store(storeName, storeLocation))
    //        {
    //            store.Open(OpenFlags.ReadOnly);
    //            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, validOnly: true);
    //            Assert.True(certificates.Count == 1,
    //                        String.Format("Did not find valid certificate with thumbprint {0} in StoreName '{1}', StoreLocation '{2}'",
    //                                      certificate.Thumbprint,
    //                                      storeName,
    //                                      storeLocation));
    //        }

    //        using (X509Store store = new X509Store(StoreName.Disallowed, storeLocation))
    //        {
    //            store.Open(OpenFlags.ReadOnly);
    //            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, validOnly: false);
    //            Assert.True(certificates.Count == 0, "Certificate was found in Disallowed store.");
    //        }
    //    }
    //    catch (InvalidOperationException e)
    //    {
    //        exception = e;
    //    }

    //    // *** VALIDATE *** \\
    //    // Validate rather than allowing an exception to propagate
    //    // to be clear the exception was anticipated. 
    //    Assert.True(exception == null, exception == null ? String.Empty : exception.ToString());
    //}
}
