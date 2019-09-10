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
        public System.ServiceModel.Security.SecureConversationVersion SecureConversationVersion { get { return default(System.ServiceModel.Security.SecureConversationVersion); } }
        public abstract System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion { get; }
        public System.ServiceModel.Security.SecurityVersion SecurityVersion { get { return default(System.ServiceModel.Security.SecurityVersion); } }
        public System.ServiceModel.Security.TrustVersion TrustVersion { get { return default(System.ServiceModel.Security.TrustVersion); } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 { get { return default(System.ServiceModel.MessageSecurityVersion); } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11 { get { return default(System.ServiceModel.MessageSecurityVersion); } }
        public static System.ServiceModel.MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 { get { return default(System.ServiceModel.MessageSecurityVersion); } }
    }
    public partial class SpnEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public SpnEndpointIdentity(string spnName) { }
        public static System.TimeSpan SpnLookupTime { get { return default(System.TimeSpan); } set { } }
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
        public System.TimeSpan MaxClockSkew { get { return default(System.TimeSpan); } set { } }
        public System.TimeSpan ReplayWindow { get { return default(System.TimeSpan); } set { } }
        public System.TimeSpan TimestampValidityDuration { get { return default(System.TimeSpan); } set { } }
        public System.ServiceModel.Channels.LocalClientSecuritySettings Clone() { return default(System.ServiceModel.Channels.LocalClientSecuritySettings); }
    }
    public abstract partial class SecurityBindingElement : System.ServiceModel.Channels.BindingElement
    {
        internal SecurityBindingElement() { }
        public System.ServiceModel.Security.Tokens.SupportingTokenParameters EndpointSupportingTokenParameters { get { return default(System.ServiceModel.Security.Tokens.SupportingTokenParameters); } }
        public bool IncludeTimestamp { get { return default(bool); } set { } }
        public System.ServiceModel.Security.SecurityAlgorithmSuite DefaultAlgorithmSuite { get { return default(System.ServiceModel.Security.SecurityAlgorithmSuite); } set { } }
        public System.ServiceModel.Channels.LocalClientSecuritySettings LocalClientSettings { get { return default(System.ServiceModel.Channels.LocalClientSecuritySettings); } }
        public System.ServiceModel.MessageSecurityVersion MessageSecurityVersion { get { return default(System.ServiceModel.MessageSecurityVersion); } set { } }
        public System.ServiceModel.Channels.SecurityHeaderLayout SecurityHeaderLayout { get { return default(System.ServiceModel.Channels.SecurityHeaderLayout); } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        protected abstract System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(System.ServiceModel.Channels.BindingContext context);
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(bool); }
        public static System.ServiceModel.Channels.SecurityBindingElement CreateSecureConversationBindingElement(System.ServiceModel.Channels.SecurityBindingElement bootstrapSecurity) { return default(System.ServiceModel.Channels.SecurityBindingElement); }
        public static System.ServiceModel.Channels.TransportSecurityBindingElement CreateUserNameOverTransportBindingElement() { return default(System.ServiceModel.Channels.TransportSecurityBindingElement); }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default(T); }
        public override string ToString() { return default(string); }
    }
    public enum SecurityHeaderLayout
    {
        Lax = 1,
        Strict = 0,
    }
    public sealed partial class TransportSecurityBindingElement : System.ServiceModel.Channels.SecurityBindingElement
    {
        public TransportSecurityBindingElement() { }
        protected override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default(System.ServiceModel.Channels.BindingElement); }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default(T); }
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
        public static System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfile10 { get { return default(System.ServiceModel.Security.BasicSecurityProfileVersion); } }
    }
    public abstract partial class SecureConversationVersion
    {
        internal SecureConversationVersion() { }
        public static System.ServiceModel.Security.SecureConversationVersion Default { get { return default(System.ServiceModel.Security.SecureConversationVersion); } }
        public System.Xml.XmlDictionaryString Namespace { get { return default(System.Xml.XmlDictionaryString); } }
        public System.Xml.XmlDictionaryString Prefix { get { return default(System.Xml.XmlDictionaryString); } }
        public static System.ServiceModel.Security.SecureConversationVersion WSSecureConversationFeb2005 { get { return default(System.ServiceModel.Security.SecureConversationVersion); } }
    }
    public abstract partial class SecurityAlgorithmSuite
    {
        static public SecurityAlgorithmSuite TripleDes { get { return default(SecurityAlgorithmSuite); } }
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
        public virtual bool IsCanonicalizationAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsDigestAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsEncryptionAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsEncryptionKeyDerivationAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsSymmetricKeyWrapAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsAsymmetricKeyWrapAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsSymmetricSignatureAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsAsymmetricSignatureAlgorithmSupported(string algorithm) { return default(bool); }
        public virtual bool IsSignatureKeyDerivationAlgorithmSupported(string algorithm) { return default(bool); }
        public abstract bool IsSymmetricKeyLengthSupported(int length);
        public abstract bool IsAsymmetricKeyLengthSupported(int length);
    }
    public abstract partial class SecurityPolicyVersion
    {
        internal SecurityPolicyVersion() { }
        public string Namespace { get { return default(string); } }
        public string Prefix { get { return default(string); } }
        public static System.ServiceModel.Security.SecurityPolicyVersion WSSecurityPolicy11 { get { return default(System.ServiceModel.Security.SecurityPolicyVersion); } }
    }
    public abstract partial class SecurityVersion
    {
        internal SecurityVersion() { }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity10 { get { return default(System.ServiceModel.Security.SecurityVersion); } }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity11 { get { return default(System.ServiceModel.Security.SecurityVersion); } }
    }
    public abstract partial class TrustVersion
    {
        internal TrustVersion() { }
        public static System.ServiceModel.Security.TrustVersion Default { get { return default(System.ServiceModel.Security.TrustVersion); } }
        public System.Xml.XmlDictionaryString Namespace { get { return default(System.Xml.XmlDictionaryString); } }
        public System.Xml.XmlDictionaryString Prefix { get { return default(System.Xml.XmlDictionaryString); } }
        public static System.ServiceModel.Security.TrustVersion WSTrustFeb2005 { get { return default(System.ServiceModel.Security.TrustVersion); } }
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
        public override string Id { get { return default(string); } }
        public override DateTime ValidFrom { get { return default(DateTime); } }
        public override DateTime ValidTo { get { return default(DateTime); } }
        public int KeySize { get { return default(int); } }
        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys { get { return default(System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey>); } }
        public byte[] GetKeyBytes() { return default(byte[]); }
    }
    public partial class SecureConversationSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        public SecureConversationSecurityTokenParameters() { }
        public SecureConversationSecurityTokenParameters(System.ServiceModel.Channels.SecurityBindingElement bootstrapSecurityBindingElement) { }
        public System.ServiceModel.Channels.SecurityBindingElement BootstrapSecurityBindingElement { get { return default(System.ServiceModel.Channels.SecurityBindingElement); } set { } }
        public bool RequireCancellation { get { return default(bool); } set { } }
    }
    public abstract partial class SecurityTokenParameters
    {
        internal SecurityTokenParameters() { }
        public bool RequireDerivedKeys { get { return default(bool); } set { } }
        public System.ServiceModel.Security.Tokens.SecurityTokenParameters Clone() { return default(System.ServiceModel.Security.Tokens.SecurityTokenParameters); }
    }
    public abstract partial class ServiceModelSecurityTokenRequirement : System.IdentityModel.Selectors.SecurityTokenRequirement
    {
        protected ServiceModelSecurityTokenRequirement() { }
        //static public string SecurityAlgorithmSuiteProperty { get { return default(string); } }
        //static public string SecurityBindingElementProperty { get { return default(string); } }
        //static public string IssuerAddressProperty { get { return default(string); } }
        //static public string IssuerBindingProperty { get { return default(string); } }
        //static public string SecureConversationSecurityBindingElementProperty { get { return default(string); } }
        //static public string SupportSecurityContextCancellationProperty { get { return default(string); } }
        //static public string MessageSecurityVersionProperty { get { return default(string); } }
        //static public string IssuerBindingContextProperty { get { return default(string); } }
        //static public string TransportSchemeProperty { get { return default(string); } }
        //static public string IsInitiatorProperty { get { return default(string); } }
        //static public string TargetAddressProperty { get { return default(string); } }
        //static public string ViaProperty { get { return default(string); } }
        //static public string ListenUriProperty { get { return default(string); } }
        //static public string AuditLogLocationProperty { get { return default(string); } }
        //static public string SuppressAuditFailureProperty { get { return default(string); } }
        //static public string MessageAuthenticationAuditLevelProperty { get { return default(string); } }
        //static public string IsOutOfBandTokenProperty { get { return default(string); } }
        //static public string PreferSslCertificateAuthenticatorProperty { get { return default(string); } }
        //static public string SupportingTokenAttachmentModeProperty { get { return default(string); } }
        //static public string MessageDirectionProperty { get { return default(string); } }
        //static public string HttpAuthenticationSchemeProperty { get { return default(string); } }
        //static public string IssuedSecurityTokenParametersProperty { get { return default(string); } }
        //static public string PrivacyNoticeUriProperty { get { return default(string); } }
        //static public string PrivacyNoticeVersionProperty { get { return default(string); } }
        //static public string DuplexClientLocalAddressProperty { get { return default(string); } }
        //static public string EndpointFilterTableProperty { get { return default(string); } }
        static public string ChannelParametersCollectionProperty { get { return default(string); } }
        //static public string ExtendedProtectionPolicy { get { return default(string); } }
        //public bool IsInitiator { get { return default(bool); } }
        //public System.ServiceModel.Security.SecurityAlgorithmSuite SecurityAlgorithmSuite { get { return default(System.ServiceModel.Security.SecurityAlgorithmSuite); } set { } }
        //public System.ServiceModel.Channels.SecurityBindingElement SecurityBindingElement { get { return default(System.ServiceModel.Channels.SecurityBindingElement); } set { } }
        //public System.ServiceModel.EndpointAddress IssuerAddress { get { return default(System.ServiceModel.EndpointAddress); } set { } }
        //public System.ServiceModel.Channels.Binding IssuerBinding { get { return default(System.ServiceModel.Channels.Binding); } set { } }
        //public System.ServiceModel.Channels.SecurityBindingElement SecureConversationSecurityBindingElement { get { return default(System.ServiceModel.Channels.SecurityBindingElement); } set { } }
        //public System.IdentityModel.Selectors.SecurityTokenVersion MessageSecurityVersion { get { return default(System.IdentityModel.Selectors.SecurityTokenVersion); } set { } }
        //public string TransportScheme { get { return default(string); } set { } }
    }
    public partial class SupportingTokenParameters
    {
        public SupportingTokenParameters() { }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> Endorsing { get { return default(System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters>); } }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> SignedEncrypted { get { return default(System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters>); } }
        public System.ServiceModel.Security.Tokens.SupportingTokenParameters Clone() { return default(System.ServiceModel.Security.Tokens.SupportingTokenParameters); }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> Signed { get; }
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
        // public SecurityKeyIdentifierClause InternalTokenReference { get; }
        // public SecurityKeyIdentifierClause ExternalTokenReference { get; }
        // public XmlElement TokenXml { get; }
        // public SecurityToken ProofToken { get; }
        // public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies { get; }
        // public override string ToString() {}
        // public override bool CanCreateKeyIdentifierClause<T>() {}
        // public override T CreateKeyIdentifierClause<T>() {}
        // public override bool MatchesKeyIdentifierClause(.SecurityKeyIdentifierClause keyIdentifierClause) {}
    }
}
