// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Infrastructure.Common
{
    // Enum to indicate a Framework.
    // There must be a 1:1 match between all the elements in
    // this enum and RuntimeInformation.FrameworkDescription.
    // It uses [Flags] because it is a bitmask that allows bitwise
    // combinations in scenarios like the [Issue] attribute.
    [Flags]
    public enum FrameworkID
    {
        None =         0x00000000,
        NetFramework = 0x00000001,   // Net 4.5 || Win8
        NetCore =      0x00000002,   // netcore50 || wpa81 || other
        NetNative =    0x00000004,   // netcore50aot

        // 'Any' explicitly names only known flags so "G" formatting
        // can be used to show a comma separated list of the bitmask.
        Any = NetFramework | NetCore | NetNative
    }
}
