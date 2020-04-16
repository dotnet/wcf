// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    using ISignatureValueSecurityElement = System.IdentityModel.ISignatureValueSecurityElement;
    using DictionaryManager = System.IdentityModel.DictionaryManager;

    internal class SignatureConfirmationElement : ISignatureValueSecurityElement
    {
        private SecurityVersion _version;
        private string _id;
        private byte[] _signatureValue;

        public SignatureConfirmationElement(string id, byte[] signatureValue, SecurityVersion version)
        {
            if (id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
            }
            if (signatureValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signatureValue");
            }
            _id = id;
            _signatureValue = signatureValue;
            _version = version;
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id
        {
            get { return _id; }
        }

        public byte[] GetSignatureValue()
        {
            return _signatureValue;
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            _version.WriteSignatureConfirmation(writer, _id, _signatureValue);
        }
    }
}
