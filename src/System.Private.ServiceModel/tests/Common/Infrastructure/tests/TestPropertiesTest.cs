// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using Xunit;

public static class TestPropertiesTest
{
    [Fact]
    public static void TestProperties_Property_Names_Are_Initialized()
    {
        // Test property names are auto-generated.
        // This test will fail to compile if these names are not generated.
        // It is not necessary to test all the known properties, just to
        // test that property name code generation occurred.
        Assert.NotNull(TestProperties.ServiceUri_PropertyName);
    }

    [Fact]
    public static void TestProperties_PropertyNames_Property_Returns_List()
    {
        IEnumerable<string> propertyNames = TestProperties.PropertyNames;
        Assert.NotNull(propertyNames);
        Assert.True(propertyNames.Any());
    }

    [Fact]
    public static void TestProperties_All_Properties_Have_Values()
    {
        IEnumerable<string> propertyNames = TestProperties.PropertyNames;
        foreach (var name in propertyNames)
        {
            string value = TestProperties.GetProperty(name);
            Assert.True(value != null, String.Format("Property '{0}' should not be null.", name));
        }
    }

    [Fact]
    public static void TestProperties_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TestProperties.GetProperty(null));
    }

    [Fact]
    public static void TestProperties_Throw_KeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => TestProperties.GetProperty("NotAProperty"));
    }
}
