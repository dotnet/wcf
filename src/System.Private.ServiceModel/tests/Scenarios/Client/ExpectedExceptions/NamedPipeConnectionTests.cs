// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using CoreWCF.Configuration;
using Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

[SupportedOSPlatform("windows")]
public class NamedPipeConnectionTests : ConditionalWcfTest
{
    private readonly ITestOutputHelper _output;

    public NamedPipeConnectionTests(ITestOutputHelper output) => _output = output;

    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [OuterLoop]
    public async Task HighConcurrency_Connections_RetryUntilAvailable()
    {
        const int ConnectionCount = 100;
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        Uri address = new Uri($"net.pipe://localhost/NamedPipeConnectionTests/{Guid.NewGuid():N}/");
        using IHost host = new HostBuilder()
            .UseNetNamedPipe(options => options.Listen(address, listenOptions => listenOptions.MaxPendingAccepts = 1))
            .ConfigureServices(services => services.AddServiceModelServices())
            .Build();
        host.UseServiceModel(services =>
        {
            services.AddService<EchoService>();
            services.AddServiceEndpoint<EchoService, IEchoService>(
                new CoreWCF.NetNamedPipeBinding(CoreWCF.NetNamedPipeSecurityMode.None), address.AbsoluteUri);
        });
        using CancellationTokenSource startupTimeout = new CancellationTokenSource(timeout);
        await host.StartAsync(startupTimeout.Token);

        ChannelFactory<IEchoService>[] factories = new ChannelFactory<IEchoService>[ConnectionCount];
        IEchoService[] proxies = new IEchoService[ConnectionCount];
        IClientChannel[] channels = new IClientChannel[ConnectionCount];
        Task[] opens = new Task[ConnectionCount];
        using CountdownEvent ready = new CountdownEvent(ConnectionCount);
        using ManualResetEventSlim start = new ManualResetEventSlim();

        try
        {
            for (int i = 0; i < ConnectionCount; i++)
            {
                CustomBinding binding = new CustomBinding(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None))
                {
                    OpenTimeout = timeout,
                    SendTimeout = timeout
                };
                binding.Elements.Find<NamedPipeTransportBindingElement>().ConnectionPoolSettings.GroupName = Guid.NewGuid().ToString();
                factories[i] = new ChannelFactory<IEchoService>(binding, new EndpointAddress(address));
                factories[i].Open();
                proxies[i] = factories[i].CreateChannel();
                IClientChannel channel = (IClientChannel)proxies[i];
                channels[i] = channel;

                // Dedicated workers create a connection burst without starving CoreWCF's accept loop.
                opens[i] = Task.Factory.StartNew(() =>
                {
                    ready.Signal();
                    Assert.True(start.Wait(timeout), "The connection burst was not released.");
                    channel.Open();
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }

            Assert.True(ready.Wait(timeout), "The connection workers did not become ready.");
            start.Set();
            Exception failure = await Record.ExceptionAsync(() => Task.WhenAll(opens));
            int established = channels.Count(channel => channel.State == CommunicationState.Opened);
            int faulted = channels.Count(channel => channel.State == CommunicationState.Faulted);
            _output.WriteLine($"Established: {established}/{ConnectionCount}; faulted: {faulted}/{ConnectionCount}.");
            Assert.True(failure == null, $"Established: {established}/{ConnectionCount}; faulted: {faulted}/{ConnectionCount}. First failure: {failure}");
            Assert.Equal(ConnectionCount, established);
            Assert.All(proxies, proxy => Assert.Equal("Hello", proxy.Echo("Hello")));
        }
        finally
        {
            start.Set();
            // Drain workers even if setup or the readiness assertion failed.
            Exception openFailure = await Record.ExceptionAsync(() => Task.WhenAll(opens.Where(task => task != null)));
            if (openFailure != null)
            {
                _output.WriteLine($"Channel opening failed: {openFailure}");
            }

            for (int i = 0; i < ConnectionCount; i++)
            {
                ScenarioTestHelpers.CloseCommunicationObjects(channels[i], factories[i]);
            }

            await host.StopAsync(timeout);
        }
    }

    [ServiceContract]
    [CoreWCF.ServiceContract]
    public interface IEchoService
    {
        [OperationContract]
        [CoreWCF.OperationContract]
        string Echo(string message);
    }

    public class EchoService : IEchoService
    {
        public string Echo(string message) => message;
    }
}
