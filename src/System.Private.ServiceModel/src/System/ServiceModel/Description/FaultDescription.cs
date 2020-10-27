// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using System.Diagnostics;
using System.Net.Security;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={_name}, Action={Action}, DetailType={DetailType}")]
    public class FaultDescription
    {
        private XmlName _name;
        private ProtectionLevel _protectionLevel;

        public FaultDescription(string action)
        {
            Action = action ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(action)));
        }

        public string Action { get; internal set; }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        public Type DetailType { get; set; }

        public string Name
        {
            get { return _name.EncodedName; }
            set { SetNameAndElement(new XmlName(value, true /*isEncoded*/)); }
        }

        public string Namespace { get; set; }

        internal XmlName ElementName { get; set; }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _protectionLevel = value;
                HasProtectionLevel = true;
            }
        }

        public bool ShouldSerializeProtectionLevel()
        {
            return HasProtectionLevel;
        }

        public bool HasProtectionLevel { get; private set; }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            HasProtectionLevel = false;
        }

        internal void SetNameAndElement(XmlName name)
        {
            ElementName = _name = name;
        }

        internal void SetNameOnly(XmlName name)
        {
            _name = name;
        }
    }
}
