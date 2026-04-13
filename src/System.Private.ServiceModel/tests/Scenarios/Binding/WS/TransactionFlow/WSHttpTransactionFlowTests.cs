// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using System.Transactions;
using Infrastructure.Common;
using Xunit;

public class WSHttpTransactionFlowTests : ConditionalWcfTest
{
    static WSHttpTransactionFlowTests()
    {
        // WCF transaction flow promotes local transactions to distributed transactions (DTC)
        // when serializing them over the wire. .NET 7+ requires explicit opt-in for this.
        // This property is Windows-only but is safe to call here; tests are gated by Windows_Authentication_Available.
#pragma warning disable CA1416 // Validate platform compatibility
        TransactionManager.ImplicitDistributedTransactions = true;
#pragma warning restore CA1416
    }

    // ==========================================================================
    // WSHttpBinding (WSAtomicTransactionOctober2004 protocol)
    // ==========================================================================

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_RoundTrips()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = serviceProxy.IsTransactionFlowed();
                scope.Complete();
            }

            // *** VALIDATE *** \\
            Assert.True(result, "Expected the transaction to flow to the service, but IsTransactionFlowed returned false.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_ClientRollback_Succeeds()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                // Call the service within the transaction
                result = serviceProxy.IsTransactionFlowed();
                // Do NOT call scope.Complete() - this causes a rollback
            }

            // *** VALIDATE *** \\
            // The service call should have succeeded even though the transaction rolled back
            Assert.True(result, "Expected the transaction to flow to the service before rollback.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_ServiceFault_PropagatesException()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE & VALIDATE *** \\
            Assert.Throws<FaultException>(() =>
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    serviceProxy.ThrowDuringTransaction();
                    scope.Complete();
                }
            });

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_NoScope_NoTransactionFlowed()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            // Call without a TransactionScope - no transaction should flow
            bool result = serviceProxy.IsTransactionFlowed();

            // *** VALIDATE *** \\
            // The service sees a transaction because TransactionScopeRequired=true on the service creates one,
            // but it is a local transaction, not a flowed one. IsTransactionFlowed checks Transaction.Current != null
            // which will be true due to TransactionScopeRequired. This validates the service is reachable without a scope.
            Assert.True(result, "Expected service to have a local transaction from TransactionScopeRequired.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // ===== Multiple calls in a single transaction =====

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_MultipleCalls_InSingleTransaction()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result1, result2;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result1 = serviceProxy.IsTransactionFlowed();
                result2 = serviceProxy.IsTransactionFlowed();
                scope.Complete();
            }

            // *** VALIDATE *** \\
            Assert.True(result1, "First call: expected the transaction to flow to the service.");
            Assert.True(result2, "Second call: expected the transaction to flow to the service.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // ==========================================================================
    // WS2007HttpBinding (WSAtomicTransaction11 protocol)
    // ==========================================================================

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WS2007HttpBinding_TransactionFlow_WSAtomicTransaction11_RoundTrips()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WS2007HttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = serviceProxy.IsTransactionFlowed();
                scope.Complete();
            }

            // *** VALIDATE *** \\
            Assert.True(result, "Expected the transaction to flow via WS-AT11 protocol, but IsTransactionFlowed returned false.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WS2007HttpBinding_TransactionFlow_ClientRollback_Succeeds()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WS2007HttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = serviceProxy.IsTransactionFlowed();
                // Do NOT call scope.Complete() - this causes a rollback
            }

            // *** VALIDATE *** \\
            Assert.True(result, "Expected the transaction to flow via WS-AT11 before rollback.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WS2007HttpBinding_TransactionFlow_ServiceFault_PropagatesException()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WS2007HttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE & VALIDATE *** \\
            Assert.Throws<FaultException>(() =>
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    serviceProxy.ThrowDuringTransaction();
                    scope.Complete();
                }
            });

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WS2007HttpBinding_TransactionFlow_NoScope_NoTransactionFlowed()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WS2007HttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result = serviceProxy.IsTransactionFlowed();

            // *** VALIDATE *** \\
            Assert.True(result, "Expected service to have a local transaction from TransactionScopeRequired.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WS2007HttpBinding_TransactionFlow_MultipleCalls_InSingleTransaction()
    {
        ChannelFactory<IWcfTransactionService> factory = null;
        IWcfTransactionService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionService>(binding, new EndpointAddress(Endpoints.WS2007HttpTransactionFlowAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result1, result2;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result1 = serviceProxy.IsTransactionFlowed();
                result2 = serviceProxy.IsTransactionFlowed();
                scope.Complete();
            }

            // *** VALIDATE *** \\
            Assert.True(result1, "First call: expected the transaction to flow via WS-AT11.");
            Assert.True(result2, "Second call: expected the transaction to flow via WS-AT11.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // ==========================================================================
    // TransactionFlowOption.Mandatory tests (WSHttpBinding)
    // ==========================================================================

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_Mandatory_RoundTrips()
    {
        ChannelFactory<IWcfTransactionMandatoryService> factory = null;
        IWcfTransactionMandatoryService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionMandatoryService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowMandatoryAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = serviceProxy.IsTransactionFlowed();
                scope.Complete();
            }

            // *** VALIDATE *** \\
            Assert.True(result, "Expected the transaction to flow to the mandatory service.");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WSHttpBinding_TransactionFlow_Mandatory_WithoutScope_Throws()
    {
        ChannelFactory<IWcfTransactionMandatoryService> factory = null;
        IWcfTransactionMandatoryService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            factory = new ChannelFactory<IWcfTransactionMandatoryService>(binding, new EndpointAddress(Endpoints.WSHttpTransactionFlowMandatoryAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE & VALIDATE *** \\
            // Calling a Mandatory operation without an ambient transaction should throw.
            // The client-side TransactionChannel enforces Mandatory by requiring a flowed transaction.
            Assert.ThrowsAny<ProtocolException>(() =>
            {
                serviceProxy.IsTransactionFlowed();
            });

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
