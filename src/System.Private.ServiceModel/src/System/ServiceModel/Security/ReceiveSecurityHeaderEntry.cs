// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    internal struct ReceiveSecurityHeaderEntry
    {
        internal ReceiveSecurityHeaderElementCategory _elementCategory;
        internal object _element;
        internal ReceiveSecurityHeaderBindingModes _bindingMode;
        internal string _id;
        internal string _encryptedFormId;
        internal bool _encrypted;
        internal byte[] _decryptedBuffer;
        internal TokenTracker _supportingTokenTracker;

        public void PreserveIdBeforeDecryption()
        {
            _encryptedFormId = _id;
        }

        public void SetElement(
            ReceiveSecurityHeaderElementCategory elementCategory, object element,
            ReceiveSecurityHeaderBindingModes bindingMode, string id, bool encrypted, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
        {
            _elementCategory = elementCategory;
            _element = element;
            _bindingMode = bindingMode;
            _encrypted = encrypted;
            _decryptedBuffer = decryptedBuffer;
            _supportingTokenTracker = supportingTokenTracker;
            _id = id;
        }
    }
}
