// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ServiceModel
{
    public partial class DnsEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public DnsEndpointIdentity(string dnsName) { }
    }
    public abstract partial class MessageSecurityVersion
    {
        internal MessageSecurityVersion() { }
        public abstract System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion { get; }
        public System.ServiceModel.Security.SecureConversationVersion SecureConversationVersion { get { return default; } }
        public abstract System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion { get; }
        public System.ServiceModel.Security.SecurityVersion SecurityVersion { get { return default; } }
        public System.ServiceModel.Security.TrustVersion TrustVersion { get { return default; } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 { get { return default; } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11 { get { return default; } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 { get { return default; } }
    }
    public partial class SpnEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public SpnEndpointIdentity(string spnName) { }
        public static System.TimeSpan SpnLookupTime { get { return default; } set { } }
    }
    public partial class UpnEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public UpnEndpointIdentity(string upnName) { }
    }
}
namespace System.ServiceModel.Channels
{
    public partial interface ISecurityCapabilities
    {
        System.Net.Security.ProtectionLevel SupportedRequestProtectionLevel { get; }
        System.Net.Security.ProtectionLevel SupportedResponseProtectionLevel { get; }
        bool SupportsClientAuthentication { get; }
        bool SupportsClientWindowsIdentity { get; }
        bool SupportsServerAuthentication { get; }
    }
    public sealed partial class LocalClientSecuritySettings
    {
        public LocalClientSecuritySettings() { }
        public System.TimeSpan MaxClockSkew { get { return default; } set { } }
        public System.TimeSpan ReplayWindow { get { return default; } set { } }
        public System.TimeSpan TimestampValidityDuration { get { return default; } set { } }
        public System.ServiceModel.Channels.LocalClientSecuritySettings Clone() { return default; }
    }
    public abstract partial class SecurityBindingElement : System.ServiceModel.Channels.BindingElement
    {
        internal SecurityBindingElement() { }
        public System.ServiceModel.Security.Tokens.SupportingTokenParameters EndpointSupportingTokenParameters { get { return default; } }
        public bool IncludeTimestamp { get { return default; } set { } }
        public System.ServiceModel.Security.SecurityAlgorithmSuite DefaultAlgorithmSuite { get { return default; } set { } }
        public System.ServiceModel.Channels.LocalClientSecuritySettings LocalClientSettings { get { return default; } }
        public System.ServiceModel.MessageSecurityVersion MessageSecurityVersion { get { return default; } set { } }
        public System.ServiceModel.Channels.SecurityHeaderLayout SecurityHeaderLayout { get { return default; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        protected abstract System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(System.ServiceModel.Channels.BindingContext context);
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public static System.ServiceModel.Channels.SecurityBindingElement CreateSecureConversationBindingElement(System.ServiceModel.Channels.SecurityBindingElement bootstrapSecurity) { return default; }
        public static System.ServiceModel.Channels.TransportSecurityBindingElement CreateUserNameOverTransportBindingElement() { return default; }
        public static TransportSecurityBindingElement CreateIssuedTokenOverTransportBindingElement(System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters issuedTokenParameters) { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override string ToString() { return default; }
        public System.ServiceModel.Security.SecurityKeyEntropyMode KeyEntropyMode { get { return default;} set { } }
    }
    public enum SecurityHeaderLayout
    {
        Lax = 1,
        Strict = 0,
    }
    public sealed partial class TransportSecurityBindingElement : System.ServiceModel.Channels.SecurityBindingElement
    {
        public TransportSecurityBindingElement() { }
        protected override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
}
namespace System.ServiceModel.Security
{
    public enum SecurityKeyEntropyMode
    {
        ClientEntropy,
        ServerEntropy,
        CombinedEntropy
    }
    public abstract partial class BasicSecurityProfileVersion
    {
        internal BasicSecurityProfileVersion() { }
        public static System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfile10 { get { return default; } }
    }
    public abstract partial class SecureConversationVersion
    {
        internal SecureConversationVersion() { }
        public static System.ServiceModel.Security.SecureConversationVersion Default { get { return default; } }
        public System.Xml.XmlDictionaryString Namespace { get { return default; } }
        public System.Xml.XmlDictionaryString Prefix { get { return default; } }
        public static System.ServiceModel.Security.SecureConversationVersion WSSecureConversationFeb2005 { get { return default; } }
    }
    public abstract partial class SecurityAlgorithmSuite
    {
        static public SecurityAlgorithmSuite TripleDes { get { return default; } }
        protected SecurityAlgorithmSuite() { }
        public abstract string DefaultCanonicalizationAlgorithm { get; }
        public abstract string DefaultDigestAlgorithm { get; }
        public abstract string DefaultEncryptionAlgorithm { get; }
        public abstract int DefaultEncryptionKeyDerivationLength { get; }
        public abstract string DefaultSymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultAsymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultSymmetricSignatureAlgorithm { get; }
        public abstract string DefaultAsymmetricSignatureAlgorithm { get; }
        public abstract int DefaultSignatureKeyDerivationLength { get; }
        public abstract int DefaultSymmetricKeyLength { get; }
    }
    public abstract partial class SecurityPolicyVersion
    {
        internal SecurityPolicyVersion() { }
        public string Namespace { get { return default; } }
        public string Prefix { get { return default; } }
        public static System.ServiceModel.Security.SecurityPolicyVersion WSSecurityPolicy11 { get { return default; } }
    }
    public abstract partial class SecurityVersion
    {
        internal SecurityVersion() { }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity10 { get { return default; } }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity11 { get { return default; } }
    }
    public abstract partial class TrustVersion
    {
        internal TrustVersion() { }
        public static System.ServiceModel.Security.TrustVersion Default { get { return default; } }
        public System.Xml.XmlDictionaryString Namespace { get { return default; } }
        public System.Xml.XmlDictionaryString Prefix { get { return default; } }
        public static System.ServiceModel.Security.TrustVersion WSTrustFeb2005 { get { return default; } }
    }
}
namespace System.ServiceModel.Security.Tokens
{
    public partial class BinarySecretSecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        public BinarySecretSecurityToken(string id, byte[] key) { }
        public BinarySecretSecurityToken(byte[] key) { }
        protected BinarySecretSecurityToken(string id, int keySizeInBits, bool allowCrypto) { }
        protected BinarySecretSecurityToken(string id, byte[] key, bool allowCrypto) { }
        public override string Id { get { return default; } }
        public override DateTime ValidFrom { get { return default; } }
        public override DateTime ValidTo { get { return default; } }
        public int KeySize { get { return default; } }
        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys { get { return default; } }
        public byte[] GetKeyBytes() { return default; }
    }
    public class IssuedSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        protected IssuedSecurityTokenParameters(IssuedSecurityTokenParameters other) { }
        public IssuedSecurityTokenParameters() { }
        public MessageSecurityVersion DefaultMessageSecurityVersion { get { return default; } set { } }
        public EndpointAddress IssuerAddress { get { return default; } set { } }
        public Channels.Binding IssuerBinding { get { return default; } set { } }
        public System.IdentityModel.Tokens.SecurityKeyType KeyType { get { return default; } set { } }
        public string TokenType { get { return default; } set { } }
    }
    public partial class SecureConversationSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        public SecureConversationSecurityTokenParameters() { }
        public SecureConversationSecurityTokenParameters(System.ServiceModel.Channels.SecurityBindingElement bootstrapSecurityBindingElement) { }
        public System.ServiceModel.Channels.SecurityBindingElement BootstrapSecurityBindingElement { get { return default; } set { } }
        public bool RequireCancellation { get { return default; } set { } }
    }
    public abstract partial class SecurityTokenParameters
    {
        internal SecurityTokenParameters() { }
        public bool RequireDerivedKeys { get { return default; } set { } }
        public System.ServiceModel.Security.Tokens.SecurityTokenParameters Clone() { return default; }
    }
    public abstract partial class ServiceModelSecurityTokenRequirement : System.IdentityModel.Selectors.SecurityTokenRequirement
    {
        protected ServiceModelSecurityTokenRequirement() { }
        static public string ChannelParametersCollectionProperty { get { return default; } }
    }
    public partial class SupportingTokenParameters
    {
        public SupportingTokenParameters() { }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> Endorsing { get { return default; } }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> SignedEncrypted { get { return default; } }
        public System.ServiceModel.Security.Tokens.SupportingTokenParameters Clone() { return default; }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> Signed { get { return default; } }
    }
    public partial class UserNameSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        public UserNameSecurityTokenParameters() { }
    }
}
namespace System.IdentityModel.Policy
{
    public partial interface IAuthorizationPolicy {}
}
namespace System.IdentityModel.Tokens
{
    public partial class GenericXmlSecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        public GenericXmlSecurityToken(System.Xml.XmlElement tokenXml,
            System.IdentityModel.Tokens.SecurityToken proofToken,
            DateTime effectiveTime,
            DateTime expirationTime,
            SecurityKeyIdentifierClause internalTokenReference,
            SecurityKeyIdentifierClause externalTokenReference,
            System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> authorizationPolicies) {}
        public override string Id { get; }
        public override DateTime ValidFrom { get; }
        public override DateTime ValidTo { get; }
        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys { get; }
    }
    public enum SecurityKeyType
    {
        SymmetricKey,
        AsymmetricKey,
        BearerKey
    }
}
