// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel;
using Infrastructure.Common;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Threading;
using Xunit.Sdk;

public class DuplexChannelWithSynchronizationContext : ConditionalWcfTest
{
    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void SingleThreadedSyncContext_SetOnInstanceContext()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService serviceProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_Certificate_Duplex_Address));
            string clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            var syncCtx = new TestTypes.SingleThreadSynchronizationContext(true);
            Task syncCtxTask = syncCtx.RunOnThreadPoolThread();
            SyncContextServiceCallback callbackService = new SyncContextServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);
            context.SynchronizationContext = syncCtx;

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Ping(guid);
            // Ping on another thread.
            //Task.Run(() => serviceProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;
            SynchronizationContext callbackSyncContext = callbackService.SynchronizationContext;

            // *** VALIDATE *** \\
            Assert.Equal(guid, returnedGuid);
            Assert.Same(syncCtx, callbackSyncContext);
            syncCtx.Complete();
            Assert.True(syncCtxTask.Wait(ScenarioTestHelpers.TestTimeout));
            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            ((ICommunicationObject)factory).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void SingleThreadedSyncContext_AmbientCapture()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService serviceProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_Certificate_Duplex_Address));
            string clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            var syncCtx = new TestTypes.SingleThreadSynchronizationContext(true);
            Task syncCtxTask = syncCtx.RunOnThreadPoolThread();
            var prevCtx = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(syncCtx);
            SyncContextServiceCallback callbackService = new SyncContextServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);
            // Capture sync context
            factory.Open();
            SynchronizationContext.SetSynchronizationContext(prevCtx);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Ping(guid);
            // Ping on another thread.
            //Task.Run(() => serviceProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;
            SynchronizationContext callbackSyncContext = callbackService.SynchronizationContext;

            // *** VALIDATE *** \\
            Assert.Equal(guid, returnedGuid);
            Assert.Same(syncCtx, callbackSyncContext);
            syncCtx.Complete();
            Assert.True(syncCtxTask.Wait(ScenarioTestHelpers.TestTimeout));
            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            ((ICommunicationObject)factory).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void SingleThreadedSyncContext_CallbackUsingDefaultSyncCtx_SyncCallNotBlocked()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => SingleThreadedSyncContext_CallbackUsingDefaultSyncCtx_SyncCallNotBlocked_Helper(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout * 100);
        Assert.True(success, "Test Scenario: TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");

    }

    private static void SingleThreadedSyncContext_CallbackUsingDefaultSyncCtx_SyncCallNotBlocked_Helper()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService serviceProxy = null;
        Guid guid = Guid.NewGuid();
        Assert.IsType<TestTypes.SingleThreadSynchronizationContext>(SynchronizationContext.Current);

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_Certificate_Duplex_Address));
            string clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            SyncContextServiceCallback callbackService = new SyncContextServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);
            // Use default sync context for callbacks
            context.SynchronizationContext = new SynchronizationContext();

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Ping(guid);
            Guid returnedGuid = callbackService.CallbackGuid;
            SynchronizationContext callbackSyncContext = callbackService.SynchronizationContext;

            // *** VALIDATE *** \\
            Assert.Equal(guid, returnedGuid);
            Assert.Null(callbackSyncContext);
            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            ((ICommunicationObject)factory).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void SingleThreadedSyncContext_NoCallback_SyncCallNotBlocked()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => SingleThreadedSyncContext_SyncCallNotBlocked_Helper(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout * 100);
        Assert.True(success, "Test Scenario: TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");

    }

    private static void SingleThreadedSyncContext_SyncCallNotBlocked_Helper()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Assert.IsType<TestTypes.SingleThreadSynchronizationContext>(SynchronizationContext.Current);

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    private class SyncContextServiceCallback : IWcfDuplexServiceCallback
    {
        private TaskCompletionSource<Guid> _tcs = new TaskCompletionSource<Guid>();

        public void OnPingCallback(Guid guid)
        {
            // Set the result in an async task with a 100ms delay to prevent a race condition
            // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
            Task.Run(async () =>
            {
                await Task.Delay(100);
                _tcs.TrySetResult(guid);
            });
            SynchronizationContext = SynchronizationContext.Current;
        }

        public Guid CallbackGuid
        {
            get
            {
                if (_tcs.Task.Wait(ScenarioTestHelpers.TestTimeout))
                {
                    return _tcs.Task.Result;
                }
                throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", ScenarioTestHelpers.TestTimeout));
            }
        }

        public SynchronizationContext SynchronizationContext { get; private set; }
    }
}
