// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract(Namespace = "http://contoso.com/calc"), XmlSerializerFormat]
    public interface ICalculator
    {
        [OperationContract, XmlSerializerFormat]
        int Sum2(int i, int j);

        [OperationContract, XmlSerializerFormat]
        int Sum(IntParams par);

        [OperationContract, XmlSerializerFormat]
        float Divide(FloatParams par);

        [OperationContract, XmlSerializerFormat]
        string Concatenate(IntParams par);

        [OperationContract, XmlSerializerFormat]
        void SetIntParamsProperty(IntParams par);

        [OperationContract, XmlSerializerFormat]
        IntParams GetIntParamsProperty();

        [OperationContract, XmlSerializerFormat]
        DateTime ReturnInputDateTime(DateTime dt);

        [OperationContract, XmlSerializerFormat]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract, XmlSerializerFormat]
    public interface IHelloWorld
    {
        [OperationContract, XmlSerializerFormat]
        void SetStringField(string testString);

        [OperationContract, XmlSerializerFormat]
        string GetStringField();
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
}
