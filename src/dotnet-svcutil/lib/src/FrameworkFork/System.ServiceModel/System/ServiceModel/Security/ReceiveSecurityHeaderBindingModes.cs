// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
