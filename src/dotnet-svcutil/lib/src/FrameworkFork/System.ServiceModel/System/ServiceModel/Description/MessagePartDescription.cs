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
    [DebuggerDisplay("Name={_name}, Namespace={_ns}, Type={Type}, Index={_index}}")]
    public class MessagePartDescription
    {
        private XmlName _name;
        private string _ns;
        private int _index;
        private Type _type;
        private int _serializationPosition;
        private ProtectionLevel _protectionLevel;
        private bool _hasProtectionLevel;
        private MemberInfo _memberInfo;
        private CustomAttributeProvider _additionalAttributesProvider;

        private bool _multiple;
        private string _baseType;
        private string _uniquePartName;

        public MessagePartDescription(string name, string ns)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name", SRServiceModel.SFxParameterNameCannotBeNull);
            }

            _name = new XmlName(name, true /*isEncoded*/);

            if (!string.IsNullOrEmpty(ns))
            {
                NamingHelper.CheckUriParameter(ns, "ns");
            }

            _ns = ns;
        }

        internal MessagePartDescription(MessagePartDescription other)
        {
            _name = other._name;
            _ns = other._ns;
            _index = other._index;
            _type = other._type;
            _serializationPosition = other._serializationPosition;
            _hasProtectionLevel = other._hasProtectionLevel;
            _protectionLevel = other._protectionLevel;
            _memberInfo = other._memberInfo;
            _multiple = other._multiple;
            _additionalAttributesProvider = other._additionalAttributesProvider;
            _baseType = other._baseType;
            _uniquePartName = other._uniquePartName;
        }

        internal virtual MessagePartDescription Clone()
        {
            return new MessagePartDescription(this);
        }

        internal string BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        internal XmlName XmlName
        {
            get { return _name; }
        }

        internal string CodeName
        {
            get { return _name.DecodedName; }
        }

        public string Name
        {
            get { return _name.EncodedName; }
        }

        public string Namespace
        {
            get { return _ns; }
        }

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        [DefaultValue(false)]
        public bool Multiple
        {
            get { return _multiple; }
            set { _multiple = value; }
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

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }

        public MemberInfo MemberInfo
        {
            get { return _memberInfo; }
            set { _memberInfo = value; }
        }

        internal CustomAttributeProvider AdditionalAttributesProvider
        {
            get { return _additionalAttributesProvider ?? _memberInfo; }
            set { _additionalAttributesProvider = value; }
        }

        internal string UniquePartName
        {
            get { return _uniquePartName; }
            set { _uniquePartName = value; }
        }

        internal int SerializationPosition
        {
            get { return _serializationPosition; }
            set { _serializationPosition = value; }
        }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            _hasProtectionLevel = false;
        }
    }
}
