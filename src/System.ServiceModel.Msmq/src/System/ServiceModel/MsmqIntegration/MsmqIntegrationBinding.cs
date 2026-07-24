// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel.MsmqIntegration
{
    public class MsmqIntegrationBinding : MsmqBindingBase
    {
        private MsmqIntegrationSecurity _security = new MsmqIntegrationSecurity();

        public MsmqIntegrationBinding()
        {
            Initialize();
        }

        public MsmqIntegrationBinding(MsmqIntegrationSecurityMode securityMode)
        {
            if (!MsmqIntegrationSecurityModeHelper.IsDefined(securityMode))
            {
                throw new InvalidEnumArgumentException(nameof(securityMode), (int)securityMode, typeof(MsmqIntegrationSecurityMode));
            }
            Initialize();
            _security.Mode = securityMode;
        }

        public MsmqIntegrationSecurity Security
        {
            get { return _security; }
            set { _security = value; }
        }

        internal Type[] TargetSerializationTypes
        {
            get { return ((MsmqIntegrationBindingElement)transport).TargetSerializationTypes; }
            set { ((MsmqIntegrationBindingElement)transport).TargetSerializationTypes = value; }
        }

        [DefaultValue(MsmqDefaults.MsmqMessageSerializationFormat)]
        public MsmqMessageSerializationFormat SerializationFormat
        {
            get { return ((MsmqIntegrationBindingElement)transport).SerializationFormat; }
            set { ((MsmqIntegrationBindingElement)transport).SerializationFormat = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSecurity()
        {
            if (_security.Mode != MsmqIntegrationSecurityMode.Transport)
            {
                return true;
            }
            if (_security.Transport.MsmqAuthenticationMode != MsmqDefaults.MsmqAuthenticationMode
                || _security.Transport.MsmqEncryptionAlgorithm != MsmqDefaults.MsmqEncryptionAlgorithm
                || _security.Transport.MsmqSecureHashAlgorithm != MsmqDefaults.MsmqSecureHashAlgorithm
                || _security.Transport.MsmqProtectionLevel != MsmqDefaults.MsmqProtectionLevel)
            {
                return true;
            }
            return false;
        }

        private void Initialize()
        {
            transport = new MsmqIntegrationBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElements = new BindingElementCollection();
            _security.ConfigureTransportSecurity(transport);
            bindingElements.Add(transport);
            return bindingElements.Clone();
        }
    }
}
