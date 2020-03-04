// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
