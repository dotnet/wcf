// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    public class WindowsStreamSecurityBindingElement : StreamUpgradeBindingElement,
        ITransportTokenAssertionProvider
    {
        private ProtectionLevel _protectionLevel;

        public WindowsStreamSecurityBindingElement()
            : base()
        {
            _protectionLevel = ConnectionOrientedTransportDefaults.ProtectionLevel;
        }

        protected WindowsStreamSecurityBindingElement(WindowsStreamSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _protectionLevel = elementToBeCloned._protectionLevel;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.ProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get
            {
                return _protectionLevel;
            }
            set
            {
                ProtectionLevelHelper.Validate(value);
                _protectionLevel = value;
            }
        }

        public override BindingElement Clone()
        {
            return new WindowsStreamSecurityBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context)
        {
            return new WindowsStreamSecurityUpgradeProvider(this, context, true);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SecurityCapabilities(true, true, true, _protectionLevel, _protectionLevel);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)IdentityVerifier.CreateDefault();
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal static void ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            XmlElement assertion = PolicyConversionContext.FindAssertion(policyContext.GetBindingAssertions(),
                TransportPolicyConstants.WindowsTransportSecurityName, TransportPolicyConstants.DotNetFramingNamespace, true);

            if (assertion != null)
            {
                WindowsStreamSecurityBindingElement windowsBindingElement
                    = new WindowsStreamSecurityBindingElement();

                XmlReader reader = new XmlNodeReader(assertion);
                reader.ReadStartElement();
                string protectionLevelString = null;
                if (reader.IsStartElement(
                    TransportPolicyConstants.ProtectionLevelName,
                    TransportPolicyConstants.DotNetFramingNamespace) && !reader.IsEmptyElement)
                {
                    protectionLevelString = reader.ReadElementContentAsString();
                }
                if (string.IsNullOrEmpty(protectionLevelString))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                        string.Format(SRServiceModel.ExpectedElementMissing, TransportPolicyConstants.ProtectionLevelName, TransportPolicyConstants.DotNetFramingNamespace)));
                }
                windowsBindingElement.ProtectionLevel = (ProtectionLevel)Enum.Parse(typeof(ProtectionLevel), protectionLevelString);
                policyContext.BindingElements.Add(windowsBindingElement);
            }
        }

        #region ITransportTokenAssertionProvider Members

        public XmlElement GetTransportTokenAssertion()
        {
            XmlDocument document = new XmlDocument();
            XmlElement assertion =
                document.CreateElement(TransportPolicyConstants.DotNetFramingPrefix,
                TransportPolicyConstants.WindowsTransportSecurityName,
                TransportPolicyConstants.DotNetFramingNamespace);
            XmlElement protectionLevelElement = document.CreateElement(TransportPolicyConstants.DotNetFramingPrefix,
                TransportPolicyConstants.ProtectionLevelName, TransportPolicyConstants.DotNetFramingNamespace);
            protectionLevelElement.AppendChild(document.CreateTextNode(this.ProtectionLevel.ToString()));
            assertion.AppendChild(protectionLevelElement);
            return assertion;
        }

        #endregion

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
            {
                return false;
            }
            WindowsStreamSecurityBindingElement security = b as WindowsStreamSecurityBindingElement;
            if (security == null)
            {
                return false;
            }
            if (_protectionLevel != security._protectionLevel)
            {
                return false;
            }

            return true;
        }
    }
}
