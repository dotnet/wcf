// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Channels
{
    internal static class ExtendedProtectionPolicyHelper
    {
        private static readonly ExtendedProtectionPolicy s_disabledPolicy = new ExtendedProtectionPolicy(PolicyEnforcement.Never);

        public static ExtendedProtectionPolicy DefaultPolicy { get; } = s_disabledPolicy;
    }
}
