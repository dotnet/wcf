// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum ReceiveErrorHandling
    {
        Fault,
        Drop,
        Reject,
        Move
    }

    internal static class ReceiveErrorHandlingHelper
    {
        internal static bool IsDefined(ReceiveErrorHandling value)
        {
            return value == ReceiveErrorHandling.Fault ||
                value == ReceiveErrorHandling.Drop ||
                value == ReceiveErrorHandling.Reject ||
                value == ReceiveErrorHandling.Move;
        }
    }
}
