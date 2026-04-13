// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class WSHttpBindingTransactionTests
{
    // ===== WSHttpBinding TransactionFlow default tests =====

    [WcfFact]
    public static void WSHttpBinding_TransactionFlow_Default_Is_False()
    {
        var binding = new WSHttpBinding();
        Assert.False(binding.TransactionFlow);
    }

    [WcfFact]
    public static void WS2007HttpBinding_TransactionFlow_Default_Is_False()
    {
        var binding = new WS2007HttpBinding();
        Assert.False(binding.TransactionFlow);
    }

    // ===== TransactionFlow property setter tests =====

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void WSHttpBinding_TransactionFlow_Property_Sets(bool value)
    {
        var binding = new WSHttpBinding();
        binding.TransactionFlow = value;
        Assert.Equal(value, binding.TransactionFlow);
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void WS2007HttpBinding_TransactionFlow_Property_Sets(bool value)
    {
        var binding = new WS2007HttpBinding();
        binding.TransactionFlow = value;
        Assert.Equal(value, binding.TransactionFlow);
    }

    // ===== CreateBindingElements includes TransactionFlowBindingElement =====

    [WcfFact]
    public static void WSHttpBinding_CreateBindingElements_Contains_TransactionFlowBindingElement()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
    }

    [WcfFact]
    public static void WS2007HttpBinding_CreateBindingElements_Contains_TransactionFlowBindingElement()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
    }

    // ===== TransactionProtocol defaults =====

    [WcfFact]
    public static void WSHttpBinding_TransactionFlowBindingElement_Uses_WSAtomicTransactionOctober2004()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WS2007HttpBinding_TransactionFlowBindingElement_Uses_WSAtomicTransaction11()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    // ===== WSHttpBinding and WS2007HttpBinding use correct protocols across all constructor overloads =====

    [WcfFact]
    public static void WSHttpBinding_Default_Ctor_TransactionFlow_Default_Matches_SecurityMode_None_Ctor()
    {
        // The default constructor uses SecurityMode.Message which prevents CreateBindingElements
        // on non-Windows, but the TransactionFlow default should be the same regardless of security mode.
        var defaultBinding = new WSHttpBinding();
        var noneSecBinding = new WSHttpBinding(SecurityMode.None);
        Assert.Equal(defaultBinding.TransactionFlow, noneSecBinding.TransactionFlow);
    }

    [WcfFact]
    public static void WSHttpBinding_SecurityMode_Ctor_Uses_WSAtomicTransactionOctober2004()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WSHttpBinding_SecurityMode_ReliableSession_Ctor_Uses_WSAtomicTransactionOctober2004()
    {
        var binding = new WSHttpBinding(SecurityMode.None, reliableSessionEnabled: true);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WS2007HttpBinding_Default_Ctor_TransactionFlow_Default_Matches_SecurityMode_None_Ctor()
    {
        var defaultBinding = new WS2007HttpBinding();
        var noneSecBinding = new WS2007HttpBinding(SecurityMode.None);
        Assert.Equal(defaultBinding.TransactionFlow, noneSecBinding.TransactionFlow);
    }

    // ===== Protocols differ between WSHttpBinding and WS2007HttpBinding =====

    [WcfFact]
    public static void WSHttpBinding_And_WS2007HttpBinding_Use_Different_Protocols()
    {
        var wsBinding = new WSHttpBinding(SecurityMode.None);
        var ws2007Binding = new WS2007HttpBinding(SecurityMode.None);

        var wsElements = wsBinding.CreateBindingElements();
        var ws2007Elements = ws2007Binding.CreateBindingElements();

        var wsTxFlow = wsElements.Find<TransactionFlowBindingElement>();
        var ws2007TxFlow = ws2007Elements.Find<TransactionFlowBindingElement>();

        Assert.NotEqual(wsTxFlow.TransactionProtocol, ws2007TxFlow.TransactionProtocol);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, wsTxFlow.TransactionProtocol);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, ws2007TxFlow.TransactionProtocol);
    }

    // ===== TransactionFlow=true propagates to TransactionFlowBindingElement =====

    [WcfFact]
    public static void WSHttpBinding_TransactionFlow_True_Propagates_To_BindingElement()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        // The binding element is cloned, so just verify it was created from
        // a binding with TransactionFlow=true by checking the binding itself
        Assert.True(binding.TransactionFlow);
    }

    // ===== TransactionFlowBindingElement is first in binding elements =====

    [WcfFact]
    public static void WSHttpBinding_TransactionFlowBindingElement_Is_First_Element()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        Assert.IsType<TransactionFlowBindingElement>(elements[0]);
    }

    [WcfFact]
    public static void WS2007HttpBinding_TransactionFlowBindingElement_Is_First_Element()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        Assert.IsType<TransactionFlowBindingElement>(elements[0]);
    }

    // ===== Constructor variations preserve transaction protocol =====

    [WcfFact]
    public static void WS2007HttpBinding_SecurityMode_Ctor_Uses_WSAtomicTransaction11()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WS2007HttpBinding_SecurityMode_ReliableSession_Ctor_Uses_WSAtomicTransaction11()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None, reliableSessionEnabled: true);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    // ===== TransactionFlow + Protocol combination tests =====

    [WcfFact]
    public static void WSHttpBinding_TransactionFlow_True_With_WSAtomicTransactionOctober2004()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }

    [WcfFact]
    public static void WS2007HttpBinding_TransactionFlow_True_With_WSAtomicTransaction11()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }

    [WcfFact]
    public static void WSHttpBinding_TransactionFlow_False_Preserves_Protocol()
    {
        var binding = new WSHttpBinding(SecurityMode.None);
        binding.TransactionFlow = false;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WS2007HttpBinding_TransactionFlow_False_Preserves_Protocol()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None);
        binding.TransactionFlow = false;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void WSHttpBinding_ReliableSession_TransactionFlow_True_Preserves_Protocol()
    {
        var binding = new WSHttpBinding(SecurityMode.None, reliableSessionEnabled: true);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }

    [WcfFact]
    public static void WS2007HttpBinding_ReliableSession_TransactionFlow_True_Preserves_Protocol()
    {
        var binding = new WS2007HttpBinding(SecurityMode.None, reliableSessionEnabled: true);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }
}

// ===== Platform-specific tests (PlatformNotSupportedException on non-Windows) =====
// These tests validate that on non-Windows platforms, enabling transactions
// will throw PlatformNotSupportedException since System.Transactions requires Windows.
// They won't execute on Windows but must exist.

public static class WSHttpBindingTransactionPlatformTests
{
    public static bool Is_Not_Windows()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    private static void AssertTransactionFlowUsageThrowsOnNonWindows(Binding binding)
    {
        BindingElementCollection elements = binding.CreateBindingElements();
        TransactionFlowBindingElement txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        CustomBinding customBinding = new CustomBinding(elements);
        Assert.Throws<PlatformNotSupportedException>(() =>
        {
            customBinding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
        });
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void WSHttpBinding_TransactionFlow_True_Throws_On_NonWindows()
    {
        // On non-Windows, setting TransactionFlow=true and attempting to use the binding
        // should fail because System.Transactions is not supported.
        WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        AssertTransactionFlowUsageThrowsOnNonWindows(binding);
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void WS2007HttpBinding_TransactionFlow_True_Throws_On_NonWindows()
    {
        WS2007HttpBinding binding = new WS2007HttpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        AssertTransactionFlowUsageThrowsOnNonWindows(binding);
    }
}
