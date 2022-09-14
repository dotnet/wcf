// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    internal static class BinaryEncodingString
    {
        public const string Binary = "application/soap+msbin1";
        public const string BinarySession = "application/soap+msbinsession1";
        public const string ExtendedBinaryGZip = Binary + "+gzip";
        public const string ExtendedBinarySessionGZip = BinarySession + "+gzip";
        public const string ExtendedBinaryDeflate = Binary + "+deflate";
        public const string ExtendedBinarySessionDeflate = BinarySession + "+deflate";
        public const string NamespaceUri = "http://schemas.microsoft.com/ws/2006/05/framing";
    }
}
