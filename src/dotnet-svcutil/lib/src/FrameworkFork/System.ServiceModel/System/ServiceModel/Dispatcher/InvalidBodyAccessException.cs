// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Dispatcher
{
    internal abstract class InvalidBodyAccessException : Exception
    {
        protected InvalidBodyAccessException(string message)
            : this(message, null)
        {
        }

        protected InvalidBodyAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
