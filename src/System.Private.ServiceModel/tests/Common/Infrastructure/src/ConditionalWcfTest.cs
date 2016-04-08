// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
namespace Infrastructure.Common
{
    // ConditionalWcfTest is expected to be the base class of any test
    // class that includes [ConditionalFact] or [ConditionalTheory]. This
    // is necessary because the conditional attributes are expected to
    // refer to members within their test class or its base classes.
    public class ConditionalWcfTest
    {
        private static bool IsConditionalTestEnabled(string testCategoryName)
        {
            string propertyValue = TestProperties.GetProperty(testCategoryName);
            bool isEnabled;
            if (!String.IsNullOrEmpty(propertyValue) && bool.TryParse(propertyValue, out isEnabled))
            {
                return isEnabled;
            }

            return false;
        }

        public static bool Domain_Joined()
        {
            return IsConditionalTestEnabled(TestProperties.Domain_Joined_PropertyName);
        }

        public static bool Root_Certificate_Installed()
        {
            return IsConditionalTestEnabled(TestProperties.Root_Certificate_Installed_PropertyName);
        }

        public static bool Client_Certificate_Installed()
        {
            return IsConditionalTestEnabled(TestProperties.Client_Certificate_Installed_PropertyName);
        }

        public static bool SPN_Available()
        {
            return IsConditionalTestEnabled(TestProperties.SPN_Available_PropertyName);
        }

        public static bool Kerberos_Available()
        {
            return IsConditionalTestEnabled(TestProperties.Kerberos_Available_PropertyName);
        }

    }
}
