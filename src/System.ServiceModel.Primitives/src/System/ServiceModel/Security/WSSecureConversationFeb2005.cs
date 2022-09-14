// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal class WSSecureConversationFeb2005 : WSSecureConversation
    {
        SecurityStateEncoder _securityStateEncoder;
        IList<Type> _knownClaimTypes;

        public WSSecureConversationFeb2005(WSSecurityTokenSerializer tokenSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes,
            int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
            : base(tokenSerializer, maxKeyDerivationOffset, maxKeyDerivationLabelLength, maxKeyDerivationNonceLength)
        {
            if (securityStateEncoder != null)
            {
                _securityStateEncoder = securityStateEncoder;
            }

            _knownClaimTypes = new List<Type>();
            if (knownTypes != null)
            {
                // Clone this collection.
                foreach (Type knownType in knownTypes)
                {
                    _knownClaimTypes.Add(knownType);
                }
            }
        }

        public override SecureConversationDictionary SerializerDictionary
        {
            get { return XD.SecureConversationFeb2005Dictionary; }
        }

        public override void PopulateTokenEntries(IList<WSSecurityTokenSerializer.TokenEntry> tokenEntryList)
        {
            base.PopulateTokenEntries(tokenEntryList);
            tokenEntryList.Add(new SecurityContextTokenEntryFeb2005(this, _securityStateEncoder, _knownClaimTypes));
        }

        class SecurityContextTokenEntryFeb2005 : SecurityContextTokenEntry
        {
            public SecurityContextTokenEntryFeb2005(WSSecureConversationFeb2005 parent, SecurityStateEncoder securityStateEncoder, IList<Type> knownClaimTypes)
                : base(parent, securityStateEncoder, knownClaimTypes)
            {
            }

            protected override bool CanReadGeneration(XmlDictionaryReader reader)
            {
                return reader.IsStartElement(DXD.SecureConversationDec2005Dictionary.Instance, XD.SecureConversationFeb2005Dictionary.Namespace);
            }

            protected override bool CanReadGeneration(XmlElement element)
            {
                return (element.LocalName == DXD.SecureConversationDec2005Dictionary.Instance.Value &&
                    element.NamespaceURI == XD.SecureConversationFeb2005Dictionary.Namespace.Value);
            }

            protected override UniqueId ReadGeneration(XmlDictionaryReader reader)
            {
                return reader.ReadElementContentAsUniqueId();
            }

            protected override UniqueId ReadGeneration(XmlElement element)
            {
                return XmlHelper.ReadTextElementAsUniqueId(element);
            }

            protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextSecurityToken sct)
            {
                // serialize the generation
                if (sct.KeyGeneration != null)
                {
                    writer.WriteStartElement(XD.SecureConversationFeb2005Dictionary.Prefix.Value, DXD.SecureConversationDec2005Dictionary.Instance,
                        XD.SecureConversationFeb2005Dictionary.Namespace);
                    XmlHelper.WriteStringAsUniqueId(writer, sct.KeyGeneration);
                    writer.WriteEndElement();
                }
            }
        }

        public class DriverFeb2005 : Driver
        {
            public DriverFeb2005()
            {
            }

            protected override SecureConversationDictionary DriverDictionary
            {
                get { return XD.SecureConversationFeb2005Dictionary; }
            }

            public override XmlDictionaryString CloseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextClose; }
            }

            public override XmlDictionaryString CloseResponseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextCloseResponse; }
            }

            public override bool IsSessionSupported
            {
                get { return true; }
            }

            public override XmlDictionaryString RenewAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenew; }
            }

            public override XmlDictionaryString RenewResponseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenewResponse; }
            }

            public override XmlDictionaryString Namespace
            {
                get { return XD.SecureConversationFeb2005Dictionary.Namespace; }
            }

            public override string TokenTypeUri
            {
                get { return XD.SecureConversationFeb2005Dictionary.SecurityContextTokenType.Value; }
            }
        }
    }
}
