// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Runtime;

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

        // fields are read from in this class, but lack of implemenation means we never assign them yet. 
        private string _id;
        private byte[] _key;
        private string _label;
        private byte[] _nonce;
        private ReadOnlyCollection<SecurityKey> _securityKeys;

        internal DerivedKeySecurityToken(int generation, int offset, int length,
            string label, int minNonceLength, SecurityToken tokenToDerive,
            SecurityKeyIdentifierClause tokenToDeriveIdentifier,
            string derivationAlgorithm, string id)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(minNonceLength);
            Fx.Assert(nonce.Length == minNonceLength, "Returned random bytes for nonce is not the expected length");
            Initialize(id, generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm);
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
            get { return TokenToDerive.ValidFrom; }
        }

        public override DateTime ValidTo
        {
            get { return TokenToDerive.ValidTo; }
        }

        public string KeyDerivationAlgorithm { get; private set; }

        public int Generation { get; private set; } = -1;

        public string Label
        {
            get { return _label; }
        }

        public int Length { get; private set; } = -1;

        internal byte[] Nonce
        {
            get { return _nonce; }
        }

        public int Offset { get; private set; } = -1;

        internal SecurityToken TokenToDerive { get; private set; }

        internal SecurityKeyIdentifierClause TokenToDeriveIdentifier { get; private set; }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                if (_securityKeys == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.DerivedKeyNotInitialized));
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

        public override string ToString()
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            writer.WriteLine("DerivedKeySecurityToken:");
            writer.WriteLine("   Generation: {0}", Generation);
            writer.WriteLine("   Offset: {0}", Offset);
            writer.WriteLine("   Length: {0}", Length);
            writer.WriteLine("   Label: {0}", Label);
            writer.WriteLine("   Nonce: {0}", Convert.ToBase64String(Nonce));
            writer.WriteLine("   TokenToDeriveFrom:");
            using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.Formatting = Formatting.Indented;
                SecurityStandardsManager.DefaultInstance.SecurityTokenSerializer.WriteKeyIdentifierClause(XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter), TokenToDeriveIdentifier);
            }
            return writer.ToString();
        }

        private void Initialize(string id, int generation, int offset, int length, string label, byte[] nonce,
    SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier, string derivationAlgorithm)
        {
            Initialize(id, generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm, true);
        }

        private void Initialize(string id, int generation, int offset, int length, string label, byte[] nonce,
    SecurityToken tokenToDerive, SecurityKeyIdentifierClause tokenToDeriveIdentifier, string derivationAlgorithm,
    bool initializeDerivedKey)
        {
            if (tokenToDerive == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenToDerive));
            }

            if (!SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, tokenToDerive))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.DerivedKeyCannotDeriveFromSecret));
            }

            if (length == -1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(length)));
            }
            if (offset == -1 && generation == -1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.DerivedKeyPosAndGenNotSpecified);
            }
            if (offset >= 0 && generation >= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.DerivedKeyPosAndGenBothSpecified);
            }

            _id = id ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(id));
            _label = label;
            _nonce = nonce ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(nonce));
            Length = length;
            Offset = offset;
            Generation = generation;
            TokenToDerive = tokenToDerive;
            TokenToDeriveIdentifier = tokenToDeriveIdentifier ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenToDeriveIdentifier));
            KeyDerivationAlgorithm = derivationAlgorithm;

            if (initializeDerivedKey)
            {
                InitializeDerivedKey(Length);
            }
        }

        internal void InitializeDerivedKey(int maxKeyLength)
        {
            if (_key != null)
            {
                return;
            }
            if (Length > maxKeyLength)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.DerivedKeyLengthTooLong, Length, maxKeyLength));
            }

            _key = SecurityUtils.GenerateDerivedKey(TokenToDerive, KeyDerivationAlgorithm,
                (_label != null ? Encoding.UTF8.GetBytes(_label) : s_DefaultLabel), _nonce, Length * 8,
                ((Offset >= 0) ? Offset : Generation * Length));
            if ((_key == null) || (_key.Length == 0))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.DerivedKeyCannotDeriveFromSecret);
            }
            List<SecurityKey> temp = new List<SecurityKey>(1);
            temp.Add(new InMemorySymmetricSecurityKey(_key, false));
            _securityKeys = temp.AsReadOnly();
        }

        internal static void EnsureAcceptableOffset(int offset, int generation, int length, int maxOffset)
        {
            if (offset != -1)
            {
                if (offset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.DerivedKeyTokenOffsetTooHigh, offset, maxOffset)));
                }
            }
            else
            {
                int effectiveOffset = generation * length;
                if ((effectiveOffset < generation && effectiveOffset < length) || effectiveOffset > maxOffset)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.DerivedKeyTokenGenerationAndLengthTooHigh, generation, length, maxOffset)));
                }
            }
        }
    }
}
