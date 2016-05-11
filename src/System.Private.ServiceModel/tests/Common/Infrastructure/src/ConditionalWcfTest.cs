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

        private static bool Server_Is_LocalHost()
        {
            return GetConditionValue(nameof(Server_Is_LocalHost),
                                     ConditionalTestDetectors.IsServerLocalHost);
        }

        private static bool Is_Windows()
        {
            return GetConditionValue(nameof(Is_Windows),
                                     ConditionalTestDetectors.IsWindows);
        }

        public static bool Domain_Joined()
        {
            return GetConditionValue(nameof(Domain_Joined),
                                     () => ConditionalTestDetectors.IsClientDomainJoined() &&
                                           ConditionalTestDetectors.IsServerDomainJoined());
        }

        public static bool Root_Certificate_Installed()
        {
            return GetConditionValue(nameof(Root_Certificate_Installed),
                                     ConditionalTestDetectors.IsRootCertificateInstalled);
        }

        public static bool Client_Certificate_Installed()
        {
            return GetConditionValue(nameof(Client_Certificate_Installed),
                                     ConditionalTestDetectors.IsClientCertificateInstalled);
        }

        public static bool UserName_And_Password_Available()
        {
            return GetConditionValue(nameof(UserName_And_Password_Available),
                                     ConditionalTestDetectors.AreUserNameAndPasswordAvailable);
        }

        public static bool SPN_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(SPN_Available),
                                     Server_Is_LocalHost);
        }

        public static bool Kerberos_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Kerberos_Available),
                                     Server_Is_LocalHost);
        }

        public static bool Server_Accepts_Certificates()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it does.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Server_Accepts_Certificates),
                                     Server_Is_LocalHost);
        }

        public static bool Basic_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Basic_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        public static bool Digest_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Digest_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        public static bool Windows_Authentication_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(Windows_Authentication_Available),
                                     Server_Is_LocalHost);
        }

        public static bool NTLM_Available()
        {
            // Temporarily use the simple heuristic that if we are running the services locally, it is.
            // Refactor this after integration to address https://github.com/dotnet/wcf/issues/1024 
            return GetConditionValue(nameof(NTLM_Available),
                                     Server_Is_LocalHost);
        }
    }
}
