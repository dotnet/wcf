// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security.Tokens
{
    public class ClaimTypeRequirement
    {
        internal const bool DefaultIsOptional = false;
        private string _claimType;
        private bool _isOptional;

        public ClaimTypeRequirement(string claimType)
            : this(claimType, DefaultIsOptional)
        {
        }

        public ClaimTypeRequirement(string claimType, bool isOptional)
        {
            if (claimType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claimType");
            }
            if (claimType.Length <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("claimType", SRServiceModel.ClaimTypeCannotBeEmpty);
            }

            _claimType = claimType;
            _isOptional = isOptional;
        }

        public string ClaimType
        {
            get { return _claimType; }
        }

        public bool IsOptional
        {
            get { return _isOptional; }
        }
    }
}
