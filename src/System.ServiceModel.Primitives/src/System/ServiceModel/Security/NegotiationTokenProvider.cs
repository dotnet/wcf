// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Security
{
    // This is the base class for all token providers that negotiate an SCT from
    // the target service.
    internal abstract class NegotiationTokenProvider<T> : IssuanceTokenProviderBase<T>
        where T : IssuanceTokenProviderState
    {
        private IChannelFactory<IAsyncRequestChannel> _rstChannelFactory;
        private bool _requiresManualReplyAddressing;
        private BindingContext _issuanceBindingContext;
        private MessageVersion _messageVersion;

        protected NegotiationTokenProvider()
            : base()
        {
        }

        public BindingContext IssuerBindingContext
        {
            get { return _issuanceBindingContext; }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                _issuanceBindingContext = value.Clone();
            }
        }

        public override XmlDictionaryString RequestSecurityTokenAction
        {
            get
            {
                return StandardsManager.TrustDriver.RequestSecurityTokenAction;
            }
        }

        public override XmlDictionaryString RequestSecurityTokenResponseAction
        {
            get
            {
                return StandardsManager.TrustDriver.RequestSecurityTokenResponseAction;
            }
        }

        protected override MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        protected override bool RequiresManualReplyAddressing
        {
            get
            {
                ThrowIfCreated();
                return _requiresManualReplyAddressing;
            }
        }

        public override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (_rstChannelFactory != null)
            {
                await _rstChannelFactory.CloseHelperAsync(timeout);
                _rstChannelFactory = null;
            }

            await base.OnCloseAsync(timeoutHelper.RemainingTime());
        }

        public override void OnAbort()
        {
            if (_rstChannelFactory != null)
            {
                _rstChannelFactory.Abort();
                _rstChannelFactory = null;
            }
            base.OnAbort();
        }

        public override async Task OnOpenAsync(TimeSpan timeout)
        {
            if (IssuerBindingContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.IssuerBuildContextNotSet, GetType())));
            }
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            SetupRstChannelFactory();
            await _rstChannelFactory.OpenHelperAsync(timeout);
            await base.OnOpenAsync(timeoutHelper.RemainingTime());
        }

        protected abstract IChannelFactory<IAsyncRequestChannel> GetNegotiationChannelFactory(IChannelFactory<IAsyncRequestChannel> transportChannelFactory, ChannelBuilder channelBuilder);

        private void SetupRstChannelFactory()
        {
            IChannelFactory<IAsyncRequestChannel> innerChannelFactory = null;
            ChannelBuilder channelBuilder = new ChannelBuilder(IssuerBindingContext.Clone(), true);
            // if the underlying transport does not support request/reply, wrap it inside
            // a service channel factory.
            if (channelBuilder.CanBuildChannelFactory<IAsyncRequestChannel>())
            {
                innerChannelFactory = channelBuilder.BuildChannelFactory<IAsyncRequestChannel>();
                _requiresManualReplyAddressing = true;
            }
            else
            {
                ClientRuntime clientRuntime = new ClientRuntime("RequestSecurityTokenContract", NamingHelper.DefaultNamespace);
                clientRuntime.ValidateMustUnderstand = false;
                ServiceChannelFactory serviceChannelFactory = ServiceChannelFactory.BuildChannelFactory(channelBuilder, clientRuntime);

                serviceChannelFactory.ClientRuntime.UseSynchronizationContext = false;
                serviceChannelFactory.ClientRuntime.AddTransactionFlowProperties = false;
                ClientOperation rstOperation = new ClientOperation(serviceChannelFactory.ClientRuntime, "RequestSecurityToken", RequestSecurityTokenAction.Value);
                rstOperation.Formatter = MessageOperationFormatter.Instance;
                serviceChannelFactory.ClientRuntime.Operations.Add(rstOperation);

                if (IsMultiLegNegotiation)
                {
                    ClientOperation rstrOperation = new ClientOperation(serviceChannelFactory.ClientRuntime, "RequestSecurityTokenResponse", RequestSecurityTokenResponseAction.Value);
                    rstrOperation.Formatter = MessageOperationFormatter.Instance;
                    serviceChannelFactory.ClientRuntime.Operations.Add(rstrOperation);
                }
                // service channel automatically adds reply headers
                _requiresManualReplyAddressing = false;
                innerChannelFactory = new SecuritySessionSecurityTokenProvider.RequestChannelFactory(serviceChannelFactory);
            }

            _rstChannelFactory = GetNegotiationChannelFactory(innerChannelFactory, channelBuilder);
            _messageVersion = channelBuilder.Binding.MessageVersion;
        }

        // negotiation message processing overrides
        protected override Task InitializeChannelFactoriesAsync(EndpointAddress target, TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override IAsyncRequestChannel CreateClientChannel(EndpointAddress target, Uri via)
        {
            if (via != null)
            {
                return _rstChannelFactory.CreateChannel(target, via);
            }
            else
            {
                return _rstChannelFactory.CreateChannel(target);
            }
        }
    }
}
