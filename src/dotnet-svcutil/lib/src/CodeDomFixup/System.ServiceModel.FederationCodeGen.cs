// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.IdentityModel.Tokens
{
    public class Saml2AssertionKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        public Saml2AssertionKeyIdentifierClause(string assertionId) : base(assertionId, null, 0) { }
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause) => default;
        public override string ToString() => default;
        public static bool Matches(string assertionId, SecurityKeyIdentifierClause keyIdentifierClause) => default;
    }
    public class SamlAssertionKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        public SamlAssertionKeyIdentifierClause(string assertionId) : base(assertionId, null, 0) { }
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause) => default;
        public override string ToString() => default;
        public static bool Matches(string assertionId, SecurityKeyIdentifierClause keyIdentifierClause) => default;
    }
}

namespace System.ServiceModel.Federation
{
    internal class WSFederationBindingElement : System.ServiceModel.Channels.BindingElement
    {
        public WSFederationBindingElement(WSTrustTokenParameters wsTrustTokenParameters, System.ServiceModel.Channels.SecurityBindingElement securityBindingElement) { }
        public WSTrustTokenParameters WSTrustTokenParameters { get => default; }
        public System.ServiceModel.Channels.SecurityBindingElement SecurityBindingElement { get => default; }
        public override System.ServiceModel.Channels.BindingElement Clone() => default;
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) => default;
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) => default;
    }
    public class WSFederationHttpBinding : System.ServiceModel.WSHttpBinding
    {
        public WSFederationHttpBinding(WSTrustTokenParameters wsTrustTokenParameters) : base(System.ServiceModel.SecurityMode.TransportWithMessageCredential) { }
        public WSTrustTokenParameters WSTrustTokenParameters { get => default; }
        protected override System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity() => default;
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() => default;
        protected override System.ServiceModel.Channels.TransportBindingElement GetTransport() => default;
    }
    public class WSTrustChannelClientCredentials : System.ServiceModel.Description.ClientCredentials
    {
        public WSTrustChannelClientCredentials() : base() { }
        public WSTrustChannelClientCredentials(System.ServiceModel.Description.ClientCredentials clientCredentials) : base(clientCredentials) { }
        protected WSTrustChannelClientCredentials(WSTrustChannelClientCredentials other) : base(other) { }
        public System.ServiceModel.Description.ClientCredentials ClientCredentials { get => default; private set { } }
        protected override System.ServiceModel.Description.ClientCredentials CloneCore() => default;
        public override System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager() => default;
    }
    public class WSTrustChannelSecurityTokenManager : System.ServiceModel.ClientCredentialsSecurityTokenManager
    {
        public WSTrustChannelSecurityTokenManager(WSTrustChannelClientCredentials wsTrustChannelClientCredentials) : base(wsTrustChannelClientCredentials) { }
        public override System.IdentityModel.Selectors.SecurityTokenProvider CreateSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement) => default;
    }
    //public class WSTrustChannelSecurityTokenProvider : System.IdentityModel.Selectors.SecurityTokenProvider
    //{
    //    public WSTrustChannelSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement) { }
    //    protected virtual Microsoft.IdentityModel.Protocols.WsTrust.WsTrustRequest CreateWsTrustRequest() => default;
    //    protected override System.IdentityModel.Tokens.SecurityToken GetTokenCore(TimeSpan timeout) => default;
    //    public override bool SupportsTokenCancellation => default;
    //    public override bool SupportsTokenRenewal => default;
    //}
    public class WSTrustTokenParameters : System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters
    {
        public static readonly bool DefaultCacheIssuedTokens = default;
        public static readonly int DefaultIssuedTokenRenewalThresholdPercentage = default;
        public static readonly TimeSpan DefaultMaxIssuedTokenCachingTime = default;
        public static readonly System.IdentityModel.Tokens.SecurityKeyType DefaultSecurityKeyType = default;
        public WSTrustTokenParameters() { }
        protected WSTrustTokenParameters(WSTrustTokenParameters other) : base(other) { }
        protected override System.ServiceModel.Security.Tokens.SecurityTokenParameters CloneCore() => default;
        public System.Collections.Generic.ICollection<System.Xml.XmlElement> AdditionalRequestParameters => default;
        public bool CacheIssuedTokens { get => default; set { } }
        //public System.Collections.Generic.ICollection<Microsoft.IdentityModel.Protocols.WsFed.ClaimType> ClaimTypes => default;
        public int IssuedTokenRenewalThresholdPercentage => default;
        public int? KeySize { get => default; set { } }
        public TimeSpan MaxIssuedTokenCachingTime { get => default; set { } }
        public MessageSecurityVersion MessageSecurityVersion { get => default; set { } }
        public string RequestContext { get => default; set { } }
        public string Target { get => default; set { } }
        public bool EstablishSecurityContext { get => default; set { } }
        public static WSTrustTokenParameters CreateWSFederationTokenParameters(System.ServiceModel.Channels.Binding issuerBinding, System.ServiceModel.EndpointAddress issuerAddress) => default;
        public static WSTrustTokenParameters CreateWS2007FederationTokenParameters(System.ServiceModel.Channels.Binding issuerBinding, System.ServiceModel.EndpointAddress issuerAddress) => default;
    }
}
