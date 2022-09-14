// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class TransportChannelFactory<TChannel> : ChannelFactoryBase<TChannel>, ITransportFactorySettings
    {
        private long _maxReceivedMessageSize;
        private MessageVersion _messageVersion;

        protected TransportChannelFactory(TransportBindingElement bindingElement, BindingContext context)
            : this(bindingElement, context, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
        }

        protected TransportChannelFactory(TransportBindingElement bindingElement, BindingContext context,
                                          MessageEncoderFactory defaultMessageEncoderFactory)
            : base(context.Binding)
        {
            ManualAddressing = bindingElement.ManualAddressing;
            MaxBufferPoolSize = bindingElement.MaxBufferPoolSize;
            _maxReceivedMessageSize = bindingElement.MaxReceivedMessageSize;

            Collection<MessageEncodingBindingElement> messageEncoderBindingElements
                = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (messageEncoderBindingElements.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.MultipleMebesInParameters));
            }
            else if (messageEncoderBindingElements.Count == 1)
            {
                MessageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
                context.BindingParameters.Remove<MessageEncodingBindingElement>();
            }
            else
            {
                MessageEncoderFactory = defaultMessageEncoderFactory;
            }

            if (null != MessageEncoderFactory)
            {
                _messageVersion = MessageEncoderFactory.MessageVersion;
            }
            else
            {
                _messageVersion = MessageVersion.None;
            }
        }

        public BufferManager BufferManager { get; private set; }

        public long MaxBufferPoolSize { get; }

        public long MaxReceivedMessageSize
        {
            get
            {
                return _maxReceivedMessageSize;
            }
        }

        public MessageEncoderFactory MessageEncoderFactory { get; }

        public MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        public bool ManualAddressing { get; }

        public abstract string Scheme { get; }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageVersion;
            }

            if (typeof(T) == typeof(FaultConverter))
            {
                if (null == MessageEncoderFactory)
                {
                    return null;
                }
                else
                {
                    return MessageEncoderFactory.Encoder.GetProperty<T>();
                }
            }

            if (typeof(T) == typeof(ITransportFactorySettings))
            {
                return (T)(object)this;
            }

            return base.GetProperty<T>();
        }


        protected override void OnAbort()
        {
            OnCloseOrAbort();
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OnCloseOrAbort();
            return base.OnBeginClose(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            OnCloseOrAbort();
            base.OnClose(timeout);
        }

        internal protected override Task OnCloseAsync(TimeSpan timeout)
        {
            OnCloseOrAbort();
            return base.OnCloseAsync(timeout);
        }

        private void OnCloseOrAbort()
        {
            if (BufferManager != null)
            {
                BufferManager.Clear();
            }
        }

        public virtual int GetMaxBufferSize()
        {
            if (MaxReceivedMessageSize > int.MaxValue)
            {
                return int.MaxValue;
            }
            else
            {
                return (int)MaxReceivedMessageSize;
            }
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            BufferManager = BufferManager.CreateBufferManager(MaxBufferPoolSize, GetMaxBufferSize());
        }

        public void ValidateScheme(Uri via)
        {
            if (via.Scheme != Scheme)
            {
                // URI schemes are case-insensitive, so try a case insensitive compare now
                if (string.Compare(via.Scheme, Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("via", SRP.Format(SRP.InvalidUriScheme,
                        via.Scheme, Scheme));
                }
            }
        }

        long ITransportFactorySettings.MaxReceivedMessageSize
        {
            get { return MaxReceivedMessageSize; }
        }

        BufferManager ITransportFactorySettings.BufferManager
        {
            get { return BufferManager; }
        }

        bool ITransportFactorySettings.ManualAddressing
        {
            get { return ManualAddressing; }
        }

        MessageEncoderFactory ITransportFactorySettings.MessageEncoderFactory
        {
            get { return MessageEncoderFactory; }
        }
    }
}
