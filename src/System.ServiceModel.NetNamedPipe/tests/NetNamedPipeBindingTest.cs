// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class NetNamedPipeBindingTest
{
    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void PipeSettings_DefaultValues()
    {
        var bindingElement = new NamedPipeTransportBindingElement();
        ApplicationContainerSettings settings = bindingElement.PipeSettings.ApplicationContainerSettings;

        Assert.Null(settings.PackageFullName);
        Assert.Equal(ApplicationContainerSettings.CurrentSession, settings.SessionId);
    }

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void PipeSettings_InvalidSessionId_Throws()
    {
        var bindingElement = new NamedPipeTransportBindingElement();
        ApplicationContainerSettings settings = bindingElement.PipeSettings.ApplicationContainerSettings;

        Assert.ThrowsAny<ArgumentException>(() => settings.SessionId = ApplicationContainerSettings.CurrentSession - 1);
    }

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void Clone_CopiesPipeSettings()
    {
        var bindingElement = new NamedPipeTransportBindingElement();
        ApplicationContainerSettings settings = bindingElement.PipeSettings.ApplicationContainerSettings;
        settings.PackageFullName = "TestPackage_1.0.0.0_neutral__publisher";
        settings.SessionId = ApplicationContainerSettings.ServiceSession;

        var clone = (NamedPipeTransportBindingElement)bindingElement.Clone();
        ApplicationContainerSettings clonedSettings = clone.PipeSettings.ApplicationContainerSettings;

        Assert.NotSame(bindingElement.PipeSettings, clone.PipeSettings);
        Assert.NotSame(settings, clonedSettings);
        Assert.Equal(settings.PackageFullName, clonedSettings.PackageFullName);
        Assert.Equal(settings.SessionId, clonedSettings.SessionId);

        clonedSettings.PackageFullName = "DifferentPackage_1.0.0.0_neutral__publisher";
        Assert.NotEqual(settings.PackageFullName, clonedSettings.PackageFullName);
    }

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void GetProperty_ReturnsPipeSettings()
    {
        var bindingElement = new NamedPipeTransportBindingElement();
        var context = new BindingContext(new CustomBinding(), new BindingParameterCollection());

        NamedPipeSettings settings = bindingElement.GetProperty<NamedPipeSettings>(context);

        Assert.Same(bindingElement.PipeSettings, settings);
    }

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void AppContextSwitch_useSha1InPipeConnectionGetHashAlgorithm()
    {
        Type t = Assembly.GetAssembly(typeof(NamedPipeTransportBindingElement))
                            .GetType(typeof(NamedPipeTransportBindingElement).Namespace + ".PipeUri");
        MethodInfo m = t.GetMethod(
            "BuildSharedMemoryName",
            BindingFlags.Static | BindingFlags.Public,
            binder: null,
            types: new Type[] { typeof(string), typeof(string), typeof(bool) },
            modifiers: null
        );

        //swtich on
        AppContext.SetSwitch("Switch.System.ServiceModel.UseSha1InPipeConnectionGetHashAlgorithm", true);
        string result = (string)m.Invoke(t, new object[] { "hostname", new string('a', 128), true });
        Assert.Equal(45, result.Length);

        //switch off
        FieldInfo f = t.GetField("s_useSha1InPipeConnectionGetHashAlgorithm", BindingFlags.Static | BindingFlags.NonPublic);
        f.SetValue(t, false);
        result = (string)m.Invoke(t, new object[] { "hostname", new string('a', 128), true });
        Assert.Equal(61, result.Length);
    }
}
