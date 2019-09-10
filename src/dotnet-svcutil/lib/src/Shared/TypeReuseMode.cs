//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

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
