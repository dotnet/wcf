// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
