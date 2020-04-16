// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.IO;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.ServiceModel.Security.Tokens;
    using System.Threading;
    using System.Globalization;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using Microsoft.Xml;

    using CanonicalFormWriter = System.IdentityModel.CanonicalFormWriter;
    using SignatureResourcePool = System.IdentityModel.SignatureResourcePool;
    using HashStream = System.IdentityModel.HashStream;

    internal abstract class WSUtilitySpecificationVersion
    {
        internal static readonly string[] AcceptedDateTimeFormats = new string[]
        {
            "yyyy-MM-ddTHH:mm:ss.fffffffZ",
            "yyyy-MM-ddTHH:mm:ss.ffffffZ",
            "yyyy-MM-ddTHH:mm:ss.fffffZ",
            "yyyy-MM-ddTHH:mm:ss.ffffZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ss.ffZ",
            "yyyy-MM-ddTHH:mm:ss.fZ",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        private readonly XmlDictionaryString _namespaceUri;

        internal WSUtilitySpecificationVersion(XmlDictionaryString namespaceUri)
        {
            _namespaceUri = namespaceUri;
        }

        public static WSUtilitySpecificationVersion Default
        {
            get { return OneDotZero; }
        }

        internal XmlDictionaryString NamespaceUri
        {
            get { return _namespaceUri; }
        }

        public static WSUtilitySpecificationVersion OneDotZero
        {
            get { return WSUtilitySpecificationVersionOneDotZero.Instance; }
        }

        internal abstract bool IsReaderAtTimestamp(XmlDictionaryReader reader);

        internal abstract SecurityTimestamp ReadTimestamp(XmlDictionaryReader reader, string digestAlgorithm, SignatureResourcePool resourcePool);

        internal abstract void WriteTimestamp(XmlDictionaryWriter writer, SecurityTimestamp timestamp);

        internal abstract void WriteTimestampCanonicalForm(Stream stream, SecurityTimestamp timestamp, byte[] buffer);

        private sealed class WSUtilitySpecificationVersionOneDotZero : WSUtilitySpecificationVersion
        {
            private static readonly WSUtilitySpecificationVersionOneDotZero s_instance = new WSUtilitySpecificationVersionOneDotZero();

            private WSUtilitySpecificationVersionOneDotZero()
                : base(XD.UtilityDictionary.Namespace)
            {
            }

            public static WSUtilitySpecificationVersionOneDotZero Instance
            {
                get { return s_instance; }
            }

            internal override bool IsReaderAtTimestamp(XmlDictionaryReader reader)
            {
                return reader.IsStartElement(XD.UtilityDictionary.Timestamp, XD.UtilityDictionary.Namespace);
            }

            internal override SecurityTimestamp ReadTimestamp(XmlDictionaryReader reader, string digestAlgorithm, SignatureResourcePool resourcePool)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            internal override void WriteTimestamp(XmlDictionaryWriter writer, SecurityTimestamp timestamp)
            {
                writer.WriteStartElement(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.Timestamp, XD.UtilityDictionary.Namespace);
                writer.WriteAttributeString(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, timestamp.Id);

                writer.WriteStartElement(XD.UtilityDictionary.CreatedElement, XD.UtilityDictionary.Namespace);
                char[] creationTime = timestamp.GetCreationTimeChars();
                writer.WriteChars(creationTime, 0, creationTime.Length);
                writer.WriteEndElement(); // wsu:Created

                writer.WriteStartElement(XD.UtilityDictionary.ExpiresElement, XD.UtilityDictionary.Namespace);
                char[] expiryTime = timestamp.GetExpiryTimeChars();
                writer.WriteChars(expiryTime, 0, expiryTime.Length);
                writer.WriteEndElement(); // wsu:Expires

                writer.WriteEndElement();
            }

            internal override void WriteTimestampCanonicalForm(Stream stream, SecurityTimestamp timestamp, byte[] workBuffer)
            {
                TimestampCanonicalFormWriter.Instance.WriteCanonicalForm(
                    stream,
                    timestamp.Id, timestamp.GetCreationTimeChars(), timestamp.GetExpiryTimeChars(),
                    workBuffer);
            }
        }

        private sealed class TimestampCanonicalFormWriter : CanonicalFormWriter
        {
            private const string timestamp = UtilityStrings.Prefix + ":" + UtilityStrings.Timestamp;
            private const string created = UtilityStrings.Prefix + ":" + UtilityStrings.CreatedElement;
            private const string expires = UtilityStrings.Prefix + ":" + UtilityStrings.ExpiresElement;
            private const string idAttribute = UtilityStrings.Prefix + ":" + UtilityStrings.IdAttribute;
            private const string ns = "xmlns:" + UtilityStrings.Prefix + "=\"" + UtilityStrings.Namespace + "\"";

            private const string xml1 = "<" + timestamp + " " + ns + " " + idAttribute + "=\"";
            private const string xml2 = "\"><" + created + ">";
            private const string xml3 = "</" + created + "><" + expires + ">";
            private const string xml4 = "</" + expires + "></" + timestamp + ">";

            private readonly byte[] _fragment1;
            private readonly byte[] _fragment2;
            private readonly byte[] _fragment3;
            private readonly byte[] _fragment4;

            private static readonly TimestampCanonicalFormWriter s_instance = new TimestampCanonicalFormWriter();

            private TimestampCanonicalFormWriter()
            {
                Encoding encoding = CanonicalFormWriter.Utf8WithoutPreamble;
                _fragment1 = encoding.GetBytes(xml1);
                _fragment2 = encoding.GetBytes(xml2);
                _fragment3 = encoding.GetBytes(xml3);
                _fragment4 = encoding.GetBytes(xml4);
            }

            public static TimestampCanonicalFormWriter Instance
            {
                get { return s_instance; }
            }

            public void WriteCanonicalForm(Stream stream, string id, char[] created, char[] expires, byte[] workBuffer)
            {
                stream.Write(_fragment1, 0, _fragment1.Length);
                EncodeAndWrite(stream, workBuffer, id);
                stream.Write(_fragment2, 0, _fragment2.Length);
                EncodeAndWrite(stream, workBuffer, created);
                stream.Write(_fragment3, 0, _fragment3.Length);
                EncodeAndWrite(stream, workBuffer, expires);
                stream.Write(_fragment4, 0, _fragment4.Length);
            }
        }
    }
}

