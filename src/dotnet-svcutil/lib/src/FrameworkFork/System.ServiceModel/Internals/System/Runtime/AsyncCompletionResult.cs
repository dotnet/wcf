// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
