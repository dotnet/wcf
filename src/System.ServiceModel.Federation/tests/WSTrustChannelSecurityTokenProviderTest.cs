// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;
using Infrastructure.Common;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFed;
using Microsoft.IdentityModel.Protocols.WsPolicy;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens.Saml2;
using Xunit;

namespace System.ServiceModel.Federation.Tests
{
    public static class WSTrustChannelSecurityTokenProviderTest
    {
        [WcfFact]
        public static void EnsibilityTest()
        {
            var claims = new Claims("dialect", new List<ClaimType>() { new ClaimType() {  Uri= "http://example.org/claims/simplecustomclaim", IsOptional=false, Value="sample claim value"} });
            var issuerAddress = new EndpointAddress(new Uri("http://localhost/issuer.svc"));
            var targetAddress = new EndpointAddress(new Uri("http://localhost/target.svc"));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            var additionalElements= new Collection<XmlElement>() { new XmlDocument().CreateElement("Element1"), new XmlDocument().CreateElement("Element2") };
            
            var tokenParams = new WSTrustTokenParameters
            {
                Claims= claims,
                IssuerAddress = issuerAddress,
                IssuerBinding = issuerBinding,
                KeyType = SecurityKeyType.SymmetricKey,
                TokenType = Saml2Constants.OasisWssSaml2TokenProfile11,
                MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11
            };
            
            foreach(XmlElement element in additionalElements)
            {
                tokenParams.AdditionalRequestParameters.Add(element);
            }

            var tokenRequirement = new System.IdentityModel.Selectors.SecurityTokenRequirement()
            {
                TokenType = "urn:oasis:names:tc:SAML:1.0:assertion"
            };

            tokenRequirement.Properties["http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/IssuedSecurityTokenParameters"] = tokenParams;
            tokenRequirement.Properties["http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/TargetAddress"] = targetAddress;
            tokenRequirement.Properties["http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/SecurityAlgorithmSuite"] = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;

            var derivedTokenProvider = new WSTrustChannelSecurityTokenProviderDerived(tokenRequirement);
            
            typeof(WSTrustChannelSecurityTokenProvider).GetField("_requestSerializationContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(derivedTokenProvider, new WsSerializationContext(WsTrustVersion.TrustFeb2005));
            typeof(WSTrustChannelSecurityTokenProvider).GetProperty("RequestContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(derivedTokenProvider, Guid.NewGuid().ToString());

            WsTrustRequest trustRequest = derivedTokenProvider.CreateWsTrustRequestHelper();
            
            Assert.True(trustRequest.Claims.Equals(tokenParams.Claims));
            foreach(XmlElement element in additionalElements)
            {
                Assert.True(trustRequest.AdditionalXmlElements.Contains(element));
            }            
        }
    }

    public class WSTrustChannelSecurityTokenProviderDerived : WSTrustChannelSecurityTokenProvider
    {
        public WSTrustChannelSecurityTokenProviderDerived(SecurityTokenRequirement tokenRequirement) : base(tokenRequirement)
        {
        }
        
        public Microsoft.IdentityModel.Protocols.WsTrust.WsTrustRequest CreateWsTrustRequestHelper()
        {
            ClientCredentials = new ClientCredentials();
            return base.CreateWsTrustRequest();
        }
    }
}
