// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.Diagnostics;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal abstract class StreamSecurityUpgradeAcceptorBase : StreamSecurityUpgradeAcceptor
    {
        private SecurityMessageProperty _remoteSecurity;
        private bool _securityUpgraded;
        private string _upgradeString;
        private EventTraceActivity _eventTraceActivity;

        protected StreamSecurityUpgradeAcceptorBase(string upgradeString)
        {
            _upgradeString = upgradeString;
        }

        internal EventTraceActivity EventTraceActivity
        {
            get
            {
                if (_eventTraceActivity == null)
                {
                    _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                }
                return _eventTraceActivity;
            }
        }

        public override Stream AcceptUpgrade(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }

            Stream result = this.OnAcceptUpgrade(stream, out _remoteSecurity);
            _securityUpgraded = true;
            return result;
        }

        public override IAsyncResult BeginAcceptUpgrade(Stream stream, AsyncCallback callback, object state)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }

            return this.OnBeginAcceptUpgrade(stream, callback, state);
        }

        public override bool CanUpgrade(string contentType)
        {
            if (_securityUpgraded)
            {
                return false;
            }

            return (contentType == _upgradeString);
        }

        public override Stream EndAcceptUpgrade(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            Stream retValue = this.OnEndAcceptUpgrade(result, out _remoteSecurity);
            _securityUpgraded = true;
            return retValue;
        }

        public override SecurityMessageProperty GetRemoteSecurity()
        {
            // this could be null if upgrade not completed.
            return _remoteSecurity;
        }

        protected abstract Stream OnAcceptUpgrade(Stream stream, out SecurityMessageProperty remoteSecurity);
        protected abstract IAsyncResult OnBeginAcceptUpgrade(Stream stream, AsyncCallback callback, object state);
        protected abstract Stream OnEndAcceptUpgrade(IAsyncResult result,
            out SecurityMessageProperty remoteSecurity);
    }
}
