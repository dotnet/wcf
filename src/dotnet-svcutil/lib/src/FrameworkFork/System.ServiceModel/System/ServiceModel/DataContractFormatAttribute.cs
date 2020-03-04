// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
