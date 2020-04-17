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
        private bool _asyncPattern = false;
        private bool _isInitiating = true;
        private bool _isOneWay = false;
        private ProtectionLevel _protectionLevel = ProtectionLevel.None;
        private bool _hasProtectionLevel = false;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == "")
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value",
                        SRServiceModel.SFxNameCannotBeEmpty));
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _action = value;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _protectionLevel = value;
                _hasProtectionLevel = true;
            }
        }

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }

        internal const string ReplyActionPropertyName = "ReplyAction";
        public string ReplyAction
        {
            get { return _replyAction; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _replyAction = value;
            }
        }

        public bool AsyncPattern
        {
            get { return _asyncPattern; }
            set { _asyncPattern = value; }
        }

        public bool IsOneWay
        {
            get { return _isOneWay; }
            set { _isOneWay = value; }
        }

        public bool IsInitiating
        {
            get { return _isInitiating; }
            set { _isInitiating = value; }
        }

        internal bool IsSessionOpenNotificationEnabled
        {
            get
            {
                return this.Action == OperationDescription.SessionOpenedAction;
            }
        }

        internal void EnsureInvariants(MethodInfo methodInfo, string operationName)
        {
            if (this.IsSessionOpenNotificationEnabled)
            {
                if (!this.IsOneWay
                 || !this.IsInitiating
                 || methodInfo.GetParameters().Length > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.ContractIsNotSelfConsistentWhenIsSessionOpenNotificationEnabled, operationName, "Action", OperationDescription.SessionOpenedAction, "IsOneWay", "IsInitiating")));
                }
            }
        }
    }
}
