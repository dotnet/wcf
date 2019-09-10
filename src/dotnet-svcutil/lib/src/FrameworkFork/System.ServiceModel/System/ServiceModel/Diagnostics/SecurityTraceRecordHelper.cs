// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Runtime.Diagnostics;

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
    }
}
