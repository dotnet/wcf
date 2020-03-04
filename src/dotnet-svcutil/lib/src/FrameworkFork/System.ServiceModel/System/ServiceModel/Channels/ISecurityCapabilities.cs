// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Net.Security;

namespace System.ServiceModel.Channels
{
    public interface ISecurityCapabilities
    {
        ProtectionLevel SupportedRequestProtectionLevel { get; }
        ProtectionLevel SupportedResponseProtectionLevel { get; }
        bool SupportsClientAuthentication { get; }
        bool SupportsClientWindowsIdentity { get; }
        bool SupportsServerAuthentication { get; }
    }
}
