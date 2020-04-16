// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
#if !NETCORE
    [Serializable]
#endif
    public class MetadataExchangeException : Exception
    {
        public MetadataExchangeException() { }
        public MetadataExchangeException(string message) : base(message) { }
        public MetadataExchangeException(string message, Exception innerException) : base(message, innerException) { }
        public MetadataExchangeException(string format, params object[] args) : base(string.Format(CultureInfo.InvariantCulture, format, args)) { }
#if !NETCORE
        protected MetadataExchangeException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
#endif
    }
}
