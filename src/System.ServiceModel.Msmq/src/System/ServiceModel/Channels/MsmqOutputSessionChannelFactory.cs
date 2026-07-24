// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputSessionChannelFactory : ChannelFactoryBase<IOutputSessionChannel>
    {
        private readonly MsmqTransportBindingElement _bindingElement;
        private readonly MessageEncoderFactory _messageEncoderFactory;
        private readonly BufferManager _bufferManager;

        internal MsmqOutputSessionChannelFactory(MsmqTransportBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            _bindingElement = bindingElement ?? throw new ArgumentNullException(nameof(bindingElement));
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            MessageEncodingBindingElement encodingElement =
                context.BindingParameters.Find<MessageEncodingBindingElement>()
                ?? new BinaryMessageEncodingBindingElement();
            _messageEncoderFactory = encodingElement.CreateMessageEncoderFactory();
            _bufferManager = BufferManager.CreateBufferManager(
                bindingElement.MaxBufferPoolSize,
                (int)Math.Min(bindingElement.MaxReceivedMessageSize, int.MaxValue));
        }

        internal MsmqTransportBindingElement BindingElement => _bindingElement;
        internal MessageEncoder MessageEncoder => _messageEncoderFactory.Encoder;
        internal BufferManager BufferManager => _bufferManager;

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageEncoder.MessageVersion;
            }
            return base.GetProperty<T>();
        }

        protected override IOutputSessionChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            return new MsmqOutputSessionChannel(this, address, via ?? address.Uri);
        }

        protected override void OnAbort() => _bufferManager?.Clear();

        protected override void OnClose(TimeSpan timeout) => _bufferManager?.Clear();

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result)
        {
            _bufferManager?.Clear();
            result.ToApmEnd();
        }

        protected override void OnOpen(TimeSpan timeout) { }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override Task OnCloseAsync(TimeSpan timeout)
        {
            _bufferManager?.Clear();
            return Task.CompletedTask;
        }
    }
}
