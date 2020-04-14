// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    public enum OperationFormatUse
    {
        Literal,
        Encoded,
    }

    internal static class OperationFormatUseHelper
    {
        static public bool IsDefined(OperationFormatUse x)
        {
            return
                x == OperationFormatUse.Literal ||
                x == OperationFormatUse.Encoded ||
                false;
        }
    }
}
