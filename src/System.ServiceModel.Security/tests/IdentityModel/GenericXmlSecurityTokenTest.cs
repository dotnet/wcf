// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class GenericXmlSecurityTokenTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        XmlDocument doc = new XmlDocument();
        XmlElement tokenXml = doc.CreateElement("ElementName");
        XmlAttribute attr = doc.CreateAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        string attrValue = "Value for Attribute Name Id";
        attr.Value = attrValue;
        tokenXml.Attributes.Append(attr);
        UserNameSecurityToken proofToken = new UserNameSecurityToken("user", "password");
        
        GenericXmlSecurityToken gxst = new GenericXmlSecurityToken(tokenXml, proofToken, DateTime.UtcNow, DateTime.MaxValue, null, null, null);
        Assert.NotNull(gxst);
        Assert.NotNull(gxst.Id);
        Assert.Equal(attrValue, gxst.Id);
        Assert.Equal(DateTime.UtcNow.Date, gxst.ValidFrom.Date);
        Assert.Equal(DateTime.MaxValue.Date, gxst.ValidTo.Date);
        Assert.NotNull(gxst.SecurityKeys);
        Assert.Equal(proofToken.SecurityKeys, gxst.SecurityKeys);

        //ProofToken is null
        GenericXmlSecurityToken gxst2 = new GenericXmlSecurityToken(tokenXml, null, DateTime.MinValue, DateTime.MaxValue, null, null, null);
        Assert.NotNull(gxst2.SecurityKeys);
        Assert.Equal(new ReadOnlyCollection<SecurityKey>(new List<SecurityKey>()), gxst2.SecurityKeys);

        //TokenXml is null
        Assert.Throws<System.ArgumentNullException>(() => new GenericXmlSecurityToken(null, null, DateTime.MinValue, DateTime.MaxValue, null, null, null));
    }    
}
