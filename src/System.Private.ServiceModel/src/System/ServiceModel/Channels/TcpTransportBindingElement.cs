// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    public class TcpTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        private TcpConnectionPoolSettings _connectionPoolSettings;

        public TcpTransportBindingElement()
            : base()
        {
            _connectionPoolSettings = new TcpConnectionPoolSettings();
        }

        protected TcpTransportBindingElement(TcpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _connectionPoolSettings = elementToBeCloned._connectionPoolSettings.Clone();
        }

        public TcpConnectionPoolSettings ConnectionPoolSettings
        {
            get { return _connectionPoolSettings; }
        }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        public override BindingElement Clone()
        {
            return new TcpTransportBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)));
            }

            return (IChannelFactory<TChannel>)(object)new TcpChannelFactory<TChannel>(this, context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper();
            }

            else if (typeof(T) == typeof(ITransportCompressionSupport))
            {
                return (T)(object)new TransportCompressionSupportHelper();
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }

            TcpTransportBindingElement tcp = b as TcpTransportBindingElement;
            if (tcp == null)
            {
                return false;
            }

            if (!_connectionPoolSettings.IsMatch(tcp._connectionPoolSettings))
            {
                return false;
            }

            return true;
        }

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            internal BindingDeliveryCapabilitiesHelper()
            {
            }
            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery
            {
                get { return true; }
            }

            bool IBindingDeliveryCapabilities.QueuedDelivery
            {
                get { return false; }
            }
        }

        private class TransportCompressionSupportHelper : ITransportCompressionSupport
        {
            public bool IsCompressionFormatSupported(CompressionFormat compressionFormat)
            {
                return true;
            }
        }
    }
}
