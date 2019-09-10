// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//------------------------------------------------------------

//------------------------------------------------------------

namespace System.ServiceModel
{
    public enum OperationFormatStyle
    {
        Document,
        Rpc,
    }

    internal static class OperationFormatStyleHelper
    {
        static public bool IsDefined(OperationFormatStyle x)
        {
            return
                x == OperationFormatStyle.Document ||
                x == OperationFormatStyle.Rpc ||
                false;
        }
    }
}
