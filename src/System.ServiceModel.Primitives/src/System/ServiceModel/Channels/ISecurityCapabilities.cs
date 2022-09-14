// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
