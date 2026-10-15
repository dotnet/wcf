// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Shared plumbing for the MSMQ send-side channel factories: encoder
    // selection, buffer management, and a single disposal path so cleanup
    // cannot drift between the synchronous, APM and task-based close overloads.
    [SupportedOSPlatform("windows")]
    internal abstract class MsmqOutputChannelFactoryBase<TChannel> : ChannelFactoryBase<TChannel>
        where TChannel : class, IChannel
    {
        private readonly MsmqTransportBindingElement _bindingElement;
        private readonly MessageEncoderFactory _messageEncoderFactory;
        private readonly BufferManager _bufferManager;

        protected MsmqOutputChannelFactoryBase(MsmqTransportBindingElement bindingElement, BindingContext context)
            : base(GetBinding(context))
        {
            _bindingElement = bindingElement ?? throw new ArgumentNullException(nameof(bindingElement));

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

        // Every close/abort path funnels through here so that adding real
        // cleanup later (a queue-handle pool, for instance) cannot accidentally
        // run twice or be missed on one of the overloads.
        private void Cleanup() => _bufferManager?.Clear();

        protected override void OnAbort() => Cleanup();

        protected override void OnClose(TimeSpan timeout) => Cleanup();

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Cleanup();
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnCloseAsync(TimeSpan timeout)
        {
            Cleanup();
            return Task.CompletedTask;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;
    }

    internal static class TaskApmExtensions
    {
        internal static IAsyncResult ToApm(this Task task, AsyncCallback callback, object state)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(state, TaskCreationOptions.None);
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
