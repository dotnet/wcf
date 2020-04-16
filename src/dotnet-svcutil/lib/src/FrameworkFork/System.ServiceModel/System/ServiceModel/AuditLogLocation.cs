// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security;
using System.ComponentModel;

namespace System.ServiceModel
{
    public enum AuditLogLocation
    {
        Default,
        Application,
        Security,
    }

    internal static class AuditLogLocationHelper
    {
        public static bool IsDefined(AuditLogLocation auditLogLocation)
        {
            if (auditLogLocation == AuditLogLocation.Security && !SecurityAuditHelper.IsSecurityAuditSupported)
                throw ExceptionHelper.PlatformNotSupported(SRServiceModel.SecurityAuditPlatformNotSupported);

            return auditLogLocation == AuditLogLocation.Default
                || auditLogLocation == AuditLogLocation.Application
                || auditLogLocation == AuditLogLocation.Security;
        }

        public static void Validate(AuditLogLocation value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(AuditLogLocation)));
            }
        }
    }
}
