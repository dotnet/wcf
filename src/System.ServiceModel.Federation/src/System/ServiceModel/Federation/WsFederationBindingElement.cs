// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Federation
{
    internal class WSFederationBindingElement : BindingElement
    {
        public WSFederationBindingElement(WSTrustTokenParameters wsTrustTokenParameters, SecurityBindingElement securityBindingElement)
        {
            WSTrustTokenParameters = wsTrustTokenParameters;
            SecurityBindingElement = securityBindingElement;
        }

        public WSTrustTokenParameters WSTrustTokenParameters { get; }

        public SecurityBindingElement SecurityBindingElement { get; }

        public override BindingElement Clone()
        {
            return new WSFederationBindingElement(WSTrustTokenParameters, SecurityBindingElement);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return SecurityBindingElement.GetProperty<T>(context);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            WSTrustChannelClientCredentials trustCredentials = Find<WSTrustChannelClientCredentials>(context.BindingParameters);
            if (trustCredentials == null)
            {
                var clientCredentials = Find<ClientCredentials>(context.BindingParameters);
                if (clientCredentials != null)
                {
                    trustCredentials = new WSTrustChannelClientCredentials(clientCredentials);
                    context.BindingParameters.Remove(typeof(ClientCredentials));
                    context.BindingParameters.Add(trustCredentials);
                }
                else
                {
                    trustCredentials = new WSTrustChannelClientCredentials();
                    context.BindingParameters.Add(trustCredentials);
                }
            }

            var channelFactory = base.BuildChannelFactory<TChannel>(context);

            return channelFactory;
        }

        private T Find<T>(BindingParameterCollection bindingParameterCollection)
        {
            for (int i = 0; i < bindingParameterCollection.Count; i++)
            {
                object settings = bindingParameterCollection[i];
                if (settings is T)
                {
                    return (T)settings;
                }
            }

            return default(T);
        }
    }
}
