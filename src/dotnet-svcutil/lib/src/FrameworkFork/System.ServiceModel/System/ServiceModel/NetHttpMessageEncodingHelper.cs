// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    internal static class NetHttpMessageEncodingHelper
    {
        internal static bool IsDefined(NetHttpMessageEncoding value)
        {
            return
                value == NetHttpMessageEncoding.Binary
                || value == NetHttpMessageEncoding.Text
                || value == NetHttpMessageEncoding.Mtom;
        }
    }
}
