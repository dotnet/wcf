// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml
{
    using System;

    // Specifies the type of node change
    public enum XmlNodeChangedAction
    {
        // A node is beeing inserted in the tree.
        Insert = 0,

        // A node is beeing removed from the tree.
        Remove = 1,

        // A node value is beeing changed.
        Change = 2
    }
}
