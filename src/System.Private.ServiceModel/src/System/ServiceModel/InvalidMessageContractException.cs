// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

