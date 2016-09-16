// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    internal static class SecurityAuditHelper
    {
        public static bool IsSecurityAuditSupported
        {
            get
            {
                return false;
            }
        }

        public static void WriteMessageAuthenticationSuccessEvent(AuditLogLocation auditLogLocation, bool suppressAuditFailure, Message message,
                                                                  Uri serviceUri, string action, string clientIdentity)
        {
        }

        public static void WriteMessageAuthenticationFailureEvent(AuditLogLocation auditLogLocation, bool suppressAuditFailure, Message message,
                                                                  Uri serviceUri, string action, string clientIdentity, Exception exception)
        {
        }
    }
}

