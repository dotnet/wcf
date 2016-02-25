// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SharedPoolsOfWCFObjects
{
    public enum TestBinding
    {
        Http = 0,
        NetTcp = 1,
        NetHttpBinding = 2
    }
    public static class TestHelpers
    {
        private static TestBinding UseBinding { get; set; }
        private static string HostName { get; set; }

        private static string s_tcpHelloUrl;
        private static string s_httpHelloUrl;
        private static string s_netHttpHelloUrl;

        private static string s_tcpDupUrl;
        private static string s_httpDupUrl;
        private static string s_netHttpDupUrl;

        private static string s_tcpStreamingUrl;
        private static string s_httpStreamingUrl;
        private static string s_netHttpDupStreamingUrl;

        public static int MaxPooledFactories { get; set; }
        public static int MaxPooledChannels { get; set; }

        public static void SetHostAndProtocol(TestBinding useBinding, string hostName, string appName)
        {
            UseBinding = useBinding;
            HostName = hostName;
            s_httpHelloUrl = "http://" + hostName + "/" + appName + "/Service1.svc";
            s_httpDupUrl = "http://" + hostName + "/" + appName + "/DuplexService.svc";
            s_httpStreamingUrl = "http://" + hostName + "/" + appName + "/StreamingService.svc";

            s_netHttpHelloUrl = "http://" + hostName + "/" + appName + "/Service1.svc/nethttp";
            s_netHttpDupUrl = "ws://" + hostName + "/" + appName + "/DuplexService.svc/websocket";
            s_netHttpDupStreamingUrl = "ws://" + hostName + "/" + appName + "/DuplexStreamingService.svc";

            s_tcpHelloUrl = "net.tcp://" + hostName + ":808/" + appName + "/Service1.svc";
            s_tcpDupUrl = "net.tcp://" + hostName + ":808/" + appName + "/DuplexService.svc";
            s_tcpStreamingUrl = "net.tcp://" + hostName + ":808/" + appName + "/StreamingService.svc";
        }

        public static EndpointAddress CreateEndPointHelloAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointAddress();
                default:
                    return null;
            }
        }
        public static EndpointAddress CreateHttpEndpointAddress()
        {
            return new EndpointAddress(s_httpHelloUrl);
        }
        public static EndpointAddress CreateNetHttpEndpointAddress()
        {
            return new EndpointAddress(s_netHttpHelloUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointAddress()
        {
            return new EndpointAddress(s_tcpHelloUrl);
        }


        public static EndpointAddress CreateEndPointDuplexAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointDuplexAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointDuplexAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointDuplexAddress();
                default:
                    return null;
            }
        }


        public static EndpointAddress CreateHttpEndpointDuplexAddress()
        {
            return new EndpointAddress(s_httpDupUrl);
        }
        public static EndpointAddress CreateNetHttpEndpointDuplexAddress()
        {
            return new EndpointAddress(s_netHttpDupUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointDuplexAddress()
        {
            return new EndpointAddress(s_tcpDupUrl);
        }

        public static EndpointAddress CreateEndPointStreamingAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointStreamingAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointDuplexStreamingAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointStreamingAddress();
                default:
                    return null;
            }
        }
        public static EndpointAddress CreateHttpEndpointStreamingAddress()
        {
            return new EndpointAddress(s_httpStreamingUrl);
        }
        public static EndpointAddress CreateNetHttpEndpointDuplexStreamingAddress()
        {
            return new EndpointAddress(s_netHttpDupStreamingUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointStreamingAddress()
        {
            return new EndpointAddress(s_tcpStreamingUrl);
        }

        public static Binding CreateBinding()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpBinding();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpBinding();
                case TestBinding.NetTcp:
                    return CreateNetTcpBinding();
                default:
                    return null;
            }
        }
        public static NetTcpBinding CreateNetTcpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security = new NetTcpSecurity();
            binding.Security.Mode = SecurityMode.None;
            return binding;
        }
        public static Binding CreateHttpBinding()
        {
            return new BasicHttpBinding();
        }

        public static Binding CreateNetHttpBinding()
        {
            Binding netHttp = new NetHttpBinding();
            return netHttp;
        }

        public static Binding CreateStreamingBinding(int maxStreamSize)
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpStreamingBinding(maxStreamSize);
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpStreamingBinding(maxStreamSize);
                case TestBinding.NetTcp:
                    return CreateNetTcpStreamingBinding(maxStreamSize);
                default:
                    //Debug.Break
                    return null;
            }
        }
        public static Binding CreateHttpStreamingBinding(int maxStreamSize)
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            return binding;
        }
        public static NetTcpBinding CreateNetTcpStreamingBinding(int maxStreamSize)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security = new NetTcpSecurity();
            binding.Security.Mode = SecurityMode.None;
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            return binding;
        }
        public static Binding CreateNetHttpStreamingBinding(int maxStreamSize)
        {
            var binding = new NetHttpBinding();

            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public static ChannelFactory<C> CreateChannelFactory<C>(EndpointAddress a, Binding b)
        {
            var factory = new ChannelFactory<C>(b, a);
            new CommunicationObjectEventVerifier(factory);
            return factory;
        }

        public static DuplexChannelFactory<C> CreateDuplexChannelFactory<C>(EndpointAddress a, Binding b, object duplexCallback)
        {
            var instanceContext = new InstanceContext(duplexCallback);

            var factory = new DuplexChannelFactory<C>(instanceContext, b, a);
            new CommunicationObjectEventVerifier(factory);
            new CommunicationObjectEventVerifier(instanceContext);
            return factory;
        }

        public static void CloseFactory<C>(ChannelFactory<C> factory)
        {
            factory.Close();
        }

        public static async Task CloseFactoryAsync<C>(ChannelFactory<C> factory)
        {
            await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);
        }

        public static C CreateChannel<C>(ChannelFactory<C> factory)
        {
            var channel = factory.CreateChannel();
            new CommunicationObjectEventVerifier(channel as ICommunicationObject);
            return channel;
        }

        public class CommunicationObjectEventVerifier
        {
            private int _openedFired;
            private int _openingFired;
            private int _closedFired;
            private int _closingFired;
            private int _faultedFired;

            public CommunicationObjectEventVerifier(ICommunicationObject channel)
            {
                channel.Opened += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _openedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Opened event fired more than once");
                    }
                };

                channel.Opening += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _openingFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Opening event fired more than once");
                    }
                };

                channel.Closed += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _closedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Closed event fired more than once");
                    }
                };

                channel.Closing += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _closingFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Closing event fired more than once");
                    }
                };

                channel.Faulted += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _faultedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Faulted event fired more than once");
                    }
                };
            }
        }

        public static void CloseChannel<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = channel as ICommunicationObject;
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not ICommunicationObject");
            }

            cc.Close();
        }
        public static async Task CloseChannelAsync<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = (channel as IClientChannel);
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not IClientChannel");
            }
            await Task.Factory.FromAsync(cc.BeginClose, cc.EndClose, TaskCreationOptions.None);
        }
    }

    public static class TestUtils
    {
        public static void ReportFailure(string message)
        {
            Console.WriteLine(message);
            Debugger.Break();
            GC.KeepAlive(message);
        }
    }
}
