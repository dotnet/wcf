// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



using System.Net.Security;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Channels
{
    public sealed class TransportSecurityBindingElement : SecurityBindingElement
    {
        public TransportSecurityBindingElement()
            : base()
        {
        }

        private TransportSecurityBindingElement(TransportSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            // empty
        }

        internal override ISecurityCapabilities GetIndividualISecurityCapabilities()
        {
            bool supportsClientAuthentication;
            bool supportsClientWindowsIdentity;
            GetSupportingTokensCapabilities(out supportsClientAuthentication, out supportsClientWindowsIdentity);
            return new SecurityCapabilities(supportsClientAuthentication, false, supportsClientWindowsIdentity,
                ProtectionLevel.None, ProtectionLevel.None);
        }

        internal override bool SessionMode
        {
            get
            {
                return false;
            }
        }

        internal override bool SupportsDuplex
        {
            get { return true; }
        }

        internal override bool SupportsRequestReply
        {
            get { return true; }
        }

        internal override SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            if (credentialsManager == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("credentialsManager");

            TransportSecurityProtocolFactory protocolFactory = new TransportSecurityProtocolFactory();
            if (isForService)
                base.ApplyAuditBehaviorSettings(context, protocolFactory);
            base.ConfigureProtocolFactory(protocolFactory, credentialsManager, isForService, issuerBindingContext, context.Binding);
            protocolFactory.DetectReplays = false;

            return protocolFactory;
        }

        protected override IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context)
        {
            ISecurityCapabilities securityCapabilities = this.GetProperty<ISecurityCapabilities>(context);
            SecurityCredentialsManager credentialsManager = context.BindingParameters.Find<SecurityCredentialsManager>();
            if (credentialsManager == null)
            {
                credentialsManager = ClientCredentials.CreateDefaultCredentials();
            }

            SecureConversationSecurityTokenParameters scParameters = null;
            if (this.EndpointSupportingTokenParameters.Endorsing.Count > 0)
            {
                scParameters = this.EndpointSupportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
            }

            // This adds the demuxer element to the context

            bool requireDemuxer = RequiresChannelDemuxer();
            ChannelBuilder channelBuilder = new ChannelBuilder(context, requireDemuxer);

            if (requireDemuxer)
            {
                throw ExceptionHelper.PlatformNotSupported("TransportSecurityBindingElement demuxing is not supported"); // Issue #31 in progress
                // ApplyPropertiesOnDemuxer(channelBuilder, context);
            }
            BindingContext issuerBindingContext = context.Clone();

            SecurityChannelFactory<TChannel> channelFactory;
            if (scParameters != null)
            {
                if (scParameters.BootstrapSecurityBindingElement == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SecureConversationSecurityTokenParametersRequireBootstrapBinding)));

                scParameters.IssuerBindingContext = issuerBindingContext;
                if (scParameters.RequireCancellation)
                {
                    throw ExceptionHelper.PlatformNotSupported("TransportSecurityBindingElement RequireCancellation is not supported"); // Issue #31 in progress

                    //SessionSymmetricTransportSecurityProtocolFactory sessionFactory = new SessionSymmetricTransportSecurityProtocolFactory();
                    //sessionFactory.SecurityTokenParameters = scParameters.Clone();
                    //((SecureConversationSecurityTokenParameters)sessionFactory.SecurityTokenParameters).IssuerBindingContext = issuerBindingContext;
                    //this.EndpointSupportingTokenParameters.Endorsing.RemoveAt(0);
                    //try
                    //{
                    //    base.ConfigureProtocolFactory(sessionFactory, credentialsManager, false, issuerBindingContext, context.Binding);
                    //}
                    //finally
                    //{
                    //    this.EndpointSupportingTokenParameters.Endorsing.Insert(0, scParameters);
                    //}

                    //SecuritySessionClientSettings<TChannel> sessionClientSettings = new SecuritySessionClientSettings<TChannel>();
                    //sessionClientSettings.ChannelBuilder = channelBuilder;
                    //sessionClientSettings.KeyRenewalInterval = this.LocalClientSettings.SessionKeyRenewalInterval;
                    //sessionClientSettings.KeyRolloverInterval = this.LocalClientSettings.SessionKeyRolloverInterval;
                    //sessionClientSettings.TolerateTransportFailures = this.LocalClientSettings.ReconnectTransportOnFailure;
                    //sessionClientSettings.CanRenewSession = scParameters.CanRenewSession;
                    //sessionClientSettings.IssuedSecurityTokenParameters = scParameters.Clone();
                    //((SecureConversationSecurityTokenParameters)sessionClientSettings.IssuedSecurityTokenParameters).IssuerBindingContext = issuerBindingContext;
                    //sessionClientSettings.SecurityStandardsManager = sessionFactory.StandardsManager;
                    //sessionClientSettings.SessionProtocolFactory = sessionFactory;
                    //channelFactory = new SecurityChannelFactory<TChannel>(securityCapabilities, context, sessionClientSettings);
                }
                else
                {
                    TransportSecurityProtocolFactory protocolFactory = new TransportSecurityProtocolFactory();
                    this.EndpointSupportingTokenParameters.Endorsing.RemoveAt(0);
                    try
                    {
                        base.ConfigureProtocolFactory(protocolFactory, credentialsManager, false, issuerBindingContext, context.Binding);
                        SecureConversationSecurityTokenParameters acceleratedTokenParameters = (SecureConversationSecurityTokenParameters)scParameters.Clone();
                        acceleratedTokenParameters.IssuerBindingContext = issuerBindingContext;
                        protocolFactory.SecurityBindingElement.EndpointSupportingTokenParameters.Endorsing.Insert(0, acceleratedTokenParameters);
                    }
                    finally
                    {
                        this.EndpointSupportingTokenParameters.Endorsing.Insert(0, scParameters);
                    }

                    channelFactory = new SecurityChannelFactory<TChannel>(securityCapabilities, context, channelBuilder, protocolFactory);
                }
            }
            else
            {
                SecurityProtocolFactory protocolFactory = this.CreateSecurityProtocolFactory<TChannel>(
                    context, credentialsManager, false, issuerBindingContext);
                channelFactory = new SecurityChannelFactory<TChannel>(securityCapabilities, context, channelBuilder, protocolFactory);
            }

            return channelFactory;

        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                throw ExceptionHelper.PlatformNotSupported("TransportSecurityBindingElement doesn't support ChannelProtectionRequirements yet.");
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        public override BindingElement Clone()
        {
            return new TransportSecurityBindingElement(this);
        }
    }
}
