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
        public static IntParams IntParamsProp { get; set; }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitSingleNsService : ICalculator
    {
        public static IntParams IntParamsProp { get; set; }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitSingleNsService : ICalculator
    {
        public static IntParams IntParamsProp { get; set; }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncDualNsService : ICalculator, IHelloWorld
    {
        public static IntParams IntParamsProp { get; set; }
        public static string StringField;

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public void SetStringField(string str)
        {
            StringField = str;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string GetStringField()
        {
            return StringField;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitDualNsService : ICalculator, IHelloWorld
    {
        public static IntParams IntParamsProp { get; set; }
        public static string StringField;

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public void SetStringField(string str)
        {
            StringField = str;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string GetStringField()
        {
            return StringField;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitDualNsService : ICalculator, IHelloWorld
    {
        public static IntParams IntParamsProp { get; set; }
        public static string StringField;

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void SetIntParamsProperty(IntParams par)
        {
            IntParamsProp = par;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public IntParams GetIntParamsProperty()
        {
            return IntParamsProp;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public void SetStringField(string str)
        {
            StringField = str;
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string GetStringField()
        {
            return StringField;
        }
    }
}
