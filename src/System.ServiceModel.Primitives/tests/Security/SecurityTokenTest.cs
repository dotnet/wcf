// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using Infrastructure.Common;
using Xunit;

public static class SecurityTokenTest
{
    [WcfFact]
    public static void Method_CreateKeyIdentifierClause()
    {
        MockSecurityToken mst = new MockSecurityToken();
        LocalIdKeyIdentifierClause idClause = mst.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
        Assert.NotNull(idClause);
        Assert.Equal(mst.Id, idClause.LocalId);

        Assert.Throws<NotSupportedException>(() => mst.CreateKeyIdentifierClause<GenericXmlSecurityKeyIdentifierClause>());
    }
}

public class MockSecurityToken : SecurityToken
{
    public override string Id
    {
        get
        {
            return "Id";
        }
    }

    public override ReadOnlyCollection<SecurityKey> SecurityKeys
    {
        get
        {
            return null;
        }
    }

    public override DateTime ValidFrom
    {
        get
        {
            return DateTime.UtcNow;
        }
    }

    public override DateTime ValidTo
    {
        get
        {
            return DateTime.MaxValue;
        }
    }
}
