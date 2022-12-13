// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.ServiceModel;
#endif

namespace WcfService
{
    [ServiceContract(Namespace = "http://contoso.com/calc")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
    public interface ICalculatorRpcEnc
    {
        [OperationContract]
        int Sum2(int i, int j);

        [OperationContract]
        int Sum(IntParams par);

        [OperationContract]
        float Divide(FloatParams par);

        [OperationContract]
        string Concatenate(IntParams par);

        [OperationContract]
        void AddIntParams(Guid guid, IntParams par);

        [OperationContract]
        IntParams GetAndRemoveIntParams(Guid guid);

        [OperationContract]
        DateTime ReturnInputDateTime(DateTime dt);

        [OperationContract]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract(Namespace = "http://contoso.com/calc")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
    public interface ICalculatorRpcLit
    {
        [OperationContract]
        int Sum2(int i, int j);

        [OperationContract]
        int Sum(IntParams par);

        [OperationContract]
        float Divide(FloatParams par);

        [OperationContract]
        string Concatenate(IntParams par);

        [OperationContract]
        void AddIntParams(Guid guid, IntParams par);

        [OperationContract]
        IntParams GetAndRemoveIntParams(Guid guid);

        [OperationContract]
        DateTime ReturnInputDateTime(DateTime dt);

        [OperationContract]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract(Namespace = "http://contoso.com/calc")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    public interface ICalculatorDocLit
    {
        [OperationContract]
        int Sum2(int i, int j);

        [OperationContract]
        int Sum(IntParams par);

        [OperationContract]
        float Divide(FloatParams par);

        [OperationContract]
        string Concatenate(IntParams par);

        [OperationContract]
        void AddIntParams(Guid guid, IntParams par);

        [OperationContract]
        IntParams GetAndRemoveIntParams(Guid guid);

        [OperationContract]
        DateTime ReturnInputDateTime(DateTime dt);

        [OperationContract]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
    public interface IHelloWorldRpcEnc
    {
        [OperationContract]
        void AddString(Guid guid, string testString);

        [OperationContract]
        string GetAndRemoveString(Guid guid);
    }

    [ServiceContract]
    [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
    public interface IHelloWorldRpcLit
    {
        [OperationContract]
        void AddString(Guid guid, string testString);

        [OperationContract]
        string GetAndRemoveString(Guid guid);
    }

    [ServiceContract]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    public interface IHelloWorldDocLit
    {
        [OperationContract]
        void AddString(Guid guid, string testString);

        [OperationContract]
        string GetAndRemoveString(Guid guid);
    }

    public class IntParams
    {
        public int P1;
        public int P2;
    }

    public class FloatParams
    {
        public float P1;
        public float P2;
    }

    public class ByteParams
    {
        public byte P1;
        public byte P2;
    }

    [ServiceContract]
    public interface IEchoRpcEncWithHeadersService
    {

        [OperationContract(Action = "http://tempuri.org/Echo", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        EchoResponse Echo(EchoRequest request);
    }

    [System.Xml.Serialization.SoapType(Namespace = "http://tempuri.org/")]
    public partial class StringHeader
    {
        public string HeaderValue { get; set; }
    }

    [MessageContract(WrapperName = "Echo", WrapperNamespace = "http://contoso.com/", IsWrapped = true)]
    public partial class EchoRequest
    {
        [MessageBodyMember(Namespace = "", Order = 0)]
        public string message;
    }

    [MessageContract(WrapperName = "EchoResponse", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
    public partial class EchoResponse
    {
        [MessageBodyMember(Namespace = "", Order = 0)]
        public string EchoResult;
    }
}
