// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
    internal sealed class CanonicalizationDriver
    {
        private XmlReader _reader;
        private string[] _inclusivePrefixes;

        public bool CloseReadersAfterProcessing { get; set; }

        public bool IncludeComments { get; set; }

        public string[] GetInclusivePrefixes() => _inclusivePrefixes;

        public void Reset() => _reader = null;

        public void SetInclusivePrefixes(string[] inclusivePrefixes) => _inclusivePrefixes = inclusivePrefixes;

        public void SetInput(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
            }

            _reader = XmlReader.Create(stream);
        }

        public byte[] GetBytes() => GetMemoryStream().ToArray();

        public MemoryStream GetMemoryStream()
        {
            MemoryStream stream = new MemoryStream();
            WriteTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public void WriteTo(HashAlgorithm hashAlgorithm)
        {
            WriteTo(new HashStream(hashAlgorithm));
        }

        public void WriteTo(Stream canonicalStream)
        {
            if (_reader != null)
            {
                XmlDictionaryReader dicReader = _reader as XmlDictionaryReader;
                if ((dicReader != null) && (dicReader.CanCanonicalize))
                {
                    dicReader.MoveToContent();
                    dicReader.StartCanonicalization(canonicalStream, IncludeComments, _inclusivePrefixes);
                    dicReader.Skip();
                    dicReader.EndCanonicalization();
                }
                else
                {
                    XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(Stream.Null);
                    if (_inclusivePrefixes != null)
                    {
                        // Add a dummy element at the top and populate the namespace
                        // declaration of all the inclusive prefixes.
                        writer.WriteStartElement("a", _reader.LookupNamespace(String.Empty));
                        for (int i = 0; i < _inclusivePrefixes.Length; ++i)
                        {
                            string ns = _reader.LookupNamespace(_inclusivePrefixes[i]);
                            if (ns != null)
                            {
                                writer.WriteXmlnsAttribute(_inclusivePrefixes[i], ns);
                            }
                        }
                    }
                    writer.StartCanonicalization(canonicalStream, IncludeComments, _inclusivePrefixes);
                    writer.WriteNode(_reader, false);
                    writer.Flush();
                    writer.EndCanonicalization();

                    if (_inclusivePrefixes != null)
                        writer.WriteEndElement();

                    writer.Close();
                }
                if (CloseReadersAfterProcessing)
                {
                    _reader.Close();
                }
                _reader = null;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NoInputIsSetForCanonicalization));
            }
        }
    }
}
