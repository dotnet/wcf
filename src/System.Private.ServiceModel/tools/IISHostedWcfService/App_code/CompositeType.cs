// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
#endif
using System.Xml;
using System.Xml.Serialization;

namespace WcfService
{
    [DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
    public class CompositeType
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

[MessageContract(WrapperName = "login", WrapperNamespace = "http://www.contoso.com/", IsWrapped = true)]
public partial class LoginRequest
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 0)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string clientId;

    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 1)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string user;

    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 2)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string pwd;

    public LoginRequest()
    {
    }

    public LoginRequest(string clientId, string user, string pwd)
    {
        this.clientId = clientId;
        this.user = user;
        this.pwd = pwd;
    }
}

[MessageContract(WrapperName = "loginResponse", WrapperNamespace = "http://www.contoso.com/", IsWrapped = true)]
public partial class LoginResponse
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 0)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string @return;

    public LoginResponse()
    {
    }

    public LoginResponse(string @return)
    {
        this.@return = @return;
    }
}

[DataContract()]
public class Widget
{
    [DataMember]
    public string Id;
    [DataMember]
    public string Catalog;
}

[DataContract()]
public class Widget0 : Widget
{
}

[DataContract()]
public class Widget1 : Widget
{
}

[DataContract()]
public class Widget2 : Widget
{
}

[DataContract()]
public class Widget3 : Widget
{
}

public class XmlVeryComplexType
{
    private int _id;
    private NonInstantiatedType _nonInstantiatedField = null;

    public NonInstantiatedType NonInstantiatedField
    {
        get
        {
            return _nonInstantiatedField;
        }
        set
        {
            _nonInstantiatedField = value;
        }
    }

    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
}

public class NonInstantiatedType
{
}

[XmlType(Namespace = "urn:TestWebServices/MyWebService/")]
public partial class MesssageHeaderCreateHeaderWithXmlSerializerTestType
{
    private string _message;

    /// <remarks/>
    [XmlElement(Order = 0)]
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }
}

[MessageContract(WrapperName = "XmlMessageContractTestRequest", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestRequest
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/XmlMessageContarctTestMessages", Order = 0)]
    public string Message;

    public XmlMessageContractTestRequest()
    {
    }

    public XmlMessageContractTestRequest(string message)
    {
        this.Message = message;
    }
}

[MessageContract(WrapperName = "XmlMessageContractTestRequestWithMessageHeader", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestRequestWithMessageHeader
{
    [MessageHeader(Name = "XmlMessageContractTestRequestWithMessageHeaderMessage", Namespace = "http://www.contoso.com", MustUnderstand = false)]
    public string Message;

    public XmlMessageContractTestRequestWithMessageHeader()
    {
    }

    public XmlMessageContractTestRequestWithMessageHeader(string message)
    {
        this.Message = message;
    }
}

[MessageContract(WrapperName = "XmlMessageContractTestResponse", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestResponse
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/XmlMessageContarctTestMessages", Order = 0)]
    public string _message;

    public XmlMessageContractTestResponse()
    {
    }

    public XmlMessageContractTestResponse(string message)
    {
        this._message = message;
    }

    [MessageHeader(Name = "OutOfBandData", Namespace = "http://www.contoso.com", MustUnderstand = false)]
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }
}

[DataContract]
public class Employee
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Age { get; set; }
}

[DataContract]
public class Manager : Employee
{
    [DataMember]
    public int OfficeId { get; set; }
}
public class ManagerDataContractResolver : DataContractResolver
{
    private string Namespace
    {
        get { return typeof(Manager).Namespace ?? "global"; }
    }

    private string Name
    {
        get { return typeof(Manager).Name; }
    }


    public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
    {
        if (typeName == this.Name && typeNamespace == this.Namespace)
        {
            return typeof(Manager);
        }
        else
        {
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
        }
    }

    public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
    {
        if (type == typeof(Manager))
        {
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(this.Name);
            typeNamespace = dic.Add(this.Namespace);
            return true;
        }
        else
        {
            return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
        }
    }
}

public class SoapComplexType
{
    private bool _boolValue;
    private string _stringValue;

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

[SoapType(Namespace = "WcfService")]
public class CustomerObject
{
    public string Name { get; set; }
    public object Data { get; set; }
}

[Serializable]
[SoapType(Namespace = "WcfService")]
public partial class AdditionalData
{
    public string Field
    {
        get; set;
    }
}

[MessageContract(WrapperName = "PingResponse", IsWrapped = true)]
public class PingEncodedResponse
{
    [MessageBodyMember(Namespace = "", Order = 0)]
    public int @Return;
}

[MessageContract(WrapperName = "Ping", IsWrapped = true)]
public class PingEncodedRequest
{

    [MessageBodyMember(Namespace = "", Order = 0)]
    public string Pinginfo;

    public PingEncodedRequest() { }

    public PingEncodedRequest(string pinginfo)
    {
        this.Pinginfo = pinginfo;
    }
}

[DataContract]
public class CustomMessage
{
}
