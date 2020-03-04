// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
