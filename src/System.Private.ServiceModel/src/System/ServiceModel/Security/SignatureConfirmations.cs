// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.ServiceModel.Security
{
    class SignatureConfirmations
    {
        private SignatureConfirmation[] _confirmations;
        private int _length;
        private bool _encrypted;

        struct SignatureConfirmation
        {
            public byte[] value;

            public SignatureConfirmation(byte[] value)
            {
                this.value = value;
            }
        }

        public SignatureConfirmations()
        {
            _confirmations = new SignatureConfirmation[1];
            _length = 0;
        }

        public int Count
        {
            get { return _length; }
        }

        public void AddConfirmation(byte[] value, bool encrypted)
        {
            if (_confirmations.Length == _length)
            {
                SignatureConfirmation[] newConfirmations = new SignatureConfirmation[_length * 2];
                Array.Copy(_confirmations, 0, newConfirmations, 0, _length);
                _confirmations = newConfirmations;
            }
            _confirmations[_length] = new SignatureConfirmation(value);
            ++_length;
            _encrypted |= encrypted;
        }

        public void GetConfirmation(int index, out byte[] value, out bool encrypted)
        {
            if (index < 0 || index >= _length)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", SR.Format(SR.ValueMustBeInRange, 0, _length)));
            }

            value = _confirmations[index].value;
            encrypted = _encrypted;
        }

        public bool IsMarkedForEncryption
        {
            get { return _encrypted; }
        }
    }
}

