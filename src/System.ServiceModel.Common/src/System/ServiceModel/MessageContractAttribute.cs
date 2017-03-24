// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.MessageContract, AllowMultiple = false)]
    public sealed class MessageContractAttribute : Attribute
    {
        private bool _isWrapped = true;
        private string _wrappedName;
        private string _wrappedNs;

        public bool IsWrapped
        {
            get { return _isWrapped; }
            set { _isWrapped = value; }
        }

        public string WrapperName
        {
            get
            {
                return _wrappedName;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                if (value == string.Empty)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        SR.SFxWrapperNameCannotBeEmpty));
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
                    NamingHelper.CheckUriProperty(value, "WrapperNamespace");
                _wrappedNs = value;
            }
        }
    }
}
