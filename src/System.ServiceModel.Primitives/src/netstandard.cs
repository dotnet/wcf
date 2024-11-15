// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Collections.Generic
{
    public partial class SynchronizedCollection<T> : IList<T>, IList
    {
        public SynchronizedCollection() { }
        public SynchronizedCollection(object syncRoot) { }
        public SynchronizedCollection(object syncRoot, IEnumerable<T> list) { }
        public SynchronizedCollection(object syncRoot, params T[] list) { }
        public int Count { get { return default; } }
        protected List<T> Items { get { return default; } }
        public object SyncRoot { get { return default; } }
        public T this[int index] { get { return default; } set { } }
        public void Add(T item) { }
        public void Clear() { }
        public void CopyTo(T[] array, int index) { }
        public bool Contains(T item) { return default; }
        public IEnumerator<T> GetEnumerator() { return default; }
        public int IndexOf(T item) { return default; }
        public void Insert(int index, T item) { }
        public bool Remove(T item) { return default; }
        public void RemoveAt(int index) { }
        protected virtual void ClearItems() { }
        protected virtual void InsertItem(int index, T item) { }
        protected virtual void RemoveItem(int index) { }
        protected virtual void SetItem(int index, T item) { }
        bool ICollection<T>.IsReadOnly { get { return default; } }
        IEnumerator IEnumerable.GetEnumerator() { return default; }
        bool ICollection.IsSynchronized { get { return default; } }
        object ICollection.SyncRoot { get { return default; } }
        void ICollection.CopyTo(Array array, int index) { }
        object IList.this[int index] { get { return default; } set { } }
        bool IList.IsReadOnly { get { return default; } }
        bool IList.IsFixedSize { get { return default; } }
        int IList.Add(object value) { return default; }
        bool IList.Contains(object value) { return default; }
        int IList.IndexOf(object value) { return default; }
        void IList.Insert(int index, object value) { }
        void IList.Remove(object value) { }
    }
    public partial class KeyedByTypeCollection<TItem> : System.Collections.ObjectModel.KeyedCollection<Type, TItem>
    {
        public KeyedByTypeCollection() { }
        public KeyedByTypeCollection(IEnumerable<TItem> items) { }
        public T Find<T>() { return default; }
        public T Remove<T>() { return default; }
        public System.Collections.ObjectModel.Collection<T> FindAll<T>() { return default; }
        public System.Collections.ObjectModel.Collection<T> RemoveAll<T>() { return default; }
        protected override Type GetKeyForItem(TItem item) { return default; }
        protected override void InsertItem(int index, TItem item) { }
        protected override void SetItem(int index, TItem item) { }
    }
}
namespace System.IdentityModel.Claims
{
    public abstract partial class ClaimSet : System.Collections.Generic.IEnumerable<System.IdentityModel.Claims.Claim>
    {
        public static ClaimSet System => default;
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
namespace System.IdentityModel.Selectors
{
    public abstract partial class SecurityTokenAuthenticator
    {
        protected SecurityTokenAuthenticator() { }
        public bool CanValidateToken(System.IdentityModel.Tokens.SecurityToken token) => default;
        public Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> ValidateToken(System.IdentityModel.Tokens.SecurityToken token) => default;
        protected abstract bool CanValidateTokenCore(System.IdentityModel.Tokens.SecurityToken token);
        protected abstract Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> ValidateTokenCore(System.IdentityModel.Tokens.SecurityToken token);
    }
    public abstract partial class SecurityTokenManager
    {
        public abstract System.IdentityModel.Selectors.SecurityTokenProvider CreateSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement);
        public abstract System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version);
        public abstract SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver);
    }
    public abstract partial class SecurityTokenProvider
    {
        protected SecurityTokenProvider() { }
        public virtual bool SupportsTokenRenewal { get { return default; } }
        public virtual bool SupportsTokenCancellation { get { return default; } }
        public System.IdentityModel.Tokens.SecurityToken GetToken(System.TimeSpan timeout) { return default; }
        public System.IAsyncResult BeginGetToken(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        public System.IdentityModel.Tokens.SecurityToken EndGetToken(System.IAsyncResult result) { return default; }
        public System.IdentityModel.Tokens.SecurityToken RenewToken(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed) { return default; }
        public System.IAsyncResult BeginRenewToken(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed, System.AsyncCallback callback, object state) { return default; }
        public System.IdentityModel.Tokens.SecurityToken EndRenewToken(System.IAsyncResult result) { return default; }
        public void CancelToken(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token) { }
        public System.IAsyncResult BeginCancelToken(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token, System.AsyncCallback callback, object state) { return default; }
        public void EndCancelToken(System.IAsyncResult result) { }
        protected abstract System.IdentityModel.Tokens.SecurityToken GetTokenCore(System.TimeSpan timeout);
        protected virtual System.IdentityModel.Tokens.SecurityToken RenewTokenCore(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed) { return default; }
        protected virtual void CancelTokenCore(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token) { }
        protected virtual System.IAsyncResult BeginGetTokenCore(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        protected virtual System.IdentityModel.Tokens.SecurityToken EndGetTokenCore(System.IAsyncResult result) { return default; }
        protected virtual System.IAsyncResult BeginRenewTokenCore(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed, System.AsyncCallback callback, object state) { return default; }
        protected virtual System.IdentityModel.Tokens.SecurityToken EndRenewTokenCore(System.IAsyncResult result) { return default; }
        protected virtual System.IAsyncResult BeginCancelTokenCore(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token, System.AsyncCallback callback, object state) { return default; }
        protected virtual void EndCancelTokenCore(System.IAsyncResult result) { }
    }
    public partial class SecurityTokenRequirement
    {
        public SecurityTokenRequirement() { }
        public string TokenType { get { return default; } set { } }
        public bool RequireCryptographicToken { get { return default; } set { } }
        public System.IdentityModel.Tokens.SecurityKeyUsage KeyUsage { get { return default; } set { } }
        public System.IdentityModel.Tokens.SecurityKeyType KeyType { get { return default; } set { } }
        public int KeySize { get { return default; } set { } }
        public System.Collections.Generic.IDictionary<string, object> Properties { get { return default; } }
        public TValue GetProperty<TValue>(string propertyName) { return default; }
        public bool TryGetProperty<TValue>(string propertyName, out TValue result) { result = default; return default; }
    }
    public abstract partial class SecurityTokenResolver
    {
        internal SecurityTokenResolver() { }
    }
    public abstract partial class SecurityTokenSerializer
    {
        public bool CanReadToken(System.Xml.XmlReader reader) { return default; }
        public bool CanWriteToken(System.IdentityModel.Tokens.SecurityToken token) { return default; }
        public bool CanReadKeyIdentifier(System.Xml.XmlReader reader) { return default; }
        public bool CanWriteKeyIdentifier(System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier) { return default; }
        public bool CanReadKeyIdentifierClause(System.Xml.XmlReader reader) { return default; }
        public bool CanWriteKeyIdentifierClause(System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause) { return default; }
        public System.IdentityModel.Tokens.SecurityToken ReadToken(System.Xml.XmlReader reader, System.IdentityModel.Selectors.SecurityTokenResolver tokenResolver) { return default; }
        public void WriteToken(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token) { }
        public System.IdentityModel.Tokens.SecurityKeyIdentifier ReadKeyIdentifier(System.Xml.XmlReader reader) { return default; }
        public void WriteKeyIdentifier(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier) { }
        public System.IdentityModel.Tokens.SecurityKeyIdentifierClause ReadKeyIdentifierClause(System.Xml.XmlReader reader) { return default; }
        public void WriteKeyIdentifierClause(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause) { }
        protected abstract bool CanReadTokenCore(System.Xml.XmlReader reader);
        protected abstract bool CanWriteTokenCore(System.IdentityModel.Tokens.SecurityToken token);
        protected abstract bool CanReadKeyIdentifierCore(System.Xml.XmlReader reader);
        protected abstract bool CanWriteKeyIdentifierCore(System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier);
        protected abstract bool CanReadKeyIdentifierClauseCore(System.Xml.XmlReader reader);
        protected abstract bool CanWriteKeyIdentifierClauseCore(System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause);
        protected abstract System.IdentityModel.Tokens.SecurityToken ReadTokenCore(System.Xml.XmlReader reader, System.IdentityModel.Selectors.SecurityTokenResolver tokenResolver);
        protected abstract void WriteTokenCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token);
        protected abstract System.IdentityModel.Tokens.SecurityKeyIdentifier ReadKeyIdentifierCore(System.Xml.XmlReader reader);
        protected abstract void WriteKeyIdentifierCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifier keyIdentifier);
        protected abstract System.IdentityModel.Tokens.SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(System.Xml.XmlReader reader);
        protected abstract void WriteKeyIdentifierClauseCore(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause);
    }
    public abstract partial class SecurityTokenVersion
    {
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<string> GetSecuritySpecifications();
    }
    public abstract partial class X509CertificateValidator
    {
        protected X509CertificateValidator() { }
        public abstract void Validate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate);
        public static X509CertificateValidator CreateChainTrustValidator(bool useMachineContext, Security.Cryptography.X509Certificates.X509ChainPolicy chainPolicy) => default;
    }
    public partial class X509SecurityTokenAuthenticator : SecurityTokenAuthenticator
    {
        public X509SecurityTokenAuthenticator() { }
        public X509SecurityTokenAuthenticator(X509CertificateValidator validator) { }
        protected override bool CanValidateTokenCore(System.IdentityModel.Tokens.SecurityToken token) => default;
        protected override Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> ValidateTokenCore(System.IdentityModel.Tokens.SecurityToken token) => default;
    }
}
namespace System.IdentityModel.Tokens
{
    public abstract partial class SecurityKey
    {
        internal SecurityKey() { }
        public abstract int KeySize { get; }
    }
    public class SecurityKeyIdentifier : System.Collections.Generic.IEnumerable<System.IdentityModel.Tokens.SecurityKeyIdentifierClause>
    {
        public SecurityKeyIdentifier() { }
        public SecurityKeyIdentifier(params System.IdentityModel.Tokens.SecurityKeyIdentifierClause[] clauses) { }
        public System.IdentityModel.Tokens.SecurityKeyIdentifierClause this[int index] { get { return default; } }
        public bool CanCreateKey { get { return default; } }
        public int Count { get { return default; } }
        public bool IsReadOnly { get { return default; } }
        public void Add(System.IdentityModel.Tokens.SecurityKeyIdentifierClause clause) { }
        public System.IdentityModel.Tokens.SecurityKey CreateKey() { return default; }
        public TClause Find<TClause>() where TClause : System.IdentityModel.Tokens.SecurityKeyIdentifierClause { return default; }
        public System.Collections.Generic.IEnumerator<System.IdentityModel.Tokens.SecurityKeyIdentifierClause> GetEnumerator() { return default; }
        public void MakeReadOnly() { }
        public override string ToString() { return default; }
        public bool TryFind<TClause>(out TClause clause) where TClause : System.IdentityModel.Tokens.SecurityKeyIdentifierClause { clause = default; return default; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default; }
    }
    public abstract class SecurityKeyIdentifierClause
    {
        protected SecurityKeyIdentifierClause(string clauseType) { }
        protected SecurityKeyIdentifierClause(string clauseType, byte[] nonce, int length) { }
        public virtual bool CanCreateKey { get { return default; } }
        public string ClauseType { get { return default; } }
        public string Id { get { return default; } set { } }
        public virtual System.IdentityModel.Tokens.SecurityKey CreateKey() { return default; }
        public virtual bool Matches(System.IdentityModel.Tokens.SecurityKeyIdentifierClause keyIdentifierClause) { return default; }
        public byte[] GetDerivationNonce() { return default; }
        public int DerivationLength { get { return default; } }
    }
    public abstract partial class SecurityToken
    {
        public abstract string Id { get; }
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys { get; }
        public abstract DateTime ValidFrom { get; }
        public abstract DateTime ValidTo { get; }
        public virtual bool CanCreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause => default;
        public virtual T CreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause => default;
        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause) => default;
        public virtual SecurityKey ResolveKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause) => default;
    }
    public abstract partial class SymmetricSecurityKey : SecurityKey
    {
        public abstract System.Security.Cryptography.SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm);
        public abstract byte[] GetSymmetricKey();
    }
    public partial class GenericXmlSecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        public GenericXmlSecurityToken(System.Xml.XmlElement tokenXml,
            System.IdentityModel.Tokens.SecurityToken proofToken,
            DateTime effectiveTime,
            DateTime expirationTime,
            SecurityKeyIdentifierClause internalTokenReference,
            SecurityKeyIdentifierClause externalTokenReference,
            System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Policy.IAuthorizationPolicy> authorizationPolicies)
        { }
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
        public SecurityTokenValidationException(string message, System.Exception innerException) { }
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
namespace System.ServiceModel
{
    public partial class ActionNotSupportedException : System.ServiceModel.CommunicationException
    {
        public ActionNotSupportedException() { }
        public ActionNotSupportedException(string message) { }
        public ActionNotSupportedException(string message, System.Exception innerException) { }
        protected ActionNotSupportedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public enum CacheSetting
    {
        Default,
        AlwaysOn,
        AlwaysOff
    }
    public abstract partial class ChannelFactory : System.ServiceModel.Channels.CommunicationObject, System.IDisposable, System.ServiceModel.Channels.IChannelFactory, System.ServiceModel.ICommunicationObject
    {
        protected ChannelFactory() { }
        public System.ServiceModel.Description.ClientCredentials Credentials { get { return default; } }
        protected override System.TimeSpan DefaultCloseTimeout { get { return default; } }
        protected override System.TimeSpan DefaultOpenTimeout { get { return default; } }
        public System.ServiceModel.Description.ServiceEndpoint Endpoint { get { return default; } }
        protected abstract System.ServiceModel.Description.ServiceEndpoint CreateDescription();
        protected virtual System.ServiceModel.Channels.IChannelFactory CreateFactory() { return default; }
        protected internal void EnsureOpened() { }
        public T GetProperty<T>() where T : class { return default; }
        protected void InitializeEndpoint(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address) { }
        protected void InitializeEndpoint(System.ServiceModel.Description.ServiceEndpoint endpoint) { }
        protected override void OnAbort() { }
        protected override System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        protected override System.IAsyncResult OnBeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        protected override void OnClose(System.TimeSpan timeout) { }
        protected override void OnEndClose(System.IAsyncResult result) { }
        protected override void OnEndOpen(System.IAsyncResult result) { }
        protected override void OnOpen(System.TimeSpan timeout) { }
        protected override void OnOpened() { }
        protected override void OnOpening() { }
        void System.IDisposable.Dispose() { }
    }
    public partial class ChannelFactory<TChannel> : System.ServiceModel.ChannelFactory, System.ServiceModel.Channels.IChannelFactory, System.ServiceModel.Channels.IChannelFactory<TChannel>, System.ServiceModel.ICommunicationObject
    {
        public ChannelFactory(System.ServiceModel.Channels.Binding binding) { }
        public ChannelFactory(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) { }
        public ChannelFactory(System.ServiceModel.Description.ServiceEndpoint endpoint) { }
        protected ChannelFactory(System.Type channelType) { }
        public TChannel CreateChannel() { return default; }
        public TChannel CreateChannel(System.ServiceModel.EndpointAddress address) { return default; }
        public virtual TChannel CreateChannel(System.ServiceModel.EndpointAddress address, System.Uri via) { return default; }
        protected override System.ServiceModel.Description.ServiceEndpoint CreateDescription() { return default; }
    }
    public partial class ChannelTerminatedException : System.ServiceModel.CommunicationException
    {
        public ChannelTerminatedException() { }
        public ChannelTerminatedException(string message) { }
        public ChannelTerminatedException(string message, System.Exception innerException) { }
        protected ChannelTerminatedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public abstract partial class ClientBase<TChannel> : System.IDisposable, System.ServiceModel.ICommunicationObject where TChannel : class
    {
        protected ClientBase() { }
        protected ClientBase(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) { }
        protected ClientBase(System.ServiceModel.Description.ServiceEndpoint endpoint) { }
        protected TChannel Channel { get { return default; } }
        public static CacheSetting CacheSetting { get { return default; } set { } }
        public System.ServiceModel.ChannelFactory<TChannel> ChannelFactory { get { return default; } }
        public System.ServiceModel.Description.ClientCredentials ClientCredentials { get { return default; } }
        public System.ServiceModel.Description.ServiceEndpoint Endpoint { get { return default; } }
        public System.ServiceModel.IClientChannel InnerChannel { get { return default; } }
        public System.ServiceModel.CommunicationState State { get { return default; } }
        event System.EventHandler System.ServiceModel.ICommunicationObject.Closed { add { } remove { } }
        event System.EventHandler System.ServiceModel.ICommunicationObject.Closing { add { } remove { } }
        event System.EventHandler System.ServiceModel.ICommunicationObject.Faulted { add { } remove { } }
        event System.EventHandler System.ServiceModel.ICommunicationObject.Opened { add { } remove { } }
        event System.EventHandler System.ServiceModel.ICommunicationObject.Opening { add { } remove { } }
        public void Abort() { }
        public void Close() { }
        public void Open() { }
        protected virtual TChannel CreateChannel() { return default; }
        protected T GetDefaultValueForInitialization<T>() { return default; }
        protected void InvokeAsync(System.ServiceModel.ClientBase<TChannel>.BeginOperationDelegate beginOperationDelegate, object[] inValues, System.ServiceModel.ClientBase<TChannel>.EndOperationDelegate endOperationDelegate, System.Threading.SendOrPostCallback operationCompletedCallback, object userState) { }
        System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginClose(System.AsyncCallback callback, object state) { return default; }
        System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginOpen(System.AsyncCallback callback, object state) { return default; }
        System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        void System.ServiceModel.ICommunicationObject.Close() { }
        void System.ServiceModel.ICommunicationObject.Close(System.TimeSpan timeout) { }
        void System.ServiceModel.ICommunicationObject.EndClose(System.IAsyncResult result) { }
        void System.ServiceModel.ICommunicationObject.EndOpen(System.IAsyncResult result) { }
        void System.ServiceModel.ICommunicationObject.Open() { }
        void System.ServiceModel.ICommunicationObject.Open(System.TimeSpan timeout) { }
        void System.IDisposable.Dispose() { }
        protected delegate System.IAsyncResult BeginOperationDelegate(object[] inValues, System.AsyncCallback asyncCallback, object state);
        protected partial class ChannelBase<T> : System.IDisposable, System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IOutputChannel, System.ServiceModel.Channels.IRequestChannel, System.ServiceModel.IClientChannel, System.ServiceModel.ICommunicationObject, System.ServiceModel.IContextChannel, System.ServiceModel.IExtensibleObject<System.ServiceModel.IContextChannel> where T : class
        {
            protected ChannelBase(System.ServiceModel.ClientBase<T> client) { }
            System.ServiceModel.EndpointAddress System.ServiceModel.Channels.IOutputChannel.RemoteAddress { get { return default; } }
            System.Uri System.ServiceModel.Channels.IOutputChannel.Via { get { return default; } }
            System.ServiceModel.EndpointAddress System.ServiceModel.Channels.IRequestChannel.RemoteAddress { get { return default; } }
            System.Uri System.ServiceModel.Channels.IRequestChannel.Via { get { return default; } }
            bool System.ServiceModel.IClientChannel.AllowInitializationUI { get { return default; } set { } }
            bool System.ServiceModel.IClientChannel.DidInteractiveInitialization { get { return default; } }
            System.Uri System.ServiceModel.IClientChannel.Via { get { return default; } }
            System.ServiceModel.CommunicationState System.ServiceModel.ICommunicationObject.State { get { return default; } }
            bool System.ServiceModel.IContextChannel.AllowOutputBatching { get { return default; } set { } }
            System.ServiceModel.Channels.IInputSession System.ServiceModel.IContextChannel.InputSession { get { return default; } }
            System.ServiceModel.EndpointAddress System.ServiceModel.IContextChannel.LocalAddress { get { return default; } }
            System.TimeSpan System.ServiceModel.IContextChannel.OperationTimeout { get { return default; } set { } }
            System.ServiceModel.Channels.IOutputSession System.ServiceModel.IContextChannel.OutputSession { get { return default; } }
            System.ServiceModel.EndpointAddress System.ServiceModel.IContextChannel.RemoteAddress { get { return default; } }
            string System.ServiceModel.IContextChannel.SessionId { get { return default; } }
            System.ServiceModel.IExtensionCollection<System.ServiceModel.IContextChannel> System.ServiceModel.IExtensibleObject<System.ServiceModel.IContextChannel>.Extensions { get { return default; } }
            event System.EventHandler<System.ServiceModel.UnknownMessageReceivedEventArgs> System.ServiceModel.IClientChannel.UnknownMessageReceived { add { } remove { } }
            event System.EventHandler System.ServiceModel.ICommunicationObject.Closed { add { } remove { } }
            event System.EventHandler System.ServiceModel.ICommunicationObject.Closing { add { } remove { } }
            event System.EventHandler System.ServiceModel.ICommunicationObject.Faulted { add { } remove { } }
            event System.EventHandler System.ServiceModel.ICommunicationObject.Opened { add { } remove { } }
            event System.EventHandler System.ServiceModel.ICommunicationObject.Opening { add { } remove { } }
            protected System.IAsyncResult BeginInvoke(string methodName, object[] args, System.AsyncCallback callback, object state) { return default; }
            protected object EndInvoke(string methodName, object[] args, System.IAsyncResult result) { return default; }
            void System.IDisposable.Dispose() { }
            TProperty System.ServiceModel.Channels.IChannel.GetProperty<TProperty>() { return default; }
            System.IAsyncResult System.ServiceModel.Channels.IOutputChannel.BeginSend(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state) { return default; }
            System.IAsyncResult System.ServiceModel.Channels.IOutputChannel.BeginSend(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
            void System.ServiceModel.Channels.IOutputChannel.EndSend(System.IAsyncResult result) { }
            void System.ServiceModel.Channels.IOutputChannel.Send(System.ServiceModel.Channels.Message message) { }
            void System.ServiceModel.Channels.IOutputChannel.Send(System.ServiceModel.Channels.Message message, System.TimeSpan timeout) { }
            System.IAsyncResult System.ServiceModel.Channels.IRequestChannel.BeginRequest(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state) { return default; }
            System.IAsyncResult System.ServiceModel.Channels.IRequestChannel.BeginRequest(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
            System.ServiceModel.Channels.Message System.ServiceModel.Channels.IRequestChannel.EndRequest(System.IAsyncResult result) { return default; }
            System.ServiceModel.Channels.Message System.ServiceModel.Channels.IRequestChannel.Request(System.ServiceModel.Channels.Message message) { return default; }
            System.ServiceModel.Channels.Message System.ServiceModel.Channels.IRequestChannel.Request(System.ServiceModel.Channels.Message message, System.TimeSpan timeout) { return default; }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            System.IAsyncResult System.ServiceModel.IClientChannel.BeginDisplayInitializationUI(System.AsyncCallback callback, object state) { return default; }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            void System.ServiceModel.IClientChannel.DisplayInitializationUI() { }
            [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
            void System.ServiceModel.IClientChannel.EndDisplayInitializationUI(System.IAsyncResult result) { }
            void System.ServiceModel.ICommunicationObject.Abort() { }
            System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginClose(System.AsyncCallback callback, object state) { return default; }
            System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
            System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginOpen(System.AsyncCallback callback, object state) { return default; }
            System.IAsyncResult System.ServiceModel.ICommunicationObject.BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
            void System.ServiceModel.ICommunicationObject.Close() { }
            void System.ServiceModel.ICommunicationObject.Close(System.TimeSpan timeout) { }
            void System.ServiceModel.ICommunicationObject.EndClose(System.IAsyncResult result) { }
            void System.ServiceModel.ICommunicationObject.EndOpen(System.IAsyncResult result) { }
            void System.ServiceModel.ICommunicationObject.Open() { }
            void System.ServiceModel.ICommunicationObject.Open(System.TimeSpan timeout) { }
        }
        protected delegate object[] EndOperationDelegate(System.IAsyncResult result);
        protected partial class InvokeAsyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
        {
            internal InvokeAsyncCompletedEventArgs() : base(default, default, default) { }
            public object[] Results { get { return default; } }
        }
    }
    public partial class ClientCredentialsSecurityTokenManager : System.IdentityModel.Selectors.SecurityTokenManager
    {
        public ClientCredentialsSecurityTokenManager(System.ServiceModel.Description.ClientCredentials clientCredentials) { }
        public System.ServiceModel.Description.ClientCredentials ClientCredentials { get { return default; } }
        public override System.IdentityModel.Selectors.SecurityTokenProvider CreateSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement) { return default; }
        public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version) { return default; }
        public override System.IdentityModel.Selectors.SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement, out System.IdentityModel.Selectors.SecurityTokenResolver outOfBandTokenResolver) { outOfBandTokenResolver = default; return default; }
    }
    public partial class CommunicationException : System.Exception
    {
        public CommunicationException() { }
        public CommunicationException(string message) { }
        public CommunicationException(string message, System.Exception innerException) { }
        protected CommunicationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial class CommunicationObjectAbortedException : System.ServiceModel.CommunicationException
    {
        public CommunicationObjectAbortedException() { }
        public CommunicationObjectAbortedException(string message) { }
        public CommunicationObjectAbortedException(string message, System.Exception innerException) { }
        protected CommunicationObjectAbortedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial class CommunicationObjectFaultedException : System.ServiceModel.CommunicationException
    {
        public CommunicationObjectFaultedException() { }
        public CommunicationObjectFaultedException(string message) { }
        public CommunicationObjectFaultedException(string message, System.Exception innerException) { }
        protected CommunicationObjectFaultedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public enum CommunicationState
    {
        Closed = 4,
        Closing = 3,
        Created = 0,
        Faulted = 5,
        Opened = 2,
        Opening = 1,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = false, AllowMultiple = false)]
    public sealed partial class DataContractFormatAttribute : System.Attribute
    {
        public DataContractFormatAttribute() { }
        public System.ServiceModel.OperationFormatStyle Style { get { return default; } set { } }
    }
    public sealed class DeliveryRequirementsAttribute : System.Attribute, System.ServiceModel.Description.IContractBehavior
    {
        public QueuedDeliveryRequirementsMode QueuedDeliveryRequirements { get; set; }
        public bool RequireOrderedDelivery { get; set; }
        void System.ServiceModel.Description.IContractBehavior.AddBindingParameters(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void System.ServiceModel.Description.IContractBehavior.ApplyClientBehavior(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        void System.ServiceModel.Description.IContractBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime) { }
        void System.ServiceModel.Description.IContractBehavior.Validate(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint) { }
    }
    public partial class EndpointAddress
    {
        public EndpointAddress(string uri) { }
        public EndpointAddress(System.Uri uri, params System.ServiceModel.Channels.AddressHeader[] addressHeaders) { }
        public EndpointAddress(System.Uri uri, System.ServiceModel.EndpointIdentity identity, params System.ServiceModel.Channels.AddressHeader[] addressHeaders) { }
        public static System.Uri AnonymousUri { get { return default; } }
        public System.ServiceModel.Channels.AddressHeaderCollection Headers { get { return default; } }
        public System.ServiceModel.EndpointIdentity Identity { get { return default; } }
        public bool IsAnonymous { get { return default; } }
        public bool IsNone { get { return default; } }
        public static System.Uri NoneUri { get { return default; } }
        public System.Uri Uri { get { return default; } }
        public void ApplyTo(System.ServiceModel.Channels.Message message) { }
        public override bool Equals(object obj) { return default; }
        public override int GetHashCode() { return default; }
        public static bool operator ==(System.ServiceModel.EndpointAddress address1, System.ServiceModel.EndpointAddress address2) { return default; }
        public static bool operator !=(System.ServiceModel.EndpointAddress address1, System.ServiceModel.EndpointAddress address2) { return default; }
        public static System.ServiceModel.EndpointAddress ReadFrom(System.ServiceModel.Channels.AddressingVersion addressingVersion, System.Xml.XmlDictionaryReader reader) { return default; }
        public override string ToString() { return default; }
        public void WriteContentsTo(System.ServiceModel.Channels.AddressingVersion addressingVersion, System.Xml.XmlDictionaryWriter writer) { }
    }
    public partial class EndpointAddressBuilder
    {
        public EndpointAddressBuilder() { }
        public EndpointAddressBuilder(System.ServiceModel.EndpointAddress address) { }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Channels.AddressHeader> Headers { get { return default; } }
        public System.ServiceModel.EndpointIdentity Identity { get { return default; } set { } }
        public System.Uri Uri { get { return default; } set { } }
        public System.ServiceModel.EndpointAddress ToEndpointAddress() { return default; }
    }
    public abstract partial class EndpointIdentity
    {
        protected EndpointIdentity() { }
        protected void Initialize(System.IdentityModel.Claims.Claim identityClaim) { }
        protected void Initialize(System.IdentityModel.Claims.Claim identityClaim, Collections.Generic.IEqualityComparer<System.IdentityModel.Claims.Claim> claimComparer) { }
        public System.IdentityModel.Claims.Claim IdentityClaim => default;
        public override bool Equals(object obj) { return default; }
        public override int GetHashCode() { return default; }
        public override string ToString() { return default; }
        public static System.ServiceModel.EndpointIdentity CreateIdentity(System.IdentityModel.Claims.Claim identity) => default;
    }
    public partial class EndpointNotFoundException : System.ServiceModel.CommunicationException
    {
        public EndpointNotFoundException(string message) { }
        public EndpointNotFoundException(string message, System.Exception innerException) { }
        protected EndpointNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public sealed partial class EnvelopeVersion
    {
        internal EnvelopeVersion() { }
        public string NextDestinationActorValue { get { return default; } }
        public static System.ServiceModel.EnvelopeVersion None { get { return default; } }
        public static System.ServiceModel.EnvelopeVersion Soap11 { get { return default; } }
        public static System.ServiceModel.EnvelopeVersion Soap12 { get { return default; } }
        public string[] GetUltimateDestinationActorValues() { return default; }
        public override string ToString() { return default; }
    }
    [System.Runtime.Serialization.DataContractAttribute]
    public partial class ExceptionDetail
    {
        public ExceptionDetail(System.Exception exception) { }
        [System.Runtime.Serialization.DataMemberAttribute]
        public string HelpLink { get { return default; } set { } }
        [System.Runtime.Serialization.DataMemberAttribute]
        public System.ServiceModel.ExceptionDetail InnerException { get { return default; } set { } }
        [System.Runtime.Serialization.DataMemberAttribute]
        public string Message { get { return default; } set { } }
        [System.Runtime.Serialization.DataMemberAttribute]
        public string StackTrace { get { return default; } set { } }
        [System.Runtime.Serialization.DataMemberAttribute]
        public string Type { get { return default; } set { } }
        public override string ToString() { return default; }
    }
    public sealed partial class ExtensionCollection<T> : System.Collections.Generic.SynchronizedCollection<System.ServiceModel.IExtension<T>>, System.ServiceModel.IExtensionCollection<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        public ExtensionCollection(T owner) { }
        public ExtensionCollection(T owner, object syncRoot) : base(syncRoot) { }
        protected override void ClearItems() { }
        public E Find<E>() { return default; }
        public System.Collections.ObjectModel.Collection<E> FindAll<E>() { return default; }
        protected override void InsertItem(int index, System.ServiceModel.IExtension<T> item) { }
        protected override void RemoveItem(int index) { }
        protected override void SetItem(int index, System.ServiceModel.IExtension<T> item) { }
    }
    public partial class FaultCode
    {
        public FaultCode(string name) { }
        public FaultCode(string name, System.ServiceModel.FaultCode subCode) { }
        public FaultCode(string name, string ns) { }
        public FaultCode(string name, string ns, System.ServiceModel.FaultCode subCode) { }
        public bool IsPredefinedFault { get { return default; } }
        public bool IsReceiverFault { get { return default; } }
        public bool IsSenderFault { get { return default; } }
        public string Name { get { return default; } }
        public string Namespace { get { return default; } }
        public System.ServiceModel.FaultCode SubCode { get { return default; } }
        public static System.ServiceModel.FaultCode CreateSenderFaultCode(System.ServiceModel.FaultCode subCode) { return default; }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple = true, Inherited = false)]
    public sealed partial class FaultContractAttribute : System.Attribute
    {
        public FaultContractAttribute(System.Type detailType) { }
        public string Action { get { return default; } set { } }
        public System.Type DetailType { get { return default; } }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
    }
    [Serializable]
    public partial class FaultException : System.ServiceModel.CommunicationException
    {
        public FaultException() { }
        public FaultException(string reason) { }
        public FaultException(System.ServiceModel.Channels.MessageFault fault, string action) { }
        public FaultException(System.ServiceModel.FaultReason reason, System.ServiceModel.FaultCode code, string action) { }
        protected FaultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public string Action { get { return default; } }
        public System.ServiceModel.FaultCode Code { get { return default; } }
        public override string Message { get { return default; } }
        public System.ServiceModel.FaultReason Reason { get { return default; } }
        public static System.ServiceModel.FaultException CreateFault(System.ServiceModel.Channels.MessageFault messageFault, string action, params System.Type[] faultDetailTypes) { return default; }
        public static System.ServiceModel.FaultException CreateFault(System.ServiceModel.Channels.MessageFault messageFault, params System.Type[] faultDetailTypes) { return default; }
        public virtual System.ServiceModel.Channels.MessageFault CreateMessageFault() { return default; }
    }
    [Serializable]
    public partial class FaultException<TDetail> : System.ServiceModel.FaultException
    {
        public FaultException(TDetail detail) { }
        public FaultException(TDetail detail, System.ServiceModel.FaultReason reason) { }
        public FaultException(TDetail detail, System.ServiceModel.FaultReason reason, System.ServiceModel.FaultCode code, string action) { }
        public FaultException(TDetail detail, System.ServiceModel.FaultReason reason, System.ServiceModel.FaultCode code) { }
        public FaultException(TDetail detail, string reason) { }
        public FaultException(TDetail detail, string reason, System.ServiceModel.FaultCode code) { }
        public FaultException(TDetail detail, string reason, System.ServiceModel.FaultCode code, string action) { }
        protected FaultException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public TDetail Detail { get { return default; } }
        public override System.ServiceModel.Channels.MessageFault CreateMessageFault() { return default; }
        public override string ToString() { return default; }
    }
    public partial class FaultReason
    {
        public FaultReason(System.Collections.Generic.IEnumerable<System.ServiceModel.FaultReasonText> translations) { }
        public FaultReason(System.ServiceModel.FaultReasonText translation) { }
        public FaultReason(string text) { }
        public System.ServiceModel.FaultReasonText GetMatchingTranslation() { return default; }
        public System.ServiceModel.FaultReasonText GetMatchingTranslation(System.Globalization.CultureInfo cultureInfo) { return default; }
        public override string ToString() { return default; }
    }
    public partial class FaultReasonText
    {
        public FaultReasonText(string text) { }
        public FaultReasonText(string text, string xmlLang) { }
        public string Text { get { return default; } }
        public string XmlLang { get { return default; } }
        public bool Matches(System.Globalization.CultureInfo cultureInfo) { return default; }
    }
    public enum HostNameComparisonMode
    {
        StrongWildcard = 0, // +
        Exact = 1,
        WeakWildcard = 2,   // *
    }
    public partial interface IClientChannel : System.IDisposable, System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject, System.ServiceModel.IContextChannel, System.ServiceModel.IExtensibleObject<System.ServiceModel.IContextChannel>
    {
        bool AllowInitializationUI { get; set; }
        bool DidInteractiveInitialization { get; }
        System.Uri Via { get; }
        event System.EventHandler<System.ServiceModel.UnknownMessageReceivedEventArgs> UnknownMessageReceived;
        System.IAsyncResult BeginDisplayInitializationUI(System.AsyncCallback callback, object state);
        void DisplayInitializationUI();
        void EndDisplayInitializationUI(System.IAsyncResult result);
    }
    public partial interface ICommunicationObject
    {
        System.ServiceModel.CommunicationState State { get; }
        event System.EventHandler Closed;
        event System.EventHandler Closing;
        event System.EventHandler Faulted;
        event System.EventHandler Opened;
        event System.EventHandler Opening;
        void Abort();
        System.IAsyncResult BeginClose(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginOpen(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        void Close();
        void Close(System.TimeSpan timeout);
        void EndClose(System.IAsyncResult result);
        void EndOpen(System.IAsyncResult result);
        void Open();
        void Open(System.TimeSpan timeout);
    }
    public partial interface IContextChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject, System.ServiceModel.IExtensibleObject<System.ServiceModel.IContextChannel>
    {
        bool AllowOutputBatching { get; set; }
        System.ServiceModel.Channels.IInputSession InputSession { get; }
        System.ServiceModel.EndpointAddress LocalAddress { get; }
        System.TimeSpan OperationTimeout { get; set; }
        System.ServiceModel.Channels.IOutputSession OutputSession { get; }
        System.ServiceModel.EndpointAddress RemoteAddress { get; }
        string SessionId { get; }
    }
    public partial interface IDefaultCommunicationTimeouts
    {
        System.TimeSpan CloseTimeout { get; }
        System.TimeSpan OpenTimeout { get; }
        System.TimeSpan ReceiveTimeout { get; }
        System.TimeSpan SendTimeout { get; }
    }
    public partial interface IExtensibleObject<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        System.ServiceModel.IExtensionCollection<T> Extensions { get; }
    }
    public partial interface IExtension<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        void Attach(T owner);
        void Detach(T owner);
    }
    public partial interface IExtensionCollection<T> : System.Collections.Generic.ICollection<System.ServiceModel.IExtension<T>>, System.Collections.Generic.IEnumerable<System.ServiceModel.IExtension<T>>, System.Collections.IEnumerable where T : System.ServiceModel.IExtensibleObject<T>
    {
        E Find<E>();
        System.Collections.ObjectModel.Collection<E> FindAll<E>();
    }
    public partial class InvalidMessageContractException : System.Exception
    {
        public InvalidMessageContractException() { }
        public InvalidMessageContractException(string message) { }
        public InvalidMessageContractException(string message, System.Exception innerException) { }
        protected InvalidMessageContractException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = false)]
    public partial class MessageHeaderAttribute : MessageContractMemberAttribute
    {
        public bool MustUnderstand { get { return default; } set { } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = false)]
    public sealed partial class MessageHeaderArrayAttribute : MessageHeaderAttribute
    {
        public MessageHeaderArrayAttribute() { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), Inherited = false)]
    public partial class MessageBodyMemberAttribute : System.ServiceModel.MessageContractMemberAttribute
    {
        public MessageBodyMemberAttribute() { }
        public int Order { get { return default; } set { } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(12), AllowMultiple = false)]
    public sealed partial class MessageContractAttribute : System.Attribute
    {
        public MessageContractAttribute() { }
        public bool IsWrapped { get { return default; } set { } }
        public string WrapperName { get { return default; } set { } }
        public string WrapperNamespace { get { return default; } set { } }
    }
    public abstract partial class MessageContractMemberAttribute : System.Attribute
    {
        protected MessageContractMemberAttribute() { }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public sealed partial class MessagePropertyAttribute : System.Attribute
    {
        public MessagePropertyAttribute() { }
        public string Name { get { return default; } set { } }
    }
    public enum MessageCredentialType
    {
        Certificate = 3,
        IssuedToken = 4,
        None = 0,
        UserName = 2,
        Windows = 1,
    }
    public partial class MessageHeader<T>
    {
        public MessageHeader() { }
        public MessageHeader(T content) { }
        public MessageHeader(T content, bool mustUnderstand, string actor, bool relay) { }
        public string Actor { get { return default; } set { } }
        public T Content { get { return default; } set { } }
        public bool MustUnderstand { get { return default; } set { } }
        public bool Relay { get { return default; } set { } }
        public System.ServiceModel.Channels.MessageHeader GetUntypedHeader(string name, string ns) { return default; }
    }
    public partial class MessageHeaderException : System.ServiceModel.ProtocolException
    {
        public MessageHeaderException(string message) : base(default) { }
        public MessageHeaderException(string message, bool isDuplicate) : base(default) { }
        public MessageHeaderException(string message, System.Exception innerException) : base(default) { }
        public MessageHeaderException(string message, string headerName, string ns) : base(default) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate) : base(default) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate, System.Exception innerException) : base(default) { }
        public MessageHeaderException(string message, string headerName, string ns, System.Exception innerException) : base(default) { }
        protected MessageHeaderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public string HeaderName { get { return default; } }
        public string HeaderNamespace { get { return default; } }
        public bool IsDuplicate { get { return default; } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(10240), Inherited = false)]
    public sealed partial class MessageParameterAttribute : System.Attribute
    {
        public MessageParameterAttribute() { }
        public string Name { get { return default; } set { } }
    }
    public sealed partial class OperationContext : System.ServiceModel.IExtensibleObject<System.ServiceModel.OperationContext>
    {
        public OperationContext(System.ServiceModel.IContextChannel channel) { }
        public static System.ServiceModel.OperationContext Current { get { return default; } set { } }
        public System.ServiceModel.IExtensionCollection<System.ServiceModel.OperationContext> Extensions { get { return default; } }
        public System.ServiceModel.Channels.MessageHeaders IncomingMessageHeaders { get { return default; } }
        public System.ServiceModel.Channels.MessageProperties IncomingMessageProperties { get { return default; } }
        public System.ServiceModel.Channels.MessageVersion IncomingMessageVersion { get { return default; } }
        public bool IsUserContext { get { return default; } }
        public System.ServiceModel.Channels.MessageHeaders OutgoingMessageHeaders { get { return default; } }
        public System.ServiceModel.Channels.MessageProperties OutgoingMessageProperties { get { return default; } }
        public System.ServiceModel.Channels.RequestContext RequestContext { get { return default; } set { } }
        public event System.EventHandler OperationCompleted { add { } remove { } }
        public T GetCallbackChannel<T>() { return default; }
        public System.ServiceModel.IContextChannel Channel { get { return default; } }
    }
    public sealed partial class OperationContextScope : System.IDisposable
    {
        public OperationContextScope(System.ServiceModel.IContextChannel channel) { }
        public OperationContextScope(System.ServiceModel.OperationContext context) { }
        public void Dispose() { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
    public sealed partial class OperationContractAttribute : System.Attribute
    {
        public OperationContractAttribute() { }
        public string Action { get { return default; } set { } }
        public bool AsyncPattern { get { return default; } set { } }
        public bool IsInitiating { get { return default; } set { } }
        public bool IsTerminating { get { return default; } set { } }
        public bool IsOneWay { get { return default; } set { } }
        public string Name { get { return default; } set { } }
        public string ReplyAction { get { return default; } set { } }
    }
    public enum OperationFormatStyle
    {
        Document = 0,
        Rpc = 1,
    }
    public enum OperationFormatUse
    {
        Literal,
        Encoded,
    }
    public class OptionalReliableSession : System.ServiceModel.ReliableSession
    {
        public OptionalReliableSession() { }
        public OptionalReliableSession(System.ServiceModel.Channels.ReliableSessionBindingElement reliableSessionBindingElement) { }
        public bool Enabled { get; set; }
    }
    public partial class ProtocolException : System.ServiceModel.CommunicationException
    {
        public ProtocolException(string message) { }
        public ProtocolException(string message, System.Exception innerException) { }
        protected ProtocolException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public enum QueuedDeliveryRequirementsMode
    {
        Allowed,
        Required,
        NotAllowed,
    }
    public partial class QuotaExceededException : System.Exception
    {
        public QuotaExceededException(string message) { }
        public QuotaExceededException(string message, System.Exception innerException) { }
        protected QuotaExceededException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public abstract class ReliableMessagingVersion
    {
        public static ReliableMessagingVersion Default { get { return default; } }
        public static ReliableMessagingVersion WSReliableMessaging11 { get { return default; } }
        public static ReliableMessagingVersion WSReliableMessagingFebruary2005 { get { return default; } }
    }
    public class ReliableSession
    {
        public ReliableSession() { }
        public ReliableSession(System.ServiceModel.Channels.ReliableSessionBindingElement reliableSessionBindingElement) { }
        public bool Ordered { get; set; }
        public System.TimeSpan InactivityTimeout { get; set; }
    }
    public enum SecurityMode
    {
        Message = 2,
        None = 0,
        Transport = 1,
        TransportWithMessageCredential = 3,
    }
    public partial class ServerTooBusyException : System.ServiceModel.CommunicationException
    {
        public ServerTooBusyException(string message) { }
        public ServerTooBusyException(string message, System.Exception innerException) { }
        protected ServerTooBusyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial class ServiceActivationException : System.ServiceModel.CommunicationException
    {
        public ServiceActivationException(string message) { }
        public ServiceActivationException(string message, System.Exception innerException) { }
        protected ServiceActivationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1028), Inherited = false, AllowMultiple = false)]
    public sealed partial class ServiceContractAttribute : System.Attribute
    {
        public ServiceContractAttribute() { }
        public System.Type CallbackContract { get { return default; } set { } }
        public string ConfigurationName { get { return default; } set { } }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
        public SessionMode SessionMode { get { return SessionMode.Allowed; } set { } }
    }
    public enum SessionMode
    {
        Allowed,
        Required,
        NotAllowed,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = true, AllowMultiple = true)]
    public sealed partial class ServiceKnownTypeAttribute : System.Attribute
    {
        public ServiceKnownTypeAttribute(string methodName) { }
        public ServiceKnownTypeAttribute(string methodName, System.Type declaringType) { }
        public ServiceKnownTypeAttribute(System.Type type) { }
        public System.Type DeclaringType { get { return default; } }
        public string MethodName { get { return default; } }
        public System.Type Type { get { return default; } }
    }
    public enum TransferMode
    {
        Buffered = 0,
        Streamed = 1,
        StreamedRequest = 2,
        StreamedResponse = 3,
    }
    public sealed partial class UnknownMessageReceivedEventArgs : System.EventArgs
    {
        internal UnknownMessageReceivedEventArgs() { }
        public System.ServiceModel.Channels.Message Message { get { return default; } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = false, AllowMultiple = false)]
    public sealed partial class XmlSerializerFormatAttribute : System.Attribute
    {
        public XmlSerializerFormatAttribute() { }
        public System.ServiceModel.OperationFormatStyle Style { get { return default; } set { } }
        public bool SupportFaults { get { return default; } set { } }
        public OperationFormatUse Use { get { throw null; } set { } }
    }
    public sealed partial class InstanceContext : System.ServiceModel.Channels.CommunicationObject, System.ServiceModel.IExtensibleObject<System.ServiceModel.InstanceContext>
    {
        public InstanceContext(object implementation) { }
        protected override System.TimeSpan DefaultCloseTimeout { get { return default(System.TimeSpan); } }
        protected override System.TimeSpan DefaultOpenTimeout { get { return default(System.TimeSpan); } }
        public System.Threading.SynchronizationContext SynchronizationContext { get { return default(System.Threading.SynchronizationContext); } set { } }
        public System.ServiceModel.IExtensionCollection<System.ServiceModel.InstanceContext> Extensions { get { return default(System.ServiceModel.IExtensionCollection<System.ServiceModel.InstanceContext>); } }
        public object GetServiceInstance(System.ServiceModel.Channels.Message message) { return default(object); }
        protected override void OnAbort() { }
        protected override System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        protected override System.IAsyncResult OnBeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default(System.IAsyncResult); }
        protected override void OnClose(System.TimeSpan timeout) { }
        protected override void OnClosed() { }
        protected override void OnEndClose(System.IAsyncResult result) { }
        protected override void OnEndOpen(System.IAsyncResult result) { }
        protected override void OnFaulted() { }
        protected override void OnOpen(System.TimeSpan timeout) { }
        protected override void OnOpened() { }
        protected override void OnOpening() { }
    }
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
    [System.AttributeUsageAttribute((System.AttributeTargets)(4))]
    public sealed partial class CallbackBehaviorAttribute : System.Attribute, System.ServiceModel.Description.IEndpointBehavior
    {
        public CallbackBehaviorAttribute() { }
        public bool AutomaticSessionShutdown { get { return default(bool); } set { } }
        public bool UseSynchronizationContext { get { return default(bool); } set { } }
        public System.ServiceModel.ConcurrencyMode ConcurrencyMode { get { return default(System.ServiceModel.ConcurrencyMode); } set { } }
        void System.ServiceModel.Description.IEndpointBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) { }
        void System.ServiceModel.Description.IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) { }
    }
    public enum ConcurrencyMode
    {
        Single = 0,
        [System.Obsolete]
        Reentrant = 1,
        Multiple = 2
    }
    public partial class DuplexChannelFactory<TChannel> : System.ServiceModel.ChannelFactory<TChannel>
    {
        public DuplexChannelFactory(Type callbackInstanceType) : base(default(System.Type)) { }
        public DuplexChannelFactory(Type callbackInstanceType, System.ServiceModel.Channels.Binding binding) : base(default(System.Type)) { }
        public DuplexChannelFactory(Type callbackInstanceType, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(default(System.Type)) { }
        public DuplexChannelFactory(Type callbackInstanceType, System.ServiceModel.Channels.Binding binding, string remoteAddress) : base(default(System.Type)) { }
        public DuplexChannelFactory(Type callbackInstanceType, System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(default(System.Type)) { }
        public DuplexChannelFactory(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, string remoteAddress) : base(default(System.Type)) { }
        public override TChannel CreateChannel(System.ServiceModel.EndpointAddress address, System.Uri via) { return default(TChannel); }
        public TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance) { return default(TChannel); }
        public TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.EndpointAddress address) { return default(TChannel); }
        public virtual TChannel CreateChannel(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.EndpointAddress address, System.Uri via) { return default(TChannel); }
    }
    public abstract partial class DuplexClientBase<TChannel> : System.ServiceModel.ClientBase<TChannel> where TChannel : class
    {
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance) { }
        protected DuplexClientBase(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) { }
    }
}
namespace System.ServiceModel.Channels
{
    public abstract partial class AddressHeader
    {
        protected AddressHeader() { }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public static System.ServiceModel.Channels.AddressHeader CreateAddressHeader(string name, string ns, object value) { return default; }
        public static System.ServiceModel.Channels.AddressHeader CreateAddressHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public override bool Equals(object obj) { return default; }
        public virtual System.Xml.XmlDictionaryReader GetAddressHeaderReader() { return default; }
        public override int GetHashCode() { return default; }
        public T GetValue<T>() { return default; }
        public T GetValue<T>(System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        protected abstract void OnWriteAddressHeaderContents(System.Xml.XmlDictionaryWriter writer);
        protected virtual void OnWriteStartAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
        public System.ServiceModel.Channels.MessageHeader ToMessageHeader() { return default; }
        public void WriteAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteAddressHeader(System.Xml.XmlWriter writer) { }
        public void WriteAddressHeaderContents(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
    }
    public sealed partial class AddressHeaderCollection : System.Collections.ObjectModel.ReadOnlyCollection<System.ServiceModel.Channels.AddressHeader>
    {
        public AddressHeaderCollection() : base(default) { }
        public AddressHeaderCollection(System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.AddressHeader> addressHeaders) : base(default) { }
        public void AddHeadersTo(System.ServiceModel.Channels.Message message) { }
        public System.ServiceModel.Channels.AddressHeader[] FindAll(string name, string ns) { return default; }
        public System.ServiceModel.Channels.AddressHeader FindHeader(string name, string ns) { return default; }
    }
    public sealed partial class AddressingVersion
    {
        internal AddressingVersion() { }
        public static System.ServiceModel.Channels.AddressingVersion None { get { return default; } }
        public static System.ServiceModel.Channels.AddressingVersion WSAddressing10 { get { return default; } }
        public static System.ServiceModel.Channels.AddressingVersion WSAddressingAugust2004 { get { return default; } }
        public override string ToString() { return default; }
    }
    public sealed partial class BinaryMessageEncodingBindingElement : System.ServiceModel.Channels.MessageEncodingBindingElement
    {
        public BinaryMessageEncodingBindingElement() { }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.Channels.CompressionFormat)(0))]
        public System.ServiceModel.Channels.CompressionFormat CompressionFormat { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(2048)]
        public int MaxSessionSize { get { return default; } set { } }
        public int MaxReadPoolSize { get { return default; } set { } }
        public int MaxWritePoolSize { get { return default; } set { } }
        public override System.ServiceModel.Channels.MessageVersion MessageVersion { get { return default; } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override System.ServiceModel.Channels.MessageEncoderFactory CreateMessageEncoderFactory() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public abstract partial class Binding : System.ServiceModel.IDefaultCommunicationTimeouts
    {
        protected Binding() { }
        protected Binding(string name, string ns) { }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:01:00")]
        public System.TimeSpan CloseTimeout { get { return default; } set { } }
        public System.ServiceModel.Channels.MessageVersion MessageVersion { get { return default; } }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:01:00")]
        public System.TimeSpan OpenTimeout { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:10:00")]
        public System.TimeSpan ReceiveTimeout { get { return default; } set { } }
        public abstract string Scheme { get; }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:01:00")]
        public System.TimeSpan SendTimeout { get { return default; } set { } }
        public System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(params object[] parameters) { return default; }
        public virtual System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default; }
        public bool CanBuildChannelFactory<TChannel>(params object[] parameters) { return default; }
        public virtual bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default; }
        public abstract System.ServiceModel.Channels.BindingElementCollection CreateBindingElements();
        public T GetProperty<T>(System.ServiceModel.Channels.BindingParameterCollection parameters) where T : class { return default; }
    }
    public partial class BindingContext
    {
        public BindingContext(System.ServiceModel.Channels.CustomBinding binding, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        public System.ServiceModel.Channels.CustomBinding Binding { get { return default; } }
        public System.ServiceModel.Channels.BindingParameterCollection BindingParameters { get { return default; } }
        public System.ServiceModel.Channels.BindingElementCollection RemainingBindingElements { get { return default; } }
        public System.ServiceModel.Channels.IChannelFactory<TChannel> BuildInnerChannelFactory<TChannel>() { return default; }
        public bool CanBuildInnerChannelFactory<TChannel>() { return default; }
        public System.ServiceModel.Channels.BindingContext Clone() { return default; }
        public T GetInnerProperty<T>() where T : class { return default; }
    }
    public abstract partial class BindingElement
    {
        protected BindingElement() { }
        protected BindingElement(System.ServiceModel.Channels.BindingElement elementToBeCloned) { }
        public virtual System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public virtual bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public abstract System.ServiceModel.Channels.BindingElement Clone();
        public abstract T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) where T : class;
    }
    public partial class BindingElementCollection : System.Collections.ObjectModel.Collection<System.ServiceModel.Channels.BindingElement>
    {
        public BindingElementCollection() { }
        public BindingElementCollection(System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.BindingElement> elements) { }
        public BindingElementCollection(System.ServiceModel.Channels.BindingElement[] elements) { }
        public void AddRange(params System.ServiceModel.Channels.BindingElement[] elements) { }
        public System.ServiceModel.Channels.BindingElementCollection Clone() { return default; }
        public bool Contains(System.Type bindingElementType) { return default; }
        public T Find<T>() { return default; }
        public System.Collections.ObjectModel.Collection<T> FindAll<T>() { return default; }
        protected override void InsertItem(int index, System.ServiceModel.Channels.BindingElement item) { }
        public T Remove<T>() { return default; }
        public System.Collections.ObjectModel.Collection<T> RemoveAll<T>() { return default; }
        protected override void SetItem(int index, System.ServiceModel.Channels.BindingElement item) { }
    }
    // TODO : Check that changing from KeyedCollection<Type,object> to KeyedByTypeCollection<object> isn't a binary break
    public partial class BindingParameterCollection : System.Collections.Generic.KeyedByTypeCollection<object>
    {
        public BindingParameterCollection() { }
    }
    public abstract partial class BodyWriter
    {
        protected BodyWriter(bool isBuffered) { }
        public bool IsBuffered { get { return default; } }
        public System.ServiceModel.Channels.BodyWriter CreateBufferedCopy(int maxBufferSize) { return default; }
        protected virtual System.ServiceModel.Channels.BodyWriter OnCreateBufferedCopy(int maxBufferSize) { return default; }
        protected abstract void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        public void WriteBodyContents(System.Xml.XmlDictionaryWriter writer) { }
    }
    public abstract partial class BufferManager
    {
        protected BufferManager() { }
        public abstract void Clear();
        public static System.ServiceModel.Channels.BufferManager CreateBufferManager(long maxBufferPoolSize, int maxBufferSize) { return default; }
        public abstract void ReturnBuffer(byte[] buffer);
        public abstract byte[] TakeBuffer(int bufferSize);
    }
    public abstract partial class ChannelBase : System.ServiceModel.Channels.CommunicationObject, System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject, System.ServiceModel.IDefaultCommunicationTimeouts
    {
        protected ChannelBase(System.ServiceModel.Channels.ChannelManagerBase channelManager) { }
        protected override System.TimeSpan DefaultCloseTimeout { get { return default; } }
        protected override System.TimeSpan DefaultOpenTimeout { get { return default; } }
        protected System.TimeSpan DefaultReceiveTimeout { get { return default; } }
        protected System.TimeSpan DefaultSendTimeout { get { return default; } }
        protected System.ServiceModel.Channels.ChannelManagerBase Manager { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.CloseTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.OpenTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.ReceiveTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.SendTimeout { get { return default; } }
        public virtual T GetProperty<T>() where T : class { return default; }
        protected override void OnClosed() { }
    }
    public abstract partial class ChannelFactoryBase : System.ServiceModel.Channels.ChannelManagerBase, System.ServiceModel.Channels.IChannelFactory, System.ServiceModel.ICommunicationObject
    {
        protected ChannelFactoryBase() { }
        protected ChannelFactoryBase(System.ServiceModel.IDefaultCommunicationTimeouts timeouts) { }
        protected override System.TimeSpan DefaultCloseTimeout { get { return default; } }
        protected override System.TimeSpan DefaultOpenTimeout { get { return default; } }
        protected override System.TimeSpan DefaultReceiveTimeout { get { return default; } }
        protected override System.TimeSpan DefaultSendTimeout { get { return default; } }
        public virtual T GetProperty<T>() where T : class { return default; }
        protected override void OnAbort() { }
        protected override System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        protected override void OnClose(System.TimeSpan timeout) { }
        protected override void OnEndClose(System.IAsyncResult result) { }
    }
    public abstract partial class ChannelFactoryBase<TChannel> : System.ServiceModel.Channels.ChannelFactoryBase, System.ServiceModel.Channels.IChannelFactory, System.ServiceModel.Channels.IChannelFactory<TChannel>, System.ServiceModel.ICommunicationObject
    {
        protected ChannelFactoryBase() { }
        protected ChannelFactoryBase(System.ServiceModel.IDefaultCommunicationTimeouts timeouts) { }
        public TChannel CreateChannel(System.ServiceModel.EndpointAddress address) { return default; }
        public TChannel CreateChannel(System.ServiceModel.EndpointAddress address, System.Uri via) { return default; }
        protected override void OnAbort() { }
        protected override System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        protected override void OnClose(System.TimeSpan timeout) { }
        protected abstract TChannel OnCreateChannel(System.ServiceModel.EndpointAddress address, System.Uri via);
        protected override void OnEndClose(System.IAsyncResult result) { }
        protected void ValidateCreateChannel() { }
    }
    public abstract partial class ChannelManagerBase : System.ServiceModel.Channels.CommunicationObject, System.ServiceModel.IDefaultCommunicationTimeouts
    {
        protected ChannelManagerBase() { }
        protected abstract System.TimeSpan DefaultReceiveTimeout { get; }
        protected abstract System.TimeSpan DefaultSendTimeout { get; }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.CloseTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.OpenTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.ReceiveTimeout { get { return default; } }
        System.TimeSpan System.ServiceModel.IDefaultCommunicationTimeouts.SendTimeout { get { return default; } }
    }
    public partial class ChannelParameterCollection : System.Collections.ObjectModel.Collection<object>
    {
        public ChannelParameterCollection() { }
        public ChannelParameterCollection(System.ServiceModel.Channels.IChannel channel) { }
        protected virtual System.ServiceModel.Channels.IChannel Channel { get { return default; } }
        protected override void ClearItems() { }
        protected override void InsertItem(int index, object item) { }
        public void PropagateChannelParameters(System.ServiceModel.Channels.IChannel innerChannel) { }
        protected override void RemoveItem(int index) { }
        protected override void SetItem(int index, object item) { }
    }
    public abstract partial class CommunicationObject : System.ServiceModel.ICommunicationObject
    {
        protected CommunicationObject() { }
        protected CommunicationObject(object mutex) { }
        protected abstract System.TimeSpan DefaultCloseTimeout { get; }
        protected abstract System.TimeSpan DefaultOpenTimeout { get; }
        protected bool IsDisposed { get { return default; } }
        public System.ServiceModel.CommunicationState State { get { return default; } }
        protected object ThisLock { get { return default; } }
        public event System.EventHandler Closed { add { } remove { } }
        public event System.EventHandler Closing { add { } remove { } }
        public event System.EventHandler Faulted { add { } remove { } }
        public event System.EventHandler Opened { add { } remove { } }
        public event System.EventHandler Opening { add { } remove { } }
        public void Abort() { }
        public System.IAsyncResult BeginClose(System.AsyncCallback callback, object state) { return default; }
        public System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        public System.IAsyncResult BeginOpen(System.AsyncCallback callback, object state) { return default; }
        public System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state) { return default; }
        public void Close() { }
        public void Close(System.TimeSpan timeout) { }
        public void EndClose(System.IAsyncResult result) { }
        public void EndOpen(System.IAsyncResult result) { }
        protected void Fault() { }
        protected virtual System.Type GetCommunicationObjectType() { return default; }
        protected abstract void OnAbort();
        protected abstract System.IAsyncResult OnBeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        protected abstract System.IAsyncResult OnBeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        protected abstract void OnClose(System.TimeSpan timeout);
        protected virtual void OnClosed() { }
        protected virtual void OnClosing() { }
        protected abstract void OnEndClose(System.IAsyncResult result);
        protected abstract void OnEndOpen(System.IAsyncResult result);
        protected virtual void OnFaulted() { }
        protected abstract void OnOpen(System.TimeSpan timeout);
        protected virtual void OnOpened() { }
        protected virtual void OnOpening() { }
        public void Open() { }
        public void Open(System.TimeSpan timeout) { }
    }
    public enum CompressionFormat
    {
        Deflate = 2,
        GZip = 1,
        None = 0,
    }
    public partial class CustomBinding : System.ServiceModel.Channels.Binding
    {
        public CustomBinding() { }
        public CustomBinding(System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.BindingElement> bindingElementsInTopDownChannelStackOrder) { }
        public CustomBinding(System.ServiceModel.Channels.Binding binding) { }
        public CustomBinding(params System.ServiceModel.Channels.BindingElement[] bindingElementsInTopDownChannelStackOrder) { }
        public CustomBinding(string name, string ns, params System.ServiceModel.Channels.BindingElement[] bindingElementsInTopDownChannelStackOrder) { }
        public System.ServiceModel.Channels.BindingElementCollection Elements { get { return default; } }
        public override string Scheme { get { return default; } }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default; }
    }
    public abstract partial class FaultConverter
    {
        protected FaultConverter() { }
        public static System.ServiceModel.Channels.FaultConverter GetDefaultFaultConverter(System.ServiceModel.Channels.MessageVersion version) { return default; }
        protected abstract bool OnTryCreateException(System.ServiceModel.Channels.Message message, System.ServiceModel.Channels.MessageFault fault, out System.Exception exception);
        protected abstract bool OnTryCreateFaultMessage(System.Exception exception, out System.ServiceModel.Channels.Message message);
        public bool TryCreateException(System.ServiceModel.Channels.Message message, System.ServiceModel.Channels.MessageFault fault, out System.Exception exception) { exception = default; return default; }
    }
    public partial interface IChannel : System.ServiceModel.ICommunicationObject
    {
        T GetProperty<T>() where T : class;
    }
    public partial interface IChannelFactory : System.ServiceModel.ICommunicationObject
    {
        T GetProperty<T>() where T : class;
    }
    public partial interface IChannelFactory<TChannel> : System.ServiceModel.Channels.IChannelFactory, System.ServiceModel.ICommunicationObject
    {
        TChannel CreateChannel(System.ServiceModel.EndpointAddress to);
        TChannel CreateChannel(System.ServiceModel.EndpointAddress to, System.Uri via);
    }
    public partial interface IBindingDeliveryCapabilities
    {
        bool AssuresOrderedDelivery { get; }
        bool QueuedDelivery { get; }
    }
    public partial interface IDuplexChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IInputChannel, System.ServiceModel.Channels.IOutputChannel, System.ServiceModel.ICommunicationObject
    {
    }
    public partial interface IDuplexSession : System.ServiceModel.Channels.IInputSession, System.ServiceModel.Channels.IOutputSession, System.ServiceModel.Channels.ISession
    {
        System.IAsyncResult BeginCloseOutputSession(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginCloseOutputSession(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        void CloseOutputSession();
        void CloseOutputSession(System.TimeSpan timeout);
        void EndCloseOutputSession(System.IAsyncResult result);
    }
    public partial interface IDuplexSessionChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IDuplexChannel, System.ServiceModel.Channels.IInputChannel, System.ServiceModel.Channels.IOutputChannel, System.ServiceModel.Channels.ISessionChannel<System.ServiceModel.Channels.IDuplexSession>, System.ServiceModel.ICommunicationObject
    {
    }
    public partial interface IInputChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject
    {
        System.ServiceModel.EndpointAddress LocalAddress { get; }
        System.IAsyncResult BeginReceive(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginReceive(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginTryReceive(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginWaitForMessage(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.ServiceModel.Channels.Message EndReceive(System.IAsyncResult result);
        bool EndTryReceive(System.IAsyncResult result, out System.ServiceModel.Channels.Message message);
        bool EndWaitForMessage(System.IAsyncResult result);
        System.ServiceModel.Channels.Message Receive();
        System.ServiceModel.Channels.Message Receive(System.TimeSpan timeout);
        bool TryReceive(System.TimeSpan timeout, out System.ServiceModel.Channels.Message message);
        bool WaitForMessage(System.TimeSpan timeout);
    }
    public partial interface IInputSession : System.ServiceModel.Channels.ISession
    {
    }
    public partial interface IInputSessionChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IInputChannel, System.ServiceModel.Channels.ISessionChannel<System.ServiceModel.Channels.IInputSession>, System.ServiceModel.ICommunicationObject
    {
    }
    public partial interface IMessageProperty
    {
        System.ServiceModel.Channels.IMessageProperty CreateCopy();
    }
    public partial interface IOutputChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject
    {
        System.ServiceModel.EndpointAddress RemoteAddress { get; }
        System.Uri Via { get; }
        System.IAsyncResult BeginSend(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginSend(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state);
        void EndSend(System.IAsyncResult result);
        void Send(System.ServiceModel.Channels.Message message);
        void Send(System.ServiceModel.Channels.Message message, System.TimeSpan timeout);
    }
    public partial interface IOutputSession : System.ServiceModel.Channels.ISession
    {
    }
    public partial interface IOutputSessionChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IOutputChannel, System.ServiceModel.Channels.ISessionChannel<System.ServiceModel.Channels.IOutputSession>, System.ServiceModel.ICommunicationObject
    {
    }
    public partial interface IRequestChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject
    {
        System.ServiceModel.EndpointAddress RemoteAddress { get; }
        System.Uri Via { get; }
        System.IAsyncResult BeginRequest(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginRequest(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.ServiceModel.Channels.Message EndRequest(System.IAsyncResult result);
        System.ServiceModel.Channels.Message Request(System.ServiceModel.Channels.Message message);
        System.ServiceModel.Channels.Message Request(System.ServiceModel.Channels.Message message, System.TimeSpan timeout);
    }
    public partial interface IRequestSessionChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.Channels.IRequestChannel, System.ServiceModel.Channels.ISessionChannel<System.ServiceModel.Channels.IOutputSession>, System.ServiceModel.ICommunicationObject
    {
    }
    public partial interface ISession
    {
        string Id { get; }
    }
    public partial interface ISessionChannel<TSession> where TSession : System.ServiceModel.Channels.ISession
    {
        TSession Session { get; }
    }
    public abstract partial class Message : System.IDisposable
    {
        protected Message() { }
        public abstract System.ServiceModel.Channels.MessageHeaders Headers { get; }
        protected bool IsDisposed { get { return default; } }
        public virtual bool IsEmpty { get { return default; } }
        public virtual bool IsFault { get { return default; } }
        public abstract System.ServiceModel.Channels.MessageProperties Properties { get; }
        public System.ServiceModel.Channels.MessageState State { get { return default; } }
        public abstract System.ServiceModel.Channels.MessageVersion Version { get; }
        public void Close() { }
        public System.ServiceModel.Channels.MessageBuffer CreateBufferedCopy(int maxBufferSize) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, object body) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, object body, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.ServiceModel.Channels.BodyWriter body) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.Xml.XmlDictionaryReader body) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.Xml.XmlReader body) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.Xml.XmlDictionaryReader envelopeReader, int maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion version) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.Xml.XmlReader envelopeReader, int maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion version) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, System.ServiceModel.FaultCode faultCode, string reason, string action) { return default; }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, System.ServiceModel.FaultCode faultCode, string reason, object detail, string action) { return default; }
        public T GetBody<T>() { return default; }
        public T GetBody<T>(System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public string GetBodyAttribute(string localName, string ns) { return default; }
        public System.Xml.XmlDictionaryReader GetReaderAtBodyContents() { return default; }
        protected virtual void OnBodyToString(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnClose() { }
        protected virtual System.ServiceModel.Channels.MessageBuffer OnCreateBufferedCopy(int maxBufferSize) { return default; }
        protected virtual T OnGetBody<T>(System.Xml.XmlDictionaryReader reader) { return default; }
        protected virtual string OnGetBodyAttribute(string localName, string ns) { return default; }
        protected virtual System.Xml.XmlDictionaryReader OnGetReaderAtBodyContents() { return default; }
        protected abstract void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        protected virtual void OnWriteMessage(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartBody(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartEnvelope(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartHeaders(System.Xml.XmlDictionaryWriter writer) { }
        void System.IDisposable.Dispose() { }
        public override string ToString() { return default; }
        public void WriteBody(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteBody(System.Xml.XmlWriter writer) { }
        public void WriteBodyContents(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteMessage(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteMessage(System.Xml.XmlWriter writer) { }
        public void WriteStartBody(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartBody(System.Xml.XmlWriter writer) { }
        public void WriteStartEnvelope(System.Xml.XmlDictionaryWriter writer) { }
    }
    public abstract partial class MessageBuffer : System.IDisposable
    {
        protected MessageBuffer() { }
        public abstract int BufferSize { get; }
        public virtual string MessageContentType { get { return default; } }
        public abstract void Close();
        public abstract System.ServiceModel.Channels.Message CreateMessage();
        void System.IDisposable.Dispose() { }
        public virtual void WriteMessage(System.IO.Stream stream) { }
    }
    public abstract partial class MessageEncoder
    {
        protected MessageEncoder() { }
        public abstract string ContentType { get; }
        public abstract string MediaType { get; }
        public abstract System.ServiceModel.Channels.MessageVersion MessageVersion { get; }
        public virtual T GetProperty<T>() where T : class { return default; }
        public virtual bool IsContentTypeSupported(string contentType) { return default; }
        public System.ServiceModel.Channels.Message ReadMessage(System.ArraySegment<byte> buffer, System.ServiceModel.Channels.BufferManager bufferManager) { return default; }
        public abstract System.ServiceModel.Channels.Message ReadMessage(System.ArraySegment<byte> buffer, System.ServiceModel.Channels.BufferManager bufferManager, string contentType);
        public System.ServiceModel.Channels.Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders) { return default; }
        public abstract System.ServiceModel.Channels.Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType);
        public override string ToString() { return default; }
        public System.ArraySegment<byte> WriteMessage(System.ServiceModel.Channels.Message message, int maxMessageSize, System.ServiceModel.Channels.BufferManager bufferManager) { return default; }
        public abstract System.ArraySegment<byte> WriteMessage(System.ServiceModel.Channels.Message message, int maxMessageSize, System.ServiceModel.Channels.BufferManager bufferManager, int messageOffset);
        public abstract void WriteMessage(System.ServiceModel.Channels.Message message, System.IO.Stream stream);
    }
    public abstract partial class MessageEncoderFactory
    {
        protected MessageEncoderFactory() { }
        public abstract System.ServiceModel.Channels.MessageEncoder Encoder { get; }
        public abstract System.ServiceModel.Channels.MessageVersion MessageVersion { get; }
        public virtual System.ServiceModel.Channels.MessageEncoder CreateSessionEncoder() { return default; }
    }
    public abstract partial class MessageEncodingBindingElement : System.ServiceModel.Channels.BindingElement
    {
        protected MessageEncodingBindingElement() { }
        protected MessageEncodingBindingElement(System.ServiceModel.Channels.MessageEncodingBindingElement elementToBeCloned) { }
        public abstract System.ServiceModel.Channels.MessageVersion MessageVersion { get; set; }
        public abstract System.ServiceModel.Channels.MessageEncoderFactory CreateMessageEncoderFactory();
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public abstract partial class MessageFault
    {
        protected MessageFault() { }
        public virtual string Actor { get { return default; } }
        public abstract System.ServiceModel.FaultCode Code { get; }
        public abstract bool HasDetail { get; }
        public virtual string Node { get { return default; } }
        public abstract System.ServiceModel.FaultReason Reason { get; }
        public static System.ServiceModel.Channels.MessageFault CreateFault(System.ServiceModel.Channels.Message message, int maxBufferSize) { return default; }
        public T GetDetail<T>() { return default; }
        public T GetDetail<T>(System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public System.Xml.XmlDictionaryReader GetReaderAtDetailContents() { return default; }
        protected virtual System.Xml.XmlDictionaryReader OnGetReaderAtDetailContents() { return default; }
        protected virtual void OnWriteDetail(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.EnvelopeVersion version) { }
        protected abstract void OnWriteDetailContents(System.Xml.XmlDictionaryWriter writer);
        protected virtual void OnWriteStartDetail(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.EnvelopeVersion version) { }
    }
    public abstract partial class MessageHeader : System.ServiceModel.Channels.MessageHeaderInfo
    {
        protected MessageHeader() { }
        public override string Actor { get { return default; } }
        public override bool IsReferenceParameter { get { return default; } }
        public override bool MustUnderstand { get { return default; } }
        public override bool Relay { get { return default; } }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand, string actor) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand, string actor, bool relay) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand, string actor) { return default; }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand, string actor, bool relay) { return default; }
        public virtual bool IsMessageVersionSupported(System.ServiceModel.Channels.MessageVersion messageVersion) { return default; }
        protected abstract void OnWriteHeaderContents(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion);
        protected virtual void OnWriteStartHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public override string ToString() { return default; }
        public void WriteHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteHeader(System.Xml.XmlWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        protected void WriteHeaderAttributes(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteHeaderContents(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteStartHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
    }
    public abstract partial class MessageHeaderInfo
    {
        protected MessageHeaderInfo() { }
        public abstract string Actor { get; }
        public abstract bool IsReferenceParameter { get; }
        public abstract bool MustUnderstand { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract bool Relay { get; }
    }
    public sealed partial class MessageHeaders : System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.MessageHeaderInfo>, System.Collections.IEnumerable
    {
        public MessageHeaders(System.ServiceModel.Channels.MessageHeaders collection) { }
        public MessageHeaders(System.ServiceModel.Channels.MessageVersion version) { }
        public MessageHeaders(System.ServiceModel.Channels.MessageVersion version, int initialSize) { }
        public string Action { get { return default; } set { } }
        public int Count { get { return default; } }
        public System.ServiceModel.EndpointAddress FaultTo { get { return default; } set { } }
        public System.ServiceModel.EndpointAddress From { get { return default; } set { } }
        public System.ServiceModel.Channels.MessageHeaderInfo this[int index] { get { return default; } }
        public System.Xml.UniqueId MessageId { get { return default; } set { } }
        public System.ServiceModel.Channels.MessageVersion MessageVersion { get { return default; } }
        public System.ServiceModel.Channels.UnderstoodHeaders UnderstoodHeaders { get { return default; } }
        public System.Xml.UniqueId RelatesTo { get { return default; } set { } }
        public System.ServiceModel.EndpointAddress ReplyTo { get { return default; } set { } }
        public System.Uri To { get { return default; } set { } }
        public void Add(System.ServiceModel.Channels.MessageHeader header) { }
        public void Clear() { }
        public void CopyHeaderFrom(System.ServiceModel.Channels.Message message, int headerIndex) { }
        public void CopyHeaderFrom(System.ServiceModel.Channels.MessageHeaders collection, int headerIndex) { }
        public void CopyHeadersFrom(System.ServiceModel.Channels.Message message) { }
        public void CopyHeadersFrom(System.ServiceModel.Channels.MessageHeaders collection) { }
        public void CopyTo(System.ServiceModel.Channels.MessageHeaderInfo[] array, int index) { }
        public int FindHeader(string name, string ns) { return default; }
        public int FindHeader(string name, string ns, params string[] actors) { return default; }
        public System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo> GetEnumerator() { return default; }
        public T GetHeader<T>(int index) { return default; }
        public T GetHeader<T>(int index, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public T GetHeader<T>(string name, string ns) { return default; }
        public T GetHeader<T>(string name, string ns, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default; }
        public T GetHeader<T>(string name, string ns, params string[] actors) { return default; }
        public System.Xml.XmlDictionaryReader GetReaderAtHeader(int headerIndex) { return default; }
        public bool HaveMandatoryHeadersBeenUnderstood() { return default; }
        public bool HaveMandatoryHeadersBeenUnderstood(params string[] actors) { return default; }
        public void Insert(int headerIndex, System.ServiceModel.Channels.MessageHeader header) { }
        public void RemoveAll(string name, string ns) { }
        public void RemoveAt(int headerIndex) { }
        public void SetAction(System.Xml.XmlDictionaryString action) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default; }
        public void WriteHeader(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteHeader(int headerIndex, System.Xml.XmlWriter writer) { }
        public void WriteHeaderContents(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteHeaderContents(int headerIndex, System.Xml.XmlWriter writer) { }
        public void WriteStartHeader(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartHeader(int headerIndex, System.Xml.XmlWriter writer) { }
    }
    public sealed partial class MessageProperties : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable, System.IDisposable
    {
        public MessageProperties() { }
        public MessageProperties(System.ServiceModel.Channels.MessageProperties properties) { }
        public bool AllowOutputBatching { get { return default; } set { } }
        public int Count { get { return default; } }
        public System.ServiceModel.Channels.MessageEncoder Encoder { get { return default; } set { } }
        public bool IsFixedSize { get { return default; } }
        public object this[string name] { get { return default; } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { return default; } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Object>>.IsReadOnly { get { return default; } }
        public System.ServiceModel.Security.SecurityMessageProperty Security { get { return default; } set { } }
        public System.Collections.Generic.ICollection<object> Values { get { return default; } }
        public System.Uri Via { get { return default; } set { } }
        public void Add(string name, object property) { }
        public void Clear() { }
        public bool ContainsKey(string name) { return default; }
        public void CopyProperties(System.ServiceModel.Channels.MessageProperties properties) { }
        public void Dispose() { }
        public bool Remove(string name) { return default; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> pair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> pair) { return default; }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] array, int index) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> pair) { return default; }
        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Object>>.GetEnumerator() { return default; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default; }
        public bool TryGetValue(string name, out object value) { value = default; return default; }
    }
    public enum MessageState
    {
        Closed = 4,
        Copied = 3,
        Created = 0,
        Read = 1,
        Written = 2,
    }
    public sealed partial class MessageVersion
    {
        internal MessageVersion() { }
        public System.ServiceModel.Channels.AddressingVersion Addressing { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Default { get { return default; } }
        public System.ServiceModel.EnvelopeVersion Envelope { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion None { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap11 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap11WSAddressing10 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap11WSAddressingAugust2004 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap12 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap12WSAddressing10 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion Soap12WSAddressingAugust2004 { get { return default; } }
        public static System.ServiceModel.Channels.MessageVersion CreateVersion(System.ServiceModel.EnvelopeVersion envelopeVersion) { return default; }
        public static System.ServiceModel.Channels.MessageVersion CreateVersion(System.ServiceModel.EnvelopeVersion envelopeVersion, System.ServiceModel.Channels.AddressingVersion addressingVersion) { return default; }
        public override bool Equals(object obj) { return default; }
        public override int GetHashCode() { return default; }
        public override string ToString() { return default; }
    }
    public sealed class MtomMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        public MtomMessageEncodingBindingElement() { }
        public MtomMessageEncodingBindingElement(MessageVersion messageVersion, System.Text.Encoding writeEncoding) { }
        [System.ComponentModel.DefaultValueAttribute(64)]
        public int MaxReadPoolSize { get; set; }
        [System.ComponentModel.DefaultValueAttribute(16)]
        public int MaxWritePoolSize { get; set; }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get; set; }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get; set; }
        public System.Text.Encoding WriteEncoding { get; set; }
        public override MessageVersion MessageVersion { get; set; }
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) { return default; }
        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) { return default; }
        public override BindingElement Clone() { return default; }
        public override MessageEncoderFactory CreateMessageEncoderFactory() { return default; }
        public override T GetProperty<T>(BindingContext context) { return default; }
    }
    public sealed class ReliableSessionBindingElement : System.ServiceModel.Channels.BindingElement
    {
        public ReliableSessionBindingElement() { }
        public ReliableSessionBindingElement(bool ordered) { }
        public System.TimeSpan AcknowledgementInterval { get; set; }
        public bool FlowControlEnabled { get; set; }
        public System.TimeSpan InactivityTimeout { get; set; }
        public int MaxPendingChannels { get; set; }
        public int MaxRetryCount { get; set; }
        public int MaxTransferWindowSize { get; set; }
        public bool Ordered { get; set; }
        public System.ServiceModel.ReliableMessagingVersion ReliableMessagingVersion { get; set; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public abstract partial class RequestContext : System.IDisposable
    {
        protected RequestContext() { }
        public abstract System.ServiceModel.Channels.Message RequestMessage { get; }
        public abstract void Abort();
        public abstract System.IAsyncResult BeginReply(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state);
        public abstract System.IAsyncResult BeginReply(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state);
        public abstract void Close();
        public abstract void Close(System.TimeSpan timeout);
        protected virtual void Dispose(bool disposing) { }
        public abstract void EndReply(System.IAsyncResult result);
        public abstract void Reply(System.ServiceModel.Channels.Message message);
        public abstract void Reply(System.ServiceModel.Channels.Message message, System.TimeSpan timeout);
        void System.IDisposable.Dispose() { }
    }
    public sealed partial class TextMessageEncodingBindingElement : System.ServiceModel.Channels.MessageEncodingBindingElement
    {
        public TextMessageEncodingBindingElement() { }
        public TextMessageEncodingBindingElement(System.ServiceModel.Channels.MessageVersion messageVersion, System.Text.Encoding writeEncoding) { }
        public override System.ServiceModel.Channels.MessageVersion MessageVersion { get { return default; } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default; } set { } }
        public System.Text.Encoding WriteEncoding { get { return default; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override System.ServiceModel.Channels.MessageEncoderFactory CreateMessageEncoderFactory() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public abstract partial class TransportBindingElement : System.ServiceModel.Channels.BindingElement
    {
        protected TransportBindingElement() { }
        protected TransportBindingElement(System.ServiceModel.Channels.TransportBindingElement elementToBeCloned) { }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public virtual bool ManualAddressing { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((long)65536)]
        public virtual long MaxReceivedMessageSize { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((long)512 * 1024)]
        public virtual long MaxBufferPoolSize { get { return default; } set { } }
        public abstract string Scheme { get; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public sealed partial class UnderstoodHeaders : System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.MessageHeaderInfo>
    {
        internal UnderstoodHeaders() { }
        public void Add(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { }
        public bool Contains(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { return default; }
        public System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo> GetEnumerator() { return default; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default; }
        public void Remove(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { }
    }
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
        public System.ServiceModel.Security.SecurityKeyEntropyMode KeyEntropyMode { get { return default; } set { } }
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
namespace System.ServiceModel.Description
{
    public partial class CallbackDebugBehavior : System.ServiceModel.Description.IEndpointBehavior
    {
        public CallbackDebugBehavior(bool includeExceptionDetailInFaults) { }
        public bool IncludeExceptionDetailInFaults { get { return default; } set { } }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime behavior) { }
        void System.ServiceModel.Description.IEndpointBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) { }
        void System.ServiceModel.Description.IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) { }
    }
    public partial class ClientCredentials : System.ServiceModel.Security.SecurityCredentialsManager, System.ServiceModel.Description.IEndpointBehavior
    {
        public ClientCredentials() { }
        protected ClientCredentials(System.ServiceModel.Description.ClientCredentials other) { }
        public System.ServiceModel.Security.X509CertificateInitiatorClientCredential ClientCertificate { get { return default; } }
        public System.ServiceModel.Security.HttpDigestClientCredential HttpDigest { get { return default; } }
        public System.ServiceModel.Security.X509CertificateRecipientClientCredential ServiceCertificate { get { return default; } }
        public System.ServiceModel.Security.UserNamePasswordClientCredential UserName { get { return default; } }
        public System.ServiceModel.Security.WindowsClientCredential Windows { get { return default; } }
        public override System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager() { return default; }
        public virtual void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.ClientRuntime behavior) { }
        public System.ServiceModel.Description.ClientCredentials Clone() { return default; }
        protected virtual System.ServiceModel.Description.ClientCredentials CloneCore() { return default; }
        void System.ServiceModel.Description.IEndpointBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void System.ServiceModel.Description.IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher) { }
        void System.ServiceModel.Description.IEndpointBehavior.Validate(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) { }
    }
    public partial class ContractDescription
    {
        public ContractDescription(string name) { }
        public ContractDescription(string name, string ns) { }
        public System.Type CallbackContractType { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string ConfigurationName { get { return default; } set { } }
        public System.Collections.ObjectModel.KeyedCollection<System.Type, System.ServiceModel.Description.IContractBehavior> ContractBehaviors { get { return default; } }
        public System.Type ContractType { get { return default; } set { } }
        public static System.ServiceModel.Description.ContractDescription GetContract(System.Type contractType) { return default; }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
        public System.ServiceModel.Description.OperationDescriptionCollection Operations { get { return default; } }
    }
    public partial class DataContractSerializerOperationBehavior : System.ServiceModel.Description.IOperationBehavior
    {
        public DataContractSerializerOperationBehavior(System.ServiceModel.Description.OperationDescription operation) { }
        public DataContractSerializerOperationBehavior(System.ServiceModel.Description.OperationDescription operation, System.ServiceModel.DataContractFormatAttribute dataContractFormatAttribute) { }
        public System.ServiceModel.DataContractFormatAttribute DataContractFormatAttribute { get { return default; } }
        public System.Runtime.Serialization.DataContractResolver DataContractResolver { get { return default; } set { } }
        public int MaxItemsInObjectGraph { get { return default; } set { } }
        public virtual System.Runtime.Serialization.XmlObjectSerializer CreateSerializer(System.Type type, string name, string ns, System.Collections.Generic.IList<System.Type> knownTypes) { return default; }
        public virtual System.Runtime.Serialization.XmlObjectSerializer CreateSerializer(System.Type type, System.Xml.XmlDictionaryString name, System.Xml.XmlDictionaryString ns, System.Collections.Generic.IList<System.Type> knownTypes) { return default; }
        void System.ServiceModel.Description.IOperationBehavior.AddBindingParameters(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyClientBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch) { }
        void System.ServiceModel.Description.IOperationBehavior.Validate(System.ServiceModel.Description.OperationDescription description) { }
    }
    public partial class FaultDescription
    {
        public FaultDescription(string action) { }
        public string Action { get { return default; } }
        public System.Type DetailType { get { return default; } set { } }
        public string Name { get { return default; } set { } }
        public string Namespace { get { return default; } set { } }
    }
    public partial class FaultDescriptionCollection : System.Collections.ObjectModel.Collection<System.ServiceModel.Description.FaultDescription>
    {
        internal FaultDescriptionCollection() { }
    }
    public partial interface IContractBehavior
    {
        void AddBindingParameters(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters);
        void ApplyClientBehavior(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime);
        void ApplyDispatchBehavior(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime);
        void Validate(System.ServiceModel.Description.ContractDescription contractDescription, System.ServiceModel.Description.ServiceEndpoint endpoint);
    }
    public partial interface IEndpointBehavior
    {
        void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters);
        void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime);
        void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher);
        void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint);
    }
    public partial interface IOperationBehavior
    {
        void AddBindingParameters(System.ServiceModel.Description.OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters);
        void ApplyClientBehavior(System.ServiceModel.Description.OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation);
        void ApplyDispatchBehavior(System.ServiceModel.Description.OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation);
        void Validate(System.ServiceModel.Description.OperationDescription operationDescription);
    }
    public partial class MessageBodyDescription
    {
        public MessageBodyDescription() { }
        public System.ServiceModel.Description.MessagePartDescriptionCollection Parts { get { return default; } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public System.ServiceModel.Description.MessagePartDescription ReturnValue { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string WrapperName { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string WrapperNamespace { get { return default; } set { } }
    }
    public partial class MessageDescription
    {
        public MessageDescription(string action, System.ServiceModel.Description.MessageDirection direction) { }
        public string Action { get { return default; } }
        public System.ServiceModel.Description.MessageBodyDescription Body { get { return default; } }
        public System.ServiceModel.Description.MessageDirection Direction { get { return default; } }
        public System.ServiceModel.Description.MessageHeaderDescriptionCollection Headers { get { return default; } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public System.Type MessageType { get { return default; } set { } }
        public System.ServiceModel.Description.MessagePropertyDescriptionCollection Properties { get { return default; } }
    }
    public partial class MessageDescriptionCollection : System.Collections.ObjectModel.Collection<System.ServiceModel.Description.MessageDescription>
    {
        internal MessageDescriptionCollection() { }
        public System.ServiceModel.Description.MessageDescription Find(string action) { return default; }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Description.MessageDescription> FindAll(string action) { return default; }
    }
    public enum MessageDirection
    {
        Input = 0,
        Output = 1,
    }
    public partial class MessageHeaderDescription : System.ServiceModel.Description.MessagePartDescription
    {
        public MessageHeaderDescription(string name, string ns) : base(default, default) { }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string Actor { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool MustUnderstand { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool Relay { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool TypedHeader { get { return default; } set { } }
    }
    public partial class MessageHeaderDescriptionCollection : System.Collections.ObjectModel.KeyedCollection<System.Xml.XmlQualifiedName, System.ServiceModel.Description.MessageHeaderDescription>
    {
        internal MessageHeaderDescriptionCollection() { }
        protected override System.Xml.XmlQualifiedName GetKeyForItem(System.ServiceModel.Description.MessageHeaderDescription item) { return default; }
    }
    public partial class MessagePartDescription
    {
        public MessagePartDescription(string name, string ns) { }
        public int Index { get { return default; } set { } }
        public System.Reflection.MemberInfo MemberInfo { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool Multiple { get { return default; } set { } }
        public string Name { get { return default; } }
        public string Namespace { get { return default; } }
        public System.Type Type { get { return default; } set { } }
    }
    public partial class MessagePartDescriptionCollection : System.Collections.ObjectModel.KeyedCollection<System.Xml.XmlQualifiedName, System.ServiceModel.Description.MessagePartDescription>
    {
        internal MessagePartDescriptionCollection() { }
        protected override System.Xml.XmlQualifiedName GetKeyForItem(System.ServiceModel.Description.MessagePartDescription item) { return default; }
    }
    public partial class MessagePropertyDescription : System.ServiceModel.Description.MessagePartDescription
    {
        public MessagePropertyDescription(string name) : base(default, default) { }
    }
    public partial class MessagePropertyDescriptionCollection : System.Collections.ObjectModel.KeyedCollection<string, System.ServiceModel.Description.MessagePropertyDescription>
    {
        internal MessagePropertyDescriptionCollection() { }
        protected override string GetKeyForItem(System.ServiceModel.Description.MessagePropertyDescription item) { return default; }
    }
    public partial class OperationDescription
    {
        public OperationDescription(string name, System.ServiceModel.Description.ContractDescription declaringContract) { }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public System.Collections.Generic.KeyedByTypeCollection<System.ServiceModel.Description.IOperationBehavior> Behaviors { get { return default; } }
        public System.ServiceModel.Description.ContractDescription DeclaringContract { get { return default; } set { } }
        public System.ServiceModel.Description.FaultDescriptionCollection Faults { get { return default; } }
        public bool IsOneWay { get { return default; } }
        public System.Collections.ObjectModel.Collection<System.Type> KnownTypes { get { return default; } }
        public System.ServiceModel.Description.MessageDescriptionCollection Messages { get { return default; } }
        public string Name { get { return default; } }
        public System.Collections.ObjectModel.KeyedCollection<System.Type, System.ServiceModel.Description.IOperationBehavior> OperationBehaviors { get { return default; } }
        public System.Reflection.MethodInfo TaskMethod { get { return default; } set { } }
        public System.Reflection.MethodInfo BeginMethod { get { return default; } set { } }
        public System.Reflection.MethodInfo EndMethod { get { return default; } set { } }
        public System.Reflection.MethodInfo SyncMethod { get { return default; } set { } }
    }
    public partial class OperationDescriptionCollection : System.Collections.ObjectModel.Collection<System.ServiceModel.Description.OperationDescription>
    {
        internal OperationDescriptionCollection() { }
        public System.ServiceModel.Description.OperationDescription Find(string name) { return default; }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Description.OperationDescription> FindAll(string name) { return default; }
        protected override void InsertItem(int index, System.ServiceModel.Description.OperationDescription item) { }
        protected override void SetItem(int index, System.ServiceModel.Description.OperationDescription item) { }
    }
    public partial class ServiceEndpoint
    {
        public ServiceEndpoint(System.ServiceModel.Description.ContractDescription contract) { }
        public ServiceEndpoint(System.ServiceModel.Description.ContractDescription contract, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address) { }
        public System.ServiceModel.EndpointAddress Address { get { return default; } set { } }
        public System.ServiceModel.Channels.Binding Binding { get { return default; } set { } }
        public System.ServiceModel.Description.ContractDescription Contract { get { return default; } set { } }
        public System.Collections.ObjectModel.KeyedCollection<System.Type, System.ServiceModel.Description.IEndpointBehavior> EndpointBehaviors { get { return default; } }
        public string Name { get { return default; } set { } }
    }
    public partial class XmlSerializerOperationBehavior : System.ServiceModel.Description.IOperationBehavior
    {
        public XmlSerializerOperationBehavior(System.ServiceModel.Description.OperationDescription operation) { }
        public XmlSerializerOperationBehavior(System.ServiceModel.Description.OperationDescription operation, System.ServiceModel.XmlSerializerFormatAttribute attribute) { }
        public System.ServiceModel.XmlSerializerFormatAttribute XmlSerializerFormatAttribute { get { return default; } }
        public System.Collections.ObjectModel.Collection<System.Xml.Serialization.XmlMapping> GetXmlMappings() { throw null; }
        void System.ServiceModel.Description.IOperationBehavior.Validate(System.ServiceModel.Description.OperationDescription description) { }
        void System.ServiceModel.Description.IOperationBehavior.AddBindingParameters(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyClientBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy) { }
    }
    public abstract partial class TypedMessageConverter
    {
        public static TypedMessageConverter Create(Type messageContract, string action) { return default; }
        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace) { return default; }
        public static TypedMessageConverter Create(Type messageContract, string action, XmlSerializerFormatAttribute formatterAttribute) { return default; }
        public static TypedMessageConverter Create(Type messageContract, string action, DataContractFormatAttribute formatterAttribute) { return default; }
        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace, XmlSerializerFormatAttribute formatterAttribute) { return default; }
        public static TypedMessageConverter Create(Type messageContract, string action, string defaultNamespace, DataContractFormatAttribute formatterAttribute) { return default; }
        public abstract System.ServiceModel.Channels.Message ToMessage(object typedMessage);
        public abstract System.ServiceModel.Channels.Message ToMessage(object typedMessage, System.ServiceModel.Channels.MessageVersion version);
        public abstract object FromMessage(System.ServiceModel.Channels.Message message);
    }
}
namespace System.ServiceModel.Dispatcher
{
    public sealed partial class ClientOperation
    {
        public ClientOperation(System.ServiceModel.Dispatcher.ClientRuntime parent, string name, string action) { }
        public ClientOperation(System.ServiceModel.Dispatcher.ClientRuntime parent, string name, string action, string replyAction) { }
        public string Action { get { return default; } }
        public System.Collections.Generic.SynchronizedCollection<FaultContractInfo> FaultContractInfos { get { return default; } }
        public System.Collections.Generic.ICollection<System.ServiceModel.Dispatcher.IParameterInspector> ClientParameterInspectors { get { return default; } }
        public bool DeserializeReply { get { return default; } set { } }
        public System.ServiceModel.Dispatcher.IClientMessageFormatter Formatter { get { return default; } set { } }
        public bool IsOneWay { get { return default; } set { } }
        public string Name { get { return default; } }
        public System.ServiceModel.Dispatcher.ClientRuntime Parent { get { return default; } }
        public string ReplyAction { get { return default; } }
        public bool SerializeRequest { get { return default; } set { } }
        public System.Reflection.MethodInfo TaskMethod { get { return default; } set { } }
        public System.Type TaskTResult { get { return default; } set { } }
        public System.Reflection.MethodInfo BeginMethod { get { return default; } set { } }
        public System.Reflection.MethodInfo EndMethod { get { return default; } set { } }
        public System.Reflection.MethodInfo SyncMethod { get { return default; } set { } }
    }
    public sealed partial class ClientRuntime
    {
        internal ClientRuntime() { }
        public System.Collections.Generic.SynchronizedCollection<IChannelInitializer> ChannelInitializers { get { return default; } }
        public System.Collections.Generic.ICollection<System.ServiceModel.Dispatcher.IClientMessageInspector> ClientMessageInspectors { get { return default; } }
        public System.ServiceModel.Dispatcher.DispatchRuntime CallbackDispatchRuntime { get { return default; } }
        public System.Collections.Generic.ICollection<System.ServiceModel.Dispatcher.ClientOperation> ClientOperations { get { return default; } }
        public System.Type ContractClientType { get { return default; } set { } }
        public string ContractName { get { return default; } }
        public string ContractNamespace { get { return default; } }
        public System.Collections.Generic.SynchronizedCollection<IInteractiveChannelInitializer> InteractiveChannelInitializers { get { return default; } }
        public bool ManualAddressing { get { return default; } set { } }
        public int MaxFaultSize { get { return default; } set { } }
        public System.ServiceModel.Dispatcher.IClientOperationSelector OperationSelector { get { return default; } set { } }
        public System.ServiceModel.Dispatcher.ClientOperation UnhandledClientOperation { get { return default; } }
        public System.Uri Via { get { return default; } set { } }
    }
    public sealed partial class DispatchOperation
    {
        public DispatchOperation(System.ServiceModel.Dispatcher.DispatchRuntime parent, string name, string action) { }
        public string Action { get { return default; } }
        public bool AutoDisposeParameters { get { return default; } set { } }
        public bool DeserializeRequest { get { return default; } set { } }
        public bool IsOneWay { get { return default; } }
        public string Name { get { return default; } }
        public System.ServiceModel.Dispatcher.DispatchRuntime Parent { get { return default; } }
        public bool SerializeReply { get { return default; } set { } }
    }
    public sealed partial class DispatchRuntime
    {
        internal DispatchRuntime() { }
        public System.Collections.Generic.SynchronizedCollection<IDispatchMessageInspector> MessageInspectors { get { return default; } }
        public System.ServiceModel.Dispatcher.ChannelDispatcher ChannelDispatcher { get { return default; } }
    }
    public partial class ChannelDispatcher
    {
        internal ChannelDispatcher() { }
        public bool IncludeExceptionDetailInFaults { get { return default; } set { } }
    }
    public partial class EndpointDispatcher
    {
        internal EndpointDispatcher() { }
    }
    public partial class FaultContractInfo
    {
        public FaultContractInfo(string action, Type detail) { }
        public string Action { get { return default; } }
        public Type Detail { get { return default; } }
    }
    public partial interface IChannelInitializer
    {
        void Initialize(IClientChannel channel);
    }
    public partial interface IClientMessageFormatter
    {
        object DeserializeReply(System.ServiceModel.Channels.Message message, object[] parameters);
        System.ServiceModel.Channels.Message SerializeRequest(System.ServiceModel.Channels.MessageVersion messageVersion, object[] parameters);
    }
    public partial interface IClientMessageInspector
    {
        void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState);
        object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel);
    }
    public interface IDispatchMessageInspector
    {
        object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, System.ServiceModel.InstanceContext instanceContext);
        void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState);
    }
    public partial interface IClientOperationSelector
    {
        bool AreParametersRequiredForSelection { get; }
        string SelectOperation(System.Reflection.MethodBase method, object[] parameters);
    }
    public partial interface IInteractiveChannelInitializer
    {
        IAsyncResult BeginDisplayInitializationUI(IClientChannel channel, AsyncCallback callback, object state);
        void EndDisplayInitializationUI(IAsyncResult result);
    }
    public partial interface IParameterInspector
    {
        void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState);
        object BeforeCall(string operationName, object[] inputs);
    }
}
namespace System.ServiceModel.Security
{
    public sealed partial class HttpDigestClientCredential
    {
        internal HttpDigestClientCredential() { }
        public System.Net.NetworkCredential ClientCredential { get { return default; } set { } }
    }
    public interface ISecuritySession : System.ServiceModel.Channels.ISession
    {
        System.ServiceModel.EndpointIdentity RemoteIdentity { get; }
    }
    public partial class MessageSecurityException : System.ServiceModel.CommunicationException
    {
        public MessageSecurityException(string message) { }
        public MessageSecurityException(string message, System.Exception innerException) { }
        protected MessageSecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public partial class SecurityAccessDeniedException : System.ServiceModel.CommunicationException
    {
        public SecurityAccessDeniedException(string message) { }
        public SecurityAccessDeniedException(string message, System.Exception innerException) { }
        protected SecurityAccessDeniedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public abstract partial class SecurityCredentialsManager
    {
        protected SecurityCredentialsManager() { }
        public abstract System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager();
    }
    public partial class SecurityNegotiationException : System.ServiceModel.CommunicationException
    {
        public SecurityNegotiationException() { }
        public SecurityNegotiationException(string message) { }
        public SecurityNegotiationException(string message, System.Exception innerException) { }
        protected SecurityNegotiationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
    public sealed partial class UserNamePasswordClientCredential
    {
        internal UserNamePasswordClientCredential() { }
        public string Password { get { return default; } set { } }
        public string UserName { get { return default; } set { } }
    }
    public sealed partial class WindowsClientCredential
    {
        internal WindowsClientCredential() { }
        public System.Security.Principal.TokenImpersonationLevel AllowedImpersonationLevel { get { return default; } set { } }
        public System.Net.NetworkCredential ClientCredential { get { return default; } set { } }
    }
    public sealed partial class X509ServiceCertificateAuthentication
    {
        public X509ServiceCertificateAuthentication() { }
        public System.ServiceModel.Security.X509CertificateValidationMode CertificateValidationMode { get { return default; } set { } }
        public System.IdentityModel.Selectors.X509CertificateValidator CustomCertificateValidator { get { return default; } set { } }
        public System.Security.Cryptography.X509Certificates.X509RevocationMode RevocationMode { get { return default; } set { } }
        public System.Security.Cryptography.X509Certificates.StoreLocation TrustedStoreLocation { get { return default; } set { } }
    }
    public sealed partial class X509CertificateInitiatorClientCredential
    {
        internal X509CertificateInitiatorClientCredential() { }
        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate { get { return default; } set { } }
        public void SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName, System.Security.Cryptography.X509Certificates.X509FindType findType, object findValue) { }
        public void SetCertificate(string subjectName, System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName) { }
    }
    public sealed partial class X509CertificateRecipientClientCredential
    {
        internal X509CertificateRecipientClientCredential() { }
        public System.ServiceModel.Security.X509ServiceCertificateAuthentication Authentication { get { return default; } }
        public System.Security.Cryptography.X509Certificates.X509Certificate2 DefaultCertificate { get { return default; } set { } }
        public System.Collections.Generic.Dictionary<System.Uri, System.Security.Cryptography.X509Certificates.X509Certificate2> ScopedCertificates { get { return default; } }
        public System.ServiceModel.Security.X509ServiceCertificateAuthentication SslCertificateAuthentication { get { return default; } set { } }
        public void SetDefaultCertificate(System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName, System.Security.Cryptography.X509Certificates.X509FindType findType, object findValue) { }
        public void SetDefaultCertificate(string subjectName, System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName) { }
        public void SetScopedCertificate(System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName, System.Security.Cryptography.X509Certificates.X509FindType findType, object findValue, System.Uri targetService) { }
        public void SetScopedCertificate(string subjectName, System.Security.Cryptography.X509Certificates.StoreLocation storeLocation, System.Security.Cryptography.X509Certificates.StoreName storeName, System.Uri targetService) { }
    }
    public enum X509CertificateValidationMode
    {
        ChainTrust = 2,
        Custom = 4,
        None = 0,
        PeerOrChainTrust = 3,
        PeerTrust = 1,
    }
    internal interface ISecurityCommunicationObject
    {
        TimeSpan DefaultOpenTimeout { get; }
        TimeSpan DefaultCloseTimeout { get; }
        void OnAbort();
        Threading.Tasks.Task OnCloseAsync(TimeSpan timeout);
        void OnClosed();
        void OnClosing();
        void OnFaulted();
        Threading.Tasks.Task OnOpenAsync(TimeSpan timeout);
        void OnOpened();
        void OnOpening();
    }
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
