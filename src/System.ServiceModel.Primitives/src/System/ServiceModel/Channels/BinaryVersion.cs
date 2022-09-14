// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel.Channels
{
    public class BinaryVersion
    {
        static public readonly BinaryVersion Version1 = new BinaryVersion(BinaryEncodingString.Binary, BinaryEncodingString.BinarySession, ServiceModelDictionary.Version1);
        static public readonly BinaryVersion GZipVersion1 = new BinaryVersion(BinaryEncodingString.ExtendedBinaryGZip, BinaryEncodingString.ExtendedBinarySessionGZip, ServiceModelDictionary.Version1);
        static public readonly BinaryVersion DeflateVersion1 = new BinaryVersion(BinaryEncodingString.ExtendedBinaryDeflate, BinaryEncodingString.ExtendedBinarySessionDeflate, ServiceModelDictionary.Version1);
        private IXmlDictionary _dictionary;

        private BinaryVersion(string contentType, string sessionContentType, IXmlDictionary dictionary)
        {
            ContentType = contentType;
            SessionContentType = sessionContentType;
            _dictionary = dictionary;
        }

        static public BinaryVersion CurrentVersion { get { return Version1; } }
        public string ContentType { get; }
        public string SessionContentType { get; }
        public IXmlDictionary Dictionary { get { return _dictionary; } }
    }
}
