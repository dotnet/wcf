// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace WcfService
{
    public static class ScenarioTestHelpers
    {
        public static ComplexCompositeType GetInitializedComplexCompositeType()
        {
            ComplexCompositeType compositeObject = new ComplexCompositeType()
            {
                BoolValue = true,
                ByteArrayValue = new byte[] { 0x60, 0x61, 0x62 },
                CharArrayValue = new char[] { 'a', 'b', 'c' },
                CharValue = 'a',
                DateTimeValue = new DateTime(2000, 1, 1),
                DayOfWeekValue = DayOfWeek.Sunday,
                DoubleValue = 3.14159265,
                FloatValue = 2.71828183f,
                GuidValue = new Guid("EFEA21A0-F59A-4F43-B5D3-B2C667CA6FB6"),
                IntValue = int.MinValue,
                LongerStringValue = GenerateStringValue(2048),
                LongValue = long.MaxValue,
                SbyteValue = (sbyte)'a',
                ShortValue = short.MaxValue,
                StringValue = "the quick brown fox jumps over the lazy dog",
                TimeSpanValue = TimeSpan.MinValue,
                UintValue = uint.MaxValue,
                UlongValue = ulong.MaxValue,
                UshortValue = ushort.MaxValue
            };

            return compositeObject;
        }

        public static string GenerateStringValue(int length)
        {
            // There's no great reason why we use this set of characters - we just want to be able to generate a longish string
            uint firstCharacter = 0x41; // A

            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                builder.Append((char)(firstCharacter + i % 25));
            }

            return builder.ToString();
        }
    }
}
