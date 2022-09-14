// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Net.Security;

namespace System.ServiceModel
{
    public abstract class MessageContractMemberAttribute : Attribute
    {
        private string _name;
        private string _ns;
        private bool _isNamespaceSetExplicit;
        private ProtectionLevel _protectionLevel = ProtectionLevel.None;
        internal const string NamespacePropertyName = "Namespace";
        public string Namespace
        {
            get { return _ns; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value.Length > 0)
                {
                    NamingHelper.CheckUriProperty(value, "Namespace");
                }
                _ns = value;
                _isNamespaceSetExplicit = true;
            }
        }

        internal bool IsNamespaceSetExplicit
        {
            get { return _isNamespaceSetExplicit; }
        }

        internal const string NamePropertyName = "Name";
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        SRP.SFxNameCannotBeEmpty));
                }
                _name = value; IsNameSetExplicit = true;
            }
        }

        internal bool IsNameSetExplicit { get; private set; }

        internal const string ProtectionLevelPropertyName = "ProtectionLevel";
        public ProtectionLevel ProtectionLevel
        {
            get
            {
                return _protectionLevel;
            }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _protectionLevel = value;
                HasProtectionLevel = true;
            }
        }

        public bool HasProtectionLevel { get; private set; } = false;
    }
}

