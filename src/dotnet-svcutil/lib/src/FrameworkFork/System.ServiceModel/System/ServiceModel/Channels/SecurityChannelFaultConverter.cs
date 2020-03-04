// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    internal class SecurityChannelFaultConverter : FaultConverter
    {
        private IChannel _innerChannel;

        internal SecurityChannelFaultConverter(IChannel innerChannel)
        {
            _innerChannel = innerChannel;
        }

        protected override bool OnTryCreateException(Message message, MessageFault fault, out Exception exception)
        {
            if (_innerChannel == null)
            {
                exception = null;
                return false;
            }

            FaultConverter inner = _innerChannel.GetProperty<FaultConverter>();
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
            if (_innerChannel == null)
            {
                message = null;
                return false;
            }

            FaultConverter inner = _innerChannel.GetProperty<FaultConverter>();
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
