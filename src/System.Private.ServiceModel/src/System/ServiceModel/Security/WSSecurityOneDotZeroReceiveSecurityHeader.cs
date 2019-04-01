// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal class WSSecurityOneDotZeroReceiveSecurityHeader : ReceiveSecurityHeader
    {
        public WSSecurityOneDotZeroReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            int headerIndex,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, headerIndex, transferDirection)
        {
        }

        protected override bool IsReaderAtReferenceList(XmlDictionaryReader reader)
        {
            return reader.IsStartElement(ReferenceList.ElementName, ReferenceList.NamespaceUri);
        }

        protected override bool IsReaderAtSignature(XmlDictionaryReader reader)
        {
            return reader.IsStartElement(XD.XmlSignatureDictionary.Signature, XD.XmlSignatureDictionary.Namespace);
        }

        protected override void EnsureDecryptionComplete()
        {
            // noop
        }

        protected override bool IsReaderAtEncryptedKey(XmlDictionaryReader reader)
        {
            return reader.IsStartElement(System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedKey, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);
        }

        protected override bool IsReaderAtEncryptedData(XmlDictionaryReader reader)
        {
            bool encrypted = reader.IsStartElement(System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedData, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);

            if (encrypted == true)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            return encrypted;
        }

        protected override bool IsReaderAtSecurityTokenReference(XmlDictionaryReader reader)
        {
            return reader.IsStartElement(XD.SecurityJan2004Dictionary.SecurityTokenReference, XD.SecurityJan2004Dictionary.Namespace);
        }
    }
}
