// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.ServiceModel.Channels
{
    public abstract class ChannelBase : CommunicationObject, IChannel, IDefaultCommunicationTimeouts
    {
        protected ChannelBase(ChannelManagerBase channelManager)
        {
            Manager = channelManager ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelManager));
        }

        TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
        {
            get { return DefaultCloseTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
        {
            get { return DefaultOpenTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
        {
            get { return DefaultReceiveTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.SendTimeout
        {
            get { return DefaultSendTimeout; }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)Manager).CloseTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)Manager).OpenTimeout; }
        }

        protected TimeSpan DefaultReceiveTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)Manager).ReceiveTimeout; }
        }

        protected TimeSpan DefaultSendTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)Manager).SendTimeout; }
        }

        protected ChannelManagerBase Manager { get; }

        public virtual T GetProperty<T>() where T : class
        {
            IChannelFactory factory = Manager as IChannelFactory;
            if (factory != null)
            {
                return factory.GetProperty<T>();
            }

            IChannelListener listener = Manager as IChannelListener;
            if (listener != null)
            {
                return listener.GetProperty<T>();
            }

            return null;
        }

        protected override void OnClosed()
        {
            base.OnClosed();
        }
    }
}
