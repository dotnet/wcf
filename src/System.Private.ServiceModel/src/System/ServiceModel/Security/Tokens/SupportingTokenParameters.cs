// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace System.ServiceModel.Security.Tokens
{
    public class SupportingTokenParameters
    {
        private Collection<SecurityTokenParameters> _signedEndorsing = new Collection<SecurityTokenParameters>();

        private SupportingTokenParameters(SupportingTokenParameters other)
        {
            if (other == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(other));
            }

            foreach (SecurityTokenParameters p in other.Signed)
            {
                Signed.Add((SecurityTokenParameters)p.Clone());
            }

            foreach (SecurityTokenParameters p in other.SignedEncrypted)
            {
                SignedEncrypted.Add((SecurityTokenParameters)p.Clone());
            }

            foreach (SecurityTokenParameters p in other.Endorsing)
            {
                Endorsing.Add((SecurityTokenParameters)p.Clone());
            }

            foreach (SecurityTokenParameters p in other._signedEndorsing)
            {
                _signedEndorsing.Add((SecurityTokenParameters)p.Clone());
            }
        }

        public SupportingTokenParameters()
        {
            // empty
        }

        public Collection<SecurityTokenParameters> Endorsing { get; } = new Collection<SecurityTokenParameters>();

        public Collection<SecurityTokenParameters> SignedEndorsing
        {
            get
            {
                return _signedEndorsing;
            }
        }

        public Collection<SecurityTokenParameters> Signed { get; } = new Collection<SecurityTokenParameters>();

        public Collection<SecurityTokenParameters> SignedEncrypted { get; } = new Collection<SecurityTokenParameters>();

        public void SetKeyDerivation(bool requireDerivedKeys)
        {
            foreach (SecurityTokenParameters t in Endorsing)
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
            foreach (SecurityTokenParameters t in Endorsing)
            {
                if (t.RequireDerivedKeys != requireDerivedKeys)
                {
                    return false;
                }
            }

            foreach (SecurityTokenParameters t in _signedEndorsing)
            {
                if (t.RequireDerivedKeys != requireDerivedKeys)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int k;

            if (Endorsing.Count == 0)
            {
                sb.AppendLine("No endorsing tokens.");
            }
            else
            {
                for (k = 0; k < Endorsing.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "Endorsing[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + Endorsing[k].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            if (Signed.Count == 0)
            {
                sb.AppendLine("No signed tokens.");
            }
            else
            {
                for (k = 0; k < Signed.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "Signed[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + Signed[k].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            if (SignedEncrypted.Count == 0)
            {
                sb.AppendLine("No signed encrypted tokens.");
            }
            else
            {
                for (k = 0; k < SignedEncrypted.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SignedEncrypted[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + SignedEncrypted[k].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            if (_signedEndorsing.Count == 0)
            {
                sb.AppendLine("No signed endorsing tokens.");
            }
            else
            {
                for (k = 0; k < _signedEndorsing.Count; k++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SignedEndorsing[{0}]", k.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("  " + _signedEndorsing[k].ToString().Trim().Replace("\n", "\n  "));
                }
            }

            return sb.ToString().Trim();
        }

        public SupportingTokenParameters Clone()
        {
            SupportingTokenParameters parameters = CloneCore();
            if (parameters == null || parameters.GetType() != GetType())
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
            return Signed.Count == 0 && SignedEncrypted.Count == 0 && Endorsing.Count == 0 && _signedEndorsing.Count == 0;
        }
    }
}
