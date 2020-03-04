// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
