// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class BinarySecretSecurityTokenTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        string id = "test id";
        byte[] keyBytes = new byte[128];
        Random rnd = new Random();
        rnd.NextBytes(keyBytes);

        BinarySecretSecurityToken bsst = new BinarySecretSecurityToken(keyBytes);
        Assert.NotNull(bsst);
        Assert.NotNull(bsst.Id);
        Assert.NotNull(bsst.SecurityKeys);
        Assert.Single(bsst.SecurityKeys);
        Assert.Equal(DateTime.UtcNow.Date, bsst.ValidFrom.Date);
        Assert.Equal(DateTime.MaxValue, bsst.ValidTo);
        Assert.Equal(keyBytes.Length * 8, bsst.KeySize);
        Assert.Equal(keyBytes, bsst.GetKeyBytes());     

        BinarySecretSecurityToken bsst2 = new BinarySecretSecurityToken(id, keyBytes);
        Assert.NotNull(bsst2);
        Assert.Equal(id, bsst2.Id);
        Assert.NotNull(bsst2.SecurityKeys);
        Assert.Single(bsst2.SecurityKeys);
        Assert.Equal(DateTime.UtcNow.Date, bsst2.ValidFrom.Date);
        Assert.Equal(DateTime.MaxValue, bsst2.ValidTo);
        Assert.Equal(keyBytes.Length * 8, bsst2.KeySize);
        Assert.Equal(keyBytes, bsst2.GetKeyBytes());
    }

    [WcfFact]
    public static void Ctor_Default_IdIsUnique()
    {
        byte[] keyBytes = new byte[128];
        Random rnd = new Random();
        rnd.NextBytes(keyBytes);

        BinarySecretSecurityToken bsst = new BinarySecretSecurityToken(keyBytes);
        BinarySecretSecurityToken bsst2 = new BinarySecretSecurityToken(keyBytes);
        Assert.NotEqual(bsst.Id, bsst2.Id);
    }
}
