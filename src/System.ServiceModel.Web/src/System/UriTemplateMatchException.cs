// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System
{
    using System.Runtime.Serialization;
    using System.Runtime.CompilerServices;

    [TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
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
        protected UriTemplateMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
