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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("name", SRP.SFXBindingNameCannotBeNullOrEmpty);
            }
            if (ns == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(ns));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _closeTimeout = value;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = GetType().Name;
                }

                return _name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SRP.SFXBindingNameCannotBeNullOrEmpty);
                }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRangeTooBig));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _receiveTimeout = value;
            }
        }

        public abstract string Scheme { get; }

        public MessageVersion MessageVersion
        {
            get
            {
                return GetProperty<MessageVersion>(new BindingParameterCollection());
            }
        }

        public TimeSpan SendTimeout
        {
            get { return _sendTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _sendTimeout = value;
            }
        }

        public IChannelFactory<TChannel> BuildChannelFactory<TChannel>(params object[] parameters)
        {
            return BuildChannelFactory<TChannel>(new BindingParameterCollection(parameters));
        }

        public virtual IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            EnsureInvariants();
            BindingContext context = new BindingContext(new CustomBinding(this), parameters);
            IChannelFactory<TChannel> channelFactory = context.BuildInnerChannelFactory<TChannel>();
            context.ValidateBindingElementsConsumed();
            ValidateSecurityCapabilities(channelFactory.GetProperty<ISecurityCapabilities>(), parameters);

            return channelFactory;
        }

        private void ValidateSecurityCapabilities(ISecurityCapabilities runtimeSecurityCapabilities, BindingParameterCollection parameters)
        {
            ISecurityCapabilities bindingSecurityCapabilities = GetProperty<ISecurityCapabilities>(parameters);

            if (!SecurityCapabilities.IsEqual(bindingSecurityCapabilities, runtimeSecurityCapabilities))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRP.Format(SRP.SecurityCapabilitiesMismatched, this)));
            }
        }

        public bool CanBuildChannelFactory<TChannel>(params object[] parameters)
        {
            return CanBuildChannelFactory<TChannel>(new BindingParameterCollection(parameters));
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
            BindingElementCollection elements = CreateBindingElements();
            TransportBindingElement transport = null;
            int index;
            for (index = 0; index < elements.Count; index++)
            {
                transport = elements[index] as TransportBindingElement;
                if (transport != null)
                {
                    break;
                }
            }

            if (transport == null)
            {
                if (contractName == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.CustomBindingRequiresTransport, Name)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.SFxCustomBindingNeedsTransport1, contractName)));
                }
            }
            if (index != elements.Count - 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.TransportBindingElementMustBeLast, Name, transport.GetType().Name)));
            }
            if (string.IsNullOrEmpty(transport.Scheme))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.InvalidBindingScheme, transport.GetType().Name)));
            }

            if (MessageVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.MessageVersionMissingFromBinding, Name)));
            }
        }

        internal void CopyTimeouts(IDefaultCommunicationTimeouts source)
        {
            CloseTimeout = source.CloseTimeout;
            OpenTimeout = source.OpenTimeout;
            ReceiveTimeout = source.ReceiveTimeout;
            SendTimeout = source.SendTimeout;
        }
    }
}

