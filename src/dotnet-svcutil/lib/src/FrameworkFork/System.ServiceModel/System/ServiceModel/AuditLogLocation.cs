// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
