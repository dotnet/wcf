// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.OperationContract, AllowMultiple = true, Inherited = false)]
    public sealed class FaultContractAttribute : Attribute
    {
        private string _action;
        private string _name;
        private string _ns;
        private Type _type;

        public FaultContractAttribute(Type detailType)
        {
            if (detailType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(detailType)));

            _type = detailType;
        }

        public Type DetailType
        {
            get { return _type; }
        }

        public string Action
        {
            get { return _action; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                _action = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                if (value == string.Empty)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        SR.SFxNameCannotBeEmpty));
                _name = value;
            }
        }

        public string Namespace
        {
            get { return _ns; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    NamingHelper.CheckUriProperty(value, "Namespace");
                _ns = value;
            }
        }
    }
}

