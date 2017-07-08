// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    static class UriUtils
    {
        public static Uri Combine(Uri rootBase, string newBase)
        {
            if (string.IsNullOrEmpty(newBase))
            {
                return rootBase;
            }

            Uri newBaseUri = new Uri(newBase, UriKind.RelativeOrAbsolute);

            if (rootBase == null || newBaseUri.IsAbsoluteUri)
            {
                return newBaseUri;
            }

            return new Uri(rootBase, newBase);
        }
    }
}
