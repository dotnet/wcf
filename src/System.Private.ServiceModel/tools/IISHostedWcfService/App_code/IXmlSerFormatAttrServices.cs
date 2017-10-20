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
        void DoSomething(IntParams par);

        [OperationContract, XmlSerializerFormat]
        DateTime GetCurrentDateTime();

        [OperationContract, XmlSerializerFormat]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract, XmlSerializerFormat]
    public interface IHelloWorld
    {
        [OperationContract, XmlSerializerFormat]
        void SayHello(string name);
    }

    public class IntParams
    {
        public int p1;
        public int p2;
    }

    public class FloatParams
    {
        public float p1;
        public float p2;
    }

    public class ByteParams
    {
        public byte p1;
        public byte p2;
    }
}
