// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using System.Net.Security;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.OperationContract, AllowMultiple = true, Inherited = false)]
    public sealed class FaultContractAttribute : Attribute
    {
        private string _action;
        private string _name;
        private string _ns;
        private ProtectionLevel _protectionLevel = ProtectionLevel.None;

        public FaultContractAttribute(Type detailType)
        {
            DetailType = detailType ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(detailType)));
        }

        public Type DetailType { get; }

        public string Action
        {
            get { return _action; }
            set
            {
                _action = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

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

                _name = value;
            }
        }

        public string Namespace
        {
            get { return _ns; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    NamingHelper.CheckUriProperty(value, "Namespace");
                }

                _ns = value;
            }
        }

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

