// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace System.ServiceModel.Channels
{
    public abstract class TransportChannelFactory<TChannel> : ChannelFactoryBase<TChannel>, ITransportFactorySettings
    {
        private BufferManager _bufferManager;
        private long _maxBufferPoolSize;
        private long _maxReceivedMessageSize;
        private MessageEncoderFactory _messageEncoderFactory;
        private bool _manualAddressing;
        private MessageVersion _messageVersion;

        protected TransportChannelFactory(TransportBindingElement bindingElement, BindingContext context)
            : this(bindingElement, context, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
        }

        protected TransportChannelFactory(TransportBindingElement bindingElement, BindingContext context,
                                          MessageEncoderFactory defaultMessageEncoderFactory)
            : base(context.Binding)
        {
            _manualAddressing = bindingElement.ManualAddressing;
            _maxBufferPoolSize = bindingElement.MaxBufferPoolSize;
            _maxReceivedMessageSize = bindingElement.MaxReceivedMessageSize;

            Collection<MessageEncodingBindingElement> messageEncoderBindingElements
                = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (messageEncoderBindingElements.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.MultipleMebesInParameters));
            }
            else if (messageEncoderBindingElements.Count == 1)
            {
                _messageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
                context.BindingParameters.Remove<MessageEncodingBindingElement>();
            }
            else
            {
                _messageEncoderFactory = defaultMessageEncoderFactory;
            }

            if (null != _messageEncoderFactory)
                _messageVersion = _messageEncoderFactory.MessageVersion;
            else
                _messageVersion = MessageVersion.None;
        }

        public BufferManager BufferManager
        {
            get
            {
                return _bufferManager;
            }
        }

        public long MaxBufferPoolSize
        {
            get
            {
                return _maxBufferPoolSize;
            }
        }

        public long MaxReceivedMessageSize
        {
            get
            {
                return _maxReceivedMessageSize;
            }
        }

        public MessageEncoderFactory MessageEncoderFactory
        {
            get
            {
                return _messageEncoderFactory;
            }
        }

        public MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        public bool ManualAddressing
        {
            get
            {
                return _manualAddressing;
            }
        }

        public abstract string Scheme { get; }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)this.MessageVersion;
            }

            if (typeof(T) == typeof(FaultConverter))
            {
                if (null == this.MessageEncoderFactory)
                    return null;
                else
                    return this.MessageEncoderFactory.Encoder.GetProperty<T>();
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

        private void OnCloseOrAbort()
        {
            if (_bufferManager != null)
            {
                _bufferManager.Clear();
            }
        }

        public virtual int GetMaxBufferSize()
        {
            if (MaxReceivedMessageSize > int.MaxValue)
                return int.MaxValue;
            else
                return (int)MaxReceivedMessageSize;
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            _bufferManager = BufferManager.CreateBufferManager(MaxBufferPoolSize, GetMaxBufferSize());
        }

        public void ValidateScheme(Uri via)
        {
            if (via.Scheme != this.Scheme)
            {
                // URI schemes are case-insensitive, so try a case insensitive compare now
                if (string.Compare(via.Scheme, this.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("via", string.Format(SRServiceModel.InvalidUriScheme,
                        via.Scheme, this.Scheme));
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
