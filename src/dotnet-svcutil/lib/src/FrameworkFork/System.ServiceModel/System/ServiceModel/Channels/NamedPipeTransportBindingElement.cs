// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.Security.Principal;

    public class NamedPipeTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        private NamedPipeSettings _settings = new NamedPipeSettings();
        NamedPipeConnectionPoolSettings _connectionPoolSettings = new NamedPipeConnectionPoolSettings();

        public NamedPipeTransportBindingElement()
            : base()
        {
        }

        protected NamedPipeTransportBindingElement(NamedPipeTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _settings = elementToBeCloned._settings.Clone();
        }

        public NamedPipeConnectionPoolSettings ConnectionPoolSettings
        {
            get { return _connectionPoolSettings; }
        }

        public NamedPipeSettings PipeSettings
        {
            get { return _settings; }
        }

        public override string Scheme
        {
            get { return "net.pipe"; }
        }

        public override BindingElement Clone()
        {
            return new NamedPipeTransportBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
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
            if (typeof(T) == typeof(NamedPipeSettings))
            {
                return (T)(object)this.PipeSettings;
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

            NamedPipeTransportBindingElement namedPipe = b as NamedPipeTransportBindingElement;
            if (namedPipe == null)
            {
                return false;
            }

            if (!this.ConnectionPoolSettings.IsMatch(namedPipe.ConnectionPoolSettings))
            {
                return false;
            }

            if (!this.PipeSettings.IsMatch(namedPipe.PipeSettings))
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
    }
}
