// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
    public class InvalidMessageContractException : Exception
    {
        public InvalidMessageContractException()
            : base()
        {
        }

        public InvalidMessageContractException(String message)
            : base(message)
        {
        }

        public InvalidMessageContractException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

