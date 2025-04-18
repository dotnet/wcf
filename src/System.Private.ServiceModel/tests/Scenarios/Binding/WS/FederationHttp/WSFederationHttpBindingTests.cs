// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation;
using Infrastructure.Common;
using Microsoft.IdentityModel.Tokens.Saml2;
using Xunit;

public class WSFederationHttpBindingTests : ConditionalWcfTest
{
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
           nameof(Client_Certificate_Installed),
           nameof(SSL_Available),
           nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    public static void WSFederationHttpBindingTests_Succeeds(MessageSecurityVersion messageSecurityVersion, SecurityKeyType securityKeyType, bool useSecureConversation, string endpointSuffix, WSMessageEncoding encoding)
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        EndpointAddress issuerAddress = null;
        EndpointAddress serviceEndpointAddress = null;
        string tokenTargetAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            issuerAddress = new EndpointAddress(new Uri(Endpoints.WSFederationAuthorityLocalSTS + endpointSuffix));
            tokenTargetAddress = Endpoints.Https_SecModeTransWithMessCred_ClientCredTypeIssuedTokenSaml2 + endpointSuffix + (useSecureConversation ? "/sc" : string.Empty) + "/" + Enum.GetName(typeof(WSMessageEncoding), encoding);
            serviceEndpointAddress = new EndpointAddress(new Uri(tokenTargetAddress));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            issuerBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            WSFederationHttpBinding federationBinding = new WSFederationHttpBinding(
                new WSTrustTokenParameters
                {
                    IssuerAddress = issuerAddress,
                    IssuerBinding = issuerBinding,
                    KeyType = securityKeyType,
                    TokenType = Saml2Constants.OasisWssSaml2TokenProfile11,
                    MessageSecurityVersion = messageSecurityVersion,
                });
            federationBinding.MessageEncoding = encoding;
            federationBinding.Security.Message.EstablishSecurityContext = useSecureConversation;
            factory = new ChannelFactory<IWcfService>(federationBinding, serviceEndpointAddress);

            factory.Credentials.UserName.UserName = "PlaceholderUser";
			// [SuppressMessage("Microsoft.Security", "CS002:SecretInNextLine", Justification="Not a real secret")]
            factory.Credentials.UserName.Password = "PlaceholderPassword";
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

    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
       nameof(Client_Certificate_Installed),
       nameof(SSL_Available),
       nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    [WcfFact]
    public static void WSTrustTokeParameters_WSStaticHelper()
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        EndpointAddress issuerAddress = null;
        EndpointAddress serviceEndpointAddress = null;
        string tokenTargetAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            issuerAddress = new EndpointAddress(new Uri(Endpoints.WSFederationAuthorityLocalSTS + "wsHttp/wstrustFeb2005"));
            tokenTargetAddress = Endpoints.Https_SecModeTransWithMessCred_ClientCredTypeIssuedTokenSaml2 + "wsHttp/wstrustFeb2005/Text";
            serviceEndpointAddress = new EndpointAddress(new Uri(tokenTargetAddress));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            issuerBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            WSFederationHttpBinding federationBinding = new WSFederationHttpBinding(WSTrustTokenParameters.CreateWSFederationTokenParameters(issuerBinding, issuerAddress));
            federationBinding.Security.Message.EstablishSecurityContext = false;
            factory = new ChannelFactory<IWcfService>(federationBinding, serviceEndpointAddress);

            factory.Credentials.UserName.UserName = "PlaceholderUser";
			// [SuppressMessage("Microsoft.Security", "CS002:SecretInNextLine", Justification="Not a real secret")]
            factory.Credentials.UserName.Password = "PlaceholderPassword";
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

    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
        nameof(Client_Certificate_Installed),
        nameof(SSL_Available),
        nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    [WcfFact]
    public static void WS2007TrustTokeParameters_WSStaticHelper()
    {
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        EndpointAddress issuerAddress = null;
        EndpointAddress serviceEndpointAddress = null;
        string tokenTargetAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            issuerAddress = new EndpointAddress(new Uri(Endpoints.WSFederationAuthorityLocalSTS + "wsHttp/wstrust13"));
            tokenTargetAddress = Endpoints.Https_SecModeTransWithMessCred_ClientCredTypeIssuedTokenSaml2 + "wsHttp/wstrust13/Text";
            serviceEndpointAddress = new EndpointAddress(new Uri(tokenTargetAddress));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            issuerBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            WSFederationHttpBinding federationBinding = new WSFederationHttpBinding(WSTrustTokenParameters.CreateWS2007FederationTokenParameters(issuerBinding, issuerAddress));
            federationBinding.Security.Message.EstablishSecurityContext = false;
            factory = new ChannelFactory<IWcfService>(federationBinding, serviceEndpointAddress);

            factory.Credentials.UserName.UserName = "PlaceholderUser";
			// [SuppressMessage("Microsoft.Security", "CS002:SecretInNextLine", Justification="Not a real secret")]
            factory.Credentials.UserName.Password = "PlaceholderPassword";
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

    public static IEnumerable<object[]> GetTestVariations()
    {
        foreach (WSMessageEncoding messageEncoding in Enum.GetValues(typeof(WSMessageEncoding)))
        {
            // Equivalent to WS2007FederationHttpBinding
            yield return new object[] { MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, SecurityKeyType.SymmetricKey, false, "wsHttp/wstrust13", messageEncoding };
            yield return new object[] { MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, SecurityKeyType.SymmetricKey, true, "wsHttp/wstrust13", messageEncoding };
            // Equivalent to WSFederationHttpBinding
            yield return new object[] { MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, SecurityKeyType.SymmetricKey, false, "wsHttp/wstrustFeb2005", messageEncoding };
            yield return new object[] { MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, SecurityKeyType.SymmetricKey, true, "wsHttp/wstrustFeb2005", messageEncoding };
        }
    }
}
