// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.IdentityModel.Selectors;
#else
using System;
using System.IdentityModel.Selectors;
#endif

namespace WcfService
{
    internal class AcceptAnyUsernamePasswordValidator : UserNamePasswordValidator
    {
        [Obsolete]
        public override void Validate(string userName, string password)
        {
            // Do Nothing as we accept anything
        }
    }
}
