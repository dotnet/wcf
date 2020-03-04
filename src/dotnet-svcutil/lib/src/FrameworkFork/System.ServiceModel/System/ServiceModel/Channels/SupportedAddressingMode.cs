// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
