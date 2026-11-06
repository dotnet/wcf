// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    public class UriTemplateMatchException : SystemException
    {
        public UriTemplateMatchException()
        {
        }

        public UriTemplateMatchException(string message)
            : base(message)
        {
        }

        public UriTemplateMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
        protected UriTemplateMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
