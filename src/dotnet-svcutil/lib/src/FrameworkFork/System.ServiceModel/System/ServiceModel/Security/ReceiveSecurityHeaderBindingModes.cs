// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
