// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Channels
{
    public abstract class ChannelBase : CommunicationObject, IChannel, IDefaultCommunicationTimeouts
    {
        private ChannelManagerBase _channelManager;

        protected ChannelBase(ChannelManagerBase channelManager)
        {
            if (channelManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelManager");
            }

            _channelManager = channelManager;
        }

        TimeSpan IDefaultCommunicationTimeouts.CloseTimeout
        {
            get { return this.DefaultCloseTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.OpenTimeout
        {
            get { return this.DefaultOpenTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout
        {
            get { return this.DefaultReceiveTimeout; }
        }

        TimeSpan IDefaultCommunicationTimeouts.SendTimeout
        {
            get { return this.DefaultSendTimeout; }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)_channelManager).CloseTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)_channelManager).OpenTimeout; }
        }

        protected TimeSpan DefaultReceiveTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)_channelManager).ReceiveTimeout; }
        }

        protected TimeSpan DefaultSendTimeout
        {
            get { return ((IDefaultCommunicationTimeouts)_channelManager).SendTimeout; }
        }

        protected ChannelManagerBase Manager
        {
            get
            {
                return _channelManager;
            }
        }

        public virtual T GetProperty<T>() where T : class
        {
            IChannelFactory factory = _channelManager as IChannelFactory;
            if (factory != null)
            {
                return factory.GetProperty<T>();
            }

            IChannelListener listener = _channelManager as IChannelListener;
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
