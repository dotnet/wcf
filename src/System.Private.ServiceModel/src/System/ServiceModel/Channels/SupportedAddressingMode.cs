// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
