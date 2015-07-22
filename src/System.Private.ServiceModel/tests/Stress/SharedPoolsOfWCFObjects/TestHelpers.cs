// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;


namespace WCFClientStressTests
{
    public static class TestHelpers
    {
        private static bool UseHttp { get; set; }
        private static string HostName { get; set; }

        private static string s_tcpUrl;
        private static string s_httpUrl;

        public static void SetHostAndProtocol(bool useHttp, string hostName, string appName)
        {
            UseHttp = useHttp;
            HostName = hostName;
            s_tcpUrl = "net.tcp://" + hostName + ":808/" + appName + "/Service1.svc";
            s_httpUrl = "http://" + hostName + "/" + appName + "/Service1.svc";
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

        public static ChannelFactory<C> CreateChannelFactory<C>(EndpointAddress a, Binding b)
        {
            var factory = new ChannelFactory<C>(b, a);
            new CommunicationObjectEventVerifier(factory);
            return factory;
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

        private static long s_closeCalls = 0;
        public static void CloseChannel<C>(C channel)
        {
            Interlocked.Increment(ref s_closeCalls);
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            // Getting a null would indicate an issue in tests
            if (channel as ICommunicationObject == null)
            {
                System.Diagnostics.Debugger.Break();
            }

            ((ICommunicationObject)channel).Close();
        }

        public static async Task CloseChannelAsync<C>(C channel)
        {
            var cc = (channel as IClientChannel);
            await Task.Factory.FromAsync(cc.BeginClose, cc.EndClose, TaskCreationOptions.None);
        }

        public static void CloseFactory<C>(ChannelFactory<C> factory)
        {
            factory.Close();
        }

        public static async Task CloseFactoryAsync<C>(ChannelFactory<C> factory)
        {
            await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);
        }
    }
}
