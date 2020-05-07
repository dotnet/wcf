// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal enum TypeReuseMode
    {
        // Type reuse disabled
        None,

        // Use types from references specified by the user only.
        Specified,

        // Use all types from all available references.
        All
    }
}
