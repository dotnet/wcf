using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Channels;
using Infrastructure.Common;
using Xunit;

public static class HttpIdentityTests
{
    [WcfFact]
    public static void SpnEndpointIdentity_NotSupportedThrows()
    {
        var identity = new SpnEndpointIdentity("SERVICE/serverhostname");
        var endpointAddress = new EndpointAddress(new Uri("https://serverhostname/fakeService.svc"), identity);
        var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
        var factory = new ChannelFactory<IService>(binding, endpointAddress);
        var channel = factory.CreateChannel();
        Assert.Throws<InvalidOperationException>(() => channel.Echo(""));
    }

    [WcfFact]
    public static void UpnEndpointIdentityThrows()
    {
        var identity = new UpnEndpointIdentity("user@contoso.com");
        var endpointAddress = new EndpointAddress(new Uri("https://serverhostname/fakeService.svc"), identity);
        var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
        var factory = new ChannelFactory<IService>(binding, endpointAddress);
        var channel = factory.CreateChannel();
        Assert.Throws<InvalidOperationException>(() => channel.Echo(""));
    }

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string Echo(string value);
    }
}
