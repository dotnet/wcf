// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(Namespace = "http://contoso.com/calc")]
    public interface ICalculator
    {
        [OperationContract]
        int Sum(IntParams par);

        [OperationContract]
        float Divide(FloatParams par);

        [OperationContract]
        string Concatenate(IntParams par);

        [OperationContract]
        void DoSomething(IntParams par);

        [OperationContract]
        DateTime GetCurrentDateTime();

        [OperationContract]
        byte[] CreateSet(ByteParams par);
    }

    [ServiceContract]
    public interface IHelloWorld
    {
        [OperationContract]
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
