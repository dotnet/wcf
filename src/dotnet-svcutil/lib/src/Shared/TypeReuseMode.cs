// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
