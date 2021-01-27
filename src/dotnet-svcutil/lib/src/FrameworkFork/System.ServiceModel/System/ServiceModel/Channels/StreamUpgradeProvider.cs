// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    public abstract class StreamUpgradeProvider : CommunicationObject, IAsyncCommunicationObject
    {
        private TimeSpan _closeTimeout;
        private TimeSpan _openTimeout;

        protected StreamUpgradeProvider()
            : this(null)
        {
        }

        protected StreamUpgradeProvider(IDefaultCommunicationTimeouts timeouts)
        {
            if (timeouts != null)
            {
                _closeTimeout = timeouts.CloseTimeout;
                _openTimeout = timeouts.OpenTimeout;
            }
            else
            {
                _closeTimeout = ServiceDefaults.CloseTimeout;
                _openTimeout = ServiceDefaults.OpenTimeout;
            }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return _closeTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return _closeTimeout; }
        }

        public virtual T GetProperty<T>() where T : class
        {
            return null;
        }

        public abstract StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via);
        public abstract StreamUpgradeAcceptor CreateUpgradeAcceptor();
    }
}
