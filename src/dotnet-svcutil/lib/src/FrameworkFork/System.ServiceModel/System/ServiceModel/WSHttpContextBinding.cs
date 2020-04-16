// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;

    public class WSHttpContextBinding : WSHttpBinding
    {
        private ProtectionLevel _contextProtectionLevel = ContextBindingElement.DefaultProtectionLevel;
        private bool _contextManagementEnabled = ContextBindingElement.DefaultContextManagementEnabled;

        public WSHttpContextBinding()
            : base()
        {
        }

        public WSHttpContextBinding(SecurityMode securityMode)
            : base(securityMode)
        {
        }

        public WSHttpContextBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
        }

        private WSHttpContextBinding(WSHttpBinding wsHttpBinding)
        {
            this.CloseTimeout = wsHttpBinding.CloseTimeout;
            this.OpenTimeout = wsHttpBinding.OpenTimeout;
            this.ReceiveTimeout = wsHttpBinding.ReceiveTimeout;
            this.SendTimeout = wsHttpBinding.SendTimeout;
        }

        [DefaultValue(null)]
        public Uri ClientCallbackAddress
        {
            get;
            set;
        }

        [DefaultValue(ContextBindingElement.DefaultContextManagementEnabled)]
        public bool ContextManagementEnabled
        {
            get
            {
                return _contextManagementEnabled;
            }
            set
            {
                _contextManagementEnabled = value;
            }
        }

        [DefaultValue(ContextBindingElement.DefaultProtectionLevel)]
        public ProtectionLevel ContextProtectionLevel
        {
            get
            {
                return _contextProtectionLevel;
            }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _contextProtectionLevel = value;
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection result;

            if (this.AllowCookies)
            {
                try
                {
                    // Passing AllowCookies=false to HttpTransportBinding means we don't want transport layer to manage
                    // cookie containers. We are going to do this at the context channel level, because we need channel 
                    // level isolation as opposed to channel factory level isolation. 

                    this.AllowCookies = false;
                    result = base.CreateBindingElements();
                }
                finally
                {
                    this.AllowCookies = true;
                }
                result.Insert(0, new ContextBindingElement(this.ContextProtectionLevel, ContextExchangeMechanism.HttpCookie, this.ClientCallbackAddress, this.ContextManagementEnabled));
            }
            else
            {
                result = base.CreateBindingElements();
                result.Insert(0, new ContextBindingElement(this.ContextProtectionLevel, ContextExchangeMechanism.ContextSoapHeader, this.ClientCallbackAddress, this.ContextManagementEnabled));
            }

            return result;
        }

        internal static new bool TryCreate(BindingElementCollection bindingElements, out Binding binding)
        {
            if (bindingElements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            }

            binding = null;

            ContextBindingElement contextBindingElement = bindingElements.Find<ContextBindingElement>();
            if (contextBindingElement != null)
            {
                BindingElementCollection bindingElementsWithoutContext = new BindingElementCollection(bindingElements);
                bindingElementsWithoutContext.Remove<ContextBindingElement>();
                Binding wsHttpBinding;
                if (WSHttpBinding.TryCreate(bindingElementsWithoutContext, out wsHttpBinding))
                {
                    bool allowCookies = ((WSHttpBinding)wsHttpBinding).AllowCookies;
                    if (allowCookies && contextBindingElement.ContextExchangeMechanism == ContextExchangeMechanism.HttpCookie
                        || !allowCookies && contextBindingElement.ContextExchangeMechanism == ContextExchangeMechanism.ContextSoapHeader)
                    {
                        WSHttpContextBinding contextBinding = new WSHttpContextBinding((WSHttpBinding)wsHttpBinding);
                        contextBinding.ContextProtectionLevel = contextBindingElement.ProtectionLevel;
                        contextBinding.ContextManagementEnabled = contextBindingElement.ContextManagementEnabled;
                        binding = contextBinding;
                    }
                }
            }

            return binding != null;
        }
    }
}
