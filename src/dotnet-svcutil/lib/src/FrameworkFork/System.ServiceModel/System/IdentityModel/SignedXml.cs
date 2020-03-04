// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Text;
using Microsoft.Xml;
using System.ServiceModel;

namespace System.IdentityModel
{
    internal sealed class SignedXml : ISignatureValueSecurityElement
    {
        public byte[] GetSignatureValue()
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id
        {
            get { throw ExceptionHelper.PlatformNotSupported(); }
            set { throw ExceptionHelper.PlatformNotSupported(); }
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
    }
}
