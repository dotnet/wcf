// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Xml.Serialization;

namespace System.ServiceModel.MsmqIntegration
{
    // Turns MsmqIntegrationMessageProperty.Body into the raw bytes that go on
    // the wire as the MSMQ message body, honouring
    // MsmqIntegrationBindingElement.SerializationFormat.
    //
    // MsmqIntegrationBinding deliberately carries no message encoder: it talks
    // to classic MSMQ applications that exchange raw payloads rather than SOAP
    // envelopes, so the payload shape is chosen entirely by SerializationFormat
    // (this mirrors .NET Framework's MsmqIntegrationChannelFactory.Serialize).
    //
    // Two of the five .NET Framework formats cannot be carried across:
    //   * Binary  — used BinaryFormatter, which is removed from modern .NET.
    //   * ActiveX — used an internal COM type serializer with no .NET Core
    //               equivalent.
    // Both throw rather than silently emitting a payload the receiver cannot
    // interpret.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqIntegrationSerializer
    {
        // .NET Framework capped its serializer cache at 1024 entries to bound
        // the dynamic assemblies XmlSerializer generates per type.
        private const int MaxSerializerCacheSize = 1024;

        private readonly MsmqMessageSerializationFormat _format;
        private readonly ConcurrentDictionary<Type, XmlSerializer> _xmlSerializers =
            new ConcurrentDictionary<Type, XmlSerializer>();

        internal MsmqIntegrationSerializer(MsmqMessageSerializationFormat format)
        {
            _format = format;
        }

        internal MsmqMessageSerializationFormat SerializationFormat => _format;

        internal byte[] Serialize(MsmqIntegrationMessageProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (property.Body == null)
            {
                return Array.Empty<byte>();
            }

            switch (_format)
            {
                case MsmqMessageSerializationFormat.Xml:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        GetXmlSerializer(property.Body.GetType()).Serialize(stream, property.Body);
                        return stream.ToArray();
                    }

                case MsmqMessageSerializationFormat.ByteArray:
                    byte[] bytes = property.Body as byte[];
                    if (bytes == null)
                    {
                        throw new SerializationException(SR.MsmqByteArrayBodyExpected);
                    }
                    return bytes;

                case MsmqMessageSerializationFormat.Stream:
                    Stream bodyStream = property.Body as Stream;
                    if (bodyStream == null)
                    {
                        throw new SerializationException(SR.MsmqStreamBodyExpected);
                    }
                    return ReadAllBytes(bodyStream);

                case MsmqMessageSerializationFormat.Binary:
                case MsmqMessageSerializationFormat.ActiveX:
                    throw new PlatformNotSupportedException(
                        SR.Format(SR.MsmqSerializationFormatNotSupported, _format));

                default:
                    throw new SerializationException(
                        SR.Format(SR.MsmqUnsupportedSerializationFormat, _format));
            }
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            using (MemoryStream buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                return buffer.ToArray();
            }
        }

        private XmlSerializer GetXmlSerializer(Type bodyType)
        {
            if (_xmlSerializers.TryGetValue(bodyType, out XmlSerializer cached))
            {
                return cached;
            }
            if (_xmlSerializers.Count >= MaxSerializerCacheSize)
            {
                throw new CommunicationException(
                    SR.Format(SR.MsmqSerializationTableFull, MaxSerializerCacheSize));
            }
            return _xmlSerializers.GetOrAdd(bodyType, static type => new XmlSerializer(type));
        }
    }
}
