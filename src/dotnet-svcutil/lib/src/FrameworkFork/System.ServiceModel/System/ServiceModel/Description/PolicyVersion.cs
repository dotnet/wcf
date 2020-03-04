// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public sealed class PolicyVersion
    {
        private string _policyNamespace;

        private static PolicyVersion s_policyVersion12;
        private static PolicyVersion s_policyVersion15;

        static PolicyVersion()
        {
            s_policyVersion12 = new PolicyVersion(MetadataStrings.WSPolicy.NamespaceUri);
            s_policyVersion15 = new PolicyVersion(MetadataStrings.WSPolicy.NamespaceUri15);
        }

        private PolicyVersion(string policyNamespace)
        {
            _policyNamespace = policyNamespace;
        }

        public static PolicyVersion Policy12 { get { return s_policyVersion12; } }
        public static PolicyVersion Policy15 { get { return s_policyVersion15; } }
        public static PolicyVersion Default { get { return s_policyVersion12; } }
        public string Namespace { get { return _policyNamespace; } }

        public override string ToString()
        {
            return _policyNamespace;
        }
    }
}
