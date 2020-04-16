// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xml;
    using Microsoft.Xml.Serialization;
    using System.Collections.ObjectModel;
    using WsdlNS = System.Web.Services.Description;
    using System.ServiceModel.Channels;

    [XmlRoot(ElementName = MetadataStrings.MetadataExchangeStrings.MetadataReference, Namespace = MetadataStrings.MetadataExchangeStrings.Namespace)]
    public class MetadataReference : IXmlSerializable
    {
        private EndpointAddress _address;
        private AddressingVersion _addressVersion;
        private Collection<XmlAttribute> _attributes = new Collection<XmlAttribute>();
        private static XmlDocument s_document = new XmlDocument();

        public MetadataReference()
        {
        }

        public MetadataReference(EndpointAddress address, AddressingVersion addressVersion)
        {
            _address = address;
            _addressVersion = addressVersion;
        }

        public EndpointAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public AddressingVersion AddressVersion
        {
            get { return _addressVersion; }
            set { _addressVersion = value; }
        }

        Microsoft.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _address = EndpointAddress.ReadFrom(XmlDictionaryReader.CreateDictionaryReader(reader), out _addressVersion);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (_address != null)
            {
                _address.WriteContentsTo(_addressVersion, writer);
            }
        }
    }
}
