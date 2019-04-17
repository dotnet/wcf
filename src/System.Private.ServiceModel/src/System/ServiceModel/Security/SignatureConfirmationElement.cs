// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel.Security
{
    using ISignatureValueSecurityElement = IdentityModel.ISignatureValueSecurityElement;
    using DictionaryManager = IdentityModel.DictionaryManager;

    internal class SignatureConfirmationElement : ISignatureValueSecurityElement
    {
        private SecurityVersion _version;
        private byte[] _signatureValue;

        public SignatureConfirmationElement(string id, byte[] signatureValue, SecurityVersion version)
        {
            Id = id ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(id));
            _signatureValue = signatureValue ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(signatureValue));
            _version = version;
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id { get; }

        public byte[] GetSignatureValue()
        {
            return _signatureValue;
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            _version.WriteSignatureConfirmation(writer, Id, _signatureValue);
        }
    }
}
