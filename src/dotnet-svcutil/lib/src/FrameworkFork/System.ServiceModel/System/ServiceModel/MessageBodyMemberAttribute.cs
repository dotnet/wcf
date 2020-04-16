// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.MessageMember, Inherited = false)]
    public class MessageBodyMemberAttribute : MessageContractMemberAttribute
    {
        private int _order = -1;
        internal const string OrderPropertyName = "Order";
        public int Order
        {
            get { return _order; }
            set
            {
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBeNonNegative));
                _order = value;
            }
        }
    }
}

