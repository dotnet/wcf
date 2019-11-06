// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.ServiceModel;

namespace WcfService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncSingleNsService : ICalculatorRpcEnc
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }                
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitSingleNsService : ICalculatorRpcLit
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitSingleNsService : ICalculatorDocLit
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcEncDualNsService : ICalculatorRpcEnc, IHelloWorldRpcEnc
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RpcLitDualNsService : ICalculatorRpcLit, IHelloWorldRpcLit
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DocLitDualNsService : ICalculatorDocLit, IHelloWorldDocLit
    {
        private static ConcurrentDictionary<Guid, object> s_sessions = new ConcurrentDictionary<Guid, object>();

        [OperationBehavior]
        public int Sum2(int i, int j)
        {
            return i + j;
        }

        [OperationBehavior]
        public int Sum(IntParams par)
        {
            return par.P1 + par.P2;
        }

        [OperationBehavior]
        public float Divide(FloatParams par)
        {
            return (float)(par.P1 / par.P2);
        }

        [OperationBehavior]
        public string Concatenate(IntParams par)
        {
            return string.Format("{0}{1}", par.P1, par.P2);
        }

        [OperationBehavior]
        public void AddIntParams(Guid guid, IntParams par)
        {
            if (!s_sessions.TryAdd(guid, par))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public IntParams GetAndRemoveIntParams(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as IntParams;
        }

        [OperationBehavior]
        public DateTime ReturnInputDateTime(DateTime dt)
        {
            return dt;
        }

        [OperationBehavior]
        public byte[] CreateSet(ByteParams par)
        {
            return new byte[] { par.P1, par.P2 };
        }

        [OperationBehavior]
        public void AddString(Guid guid, string testString)
        {
            if (!s_sessions.TryAdd(guid, testString))
            {
                throw new InvalidOperationException(string.Format("Guid {0} already existed, and the value was {1}.", guid, s_sessions[guid]));
            }
        }

        [OperationBehavior]
        public string GetAndRemoveString(Guid guid)
        {
            object value;
            s_sessions.TryRemove(guid, out value);
            return value as string;
        }
    }
}
