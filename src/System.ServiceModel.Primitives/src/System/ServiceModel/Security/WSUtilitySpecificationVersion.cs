// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security
{
    using System.ServiceModel;
    using System.IO;
    using System.Text;
    using System.Xml;

    using CanonicalFormWriter = IdentityModel.CanonicalFormWriter;
    using SignatureResourcePool = IdentityModel.SignatureResourcePool;
    using HashStream = IdentityModel.HashStream;

    internal abstract class WSUtilitySpecificationVersion
    {
        internal static string[] AcceptedDateTimeFormats { get; } = new string[]
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

        internal WSUtilitySpecificationVersion(XmlDictionaryString namespaceUri)
        {
            NamespaceUri = namespaceUri;
        }

        public static WSUtilitySpecificationVersion Default
        {
            get { return OneDotZero; }
        }

        internal XmlDictionaryString NamespaceUri { get; }

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
            private WSUtilitySpecificationVersionOneDotZero()
                : base(XD.UtilityDictionary.Namespace)
            {
            }

            public static WSUtilitySpecificationVersionOneDotZero Instance { get; } = new WSUtilitySpecificationVersionOneDotZero();

            internal override bool IsReaderAtTimestamp(XmlDictionaryReader reader)
            {
                return reader.IsStartElement(XD.UtilityDictionary.Timestamp, XD.UtilityDictionary.Namespace);
            }

            internal override SecurityTimestamp ReadTimestamp(XmlDictionaryReader reader, string digestAlgorithm, SignatureResourcePool resourcePool)
            {
                bool canonicalize = digestAlgorithm != null && reader.CanCanonicalize;
                HashStream hashStream = null;

                reader.MoveToStartElement(XD.UtilityDictionary.Timestamp, XD.UtilityDictionary.Namespace);
                if (canonicalize)
                {
                    hashStream = resourcePool.TakeHashStream(digestAlgorithm);
                    reader.StartCanonicalization(hashStream, false, null);
                }
                string id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                reader.ReadStartElement();

                reader.ReadStartElement(XD.UtilityDictionary.CreatedElement, XD.UtilityDictionary.Namespace);
                DateTime creationTimeUtc = reader.ReadContentAsDateTime().ToUniversalTime();
                reader.ReadEndElement();

                DateTime expiryTimeUtc;
                if (reader.IsStartElement(XD.UtilityDictionary.ExpiresElement, XD.UtilityDictionary.Namespace))
                {
                    reader.ReadStartElement();
                    expiryTimeUtc = reader.ReadContentAsDateTime().ToUniversalTime();
                    reader.ReadEndElement();
                }
                else
                {
                    expiryTimeUtc = SecurityUtils.MaxUtcDateTime;
                }

                reader.ReadEndElement();

                byte[] digest;
                if (canonicalize)
                {
                    reader.EndCanonicalization();
                    digest = hashStream.FlushHashAndGetValue();
                }
                else
                {
                    digest = null;
                }
                return new SecurityTimestamp(creationTimeUtc, expiryTimeUtc, id, digestAlgorithm, digest);
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
            private const string Timestamp = UtilityStrings.Prefix + ":" + UtilityStrings.Timestamp;
            private const string Created = UtilityStrings.Prefix + ":" + UtilityStrings.CreatedElement;
            private const string Expires = UtilityStrings.Prefix + ":" + UtilityStrings.ExpiresElement;
            private const string IdAttribute = UtilityStrings.Prefix + ":" + UtilityStrings.IdAttribute;
            private const string Ns = "xmlns:" + UtilityStrings.Prefix + "=\"" + UtilityStrings.Namespace + "\"";

            private const string Xml1 = "<" + Timestamp + " " + Ns + " " + IdAttribute + "=\"";
            private const string Xml2 = "\"><" + Created + ">";
            private const string Xml3 = "</" + Created + "><" + Expires + ">";
            private const string Xml4 = "</" + Expires + "></" + Timestamp + ">";

            private readonly byte[] _fragment1;
            private readonly byte[] _fragment2;
            private readonly byte[] _fragment3;
            private readonly byte[] _fragment4;

            private TimestampCanonicalFormWriter()
            {
                Encoding encoding = CanonicalFormWriter.Utf8WithoutPreamble;
                _fragment1 = encoding.GetBytes(Xml1);
                _fragment2 = encoding.GetBytes(Xml2);
                _fragment3 = encoding.GetBytes(Xml3);
                _fragment4 = encoding.GetBytes(Xml4);
            }

            public static TimestampCanonicalFormWriter Instance { get; } = new TimestampCanonicalFormWriter();

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

