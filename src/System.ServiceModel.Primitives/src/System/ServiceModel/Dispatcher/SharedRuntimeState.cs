// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Dispatcher
{
    internal class SharedRuntimeState
    {
        private bool _isImmutable;
        private bool _validateMustUnderstand = true;

        internal SharedRuntimeState(bool isOnServer)
        {
            IsOnServer = isOnServer;
        }

        internal bool EnableFaults { get; set; } = true;

        internal bool IsOnServer { get; }

        internal bool ManualAddressing { get; set; }

        internal bool ValidateMustUnderstand
        {
            get { return _validateMustUnderstand; }
            set { _validateMustUnderstand = value; }
        }

        internal void LockDownProperties()
        {
            _isImmutable = true;
        }

        internal void ThrowIfImmutable()
        {
            if (_isImmutable)
            {
                if (IsOnServer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxImmutableServiceHostBehavior0));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxImmutableChannelFactoryBehavior0));
                }
            }
        }
    }
}
