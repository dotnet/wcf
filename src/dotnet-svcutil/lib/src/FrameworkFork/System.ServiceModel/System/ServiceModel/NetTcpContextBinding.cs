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

    public class NetTcpContextBinding : NetTcpBinding
    {
        private bool _contextManagementEnabled = ContextBindingElement.DefaultContextManagementEnabled;
        private ProtectionLevel _contextProtectionLevel = ContextBindingElement.DefaultProtectionLevel;

        public NetTcpContextBinding()
            : base()
        {
        }

        public NetTcpContextBinding(SecurityMode securityMode)
            : base(securityMode)
        {
        }

        public NetTcpContextBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : base(securityMode, reliableSessionEnabled)
        {
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
            BindingElementCollection result = base.CreateBindingElements();
            result.Insert(0, new ContextBindingElement(this.ContextProtectionLevel, ContextExchangeMechanism.ContextSoapHeader, this.ClientCallbackAddress, this.ContextManagementEnabled));
            return result;
        }
    }
}
