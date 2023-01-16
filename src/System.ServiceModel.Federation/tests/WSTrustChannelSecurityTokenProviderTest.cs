// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel.Description;
using System.Xml;
using Infrastructure.Common;
using Microsoft.IdentityModel.Protocols.WsFed;
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
            string claimUri = "http://example.org/claims/simplecustomclaim";
            string claimValue = "sample claim value";
            var claims = new Claims("dialect", new List<ClaimType>() { new ClaimType() { Uri = claimUri, IsOptional = false, Value = claimValue } });
            var issuerAddress = new EndpointAddress(new Uri("http://localhost/issuer.svc"));
            var targetAddress = new EndpointAddress(new Uri("http://localhost/target.svc"));
            var issuerBinding = new WSHttpBinding(SecurityMode.Transport);
            string eln1 = "Element1";
            string eln2 = "Element2";
            var additionalElements= new Collection<XmlElement>() { new XmlDocument().CreateElement(eln1), new XmlDocument().CreateElement(eln2) };
            
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

            (derivedTokenProvider as ICommunicationObject).Open();
            
            WsTrustRequest trustRequest = derivedTokenProvider.CreateWsTrustRequestHelper();

            Assert.NotNull(trustRequest);
            Assert.NotNull(trustRequest.Claims);
            Assert.Equal(claims.Dialect, trustRequest.Claims.Dialect);
            ClaimType ctype = trustRequest.Claims.ClaimTypes.FirstOrDefault();
            Assert.NotNull(ctype);
            Assert.Equal(claimUri, ctype.Uri);
            Assert.Equal(claimValue, ctype.Value);
            Assert.False(ctype.IsOptional);
            Assert.Equal(2, trustRequest.AdditionalXmlElements.Count);
            Assert.Equal(eln1, trustRequest.AdditionalXmlElements[0].Name);
            Assert.Equal(eln2, trustRequest.AdditionalXmlElements[1].Name);
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
