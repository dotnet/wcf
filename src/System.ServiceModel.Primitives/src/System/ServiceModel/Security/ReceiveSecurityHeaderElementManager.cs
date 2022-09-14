// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal sealed class ReceiveSecurityHeaderElementManager
    {
        private const int InitialCapacity = 8;
        private readonly ReceiveSecurityHeader _securityHeader;
        private ReceiveSecurityHeaderEntry[] _elements;
        private readonly string[] _headerIds;

        public ReceiveSecurityHeaderElementManager(ReceiveSecurityHeader securityHeader)
        {
            _securityHeader = securityHeader;
            _elements = new ReceiveSecurityHeaderEntry[InitialCapacity];
            if (securityHeader.RequireMessageProtection)
            {
                _headerIds = new string[securityHeader.ProcessedMessage.Headers.Count];
            }
        }

        public int Count { get; private set; }

        public bool IsPrimaryTokenSigned { get; set; } = false;

        public void AppendElement(
            ReceiveSecurityHeaderElementCategory elementCategory, object element,
            ReceiveSecurityHeaderBindingModes bindingMode, string id, TokenTracker supportingTokenTracker)
        {
            if (id != null)
            {
                VerifyIdUniquenessInSecurityHeader(id);
            }
            EnsureCapacityToAdd();
            _elements[Count++].SetElement(elementCategory, element, bindingMode, id, false, null, supportingTokenTracker);
        }

        public void AppendTimestamp(SecurityTimestamp timestamp)
        {
            AppendElement(ReceiveSecurityHeaderElementCategory.Timestamp, timestamp,
                ReceiveSecurityHeaderBindingModes.Unknown, timestamp.Id, null);
        }

        public void AppendToken(SecurityToken token, ReceiveSecurityHeaderBindingModes mode, TokenTracker supportingTokenTracker)
        {
            AppendElement(ReceiveSecurityHeaderElementCategory.Token, token,
                mode, token.Id, supportingTokenTracker);
        }

        private void EnsureCapacityToAdd()
        {
            if (Count == _elements.Length)
            {
                ReceiveSecurityHeaderEntry[] newElements = new ReceiveSecurityHeaderEntry[_elements.Length * 2];
                Array.Copy(_elements, 0, newElements, 0, Count);
                _elements = newElements;
            }
        }

        public object GetElement(int index)
        {
            Fx.Assert(0 <= index && index < Count, "");
            return _elements[index]._element;
        }

        public void GetElementEntry(int index, out ReceiveSecurityHeaderEntry element)
        {
            Fx.Assert(0 <= index && index < Count, "index out of range");
            element = _elements[index];
        }

        public ReceiveSecurityHeaderElementCategory GetElementCategory(int index)
        {
            Fx.Assert(0 <= index && index < Count, "index out of range");
            return _elements[index]._elementCategory;
        }

        internal XmlDictionaryReader GetReader(int index, bool requiresEncryptedFormReader)
        {
            Fx.Assert(0 <= index && index < Count, "index out of range");
            if (!requiresEncryptedFormReader)
            {
                throw ExceptionHelper.PlatformNotSupported();
                //byte[] decryptedBuffer = elements[index].decryptedBuffer;
                //if (decryptedBuffer != null)
                //{
                //    return securityHeader.CreateDecryptedReader(decryptedBuffer);
                //}
            }
            XmlDictionaryReader securityHeaderReader = _securityHeader.CreateSecurityHeaderReader();
            securityHeaderReader.ReadStartElement();
            for (int i = 0; securityHeaderReader.IsStartElement() && i < index; i++)
            {
                securityHeaderReader.Skip();
            }
            return securityHeaderReader;
        }

        private void OnDuplicateId(string id)
        {
            throw TraceUtility.ThrowHelperError(
                new MessageSecurityException(SRP.Format(SRP.DuplicateIdInMessageToBeVerified, id)), _securityHeader.SecurityVerifiedMessage);
        }

        public void SetBindingMode(int index, ReceiveSecurityHeaderBindingModes bindingMode)
        {
            Fx.Assert(0 <= index && index < Count, "index out of range");
            _elements[index]._bindingMode = bindingMode;
        }

        public void SetElementAfterDecryption(
            int index,
            ReceiveSecurityHeaderElementCategory elementCategory, object element,
            ReceiveSecurityHeaderBindingModes bindingMode, string id, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
        {
            Fx.Assert(0 <= index && index < Count, "index out of range");
            Fx.Assert(_elements[index]._elementCategory == ReceiveSecurityHeaderElementCategory.EncryptedData, "Replaced item must be EncryptedData");
            if (id != null)
            {
                VerifyIdUniquenessInSecurityHeader(id);
            }
            _elements[index].PreserveIdBeforeDecryption();
            _elements[index].SetElement(elementCategory, element, bindingMode, id, true, decryptedBuffer, supportingTokenTracker);
        }

        public void SetTokenAfterDecryption(int index, SecurityToken token, ReceiveSecurityHeaderBindingModes mode, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
        {
            SetElementAfterDecryption(index, ReceiveSecurityHeaderElementCategory.Token, token, mode, token.Id, decryptedBuffer, supportingTokenTracker);
        }

        private void VerifyIdUniquenessInSecurityHeader(string id)
        {
            Fx.Assert(id != null, "Uniqueness should only be tested for non-empty ids");
            for (int i = 0; i < Count; i++)
            {
                if (_elements[i]._id == id || _elements[i]._encryptedFormId == id)
                {
                    OnDuplicateId(id);
                }
            }
        }
    }
}
