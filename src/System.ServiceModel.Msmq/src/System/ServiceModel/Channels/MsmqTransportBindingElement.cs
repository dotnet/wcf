// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    public sealed class MsmqTransportBindingElement : MsmqBindingElementBase
    {
        private int _maxPoolSize = MsmqDefaults.MaxPoolSize;
        private bool _useActiveDirectory = MsmqDefaults.UseActiveDirectory;
        private QueueTransferProtocol _queueTransferProtocol = MsmqDefaults.QueueTransferProtocol;

        public MsmqTransportBindingElement()
        {
        }

        private MsmqTransportBindingElement(MsmqTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _useActiveDirectory = elementToBeCloned._useActiveDirectory;
            _maxPoolSize = elementToBeCloned._maxPoolSize;
            _queueTransferProtocol = elementToBeCloned._queueTransferProtocol;
        }

        internal override MsmqUri.IAddressTranslator AddressTranslator
        {
            // Filled in by the slice that ports MsmqUri's per-protocol translators.
            get { return null; }
        }

        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.MsmqNonNegativeArgumentExpected);
                }
                _maxPoolSize = value;
            }
        }

        public QueueTransferProtocol QueueTransferProtocol
        {
            get { return _queueTransferProtocol; }
            set
            {
                if (!QueueTransferProtocolHelper.IsDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _queueTransferProtocol = value;
            }
        }

        public override string Scheme => "net.msmq";

        public bool UseActiveDirectory
        {
            get { return _useActiveDirectory; }
            set { _useActiveDirectory = value; }
        }

        public override BindingElement Clone()
        {
            return new MsmqTransportBindingElement(this);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IOutputChannel)
                || typeof(TChannel) == typeof(IOutputSessionChannel);
        }

        [SupportedOSPlatform("windows")]
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MsmqOutputChannelFactory(this, context);
            }
            if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MsmqOutputSessionChannelFactory(this, context);
            }
            throw new ArgumentException(SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)), "TChannel");
        }
    }
}
