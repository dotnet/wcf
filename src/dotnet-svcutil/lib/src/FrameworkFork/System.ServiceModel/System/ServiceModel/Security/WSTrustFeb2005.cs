// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Microsoft.Xml;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Security.Tokens;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Runtime.Serialization;
    using System.ServiceModel.Dispatcher;

    using KeyIdentifierEntry = WSSecurityTokenSerializer.KeyIdentifierEntry;
    using KeyIdentifierClauseEntry = WSSecurityTokenSerializer.KeyIdentifierClauseEntry;
    using TokenEntry = WSSecurityTokenSerializer.TokenEntry;
    using StrEntry = WSSecurityTokenSerializer.StrEntry;

    internal class WSTrustFeb2005 : WSTrust
    {
        public WSTrustFeb2005(WSSecurityTokenSerializer tokenSerializer)
            : base(tokenSerializer)
        {
        }

        public override TrustDictionary SerializerDictionary
        {
            get { return XD.TrustFeb2005Dictionary; }
        }

        public class DriverFeb2005 : Driver
        {
            public DriverFeb2005(SecurityStandardsManager standardsManager)
                : base(standardsManager)
            {
            }

            public override TrustDictionary DriverDictionary
            {
                get
                {
                    return XD.TrustFeb2005Dictionary;
                }
            }

            public override XmlDictionaryString RequestSecurityTokenResponseFinalAction
            {
                get
                {
                    return XD.TrustFeb2005Dictionary.RequestSecurityTokenIssuanceResponse;
                }
            }

            public override bool IsSessionSupported
            {
                get
                {
                    return true;
                }
            }

            public override bool IsIssuedTokensSupported
            {
                get
                {
                    return true;
                }
            }

            public override string IssuedTokensHeaderName
            {
                get
                {
                    return this.DriverDictionary.IssuedTokensHeader.Value;
                }
            }

            public override string IssuedTokensHeaderNamespace
            {
                get
                {
                    return this.DriverDictionary.Namespace.Value;
                }
            }

            public override string RequestTypeRenew
            {
                get
                {
                    return this.DriverDictionary.RequestTypeRenew.Value;
                }
            }

            public override string RequestTypeClose
            {
                get
                {
                    return this.DriverDictionary.RequestTypeClose.Value;
                }
            }

            public override Collection<XmlElement> ProcessUnknownRequestParameters(Collection<XmlElement> unknownRequestParameters, Collection<XmlElement> originalRequestParameters)
            {
                return unknownRequestParameters;
            }

            protected override void ReadReferences(XmlElement rstrXml, out SecurityKeyIdentifierClause requestedAttachedReference,
                    out SecurityKeyIdentifierClause requestedUnattachedReference)
            {
                throw new NotImplementedException();
            }

            protected override bool ReadRequestedTokenClosed(XmlElement rstrXml)
            {
                for (int i = 0; i < rstrXml.ChildNodes.Count; ++i)
                {
                    XmlElement child = (rstrXml.ChildNodes[i] as XmlElement);
                    if (child != null)
                    {
                        if (child.LocalName == this.DriverDictionary.RequestedTokenClosed.Value && child.NamespaceURI == this.DriverDictionary.Namespace.Value)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            protected override void ReadTargets(XmlElement rstXml, out SecurityKeyIdentifierClause renewTarget, out SecurityKeyIdentifierClause closeTarget)
            {
                renewTarget = null;
                closeTarget = null;

                for (int i = 0; i < rstXml.ChildNodes.Count; ++i)
                {
                    XmlElement child = (rstXml.ChildNodes[i] as XmlElement);
                    if (child != null)
                    {
                        if (child.LocalName == this.DriverDictionary.RenewTarget.Value && child.NamespaceURI == this.DriverDictionary.Namespace.Value)
                            renewTarget = this.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(new XmlNodeReader(child.FirstChild));
                        else if (child.LocalName == this.DriverDictionary.CloseTarget.Value && child.NamespaceURI == this.DriverDictionary.Namespace.Value)
                            closeTarget = this.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(new XmlNodeReader(child.FirstChild));
                    }
                }
            }

            protected override void WriteReferences(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
            {
                if (rstr.RequestedAttachedReference != null)
                {
                    writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedAttachedReference, this.DriverDictionary.Namespace);
                    this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, rstr.RequestedAttachedReference);
                    writer.WriteEndElement();
                }

                if (rstr.RequestedUnattachedReference != null)
                {
                    writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedUnattachedReference, this.DriverDictionary.Namespace);
                    this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, rstr.RequestedUnattachedReference);
                    writer.WriteEndElement();
                }
            }

            protected override void WriteRequestedTokenClosed(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
            {
                if (rstr.IsRequestedTokenClosed)
                {
                    writer.WriteElementString(this.DriverDictionary.RequestedTokenClosed, this.DriverDictionary.Namespace, String.Empty);
                }
            }

            protected override void WriteTargets(RequestSecurityToken rst, XmlDictionaryWriter writer)
            {
                if (rst.RenewTarget != null)
                {
                    writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RenewTarget, this.DriverDictionary.Namespace);
                    this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, rst.RenewTarget);
                    writer.WriteEndElement();
                }

                if (rst.CloseTarget != null)
                {
                    writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.CloseTarget, this.DriverDictionary.Namespace);
                    this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, rst.CloseTarget);
                    writer.WriteEndElement();
                }
            }

            // this is now the abstract in WSTrust
            public override IChannelFactory<IRequestChannel> CreateFederationProxy(EndpointAddress address, Binding binding, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors)
            {
                if (channelBehaviors == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelBehaviors");

                ChannelFactory<IWsTrustFeb2005SecurityTokenService> result = new ChannelFactory<IWsTrustFeb2005SecurityTokenService>(binding, address);
                SetProtectionLevelForFederation(result.Endpoint.Contract.Operations);
                // remove the default client credentials that gets added to channel factories
                result.Endpoint.Behaviors.Remove<ClientCredentials>();
                for (int i = 0; i < channelBehaviors.Count; ++i)
                {
                    result.Endpoint.Behaviors.Add(channelBehaviors[i]);
                }
                // add a behavior that removes the UI channel initializer added by the client credentials since there should be no UI
                // initializer popped up as part of obtaining the federation token (the UI should already have been popped up for the main channel)
                result.Endpoint.Behaviors.Add(new InteractiveInitializersRemovingBehavior());

                return new RequestChannelFactory<IWsTrustFeb2005SecurityTokenService>(result);
            }

            [ServiceContract]
            internal interface IWsTrustFeb2005SecurityTokenService
            {
                [OperationContract(IsOneWay = false,
                                   Action = TrustFeb2005Strings.RequestSecurityTokenIssuance,
                                   ReplyAction = TrustFeb2005Strings.RequestSecurityTokenIssuanceResponse)]
                [FaultContract(typeof(string), Action = "*", ProtectionLevel = System.Net.Security.ProtectionLevel.Sign)]
                Message RequestToken(Message message);
            }

            public class InteractiveInitializersRemovingBehavior : IEndpointBehavior
            {
                public void Validate(ServiceEndpoint serviceEndpoint) { }
                public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters) { }
                public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { }
                public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
                {
                    // it is very unlikely that InteractiveChannelInitializers will be null, this is defensive in case ClientRuntime every has a 
                    // bug.  I am OK with this as ApplyingClientBehavior is a one-time channel setup.
                    if (behavior != null && behavior.InteractiveChannelInitializers != null)
                    {
                        // clear away any interactive initializer
                        behavior.InteractiveChannelInitializers.Clear();
                    }
                }
            }

            public class RequestChannelFactory<TokenService> : ChannelFactoryBase, IChannelFactory<IRequestChannel>
            {
                private ChannelFactory<TokenService> _innerChannelFactory;

                public RequestChannelFactory(ChannelFactory<TokenService> innerChannelFactory)
                {
                    _innerChannelFactory = innerChannelFactory;
                }

                public IRequestChannel CreateChannel(EndpointAddress address)
                {
                    throw new NotImplementedException();
                }

                public IRequestChannel CreateChannel(EndpointAddress address, Uri via)
                {
                    throw new NotImplementedException();
                }

                protected override void OnAbort()
                {
                    _innerChannelFactory.Abort();
                }

                protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return _innerChannelFactory.BeginOpen(timeout, callback, state);
                }

                protected override void OnEndOpen(IAsyncResult result)
                {
                    _innerChannelFactory.EndOpen(result);
                }

                protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    return _innerChannelFactory.BeginClose(timeout, callback, state);
                }

                protected override void OnEndClose(IAsyncResult result)
                {
                    _innerChannelFactory.EndClose(result);
                }

                protected override void OnClose(TimeSpan timeout)
                {
                    _innerChannelFactory.Close(timeout);
                }

                protected override void OnOpen(TimeSpan timeout)
                {
                    _innerChannelFactory.Open(timeout);
                }

                public override T GetProperty<T>()
                {
                    return _innerChannelFactory.GetProperty<T>();
                }
            }
        }
    }
}
