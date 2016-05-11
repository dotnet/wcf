// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.InteropServices;

namespace Infrastructure.Common
{
    // This class contains helper "detector" methods to detect specific
    // conditions about the environment in which the tests are run.
    // They are used for [ConditionalFact] and [ConditionalTheory]
    // tests.  They are called at most once, and the return value cached.
    internal static class ConditionalTestDetectors
    {
        // Detector used by [ConditionalFact(nameof(Root_Certificate_Installed)].
        // It will attempt to install the root certificate in the root store if
        // is not already present, and then it will check whether the install
        // succeeded.  A 'true' return is a guarantee a root certificate is
        // installed in the root store.
        public static bool IsRootCertificateInstalled()
        {
            return ServiceUtilHelper.TryEnsureRootCertificateInstalled();
        }

        // Detector used by [ConditionalFact(nameof(Client_Certificate_Installed)].
        // It will attempt to install the client certificate in the certificate store if
        // is not already present, and then it will check whether the install
        // succeeded.  A 'true' return is a guarantee a client certificate is
        // installed in the certificate store.
        public static bool IsClientCertificateInstalled()
        {
            return ServiceUtilHelper.TryEnsureLocalClientCertificateInstalled();
        }

        public static bool IsClientDomainJoined()
        {
            // Currently non-Windows clients always return false until we have
            // a cross-platform way to detect this.
            if (!IsWindows())
            {
                return false;
            }

            // To determine whether the client is running on a domain joined machine,
            // we use this heuristic based on well known environment variables.
            string computerName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            string logonServer = Environment.GetEnvironmentVariable("LOGONSERVER");
            string userDomain = Environment.GetEnvironmentVariable("USERDOMAIN");

            return !String.Equals(computerName, logonServer, StringComparison.OrdinalIgnoreCase) &&
                   !String.Equals(computerName, userDomain, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsServerDomainJoined()
        {
            // Requires solution to https://github.com/dotnet/wcf/issues/1095
            // Till then, heuristic is that running localhost == domain joined if client is domain joined
            return IsServerLocalHost() && IsClientDomainJoined();
        }

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        // This test is meant to be used only to detect that our service URI
        // indicates localhost.  It is not intended to be used to detect that
        // the services are running locally but using an explicit host name.
        public static bool IsServerLocalHost()
        {
            string host = TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName);

            if (String.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            int index = host.IndexOf("localhost", 0, StringComparison.OrdinalIgnoreCase);
            return index == 0;
        }

        public static bool AreUserNameAndPasswordAvailable()
        {
            return !String.IsNullOrWhiteSpace(TestProperties.GetProperty(TestProperties.TestUserName_PropertyName)) &&
                   !String.IsNullOrWhiteSpace(TestProperties.GetProperty(TestProperties.TestPassword_PropertyName));
        }
    }
}
