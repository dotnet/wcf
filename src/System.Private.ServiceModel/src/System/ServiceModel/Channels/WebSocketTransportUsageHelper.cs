// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    internal static class WebSocketTransportUsageHelper
    {
        internal static bool IsDefined(WebSocketTransportUsage value)
        {
            return value == WebSocketTransportUsage.WhenDuplex
                || value == WebSocketTransportUsage.Never
                || value == WebSocketTransportUsage.Always;
        }

        internal static void Validate(WebSocketTransportUsage value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidEnumArgumentException("value", (int)value, typeof(WebSocketTransportUsage)));
            }
        }
    }
}
