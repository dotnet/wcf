// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
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

        public override async Task<Stream> InitiateUpgradeAsync(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
            }

            if (!_isOpen)
            {
                await OpenAsync(TimeSpan.Zero);
            }

            Stream result;
            (result, _remoteSecurity) = await OnInitiateUpgradeAsync(stream);
            _securityUpgraded = true;
            return result;
        }

        internal override async ValueTask OpenAsync(TimeSpan timeout)
        {
            await base.OpenAsync(timeout);
            _isOpen = true;
        }

        internal override async ValueTask CloseAsync(TimeSpan timeout)
        {
            await base.CloseAsync(timeout);
            _isOpen = false;
        }

        protected abstract Task<(Stream upgradedStream, SecurityMessageProperty remoteSecurity)> OnInitiateUpgradeAsync(Stream stream);
    }
}
