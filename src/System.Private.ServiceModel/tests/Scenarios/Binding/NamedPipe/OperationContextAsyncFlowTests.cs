//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.ExceptionServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using ScenarioTests.Common;
using Xunit;

public class OperationContextAsyncFlowTests
{
    [WcfFact]
    public static async Task OperationContextScopeAsyncFlow()
    {
        // This test lives in the scenario tests as it needs a real channel to test OperationContext
        // even though nothing is being sent across the wire.
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        var exisitingSyncContext = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(ThreadHoppingSynchronizationContext.Instance);
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.NamedPipe_DefaultBinding_Address));
            serviceProxy = factory.CreateChannel();
            Assert.Null(OperationContext.Current);
            using (var scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                Assert.NotNull(OperationContext.Current);
                var currentContext = OperationContext.Current;
                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                await Task.Yield();
                Assert.NotEqual(currentThreadId, Thread.CurrentThread.ManagedThreadId);
                Assert.Equal(currentContext, OperationContext.Current);
            }
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

    //    [WcfFact]
    //    public static async Task OperationContextScopeNestedAsyncFlow()
    //    {
    //        // This test lives in the scenario tests as it needs a real channel to test OperationContext
    //        // even though nothing is being sent across the wire.
    //        ChannelFactory<IWcfService> factory = null;
    //        IWcfService serviceProxy = null;
    //        var exisitingSyncContext = SynchronizationContext.Current;
    //        try
    //        {
    //            SynchronizationContext.SetSynchronizationContext(ThreadHoppingSynchronizationContext.Instance);
    //            NetTcpBinding binding = new NetTcpBinding();
    //            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));
    //            serviceProxy = factory.CreateChannel();
    //            Assert.Null(OperationContext.Current);
    //            using (var scope = new OperationContextScope((IContextChannel)serviceProxy))
    //            {
    //                Assert.NotNull(OperationContext.Current);
    //                var firstContext = OperationContext.Current;
    //                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
    //                await Task.Yield();
    //                Assert.NotEqual(currentThreadId, Thread.CurrentThread.ManagedThreadId);
    //                Assert.Equal(firstContext, OperationContext.Current);
    //                using (var scope2 = new OperationContextScope((IContextChannel)serviceProxy))
    //                {
    //                    Assert.NotEqual(firstContext, OperationContext.Current);
    //                    var secondContext = OperationContext.Current;
    //                    currentThreadId = Thread.CurrentThread.ManagedThreadId;
    //                    await Task.Yield();
    //                    Assert.NotEqual(currentThreadId, Thread.CurrentThread.ManagedThreadId);
    //                    Assert.Equal(secondContext, OperationContext.Current);
    //                }
    //                Assert.Equal(firstContext, OperationContext.Current);
    //            }
    //            Assert.Null(OperationContext.Current);
    //            ((IClientChannel)serviceProxy).Close();
    //            factory.Close();
    //        }
    //        finally
    //        {
    //            // *** ENSURE CLEANUP *** \\
    //            SynchronizationContext.SetSynchronizationContext(exisitingSyncContext);
    //            await Task.Yield(); // Hop back to original sync context
    //            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
    //        }
    //    }


    //    [WcfFact]
    //    public static async Task OperationContextCallbackAsyncFlow()
    //    {
    //        ChannelFactory<IDuplexChannelService> factory = null;
    //        IDuplexChannelService serviceProxy = null;
    //        Guid guid = Guid.NewGuid();

    //        try
    //        {
    //            // *** SETUP *** \\
    //            NetTcpBinding binding = new NetTcpBinding();
    //            binding.Security.Mode = SecurityMode.None;
    //            DuplexChannelServiceCallback callbackService = new DuplexChannelServiceCallback();
    //            InstanceContext context = new InstanceContext(callbackService);
    //            EndpointAddress endpointAddress = new EndpointAddress(Endpoints.Tcp_NoSecurity_DuplexCallback_Address);
    //            factory = new DuplexChannelFactory<IDuplexChannelService>(context, binding, endpointAddress);
    //            serviceProxy = factory.CreateChannel();

    //            // *** EXECUTE *** \\
    //            serviceProxy.Ping(guid);
    //            await callbackService.CallCompletedTask;

    //            // *** VALIDATE *** \\
    //            if (callbackService.Exception != null)
    //            {
    //                ExceptionDispatchInfo.Capture(callbackService.Exception).Throw();
    //            }

    //            // *** CLEANUP *** \\
    //            ((ICommunicationObject)serviceProxy).Close();
    //            factory.Close();
    //        }
    //        finally
    //        {
    //            // *** ENSURE CLEANUP *** \\
    //            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
    //        }
    //    }

    //    public class DuplexChannelServiceCallback : IDuplexChannelCallback
    //    {
    //        private TaskCompletionSource<object> _tcs;

    //        public DuplexChannelServiceCallback()
    //        {
    //            _tcs = new TaskCompletionSource<object>();
    //        }

    //        public Exception Exception { get; private set; }

    //        public Task CallCompletedTask => _tcs.Task;

    //        public async Task OnPingCallbackAsync(Guid guid)
    //        {
    //            var exisitingSyncContext = SynchronizationContext.Current;
    //            try
    //            {
    //                SynchronizationContext.SetSynchronizationContext(ThreadHoppingSynchronizationContext.Instance);
    //                var asyncFlowDisabled = AppContext.TryGetSwitch("System.ServiceModel.OperationContext.DisableAsyncFlow", out var enabled) && enabled;
    //                Assert.False(asyncFlowDisabled, "DisableAsyncFlow should not be set to true");
    //                var opContext = OperationContext.Current;
    //                Assert.NotNull(opContext);
    //                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
    //                await Task.Yield();
    //                Assert.NotEqual(currentThreadId, Thread.CurrentThread.ManagedThreadId);
    //                Assert.Equal(opContext, OperationContext.Current);
    //                _tcs.TrySetResult(null);
    //            }
    //            catch (Exception e)
    //            {
    //                Exception = e;
    //            }
    //            finally
    //            {
    //                // *** ENSURE CLEANUP *** \\
    //                SynchronizationContext.SetSynchronizationContext(exisitingSyncContext);
    //                await Task.Yield(); // Hop back to original sync context
    //            }
    //        }
    //    }

    //    [ServiceContract(CallbackContract = typeof(IDuplexChannelCallback))]
    //    public interface IDuplexChannelService
    //    {
    //        [OperationContract(IsOneWay = true)]
    //        void Ping(Guid guid);
    //    }

    //    public interface IDuplexChannelCallback
    //    {
    //        [OperationContract(IsOneWay = true)]
    //        Task OnPingCallbackAsync(Guid guid);
    //    }
}
