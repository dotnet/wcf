// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml;

namespace System.IdentityModel
{
    internal interface ISignatureValueSecurityElement : ISecurityElement
    {
        byte[] GetSignatureValue();
    }
}
