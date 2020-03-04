// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
