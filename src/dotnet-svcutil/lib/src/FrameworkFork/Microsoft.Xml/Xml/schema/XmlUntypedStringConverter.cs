// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Xml;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Diagnostics;

#if SILVERLIGHT

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    // This is an atomic value converted for Silverlight XML core that knows only how to convert to and from string. 
    // It does not recognize XmlAtomicValue or XPathItemType.
    internal class XmlUntypedStringConverter {
        // Fields
        bool listsAllowed;
        XmlUntypedStringConverter listItemConverter;

        // Cached types
        static readonly Type DecimalType = typeof(decimal);
        static readonly Type Int32Type = typeof(int);
        static readonly Type Int64Type = typeof(long);
        static readonly Type StringType = typeof(string);
        static readonly Type ObjectType = typeof(object);
        static readonly Type ByteType = typeof(byte);
        static readonly Type Int16Type = typeof(short);
        static readonly Type SByteType = typeof(sbyte);
        static readonly Type UInt16Type = typeof(ushort);
        static readonly Type UInt32Type = typeof(uint);
        static readonly Type UInt64Type = typeof(ulong);
        static readonly Type DoubleType = typeof(double);
        static readonly Type SingleType = typeof(float);
        static readonly Type DateTimeType = typeof(DateTime);
        static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
        static readonly Type BooleanType = typeof(bool);
        static readonly Type ByteArrayType = typeof(Byte[]);
        static readonly Type XmlQualifiedNameType = typeof(XmlQualifiedName);
        static readonly Type UriType = typeof(Uri);
        static readonly Type TimeSpanType = typeof(TimeSpan);

        static readonly string UntypedStringTypeName = "xdt:untypedAtomic";

        // Static convertor instance
        internal static XmlUntypedStringConverter Instance = new XmlUntypedStringConverter(true);
        
        private XmlUntypedStringConverter(bool listsAllowed) {
            this.listsAllowed = listsAllowed;
            if (listsAllowed) {
                this.listItemConverter = new XmlUntypedStringConverter(false);
            }
        }

        internal string ToString(object value, IXmlNamespaceResolver nsResolver) {
            if (value == null) throw new ArgumentNullException("value");

            Type sourceType = value.GetType();

            if (sourceType == BooleanType) return XmlConvert.ToString((bool)value);
            if (sourceType == ByteType) return XmlConvert.ToString((byte)value);
            if (sourceType == ByteArrayType) return Base64BinaryToString((byte[])value);
            if (sourceType == DateTimeType) return DateTimeToString((DateTime)value);
            if (sourceType == DateTimeOffsetType) return DateTimeOffsetToString((DateTimeOffset)value);
            if (sourceType == DecimalType) return XmlConvert.ToString((decimal)value);
            if (sourceType == DoubleType) return XmlConvert.ToString((double)value);
            if (sourceType == Int16Type) return XmlConvert.ToString((short)value);
            if (sourceType == Int32Type) return XmlConvert.ToString((int)value);
            if (sourceType == Int64Type) return XmlConvert.ToString((long)value);
            if (sourceType == SByteType) return XmlConvert.ToString((sbyte)value);
            if (sourceType == SingleType) return XmlConvert.ToString((float)value);
            if (sourceType == StringType) return ((string)value);
            if (sourceType == TimeSpanType) return DurationToString((TimeSpan)value);
            if (sourceType == UInt16Type) return XmlConvert.ToString((ushort)value);
            if (sourceType == UInt32Type) return XmlConvert.ToString((uint)value);
            if (sourceType == UInt64Type) return XmlConvert.ToString((ulong)value);
            if (IsDerivedFrom(sourceType, UriType)) return AnyUriToString((Uri)value);
            if (IsDerivedFrom(sourceType, XmlQualifiedNameType)) return QNameToString((XmlQualifiedName)value, nsResolver);

            return (string)ListTypeToString(value, nsResolver);
        }

        internal object FromString(string value, Type destinationType, IXmlNamespaceResolver nsResolver) {
            if (value == null) throw new ArgumentNullException("value");
            if (destinationType == null) throw new ArgumentNullException("destinationType");

            if (destinationType == ObjectType) destinationType = typeof(string);
            if (destinationType == BooleanType) return XmlConvert.ToBoolean((string)value);
            if (destinationType == ByteType) return Int32ToByte(XmlConvert.ToInt32((string)value));
            if (destinationType == ByteArrayType) return StringToBase64Binary((string)value);
            if (destinationType == DateTimeType) return StringToDateTime((string)value);
            if (destinationType == DateTimeOffsetType) return StringToDateTimeOffset((string)value);
            if (destinationType == DecimalType) return XmlConvert.ToDecimal((string)value);
            if (destinationType == DoubleType) return XmlConvert.ToDouble((string)value);
            if (destinationType == Int16Type) return Int32ToInt16(XmlConvert.ToInt32((string)value));
            if (destinationType == Int32Type) return XmlConvert.ToInt32((string)value);
            if (destinationType == Int64Type) return XmlConvert.ToInt64((string)value);
            if (destinationType == SByteType) return Int32ToSByte(XmlConvert.ToInt32((string)value));
            if (destinationType == SingleType) return XmlConvert.ToSingle((string)value);
            if (destinationType == TimeSpanType) return StringToDuration((string)value);
            if (destinationType == UInt16Type) return Int32ToUInt16(XmlConvert.ToInt32((string)value));
            if (destinationType == UInt32Type) return Int64ToUInt32(XmlConvert.ToInt64((string)value));
            if (destinationType == UInt64Type) return DecimalToUInt64(XmlConvert.ToDecimal((string)value));
            if (destinationType == UriType) return XmlConvert.ToUri((string)value);
            if (destinationType == XmlQualifiedNameType) return StringToQName((string)value, nsResolver);
            if (destinationType == StringType) return ((string)value);

            return StringToListType(value, destinationType, nsResolver);
        }

        byte Int32ToByte(int value) {
            if (value < (int)Byte.MinValue || value > (int)Byte.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "Byte" }));

            return (byte)value;
        }

        short Int32ToInt16(int value) {
            if (value < (int)Int16.MinValue || value > (int)Int16.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "Int16" }));

            return (short)value;
        }

        sbyte Int32ToSByte(int value) {
            if (value < (int)SByte.MinValue || value > (int)SByte.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "SByte" }));

            return (sbyte)value;
        }

        ushort Int32ToUInt16(int value) {
            if (value < (int)UInt16.MinValue || value > (int)UInt16.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "UInt16" }));

            return (ushort)value;
        }

        uint Int64ToUInt32(long value) {
            if (value < (long)UInt32.MinValue || value > (long)UInt32.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "UInt32" }));

            return (uint)value;
        }

        ulong DecimalToUInt64(decimal value) {
            if (value < (decimal)UInt64.MinValue || value > (decimal)UInt64.MaxValue)
                throw new OverflowException(Res.GetString(Res.XmlConvert_Overflow, new string[] { XmlConvert.ToString(value), "UInt64" }));

            return (ulong)value;
        }

        string Base64BinaryToString(byte[] value) {
            return Convert.ToBase64String(value);
        }

        byte[] StringToBase64Binary(string value) {
            return Convert.FromBase64String(XmlConvert.TrimString(value));
        }

        string DateTimeToString(DateTime value) {
            return (new XsdDateTime(value, XsdDateTimeFlags.DateTime)).ToString();
        }

        static DateTime StringToDateTime(string value) {
            return (DateTime)(new XsdDateTime(value, XsdDateTimeFlags.AllXsd));
        }

        static string DateTimeOffsetToString(DateTimeOffset value) {
            return (new XsdDateTime(value, XsdDateTimeFlags.DateTime)).ToString();
        }

        static DateTimeOffset StringToDateTimeOffset(string value) {
            return (DateTimeOffset)(new XsdDateTime(value, XsdDateTimeFlags.AllXsd));
        }

        string DurationToString(TimeSpan value) {
            return new XsdDuration(value, XsdDuration.DurationType.Duration).ToString(XsdDuration.DurationType.Duration);
        }

        TimeSpan StringToDuration(string value) {
            return new XsdDuration(value, XsdDuration.DurationType.Duration).ToTimeSpan(XsdDuration.DurationType.Duration);
        }

        string AnyUriToString(Uri value) {
            return value.OriginalString;
        }

        protected static string QNameToString(XmlQualifiedName qname, IXmlNamespaceResolver nsResolver) {
            string prefix;

            if (nsResolver == null) {
                return string.Concat("{", qname.Namespace, "}", qname.Name);
            }

            prefix = nsResolver.LookupPrefix(qname.Namespace);
            if (prefix == null) {
                throw new InvalidCastException(Res.GetString(Res.XmlConvert_TypeNoPrefix, qname.ToString(), qname.Namespace));
            }

            return (prefix.Length != 0) ? string.Concat(prefix, ":", qname.Name) : qname.Name;
        }

        static XmlQualifiedName StringToQName(string value, IXmlNamespaceResolver nsResolver) {
            string prefix, localName, ns;

            value = value.Trim();

            // Parse prefix:localName
            try {
                ValidateNames.ParseQNameThrow(value, out prefix, out localName);
            }
            catch (XmlException e) {
                throw new FormatException(e.Message);
            }

            // Throw error if no namespaces are in scope
            if (nsResolver == null)
                throw new InvalidCastException(Res.GetString(Res.XmlConvert_TypeNoNamespace, value, prefix));

            // Lookup namespace
            ns = nsResolver.LookupNamespace(prefix);
            if (ns == null)
                throw new InvalidCastException(Res.GetString(Res.XmlConvert_TypeNoNamespace, value, prefix));

            // Create XmlQualfiedName
            return new XmlQualifiedName(localName, ns);
        }

        string ListTypeToString(object value, IXmlNamespaceResolver nsResolver) {
            if (!listsAllowed || !(value is IEnumerable)) {
                throw CreateInvalidClrMappingException(value.GetType(), typeof(string));
            }

            StringBuilder bldr = new StringBuilder();

            foreach (object item in ((IEnumerable)value)) {
                // skip null values
                if (item != null) {
                    // Separate values by single space character
                    if (bldr.Length != 0)
                        bldr.Append(' ');

                    // Append string value of next item in the list
                    bldr.Append(this.listItemConverter.ToString(item, nsResolver));
                }
            }

            return bldr.ToString();
        }

        object StringToListType(string value, Type destinationType, IXmlNamespaceResolver nsResolver) {
            
            if (listsAllowed && destinationType.IsArray) {
                
                Type itemTypeDst = destinationType.GetElementType();

                // Different StringSplitOption needs to be used because of following bugs:
                // 566053: Behavior change between SL2 and Dev10 in the way string arrays are deserialized by the XmlReader.ReadContentsAs method	
                // 643697: Deserialization of typed arrays by the XmlReader.ReadContentsAs method fails	
                //
                // In Silverligt 2 the XmlConvert.SplitString was not using the StringSplitOptions, which is the same as using StringSplitOptions.None.
                // What it meant is that whenever there is a double space between two values in the input string it turned into 
                // an string.Empty entry in the intermediate string array. In Dev10 empty entries were always removed (StringSplitOptions.RemoveEmptyEntries).
                //
                // Moving forward in coreclr we'll use Dev10 behavior which empty entries were always removed (StringSplitOptions.RemoveEmptyEntries).
                // we didn't quirk the change because we discover not many apps using ReadContentAs with string array type parameter
                //
                // The types Object, Byte[], String and Uri can be successfully deserialized from string.Empty, so we need to preserve the 
                // Silverlight 2 behavior for back-compat (=use StringSplitOptions.None). All the other array types failed to deserialize
                // from string.Empty in Silverlight 2 (threw an exception), so we can fix all of these as they are not breaking changes 
                // (=use StringSplitOptions.RemoveEmptyEntries).

                if (itemTypeDst == ObjectType)           return ToArray<object>(          XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == BooleanType)          return ToArray<bool>(            XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == ByteType)             return ToArray<byte>(            XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == ByteArrayType)        return ToArray<byte[]>(          XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == DateTimeType)         return ToArray<DateTime>(        XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == DateTimeOffsetType)   return ToArray<DateTimeOffset>(  XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == DecimalType)          return ToArray<decimal>(         XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == DoubleType)           return ToArray<double>(          XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == Int16Type)            return ToArray<short>(           XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == Int32Type)            return ToArray<int>(             XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == Int64Type)            return ToArray<long>(            XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == SByteType)            return ToArray<sbyte>(           XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == SingleType)           return ToArray<float>(           XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == StringType)           return ToArray<string>(          XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == TimeSpanType)         return ToArray<TimeSpan>(        XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == UInt16Type)           return ToArray<ushort>(          XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == UInt32Type)           return ToArray<uint>(            XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == UInt64Type)           return ToArray<ulong>(           XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == UriType)              return ToArray<Uri>(             XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
                if (itemTypeDst == XmlQualifiedNameType) return ToArray<XmlQualifiedName>(XmlConvert.SplitString(value, StringSplitOptions.RemoveEmptyEntries), nsResolver);
            }
            throw CreateInvalidClrMappingException(typeof(string), destinationType);
        }

        private T[] ToArray<T>(string[] stringArray, IXmlNamespaceResolver nsResolver) {
            T[] arrDst = new T[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++) {
                arrDst[i] = (T)this.listItemConverter.FromString(stringArray[i], typeof(T), nsResolver);
            }
            return arrDst;
        }

        /// <summary>
        /// Type.IsSubtypeOf does not return true if types are equal, this method does.
        /// </summary>
        static bool IsDerivedFrom(Type derivedType, Type baseType) {
            while (derivedType != null) {
                if (derivedType == baseType)
                    return true;

                derivedType = derivedType.BaseType;
            }
            return false;
        }

        private Exception CreateInvalidClrMappingException(Type sourceType, Type destinationType) {
            return new InvalidCastException(Res.GetString(Res.XmlConvert_TypeListBadMapping2, UntypedStringTypeName, sourceType.Name, destinationType.Name));
        }
    }
}

#endif
