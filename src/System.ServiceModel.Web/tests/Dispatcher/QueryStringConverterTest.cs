// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class QueryStringConverterTest
{
    // Every type QueryStringConverter claims to handle out of the box.
    private static readonly Type[] s_supportedTypes =
    {
        typeof(byte), typeof(sbyte), typeof(short), typeof(int), typeof(long),
        typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(double),
        typeof(bool), typeof(char), typeof(decimal), typeof(string), typeof(object),
        typeof(DateTime), typeof(TimeSpan), typeof(byte[]), typeof(Guid), typeof(Uri),
        typeof(DateTimeOffset)
    };

    [WcfFact]
    public static void CanConvert_Returns_True_For_All_Supported_Types()
    {
        QueryStringConverter converter = new QueryStringConverter();
        foreach (Type type in s_supportedTypes)
        {
            Assert.True(converter.CanConvert(type), $"Expected CanConvert({type}) to be true");
        }
    }

    [WcfFact]
    public static void CanConvert_Returns_True_For_Enums()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.True(converter.CanConvert(typeof(SampleEnum)));
        Assert.True(converter.CanConvert(typeof(SampleFlagsEnum)));
        Assert.True(converter.CanConvert(typeof(SampleByteEnum)));
    }

    [WcfFact]
    public static void CanConvert_Returns_True_For_Type_With_StringTypeConverter()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.True(converter.CanConvert(typeof(ConvertibleThing)));
    }

    // Types with no string TypeConverter must be rejected so WebHttpBehavior
    // can fail validation early rather than at dispatch time.
    [WcfFact]
    public static void CanConvert_Returns_False_For_Unsupported_Types()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.False(converter.CanConvert(typeof(UnsupportedThing)));
        Assert.False(converter.CanConvert(typeof(List<string>)));
        Assert.False(converter.CanConvert(typeof(int[])));
    }

    // Nullable<T> is deliberately NOT supported (matches .NET Framework):
    // Type.GetTypeCode(typeof(int?)) is Object and Nullable<T> carries no
    // TypeConverterAttribute.
    [WcfFact]
    public static void CanConvert_Returns_False_For_Nullable_Primitives()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.False(converter.CanConvert(typeof(int?)));
        Assert.False(converter.CanConvert(typeof(DateTime?)));
        Assert.False(converter.CanConvert(typeof(Guid?)));
    }

    [WcfFact]
    public static void Primitive_Values_RoundTrip()
    {
        QueryStringConverter converter = new QueryStringConverter();

        AssertRoundTrips(converter, (byte)200);
        AssertRoundTrips(converter, (sbyte)-100);
        AssertRoundTrips(converter, (short)-30000);
        AssertRoundTrips(converter, 123456);
        AssertRoundTrips(converter, -9007199254740993L);
        AssertRoundTrips(converter, (ushort)65535);
        AssertRoundTrips(converter, 4294967295u);
        AssertRoundTrips(converter, 18446744073709551615ul);
        AssertRoundTrips(converter, 1.5f);
        AssertRoundTrips(converter, 1.7976931348623157E+308d);
        AssertRoundTrips(converter, true);
        AssertRoundTrips(converter, false);
        AssertRoundTrips(converter, 'x');
        AssertRoundTrips(converter, 79228162514264337593543950335m);
        AssertRoundTrips(converter, "hello world");
        AssertRoundTrips(converter, Guid.Parse("6F9619FF-8B86-D011-B42D-00C04FC964FF"));
        AssertRoundTrips(converter, TimeSpan.FromMinutes(90));
        AssertRoundTrips(converter, new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc));
        AssertRoundTrips(converter, new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(2)));
    }

    // XmlConvert is used for the numeric/boolean/date conversions, so the
    // output must be culture invariant. Run the whole matrix under a culture
    // that uses ',' as the decimal separator to prove it.
    [WcfFact]
    public static void Conversions_Are_Culture_Invariant()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            QueryStringConverter converter = new QueryStringConverter();

            Assert.Equal("1.5", converter.ConvertValueToString(1.5d, typeof(double)));
            Assert.Equal("1.5", converter.ConvertValueToString(1.5f, typeof(float)));
            Assert.Equal("1.5", converter.ConvertValueToString(1.5m, typeof(decimal)));
            Assert.Equal("true", converter.ConvertValueToString(true, typeof(bool)));
            Assert.Equal(1.5d, converter.ConvertStringToValue("1.5", typeof(double)));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [WcfFact]
    public static void Bool_Uses_Xml_Lowercase_Form()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.Equal("true", converter.ConvertValueToString(true, typeof(bool)));
        Assert.Equal("false", converter.ConvertValueToString(false, typeof(bool)));

        // Parsing accepts the .NET forms as well as the XML ones.
        Assert.Equal(true, converter.ConvertStringToValue("true", typeof(bool)));
        Assert.Equal(true, converter.ConvertStringToValue("True", typeof(bool)));
        Assert.Equal(false, converter.ConvertStringToValue("False", typeof(bool)));
    }

    [WcfFact]
    public static void ByteArray_Uses_Base64()
    {
        QueryStringConverter converter = new QueryStringConverter();
        byte[] bytes = { 0x00, 0x01, 0xFE, 0xFF };

        string text = converter.ConvertValueToString(bytes, typeof(byte[]));
        Assert.Equal(Convert.ToBase64String(bytes), text);
        Assert.Equal(bytes, (byte[])converter.ConvertStringToValue(text, typeof(byte[])));
    }

    [WcfFact]
    public static void ByteArray_Null_And_Empty()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Null(converter.ConvertValueToString(null, typeof(byte[])));
        Assert.Equal(Array.Empty<byte>(), (byte[])converter.ConvertStringToValue(null, typeof(byte[])));
        Assert.Equal(Array.Empty<byte>(), (byte[])converter.ConvertStringToValue(string.Empty, typeof(byte[])));
    }

    [WcfFact]
    public static void Uri_RoundTrips_Absolute_And_Relative()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Uri absolute = new Uri("http://contoso.example/a/b?c=d");
        Assert.Equal(absolute.OriginalString, converter.ConvertValueToString(absolute, typeof(Uri)));
        Assert.Equal(absolute, converter.ConvertStringToValue(absolute.OriginalString, typeof(Uri)));

        Uri relative = new Uri("a/b", UriKind.Relative);
        Assert.Equal("a/b", converter.ConvertValueToString(relative, typeof(Uri)));
        Assert.Equal(relative, converter.ConvertStringToValue("a/b", typeof(Uri)));
    }

    [WcfFact]
    public static void Uri_Null_And_Empty()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Null(converter.ConvertValueToString(null, typeof(Uri)));
        Assert.Null(converter.ConvertStringToValue(null, typeof(Uri)));
        Assert.Null(converter.ConvertStringToValue(string.Empty, typeof(Uri)));
    }

    [WcfFact]
    public static void Object_Passes_Strings_Through()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Equal("abc", converter.ConvertStringToValue("abc", typeof(object)));
        Assert.Equal("abc", converter.ConvertValueToString("abc", typeof(object)));
        Assert.Null(converter.ConvertStringToValue(null, typeof(object)));
        Assert.Null(converter.ConvertValueToString(null, typeof(object)));
    }

    [WcfFact]
    public static void String_Passes_Through_Unchanged()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Equal(string.Empty, converter.ConvertStringToValue(string.Empty, typeof(string)));
        Assert.Null(converter.ConvertStringToValue(null, typeof(string)));
        Assert.Null(converter.ConvertValueToString(null, typeof(string)));
        Assert.Equal("a b", converter.ConvertStringToValue("a b", typeof(string)));
    }

    [WcfFact]
    public static void Enum_Converts_By_Name_And_Is_Case_Insensitive()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Equal("Second", converter.ConvertValueToString(SampleEnum.Second, typeof(SampleEnum)));
        Assert.Equal(SampleEnum.Second, converter.ConvertStringToValue("Second", typeof(SampleEnum)));
        Assert.Equal(SampleEnum.Second, converter.ConvertStringToValue("second", typeof(SampleEnum)));
        Assert.Equal(SampleEnum.Second, converter.ConvertStringToValue("SECOND", typeof(SampleEnum)));
    }

    [WcfFact]
    public static void Enum_Converts_By_Numeric_Value()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.Equal(SampleEnum.Second, converter.ConvertStringToValue("1", typeof(SampleEnum)));
    }

    [WcfFact]
    public static void Non_Int32_Enum_Is_Reported_Supported_But_Does_Not_RoundTrip()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.True(converter.CanConvert(typeof(SampleByteEnum)));
        Assert.Equal("1", converter.ConvertValueToString(SampleByteEnum.One, typeof(SampleByteEnum)));
        Assert.IsType<byte>(converter.ConvertStringToValue("1", typeof(SampleByteEnum)));
    }

    [WcfFact]
    public static void Enum_Unknown_Name_Throws()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.ThrowsAny<ArgumentException>(() => converter.ConvertStringToValue("NoSuchMember", typeof(SampleEnum)));
    }

    [WcfFact]
    public static void FlagsEnum_Converts_Combined_Values()
    {
        QueryStringConverter converter = new QueryStringConverter();
        SampleFlagsEnum value = SampleFlagsEnum.A | SampleFlagsEnum.C;

        string text = converter.ConvertValueToString(value, typeof(SampleFlagsEnum));
        Assert.Equal(value, converter.ConvertStringToValue(text, typeof(SampleFlagsEnum)));
    }

    [WcfFact]
    public static void TimeSpan_Accepts_Clock_And_Xsd_Duration_Forms()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Equal(TimeSpan.FromHours(1), converter.ConvertStringToValue("01:00:00", typeof(TimeSpan)));
        Assert.Equal(TimeSpan.FromHours(1), converter.ConvertStringToValue("PT1H", typeof(TimeSpan)));

        // The XSD duration form is what gets written back out.
        Assert.Equal("PT1H", converter.ConvertValueToString(TimeSpan.FromHours(1), typeof(TimeSpan)));
    }

    [WcfFact]
    public static void DateTime_Preserves_Kind()
    {
        QueryStringConverter converter = new QueryStringConverter();

        DateTime utc = new DateTime(2020, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        DateTime parsedUtc = (DateTime)converter.ConvertStringToValue(
            converter.ConvertValueToString(utc, typeof(DateTime)), typeof(DateTime));
        Assert.Equal(utc, parsedUtc);
        Assert.Equal(DateTimeKind.Utc, parsedUtc.Kind);

        DateTime unspecified = new DateTime(2020, 6, 1, 12, 0, 0, DateTimeKind.Unspecified);
        DateTime parsedUnspecified = (DateTime)converter.ConvertStringToValue(
            converter.ConvertValueToString(unspecified, typeof(DateTime)), typeof(DateTime));
        Assert.Equal(unspecified, parsedUnspecified);
        Assert.Equal(DateTimeKind.Unspecified, parsedUnspecified.Kind);
    }

    // A null string maps to default(T) for value types rather than throwing,
    // so an absent query parameter binds to 0 / false / DateTime.MinValue.
    [WcfFact]
    public static void ConvertStringToValue_Null_Yields_Default_For_ValueTypes()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Equal((byte)0, converter.ConvertStringToValue(null, typeof(byte)));
        Assert.Equal(0, converter.ConvertStringToValue(null, typeof(int)));
        Assert.Equal(0L, converter.ConvertStringToValue(null, typeof(long)));
        Assert.Equal(0d, converter.ConvertStringToValue(null, typeof(double)));
        Assert.Equal(0m, converter.ConvertStringToValue(null, typeof(decimal)));
        Assert.Equal(false, converter.ConvertStringToValue(null, typeof(bool)));
        Assert.Equal(default(char), converter.ConvertStringToValue(null, typeof(char)));
        Assert.Equal(default(Guid), converter.ConvertStringToValue(null, typeof(Guid)));
        Assert.Equal(default(TimeSpan), converter.ConvertStringToValue(null, typeof(TimeSpan)));
        Assert.Equal(default(DateTime), converter.ConvertStringToValue(null, typeof(DateTime)));
        Assert.Equal(default(DateTimeOffset), converter.ConvertStringToValue(null, typeof(DateTimeOffset)));
    }

    [WcfFact]
    public static void ConvertValueToString_Null_ValueType_Throws()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Throws<ArgumentNullException>(() => converter.ConvertValueToString(null, typeof(int)));
        Assert.Throws<ArgumentNullException>(() => converter.ConvertValueToString(null, typeof(Guid)));
        Assert.Throws<ArgumentNullException>(() => converter.ConvertValueToString(null, typeof(DateTime)));
    }

    [WcfFact]
    public static void Null_ParameterType_Throws()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Throws<ArgumentNullException>(() => converter.ConvertStringToValue("1", null));
        Assert.Throws<ArgumentNullException>(() => converter.ConvertValueToString(1, null));
    }

    [WcfFact]
    public static void Malformed_Input_Throws()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.ThrowsAny<FormatException>(() => converter.ConvertStringToValue("not-a-number", typeof(int)));
        Assert.ThrowsAny<FormatException>(() => converter.ConvertStringToValue(string.Empty, typeof(int)));
        Assert.ThrowsAny<FormatException>(() => converter.ConvertStringToValue("not-a-guid", typeof(Guid)));
        Assert.ThrowsAny<FormatException>(() => converter.ConvertStringToValue("too long", typeof(char)));
        Assert.ThrowsAny<FormatException>(() => converter.ConvertStringToValue("not base64!!", typeof(byte[])));
    }

    [WcfFact]
    public static void Overflow_Throws()
    {
        QueryStringConverter converter = new QueryStringConverter();
        Assert.ThrowsAny<OverflowException>(() => converter.ConvertStringToValue("99999999999999999999", typeof(int)));
    }

    [WcfFact]
    public static void Unsupported_Type_Throws_NotSupportedException()
    {
        QueryStringConverter converter = new QueryStringConverter();

        Assert.Throws<NotSupportedException>(() => converter.ConvertStringToValue("x", typeof(UnsupportedThing)));
        Assert.Throws<NotSupportedException>(() => converter.ConvertValueToString(new UnsupportedThing(), typeof(UnsupportedThing)));
    }

    [WcfFact]
    public static void Type_With_TypeConverter_RoundTrips()
    {
        QueryStringConverter converter = new QueryStringConverter();
        ConvertibleThing thing = new ConvertibleThing { Value = "abc" };

        string text = converter.ConvertValueToString(thing, typeof(ConvertibleThing));
        Assert.Equal("abc", text);

        ConvertibleThing parsed = Assert.IsType<ConvertibleThing>(
            converter.ConvertStringToValue("abc", typeof(ConvertibleThing)));
        Assert.Equal("abc", parsed.Value);
    }

    // The converter caches TypeConverter instances per type; repeated calls
    // must keep working (and not throw from a duplicate cache insert).
    [WcfFact]
    public static void TypeConverter_Lookup_Is_Cached_And_Repeatable()
    {
        QueryStringConverter converter = new QueryStringConverter();

        for (int i = 0; i < 3; i++)
        {
            Assert.True(converter.CanConvert(typeof(ConvertibleThing)));
            Assert.Equal("abc", converter.ConvertValueToString(new ConvertibleThing { Value = "abc" }, typeof(ConvertibleThing)));
        }
    }

    [WcfFact]
    public static void TypeConverter_Lookup_Is_Thread_Safe()
    {
        const int WorkerCount = 8;
        QueryStringConverter converter = new QueryStringConverter();
        ConcurrentConvertibleThingConverter.Prepare(WorkerCount);
        Task<bool>[] tasks = new Task<bool>[WorkerCount];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => converter.CanConvert(typeof(ConcurrentConvertibleThing)));
        }

        bool allConstructorsEntered;
        try
        {
            allConstructorsEntered = ConcurrentConvertibleThingConverter.WaitForConstructors(TimeSpan.FromSeconds(10));
        }
        finally
        {
            ConcurrentConvertibleThingConverter.ReleaseConstructors();
        }

        Assert.True(allConstructorsEntered, "The concurrent TypeConverter constructors did not start within the timeout.");
        Task.WaitAll(tasks);
        foreach (Task<bool> task in tasks)
        {
            Assert.True(task.Result);
        }
    }

    // Derived converters are the documented extensibility point used by
    // WebHttpBehavior.GetQueryStringConverter.
    [WcfFact]
    public static void Derived_Converter_Can_Extend_Supported_Types()
    {
        QueryStringConverter converter = new UnsupportedThingConverter();

        Assert.True(converter.CanConvert(typeof(UnsupportedThing)));
        Assert.Equal("custom", converter.ConvertValueToString(new UnsupportedThing(), typeof(UnsupportedThing)));
        Assert.IsType<UnsupportedThing>(converter.ConvertStringToValue("custom", typeof(UnsupportedThing)));

        // Base behaviour is still intact for the built-in types.
        Assert.True(converter.CanConvert(typeof(int)));
        Assert.Equal("5", converter.ConvertValueToString(5, typeof(int)));
    }

    private static void AssertRoundTrips<T>(QueryStringConverter converter, T value)
    {
        string text = converter.ConvertValueToString(value, typeof(T));
        object parsed = converter.ConvertStringToValue(text, typeof(T));
        Assert.Equal(value, Assert.IsType<T>(parsed));
    }

    public enum SampleEnum
    {
        First = 0,
        Second = 1,
        Third = 2
    }

    public enum SampleByteEnum : byte
    {
        Zero = 0,
        One = 1
    }

    [Flags]
    public enum SampleFlagsEnum
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4
    }

    public sealed class UnsupportedThing
    {
    }

    [TypeConverter(typeof(ConvertibleThingConverter))]
    public sealed class ConvertibleThing
    {
        public string Value { get; set; }
    }

    public sealed class ConvertibleThingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => value is string text ? new ConvertibleThing { Value = text } : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => destinationType == typeof(string) ? ((ConvertibleThing)value).Value : base.ConvertTo(context, culture, value, destinationType);
    }

    [TypeConverter(typeof(ConcurrentConvertibleThingConverter))]
    public sealed class ConcurrentConvertibleThing
    {
    }

    public sealed class ConcurrentConvertibleThingConverter : TypeConverter
    {
        private static CountdownEvent s_constructorsEntered;
        private static ManualResetEventSlim s_releaseConstructors;

        public ConcurrentConvertibleThingConverter()
        {
            s_constructorsEntered.Signal();
            s_releaseConstructors.Wait(TimeSpan.FromSeconds(10));
        }

        public static void Prepare(int constructorCount)
        {
            s_constructorsEntered = new CountdownEvent(constructorCount);
            s_releaseConstructors = new ManualResetEventSlim();
        }

        public static bool WaitForConstructors(TimeSpan timeout) => s_constructorsEntered.Wait(timeout);

        public static void ReleaseConstructors() => s_releaseConstructors.Set();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    private sealed class UnsupportedThingConverter : QueryStringConverter
    {
        public override bool CanConvert(Type type)
            => type == typeof(UnsupportedThing) || base.CanConvert(type);

        public override object ConvertStringToValue(string parameter, Type parameterType)
            => parameterType == typeof(UnsupportedThing) ? new UnsupportedThing() : base.ConvertStringToValue(parameter, parameterType);

        public override string ConvertValueToString(object parameter, Type parameterType)
            => parameterType == typeof(UnsupportedThing) ? "custom" : base.ConvertValueToString(parameter, parameterType);
    }
}
