// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security
{
    [Flags]
    internal enum ReceiveSecurityHeaderBindingModes
    {
        Unknown = 0x0,
        Primary = 0x1,
        Endorsing = 0x2,
        Signed = 0x4,
        SignedEndorsing = 0x8,
        Basic = 0x10,
    }
}
