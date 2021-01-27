// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;

    public class InvalidChannelBindingException : Exception
    {
        public InvalidChannelBindingException() { }
        public InvalidChannelBindingException(string message) : base(message) { }
        public InvalidChannelBindingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
