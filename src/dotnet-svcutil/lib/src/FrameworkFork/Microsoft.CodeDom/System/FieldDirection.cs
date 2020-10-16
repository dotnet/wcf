// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Specifies values used to indicate field and parameter directions.
    ///    </para>
    /// </devdoc>
    [
        ComVisible(true),
    // Serializable,
    ]
    public enum FieldDirection
    {
        /// <devdoc>
        ///    <para>
        ///       Incoming field.
        ///    </para>
        /// </devdoc>
        In,
        /// <devdoc>
        ///    <para>
        ///       Outgoing field.
        ///    </para>
        /// </devdoc>
        Out,
        /// <devdoc>
        ///    <para>
        ///       Field by reference.
        ///    </para>
        /// </devdoc>
        Ref,
    }
}
