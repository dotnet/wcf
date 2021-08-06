// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using ScenarioTests.Common;
using Xunit;

public static class OperationContextTests
{
#pragma warning disable xUnit1013 // Public method should be marked as test
    [ModuleInitializer]
    public static void InitializeModule()
    {
        AppContext.SetSwitch("System.ServiceModel.OperationContext.DisableAsyncFlow", true);
    }
#pragma warning restore xUnit1013 // Public method should be marked as test

    // This test verifies that using the AppContext switch does revert to the old behavior and has the
    // same 2 failure modes (not flowing past an async, and disposing the OperationContextScope throws
    // after a thread switch.
    // This test lives in the scenario tests as it needs a real channel to test OperationContext
    // even though nothing is being sent across the wire.
    [WcfFact]
    public static async Task OperationContextLegacyBehavior()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        var exisitingSyncContext = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(ThreadHoppingSynchronizationContext.Instance);
            bool asyncFlowDisabled = AppContext.TryGetSwitch("System.ServiceModel.OperationContext.DisableAsyncFlow", out bool switchEnabled) && switchEnabled;
            Assert.True(asyncFlowDisabled, "Async flow of Operation Context isn't disabled");
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();
            ((IClientChannel)serviceProxy).Open();
            Assert.Null(OperationContext.Current);
            var scope = new OperationContextScope((IContextChannel)serviceProxy);
            Assert.NotNull(OperationContext.Current);
            var currentContext = OperationContext.Current;
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.Yield();
            Assert.NotEqual(currentThreadId, Thread.CurrentThread.ManagedThreadId);
            Assert.NotEqual(currentContext, OperationContext.Current);
            Assert.Throws<InvalidOperationException>(() => scope.Dispose());
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(exisitingSyncContext);
            await Task.Yield(); // Hop back to original sync context
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
