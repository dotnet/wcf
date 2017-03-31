// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;
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

    // SupportFaults property
    // true if the XmlSerializer should be used for reading and writing faults; false if the DataContractSerializer should be used. The default is false.
    [System.Runtime.Serialization.DataContract(Name = "FaultDetailWithXmlSerializerFormatAttribute_SupportFaults", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class FaultDetailWithXmlSerializerFormatAttribute_SupportFaults
    {
        private string _report;

        public FaultDetailWithXmlSerializerFormatAttribute_SupportFaults()
        {
        }

        public FaultDetailWithXmlSerializerFormatAttribute_SupportFaults(string message)
        {
            _report = message;
        }

        [DataMember]
        [XmlElement(ElementName = "FooBar")]
        public string Message
        {
            get { return _report; }
            set { _report = value; }
        }
    }
}
