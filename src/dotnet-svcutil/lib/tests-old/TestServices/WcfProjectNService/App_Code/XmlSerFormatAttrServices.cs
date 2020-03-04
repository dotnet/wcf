// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Concurrent;
using System.ServiceModel;

namespace WcfProjectNService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncSingleNsService : ICalculator
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }                
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitDualNsService : ICalculator, IHelloWorld
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Literal)]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitDualNsService : ICalculator, IHelloWorld
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

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
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
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
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }
}
