// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Security
{
    public abstract class BasicSecurityProfileVersion
    {
        internal BasicSecurityProfileVersion() { }

        public static BasicSecurityProfileVersion BasicSecurityProfile10
        {
            get { return BasicSecurityProfile10BasicSecurityProfileVersion.Instance; }
        }

        internal class BasicSecurityProfile10BasicSecurityProfileVersion : BasicSecurityProfileVersion
        {
            private static BasicSecurityProfile10BasicSecurityProfileVersion s_instance = new BasicSecurityProfile10BasicSecurityProfileVersion();

            public static BasicSecurityProfile10BasicSecurityProfileVersion Instance { get { return s_instance; } }

            public override string ToString()
            {
                return "BasicSecurityProfile10";
            }
        }
    }
}
