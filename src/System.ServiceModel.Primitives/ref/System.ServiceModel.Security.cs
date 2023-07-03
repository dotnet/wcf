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
        public static System.ServiceModel.MessageSecurityVersion WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10 { get { return default; } }
    }
    public class ServiceSecurityContext
    {
        public ServiceSecurityContext(System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> authorizationPolicies) { }
        public ServiceSecurityContext(System.IdentityModel.Policy.AuthorizationContext authorizationContext) { }
        public ServiceSecurityContext(System.IdentityModel.Policy.AuthorizationContext authorizationContext, System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> authorizationPolicies) { }
        public static System.ServiceModel.ServiceSecurityContext Anonymous => default;
        public bool IsAnonymous => default;
        public System.Security.Principal.IIdentity PrimaryIdentity => default;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> AuthorizationPolicies { get; set; }
        public System.IdentityModel.Policy.AuthorizationContext AuthorizationContext => default;
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
    public partial class X509CertificateEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public X509CertificateEndpointIdentity(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate2Collection Certificates { get; }
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
        public bool DetectReplays { get { return default; } set { } }
        public System.TimeSpan MaxClockSkew { get { return default; } set { } }
        public bool ReconnectTransportOnFailure { get { return default; } set { } }
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
        public static TransportSecurityBindingElement CreateCertificateOverTransportBindingElement() { return default; }
        public static TransportSecurityBindingElement CreateCertificateOverTransportBindingElement(MessageSecurityVersion version) { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override string ToString() { return default; }
        public System.ServiceModel.Security.SecurityKeyEntropyMode KeyEntropyMode { get { return default;} set { } }
        public bool EnableUnsecuredResponse { get { return default; } set { } }
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
    public abstract class IdentityVerifier
    {
        protected IdentityVerifier() { }
        public static IdentityVerifier CreateDefault() => default;
        public abstract bool CheckAccess(System.ServiceModel.EndpointIdentity identity, System.IdentityModel.Policy.AuthorizationContext authContext);
        public abstract bool TryGetIdentity(System.ServiceModel.EndpointAddress reference, out System.ServiceModel.EndpointIdentity identity);
    }
    public partial class SecurityMessageProperty : System.ServiceModel.Channels.IMessageProperty, IDisposable
    {
        public SecurityMessageProperty() { }
        public ServiceSecurityContext ServiceSecurityContext { get; set; }
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> ExternalAuthorizationPolicies { get; set; }
        public SecurityTokenSpecification ProtectionToken { get; set; }
        public SecurityTokenSpecification InitiatorToken { get; set; }
        public SecurityTokenSpecification RecipientToken { get; set; }
        public SecurityTokenSpecification TransportToken { get; set; }
        public string SenderIdPrefix { get; set; }
        public bool HasIncomingSupportingTokens => default;
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.SupportingTokenSpecification> IncomingSupportingTokens => default;
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.SupportingTokenSpecification> OutgoingSupportingTokens => default;
        internal bool HasOutgoingSupportingTokens => default;
        public System.ServiceModel.Channels.IMessageProperty CreateCopy() => default;
        public static SecurityMessageProperty GetOrCreate(System.ServiceModel.Channels.Message message) => default;
        public void Dispose() { }
    }
    public abstract partial class SecureConversationVersion
    {
        internal SecureConversationVersion() { }
        public static System.ServiceModel.Security.SecureConversationVersion Default { get { return default; } }
        public System.Xml.XmlDictionaryString Namespace { get { return default; } }
        public System.Xml.XmlDictionaryString Prefix { get { return default; } }
        public static System.ServiceModel.Security.SecureConversationVersion WSSecureConversationFeb2005 { get { return default; } }
        public static System.ServiceModel.Security.SecureConversationVersion WSSecureConversation13 { get { return default; } }
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
        public abstract bool IsSymmetricKeyLengthSupported(int length);
        public abstract bool IsAsymmetricKeyLengthSupported(int length);
        public static SecurityAlgorithmSuite Default { get; }
        public static SecurityAlgorithmSuite Basic256 { get; }
        public static SecurityAlgorithmSuite Basic256Sha256 { get; }
    }
    public enum SecurityTokenAttachmentMode
    {
        Signed,
        Endorsing,
        SignedEndorsing,
        SignedEncrypted
    }
    public partial class SecurityTokenSpecification
    {
        public SecurityTokenSpecification(System.IdentityModel.Tokens.SecurityToken token, System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> tokenPolicies) { }
        public System.IdentityModel.Tokens.SecurityToken SecurityToken => default;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> SecurityTokenPolicies => default;
    }
    public abstract partial class SecurityPolicyVersion
    {
        internal SecurityPolicyVersion() { }
        public string Namespace { get { return default; } }
        public string Prefix { get { return default; } }
        public static System.ServiceModel.Security.SecurityPolicyVersion WSSecurityPolicy11 { get { return default; } }
        public static System.ServiceModel.Security.SecurityPolicyVersion WSSecurityPolicy12 { get { return default; } }
    }
    public abstract partial class SecurityVersion
    {
        internal SecurityVersion() { }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity10 { get { return default; } }
        public static System.ServiceModel.Security.SecurityVersion WSSecurity11 { get { return default; } }
    }
    public partial class SupportingTokenSpecification : System.ServiceModel.Security.SecurityTokenSpecification
    {
        public SupportingTokenSpecification(System.IdentityModel.Tokens.SecurityToken token, System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> tokenPolicies, System.ServiceModel.Security.SecurityTokenAttachmentMode attachmentMode) : base(default, default) { }
        public SupportingTokenSpecification(System.IdentityModel.Tokens.SecurityToken token, System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> tokenPolicies, System.ServiceModel.Security.SecurityTokenAttachmentMode attachmentMode, System.ServiceModel.Security.Tokens.SecurityTokenParameters tokenParameters) : base(default, default) { }
        public System.ServiceModel.Security.SecurityTokenAttachmentMode SecurityTokenAttachmentMode => default;
    }
    public abstract partial class TrustVersion
    {
        internal TrustVersion() { }
        public static System.ServiceModel.Security.TrustVersion Default { get { return default; } }
        public System.Xml.XmlDictionaryString Namespace { get { return default; } }
        public System.Xml.XmlDictionaryString Prefix { get { return default; } }
        public static System.ServiceModel.Security.TrustVersion WSTrustFeb2005 { get { return default; } }
        public static System.ServiceModel.Security.TrustVersion WSTrust13 { get { return default; } }
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
    public sealed class InitiatorServiceModelSecurityTokenRequirement : System.ServiceModel.Security.Tokens.ServiceModelSecurityTokenRequirement
    {
        public InitiatorServiceModelSecurityTokenRequirement() { }
        public EndpointAddress TargetAddress { get { return default; } set { } }
        public Uri Via { get { return default; } set { } }
        public override string ToString() => default;
    }
    public class IssuedSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        protected IssuedSecurityTokenParameters(IssuedSecurityTokenParameters other) { }
        public IssuedSecurityTokenParameters() { }
        protected override SecurityTokenParameters CloneCore() { return default; }
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
        protected override SecurityTokenParameters CloneCore() { return default; }
        public bool RequireCancellation { get { return default; } set { } }
    }
    public abstract partial class SecurityTokenParameters
    {
        internal SecurityTokenParameters() { }
        protected abstract SecurityTokenParameters CloneCore();
        public bool RequireDerivedKeys { get { return default; } set { } }
        public System.ServiceModel.Security.Tokens.SecurityTokenParameters Clone() { return default; }
    }
    public abstract partial class ServiceModelSecurityTokenRequirement : System.IdentityModel.Selectors.SecurityTokenRequirement
    {
        protected ServiceModelSecurityTokenRequirement() { }
        public bool IsInitiator { get { return default; } }
        public SecurityAlgorithmSuite SecurityAlgorithmSuite { get { return default; } set { } }
        public System.ServiceModel.Channels.SecurityBindingElement SecurityBindingElement { get { return default; } set { } }
        public System.ServiceModel.EndpointAddress IssuerAddress { get { return default; } set { } }
        public System.ServiceModel.Channels.Binding IssuerBinding { get { return default; } set { } }
        public System.ServiceModel.Channels.SecurityBindingElement SecureConversationSecurityBindingElement { get { return default; } set { } }
        public System.IdentityModel.Selectors.SecurityTokenVersion MessageSecurityVersion { get { return default; } set { } }
        public string TransportScheme { get { return default; } set { } }
        public static string ChannelParametersCollectionProperty => default;
    }
    public class SspiSecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        public SspiSecurityToken(System.Security.Principal.TokenImpersonationLevel impersonationLevel, bool allowNtlm, System.Net.NetworkCredential networkCredential) { }
        public SspiSecurityToken(System.Net.NetworkCredential networkCredential, bool extractGroupsForWindowsAccounts, bool allowUnauthenticatedCallers) { }
        public override string Id => default;
        public override System.DateTime ValidFrom => default;
        public override System.DateTime ValidTo => default;
        public bool AllowUnauthenticatedCallers => default;
        public System.Security.Principal.TokenImpersonationLevel ImpersonationLevel => default;
        public bool AllowNtlm => default;
        public System.Net.NetworkCredential NetworkCredential => default;
        public bool ExtractGroupsForWindowsAccounts => default;
        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys => default;
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
        protected override SecurityTokenParameters CloneCore() { return default; }

    }
}
namespace System.IdentityModel.Claims
{
    public abstract partial class ClaimSet : System.Collections.Generic.IEnumerable<System.IdentityModel.Claims.Claim>
    {
        public static ClaimSet System  => default;
        public virtual bool ContainsClaim(System.IdentityModel.Claims.Claim claim, System.Collections.Generic.IEqualityComparer<System.IdentityModel.Claims.Claim> comparer) => default;
        public virtual bool ContainsClaim(System.IdentityModel.Claims.Claim claim) => default;
        public abstract System.IdentityModel.Claims.Claim this[int index] { get; }
        public abstract int Count { get; }
        public abstract System.IdentityModel.Claims.ClaimSet Issuer { get; }
        public abstract System.Collections.Generic.IEnumerable<System.IdentityModel.Claims.Claim> FindClaims(string claimType, string right);
        public abstract System.Collections.Generic.IEnumerator<System.IdentityModel.Claims.Claim> GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => default;
    }
    public partial class Claim
    {
        public Claim(string claimType, object resource, string right) { }
        public static System.Collections.Generic.IEqualityComparer<System.IdentityModel.Claims.Claim> DefaultComparer => default;
        public static System.IdentityModel.Claims.Claim System => default;
        public object Resource => default;
        public string ClaimType => default;
        public string Right => default;
        public static System.IdentityModel.Claims.Claim CreateDnsClaim(string dns) => default;
        public static System.IdentityModel.Claims.Claim CreateHashClaim(byte[] hash) => default;
        public static System.IdentityModel.Claims.Claim CreateNameClaim(string name) => default;
        public static System.IdentityModel.Claims.Claim CreateRsaClaim(System.Security.Cryptography.RSA rsa) => default;
        public static System.IdentityModel.Claims.Claim CreateSpnClaim(string spn) => default;
        public static System.IdentityModel.Claims.Claim CreateThumbprintClaim(byte[] thumbprint) => default;
        public static System.IdentityModel.Claims.Claim CreateUpnClaim(string upn) => default;
        public static System.IdentityModel.Claims.Claim CreateUriClaim(System.Uri uri) => default;
        public static System.IdentityModel.Claims.Claim CreateWindowsSidClaim(System.Security.Principal.SecurityIdentifier sid) => default;
        public static System.IdentityModel.Claims.Claim CreateX500DistinguishedNameClaim(System.Security.Cryptography.X509Certificates.X500DistinguishedName x500DistinguishedName) => default;
        public override bool Equals(object obj) => default;
        public override int GetHashCode() => default;
        public override string ToString() => default;
    }
    public static class ClaimTypes
    {
        public static string Anonymous => default;
        public static string DenyOnlySid => default;
        public static string Dns => default;
        public static string Email => default;
        public static string Hash => default;
        public static string Name => default;
        public static string Rsa => default;
        public static string Sid => default;
        public static string Spn => default;
        public static string System => default;
        public static string Thumbprint => default;
        public static string Upn => default;
        public static string Uri => default;
        public static string X500DistinguishedName => default;
        public static string NameIdentifier => default;
        public static string Authentication => default;
        public static string AuthorizationDecision => default;
    }
    public class DefaultClaimSet : ClaimSet
    {
        public DefaultClaimSet(params Claim[] claims) { }
        public DefaultClaimSet(System.Collections.Generic.IList<Claim> claims) { }
        public DefaultClaimSet(System.IdentityModel.Claims.ClaimSet issuer, params System.IdentityModel.Claims.Claim[] claims) { }
        public DefaultClaimSet(System.IdentityModel.Claims.ClaimSet issuer, System.Collections.Generic.IList<System.IdentityModel.Claims.Claim> claims) { }
        public override System.IdentityModel.Claims.Claim this[int index] => default;
        public override int Count => default;
        public override System.IdentityModel.Claims.ClaimSet Issuer => default;
        public override bool ContainsClaim(System.IdentityModel.Claims.Claim claim) => default;
        public override System.Collections.Generic.IEnumerable<System.IdentityModel.Claims.Claim> FindClaims(string claimType, string right) => default;
        public override System.Collections.Generic.IEnumerator<System.IdentityModel.Claims.Claim> GetEnumerator() => default;
        protected void Initialize(ClaimSet issuer, System.Collections.Generic.IList<Claim> claims) { }
        public override string ToString() => default;
    }
    public class X509CertificateClaimSet : System.IdentityModel.Claims.ClaimSet, System.IDisposable
    {
        public X509CertificateClaimSet(Security.Cryptography.X509Certificates.X509Certificate2 certificate) { }
        public override System.IdentityModel.Claims.Claim this[int index] => default;
        public override int Count => default;
        public DateTime ExpirationTime => default;
        public override System.IdentityModel.Claims.ClaimSet Issuer => default;
        public Security.Cryptography.X509Certificates.X509Certificate2 X509Certificate => default;
        public void Dispose() { }
        public override System.Collections.Generic.IEnumerable<System.IdentityModel.Claims.Claim> FindClaims(string claimType, string right) => default;
        public override System.Collections.Generic.IEnumerator<System.IdentityModel.Claims.Claim> GetEnumerator() => default;
        public override string ToString() => default;
    }
}
namespace System.IdentityModel.Policy
{
    public partial interface IAuthorizationPolicy
    {
        System.IdentityModel.Claims.ClaimSet Issuer { get; }
        bool Evaluate(EvaluationContext evaluationContext, ref object state);
    }
    public partial interface IAuthorizationComponent
    {
        string Id { get; }
    }
    public abstract partial class AuthorizationContext : IAuthorizationComponent
    {
        public abstract string Id { get; }
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Claims.ClaimSet> ClaimSets { get; }
        public abstract System.DateTime ExpirationTime { get; }
        public abstract System.Collections.Generic.IDictionary<string, object> Properties { get; }
        public static AuthorizationContext CreateDefaultAuthorizationContext(System.Collections.Generic.IList<System.IdentityModel.Policy.IAuthorizationPolicy> authorizationPolicies) => default;
    }
    public abstract class EvaluationContext
    {
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Claims.ClaimSet> ClaimSets { get; }
        public abstract System.Collections.Generic.IDictionary<string, object> Properties { get; }
        public abstract int Generation { get; }
        public abstract void AddClaimSet(IAuthorizationPolicy policy, System.IdentityModel.Claims.ClaimSet claimSet);
        public abstract void RecordExpirationTime(DateTime expirationTime);
    }
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
        public override string Id => default;
        public override DateTime ValidFrom => default;
        public override DateTime ValidTo => default;
        public SecurityKeyIdentifierClause InternalTokenReference => default;
        public SecurityKeyIdentifierClause ExternalTokenReference => default;
        public System.Xml.XmlElement TokenXml => default;
        public SecurityToken ProofToken => default;
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> AuthorizationPolicies => default;
        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys => default;
        public override bool CanCreateKeyIdentifierClause<T>() => default;
        public override T CreateKeyIdentifierClause<T>() => default;
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause) => default;
        public override string ToString() => default;
    }
    public partial class GenericXmlSecurityKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        public GenericXmlSecurityKeyIdentifierClause(System.Xml.XmlElement referenceXml) : this(referenceXml, null, 0) { }
        public GenericXmlSecurityKeyIdentifierClause(System.Xml.XmlElement referenceXml, byte[] derivationNonce, int derivationLength) : base(null, derivationNonce, derivationLength) { }
        public System.Xml.XmlElement ReferenceXml => default;
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause) => default;
    }
    public enum SecurityKeyType
    {
        SymmetricKey,
        AsymmetricKey,
        BearerKey
    }
    public enum SecurityKeyUsage
    {
        Exchange,
        Signature
    }
    [Serializable]
    public class SecurityTokenException : System.Exception
    {
        public SecurityTokenException() { }
        public SecurityTokenException(string message) { }
        public SecurityTokenException(string message, System.Exception innerException) { }
        protected SecurityTokenException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    [Serializable]
    public class SecurityTokenValidationException : SecurityTokenException
    {
        public SecurityTokenValidationException() { }
        public SecurityTokenValidationException(string message) { }
        public SecurityTokenValidationException(string message, System.Exception innerException)  { }
        protected SecurityTokenValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial class UserNameSecurityToken : SecurityToken
    {
        public UserNameSecurityToken(string userName, string password) { }
        public UserNameSecurityToken(string userName, string password, string id) { }
        public override string Id => default;
        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys => default;
        public override DateTime ValidFrom => default;
        public override DateTime ValidTo => default;
        public string UserName => default;
        public string Password => default;
    }
    public partial class X509SecurityToken : SecurityToken, IDisposable
    {
        public X509SecurityToken(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate) { }
        public X509SecurityToken(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, string id) { }
        public override string Id => default;
        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys => default;
        public override DateTime ValidFrom => default;
        public override DateTime ValidTo => default;
        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate => default;
        public override bool CanCreateKeyIdentifierClause<T>() => default;
        public override T CreateKeyIdentifierClause<T>() => default;
        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause) => default;
        public virtual void Dispose() { }
        protected void ThrowIfDisposed() { }
    }
}
