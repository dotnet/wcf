// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
