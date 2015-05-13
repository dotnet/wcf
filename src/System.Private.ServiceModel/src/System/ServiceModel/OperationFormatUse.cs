// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
