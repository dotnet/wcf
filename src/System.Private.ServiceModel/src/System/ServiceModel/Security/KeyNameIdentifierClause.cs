// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
    public class KeyNameIdentifierClause : SecurityKeyIdentifierClause
    {
        private string _keyName;

        public KeyNameIdentifierClause(string keyName)
            : base(null)
        {
            if (keyName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyName");
            }
            _keyName = keyName;
        }

        public string KeyName
        {
            get { return _keyName; }
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            KeyNameIdentifierClause that = keyIdentifierClause as KeyNameIdentifierClause;
            return ReferenceEquals(this, that) || (that != null && that.Matches(_keyName));
        }

        public bool Matches(string keyName)
        {
            return _keyName == keyName;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "KeyNameIdentifierClause(KeyName = '{0}')", this.KeyName);
        }
    }
}

