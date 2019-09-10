// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException()
            : base()
        {
        }

        public QuotaExceededException(string message)
            : base(message)
        {
        }

        public QuotaExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

