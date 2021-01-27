// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime
{
    public enum AsyncCompletionResult
    {
        /// <summary>
        /// Inidicates that the operation has been queued for completion.
        /// </summary>
        Queued,

        /// <summary>
        /// Indicates the operation has completed.
        /// </summary>
        Completed,
    }
}
