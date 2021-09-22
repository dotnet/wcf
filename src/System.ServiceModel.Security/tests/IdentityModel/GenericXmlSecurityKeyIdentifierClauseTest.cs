// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class GenericXmlSecurityKeyIdentifierClauseTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new GenericXmlSecurityKeyIdentifierClause(null));
        Assert.Equal("referenceXml", exception.ParamName);
        XmlElement tokenReference = CreateTokenReference(Guid.NewGuid());
        var genericXmlSecurityKeyIdentifierClause = new GenericXmlSecurityKeyIdentifierClause(tokenReference);
        Assert.Null(genericXmlSecurityKeyIdentifierClause.Id);
        Assert.False(genericXmlSecurityKeyIdentifierClause.CanCreateKey);
        Assert.Null(genericXmlSecurityKeyIdentifierClause.ClauseType);
        Assert.Throws<NotSupportedException>(() => genericXmlSecurityKeyIdentifierClause.CreateKey());
        Assert.Null(genericXmlSecurityKeyIdentifierClause.GetDerivationNonce());
        Assert.Equal(0, genericXmlSecurityKeyIdentifierClause.DerivationLength);
        Assert.Equal(tokenReference, genericXmlSecurityKeyIdentifierClause.ReferenceXml);
    }

    [WcfFact]
    public static void Ctor_Deriviation_Properties()
    {
        XmlElement tokenReference = CreateTokenReference(Guid.NewGuid());
        byte[] derivationNonce = new byte[] { 1, 2, 3, 4, 5, 6 };
        int derivationLength = 128;
        var genericXmlSecurityKeyIdentifierClause = new GenericXmlSecurityKeyIdentifierClause(tokenReference, (byte[])derivationNonce.Clone(), derivationLength);
        Assert.Equal(derivationNonce, genericXmlSecurityKeyIdentifierClause.GetDerivationNonce());
        Assert.Equal(derivationLength, genericXmlSecurityKeyIdentifierClause.DerivationLength);
    }

    [WcfFact]
    public static void Matches()
    {
        Guid guid = Guid.NewGuid();
        XmlElement tokenReference = CreateTokenReference(guid);
        var genericXmlSecurityKeyIdentifierClause = new GenericXmlSecurityKeyIdentifierClause(tokenReference);
        tokenReference = CreateTokenReference(guid); // Create equivalant but different instance
        var genericXmlSecurityKeyIdentifierClause2 = new GenericXmlSecurityKeyIdentifierClause(tokenReference);
        Assert.True(genericXmlSecurityKeyIdentifierClause.Matches(genericXmlSecurityKeyIdentifierClause2));
        tokenReference = CreateTokenReference(Guid.NewGuid());
        genericXmlSecurityKeyIdentifierClause2 = new GenericXmlSecurityKeyIdentifierClause(tokenReference);
        Assert.False(genericXmlSecurityKeyIdentifierClause.Matches(genericXmlSecurityKeyIdentifierClause2));
    }

    private static XmlElement CreateTokenReference(Guid guid)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement tokenReference = doc.CreateElement("wsse", "KeyIdentifier", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        XmlAttribute attr = doc.CreateAttribute("ValueType");
        attr.Value = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID";
        tokenReference.Attributes.Append(attr);
        tokenReference.InnerText = "_" + guid.ToString("D");
        return tokenReference;
    }
}
