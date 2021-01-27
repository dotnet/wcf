// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    public abstract class ChannelManagerBase : CommunicationObject, IDefaultCommunicationTimeouts
    {
        protected ChannelManagerBase()
        {
        }

        protected abstract TimeSpan DefaultReceiveTimeout { get; }
        protected abstract TimeSpan DefaultSendTimeout { get; }

        internal TimeSpan InternalReceiveTimeout
        {
            get { return this.DefaultReceiveTimeout; }
        }

        internal TimeSpan InternalSendTimeout
        {
            get { return this.DefaultSendTimeout; }
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

        internal Exception CreateChannelTypeNotSupportedException(Type type)
        {
            return new ArgumentException(string.Format(SRServiceModel.ChannelTypeNotSupported, type), "TChannel");
        }
    }
}
