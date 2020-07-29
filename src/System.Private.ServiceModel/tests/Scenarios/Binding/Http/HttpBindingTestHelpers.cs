using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class HttpMessageHandlerBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(GetHttpMessageHandler));
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

    public void Validate(ServiceEndpoint endpoint) { }

    public HttpMessageHandler GetHttpMessageHandler(HttpClientHandler httpClientHandler)
    {
        return new InterceptingHttpMessageHandler(httpClientHandler, this);
    }

    public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> OnSendingAsync { get; set; }
    public Func<HttpResponseMessage, CancellationToken, Task<HttpResponseMessage>> OnSentAsync { get; set; }

}

public class InterceptingHttpMessageHandler : DelegatingHandler
{
    private readonly HttpMessageHandlerBehavior _parent;

    public InterceptingHttpMessageHandler(HttpMessageHandler innerHandler, HttpMessageHandlerBehavior parent)
    {
        InnerHandler = innerHandler;
        _parent = parent;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        if (_parent.OnSendingAsync != null)
        {
            response = await _parent.OnSendingAsync(request, cancellationToken);
            if (response != null)
                return response;
        }

        response = await base.SendAsync(request, cancellationToken);

        if (_parent.OnSentAsync != null)
        {
            return await _parent.OnSentAsync(response, cancellationToken);
        }

        return response;
    }
}
