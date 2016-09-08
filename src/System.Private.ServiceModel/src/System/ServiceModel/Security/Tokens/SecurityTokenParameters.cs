// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
    public abstract class SecurityTokenParameters
    {
        internal const SecurityTokenInclusionMode defaultInclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
        internal const bool defaultRequireDerivedKeys = true;

        private SecurityTokenInclusionMode _inclusionMode = defaultInclusionMode;
        private bool _requireDerivedKeys = defaultRequireDerivedKeys;

        protected SecurityTokenParameters(SecurityTokenParameters other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            _inclusionMode = other._inclusionMode;
            _requireDerivedKeys = other._requireDerivedKeys;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SecurityTokenParametersCloneInvalidResult, this.GetType().ToString())));

            return result;
        }

        protected abstract SecurityTokenParameters CloneCore();

        internal protected abstract SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle);

        internal protected abstract void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement);

        internal SecurityKeyIdentifierClause CreateKeyIdentifierClause<TExternalClause, TInternalClause>(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
    where TExternalClause : SecurityKeyIdentifierClause
    where TInternalClause : SecurityKeyIdentifierClause
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            SecurityKeyIdentifierClause result;

            switch (referenceStyle)
            {
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                        SR.Format(SR.TokenDoesNotSupportKeyIdentifierClauseCreation, token.GetType().Name, referenceStyle)));
                case SecurityTokenReferenceStyle.External:
                    result = token.CreateKeyIdentifierClause<TExternalClause>();
                    break;
                case SecurityTokenReferenceStyle.Internal:
                    result = token.CreateKeyIdentifierClause<TInternalClause>();
                    break;
            }

            return result;
        }

        internal SecurityKeyIdentifierClause CreateGenericXmlTokenKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            GenericXmlSecurityToken xmlToken = token as GenericXmlSecurityToken;
            if (xmlToken != null)
            {
                if (referenceStyle == SecurityTokenReferenceStyle.Internal && xmlToken.InternalTokenReference != null)
                    return xmlToken.InternalTokenReference;

                if (referenceStyle == SecurityTokenReferenceStyle.External && xmlToken.ExternalTokenReference != null)
                    return xmlToken.ExternalTokenReference;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.UnableToCreateTokenReference)));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "InclusionMode: {0}", _inclusionMode.ToString()));
            sb.Append(String.Format(CultureInfo.InvariantCulture, "RequireDerivedKeys: {0}", _requireDerivedKeys.ToString()));

            return sb.ToString();
        }
    }
}
