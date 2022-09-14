// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net.Security;
using System.ServiceModel.Security;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.MessageContract, AllowMultiple = false)]
    public sealed class MessageContractAttribute : Attribute
    {
        private string _wrappedName;
        private string _wrappedNs;
        private ProtectionLevel _protectionLevel = ProtectionLevel.None;
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

        public bool IsWrapped { get; set; } = true;

        public string WrapperName
        {
            get
            {
                return _wrappedName;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        SRP.SFxWrapperNameCannotBeEmpty));
                }

                _wrappedName = value;
            }
        }

        public string WrapperNamespace
        {
            get
            {
                return _wrappedNs;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NamingHelper.CheckUriProperty(value, "WrapperNamespace");
                }

                _wrappedNs = value;
            }
        }
    }
}
