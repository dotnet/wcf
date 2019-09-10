// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.Parameter, Inherited = false)]
    public sealed class MessageParameterAttribute : Attribute
    {
        private string _name;
        private bool _isNameSetExplicit;
        internal const string NamePropertyName = "Name";
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == string.Empty)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value",
                        SRServiceModel.SFxNameCannotBeEmpty));
                }
                _name = value; _isNameSetExplicit = true;
            }
        }

        internal bool IsNameSetExplicit
        {
            get { return _isNameSetExplicit; }
        }
    }
}
