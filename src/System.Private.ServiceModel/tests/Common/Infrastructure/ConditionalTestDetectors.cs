// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

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
            try
            {
                ServiceUtilHelper.EnsureRootCertificateInstalled();
                return true;
            }
            catch
            {
                // Errors installing the certificate are captured and will be
                // reported when an attempt is made to use it.  But for the
                // purposes of this detector, a failure only propagates as
                // a 'false' return.
                return false;
            }
        }

        // Detector used by [ConditionalFact(nameof(Client_Certificate_Installed)].
        // It will attempt to install the client certificate in the certificate store if
        // is not already present, and then it will check whether the install
        // succeeded.  A 'true' return is a guarantee a client certificate is
        // installed in the certificate store.
        public static bool IsClientCertificateInstalled()
        {
            try { 
                ServiceUtilHelper.EnsureClientCertificateInstalled();
                return true;
            }
            catch
            {
                // Errors installing the certificate are captured and will be
                // reported when an attempt is made to use it.  But for the
                // purposes of this detector, a failure only propagates as
                // a 'false' return.
                return false;
            }
        }

        // Detector used by [ConditionalFact(nameof(OSXPeer_Certificate_Installed)].
        // It will attempt to install the server certificate in a local keychain if
        // is not already present, and then it will check whether the install
        // succeeded.  A 'true' return is a guarantee a server certificate is
        // installed in the certificate store.
        public static bool IsOSXKeychainCertificateInstalled()
        {
            try
            {
                ServiceUtilHelper.EnsureOSXKeychainCertificateInstalled();
                return true;
            }
            catch
            {
                // Errors installing the certificate are captured and will be
                // reported when an attempt is made to use it.  But for the
                // purposes of this detector, a failure only propagates as
                // a 'false' return.
                return false;
            }
        }

        // Detector used by [ConditionalFact(nameof(Peer_Certificate_Installed)].
        // It will attempt to install the server certificate in the certificate store if
        // is not already present, and then it will check whether the install
        // succeeded.  A 'true' return is a guarantee a server certificate is
        // installed in the certificate store.
        public static bool IsPeerCertificateInstalled()
        {
            try
            {
                ServiceUtilHelper.EnsurePeerCertificateInstalled();
                return true;
            }
            catch
            {
                // Errors installing the certificate are captured and will be
                // reported when an attempt is made to use it.  But for the
                // purposes of this detector, a failure only propagates as
                // a 'false' return.
                return false;
            }
        }

        // Returns 'true' if the server is running IIS-hosted
        public static bool IsIISHosted()
        {
            return ServiceUtilHelper.IISHosted;
        }

        // Tests whether the client is domain-joined.  This test does not
        // consider whether the server is also domain-joined.
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

        // Tests whether the server is domain-joined.  This test does not consider
        // whether the client is domain-joined.
        public static bool IsServerDomainJoined()
        {
            // Requires solution to https://github.com/dotnet/wcf/issues/1095
            // Till then, heuristic is that running localhost == domain joined
            return IsServerLocalHost();
        }

        // Returns 'true' if the client is running on a Windows OS
        public static bool IsWindows()
        {
            return OSID.AnyWindows.MatchesCurrent();
        }

        // Returns 'true' if the server is running with CoreWCF Service
        public static bool IsRunWithCoreWCFService()
        {
            string runWithCoreWCFService = TestProperties.GetProperty(TestProperties.RunWithCoreWCF_PropertyName);

            if (String.IsNullOrWhiteSpace(runWithCoreWCFService))
            {
                return false;
            }

            return true;
        }

        // Returns 'true' if the server is running as localhost.
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

        // Returns 'true' if ambient credentials are available to use
        public static bool AreAmbientCredentialsAvailable()
        {
            // Requires solution to https://github.com/dotnet/wcf/issues/1095
            // Till then, heuristic is that running localhost == ambient credentials available
            return IsServerLocalHost();
        }

        // Returns 'true' if explicit credentials are available.
        // Currently, 'true' means the TestProperties for ExplicitUserName
        // and ExplicitPassword are non-blank.
        public static bool AreExplicitCredentialsAvailable()
        {
            return !String.IsNullOrWhiteSpace(GetExplicitUserName()) &&
                   !String.IsNullOrWhiteSpace(GetExplicitPassword());
        }

        // Returns 'true' if a domain is available.  TestProperties for
        // NegotiateTestDomain takes precedence, but for Windows it can
        // infer the value.
        public static bool IsDomainAvailable()
        {
            return !String.IsNullOrWhiteSpace(GetDomain());
        }

        // Returns 'true if SPN is available
        public static bool IsSPNAvailable()
        {
            // SPN is available only if NegotiateTestSpn has been explicitly set.
            // When https://github.com/dotnet/wcf/issues/1283 is fixed, this should be updated to
            // handle the self-hosted as Local System scenario as well.
            return !String.IsNullOrWhiteSpace(GetSPN());
        }

        // Returns 'true if UPN is available
        public static bool IsUPNAvailable()
        {
            // UPN is available only if NegotiateTestSpn has been explicitly set.
            // When https://github.com/dotnet/wcf/issues/1283 is fixed, this should be updated to
            // handle the self-hosted as Local System scenario as well.
            return !String.IsNullOrWhiteSpace(GetUPN());
        }

        // Returns the explicit user name if available
        internal static string GetExplicitUserName()
        {
            return TestProperties.GetProperty(TestProperties.ExplicitUserName_PropertyName);
        }

        // Returns the explicit password if available
        internal static string GetExplicitPassword()
        {
            return TestProperties.GetProperty(TestProperties.ExplicitPassword_PropertyName);
        }

        // Returns the Domain if available.
        // TestProperties takes precedence, but if it has not been specified
        // and this is a Windows client, we infer it.
        public static string GetDomain()
        {
            string result = TestProperties.GetProperty(TestProperties.NegotiateTestDomain_PropertyName);
            if (String.IsNullOrEmpty(result) && IsClientDomainJoined())
            {
                result = Environment.GetEnvironmentVariable("USERDOMAIN");
            }

            return result;
        }

        // Gets the UPN if available
        internal static string GetUPN()
        {
            return TestProperties.GetProperty(TestProperties.NegotiateTestUpn_PropertyName);
        }

        // Gets the SPN if available
        internal static string GetSPN()
        {
            return TestProperties.GetProperty(TestProperties.NegotiateTestSpn_PropertyName);
        }

    }
}
