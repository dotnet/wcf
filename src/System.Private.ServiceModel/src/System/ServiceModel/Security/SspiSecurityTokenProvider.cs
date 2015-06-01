// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    public class SspiSecurityTokenProvider
    {
        internal const bool DefaultAllowNtlm = true;
        internal const bool DefaultExtractWindowsGroupClaims = true;
        internal const bool DefaultAllowUnauthenticatedCallers = false;
    }
}
