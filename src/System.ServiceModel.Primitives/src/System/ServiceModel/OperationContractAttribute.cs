// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net.Security;
using System.Reflection;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.OperationContract)]
    public sealed class OperationContractAttribute : Attribute
    {
        private string _name = null;
        private string _action = null;
        private string _replyAction = null;
        private bool _isOneWay = false;
        private ProtectionLevel _protectionLevel = ProtectionLevel.None;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }
                if (value == "")
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        SRP.SFxNameCannotBeEmpty));
                }

                _name = value;
            }
        }

        internal const string ActionPropertyName = "Action";
        public string Action
        {
            get { return _action; }
            set
            {
                _action = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
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

        internal const string ReplyActionPropertyName = "ReplyAction";
        public string ReplyAction
        {
            get { return _replyAction; }
            set
            {
                _replyAction = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        public bool AsyncPattern { get; set; } = false;

        public bool IsOneWay
        {
            get { return _isOneWay; }
            set { _isOneWay = value; }
        }

        public bool IsInitiating { get; set; } = true;

        public bool IsTerminating { get; set; } = false;

        internal bool IsSessionOpenNotificationEnabled
        {
            get
            {
                return Action == OperationDescription.SessionOpenedAction;
            }
        }

        internal void EnsureInvariants(MethodInfo methodInfo, string operationName)
        {
            if (IsSessionOpenNotificationEnabled)
            {
                if (!IsOneWay
                 || !IsInitiating
                 || methodInfo.GetParameters().Length > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        SRP.Format(SRP.ContractIsNotSelfConsistentWhenIsSessionOpenNotificationEnabled, operationName, "Action", OperationDescription.SessionOpenedAction, "IsOneWay", "IsInitiating")));
                }
            }
        }
    }
}
