// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel.Channels
{
    internal static class UnsafeNativeMethods
    {
        // WinError.h codes:
        internal const int ERROR_FILE_NOT_FOUND = 0x2;
        internal const int ERROR_BROKEN_PIPE = 109;
        internal const int ERROR_PIPE_BUSY = 231;
        internal const int ERROR_NO_DATA = 232;
    }
}
