// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.Security.Claims;
using System.Security.Principal;

namespace System.IdentityModel
{
	internal static partial class SecurityUtils
	{
        public const string Identities = "Identities";
        private static IIdentity s_anonymousIdentity;

        internal static IIdentity AnonymousIdentity
        {
            get
            {
                if (s_anonymousIdentity == null)
                {
                    s_anonymousIdentity = new GenericIdentity(string.Empty);
                }
                return s_anonymousIdentity;
            }
        }

        public static DateTime MaxUtcDateTime
        {
            get
            {
                // + and -  TimeSpan.TicksPerDay is to compensate the DateTime.ParseExact (to localtime) overflow.
                return new DateTime(DateTime.MaxValue.Ticks - TimeSpan.TicksPerDay, DateTimeKind.Utc);
            }
        }

        public static void DisposeIfNecessary(IDisposable obj)
        {
            obj?.Dispose();
        }
    }
}
