// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Send-side channel factory backed by MSMQ.Messaging.MessageQueue.
    // This is intentionally minimal: it supports IOutputChannel for
    // NetMsmqBinding (binary-encoded SOAP messages going into a single
    // queue). IOutputSessionChannel, transactional retries, message
    // pooling, and the receive side land in later slices.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputChannelFactory : ChannelFactoryBase<IOutputChannel>
    {
        private readonly MsmqTransportBindingElement _bindingElement;
        private readonly MessageEncoderFactory _messageEncoderFactory;
        private readonly BufferManager _bufferManager;

        internal MsmqOutputChannelFactory(MsmqTransportBindingElement bindingElement, BindingContext context)
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

        protected override IOutputChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            return new MsmqOutputChannel(this, address, via ?? address.Uri);
        }

        protected override void OnAbort()
        {
            _bufferManager?.Clear();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _bufferManager?.Clear();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _bufferManager?.Clear();
            result.ToApmEnd();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override Task OnCloseAsync(TimeSpan timeout)
        {
            _bufferManager?.Clear();
            return Task.CompletedTask;
        }
    }

    internal static class TaskApmExtensions
    {
        internal static IAsyncResult ToApm(this Task task, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<bool>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(true);
                }
                callback?.Invoke(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        internal static void ToApmEnd(this IAsyncResult result)
        {
            ((Task)result).GetAwaiter().GetResult();
        }
    }
}
