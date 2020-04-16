// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;

    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = false, AllowMultiple = false)]
    public sealed class XmlSerializerFormatAttribute : Attribute
    {
        private bool _supportFaults = false;
        private OperationFormatStyle _style;
        private bool _isStyleSet;
        private OperationFormatUse _use;

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
                _isStyleSet = true;
            }
        }

        public OperationFormatUse Use
        {
            get { return _use; }
            set
            {
                ValidateOperationFormatUse(value);
                _use = value;
                if (!_isStyleSet && IsEncoded)
                    Style = OperationFormatStyle.Rpc;
            }
        }

        internal bool IsEncoded
        {
            get { return _use == OperationFormatUse.Encoded; }
            set { _use = value ? OperationFormatUse.Encoded : OperationFormatUse.Literal; }
        }

        static internal void ValidateOperationFormatStyle(OperationFormatStyle value)
        {
            if (!OperationFormatStyleHelper.IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }

        static internal void ValidateOperationFormatUse(OperationFormatUse value)
        {
            if (!OperationFormatUseHelper.IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }
    }
}
