// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace SharedPoolsOfWCFObjects
{
    public static class TestHelpers
    {
        private static bool UseHttp { get; set; }
        private static string HostName { get; set; }

        private static string s_tcpUrl;
        private static string s_httpUrl;
        private static string s_tcpDupUrl;
        private static string s_httpDupUrl;
        private static string s_tcpStreamingUrl;
        private static string s_httpStreamingUrl;

       
        public static void SetHostAndProtocol(bool useHttp, string hostName, string appName)
        {
            UseHttp = useHttp;
            HostName = hostName;
            s_tcpUrl = "net.tcp://" + hostName + ":808/" + appName + "/Service1.svc";
            s_httpUrl = "http://" + hostName + "/" + appName + "/Service1.svc";
            s_tcpDupUrl = "net.tcp://" + hostName + ":808/" + appName + "/DuplexService.svc";
            s_httpDupUrl = "http://" + hostName + "/" + appName + "/DuplexService.svc";
            s_tcpStreamingUrl = "net.tcp://" + hostName + ":808/" + appName + "/StreamingService.svc";
            s_httpStreamingUrl = "http://" + hostName + "/" + appName + "/StreamingService.svc";
            //s_httpUrl = HttpUrl;
        }

        public static EndpointAddress CreateEndPointAddress()
        {
            return UseHttp ? CreateHttpEndpointAddress() : CreateNetTcpEndpointAddress();
        }

        public static EndpointAddress CreateNetTcpEndpointAddress()
        {
            //string address = "net.tcp://cspod222-04vm4.corp.microsoft.com/WcfService1/Service1.svc";
            return new EndpointAddress(s_tcpUrl);
        }

        public static EndpointAddress CreateHttpEndpointAddress()
        {
            return new EndpointAddress(s_httpUrl);
        }

        public static EndpointAddress CreateEndPointDuplexAddress()
        {
            return UseHttp ? CreateHttpEndpointDuplexAddress() : CreateNetTcpEndpointDuplexAddress();
        }

        public static EndpointAddress CreateNetTcpEndpointDuplexAddress()
        {
            //string address = "net.tcp://cspod222-04vm4.corp.microsoft.com/WcfService1/Service1.svc";
            return new EndpointAddress(s_tcpDupUrl);
        }

        public static EndpointAddress CreateHttpEndpointDuplexAddress()
        {
            return new EndpointAddress(s_httpDupUrl);
        }

        public static EndpointAddress CreateEndPointStreamingAddress()
        {
            return UseHttp ? CreateHttpEndpointStreamingAddress() : CreateNetTcpEndpointStreamingAddress();
        }

        public static EndpointAddress CreateNetTcpEndpointStreamingAddress()
        {
            return new EndpointAddress(s_tcpStreamingUrl);
        }

        public static EndpointAddress CreateHttpEndpointStreamingAddress()
        {
            return new EndpointAddress(s_httpStreamingUrl);
        }

        public static Binding CreateBinding()
        {
            return UseHttp ? CreateHttpBinding() : CreateNetTcpBinding();
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

        public static Binding CreateStreamingBinding(int maxStreamSize)
        {
            return UseHttp ? CreateHttpStreamingBinding(maxStreamSize) : CreateNetTcpStreamingBinding(maxStreamSize);
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

        public static Binding CreateHttpStreamingBinding(int maxStreamSize)
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
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
                        System.Diagnostics.Debugger.Break();
                    }
                };

                channel.Opening += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _openingFired, 1, 0) != 0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                };

                channel.Closed += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _closedFired, 1, 0) != 0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                };

                channel.Closing += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _closingFired, 1, 0) != 0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                };

                channel.Faulted += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _faultedFired, 1, 0) != 0)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                };
            }
        }

        public static void CloseChannel<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            var cc = channel as ICommunicationObject;
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                System.Diagnostics.Debugger.Break();
            }

            cc.Close();
        }
        public static async Task CloseChannelAsync<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            var cc = (channel as IClientChannel);
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            await Task.Factory.FromAsync(cc.BeginClose, cc.EndClose, TaskCreationOptions.None);
        }

    }
}
