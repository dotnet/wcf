// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel.Diagnostics;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
    public class SupportingTokenParameters
    {
        private Collection<SecurityTokenParameters> _signed = new Collection<SecurityTokenParameters>();
        private Collection<SecurityTokenParameters> _signedEncrypted = new Collection<SecurityTokenParameters>();
        private Collection<SecurityTokenParameters> _endorsing = new Collection<SecurityTokenParameters>();
        private Collection<SecurityTokenParameters> _signedEndorsing = new Collection<SecurityTokenParameters>();

        private SupportingTokenParameters(SupportingTokenParameters other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            foreach (SecurityTokenParameters p in other._signed)
                _signed.Add((SecurityTokenParameters)p.Clone());
            foreach (SecurityTokenParameters p in other._signedEncrypted)
                _signedEncrypted.Add((SecurityTokenParameters)p.Clone());
            foreach (SecurityTokenParameters p in other._endorsing)
                _endorsing.Add((SecurityTokenParameters)p.Clone());
            foreach (SecurityTokenParameters p in other._signedEndorsing)
                _signedEndorsing.Add((SecurityTokenParameters)p.Clone());
        }

        public SupportingTokenParameters()
        {
            // empty
        }

        public Collection<SecurityTokenParameters> Endorsing
        {
            get
            {
                return _endorsing;
            }
        }

        public Collection<SecurityTokenParameters> SignedEndorsing
        {
            get
            {
                return _signedEndorsing;
            }
        }

        public Collection<SecurityTokenParameters> Signed
        {
            get
            {
                return _signed;
            }
        }

        public Collection<SecurityTokenParameters> SignedEncrypted
        {
            get
            {
                return _signedEncrypted;
            }
        }

        public void SetKeyDerivation(bool requireDerivedKeys)
        {
            foreach (SecurityTokenParameters t in _endorsing)
            {
                if (t.HasAsymmetricKey)
                {
                    t.RequireDerivedKeys = false;
                }
                else
                {
                    t.RequireDerivedKeys = requireDerivedKeys;
                }
            }
            foreach (SecurityTokenParameters t in _signedEndorsing)
            {
                if (t.HasAsymmetricKey)
                {
                    t.RequireDerivedKeys = false;
                }
                else
                {
                    t.RequireDerivedKeys = requireDerivedKeys;
                }
            }
        }

        internal bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            foreach (SecurityTokenParameters t in _endorsing)
                if (t.RequireDerivedKeys != requireDerivedKeys)
                    return false;

            foreach (SecurityTokenParameters t in _signedEndorsing)
                if (t.RequireDerivedKeys != requireDerivedKeys)
                    return false;
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int k;

            if (_endorsing.Count == 0)
                sb.AppendLine("No endorsing tokens.");
            else
                for (k = 0; k < _endorsing.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "Endorsing[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + _endorsing[k].ToString().Trim().Replace("\n", "\n  "));
                }

            if (_signed.Count == 0)
                sb.AppendLine("No signed tokens.");
            else
                for (k = 0; k < _signed.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "Signed[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + _signed[k].ToString().Trim().Replace("\n", "\n  "));
                }

            if (_signedEncrypted.Count == 0)
                sb.AppendLine("No signed encrypted tokens.");
            else
                for (k = 0; k < _signedEncrypted.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SignedEncrypted[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + _signedEncrypted[k].ToString().Trim().Replace("\n", "\n  "));
                }

            if (_signedEndorsing.Count == 0)
                sb.AppendLine("No signed endorsing tokens.");
            else
                for (k = 0; k < _signedEndorsing.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SignedEndorsing[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + _signedEndorsing[k].ToString().Trim().Replace("\n", "\n  "));
                }

            return sb.ToString().Trim();
        }

        public SupportingTokenParameters Clone()
        {
            SupportingTokenParameters parameters = this.CloneCore();
            if (parameters == null || parameters.GetType() != this.GetType())
            {
            }

            return parameters;
        }

        protected virtual SupportingTokenParameters CloneCore()
        {
            return new SupportingTokenParameters(this);
        }

        internal bool IsEmpty()
        {
            return _signed.Count == 0 && _signedEncrypted.Count == 0 && _endorsing.Count == 0 && _signedEndorsing.Count == 0;
        }
    }
}
