// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [
        ComVisible(true),
    // Serializable,
    ]
    public enum CodeIfMode
    {
        None = 0,
        Start = 1,
        End = 2,
    }
}
