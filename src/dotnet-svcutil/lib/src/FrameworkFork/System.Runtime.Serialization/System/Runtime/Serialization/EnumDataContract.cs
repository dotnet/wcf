// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Text;
using Microsoft.Xml;
using System.Security;
using System.Linq;


namespace System.Runtime.Serialization
{
#if NET_NATIVE
    public sealed class EnumDataContract : DataContract
#else
    internal sealed class EnumDataContract : DataContract
#endif
    {
        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - holds instance of CriticalHelper which keeps state that is cached statically for serialization. 
        ///            Static fields are marked SecurityCritical or readonly to prevent
        ///            data from being modified or leaked to other components in appdomain.
        /// </SecurityNote>
        private EnumDataContractCriticalHelper _helper;

        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// Safe - doesn't leak anything
        /// </SecurityNote>
        [SecuritySafeCritical]
        public EnumDataContract() : base(new EnumDataContractCriticalHelper())
        {
            _helper = base.Helper as EnumDataContractCriticalHelper;
        }

        /// <SecurityNote>
        /// Critical - Accesses SecurityCritical static cache to look up a base contract name
        /// Safe - Read only access
        /// </SecurityNote>
        [SecuritySafeCritical]
        static internal Type GetBaseType(XmlQualifiedName baseContractName)
        {
            return EnumDataContractCriticalHelper.GetBaseType(baseContractName);
        }

        internal XmlQualifiedName BaseContractName
        {
            // TODO: [Fx.Tag.SecurityNote(Critical = "Fetches the critical BaseContractName property.",
            //    Safe = "BaseContractName only needs to be protected for write.")]
            [SecuritySafeCritical]
            get { return _helper.BaseContractName; }

            // TODO: [Fx.Tag.SecurityNote(Critical = "Sets the critical BaseContractName property.")]
            [SecurityCritical]
            set { _helper.BaseContractName = value; }
        }


        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// Safe - doesn't leak anything
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal EnumDataContract(Type type) : base(new EnumDataContractCriticalHelper(type))
        {
            _helper = base.Helper as EnumDataContractCriticalHelper;
        }
        public List<DataMember> Members
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Members property
            /// Safe - Members only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.Members; }
            set { _helper.Members = value; }
        }

        public List<long> Values
        {
            /// <SecurityNote>
            /// Critical - fetches the critical Values property
            /// Safe - Values only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.Values; }
            set { _helper.Values = value; }
        }

        public bool IsFlags
        {
            /// <SecurityNote>
            /// Critical - fetches the critical IsFlags property
            /// Safe - IsFlags only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsFlags; }
            set { _helper.IsFlags = value; }
        }

        public bool IsULong
        {
            /// <SecurityNote>
            /// Critical - fetches the critical IsULong property
            /// Safe - IsULong only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsULong; }
            set { _helper.IsULong = value; }
        }

        public XmlDictionaryString[] ChildElementNames
        {
            /// <SecurityNote>
            /// Critical - fetches the critical ChildElementNames property
            /// Safe - ChildElementNames only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.ChildElementNames; }
            set { _helper.ChildElementNames = value; }
        }

        internal override bool CanContainReferences
        {
            get { return false; }
        }
        [SecurityCritical]

        /// <SecurityNote>
        /// Critical - holds all state used for (de)serializing enums.
        ///            since the data is cached statically, we lock down access to it.
        /// </SecurityNote>
        private class EnumDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private static Dictionary<Type, XmlQualifiedName> s_typeToName;
            private static Dictionary<XmlQualifiedName, Type> s_nameToType;

            XmlQualifiedName baseContractName;
            private List<DataMember> _members;
            private List<long> _values;
            private bool _isULong;
            private bool _isFlags;
            private bool _hasDataContract;
            private XmlDictionaryString[] _childElementNames;

            static EnumDataContractCriticalHelper()
            {
                s_typeToName = new Dictionary<Type, XmlQualifiedName>();
                s_nameToType = new Dictionary<XmlQualifiedName, Type>();
                Add(typeof(sbyte), "byte");
                Add(typeof(byte), "unsignedByte");
                Add(typeof(short), "short");
                Add(typeof(ushort), "unsignedShort");
                Add(typeof(int), "int");
                Add(typeof(uint), "unsignedInt");
                Add(typeof(long), "long");
                Add(typeof(ulong), "unsignedLong");
            }

            static internal void Add(Type type, string localName)
            {
                XmlQualifiedName stableName = CreateQualifiedName(localName, Globals.SchemaNamespace);
                s_typeToName.Add(type, stableName);
                s_nameToType.Add(stableName, type);
            }

            static internal Type GetBaseType(XmlQualifiedName baseContractName)
            {
                Type retVal = null;
                s_nameToType.TryGetValue(baseContractName, out retVal);
                return retVal;
            }

            internal EnumDataContractCriticalHelper()
            {
                IsValueType = true;
            }

            internal EnumDataContractCriticalHelper(Type type) : base(type)
            {
                this.StableName = DataContract.GetStableName(type, out _hasDataContract);
                Type baseType = Enum.GetUnderlyingType(type);
                ImportBaseType(baseType);
                IsFlags = type.GetTypeInfo().IsDefined(Globals.TypeOfFlagsAttribute, false);
                ImportDataMembers();

                XmlDictionary dictionary = new XmlDictionary(2 + Members.Count);
                Name = dictionary.Add(StableName.Name);
                Namespace = dictionary.Add(StableName.Namespace);
                _childElementNames = new XmlDictionaryString[Members.Count];
                for (int i = 0; i < Members.Count; i++)
                    _childElementNames[i] = dictionary.Add(Members[i].Name);
                DataContractAttribute dataContractAttribute;
                if (TryGetDCAttribute(type, out dataContractAttribute))
                {
                    if (dataContractAttribute.IsReference)
                    {
                        DataContract.ThrowInvalidDataContractException(
                                SRSerialization.Format(SRSerialization.EnumTypeCannotHaveIsReference,
                                    DataContract.GetClrTypeFullName(type),
                                    dataContractAttribute.IsReference,
                                    false),
                                type);
                    }
                }
            }

            internal XmlQualifiedName BaseContractName
            {
                get
                {
                    return baseContractName;
                }
                set
                {
                    baseContractName = value;
                    Type baseType = GetBaseType(baseContractName);
                    if (baseType == null)
                        ThrowInvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidEnumBaseType, value.Name, value.Namespace, StableName.Name, StableName.Namespace));
                    ImportBaseType(baseType);
                }
            }

            internal List<DataMember> Members
            {
                get { return _members; }
                set { _members = value; }
            }

            internal List<long> Values
            {
                get { return _values; }
                set { _values = value; }
            }

            internal bool IsFlags
            {
                get { return _isFlags; }
                set { _isFlags = value; }
            }

            internal bool IsULong
            {
                get { return _isULong; }
                set { _isULong = value; }
            }

            internal XmlDictionaryString[] ChildElementNames
            {
                get { return _childElementNames; }
                set { _childElementNames = value; }
            }

            private void ImportBaseType(Type baseType)
            {
                _isULong = (baseType == Globals.TypeOfULong);
            }

            private void ImportDataMembers()
            {
                Type type = this.UnderlyingType;
                FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                Dictionary<string, DataMember> memberValuesTable = new Dictionary<string, DataMember>();
                List<DataMember> tempMembers = new List<DataMember>(fields.Length);
                List<long> tempValues = new List<long>(fields.Length);

                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    bool enumMemberValid = false;
                    if (_hasDataContract)
                    {
                        object[] memberAttributes = field.GetCustomAttributes(Globals.TypeOfEnumMemberAttribute, false).ToArray();
                        if (memberAttributes != null && memberAttributes.Length > 0)
                        {
                            if (memberAttributes.Length > 1)
                                ThrowInvalidDataContractException(SRSerialization.Format(SRSerialization.TooManyEnumMembers, DataContract.GetClrTypeFullName(field.DeclaringType), field.Name));
                            EnumMemberAttribute memberAttribute = (EnumMemberAttribute)memberAttributes[0];

                            DataMember memberContract = new DataMember(field);
                            if (memberAttribute.IsValueSetExplicitly)
                            {
                                if (memberAttribute.Value == null || memberAttribute.Value.Length == 0)
                                    ThrowInvalidDataContractException(SRSerialization.Format(SRSerialization.InvalidEnumMemberValue, field.Name, DataContract.GetClrTypeFullName(type)));
                                memberContract.Name = memberAttribute.Value;
                            }
                            else
                                memberContract.Name = field.Name;
                            ClassDataContract.CheckAndAddMember(tempMembers, memberContract, memberValuesTable);
                            enumMemberValid = true;
                        }

                        object[] dataMemberAttributes = field.GetCustomAttributes(Globals.TypeOfDataMemberAttribute, false).ToArray();
                        if (dataMemberAttributes != null && dataMemberAttributes.Length > 0)
                            ThrowInvalidDataContractException(SRSerialization.Format(SRSerialization.DataMemberOnEnumField, DataContract.GetClrTypeFullName(field.DeclaringType), field.Name));
                    }
                    else
                    {
                        DataMember memberContract = new DataMember(field);
                        memberContract.Name = field.Name;
                        ClassDataContract.CheckAndAddMember(tempMembers, memberContract, memberValuesTable);
                        enumMemberValid = true;
                    }

                    if (enumMemberValid)
                    {
                        object enumValue = field.GetValue(null);
                        if (_isULong)
                            tempValues.Add((long)Convert.ToUInt64(enumValue, null));
                        else
                            tempValues.Add(Convert.ToInt64(enumValue, null));
                    }
                }

                Interlocked.MemoryBarrier();
                _members = tempMembers;
                _values = tempValues;
            }
        }

        internal void WriteEnumValue(XmlWriterDelegator writer, object value)
        {
            long longValue = IsULong ? (long)Convert.ToUInt64(value, null) : Convert.ToInt64(value, null);
            for (int i = 0; i < Values.Count; i++)
            {
                if (longValue == Values[i])
                {
                    writer.WriteString(ChildElementNames[i].Value);
                    return;
                }
            }
            if (IsFlags)
            {
                int zeroIndex = -1;
                bool noneWritten = true;
                for (int i = 0; i < Values.Count; i++)
                {
                    long current = Values[i];
                    if (current == 0)
                    {
                        zeroIndex = i;
                        continue;
                    }
                    if (longValue == 0)
                        break;
                    if ((current & longValue) == current)
                    {
                        if (noneWritten)
                            noneWritten = false;
                        else
                            writer.WriteString(DictionaryGlobals.Space.Value);

                        writer.WriteString(ChildElementNames[i].Value);
                        longValue &= ~current;
                    }
                }
                // enforce that enum value was completely parsed
                if (longValue != 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SRSerialization.Format(SRSerialization.InvalidEnumValueOnWrite, value, DataContract.GetClrTypeFullName(UnderlyingType))));

                if (noneWritten && zeroIndex >= 0)
                    writer.WriteString(ChildElementNames[zeroIndex].Value);
            }
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SRSerialization.Format(SRSerialization.InvalidEnumValueOnWrite, value, DataContract.GetClrTypeFullName(UnderlyingType))));
        }

        internal object ReadEnumValue(XmlReaderDelegator reader)
        {
            string stringValue = reader.ReadElementContentAsString();
            long longValue = 0;
            int i = 0;
            if (IsFlags)
            {
                // Skip initial spaces
                for (; i < stringValue.Length; i++)
                    if (stringValue[i] != ' ')
                        break;

                // Read space-delimited values
                int startIndex = i;
                int count = 0;
                for (; i < stringValue.Length; i++)
                {
                    if (stringValue[i] == ' ')
                    {
                        count = i - startIndex;
                        if (count > 0)
                            longValue |= ReadEnumValue(stringValue, startIndex, count);
                        for (++i; i < stringValue.Length; i++)
                            if (stringValue[i] != ' ')
                                break;
                        startIndex = i;
                        if (i == stringValue.Length)
                            break;
                    }
                }
                count = i - startIndex;
                if (count > 0)
                    longValue |= ReadEnumValue(stringValue, startIndex, count);
            }
            else
            {
                if (stringValue.Length == 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SRSerialization.Format(SRSerialization.InvalidEnumValueOnRead, stringValue, DataContract.GetClrTypeFullName(UnderlyingType))));
                longValue = ReadEnumValue(stringValue, 0, stringValue.Length);
            }

            if (IsULong)
                return Enum.ToObject(UnderlyingType, (object)(ulong)longValue);
            return Enum.ToObject(UnderlyingType, (object)longValue);
        }

        private long ReadEnumValue(string value, int index, int count)
        {
            for (int i = 0; i < Members.Count; i++)
            {
                string memberName = Members[i].Name;
                if (memberName.Length == count && String.CompareOrdinal(value, index, memberName, 0, count) == 0)
                {
                    return Values[i];
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SRSerialization.Format(SRSerialization.InvalidEnumValueOnRead, value.Substring(index, count), DataContract.GetClrTypeFullName(UnderlyingType))));
        }
        public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            WriteEnumValue(xmlWriter, obj);
        }

        public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            object obj = ReadEnumValue(xmlReader);
            if (context != null)
                context.AddNewObject(obj);
            return obj;
        }

        internal string GetStringFromEnumValue(long value)
        {
            if (IsULong)
                return XmlConvert.ToString((ulong)value);
            else
                return XmlConvert.ToString(value);
        }

        internal long GetEnumValueFromString(string value)
        {
            if (IsULong)
                return (long)XmlConverter.ToUInt64(value);
            else
                return XmlConverter.ToInt64(value);
        }
    }
}
