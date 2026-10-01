// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Web
{
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

        // The .NET Framework string- and type-based constructors are omitted because
        // ChannelFactory<T> in dotnet/wcf does not expose matching base constructors.
        // Use WebChannelFactory(Binding, Uri) or WebChannelFactory(ServiceEndpoint) instead.

        public WebChannelFactory(Uri remoteAddress)
            : this(GetDefaultBinding(remoteAddress), remoteAddress)
        {
        }

        public WebChannelFactory(Binding binding, Uri remoteAddress)
            : base(binding, (remoteAddress != null) ? new EndpointAddress(remoteAddress) : (EndpointAddress)null)
        {
        }

        // The endpoint-configuration constructor with a remote address is omitted for the same reason.

        protected override void OnOpening()
        {
            if (Endpoint == null)
            {
                return;
            }

            // if the binding is missing, set up a default binding
            if (Endpoint.Binding == null && Endpoint.Address != null)
            {
                Endpoint.Binding = GetDefaultBinding(Endpoint.Address.Uri);
            }
            SetRawContentTypeMapperIfNecessary(Endpoint);
            if (Endpoint.Behaviors.Find<WebHttpBehavior>() == null)
            {
                Endpoint.Behaviors.Add(new WebHttpBehavior());
            }
            base.OnOpening();
        }

        private static void SetRawContentTypeMapperIfNecessary(ServiceEndpoint endpoint)
        {
            Binding binding = endpoint.Binding;
            if (binding == null)
            {
                return;
            }

            CustomBinding customBinding = new CustomBinding(binding);
            WebMessageEncodingBindingElement encodingElement =
                customBinding.Elements.Find<WebMessageEncodingBindingElement>();
            if (encodingElement == null || encodingElement.ContentTypeMapper != null)
            {
                return;
            }

            int streamOperationCount = 0;
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                if (!IsRawContentMapperCompatibleClientOperation(operation, ref streamOperationCount))
                {
                    return;
                }
            }

            if (streamOperationCount > 0)
            {
                encodingElement.ContentTypeMapper = RawContentTypeMapper.Instance;
                endpoint.Binding = customBinding;
            }
        }

        private static bool IsRawContentMapperCompatibleClientOperation(
            OperationDescription operation, ref int streamOperationCount)
        {
            return operation.Messages.Count <= 1 ||
                IsResponseStreamOrVoid(operation, ref streamOperationCount);
        }

        private static bool IsResponseStreamOrVoid(
            OperationDescription operation, ref int streamOperationCount)
        {
            if (operation.Messages.Count <= 1)
            {
                return true;
            }

            MessageDescription message = operation.Messages[1];
            if (WebHttpBehavior.IsTypedMessage(message) || WebHttpBehavior.IsUntypedMessage(message))
            {
                return false;
            }

            if (message.Body.Parts.Count == 0)
            {
                if (message.Body.ReturnValue == null || IsVoidPart(message.Body.ReturnValue.Type))
                {
                    return true;
                }

                if (message.Body.ReturnValue.Type == typeof(Stream))
                {
                    streamOperationCount++;
                    return true;
                }
            }

            return false;
        }

        private static bool IsVoidPart(Type type) => type == null || type == typeof(void);

        private static Binding GetDefaultBinding(Uri remoteAddress)
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
