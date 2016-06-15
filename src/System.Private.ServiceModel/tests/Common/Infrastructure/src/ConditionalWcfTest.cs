// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Common
{
    // ConditionalWcfTest is expected to be the base class of any test
    // class that includes [ConditionalFact] or [ConditionalTheory]. This
    // is necessary because the conditional attributes are expected to
    // refer to members within their test class or its base classes.
    public class ConditionalWcfTest
    {
        private static Dictionary<string, bool> s_evaluatedConditions = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        private static object s_evaluationLock = new object();

        // Returns 'true' or 'false' for the given condition name.
        // There are several levels of precedence to evaluate the condition.
        // The value is cached after being evaluated the first time.
        // Precedence order for evaluation is:
        //  1. Environment variable, if set and is 'true' or 'false'.
        //  2. TestProperties, if set and is 'true' or 'false'
        //  3. detection func, if specified
        //  4. If none of the above, 'false'
        private static bool GetConditionValue(string conditionName, Func<bool> detectFunc = null)
        {
            // Lock to evaluate once only
            lock (s_evaluationLock)
            {
                bool result = false;
                if (s_evaluatedConditions.TryGetValue(conditionName, out result))
                {
                    return result;
                }

                bool evaluatedResult = false;

                // Highest precedence: environment variable if set and can be parsed
                string value = Environment.GetEnvironmentVariable(conditionName);
                bool parsedValue = false;
                if (!String.IsNullOrWhiteSpace(value) && bool.TryParse(value, out parsedValue))
                {
                    result = parsedValue;
                    evaluatedResult = true;
                }

                // Next precedence: TestProperties if present and can be parsed
                else if (TestProperties.PropertyNames.Contains(conditionName))
                {
                    value = TestProperties.GetProperty(conditionName);
                    if (!String.IsNullOrWhiteSpace(value) && bool.TryParse(value, out parsedValue))
                    {
                        result = parsedValue;
                        evaluatedResult = true;
                    }
                }

                // Next precedence: optional runtime detection func
                if (!evaluatedResult && detectFunc != null)
                {
                    result = detectFunc();
                    evaluatedResult = true;
                }

                // Final precedence: false is default
                if (!evaluatedResult)
                {
                    result = false;
                }

                s_evaluatedConditions[conditionName] = result;
                return result;
            }
        }

        // Returns 'true' if the server is known running on localhost
        private static bool Server_Is_LocalHost()
        {
            return GetConditionValue(nameof(Server_Is_LocalHost),
                                     ConditionalTestDetectors.IsServerLocalHost);
        }
        
        // Returns 'true' if the client code is executing on a Windows OS
        private static bool Is_Windows()
        {
            return GetConditionValue(nameof(Is_Windows),
                                     ConditionalTestDetectors.IsWindows);
        }

        // Returns 'true' if both the server and the client are domain-joined.
        public static bool Domain_Joined()
        {
            return GetConditionValue(nameof(Domain_Joined),
                                     () => ConditionalTestDetectors.IsClientDomainJoined() && 
                                           Server_Domain_Joined());
        }

        // Returns 'true' if the server is domain-joined.
        // This test does not consider whether the client is domain-joined.
        public static bool Server_Domain_Joined()
        {
            return GetConditionValue(nameof(Server_Domain_Joined),
                                     ConditionalTestDetectors.IsServerDomainJoined);
        }

        // Returns 'true' if the root certificate is installed and
        // can be used.  This test will attempt to install the root certificate
        // when necessary.  A 'false' from this test usually indicates that
        // the root certificate could not be installed and was also not found
        // in the root store.
        public static bool Root_Certificate_Installed()
        {
            return GetConditionValue(nameof(Root_Certificate_Installed),
                                     ConditionalTestDetectors.IsRootCertificateInstalled);
        }

        // Returns 'true' if the client certificate is installed and
        // can be used.  This test will attempt to install the client certificate
        // when necessary.  A 'false' from this test usually indicates that
        // the client certificate could not be installed and was also not found
        // in the client store.
        public static bool Client_Certificate_Installed()
        {
            return GetConditionValue(nameof(Client_Certificate_Installed),
                                     ConditionalTestDetectors.IsClientCertificateInstalled);
        }

        // Returns 'true' if ambient credentials are available to use.
        public static bool Ambient_Credentials_Available()
        {
            return GetConditionValue(nameof(Ambient_Credentials_Available),
                                     ConditionalTestDetectors.AreAmbientCredentialsAvailable);
        }

        // Returns 'true' if explicit credentials are available to use.
        public static bool Explicit_Credentials_Available()
        {
            return GetConditionValue(nameof(Explicit_Credentials_Available),
                                     ConditionalTestDetectors.AreExplicitCredentialsAvailable);
        }

        // Returns 'true' if the domain is available to use.
        public static bool Domain_Available()
        {
            return GetConditionValue(nameof(Domain_Available),
                                     ConditionalTestDetectors.IsDomainAvailable);
        }

        // Returns 'true' if SPN is available
        public static bool SPN_Available()
        {
            return GetConditionValue(nameof(SPN_Available),
                                     ConditionalTestDetectors.IsSPNAvailable);
        }

        // Returns 'true' if UPN is available
        public static bool UPN_Available()
        {
            return GetConditionValue(nameof(UPN_Available),
                                     ConditionalTestDetectors.IsUPNAvailable);
        }

        // Returns 'true' if the server is configured to accept client certificates.
        public static bool Server_Accepts_Certificates()
        {
            //Both IIS hosted and self hosted servers accept server certificate now
            return true;
        }

        // Returns 'true' if the server is configured to allow Basic Authentication.
        public static bool Basic_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Basic_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        // Returns 'true' if the server is configured to allow Digest Authentication.
        public static bool Digest_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Digest_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        // Returns 'true' if the server is configured to allow Windows Authentication.
        public static bool Windows_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Windows_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        // Returns true if NTLM is available to use.
        public static bool NTLM_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(NTLM_Available),
                                     Server_Is_LocalHost);
        }

        // Returns the Domain if available.
        // TestProperties takes precedence, but if it has not been specified
        // and this is a Windows client, we infer it.
        public static string GetDomain()
        {
            return ConditionalTestDetectors.GetDomain();
        }

        // Returns the explicit user name if available
        public static string GetExplicitUserName()
        {
            return ConditionalTestDetectors.GetExplicitUserName();
        }

        // Returns the explicit password if available
        public static string GetExplicitPassword()
        {
            return ConditionalTestDetectors.GetExplicitPassword();
        }

        // Gets the UPN if available
        public static string GetUPN()
        {
            return ConditionalTestDetectors.GetUPN();
        }

        // Gets the UPN if available
        public static string GetSPN()
        {
            return ConditionalTestDetectors.GetSPN();
        }
    }
}
