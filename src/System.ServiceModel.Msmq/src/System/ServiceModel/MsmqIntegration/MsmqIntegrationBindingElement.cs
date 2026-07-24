// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.Channels;

namespace System.ServiceModel.MsmqIntegration
{
    public sealed class MsmqIntegrationBindingElement : MsmqBindingElementBase
    {
        private MsmqMessageSerializationFormat _serializationFormat;
        private Type[] _targetSerializationTypes;

        public MsmqIntegrationBindingElement()
        {
            _serializationFormat = MsmqDefaults.MsmqMessageSerializationFormat;
        }

        private MsmqIntegrationBindingElement(MsmqIntegrationBindingElement other)
            : base(other)
        {
            _serializationFormat = other._serializationFormat;
            if (other._targetSerializationTypes != null)
            {
                _targetSerializationTypes = (Type[])other._targetSerializationTypes.Clone();
            }
        }

        public override string Scheme => "msmq.formatname";

        internal override MsmqUri.IAddressTranslator AddressTranslator
        {
            // Filled in by the slice that ports MsmqUri.FormatNameAddressTranslator.
            get { return null; }
        }

        public MsmqMessageSerializationFormat SerializationFormat
        {
            get { return _serializationFormat; }
            set
            {
                if (!MsmqMessageSerializationFormatHelper.IsDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _serializationFormat = value;
            }
        }

        public Type[] TargetSerializationTypes
        {
            get
            {
                return _targetSerializationTypes == null
                    ? null
                    : (Type[])_targetSerializationTypes.Clone();
            }
            set
            {
                _targetSerializationTypes = value == null
                    ? null
                    : (Type[])value.Clone();
            }
        }

        public override BindingElement Clone()
        {
            return new MsmqIntegrationBindingElement(this);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IOutputChannel);
        }

        [SupportedOSPlatform("windows")]
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (typeof(TChannel) != typeof(IOutputChannel))
            {
                throw new ArgumentException(SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)), "TChannel");
            }
            return (IChannelFactory<TChannel>)(object)new MsmqIntegrationOutputChannelFactory(this, context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageVersion.None;
            }
            return base.GetProperty<T>(context);
        }
    }
}
