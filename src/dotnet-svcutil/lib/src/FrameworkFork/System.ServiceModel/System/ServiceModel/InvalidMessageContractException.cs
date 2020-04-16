// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

