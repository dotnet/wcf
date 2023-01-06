// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
            public static BasicSecurityProfile10BasicSecurityProfileVersion Instance { get; } = new BasicSecurityProfile10BasicSecurityProfileVersion();

            public override string ToString()
            {
                return "BasicSecurityProfile10";
            }
        }
    }
}
