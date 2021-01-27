// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
    public abstract class SecurityTokenParameters
    {
        internal const SecurityTokenInclusionMode defaultInclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
        internal const SecurityTokenReferenceStyle defaultReferenceStyle = SecurityTokenReferenceStyle.Internal;
        internal const bool defaultRequireDerivedKeys = true;

        private SecurityTokenInclusionMode _inclusionMode = defaultInclusionMode;
        private SecurityTokenReferenceStyle _referenceStyle = defaultReferenceStyle;
        private bool _requireDerivedKeys = defaultRequireDerivedKeys;

        protected SecurityTokenParameters(SecurityTokenParameters other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            _requireDerivedKeys = other._requireDerivedKeys;
            _inclusionMode = other._inclusionMode;
            _referenceStyle = other._referenceStyle;
        }

        protected SecurityTokenParameters()
        {
            // empty
        }

        internal protected abstract bool HasAsymmetricKey { get; }

        public SecurityTokenInclusionMode InclusionMode
        {
            get
            {
                return _inclusionMode;
            }
            set
            {
                SecurityTokenInclusionModeHelper.Validate(value);
                _inclusionMode = value;
            }
        }

        public SecurityTokenReferenceStyle ReferenceStyle
        {
            get
            {
                return _referenceStyle;
            }
            set
            {
                TokenReferenceStyleHelper.Validate(value);
                _referenceStyle = value;
            }
        }

        public bool RequireDerivedKeys
        {
            get
            {
                return _requireDerivedKeys;
            }
            set
            {
                _requireDerivedKeys = value;
            }
        }

        internal protected abstract bool SupportsClientAuthentication { get; }
        internal protected abstract bool SupportsServerAuthentication { get; }
        internal protected abstract bool SupportsClientWindowsIdentity { get; }

        public SecurityTokenParameters Clone()
        {
            SecurityTokenParameters result = this.CloneCore();

            if (result == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SecurityTokenParametersCloneInvalidResult, this.GetType().ToString())));

            return result;
        }

        protected abstract SecurityTokenParameters CloneCore();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.Append(String.Format(CultureInfo.InvariantCulture, "RequireDerivedKeys: {0}", _requireDerivedKeys.ToString()));

            return sb.ToString();
        }
    }
}
