// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    public class QueryStringConverter
    {
        // The set of types the converter supports out of the box never changes, so it is a single
        // shared FrozenSet<Type> rather than a per-instance Hashtable (fast, allocation-free lookups).
        private static readonly FrozenSet<Type> s_defaultSupportedQueryStringTypes =
            new[]
            {
                typeof(byte), typeof(sbyte), typeof(short), typeof(int), typeof(long),
                typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(double),
                typeof(bool), typeof(char), typeof(decimal), typeof(string), typeof(object),
                typeof(DateTime), typeof(TimeSpan), typeof(byte[]), typeof(Guid), typeof(Uri),
                typeof(DateTimeOffset),
            }.ToFrozenSet();

        // the cache does not have a quota since it is per endpoint and is
        // bounded by the number of types in the contract at the endpoint
        private readonly ConcurrentDictionary<Type, TypeConverter> _typeConverterCache;

        public QueryStringConverter()
        {
            _typeConverterCache = new ConcurrentDictionary<Type, TypeConverter>();
        }

        public virtual bool CanConvert(Type type)
        {
            if (s_defaultSupportedQueryStringTypes.Contains(type))
            {
                return true;
            }

            // otherwise check if its an enum
            if (typeof(Enum).IsAssignableFrom(type))
            {
                return true;
            }

            // check if there's a typeconverter defined on the type
            return (GetStringConverter(type) != null);
        }

        public virtual object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameterType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameterType));
            }

            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Byte:
                    return parameter == null ? default : XmlConvert.ToByte(parameter);
                case TypeCode.SByte:
                    return parameter == null ? default : XmlConvert.ToSByte(parameter);
                case TypeCode.Int16:
                    return parameter == null ? default : XmlConvert.ToInt16(parameter);
                case TypeCode.Int32:
                    {
                        if (typeof(Enum).IsAssignableFrom(parameterType))
                        {
                            return Enum.Parse(parameterType, parameter, true);
                        }
                        else
                        {
                            return parameter == null ? default : XmlConvert.ToInt32(parameter);
                        }
                    }
                case TypeCode.Int64:
                    return parameter == null ? default : XmlConvert.ToInt64(parameter);
                case TypeCode.UInt16:
                    return parameter == null ? default : XmlConvert.ToUInt16(parameter);
                case TypeCode.UInt32:
                    return parameter == null ? default : XmlConvert.ToUInt32(parameter);
                case TypeCode.UInt64:
                    return parameter == null ? default : XmlConvert.ToUInt64(parameter);
                case TypeCode.Single:
                    return parameter == null ? default : XmlConvert.ToSingle(parameter);
                case TypeCode.Double:
                    return parameter == null ? default : XmlConvert.ToDouble(parameter);
                case TypeCode.Char:
                    return parameter == null ? default : XmlConvert.ToChar(parameter);
                case TypeCode.Decimal:
                    return parameter == null ? default : XmlConvert.ToDecimal(parameter);
                case TypeCode.Boolean:
                    return parameter == null ? default : Convert.ToBoolean(parameter, CultureInfo.InvariantCulture);
                case TypeCode.String:
                    return parameter;
                case TypeCode.DateTime:
                    return parameter == null ? default : DateTime.Parse(parameter, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                default:
                    {
                        if (parameterType == typeof(TimeSpan))
                        {
                            // Support both XML and default TimeSpan representations.
                            TimeSpan result;
                            if (!TimeSpan.TryParse(parameter, out result))
                            {
                                result = parameter == null ? default : XmlConvert.ToTimeSpan(parameter);
                            }
                            return result;
                        }
                        else if (parameterType == typeof(Guid))
                        {
                            return parameter == null ? default : XmlConvert.ToGuid(parameter);
                        }
                        else if (parameterType == typeof(DateTimeOffset))
                        {
                            return (parameter == null) ? default : DateTimeOffset.Parse(parameter, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces);
                        }
                        else if (parameterType == typeof(byte[]))
                        {
                            return (!string.IsNullOrEmpty(parameter)) ? Convert.FromBase64String(parameter) : Array.Empty<byte>();
                        }
                        else if (parameterType == typeof(Uri))
                        {
                            return (!string.IsNullOrEmpty(parameter)) ? new Uri(parameter, UriKind.RelativeOrAbsolute) : null;
                        }
                        else if (parameterType == typeof(object))
                        {
                            return parameter;
                        }
                        else
                        {
                            TypeConverter stringConverter = GetStringConverter(parameterType);
                            if (stringConverter == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                                    SR.Format(
                                    SR.TypeNotSupportedByQueryStringConverter,
                                    parameterType.ToString(), GetType().Name)));
                            }

                            return stringConverter.ConvertFromInvariantString(parameter);
                        }
                    }
            }
        }

        public virtual string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameterType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameterType));
            }

            if (parameterType.IsValueType && parameter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parameter));
            }

            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Byte:
                    return XmlConvert.ToString((byte)parameter);
                case TypeCode.SByte:
                    return XmlConvert.ToString((sbyte)parameter);
                case TypeCode.Int16:
                    return XmlConvert.ToString((short)parameter);
                case TypeCode.Int32:
                    {
                        if (typeof(Enum).IsAssignableFrom(parameterType))
                        {
                            return Enum.Format(parameterType, parameter, "G");
                        }
                        else
                        {
                            return XmlConvert.ToString((int)parameter);
                        }
                    }
                case TypeCode.Int64:
                    return XmlConvert.ToString((long)parameter);
                case TypeCode.UInt16:
                    return XmlConvert.ToString((ushort)parameter);
                case TypeCode.UInt32:
                    return XmlConvert.ToString((uint)parameter);
                case TypeCode.UInt64:
                    return XmlConvert.ToString((ulong)parameter);
                case TypeCode.Single:
                    return XmlConvert.ToString((float)parameter);
                case TypeCode.Double:
                    return XmlConvert.ToString((double)parameter);
                case TypeCode.Char:
                    return XmlConvert.ToString((char)parameter);
                case TypeCode.Decimal:
                    return XmlConvert.ToString((decimal)parameter);
                case TypeCode.Boolean:
                    return XmlConvert.ToString((bool)parameter);
                case TypeCode.String:
                    return (string)parameter;
                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime)parameter, XmlDateTimeSerializationMode.RoundtripKind);
                default:
                    {
                        if (parameterType == typeof(TimeSpan))
                        {
                            return XmlConvert.ToString((TimeSpan)parameter);
                        }
                        else if (parameterType == typeof(Guid))
                        {
                            return XmlConvert.ToString((Guid)parameter);
                        }
                        else if (parameterType == typeof(DateTimeOffset))
                        {
                            return XmlConvert.ToString((DateTimeOffset)parameter);
                        }
                        else if (parameterType == typeof(byte[]))
                        {
                            return (parameter != null) ? Convert.ToBase64String((byte[])parameter, Base64FormattingOptions.None) : null;
                        }
                        else if (parameterType == typeof(Uri) || parameterType == typeof(object))
                        {
                            // URI or object
                            return (parameter != null) ? Convert.ToString(parameter, CultureInfo.InvariantCulture) : null;
                        }
                        else
                        {
                            TypeConverter stringConverter = GetStringConverter(parameterType);
                            if (stringConverter == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                                    SR.Format(
                                    SR.TypeNotSupportedByQueryStringConverter,
                                    parameterType.ToString(), GetType().Name)));
                            }
                            else
                            {
                                return stringConverter.ConvertToInvariantString(parameter);
                            }
                        }
                    }
            }
        }

        private TypeConverter GetStringConverter(Type parameterType)
        {
            if (_typeConverterCache.TryGetValue(parameterType, out TypeConverter cachedConverter))
            {
                return cachedConverter;
            }

            if (parameterType.GetCustomAttributes(typeof(TypeConverterAttribute), true) is TypeConverterAttribute[] typeConverterAttrs)
            {
                foreach (TypeConverterAttribute converterAttr in typeConverterAttrs)
                {
                    Type converterType = Type.GetType(converterAttr.ConverterTypeName, false, true);
                    if (converterType != null)
                    {
                        TypeConverter converter = null;
                        Exception handledException = null;
                        try
                        {
                            converter = (TypeConverter)Activator.CreateInstance(converterType);
                        }
                        catch (TargetInvocationException e)
                        {
                            handledException = e;
                        }
                        catch (MemberAccessException e)
                        {
                            handledException = e;
                        }
                        catch (TypeLoadException e)
                        {
                            handledException = e;
                        }
                        catch (COMException e)
                        {
                            handledException = e;
                        }
                        catch (InvalidComObjectException e)
                        {
                            handledException = e;
                        }
                        finally
                        {
                            if (handledException != null)
                            {
                                if (Fx.IsFatal(handledException))
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(handledException);
                                }
                                DiagnosticUtility.TraceHandledException(handledException, TraceEventType.Warning);
                            }
                        }

                        if (converter == null)
                        {
                            continue;
                        }

                        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                        {
                            return _typeConverterCache.GetOrAdd(parameterType, converter);
                        }
                    }
                }
            }

            return null;
        }
    }
}
