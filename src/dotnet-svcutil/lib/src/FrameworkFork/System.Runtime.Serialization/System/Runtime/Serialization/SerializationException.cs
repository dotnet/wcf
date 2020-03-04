// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace System.Runtime.Serialization
{
    public class SerializationException : Exception
    {
        public SerializationException() { }

        public SerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = __HResults.COR_E_SYSTEM;
        }

        public SerializationException(string message)
            : base(message)
        {
            HResult = __HResults.COR_E_SYSTEM;
        }
    }
}
