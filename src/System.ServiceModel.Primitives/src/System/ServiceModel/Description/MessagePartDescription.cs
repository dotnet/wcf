// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Diagnostics;
using System.Net.Security;
using System.Reflection;
using System.ServiceModel.Security;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Name={XmlName}, Namespace={Namespace}, Type={Type}, Index={Index}}")]
    public class MessagePartDescription
    {
        private ProtectionLevel _protectionLevel;
        private ICustomAttributeProvider _additionalAttributesProvider;
        private string _uniquePartName;

        public MessagePartDescription(string name, string ns)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(name), SRP.SFxParameterNameCannotBeNull);
            }

            XmlName = new XmlName(name, true /*isEncoded*/);

            if (!string.IsNullOrEmpty(ns))
            {
                NamingHelper.CheckUriParameter(ns, nameof(ns));
            }

            Namespace = ns;
        }

        internal MessagePartDescription(MessagePartDescription other)
        {
            XmlName = other.XmlName;
            Namespace = other.Namespace;
            Index = other.Index;
            Type = other.Type;
            SerializationPosition = other.SerializationPosition;
            HasProtectionLevel = other.HasProtectionLevel;
            _protectionLevel = other._protectionLevel;
            MemberInfo = other.MemberInfo;
            Multiple = other.Multiple;
            _additionalAttributesProvider = other._additionalAttributesProvider;
            BaseType = other.BaseType;
            _uniquePartName = other._uniquePartName;
        }

        internal virtual MessagePartDescription Clone()
        {
            return new MessagePartDescription(this);
        }

        internal string BaseType { get; set; }

        internal XmlName XmlName { get; }

        internal string CodeName
        {
            get { return XmlName.DecodedName; }
        }

        public string Name
        {
            get { return XmlName.EncodedName; }
        }

        public string Namespace { get; }

        public Type Type { get; set; }

        public int Index { get; set; }

        [DefaultValue(false)]
        public bool Multiple { get; set; }

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

        public bool HasProtectionLevel { get; private set; }

        public MemberInfo MemberInfo { get; set; }

        internal ICustomAttributeProvider AdditionalAttributesProvider
        {
            get { return _additionalAttributesProvider ?? MemberInfo; }
            set { _additionalAttributesProvider = value; }
        }

        internal string UniquePartName
        {
            get { return _uniquePartName; }
            set { _uniquePartName = value; }
        }

        internal int SerializationPosition { get; set; }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            HasProtectionLevel = false;
        }
    }
}
