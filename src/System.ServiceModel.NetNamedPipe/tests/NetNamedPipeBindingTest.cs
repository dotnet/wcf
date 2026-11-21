// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public class NetNamedPipeBindingTest : ConditionalWcfTest
{
    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void TransactionFlow_Defaults()
    {
        var binding = new NetNamedPipeBinding();

        Assert.False(binding.TransactionFlow);
        Assert.Same(TransactionProtocol.OleTransactions, binding.TransactionProtocol);
        var transactionFlow = Assert.IsType<TransactionFlowBindingElement>(binding.CreateBindingElements()[0]);
        var context = new BindingContext(new CustomBinding(), new BindingParameterCollection());
        Assert.Null(transactionFlow.GetProperty<ChannelProtectionRequirements>(context));
    }

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [SupportedOSPlatform("windows")]
    public static void TransactionFlow_ConfiguresChannelStack()
    {
        var binding = new NetNamedPipeBinding
        {
            TransactionFlow = true,
            TransactionProtocol = TransactionProtocol.WSAtomicTransaction11
        };

        var elements = binding.CreateBindingElements();
        var transactionFlow = Assert.IsType<TransactionFlowBindingElement>(elements[0]);
        Assert.Same(TransactionProtocol.WSAtomicTransaction11, transactionFlow.TransactionProtocol);
        var context = new BindingContext(new CustomBinding(), new BindingParameterCollection());
        Assert.NotNull(transactionFlow.GetProperty<ChannelProtectionRequirements>(context));

        var factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
        try
        {
            factory.Open();
            factory.Close();
        }
        finally
        {
            factory.Abort();
        }

        transactionFlow.TransactionProtocol = TransactionProtocol.OleTransactions;
        Assert.Same(TransactionProtocol.WSAtomicTransaction11, binding.TransactionProtocol);

        binding.TransactionFlow = false;
        var disabledTransactionFlow = binding.CreateBindingElements().Find<TransactionFlowBindingElement>();
        Assert.Null(disabledTransactionFlow.GetProperty<ChannelProtectionRequirements>(context));
    }

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
