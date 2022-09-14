// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    [Flags]
    internal enum UnifiedSecurityMode
    {
        None = 0x001,
        Transport = 0x004,
        Message = 0x008,
        Both = 0x010,
        TransportWithMessageCredential = 0x020,
        TransportCredentialOnly = 0x040,
    }
}
