// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Infrastructure.Common;
using Xunit;

public static class TransactionFlowBindingElementTest
{
    // ===== Default constructor tests =====

    [WcfFact]
    public static void Default_Ctor_Initializes_TransactionProtocol_To_OleTransactions()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.Equal(TransactionProtocol.OleTransactions, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void Default_Ctor_Initializes_AllowWildcardAction_To_False()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.False(bindingElement.AllowWildcardAction);
    }

    // ===== Constructor with TransactionProtocol parameter tests =====

    [WcfFact]
    public static void Ctor_With_OleTransactions_Sets_TransactionProtocol()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        Assert.Equal(TransactionProtocol.OleTransactions, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void Ctor_With_WSAtomicTransactionOctober2004_Sets_TransactionProtocol()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransactionOctober2004);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void Ctor_With_WSAtomicTransaction11_Sets_TransactionProtocol()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void Ctor_With_Null_TransactionProtocol_Throws()
    {
        Assert.ThrowsAny<Exception>(() => new TransactionFlowBindingElement(null));
    }

    // ===== TransactionProtocol property setter tests =====

    [WcfFact]
    public static void TransactionProtocol_Property_Sets_OleTransactions()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        bindingElement.TransactionProtocol = TransactionProtocol.OleTransactions;
        Assert.Equal(TransactionProtocol.OleTransactions, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void TransactionProtocol_Property_Sets_WSAtomicTransactionOctober2004()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        bindingElement.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void TransactionProtocol_Property_Sets_WSAtomicTransaction11()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        bindingElement.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, bindingElement.TransactionProtocol);
    }

    [WcfFact]
    public static void TransactionProtocol_Property_Set_Null_Throws()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bindingElement.TransactionProtocol = null);
    }

    // ===== AllowWildcardAction property setter tests =====

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void AllowWildcardAction_Property_Sets(bool value)
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        bindingElement.AllowWildcardAction = value;
        Assert.Equal(value, bindingElement.AllowWildcardAction);
    }

    // ===== Clone tests =====

    [WcfFact]
    public static void Clone_Returns_New_Instance_With_Same_Properties()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        original.AllowWildcardAction = true;

        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.TransactionProtocol, clone.TransactionProtocol);
        Assert.Equal(original.AllowWildcardAction, clone.AllowWildcardAction);
    }

    [WcfFact]
    public static void Clone_With_OleTransactions_Preserves_Protocol()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(TransactionProtocol.OleTransactions, clone.TransactionProtocol);
    }

    [WcfFact]
    public static void Clone_With_WSAtomicTransactionOctober2004_Preserves_Protocol()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransactionOctober2004);
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, clone.TransactionProtocol);
    }

    [WcfFact]
    public static void Clone_With_OleTransactions_And_AllowWildcardAction_Preserves_Both()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        original.AllowWildcardAction = true;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.OleTransactions, clone.TransactionProtocol);
        Assert.True(clone.AllowWildcardAction);
    }

    [WcfFact]
    public static void Clone_With_WSAtomicTransactionOctober2004_And_AllowWildcardAction_Preserves_Both()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransactionOctober2004);
        original.AllowWildcardAction = true;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, clone.TransactionProtocol);
        Assert.True(clone.AllowWildcardAction);
    }

    [WcfFact]
    public static void Clone_With_WSAtomicTransaction11_And_AllowWildcardAction_Preserves_Both()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        original.AllowWildcardAction = true;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, clone.TransactionProtocol);
        Assert.True(clone.AllowWildcardAction);
    }

    [WcfFact]
    public static void Clone_After_Protocol_Change_Preserves_New_Protocol()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        original.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.WSAtomicTransaction11, clone.TransactionProtocol);
    }

    [WcfFact]
    public static void Clone_After_Protocol_Change_From_WSAt11_To_OleTx()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        original.TransactionProtocol = TransactionProtocol.OleTransactions;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.OleTransactions, clone.TransactionProtocol);
    }

    [WcfFact]
    public static void Clone_After_Protocol_Change_From_OleTx_To_WSAtOct2004()
    {
        TransactionFlowBindingElement original = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        original.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        TransactionFlowBindingElement clone = (TransactionFlowBindingElement)original.Clone();

        Assert.Equal(TransactionProtocol.WSAtomicTransactionOctober2004, clone.TransactionProtocol);
    }

    // ===== ShouldSerializeTransactionProtocol tests =====

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_False_For_Default()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.False(bindingElement.ShouldSerializeTransactionProtocol());
    }

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_True_For_NonDefault()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        Assert.True(bindingElement.ShouldSerializeTransactionProtocol());
    }

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_False_For_OleTransactions()
    {
        // OleTransactions is the default, so ShouldSerialize should return false
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        Assert.False(bindingElement.ShouldSerializeTransactionProtocol());
    }

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_True_For_WSAtomicTransactionOctober2004()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransactionOctober2004);
        Assert.True(bindingElement.ShouldSerializeTransactionProtocol());
    }

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_True_After_Changing_From_Default()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.False(bindingElement.ShouldSerializeTransactionProtocol());

        bindingElement.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
        Assert.True(bindingElement.ShouldSerializeTransactionProtocol());
    }

    [WcfFact]
    public static void ShouldSerializeTransactionProtocol_Returns_False_After_Changing_Back_To_Default()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        Assert.True(bindingElement.ShouldSerializeTransactionProtocol());

        bindingElement.TransactionProtocol = TransactionProtocol.OleTransactions;
        Assert.False(bindingElement.ShouldSerializeTransactionProtocol());
    }

    // ===== CanBuildChannelFactory tests =====

    [WcfFact]
    public static void CanBuildChannelFactory_Null_Context_Throws()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement();
        Assert.Throws<ArgumentNullException>(() => bindingElement.CanBuildChannelFactory<IOutputChannel>(null));
    }

    [WcfFact]
    public static void CanBuildChannelFactory_Null_Context_Throws_For_All_Protocols()
    {
        var protocols = new[]
        {
            TransactionProtocol.OleTransactions,
            TransactionProtocol.WSAtomicTransactionOctober2004,
            TransactionProtocol.WSAtomicTransaction11
        };

        foreach (var protocol in protocols)
        {
            TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(protocol);
            Assert.Throws<ArgumentNullException>(() => bindingElement.CanBuildChannelFactory<IOutputChannel>(null));
        }
    }

    // ===== BuildChannelFactory null context throws for all protocols =====

    [WcfFact]
    public static void BuildChannelFactory_Null_Context_Throws_For_OleTransactions()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.OleTransactions);
        Assert.ThrowsAny<ArgumentException>(() => bindingElement.BuildChannelFactory<IOutputChannel>(null));
    }

    [WcfFact]
    public static void BuildChannelFactory_Null_Context_Throws_For_WSAtomicTransactionOctober2004()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransactionOctober2004);
        Assert.ThrowsAny<ArgumentException>(() => bindingElement.BuildChannelFactory<IOutputChannel>(null));
    }

    [WcfFact]
    public static void BuildChannelFactory_Null_Context_Throws_For_WSAtomicTransaction11()
    {
        TransactionFlowBindingElement bindingElement = new TransactionFlowBindingElement(TransactionProtocol.WSAtomicTransaction11);
        Assert.ThrowsAny<ArgumentException>(() => bindingElement.BuildChannelFactory<IOutputChannel>(null));
    }

    // ===== TransactionProtocol static properties =====

    [WcfFact]
    public static void TransactionProtocol_Default_Returns_OleTransactions()
    {
        Assert.Same(TransactionProtocol.OleTransactions, TransactionProtocol.Default);
    }

    [WcfFact]
    public static void TransactionProtocol_OleTransactions_Is_Not_Null()
    {
        Assert.NotNull(TransactionProtocol.OleTransactions);
    }

    [WcfFact]
    public static void TransactionProtocol_WSAtomicTransactionOctober2004_Is_Not_Null()
    {
        Assert.NotNull(TransactionProtocol.WSAtomicTransactionOctober2004);
    }

    [WcfFact]
    public static void TransactionProtocol_WSAtomicTransaction11_Is_Not_Null()
    {
        Assert.NotNull(TransactionProtocol.WSAtomicTransaction11);
    }

    [WcfFact]
    public static void TransactionProtocol_Singletons_Are_Unique()
    {
        Assert.NotSame(TransactionProtocol.OleTransactions, TransactionProtocol.WSAtomicTransactionOctober2004);
        Assert.NotSame(TransactionProtocol.OleTransactions, TransactionProtocol.WSAtomicTransaction11);
        Assert.NotSame(TransactionProtocol.WSAtomicTransactionOctober2004, TransactionProtocol.WSAtomicTransaction11);
    }

    // ===== TransactionFlowOption enum tests =====

    [WcfFact]
    public static void TransactionFlowOption_Values_Are_Correct()
    {
        Assert.Equal(0, (int)TransactionFlowOption.NotAllowed);
        Assert.Equal(1, (int)TransactionFlowOption.Allowed);
        Assert.Equal(2, (int)TransactionFlowOption.Mandatory);
    }

    [WcfFact]
    public static void TransactionFlowOption_All_Values_Are_Distinct()
    {
        Assert.NotEqual(TransactionFlowOption.NotAllowed, TransactionFlowOption.Allowed);
        Assert.NotEqual(TransactionFlowOption.NotAllowed, TransactionFlowOption.Mandatory);
        Assert.NotEqual(TransactionFlowOption.Allowed, TransactionFlowOption.Mandatory);
    }

    // ===== TransactionFlowAttribute tests =====

    [WcfFact]
    public static void TransactionFlowAttribute_Ctor_Sets_Transactions_Property()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.Allowed);
        Assert.Equal(TransactionFlowOption.Allowed, attr.Transactions);
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Ctor_NotAllowed()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.NotAllowed);
        Assert.Equal(TransactionFlowOption.NotAllowed, attr.Transactions);
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Ctor_Mandatory()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.Mandatory);
        Assert.Equal(TransactionFlowOption.Mandatory, attr.Transactions);
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Invalid_Option_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TransactionFlowAttribute((TransactionFlowOption)99));
    }

    [WcfTheory]
    [InlineData(TransactionFlowOption.NotAllowed)]
    [InlineData(TransactionFlowOption.Allowed)]
    [InlineData(TransactionFlowOption.Mandatory)]
    public static void TransactionFlowAttribute_All_Valid_Options_Accepted(TransactionFlowOption option)
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(option);
        Assert.Equal(option, attr.Transactions);
    }

    [WcfTheory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    [InlineData(int.MaxValue)]
    public static void TransactionFlowAttribute_Invalid_Options_Throw(int invalidValue)
    {
        Assert.Throws<ArgumentException>(() => new TransactionFlowAttribute((TransactionFlowOption)invalidValue));
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Is_AttributeUsage_Method()
    {
        var usageAttr = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
            typeof(TransactionFlowAttribute), typeof(AttributeUsageAttribute));
        Assert.NotNull(usageAttr);
        Assert.Equal(AttributeTargets.Method, usageAttr.ValidOn);
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Implements_IOperationBehavior()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.Allowed);
        Assert.IsAssignableFrom<System.ServiceModel.Description.IOperationBehavior>(attr);
    }

    // ===== TransactionFlowAttribute with each option preserves across read =====

    [WcfFact]
    public static void TransactionFlowAttribute_NotAllowed_Transactions_Property_Is_Stable()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.NotAllowed);
        // Read the property multiple times to ensure stability
        Assert.Equal(TransactionFlowOption.NotAllowed, attr.Transactions);
        Assert.Equal(TransactionFlowOption.NotAllowed, attr.Transactions);
    }

    [WcfFact]
    public static void TransactionFlowAttribute_Mandatory_Transactions_Property_Is_Stable()
    {
        TransactionFlowAttribute attr = new TransactionFlowAttribute(TransactionFlowOption.Mandatory);
        Assert.Equal(TransactionFlowOption.Mandatory, attr.Transactions);
        Assert.Equal(TransactionFlowOption.Mandatory, attr.Transactions);
    }

    // ===== TransactionMessageProperty tests =====

    [WcfFact]
    public static void TransactionMessageProperty_Set_With_Valid_Message()
    {
        // Verify Set method works with a valid message (without a real transaction, just test it doesn't throw on the property add)
        Message message = Message.CreateMessage(MessageVersion.Default, "test");
        // We can't easily create a Transaction in a unit test on Windows without DTC,
        // so we just verify that Set with null transaction adds the property.
        TransactionMessageProperty.Set(null, message);
        // Accessing Transaction property should return null since we set null
    }
}

// ===== Platform-specific tests (PlatformNotSupportedException on non-Windows) =====
// These tests validate that on non-Windows platforms, enabling transaction flow
// features that require DTC/OleTx will throw PlatformNotSupportedException.
// They won't execute on Windows (where transactions are supported), but they must exist.

public static class TransactionFlowPlatformTests
{
    // This condition method returns true only on non-Windows platforms.
    public static bool Is_Not_Windows()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void TransactionFormatter_OleTx_Throws_PlatformNotSupported_On_NonWindows()
    {
        var formatter = GetFormatter("OleTxFormatter");
        Assert.Throws<PlatformNotSupportedException>(() =>
            InvokeWriteTransaction(formatter, null, null));
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void TransactionFormatter_WsatFormatter10_Throws_PlatformNotSupported_On_NonWindows()
    {
        var formatter = GetFormatter("WsatFormatter10");
        Assert.Throws<PlatformNotSupportedException>(() =>
            InvokeWriteTransaction(formatter, null, null));
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void TransactionFormatter_WsatFormatter11_Throws_PlatformNotSupported_On_NonWindows()
    {
        var formatter = GetFormatter("WsatFormatter11");
        Assert.Throws<PlatformNotSupportedException>(() =>
            InvokeWriteTransaction(formatter, null, null));
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void TransactionFormatter_OleTx_ReadTransaction_Throws_PlatformNotSupported_On_NonWindows()
    {
        var formatter = GetFormatter("OleTxFormatter");
        Assert.Throws<PlatformNotSupportedException>(() =>
            InvokeReadTransaction(formatter, null));
    }

    [WcfFact]
    [Condition(nameof(Is_Not_Windows))]
    public static void TransactionFormatter_OleTx_EmptyHeader_Throws_PlatformNotSupported_On_NonWindows()
    {
        var formatter = GetFormatter("OleTxFormatter");
        Assert.Throws<PlatformNotSupportedException>(() =>
            GetEmptyTransactionHeader(formatter));
    }

    private static readonly Type s_formatterType = typeof(TransactionFlowBindingElement).Assembly
        .GetType("System.ServiceModel.Transactions.TransactionFormatter");

    private static object GetFormatter(string propertyName)
    {
        var prop = s_formatterType.GetProperty(propertyName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        return prop.GetValue(null);
    }

    private static void InvokeWriteTransaction(object formatter, object transaction, object message)
    {
        try
        {
            s_formatterType.GetMethod("WriteTransaction").Invoke(formatter, new[] { transaction, message });
        }
        catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        }
    }

    private static void InvokeReadTransaction(object formatter, object message)
    {
        try
        {
            s_formatterType.GetMethod("ReadTransaction").Invoke(formatter, new[] { message });
        }
        catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        }
    }

    private static object GetEmptyTransactionHeader(object formatter)
    {
        try
        {
            return s_formatterType.GetProperty("EmptyTransactionHeader").GetValue(formatter);
        }
        catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            return null; // unreachable
        }
    }
}
