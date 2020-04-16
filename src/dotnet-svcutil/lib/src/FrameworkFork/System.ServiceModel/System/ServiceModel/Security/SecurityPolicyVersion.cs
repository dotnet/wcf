// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    public abstract class SecurityPolicyVersion
    {
        private readonly String _spNamespace;
        private readonly String _prefix;

        internal SecurityPolicyVersion(String ns, String prefix)
        {
            _spNamespace = ns;
            _prefix = prefix;
        }

        public String Namespace
        {
            get
            {
                return _spNamespace;
            }
        }

        public String Prefix
        {
            get
            {
                return _prefix;
            }
        }

        public static SecurityPolicyVersion WSSecurityPolicy11
        {
            get { return WSSecurityPolicyVersion11.Instance; }
        }

        public static SecurityPolicyVersion WSSecurityPolicy12
        {
            get { return WSSecurityPolicyVersion12.Instance; }
        }

        internal class WSSecurityPolicyVersion11 : SecurityPolicyVersion
        {
            private static readonly WSSecurityPolicyVersion11 s_instance = new WSSecurityPolicyVersion11();

            protected WSSecurityPolicyVersion11()
                : base("http://schemas.xmlsoap.org/ws/2005/07/securitypolicy", WSSecurityPolicy.WsspPrefix)
            {
            }

            public static SecurityPolicyVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }

        internal class WSSecurityPolicyVersion12 : SecurityPolicyVersion
        {
            private static readonly WSSecurityPolicyVersion12 s_instance = new WSSecurityPolicyVersion12();

            protected WSSecurityPolicyVersion12()
                : base("http://docs.oasis-open.org/ws-sx/ws-securitypolicy/200702", WSSecurityPolicy.WsspPrefix)
            {
            }

            public static SecurityPolicyVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }
    }
}
