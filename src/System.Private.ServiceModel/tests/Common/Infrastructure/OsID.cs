// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Infrastructure.Common
{
    // Enum to indicate a known operating system.
    // The elements in this enum represent all known operating systems
    // against which the product can be run.  Please update this enum
    // as new operating systems bceome available for testing.
    // It uses [Flags] because it is a bitmask that allows bitwise
    // combinations in scenarios like the [Issue] attribute.

    [Flags]
    public enum OSID
    {
        None =            0x00000000,

        Windows_7 =       0x00000001,
        Windows_8 =       0x00000002,
        Windows_8_1 =     0x00000004,
        Windows_10 =      0x00000008,
        Windows_2008_R2 = 0x00000010,
        WindowsPhone =    0x00000020,
        Windows_Nano =    0x00000040,

        CentOS_7 =        0x00000080,
        CentOS_7_1 =      0x00000100,
        AnyCentOS = CentOS_7 | CentOS_7_1,

        Debian_8 =        0x00000200,
        Debian_8_2 =      0x00000400, 
        AnyDebian = Debian_8 | Debian_8_2,

        Fedora_23 =       0x00000800,
        AnyFedora = Fedora_23,

        OpenSUSE_13_2 =   0x00001000,
        AnyOpenSUSE = OpenSUSE_13_2,

        OSX_10_10 =       0x00002000,
        OSX_10_11 =       0x00004000,
        AnyOSX = OSX_10_10 | OSX_10_11,

        RHEL_7 =          0x00008000,
        AnyRHEL = RHEL_7,

        Ubuntu_14_04 =    0x00010000,
        Ubuntu_16_04 =    0x00020000,
        AnyUbuntu = Ubuntu_14_04 | Ubuntu_16_04,

        AnyUnix = AnyCentOS | AnyDebian | AnyFedora | AnyOpenSUSE | AnyOSX | AnyRHEL | AnyUbuntu,
        AnyWindows = Windows_7 | Windows_8 | Windows_8_1 | Windows_2008_R2 | Windows_10 | WindowsPhone | Windows_Nano,

        // 'Any' explicitly names only known flags so "G" formatting
        // can be used to show a comma separated list of the bitmask.
        Any = AnyUnix | AnyWindows
    }
}
