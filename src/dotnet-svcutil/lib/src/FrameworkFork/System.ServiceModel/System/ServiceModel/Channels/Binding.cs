// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Description;

namespace System.ServiceModel.Channels
{
    public abstract class Binding : IDefaultCommunicationTimeouts
    {
        private TimeSpan _closeTimeout = ServiceDefaults.CloseTimeout;
        private string _name;
        private string _namespaceIdentifier;
        private TimeSpan _openTimeout = ServiceDefaults.OpenTimeout;
        private TimeSpan _receiveTimeout = ServiceDefaults.ReceiveTimeout;
        private TimeSpan _sendTimeout = ServiceDefaults.SendTimeout;
        internal const string DefaultNamespace = NamingHelper.DefaultNamespace;

        protected Binding()
        {
            _name = null;
            _namespaceIdentifier = DefaultNamespace;
        }

        protected Binding(string name, string ns)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("name", SRServiceModel.SFXBindingNameCannotBeNullOrEmpty);
            }
            if (ns == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ns");
            }

            if (ns.Length > 0)
            {
                NamingHelper.CheckUriParameter(ns, "ns");
            }

            _name = name;
            _namespaceIdentifier = ns;
        }

        public TimeSpan CloseTimeout
        {
            get { return _closeTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _closeTimeout = value;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = this.GetType().Name;

                return _name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SRServiceModel.SFXBindingNameCannotBeNullOrEmpty);

                _name = value;
            }
        }

        public string Namespace
        {
            get { return _namespaceIdentifier; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                if (value.Length > 0)
                {
                    NamingHelper.CheckUriProperty(value, "Namespace");
                }
                _namespaceIdentifier = value;
            }
        }

        public TimeSpan OpenTimeout
        {
            get { return _openTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _openTimeout = value;
            }
        }

        public TimeSpan ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _receiveTimeout = value;
            }
        }

        public abstract string Scheme { get; }

        public MessageVersion MessageVersion
        {
            get
            {
                return this.GetProperty<MessageVersion>(new BindingParameterCollection());
            }
        }

        public TimeSpan SendTimeout
        {
            get { return _sendTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _sendTimeout = value;
            }
        }

        public IChannelFactory<TChannel> BuildChannelFactory<TChannel>(params object[] parameters)
        {
            return this.BuildChannelFactory<TChannel>(new BindingParameterCollection(parameters));
        }

        public virtual IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            EnsureInvariants();
            BindingContext context = new BindingContext(new CustomBinding(this), parameters);
            IChannelFactory<TChannel> channelFactory = context.BuildInnerChannelFactory<TChannel>();
            context.ValidateBindingElementsConsumed();
            this.ValidateSecurityCapabilities(channelFactory.GetProperty<ISecurityCapabilities>(), parameters);

            return channelFactory;
        }

        private void ValidateSecurityCapabilities(ISecurityCapabilities runtimeSecurityCapabilities, BindingParameterCollection parameters)
        {
            ISecurityCapabilities bindingSecurityCapabilities = this.GetProperty<ISecurityCapabilities>(parameters);

            if (!SecurityCapabilities.IsEqual(bindingSecurityCapabilities, runtimeSecurityCapabilities))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(string.Format(SRServiceModel.SecurityCapabilitiesMismatched, this)));
            }
        }

        public bool CanBuildChannelFactory<TChannel>(params object[] parameters)
        {
            return this.CanBuildChannelFactory<TChannel>(new BindingParameterCollection(parameters));
        }

        public virtual bool CanBuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            BindingContext context = new BindingContext(new CustomBinding(this), parameters);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        // the elements should NOT reference internal elements used by the Binding
        public abstract BindingElementCollection CreateBindingElements();

        public T GetProperty<T>(BindingParameterCollection parameters)
            where T : class
        {
            BindingContext context = new BindingContext(new CustomBinding(this), parameters);
            return context.GetInnerProperty<T>();
        }

        private void EnsureInvariants()
        {
            EnsureInvariants(null);
        }

        internal void EnsureInvariants(string contractName)
        {
            BindingElementCollection elements = this.CreateBindingElements();
            TransportBindingElement transport = null;
            int index;
            for (index = 0; index < elements.Count; index++)
            {
                transport = elements[index] as TransportBindingElement;
                if (transport != null)
                    break;
            }

            if (transport == null)
            {
                if (contractName == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.CustomBindingRequiresTransport, this.Name)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxCustomBindingNeedsTransport1, contractName)));
                }
            }
            if (index != elements.Count - 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.TransportBindingElementMustBeLast, this.Name, transport.GetType().Name)));
            }
            if (string.IsNullOrEmpty(transport.Scheme))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.InvalidBindingScheme, transport.GetType().Name)));
            }

            if (this.MessageVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.MessageVersionMissingFromBinding, this.Name)));
            }
        }

        internal void CopyTimeouts(IDefaultCommunicationTimeouts source)
        {
            this.CloseTimeout = source.CloseTimeout;
            this.OpenTimeout = source.OpenTimeout;
            this.ReceiveTimeout = source.ReceiveTimeout;
            this.SendTimeout = source.SendTimeout;
        }
    }
}

