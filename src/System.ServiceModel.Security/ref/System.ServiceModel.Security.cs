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
    public partial class SecureConversationSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        public SecureConversationSecurityTokenParameters() { }
        public SecureConversationSecurityTokenParameters(System.ServiceModel.Channels.SecurityBindingElement bootstrapSecurityBindingElement) { }
        public System.ServiceModel.Channels.SecurityBindingElement BootstrapSecurityBindingElement { get { return default(System.ServiceModel.Channels.SecurityBindingElement); } set { } }
    }
    public abstract partial class SecurityTokenParameters
    {
        internal SecurityTokenParameters() { }
        public System.ServiceModel.Security.Tokens.SecurityTokenParameters Clone() { return default(System.ServiceModel.Security.Tokens.SecurityTokenParameters); }
    }
    public partial class SupportingTokenParameters
    {
        public SupportingTokenParameters() { }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> Endorsing { get { return default(System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters>); } }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters> SignedEncrypted { get { return default(System.Collections.ObjectModel.Collection<System.ServiceModel.Security.Tokens.SecurityTokenParameters>); } }
        public System.ServiceModel.Security.Tokens.SupportingTokenParameters Clone() { return default(System.ServiceModel.Security.Tokens.SupportingTokenParameters); }
    }
    public partial class UserNameSecurityTokenParameters : System.ServiceModel.Security.Tokens.SecurityTokenParameters
    {
        public UserNameSecurityTokenParameters() { }
    }
}
