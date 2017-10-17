// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;

namespace WcfService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncSingleNsService : ICalculator
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitSingleNsService : ICalculator
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitSingleNsService : ICalculator
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncMultiNsService : ICalculator, IHelloWorld
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void SayHello(string name)
        {
            Console.WriteLine("Hello {0}", name);
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitMultiNsService : ICalculator, IHelloWorld
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void SayHello(string name)
        {
            Console.WriteLine("Hello {0}", name);
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitMultiNsService : ICalculator, IHelloWorld
    {
        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.p1 + par.p2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.p1 / par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void DoSomething(IntParams par)
        {
            Console.WriteLine("Inside DoSomething method...params: {0} {1}", par.p1, par.p2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.p1, par.p2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void SayHello(string name)
        {
            Console.WriteLine("Hello {0}", name);
        }
    }
}
