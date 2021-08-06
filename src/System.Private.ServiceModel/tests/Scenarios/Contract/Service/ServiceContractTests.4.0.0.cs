// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static partial class ServiceContractTests
{
    // End operation includes keyword "out" on an Int as an arg.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_IntOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractIntOutService> factory = null;
        IServiceContractIntOutService serviceProxy = null;
        int number = 0;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractIntOutService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncIntOut_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(out number, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((number == message.Count<char>()), String.Format("The local int variable was not correctly set, expected {0} but got {1}", message.Count<char>(), number));

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

    // End operation includes keyword "out" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_UniqueTypeOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeOutService> factory = null;
        IServiceContractUniqueTypeOutService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeOutService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncUniqueTypeOut_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(out uniqueType, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((uniqueType.stringValue == message),
                String.Format("The 'stringValue' field in the instance of 'UniqueType' was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // End & Begin operations include keyword "ref" on an Int as an arg.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_IntRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractIntRefService> factory = null;
        IServiceContractIntRefService serviceProxy = null;
        int number = 0;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractIntRefService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncIntRef_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(ref number, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, ref number, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((number == message.Count<char>()),
                String.Format("The value of the integer sent by reference was not the expected value. expected {0} but got {1}", message.Count<char>(), number));

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

    // End & Begin operations include keyword "ref" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_UniqueTypeRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeRefService> factory = null;
        IServiceContractUniqueTypeRefService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeRefService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncUniqueTypeRef_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(ref uniqueType, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, ref uniqueType, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((uniqueType.stringValue == message),
                String.Format("The 'stringValue' field in the instance of 'UniqueType' was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // Synchronous operation using the keyword "out" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_SyncOperation_UniqueTypeOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeOutSyncService> factory = null;
        IServiceContractUniqueTypeOutSyncService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeOutSyncService>(binding, new EndpointAddress(Endpoints.ServiceContractSyncUniqueTypeOut_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Request(message, out uniqueType);

            // *** VALIDATE *** \\
            Assert.True((uniqueType.stringValue == message),
                String.Format("The value of the 'stringValue' field in the UniqueType instance was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

            // *** EXECUTE *** \\
            uniqueType = null;
            serviceProxy.Request2(out uniqueType, message);

            // *** VALIDATE *** \\
            Assert.True((uniqueType.stringValue == message),
                String.Format("The value of the 'stringValue' field in the UniqueType instance was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // Synchronous operation using the keyword "ref" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_SyncOperation_UniqueTypeRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeRefSyncService> factory = null;
        IServiceContractUniqueTypeRefSyncService serviceProxy = null;
        UniqueType uniqueType = new UniqueType();

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeRefSyncService>(binding, new EndpointAddress(Endpoints.ServiceContractSyncUniqueTypeRef_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Request(message, ref uniqueType);

            // *** VALIDATE *** \\
            Assert.True((uniqueType.stringValue == message),
                String.Format("The value of the 'stringValue' field in the UniqueType instance was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Open_ChannelFactory()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.OpenTimeout = ScenarioTestHelpers.TestTimeout;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));

            // *** EXECUTE *** \\
            Task t = Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None);

            // *** VALIDATE *** \\
            t.GetAwaiter().GetResult();
            Assert.True(factory.State == CommunicationState.Opened,
                        String.Format("Expected factory state 'Opened', actual was '{0}'", factory.State));

            serviceProxy = factory.CreateChannel();
            result = serviceProxy.Echo(testString); // verifies factory did open correctly

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

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Open_ChannelFactory_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_Async_Open_ChannelFactory(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_Async_Open_ChannelFactory_WithSingleThreadedSyncContext timed-out.");
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Open_Proxy()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            ICommunicationObject proxyAsCommunicationObject = (ICommunicationObject)serviceProxy;
            Task t = Task.Factory.FromAsync(proxyAsCommunicationObject.BeginOpen, proxyAsCommunicationObject.EndOpen, TaskCreationOptions.None);

            // *** VALIDATE *** \\
            t.GetAwaiter().GetResult();
            Assert.True(proxyAsCommunicationObject.State == CommunicationState.Opened,
                        String.Format("Expected proxy state 'Opened', actual was '{0}'", proxyAsCommunicationObject.State));

            result = serviceProxy.Echo(testString); // verifies proxy did open correctly

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

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Open_Proxy_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_Async_Open_Proxy(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_Async_Open_Proxy_WithSingleThreadedSyncContext timed-out.");
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Close_ChannelFactory()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.CloseTimeout = ScenarioTestHelpers.TestTimeout;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();
            result = serviceProxy.Echo(testString);         // force proxy and factory to open as part of setup
            ((ICommunicationObject)serviceProxy).Close();   // force proxy closed before close factory

            // *** EXECUTE *** \\
            Task t = Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);

            // *** VALIDATE *** \\
            t.GetAwaiter().GetResult();
            Assert.True(factory.State == CommunicationState.Closed,
                        String.Format("Expected factory state 'Closed', actual was '{0}'", factory.State));

            // *** CLEANUP *** \\
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Close_ChannelFactory_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_Async_Close_ChannelFactory(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_Async_Close_ChannelFactory_WithSingleThreadedSyncContext timed-out.");
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Close_Proxy()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();
            serviceProxy.Echo(testString);  // force sync open as part of setup

            // *** EXECUTE *** \\
            ICommunicationObject proxyAsCommunicationObject = (ICommunicationObject)serviceProxy;
            Task t = Task.Factory.FromAsync(proxyAsCommunicationObject.BeginClose, proxyAsCommunicationObject.EndClose, TaskCreationOptions.None);

            // *** VALIDATE *** \\
            t.GetAwaiter().GetResult();
            Assert.True(proxyAsCommunicationObject.State == CommunicationState.Closed,
                        String.Format("Expected proxy state 'Closed', actual was '{0}'", proxyAsCommunicationObject.State));

            // *** CLEANUP *** \\
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Close_Proxy_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_Async_Close_Proxy(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_Async_Close_Proxy_WithSingleThreadedSyncContext timed-out.");
    }

    private static string StreamToString(Stream stream)
    {
        stream.Read(Array.Empty<byte>(), 0, 0);
        var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static Stream StringToStream(string str)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms, Encoding.UTF8);
        sw.Write(str);
        sw.Flush();
        ms.Position = 0;
        return ms;
    }
}
