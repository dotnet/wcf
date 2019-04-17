// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



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
