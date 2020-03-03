using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Federation;
using System.Text;
using Infrastructure.Common;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens.Saml2;
using Xunit;

public class WSFederationHttpBindingTestsTests : ConditionalWcfTest
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
                    KeyType = SecurityKe    yType.BearerKey,
                    Target = tokenTargetAddress,
                    TokenType = Saml2Constants.OasisWssSaml2TokenProfile11
                });

            factory = new ChannelFactory<IWcfService>(federationBinding, serviceEndpointAddress);
            // TODO: Fix the need for this
            var credentials = new WsTrustChannelClientCredentials();
            credentials.UserName.UserName = "AUser";
            credentials.UserName.Password = "MyPassword";

            factory.Endpoint.EndpointBehaviors.Remove(typeof(ClientCredentials));
            factory.Endpoint.EndpointBehaviors.Add(credentials);

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
