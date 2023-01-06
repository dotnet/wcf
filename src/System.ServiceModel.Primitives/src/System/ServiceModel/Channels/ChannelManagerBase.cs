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
            get { return DefaultReceiveTimeout; }
        }

        internal TimeSpan InternalSendTimeout
        {
            get { return DefaultSendTimeout; }
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

        internal Exception CreateChannelTypeNotSupportedException(Type type)
        {
            return new ArgumentException(SRP.Format(SRP.ChannelTypeNotSupported, type), "TChannel");
        }
    }
}
