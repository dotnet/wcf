// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 


using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using Infrastructure.Common;
using Xunit;

public static class SymmetricSecurityKeyTest
{
    [WcfFact]
    public static void method_GetSymmetricKey()
    {
        byte[] gsk = new byte[] { 0x02, 0x01, 0x04, 0x03 };
        MockGetSymmetricAlgorithm mgsakey = new MockGetSymmetricAlgorithm();
        Assert.NotNull(mgsakey.GetSymmetricKey());
        Assert.Equal(gsk, mgsakey.GetSymmetricKey());
    }

    [WcfFact]
    public static void method_GetSymmetricAlgorithm()
    {
        string algorit = "DES";
        MockGetSymmetricAlgorithm mgsaalg = new MockGetSymmetricAlgorithm();
        Assert.NotNull(mgsaalg.GetSymmetricAlgorithm(algorit));
        Assert.IsAssignableFrom<DES>(mgsaalg.GetSymmetricAlgorithm(algorit));
    }
}

public class MockGetSymmetricAlgorithm : SymmetricSecurityKey
{
    public override int KeySize => throw new System.NotImplementedException();

    public override byte[] DecryptKey(string algorithm, byte[] keyData)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] EncryptKey(string algorithm, byte[] keyData)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] GenerateDerivedKey(string algorithm, byte[] label, byte[] nonce, int derivedKeyLength, int offset)
    {
        throw new System.NotImplementedException();
    }

    public override ICryptoTransform GetDecryptionTransform(string algorithm, byte[] iv)
    {
        throw new System.NotImplementedException();
    }

    public override ICryptoTransform GetEncryptionTransform(string algorithm, byte[] iv)
    {
        throw new System.NotImplementedException();
    }

    public override int GetIVSize(string algorithm)
    {
        throw new System.NotImplementedException();
    }

    public override KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm)
    {
        throw new System.NotImplementedException();
    }

    public override SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm)
    {
        if (algorithm == "DES")
        {
            return DES.Create();
        }
        else
        {
            return null;
        }
    }

    public override byte[] GetSymmetricKey()
    {
        byte[] gsk = new byte[] { 0x02, 0x01, 0x04, 0x03 };
        return gsk;
    }

    public override bool IsAsymmetricAlgorithm(string algorithm)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsSupportedAlgorithm(string algorithm)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsSymmetricAlgorithm(string algorithm)
    {
        throw new System.NotImplementedException();
    }
}
