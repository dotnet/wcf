// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
    internal class CompositeType
    {
        private bool _boolValue = true;
        private string _stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }
    }

    public class XmlCompositeType
    {
        private bool _boolValue = true;
        private string _stringValue = "Hello ";

        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }
    }

    // This type should only be used by test Contract.DataContractTests.NetTcpBinding_DuplexCallback_ReturnsXmlComplexType
    // It tests a narrow scenario that returns an Xml attributed type in the callback method that is not known by the ServiceContract attributed interface
    // This test is designed to make sure the NET Native toolchain creates the needed serializer
    public class XmlCompositeTypeDuplexCallbackOnly
    {
        private bool _boolValue = true;
        private string _stringValue = "Hello ";

        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }
    }
}
