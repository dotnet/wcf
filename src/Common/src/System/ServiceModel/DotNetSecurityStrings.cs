// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    internal static class DotNetSecurityStrings
    {
        // Main dictionary strings
        public const string Namespace = "http://schemas.microsoft.com/ws/2006/05/security"; // ServiceModelStringsVersion1.String162;
        public const string Prefix = "dnse"; // ServiceModelStringsVersion1.String163;
        // String constants
        public const string KeyRenewalNeededFault = "ExpiredSecurityContextTokenKey";
        public const string SecuritySessionAbortedFault = "SecuritySessionAborted";
        public const string SecurityServerTooBusyFault = "ServerTooBusy";
        public const string SecuritySessionFaultAction = "http://schemas.microsoft.com/ws/2006/05/security/SecureConversationFault";
        public const string SecureConversationCancelNotAllowedFault = "SecureConversationCancellationNotAllowed";
    }
}
