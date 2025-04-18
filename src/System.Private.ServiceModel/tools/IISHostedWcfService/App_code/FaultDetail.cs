// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System.Runtime.Serialization;
#endif
using System.Xml.Serialization;

namespace WcfService
{
    [DataContract(Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class FaultDetail
    {
        private string _report;

        public FaultDetail(string message)
        {
            _report = message;
        }

        [DataMember]
        public string Message
        {
            get { return _report; }
            set { _report = value; }
        }
    }

    // FaultException<TDetail> Class with a property that will map to a different element name on the client depending on what serializer is used.
    [DataContract(Name = "FaultDetailWithXmlSerializerFormatAttribute", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class FaultDetailWithXmlSerializerFormatAttribute
    {
        private bool _usedSerializer;

        public FaultDetailWithXmlSerializerFormatAttribute()
        {
        }

        // If XmlSerializer is use this property will map to the element named "UsedXmlSerializer" on the client.
        // If DataContract Serializer is used this property will map to the element named "UsedDataContractSerializer" on the client.
        [DataMember]
        [XmlElement(ElementName = "UsedXmlSerializer")]
        public bool UsedDataContractSerializer
        {
            get { return _usedSerializer; }
            set { _usedSerializer = value; }
        }
    }
}
