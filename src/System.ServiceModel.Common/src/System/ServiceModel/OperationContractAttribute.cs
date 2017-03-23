// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                        SR.SFxNameCannotBeEmpty));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                _action = value;
            }
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
    }
}
