// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;

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

#pragma warning disable 0649 // Issue #31 in progress. Remove this once we do real implementation, this prevents "field is never assigned to" warning. 
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

        // create from scratch
        public DerivedKeySecurityToken(SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier, int length)
            : this(tokenToDerive, tokenToDeriveIdentifier, length, SecurityUtils.GenerateId())
        {
        }

        internal DerivedKeySecurityToken(SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier,
            int length, string id)
        {
            if (length != 16 && length != 24 && length != 32)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.Psha1KeyLengthInvalid, length * 8)));

            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //byte[] nonce = new byte[DefaultNonceLength];
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //rng.GetBytes(nonce);

            //Initialize(id, -1, 0, length, null, nonce, tokenToDerive, tokenToDeriveIdentifier, SecurityAlgorithms.Psha1KeyDerivation);
        }

        internal DerivedKeySecurityToken(int generation, int offset, int length,
            string label, int minNonceLength, SecurityToken tokenToDerive,
            SecurityKeyIdentifierClause tokenToDeriveIdentifier,
            string derivationAlgorithm, string id)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //byte[] nonce = new byte[minNonceLength];
            //RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            //rng.GetBytes(nonce);

            //Initialize(id, generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm);
        }

        // create from xml
        internal DerivedKeySecurityToken(int generation, int offset, int length,
            string label, byte[] nonce, SecurityToken tokenToDerive,
            SecurityKeyIdentifierClause tokenToDeriveIdentifier, string derivationAlgorithm, string id)
        {
            Initialize(id, generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm, false);
        }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.DerivedKeyNotInitialized));
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

        void Initialize(string id, int generation, int offset, int length, string label, byte[] nonce,
    SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier, string derivationAlgorithm)
        {
            Initialize(id, generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm, true);
        }

        void Initialize(string id, int generation, int offset, int length, string label, byte[] nonce,
            SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier, string derivationAlgorithm,
            bool initializeDerivedKey)
        {
            if (id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
            }
            if (tokenToDerive == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenToDerive");
            }
            if (tokenToDeriveIdentifier == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokentoDeriveIdentifier");
            }
            if (!SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, tokenToDerive))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.DerivedKeyCannotDeriveFromSecret)));
            }
            if (nonce == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("nonce");
            }
            if (length == -1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("length"));
            }
            if (offset == -1 && generation == -1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.DerivedKeyPosAndGenNotSpecified));
            }
            if (offset >= 0 && generation >= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.DerivedKeyPosAndGenBothSpecified));
            }

            _id = id;
            _label = label;
            _nonce = nonce;
            _length = length;
            _offset = offset;
            _generation = generation;
            _tokenToDerive = tokenToDerive;
            _tokenToDeriveIdentifier = tokenToDeriveIdentifier;
            _keyDerivationAlgorithm = derivationAlgorithm;

            if (initializeDerivedKey)
            {
                InitializeDerivedKey(_length);
            }
        }

        internal void InitializeDerivedKey(int maxKeyLength)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (_key != null)
            //{
            //    return;
            //}
            //if (_length > maxKeyLength)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.DerivedKeyLengthTooLong, _length, maxKeyLength));
            //}

            //_key = SecurityUtils.GenerateDerivedKey(_tokenToDerive, _keyDerivationAlgorithm,
            //    (_label != null ? Encoding.UTF8.GetBytes(_label) : DefaultLabel), _nonce, _length * 8,
            //    ((_offset >= 0) ? _offset : _generation * _length));
            //if ((_key == null) || (_key.Length == 0))
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.DerivedKeyCannotDeriveFromSecret));
            //}
            //List<SecurityKey> temp = new List<SecurityKey>(1);
            //temp.Add(new InMemorySymmetricSecurityKey(_key, false));
            //_securityKeys = temp.AsReadOnly();
        }

        internal void InitializeDerivedKey(ReadOnlyCollection<SecurityKey> securityKeys)
        {
            _key = ((SymmetricSecurityKey)securityKeys[0]).GetSymmetricKey();
            _securityKeys = securityKeys;
        }

        internal static void EnsureAcceptableOffset(int offset, int generation, int length, int maxOffset)
        {
            if (offset != -1)
            {
                if (offset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.DerivedKeyTokenOffsetTooHigh, offset, maxOffset)));
                }
            }
            else
            {
                int effectiveOffset = generation * length;
                if ((effectiveOffset < generation && effectiveOffset < length) || effectiveOffset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SR.Format(SR.DerivedKeyTokenGenerationAndLengthTooHigh, generation, length, maxOffset)));
                }
            }
        }
    }
}
