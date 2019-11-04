// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public class SecurityTokenSerializerTest
{
    [WcfFact]
    public static void Methods_NonNullParam_InvokeAndReturn()
    {
        var xmlReader = XmlReader.Create(new MemoryStream());
        var xmlWriter = XmlWriter.Create(new MemoryStream());
        var dummyToken = new DummySecurityToken();
        var keyIdentifier = new SecurityKeyIdentifier();
        var keyIdentifierClause = new SecurityKeyIdentifierClauseImpl("DummyClause");
        var sts = new SecurityTokenSerializerImpl();

        Assert.NotNull(sts);
        Assert.True(sts.CanReadKeyIdentifier(xmlReader));
        Assert.True(sts.CanReadKeyIdentifierClause(xmlReader));
        Assert.True(sts.CanReadToken(xmlReader));
        Assert.True(sts.CanWriteKeyIdentifier(keyIdentifier));
        Assert.True(sts.CanWriteKeyIdentifierClause(keyIdentifierClause));
        Assert.True(sts.CanWriteToken(dummyToken));

        SecurityToken token = sts.ReadToken(xmlReader, SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new ReadOnlyCollection<SecurityToken>(new List<SecurityToken>() { dummyToken}), false));
        SecurityKeyIdentifier identifier = sts.ReadKeyIdentifier(xmlReader);
        SecurityKeyIdentifierClause identifierClause = sts.ReadKeyIdentifierClause(xmlReader);
        Assert.IsType<DummySecurityToken>(token);
        Assert.IsType<SecurityKeyIdentifier>(identifier);
        Assert.IsType<SecurityKeyIdentifierClauseImpl>(identifierClause);

        sts.WriteToken(xmlWriter, dummyToken);
        sts.WriteKeyIdentifier(xmlWriter, keyIdentifier);
        sts.WriteKeyIdentifierClause(xmlWriter, keyIdentifierClause);
        Assert.True(sts.WriteTokenCoreCalled);
        Assert.True(sts.WriteKeyIdentifierCoreCalled);
        Assert.True(sts.WriteKeyIdentifierClauseCoreCalled);
    }

    [WcfFact]
    public static void Methods_NullParam_Throws()
    {
        var sts = new SecurityTokenSerializerImpl();

        Assert.NotNull(sts);
        Assert.Throws<ArgumentNullException>(() => sts.CanReadKeyIdentifier(null));
        Assert.Throws<ArgumentNullException>(() => sts.CanReadKeyIdentifierClause(null));
        Assert.Throws<ArgumentNullException>(() => sts.CanReadToken(null));
        Assert.Throws<ArgumentNullException>(() => sts.CanWriteKeyIdentifier(null));
        Assert.Throws<ArgumentNullException>(() => sts.CanWriteKeyIdentifierClause(null));
        Assert.Throws<ArgumentNullException>(() => sts.CanWriteToken(null));
        Assert.Throws<ArgumentNullException>(() => sts.ReadToken(null, null));
        Assert.Throws<ArgumentNullException>(() => sts.ReadKeyIdentifier(null));
        Assert.Throws<ArgumentNullException>(() => sts.ReadKeyIdentifierClause(null));
        Assert.Throws<ArgumentNullException>(() => sts.WriteToken(null, null));
        Assert.Throws<ArgumentNullException>(() => sts.WriteKeyIdentifier(null, null));
        Assert.Throws<ArgumentNullException>(() => sts.WriteKeyIdentifierClause(null, null));
        Assert.False(sts.WriteTokenCoreCalled);
        Assert.False(sts.WriteKeyIdentifierCoreCalled);
        Assert.False(sts.WriteKeyIdentifierClauseCoreCalled);
    }
}

public class SecurityTokenSerializerImpl : SecurityTokenSerializer
{
    public bool WriteTokenCoreCalled = false;
    public bool WriteKeyIdentifierCoreCalled = false;
    public bool WriteKeyIdentifierClauseCoreCalled = false;

    protected override bool CanReadKeyIdentifierClauseCore(XmlReader reader)
    {
        return true;
    }

    protected override bool CanReadKeyIdentifierCore(XmlReader reader)
    {
        return true;
    }

    protected override bool CanReadTokenCore(XmlReader reader)
    {
        return true;
    }

    protected override bool CanWriteKeyIdentifierClauseCore(SecurityKeyIdentifierClause keyIdentifierClause)
    {
        return true;
    }

    protected override bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier)
    {
        return true;
    }

    protected override bool CanWriteTokenCore(SecurityToken token)
    {
        return true;
    }

    protected override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader)
    {
        return new SecurityKeyIdentifierClauseImpl("DummyClause");
    }

    protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader)
    {
        return new SecurityKeyIdentifier();
    }

    protected override SecurityToken ReadTokenCore(XmlReader reader, SecurityTokenResolver tokenResolver)
    {
        return new DummySecurityToken();
    }

    protected override void WriteKeyIdentifierClauseCore(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
    {
        WriteKeyIdentifierClauseCoreCalled = true;
    }

    protected override void WriteKeyIdentifierCore(XmlWriter writer, SecurityKeyIdentifier keyIdentifier)
    {
        WriteKeyIdentifierCoreCalled = true;
    }

    protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
    {
        WriteTokenCoreCalled = true;
    }
}

public class SecurityKeyIdentifierClauseImpl : SecurityKeyIdentifierClause
{
    public SecurityKeyIdentifierClauseImpl(string clauseType) : base(clauseType)
    {
    }

    public SecurityKeyIdentifierClauseImpl(string clauseType, byte[] nonce, int length) : base(clauseType, nonce, length)
    {
    }
}
