// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace System.ServiceModel.MsmqIntegration
{
    // Send-side channel factory for MsmqIntegrationBinding.
    //
    // Unlike the NetMsmqBinding factories this one holds no MessageEncoder.
    // The binding's MessageVersion is None and the payload is a raw MSMQ body
    // produced by MsmqIntegrationSerializer, so running the message through a
    // SOAP encoder would both corrupt the payload and throw on the version
    // mismatch.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqIntegrationOutputChannelFactory : ChannelFactoryBase<IOutputChannel>
    {
        private readonly MsmqIntegrationBindingElement _bindingElement;
        private readonly MsmqIntegrationSerializer _serializer;

        internal MsmqIntegrationOutputChannelFactory(MsmqIntegrationBindingElement bindingElement, BindingContext context)
            : base(GetBinding(context))
        {
            _bindingElement = bindingElement ?? throw new ArgumentNullException(nameof(bindingElement));
            _serializer = new MsmqIntegrationSerializer(bindingElement.SerializationFormat);
        }

        internal MsmqIntegrationBindingElement BindingElement => _bindingElement;

        internal MsmqIntegrationSerializer Serializer => _serializer;

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageVersion.None;
            }
            return base.GetProperty<T>();
        }

        // Validates the argument before it reaches the base constructor, which
        // would otherwise dereference it and raise NullReferenceException
        // instead of ArgumentNullException.
        private static IDefaultCommunicationTimeouts GetBinding(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.Binding;
        }

        protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            return new MsmqIntegrationOutputChannel(this, address, via ?? address.Uri);
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override Task OnCloseAsync(TimeSpan timeout) => Task.CompletedTask;
    }
}
