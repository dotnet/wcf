// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel.Diagnostics
{
    internal class SecurityTraceRecord : TraceRecord
    {
        private string _traceName;
        internal SecurityTraceRecord(string traceName)
        {
            if (string.IsNullOrEmpty(traceName))
                _traceName = "Empty";
            else
                _traceName = traceName;
        }

        internal override string EventId { get { return BuildEventId(_traceName); } }
    }

    internal static class SecurityTraceRecordHelper
    {
        internal static void TraceIdentityVerificationSuccess(EventTraceActivity eventTraceActivity, EndpointIdentity identity, Claim claim, Type identityVerifier)
        {
        }

        internal static void TraceIdentityVerificationFailure(EndpointIdentity identity, AuthorizationContext authContext, Type identityVerifier)
        {
        }

        internal static void TraceIdentityDeterminationSuccess(EndpointAddress epr, EndpointIdentity identity, Type identityVerifier)
        {
        }

        internal static void TraceIdentityDeterminationFailure(EndpointAddress epr, Type identityVerifier)
        {
        }

        internal static void TraceOutgoingMessageSecured(SecurityProtocol binding, Message message)
        {
        }

        internal static void TraceVerifyIncomingMessageFailure(SecurityProtocol binding, Message message)
        {
        }

        internal static void TraceSecureOutgoingMessageFailure(SecurityProtocol binding, Message message)
        {
        }

        internal static void TraceIncomingMessageVerified(SecurityProtocol binding, Message message)
        {
        }

        internal static void TraceCloseMessageSent(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
        }

        internal static void TraceCloseResponseMessageSent(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
        }
        
        internal static void TraceSpnToSidMappingFailure(string spn, Exception e)
        {
        }
    }
}
