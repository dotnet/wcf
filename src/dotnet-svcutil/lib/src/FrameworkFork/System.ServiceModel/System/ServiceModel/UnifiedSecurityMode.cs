// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    [Flags]
    public enum UnifiedSecurityMode
    {
        None = 0x001,
        Transport = 0x004,
        Message = 0x008,
        Both = 0x010,
        TransportWithMessageCredential = 0x020,
        TransportCredentialOnly = 0x040,
    }
}
