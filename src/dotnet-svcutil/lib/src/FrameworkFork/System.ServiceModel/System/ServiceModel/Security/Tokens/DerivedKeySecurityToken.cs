// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ServiceModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Xml;

namespace System.ServiceModel.Security.Tokens
{
    internal sealed class DerivedKeySecurityToken : SecurityToken
    {
        //        public const string DefaultLabel = "WS-SecureConversationWS-SecureConversation";
        private static readonly byte[] s_DefaultLabel = new byte[]
            {
                (byte)'W', (byte)'S', (byte)'-', (byte)'S', (byte)'e', (byte)'c', (byte)'u', (byte)'r', (byte)'e',
                (byte)'C', (byte)'o', (byte)'n', (byte)'v', (byte)'e', (byte)'r', (byte)'s', (byte)'a', (byte)'t', (byte)'i', (byte)'o', (byte)'n',
                (byte)'W', (byte)'S', (byte)'-', (byte)'S', (byte)'e', (byte)'c', (byte)'u', (byte)'r', (byte)'e',
                (byte)'C', (byte)'o', (byte)'n', (byte)'v', (byte)'e', (byte)'r', (byte)'s', (byte)'a', (byte)'t', (byte)'i', (byte)'o', (byte)'n'
            };

        public const int DefaultNonceLength = 16;
        public const int DefaultDerivedKeyLength = 32;

#pragma warning disable 0649 // Remove this once we do real implementation, this prevents "field is never assigned to" warning. 
        // fields are read from in this class, but lack of implemenation means we never assign them yet. 
        private string _id;
        private byte[] _key;
        private string _keyDerivationAlgorithm;
        private string _label;
        private int _length = -1;
        private byte[] _nonce;
        // either offset or generation must be specified.
        private int _offset = -1;
        private int _generation = -1;
        private SecurityToken _tokenToDerive;
        private SecurityKeyIdentifierClause _tokenToDeriveIdentifier;
        private ReadOnlyCollection<SecurityKey> _securityKeys;
#pragma warning restore 0649


        public override string Id
        {
            get { return _id; }
        }

        public override DateTime ValidFrom
        {
            get { return _tokenToDerive.ValidFrom; }
        }

        public override DateTime ValidTo
        {
            get { return _tokenToDerive.ValidTo; }
        }

        public string KeyDerivationAlgorithm
        {
            get { return _keyDerivationAlgorithm; }
        }

        public int Generation
        {
            get { return _generation; }
        }

        public string Label
        {
            get { return _label; }
        }

        public int Length
        {
            get { return _length; }
        }

        internal byte[] Nonce
        {
            get { return _nonce; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        internal SecurityToken TokenToDerive
        {
            get { return _tokenToDerive; }
        }

        internal SecurityKeyIdentifierClause TokenToDeriveIdentifier
        {
            get { return _tokenToDeriveIdentifier; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                if (_securityKeys == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.DerivedKeyNotInitialized));
                }
                return _securityKeys;
            }
        }

        public byte[] GetKeyBytes()
        {
            return SecurityUtils.CloneBuffer(_key);
        }

        public byte[] GetNonce()
        {
            return SecurityUtils.CloneBuffer(_nonce);
        }

        internal bool TryGetSecurityKeys(out ReadOnlyCollection<SecurityKey> keys)
        {
            keys = _securityKeys;
            return (keys != null);
        }


        internal static void EnsureAcceptableOffset(int offset, int generation, int length, int maxOffset)
        {
            if (offset != -1)
            {
                if (offset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(string.Format(SRServiceModel.DerivedKeyTokenOffsetTooHigh, offset, maxOffset)));
                }
            }
            else
            {
                int effectiveOffset = generation * length;
                if ((effectiveOffset < generation && effectiveOffset < length) || effectiveOffset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(string.Format(SRServiceModel.DerivedKeyTokenGenerationAndLengthTooHigh, generation, length, maxOffset)));
                }
            }
        }
    }
}
