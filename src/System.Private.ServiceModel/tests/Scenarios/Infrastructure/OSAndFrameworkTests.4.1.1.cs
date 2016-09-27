// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Infrastructure.Common;
using Xunit;

public class OSAndFrameworkTests
{
    [WcfFact]
    [OuterLoop]
    public static void FrameworkID_Was_Detected()
    {
        Assert.True(FrameworkHelper.Current != FrameworkID.None,
                    String.Format("FrameworkID was not properly detected from: RuntimeInformation.FrameworkDescription = \'{0}\"",
                                   RuntimeInformation.FrameworkDescription));
    }

    [WcfFact]
    [OuterLoop]
    public static void OSID_Was_Detected()
    {
        Assert.True(OSHelper.Current != OSID.None,
                    String.Format("OSID was not properly detected from:{0}  TestProperties[TestNugetRuntimeId] = \"{1}\"{0}  RuntimeInformation.OSDescription = \'{2}\"",
                                   Environment.NewLine,
                                   TestProperties.GetProperty(TestProperties.TestNugetRuntimeId_PropertyName),
                                   RuntimeInformation.OSDescription));
    }

    [WcfFact]
    [OuterLoop]
    public static void FrameworkID_Name_Formats_Correctly()
    {
        FrameworkID id = FrameworkID.NetCore | FrameworkID.NetNative;
        string formatted = id.Name();

        Assert.True(formatted.Contains("NetCore") && formatted.Contains("NetNative"),
                    String.Format("FrameworkID.Name should have contained NetCore and NetNative, but actual was \"{0}\"", formatted));
    }

    [WcfFact]
    [OuterLoop]
    public static void OSID_Name_Formats_Correctly()
    {
        OSID id = OSID.Windows_7 | OSID.Ubuntu_14_04;
        string formatted = id.Name();

        Assert.True(formatted.Contains("Windows_7") && formatted.Contains("Ubuntu_14_04"),
                    String.Format("FrameworkID.Name should have contained Windows_7 and Ubuntu_14_04, but actual was \"{0}\"", formatted));
    }
}

