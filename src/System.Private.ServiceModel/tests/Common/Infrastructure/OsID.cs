// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Infrastructure.Common
{
    // Enum to indicate a known operating system.
    // The elements in this enum represent all known operating systems
    // against which the product can be run.  Please update this enum
    // as new operating systems become available for testing.
    // It uses [Flags] because it is a bitmask that allows bitwise
    // combinations in scenarios like the [Issue] attribute.

    [Flags]
    public enum OSID
    {
        None =                   0x00000000,

        Windows_7 =              0x00000001,
        Windows_8 =              0x00000002,
        Windows_8_1 =            0x00000004,
        Windows_10 =             0x00000008,
        Windows_Server_2008 =    0x00000010,
        Windows_Server_2008_R2 = 0x00000020,
        Windows_Server_2012 =    0x00000040,
        Windows_Server_2012_R2 = 0x00000080,
        Windows_Server_2016 =    0x00000100,
        WindowsPhone =           0x00000200,
        Windows_Nano =           0x00000400,

        // 'Any' combinations must explicitly name only known flags so "G" formatting
        // can be used to show a comma separated list of the bitmask.
        AnyWindows = Windows_7 | Windows_8 | Windows_8_1 | Windows_10 |
             Windows_Server_2008 | Windows_Server_2008_R2 | Windows_Server_2012 | Windows_Server_2012_R2 | Windows_Server_2016 |
             WindowsPhone | Windows_Nano,

        Mariner =                 0x00000800,
        Debian =                 0x00001000,
        Fedora =                 0x00002000,
        SLES =                   0x00004000,
        OpenSUSE =               0x00008000,
        OSX =                    0x00010000,
        RHEL =                   0x00020000,
        Ubuntu =                 0x00040000,

        // 'Any' combinations must explicitly name only known flags so "G" formatting
        // can be used to show a comma separated list of the bitmask.
        AnyUnix = Mariner | Debian | Fedora | OpenSUSE | OSX | RHEL | Ubuntu,

        Any = AnyUnix | AnyWindows
    }
}
