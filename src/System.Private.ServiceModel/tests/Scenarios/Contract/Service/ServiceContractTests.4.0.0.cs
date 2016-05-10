// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public static partial class ServiceContractTests
{
    // End operation includes keyword "out" on an Int as an arg.
    [Fact]
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
    [Fact]
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
    [Fact]
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
    [Fact]
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
    [Fact]
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
    [Fact]
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

    private static void PrintInnerExceptionsHresult(Exception e, StringBuilder errorBuilder)
    {
        if (e.InnerException != null)
        {
            errorBuilder.AppendLine(string.Format("\r\n InnerException type: '{0}', Hresult:'{1}'", e.InnerException, e.InnerException.HResult));
            PrintInnerExceptionsHresult(e.InnerException, errorBuilder);
        }
    }

    private static string StreamToString(Stream stream)
    {
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
