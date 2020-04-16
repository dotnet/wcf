// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
