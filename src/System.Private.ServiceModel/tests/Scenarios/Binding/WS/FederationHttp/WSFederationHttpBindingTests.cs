// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation;
using Infrastructure.Common;
using Microsoft.IdentityModel.Tokens.Saml2;
using Xunit;

public class WSFederationHttpBindingTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(2870, OS = OSID.AnyOSX)]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(SSL_Available))]
    [OuterLoop]
    public static void WSFederationHttpBindingTests_Succeeds()
    {
        EndpointAddress issuerAddress = null;
        EndpointAddress serviceEndpointAddress = null;
        string tokenTargetAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            issuerAddress = new EndpointAddress(new Uri(Endpoints.WSFederationAuthorityLocalSTS));
            tokenTargetAddress = Endpoints.Https_SecModeTransWithMessCred_ClientCredTypeIssuedTokenSaml2;
            serviceEndpointAddress = new EndpointAddress(new Uri(tokenTargetAddress));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            issuerBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            WsFederationHttpBinding federationBinding = new WsFederationHttpBinding(
                new WsTrustTokenParameters
                {
                    IssuerAddress = issuerAddress,
                    IssuerBinding = issuerBinding,
                    KeyType = SecurityKeyType.BearerKey,
                    Target = tokenTargetAddress,
                    TokenType = Saml2Constants.OasisWssSaml2TokenProfile11
                });
            //federationBinding.Security.Message.EstablishSecurityContext = false;
            var customBinding = new CustomBinding(federationBinding);
            var sbe = customBinding.Elements.Find<SecurityBindingElement>();
            sbe.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

            factory = new ChannelFactory<IWcfService>(customBinding, serviceEndpointAddress);
            // TODO: Fix the need for this
            factory.Credentials.UserName.UserName = "AUser";
            factory.Credentials.UserName.Password = "MyPassword";
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
