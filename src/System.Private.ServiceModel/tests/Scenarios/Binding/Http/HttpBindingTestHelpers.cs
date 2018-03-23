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

    public Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> OnSending { get; set; }
    public Func<HttpResponseMessage, CancellationToken, HttpResponseMessage> OnSent { get; set; }

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
        if (_parent.OnSending != null)
        {
            response = _parent.OnSending(request, cancellationToken);
            if (response != null)
                return response;
        }

        response = await base.SendAsync(request, cancellationToken);

        if (_parent.OnSent != null)
        {
            return _parent.OnSent(response, cancellationToken);
        }

        return response;
    }
}
