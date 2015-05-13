// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal abstract class StreamSecurityUpgradeInitiatorBase : StreamSecurityUpgradeInitiator
    {
        private EndpointAddress _remoteAddress;
        private Uri _via;
        private SecurityMessageProperty _remoteSecurity;
        private bool _securityUpgraded;
        private string _nextUpgrade;
        private bool _isOpen;

        protected StreamSecurityUpgradeInitiatorBase(string upgradeString, EndpointAddress remoteAddress, Uri via)
        {
            _remoteAddress = remoteAddress;
            _via = via;
            _nextUpgrade = upgradeString;
        }

        protected EndpointAddress RemoteAddress
        {
            get
            {
                return _remoteAddress;
            }
        }

        protected Uri Via
        {
            get
            {
                return _via;
            }
        }

        public override IAsyncResult BeginInitiateUpgrade(Stream stream, AsyncCallback callback, object state)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }

            if (!_isOpen)
            {
                this.Open(TimeSpan.Zero);
            }

            return this.OnBeginInitiateUpgrade(stream, callback, state);
        }

        public override Stream EndInitiateUpgrade(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            Stream retValue = this.OnEndInitiateUpgrade(result, out _remoteSecurity);
            _securityUpgraded = true;
            return retValue;
        }

        public override string GetNextUpgrade()
        {
            string result = _nextUpgrade;
            _nextUpgrade = null;
            return result;
        }

        public override SecurityMessageProperty GetRemoteSecurity()
        {
            if (!_securityUpgraded)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.OperationInvalidBeforeSecurityNegotiation));
            }

            return _remoteSecurity;
        }

        public override Stream InitiateUpgrade(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }

            if (!_isOpen)
            {
                this.Open(TimeSpan.Zero);
            }

            Stream result = this.OnInitiateUpgrade(stream, out _remoteSecurity);
            _securityUpgraded = true;
            return result;
        }

        internal override void EndOpen(IAsyncResult result)
        {
            base.EndOpen(result);
            _isOpen = true;
        }

        internal override void Open(TimeSpan timeout)
        {
            base.Open(timeout);
            _isOpen = true;
        }

        internal override void EndClose(IAsyncResult result)
        {
            base.EndClose(result);
            _isOpen = false;
        }

        internal override void Close(TimeSpan timeout)
        {
            base.Close(timeout);
            _isOpen = false;
        }

        protected abstract IAsyncResult OnBeginInitiateUpgrade(Stream stream, AsyncCallback callback, object state);
        protected abstract Stream OnEndInitiateUpgrade(IAsyncResult result,
            out SecurityMessageProperty remoteSecurity);
        protected abstract Stream OnInitiateUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity);
    }
}
