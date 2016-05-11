// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = false, AllowMultiple = false)]
    public sealed class XmlSerializerFormatAttribute : Attribute
    {
        private bool _supportFaults = false;
        private OperationFormatStyle _style;

        public bool SupportFaults
        {
            get { return _supportFaults; }
            set { _supportFaults = value; }
        }

        public OperationFormatStyle Style
        {
            get { return _style; }
            set
            {
                ValidateOperationFormatStyle(value);
                _style = value;
            }
        }

        static internal void ValidateOperationFormatStyle(OperationFormatStyle value)
        {
            if (!OperationFormatStyleHelper.IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }
    }
}
