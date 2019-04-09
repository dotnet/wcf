// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class StreamSecurityUpgradeInitiatorBase : StreamSecurityUpgradeInitiator
    {
        private SecurityMessageProperty _remoteSecurity;
        private bool _securityUpgraded;
        private string _nextUpgrade;
        private bool _isOpen;

        protected StreamSecurityUpgradeInitiatorBase(string upgradeString, EndpointAddress remoteAddress, Uri via)
        {
            RemoteAddress = remoteAddress;
            Via = via;
            _nextUpgrade = upgradeString;
        }

        protected EndpointAddress RemoteAddress { get; }

        protected Uri Via { get; }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
            }

            if (!_isOpen)
            {
                Open(TimeSpan.Zero);
            }

            Stream result = OnInitiateUpgrade(stream, out _remoteSecurity);
            _securityUpgraded = true;
            return result;
        }

        internal override async Task<Stream> InitiateUpgradeAsync(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
            }

            if (!_isOpen)
            {
                Open(TimeSpan.Zero);
            }

            var remoteSecurityWrapper = new OutWrapper<SecurityMessageProperty>();
            Stream result = await OnInitiateUpgradeAsync(stream, remoteSecurityWrapper);
            _remoteSecurity = remoteSecurityWrapper;
            _securityUpgraded = true;
            return result;
        }

        internal override void Open(TimeSpan timeout)
        {
            _isOpen = true;
        }

        internal override Task OpenAsync(TimeSpan timeout)
        {
            _isOpen = true;
            return Task.CompletedTask;
        }

        internal override void Close(TimeSpan timeout)
        {
            _isOpen = false;
        }

        internal override Task CloseAsync(TimeSpan timeout)
        {
            _isOpen = false;
            return Task.CompletedTask;
        }

        protected abstract Stream OnInitiateUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity);
        protected abstract Task<Stream> OnInitiateUpgradeAsync(Stream stream, OutWrapper<SecurityMessageProperty> remoteSecurity);
    }
}
