// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using FxResources.System.ServiceModel.Primitives;

namespace System.ServiceModel.Channels
{
    internal class TransactionChannelFaultConverter<TChannel> : FaultConverter
        where TChannel : class, IChannel
    {
        private TransactionChannel<TChannel> _channel;

        internal TransactionChannelFaultConverter(TransactionChannel<TChannel> channel)
        {
            _channel = channel;
        }

        protected override bool OnTryCreateException(Message message, MessageFault fault, out Exception exception)
        {
            if (message.Headers.Action == FaultCodeConstants.Actions.Transactions)
            {
                exception = new ProtocolException(fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text);
                return true;
            }

            if (fault.IsMustUnderstandFault)
            {
                MessageHeader header = _channel.Formatter.EmptyTransactionHeader;
                if (MessageFault.WasHeaderNotUnderstood(message.Headers, header.Name, header.Namespace))
                {
                    exception = new ProtocolException(SRP.Format(SRP.SFxTransactionHeaderNotUnderstood, header.Name, header.Namespace, _channel.Protocol));
                    return true;
                }
            }

            FaultConverter inner = _channel.GetInnerProperty<FaultConverter>();
            if (inner != null)
            {
                return inner.TryCreateException(message, fault, out exception);
            }
            else
            {
                exception = null;
                return false;
            }
        }

        protected override bool OnTryCreateFaultMessage(Exception exception, out Message message)
        {
            FaultConverter inner = _channel.GetInnerProperty<FaultConverter>();
            if (inner != null)
            {
                return inner.TryCreateFaultMessage(exception, out message);
            }
            else
            {
                message = null;
                return false;
            }
        }
    }
}
