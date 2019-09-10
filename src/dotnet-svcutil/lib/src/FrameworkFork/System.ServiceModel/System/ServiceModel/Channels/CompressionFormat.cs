// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Supported compression formats
    /// </summary>
    public enum CompressionFormat
    {
        /// <summary>
        /// Default to compression off
        /// </summary>
        None,

        /// <summary>
        /// GZip compression
        /// </summary>
        GZip,

        /// <summary>
        /// Deflate compression
        /// </summary>
        Deflate,
    }
}
