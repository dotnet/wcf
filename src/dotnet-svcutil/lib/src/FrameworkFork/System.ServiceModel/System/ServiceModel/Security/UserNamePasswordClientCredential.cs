// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security
{
    public sealed class UserNamePasswordClientCredential
    {
        private string _userName;
        private string _password;
        private bool _isReadOnly;

        internal UserNamePasswordClientCredential()
        {
            // empty
        }

        internal UserNamePasswordClientCredential(UserNamePasswordClientCredential other)
        {
            _userName = other._userName;
            _password = other._password;
            _isReadOnly = other._isReadOnly;
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                ThrowIfImmutable();
                _userName = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                ThrowIfImmutable();
                _password = value;
            }
        }

        internal void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        private void ThrowIfImmutable()
        {
            if (_isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
            }
        }
    }
}
