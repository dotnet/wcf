// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
using Infrastructure.Common;
using Xunit;

// Regression tests for dotnet/wcf#5883.
//
// On a NetTcp + TransportWithMessageCredential + Certificate session the client establishes
// a Secure Conversation Token (SCT). When the server's SessionKeyRenewalInterval elapses the
// next outgoing message triggers an RST/SCT/Renew exchange that is signed with the SCT-derived
// symmetric key (KeyedHashAlgorithm).
//
// Pre-fix, WSSecurityOneDotZeroSendSecurityHeader.CompletePrimarySignatureCore added a To-header
// reference for any signing key (symmetric or asymmetric). That had two visible effects on the
// renew exchange:
//   1. _toHeaderStream was consumed by the primary signature and then re-read for the endorsing
//      signature, which threw XmlException "Root element is missing." from inside
//      SignedXml.ComputeSignature and surfaced as a CommunicationObjectFaultedException.
//   2. The extra <Reference> on the symmetric primary signature is a shape that .NET Framework
//      4.8 servers reject with "The security protocol cannot verify the incoming message."
//
// The fix limits the To-header reference to the asymmetric primary signature path. This test
// hosts the service with a short SessionKeyRenewalInterval so the renew exchange is forced a
// few seconds in, then sends messages across the renewal boundary. Without the fix, the call
// that triggers the renew faults the channel.
public class SctRenewalRegressionTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NetTcp_TransWithMessCred_CertClientCredential_SctRenewal_Succeeds()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            // The server's SessionKeyRenewalInterval is configured to ~10 seconds. Run the
            // echo loop for ~25 seconds so we cross at least one renewal boundary.
            const int RenewalCrossingTotalSeconds = 25;
            const int CallIntervalSeconds = 2;

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_SecModeTransWithMessCred_ClientCredTypeCert_Renew));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();
            ((ICommunicationObject)serviceProxy).Open();

            // *** EXECUTE *** \\
            int callsSent = 0;
            int elapsed = 0;
            while (elapsed < RenewalCrossingTotalSeconds)
            {
                callsSent++;
                string testString = string.Format("Hello #{0}", callsSent);
                string result = serviceProxy.Echo(testString);

                Assert.Equal(testString, result);

                Thread.Sleep(TimeSpan.FromSeconds(CallIntervalSeconds));
                elapsed += CallIntervalSeconds;
            }

            // *** VALIDATE *** \\
            // The pre-fix bug manifested as the channel faulting at the renew boundary, so the
            // surviving signal is "all calls round-tripped and the channel is still Opened".
            Assert.Equal(CommunicationState.Opened, ((ICommunicationObject)serviceProxy).State);

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
