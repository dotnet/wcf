// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
