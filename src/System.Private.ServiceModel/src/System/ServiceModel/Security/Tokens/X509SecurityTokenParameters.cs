﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Text;
using System.Globalization;

namespace System.ServiceModel.Security.Tokens
{
    public class X509SecurityTokenParameters : SecurityTokenParameters
    {
        internal const X509KeyIdentifierClauseType defaultX509ReferenceStyle = X509KeyIdentifierClauseType.Any;

        private X509KeyIdentifierClauseType _x509ReferenceStyle;

        protected X509SecurityTokenParameters(X509SecurityTokenParameters other)
            : base(other)
        {
            _x509ReferenceStyle = other._x509ReferenceStyle;
        }

        public X509SecurityTokenParameters()
            : this(X509SecurityTokenParameters.defaultX509ReferenceStyle, SecurityTokenParameters.defaultInclusionMode)
        {
            // empty
        }

        public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle)
            : this(x509ReferenceStyle, SecurityTokenParameters.defaultInclusionMode)
        {
            // empty
        }

        public X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode)
            : this(x509ReferenceStyle, inclusionMode, SecurityTokenParameters.defaultRequireDerivedKeys)
        {
        }

        internal X509SecurityTokenParameters(X509KeyIdentifierClauseType x509ReferenceStyle, SecurityTokenInclusionMode inclusionMode,
            bool requireDerivedKeys)
            : base()
        {
            X509ReferenceStyle = x509ReferenceStyle;
            InclusionMode = inclusionMode;
            RequireDerivedKeys = requireDerivedKeys;
        }

        internal protected override bool HasAsymmetricKey { get { return true; } }

        public X509KeyIdentifierClauseType X509ReferenceStyle
        {
            get
            {
                return _x509ReferenceStyle;
            }
            set
            {
                X509SecurityTokenReferenceStyleHelper.Validate(value);
                _x509ReferenceStyle = value;
            }
        }

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return true; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new X509SecurityTokenParameters(this);
        }

        internal protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            SecurityKeyIdentifierClause result = null;

            switch (_x509ReferenceStyle)
            {
                default:
                case X509KeyIdentifierClauseType.Any:
                    if (referenceStyle == SecurityTokenReferenceStyle.External)
                    {
                        X509SecurityToken x509Token = token as X509SecurityToken;
                        if (x509Token != null)
                        {
                            X509SubjectKeyIdentifierClause x509KeyIdentifierClause;
                            if (X509SubjectKeyIdentifierClause.TryCreateFrom(x509Token.Certificate, out x509KeyIdentifierClause))
                            {
                                result = x509KeyIdentifierClause;
                            }
                        }

                        if (result == null)
                        {
                            throw new PlatformNotSupportedException();
                        }
                    }
                    else
                    {
                        result = token.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
                    }

                    break;
                case X509KeyIdentifierClauseType.Thumbprint:
                    result = CreateKeyIdentifierClause<X509ThumbprintKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
                    break;
                case X509KeyIdentifierClauseType.SubjectKeyIdentifier:
                    result = CreateKeyIdentifierClause<X509SubjectKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
                    break;
                case X509KeyIdentifierClauseType.IssuerSerial:
                    result = CreateKeyIdentifierClause<X509IssuerSerialKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
                    break;
                case X509KeyIdentifierClauseType.RawDataKeyIdentifier:
                    result = CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
                    break;
            }

            return result;
        }

        protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = SecurityTokenTypes.X509Certificate;
            requirement.RequireCryptographicToken = true;
            requirement.KeyType = SecurityKeyType.AsymmetricKey;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.Append(String.Format(CultureInfo.InvariantCulture, "X509ReferenceStyle: {0}", _x509ReferenceStyle.ToString()));

            return sb.ToString();
        }
    }
}
