// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Infrastructure.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RetryOnFailureAttribute : Attribute
    {
        public RetryOnFailureAttribute(int maxAttempts, int retryDelayMilliseconds = 0)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "The maximum number of attempts must be at least 1.");
            }

            if (retryDelayMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryDelayMilliseconds), retryDelayMilliseconds, "The retry delay must be non-negative.");
            }

            MaxAttempts = maxAttempts;
            RetryDelayMilliseconds = retryDelayMilliseconds;
        }

        public int MaxAttempts { get; }

        public int RetryDelayMilliseconds { get; }
    }
}
