//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
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
