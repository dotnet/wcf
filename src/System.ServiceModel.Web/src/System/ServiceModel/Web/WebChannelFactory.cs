// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System.ServiceModel.Web
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    public class WebChannelFactory<TChannel> : ChannelFactory<TChannel>
        where TChannel : class
    {
        public WebChannelFactory()
            : base()
        {
        }

        public WebChannelFactory(Binding binding)
            : base(binding)
        {
        }

        public WebChannelFactory(ServiceEndpoint endpoint) :
            base(endpoint)
        {
        }

        // The following constructors were available in .NET Framework's WebChannelFactory<T>
        // but ChannelFactory<T> in dotnet/wcf does not expose matching base constructors
        // (no (string endpointConfigurationName) or (Type channelType) overload), so they
        // are intentionally omitted. Use WebChannelFactory(Binding, Uri) or
        // WebChannelFactory(ServiceEndpoint) instead.
        //
        // [SuppressMessage(...)]
        // public WebChannelFactory(string endpointConfigurationName)
        //     : base(endpointConfigurationName) { }
        //
        // public WebChannelFactory(Type channelType)
        //     : base(channelType) { }

        public WebChannelFactory(Uri remoteAddress)
            : this(GetDefaultBinding(remoteAddress), remoteAddress)
        {
        }

        public WebChannelFactory(Binding binding, Uri remoteAddress)
            : base(binding, (remoteAddress != null) ? new EndpointAddress(remoteAddress) : (EndpointAddress)null)
        {
        }

        // WebChannelFactory(string endpointConfigurationName, Uri remoteAddress) omitted
        // (same reason as above).

        protected override void OnOpening()
        {
            if (this.Endpoint == null)
            {
                return;
            }

            // if the binding is missing, set up a default binding
            if (this.Endpoint.Binding == null && this.Endpoint.Address != null)
            {
                this.Endpoint.Binding = GetDefaultBinding(this.Endpoint.Address.Uri);
            }
            // WebServiceHost.SetRawContentTypeMapperIfNecessary is server-side and not part of
            // this client-only port. Raw content-type mapping must be configured manually via
            // WebMessageEncodingBindingElement.ContentTypeMapper if needed.
            if (this.Endpoint.Behaviors.Find<WebHttpBehavior>() == null)
            {
                this.Endpoint.Behaviors.Add(new WebHttpBehavior());
            }
            base.OnOpening();
        }

        static Binding GetDefaultBinding(Uri remoteAddress)
        {
            if (remoteAddress == null || (remoteAddress.Scheme != Uri.UriSchemeHttp && remoteAddress.Scheme != Uri.UriSchemeHttps))
            {
                return null;
            }
            if (remoteAddress.Scheme == Uri.UriSchemeHttp)
            {
                return new WebHttpBinding();
            }
            else
            {
                WebHttpBinding result = new WebHttpBinding();
                result.Security.Mode = WebHttpSecurityMode.Transport;
                result.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                return result;
            }
        }
    }
}
