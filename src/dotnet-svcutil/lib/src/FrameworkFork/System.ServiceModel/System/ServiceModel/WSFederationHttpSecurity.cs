// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.Runtime;
    using System.ServiceModel.Channels;
    using System.ComponentModel;

    public sealed class WSFederationHttpSecurity
    {
        internal const WSFederationHttpSecurityMode DefaultMode = WSFederationHttpSecurityMode.Message;

        private WSFederationHttpSecurityMode _mode;
        private FederatedMessageSecurityOverHttp _messageSecurity;

        public WSFederationHttpSecurity()
            : this(DefaultMode, new FederatedMessageSecurityOverHttp())
        {
        }

        private WSFederationHttpSecurity(WSFederationHttpSecurityMode mode, FederatedMessageSecurityOverHttp messageSecurity)
        {
            Fx.Assert(WSFederationHttpSecurityModeHelper.IsDefined(mode), string.Format("Invalid WSFederationHttpSecurityMode value: {0}", mode.ToString()));

            _mode = mode;
            _messageSecurity = messageSecurity == null ? new FederatedMessageSecurityOverHttp() : messageSecurity;
        }

        public WSFederationHttpSecurityMode Mode
        {
            get { return _mode; }
            set
            {
                if (!WSFederationHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _mode = value;
            }
        }

        public FederatedMessageSecurityOverHttp Message
        {
            get { return _messageSecurity; }
            set { _messageSecurity = value; }
        }

        internal SecurityBindingElement CreateMessageSecurity(bool isReliableSessionEnabled, MessageSecurityVersion version)
        {
            if (_mode == WSFederationHttpSecurityMode.Message || _mode == WSFederationHttpSecurityMode.TransportWithMessageCredential)
            {
                return _messageSecurity.CreateSecurityBindingElement(this.Mode == WSFederationHttpSecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, version);
            }
            else
            {
                return null;
            }
        }

        internal static bool TryCreate(SecurityBindingElement sbe,
                                       WSFederationHttpSecurityMode mode,
                                       HttpTransportSecurity transportSecurity,
                                       bool isReliableSessionEnabled,
                                       MessageSecurityVersion version,
                                       out WSFederationHttpSecurity security)
        {
            security = null;
            FederatedMessageSecurityOverHttp messageSecurity = null;
            if (sbe == null)
            {
                mode = WSFederationHttpSecurityMode.None;
            }
            else
            {
                mode &= WSFederationHttpSecurityMode.Message | WSFederationHttpSecurityMode.TransportWithMessageCredential;
                Fx.Assert(WSFederationHttpSecurityModeHelper.IsDefined(mode), string.Format("Invalid WSFederationHttpSecurityMode value: {0}", mode.ToString()));

                if (!FederatedMessageSecurityOverHttp.TryCreate(sbe, mode == WSFederationHttpSecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, version, out messageSecurity))
                    return false;
            }
            security = new WSFederationHttpSecurity(mode, messageSecurity);
            return true;
        }

        internal bool InternalShouldSerialize()
        {
            return this.ShouldSerializeMode()
                || this.ShouldSerializeMessage();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMode()
        {
            return this.Mode != DefaultMode;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessage()
        {
            return this.Message.InternalShouldSerialize();
        }
    }
}
