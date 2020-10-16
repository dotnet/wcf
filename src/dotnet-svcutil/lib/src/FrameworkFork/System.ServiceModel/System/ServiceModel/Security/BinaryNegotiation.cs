// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    internal sealed class BinaryNegotiation
    {
        private byte[] _negotiationData;
        private XmlDictionaryString _valueTypeUriDictionaryString;
        private string _valueTypeUri;

        public BinaryNegotiation(
            string valueTypeUri,
            byte[] negotiationData)
        {
            if (valueTypeUri == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("valueTypeUri");
            }
            if (negotiationData == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("negotiationData");
            }
            _valueTypeUriDictionaryString = null;
            _valueTypeUri = valueTypeUri;
            _negotiationData = negotiationData;
        }

        public BinaryNegotiation(
            XmlDictionaryString valueTypeDictionaryString,
            byte[] negotiationData)
        {
            if (valueTypeDictionaryString == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("valueTypeDictionaryString");
            }
            if (negotiationData == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("negotiationData");
            }
            _valueTypeUriDictionaryString = valueTypeDictionaryString;
            _valueTypeUri = valueTypeDictionaryString.Value;
            _negotiationData = negotiationData;
        }

        public void Validate(XmlDictionaryString valueTypeUriDictionaryString)
        {
            if (_valueTypeUri != valueTypeUriDictionaryString.Value)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(string.Format(SRServiceModel.IncorrectBinaryNegotiationValueType, _valueTypeUri)));
            }
            _valueTypeUriDictionaryString = valueTypeUriDictionaryString;
        }

        public void WriteTo(XmlDictionaryWriter writer, string prefix, XmlDictionaryString localName, XmlDictionaryString ns, XmlDictionaryString valueTypeLocalName, XmlDictionaryString valueTypeNs)
        {
            writer.WriteStartElement(prefix, localName, ns);
            writer.WriteStartAttribute(valueTypeLocalName, valueTypeNs);
            if (_valueTypeUriDictionaryString != null)
                writer.WriteString(_valueTypeUriDictionaryString);
            else
                writer.WriteString(_valueTypeUri);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute(XD.SecurityJan2004Dictionary.EncodingType, null);
            writer.WriteString(XD.SecurityJan2004Dictionary.EncodingTypeValueBase64Binary);
            writer.WriteEndAttribute();
            writer.WriteBase64(_negotiationData, 0, _negotiationData.Length);
            writer.WriteEndElement();
        }

        public string ValueTypeUri
        {
            get
            {
                return _valueTypeUri;
            }
        }

        public byte[] GetNegotiationData()
        {
            // avoid copying since this is internal and callers use it as read-only
            return _negotiationData;
        }
    }
}
