// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    internal struct ReceiveSecurityHeaderEntry
    {
        internal ReceiveSecurityHeaderElementCategory elementCategory;
        internal object element;
        internal ReceiveSecurityHeaderBindingModes bindingMode;
        internal string id;
        internal string encryptedFormId;
        internal bool encrypted;
        internal byte[] decryptedBuffer;
        internal TokenTracker supportingTokenTracker;

        public void PreserveIdBeforeDecryption()
        {
            this.encryptedFormId = this.id;
        }

        public void SetElement(
            ReceiveSecurityHeaderElementCategory elementCategory, object element,
            ReceiveSecurityHeaderBindingModes bindingMode, string id, bool encrypted, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
        {
            this.elementCategory = elementCategory;
            this.element = element;
            this.bindingMode = bindingMode;
            this.encrypted = encrypted;
            this.decryptedBuffer = decryptedBuffer;
            this.supportingTokenTracker = supportingTokenTracker;
            this.id = id;
        }
    }
}
