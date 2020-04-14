// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
