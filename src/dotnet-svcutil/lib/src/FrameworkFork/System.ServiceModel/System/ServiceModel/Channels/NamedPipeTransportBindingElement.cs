//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.Security.Principal;

    public class NamedPipeTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        NamedPipeSettings settings = new NamedPipeSettings();

        public NamedPipeTransportBindingElement()
            : base()
        {
        }

        protected NamedPipeTransportBindingElement(NamedPipeTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.settings = elementToBeCloned.settings.Clone();
        }

        public NamedPipeSettings PipeSettings
        {
            get { return this.settings; }
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
            throw new NotImplementedException();
        }

        class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
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
