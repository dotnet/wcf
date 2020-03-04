// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;


namespace System.Runtime.Serialization
{
    public class InvalidDataContractException : Exception
    {
        public InvalidDataContractException() : base()
        {
        }

        public InvalidDataContractException(String message) : base(message)
        {
        }

        public InvalidDataContractException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

