// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = false, AllowMultiple = false)]
    public sealed class DataContractFormatAttribute : Attribute
    {
        private OperationFormatStyle _style;
        public OperationFormatStyle Style
        {
            get { return _style; }
            set
            {
                XmlSerializerFormatAttribute.ValidateOperationFormatStyle(_style);
                _style = value;
            }
        }
    }
}
