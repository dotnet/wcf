// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;

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

    public class EchoRpcEncWithHeadersService : IEchoRpcEncWithHeadersService
    {
        static EchoRpcEncWithHeadersService()
        {
            var member = new XmlReflectionMember
            {
                MemberName = "StringHeader",
                MemberType = typeof(StringHeader),
                SoapAttributes = new SoapAttributes { SoapElement = new SoapElementAttribute("StringHeader") }
            };
            var members = new XmlReflectionMember[] { member };
            var mappings = new SoapReflectionImporter().ImportMembersMapping("EchoServiceSoap", "http://tempuri.org/", members, false, false);
            var serializers = XmlSerializer.FromMappings(new XmlMembersMapping[] { mappings }, typeof(EchoRpcEncWithHeadersService));
            s_stringHeaderSerializer = serializers[0];
        }

        private static XmlSerializer s_stringHeaderSerializer { get; set; }

        public EchoResponse Echo(EchoRequest request)
        {
            var incomingHeaders = OperationContext.Current.IncomingMessageHeaders;
            int headerPos = incomingHeaders.FindHeader("StringHeader", "http://tempuri.org/");
            var xmlReader = incomingHeaders.GetReaderAtHeader(headerPos);

            var objs = (object[])s_stringHeaderSerializer.Deserialize(xmlReader, "http://schemas.xmlsoap.org/soap/encoding/");
            if (objs.Length > 0)
            {
                var header = (StringHeader)objs[0];
                MessageHeader responseHeader = CreateHeader(new StringHeader { HeaderValue = header.HeaderValue + header.HeaderValue });
                OperationContext.Current.OutgoingMessageHeaders.Add(responseHeader);
            }

            return new EchoResponse { EchoResult = request.message };
        }

        private MessageHeader CreateHeader(StringHeader header)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlDictionaryWriter bufferWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream);
            bufferWriter.WriteStartElement("root");
            XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
            xs.Add("", "");
            xs.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xs.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            xs.Add("tns", "http://tempuri.org/");
            s_stringHeaderSerializer.Serialize(bufferWriter, new object[] { header }, xs, "http://schemas.xmlsoap.org/soap/encoding/");
            bufferWriter.WriteEndElement();
            bufferWriter.Flush();
            XmlDocument doc = new XmlDocument();
            memoryStream.Position = 0;
            doc.Load(new XmlTextReader(memoryStream) { DtdProcessing = DtdProcessing.Prohibit });
            XmlElement stringHeaderElement = doc.DocumentElement.ChildNodes[0] as XmlElement;
            return new XmlElementMessageHeader("StringHeader", "http://tempuri.org/", stringHeaderElement);
        }

        public class XmlElementMessageHeader : MessageHeader
        {
            MessageHeader innerHeader;
            private XmlElement _headerValue;

            public XmlElementMessageHeader(string name, string ns, XmlElement headerValue)
            {
                innerHeader = CreateHeader(name, ns, null, false, "", false);
                _headerValue = headerValue;
            }

            public override string Name { get { return innerHeader.Name; } }
            public override string Namespace { get { return innerHeader.Namespace; } }
            public override bool MustUnderstand { get { return innerHeader.MustUnderstand; } }
            public override bool Relay { get { return innerHeader.Relay; } }
            public override string Actor { get { return innerHeader.Actor; } }

            protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteStartElement("tns", Name, Namespace);
                writer.WriteStartAttribute("tns", Name, Namespace);
                OnWriteHeaderAttributes(writer, messageVersion);
            }

            protected void OnWriteHeaderAttributes(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                base.WriteHeaderAttributes(writer, messageVersion);
                XmlDictionaryReader nodeReader = XmlDictionaryReader.CreateDictionaryReader(new XmlNodeReader(_headerValue));
                nodeReader.MoveToContent();
                writer.WriteAttributes(nodeReader, false);
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                _headerValue.WriteContentTo(writer);
            }
        }
    }
}
