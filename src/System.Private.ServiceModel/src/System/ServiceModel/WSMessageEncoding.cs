// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public enum WSMessageEncoding
    {
        Text = 0,
    }

    internal static class WSMessageEncodingHelper
    {
        internal static bool IsDefined(WSMessageEncoding value)
        {
            return value == WSMessageEncoding.Text;
        }
    }
}
