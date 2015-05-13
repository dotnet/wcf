// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    public enum SupportedAddressingMode
    {
        Anonymous,
        NonAnonymous,
        Mixed
    }

    internal static class SupportedAddressingModeHelper
    {
        internal static bool IsDefined(SupportedAddressingMode value)
        {
            return (value == SupportedAddressingMode.Anonymous ||
                value == SupportedAddressingMode.NonAnonymous ||
                value == SupportedAddressingMode.Mixed);
        }
    }
}
