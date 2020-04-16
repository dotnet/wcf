// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security;
using System.Diagnostics;
using System.Net.Security;
using Microsoft.CodeDom;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={_name}, Action={_action}, DetailType={_detailType}")]
    public class FaultDescription
    {
        private string _action;
        private Type _detailType;
        private CodeTypeReference _detailTypeReference;
        private XmlName _elementName;
        private XmlName _name;
        private string _ns;
        private ProtectionLevel _protectionLevel;
        private bool _hasProtectionLevel;

        public FaultDescription(string action)
        {
            if (action == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("action"));

            _action = action;
        }

        public string Action
        {
            get { return _action; }
            internal set { _action = value; }
        }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public Type DetailType
        {
            get { return _detailType; }
            set { _detailType = value; }
        }

        internal CodeTypeReference DetailTypeReference
        {
            get { return _detailTypeReference; }
            set { _detailTypeReference = value; }
        }

        public string Name
        {
            get { return _name.EncodedName; }
            set { SetNameAndElement(new XmlName(value, true /*isEncoded*/)); }
        }

        public string Namespace
        {
            get { return _ns; }
            set { _ns = value; }
        }

        internal XmlName ElementName
        {
            get { return _elementName; }
            set { _elementName = value; }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _protectionLevel = value;
                _hasProtectionLevel = true;
            }
        }

        public bool ShouldSerializeProtectionLevel()
        {
            return this.HasProtectionLevel;
        }

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            _hasProtectionLevel = false;
        }

        internal void SetNameAndElement(XmlName name)
        {
            _elementName = _name = name;
        }

        internal void SetNameOnly(XmlName name)
        {
            _name = name;
        }
    }
}
