// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.Net.Http;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class HttpsChannelFactory<TChannel> : HttpChannelFactory<TChannel>
    {
        private bool _requireClientCertificate;



        internal HttpsChannelFactory(HttpsTransportBindingElement httpsBindingElement, BindingContext context)
            : base(httpsBindingElement, context)
        {
            _requireClientCertificate = httpsBindingElement.RequireClientCertificate;
            ClientCredentials credentials = context.BindingParameters.Find<ClientCredentials>();
        }

        public override string Scheme
        {
            get
            {
                return UriEx.UriSchemeHttps;
            }
        }

        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
        }

        public override bool IsChannelBindingSupportEnabled
        {
            get
            {
                return false;
            }
        }

        public override T GetProperty<T>()
        {
            return base.GetProperty<T>();
        }


        protected override void ValidateCreateChannelParameters(EndpointAddress remoteAddress, Uri via)
        {
            base.ValidateCreateChannelParameters(remoteAddress, via);
        }

        protected override TChannel OnCreateChannelCore(EndpointAddress address, Uri via)
        {
            ValidateCreateChannelParameters(address, via);

            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new HttpsClientRequestChannel((HttpsChannelFactory<IRequestChannel>)(object)this, address, via, ManualAddressing);
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported("Channel shape not supported");
            }
        }

        protected override bool IsSecurityTokenManagerRequired()
        {
            return _requireClientCertificate || base.IsSecurityTokenManagerRequired();
        }


        private void OnOpenCore()
        {
            if (_requireClientCertificate)
            {
                throw ExceptionHelper.PlatformNotSupported("Client certificates");
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            base.OnEndOpen(result);
            OnOpenCore();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            base.OnOpen(timeout);
            OnOpenCore();
        }

        internal SecurityTokenProvider CreateAndOpenCertificateTokenProvider(EndpointAddress target, Uri via, ChannelParameterCollection channelParameters, TimeSpan timeout)
        {
            if (!this.RequireClientCertificate)
            {
                return null;
            }
            throw ExceptionHelper.PlatformNotSupported("Client certificates not supported");
        }





        protected class HttpsClientRequestChannel : HttpClientRequestChannel
        {
            private SecurityTokenProvider _certificateProvider;
            private HttpsChannelFactory<IRequestChannel> _factory;

            public HttpsClientRequestChannel(HttpsChannelFactory<IRequestChannel> factory, EndpointAddress to, Uri via, bool manualAddressing)
                : base(factory, to, via, manualAddressing)
            {
                _factory = factory;
            }

            public new HttpsChannelFactory<IRequestChannel> Factory
            {
                get { return _factory; }
            }

            private void CreateAndOpenTokenProvider(TimeSpan timeout)
            {
                if (!ManualAddressing && this.Factory.RequireClientCertificate)
                {
                    _certificateProvider = Factory.CreateAndOpenCertificateTokenProvider(this.RemoteAddress, this.Via, this.ChannelParameters, timeout);
                }
            }

            private void CloseTokenProvider(TimeSpan timeout)
            {
                if (_certificateProvider != null)
                {
                    SecurityUtils.CloseTokenProviderIfRequired(_certificateProvider, timeout);
                }
            }

            private void AbortTokenProvider()
            {
                if (_certificateProvider != null)
                {
                    SecurityUtils.AbortTokenProviderIfRequired(_certificateProvider);
                }
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CreateAndOpenTokenProvider(timeoutHelper.RemainingTime());
                return base.OnBeginOpen(timeoutHelper.RemainingTime(), callback, state);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CreateAndOpenTokenProvider(timeoutHelper.RemainingTime());
                base.OnOpen(timeoutHelper.RemainingTime());
            }

            protected override void OnAbort()
            {
                AbortTokenProvider();
                base.OnAbort();
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CloseTokenProvider(timeoutHelper.RemainingTime());
                return base.OnBeginClose(timeoutHelper.RemainingTime(), callback, state);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                CloseTokenProvider(timeoutHelper.RemainingTime());
                base.OnClose(timeoutHelper.RemainingTime());
            }

            internal override void OnHttpRequestCompleted(HttpRequestMessage request)
            {
            }
        }
    }
}
