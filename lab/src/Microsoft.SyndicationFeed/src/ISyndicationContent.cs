// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationContent
    {
        Stream Stream { get; }

        //
        // TODO:
        // Add support for Extensions
    }
}
