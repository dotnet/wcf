// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;

    /// <summary>
    /// Represens a verbosity level.
    /// </summary>
    internal enum Verbosity
    {
        /// <summary>
        /// Only errors displayed.
        /// </summary>
        Silent,

        /// <summary>
        /// Include all the above plus warning and important messages.
        /// </summary>
        Minimal,

        /// <summary>
        /// Include all the above plus informational messages.
        /// </summary>
        Normal,

        /// <summary>
        /// Include all the above plus extra messages.
        /// </summary>
        Verbose,

        /// <summary>
        /// Include all the above plus debugging messages.
        /// </summary>
        Debug
    }
}
