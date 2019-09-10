// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

