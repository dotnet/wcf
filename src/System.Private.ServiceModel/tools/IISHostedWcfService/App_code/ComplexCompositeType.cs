// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
using System.Runtime.Serialization;
#endif
using System.Text;

namespace WcfService
{
    [DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
    public class ComplexCompositeType : IEquatable<ComplexCompositeType>
    {
        private bool _boolValue;
        private char _charValue;
        private DateTime _dateTimeValue;

        private int _intValue;
        private short _shortValue;
        private float _floatValue;
        private long _longValue;
        private double _doubleValue;

        private uint _uintValue;
        private ushort _ushortValue;
        private ulong _ulongValue;

        private Guid _guidValue;

        private byte[] _byteArrayValue;
        private char[] _charArrayValue;

        private string _stringValue;
        private string _longerStringValue;

        private sbyte _sbyteValue;
        private TimeSpan _timeSpanValue;
        private DayOfWeek _dayOfWeekValue;

        [DataMember]
        public bool BoolValue
        {
            get { return _boolValue; }
            set { _boolValue = value; }
        }

        [DataMember]
        public char CharValue
        {
            get { return _charValue; }
            set { _charValue = value; }
        }

        [DataMember]
        public DateTime DateTimeValue
        {
            get { return _dateTimeValue; }
            set { _dateTimeValue = value; }
        }

        [DataMember]
        public int IntValue
        {
            get { return _intValue; }
            set { _intValue = value; }
        }

        [DataMember]
        public short ShortValue
        {
            get { return _shortValue; }
            set { _shortValue = value; }
        }

        [DataMember]
        public float FloatValue
        {
            get { return _floatValue; }
            set { _floatValue = value; }
        }

        [DataMember]
        public long LongValue
        {
            get { return _longValue; }
            set { _longValue = value; }
        }

        [DataMember]
        public double DoubleValue
        {
            get { return _doubleValue; }
            set { _doubleValue = value; }
        }

        [DataMember]
        public uint UintValue
        {
            get { return _uintValue; }
            set { _uintValue = value; }
        }

        [DataMember]
        public ushort UshortValue
        {
            get { return _ushortValue; }
            set { _ushortValue = value; }
        }

        [DataMember]
        public ulong UlongValue
        {
            get { return _ulongValue; }
            set { _ulongValue = value; }
        }

        [DataMember]
        public Guid GuidValue
        {
            get { return _guidValue; }
            set { _guidValue = value; }
        }

        [DataMember]
        public byte[] ByteArrayValue
        {
            get { return _byteArrayValue; }
            set { _byteArrayValue = value; }
        }

        [DataMember]
        public char[] CharArrayValue
        {
            get { return _charArrayValue; }
            set { _charArrayValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        [DataMember]
        public string LongerStringValue
        {
            get { return _longerStringValue; }
            set { _longerStringValue = value; }
        }

        [DataMember]
        public sbyte SbyteValue
        {
            get { return _sbyteValue; }
            set { _sbyteValue = value; }
        }

        [DataMember]
        public TimeSpan TimeSpanValue
        {
            get { return _timeSpanValue; }
            set { _timeSpanValue = value; }
        }

        [DataMember]
        public DayOfWeek DayOfWeekValue
        {
            get { return _dayOfWeekValue; }
            set { _dayOfWeekValue = value; }
        }

        public bool Equals(ComplexCompositeType other)
        {
            if (other == null) { return false; }
            if (object.ReferenceEquals(this, other)) { return true; }

            if (_boolValue != other._boolValue) { return false; }
            if (_charValue != other._charValue) { return false; }
            if (_dateTimeValue != other._dateTimeValue) { return false; }

            if (_intValue != other._intValue) { return false; }
            if (_shortValue != other._shortValue) { return false; }
            if (_floatValue != other._floatValue) { return false; }
            if (_longValue != other._longValue) { return false; }
            if (_doubleValue != other._doubleValue) { return false; }

            if (_uintValue != other._uintValue) { return false; }
            if (_ushortValue != other._ushortValue) { return false; }
            if (_ulongValue != other._ulongValue) { return false; }

            if (_guidValue != other._guidValue) { return false; }

            if (_byteArrayValue.Length != other._byteArrayValue.Length) { return false; }
            for (int i = 0; i < _byteArrayValue.Length; i++)
            {
                if (_byteArrayValue[i] != other._byteArrayValue[i]) { return false; }
            }

            for (int i = 0; i < _charArrayValue.Length; i++)
            {
                if (_charArrayValue[i] != other._charArrayValue[i]) { return false; }
            }

            if (_stringValue != other._stringValue) { return false; }
            if (_longerStringValue != other._longerStringValue) { return false; }

            if (_sbyteValue != other._sbyteValue) { return false; }
            if (_timeSpanValue != other._timeSpanValue) { return false; }
            if (_dayOfWeekValue != other._dayOfWeekValue) { return false; }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("BoolValue: " + _boolValue);
            sb.AppendLine("CharValue: " + _charValue);
            sb.AppendLine("DateTimeValue: " + _dateTimeValue);
            sb.AppendLine("IntValue: " + _intValue);
            sb.AppendLine("ShortValue: " + _shortValue);
            sb.AppendLine("FloatValue: " + _floatValue);
            sb.AppendLine("LongValue: " + _longValue);
            sb.AppendLine("DoubleValue: " + _doubleValue);
            sb.AppendLine("UintValue: " + _uintValue);
            sb.AppendLine("ushortValue: " + _ushortValue);
            sb.AppendLine("ulongValue: " + _ulongValue);
            sb.AppendLine("GuidValue: " + _guidValue);
            sb.AppendLine("ByteArrayValue: " + (_byteArrayValue != null ? "Length: " + _byteArrayValue.Length : "null"));
            sb.AppendLine("CharArrayValue: " + (_charArrayValue != null ? "Length: " + _charArrayValue.Length : "null"));
            sb.AppendLine("StringValue: " + (string.IsNullOrEmpty(_stringValue) ? "<empty>" : _stringValue.ToString()));
            sb.AppendLine("LongerStringValue: " + (string.IsNullOrEmpty(_longerStringValue) ? "<empty>" : _longerStringValue.ToString()));
            sb.AppendLine("SbyteValue: " + _sbyteValue);
            sb.AppendLine("TimeSpanValue: " + _timeSpanValue);
            sb.AppendLine("DayOfWeekValue: " + _dayOfWeekValue);

            return sb.ToString();
        }
    }

    // This type should only be used by test Contract.DataContractTests.NetTcpBinding_DuplexCallback_ReturnsDataContractComplexType
    // It tests a narrow scenario that returns a DataContract attributed type in the callback method that is not known by the ServiceContract attributed interface
    // This test is designed to make sure the NET Native toolchain creates the needed serializer
    [DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
    public class ComplexCompositeTypeDuplexCallbackOnly : IEquatable<ComplexCompositeTypeDuplexCallbackOnly>
    {
        private Guid _guidValue;

        [DataMember]
        public Guid GuidValue
        {
            get { return _guidValue; }
            set { _guidValue = value; }
        }

        public bool Equals(ComplexCompositeTypeDuplexCallbackOnly other)
        {
            if (other == null) { return false; }
            if (object.ReferenceEquals(this, other)) { return true; }

            if (_guidValue != other._guidValue) { return false; }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GuidValue: " + _guidValue);

            return sb.ToString();
        }
    }
}
