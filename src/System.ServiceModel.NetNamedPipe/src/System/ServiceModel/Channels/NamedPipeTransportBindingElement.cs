// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    public class NamedPipeTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        private readonly List<SecurityIdentifier> _allowedUsers = new List<SecurityIdentifier>();
        private readonly NamedPipeConnectionPoolSettings _connectionPoolSettings = new NamedPipeConnectionPoolSettings();

        public NamedPipeTransportBindingElement() : base()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.PlatformNotSupported_NetNamedPipe));
            }
        }

        protected NamedPipeTransportBindingElement(NamedPipeTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned._allowedUsers != null)
            {
                foreach (SecurityIdentifier id in elementToBeCloned._allowedUsers)
                {
                    _allowedUsers.Add(id);
                }
            }

            _connectionPoolSettings = elementToBeCloned._connectionPoolSettings.Clone();
        }

        public NamedPipeConnectionPoolSettings ConnectionPoolSettings
        {
            get { return _connectionPoolSettings; }
        }

        public override string Scheme
        {
            get { return Uri.UriSchemeNetPipe; }
        }

        public override BindingElement Clone()
        {
            return new NamedPipeTransportBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(TChannel), SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)));
            }

            return (IChannelFactory<TChannel>)(object)new NamedPipeChannelFactory<TChannel>(this, context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper();
            }
            else
            {
                return base.GetProperty<T>(context);
            }
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
