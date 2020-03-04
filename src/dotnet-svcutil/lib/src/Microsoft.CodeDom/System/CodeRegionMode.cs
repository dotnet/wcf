// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom {

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [
        ComVisible(true),
        // Serializable,
    ]
    public enum CodeRegionMode {
        None = 0,
        Start = 1,
        End = 2,
    }
}
