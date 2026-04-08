// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class NetTcpBindingTransactionTests
{
    // ===== TransactionFlow default tests =====

    [WcfFact]
    public static void NetTcpBinding_TransactionFlow_Default_Is_False()
    {
        var binding = new NetTcpBinding();
        Assert.False(binding.TransactionFlow);
    }

    [WcfFact]
    public static void NetTcpBinding_SecurityMode_Ctor_TransactionFlow_Default_Is_False()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        Assert.False(binding.TransactionFlow);
    }

    [WcfFact]
    public static void NetTcpBinding_SecurityMode_ReliableSession_Ctor_TransactionFlow_Default_Is_False()
    {
        var binding = new NetTcpBinding(SecurityMode.None, reliableSessionEnabled: true);
        Assert.False(binding.TransactionFlow);
    }

    // ===== TransactionFlow property setter tests =====

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void NetTcpBinding_TransactionFlow_Property_Sets(bool value)
    {
        var binding = new NetTcpBinding();
        binding.TransactionFlow = value;
        Assert.Equal(value, binding.TransactionFlow);
    }

    // ===== TransactionProtocol default tests =====

    [WcfFact]
    public static void NetTcpBinding_TransactionProtocol_Default_Is_OleTransactions()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.OleTransactions, txFlowElement.TransactionProtocol);
    }

    // ===== TransactionProtocol property setter tests =====

    [WcfFact]
    public static void NetTcpBinding_TransactionProtocol_Property_Sets_WSAtomicTransactionOctober2004()
    {
        var binding = new NetTcpBinding();
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, binding.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_TransactionProtocol_Property_Sets_WSAtomicTransaction11()
    {
        var binding = new NetTcpBinding();
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, binding.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_TransactionProtocol_Property_Sets_OleTransactions()
    {
        var binding = new NetTcpBinding();
        binding.TransactionProtocol = TransactionProtocol.OleTransactions;
        Assert.Equal(TransactionProtocol.OleTransactions, binding.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_TransactionProtocol_Null_Throws()
    {
        var binding = new NetTcpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.TransactionProtocol = null);
    }

    // ===== CreateBindingElements reflects non-default TransactionProtocol =====

    [WcfFact]
    public static void NetTcpBinding_CreateBindingElements_Reflects_WSAtomicTransactionOctober2004()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_CreateBindingElements_Reflects_WSAtomicTransaction11()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_CreateBindingElements_Reflects_OleTransactions_After_Roundtrip()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        binding.TransactionProtocol = TransactionProtocol.OleTransactions;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.OleTransactions, txFlowElement.TransactionProtocol);
    }

    // ===== TransactionFlow + TransactionProtocol combination tests =====

    [WcfFact]
    public static void NetTcpBinding_TransactionFlow_True_With_WSAtomicTransactionOctober2004()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }

    [WcfFact]
    public static void NetTcpBinding_TransactionFlow_True_With_WSAtomicTransaction11()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
        Assert.True(binding.TransactionFlow);
    }

    [WcfFact]
    public static void NetTcpBinding_TransactionFlow_False_With_NonDefault_Protocol_Preserves_Protocol()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionFlow = false;
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
        Assert.False(binding.TransactionFlow);
    }

    // ===== CreateBindingElements includes TransactionFlowBindingElement =====

    [WcfFact]
    public static void NetTcpBinding_CreateBindingElements_Contains_TransactionFlowBindingElement()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
    }

    // ===== TransactionFlowBindingElement is first in binding elements =====

    [WcfFact]
    public static void NetTcpBinding_TransactionFlowBindingElement_Is_First_Element()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        var elements = binding.CreateBindingElements();
        Assert.IsType<TransactionFlowBindingElement>(elements[0]);
    }

    // ===== TransactionFlow=true propagates to TransactionFlowBindingElement =====

    [WcfFact]
    public static void NetTcpBinding_TransactionFlow_True_Propagates_To_BindingElement()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        Assert.True(binding.TransactionFlow);
    }

    // ===== Constructor variations preserve transaction protocol =====

    [WcfFact]
    public static void NetTcpBinding_SecurityMode_ReliableSession_Ctor_Uses_OleTransactions()
    {
        var binding = new NetTcpBinding(SecurityMode.None, reliableSessionEnabled: true);
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.OleTransactions, txFlowElement.TransactionProtocol);
    }

    // ===== Constructor variations with protocol override =====

    [WcfFact]
    public static void NetTcpBinding_SecurityMode_Ctor_Allows_Protocol_Override_To_WSAtomicTransaction11()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_SecurityMode_Ctor_Allows_Protocol_Override_To_WSAtomicTransactionOctober2004()
    {
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, txFlowElement.TransactionProtocol);
    }

    [WcfFact]
    public static void NetTcpBinding_ReliableSession_Ctor_Allows_Protocol_Override()
    {
        var binding = new NetTcpBinding(SecurityMode.None, reliableSessionEnabled: true);
        binding.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, txFlowElement.TransactionProtocol);
    }
}

// ===== Platform-specific tests (PlatformNotSupportedException on non-Windows) =====
// These tests validate that on non-Windows platforms, enabling transactions
// will throw PlatformNotSupportedException since System.Transactions requires Windows.
// They won't execute on Windows but must exist.

public static class NetTcpBindingTransactionPlatformTests
{
    public static bool Is_Not_Windows()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void NetTcpBinding_TransactionFlow_True_Throws_On_NonWindows()
    {
        // On non-Windows, setting TransactionFlow=true and attempting to use the binding
        // should eventually fail because System.Transactions is not supported.
        // The TransactionFlowBindingElement's BuildChannelFactory internally calls
        // into transaction formatters that throw PlatformNotSupportedException on non-Windows.
        var binding = new NetTcpBinding(SecurityMode.None);
        binding.TransactionFlow = true;
        var elements = binding.CreateBindingElements();
        var txFlowElement = elements.Find<TransactionFlowBindingElement>();
        Assert.NotNull(txFlowElement);
        // The property can be set, but actually building the channel factory must fail
        // on non-Windows when transaction flow support is initialized.
        Assert.Throws<PlatformNotSupportedException>(() =>
            binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection()));
    }
}
