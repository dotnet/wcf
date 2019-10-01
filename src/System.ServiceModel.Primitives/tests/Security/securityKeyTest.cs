// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 


using System;
using System.IdentityModel.Tokens;
using Infrastructure.Common;
using Xunit;

public static class SecurityKeyTest
{
    [WcfFact]
    public static void Property_KeySize_Get()
    {
        MockSecurityKey msk = new MockSecurityKey();
        Assert.Equal(int.MaxValue, msk.KeySize);
    }

    [WcfFact]
    public static void Method_EncryptKey_Return_Byte()
    {
        string algorithm = "cryptographic";
        byte[] keyData = new byte[] { 0x02, 0x01, 0x04, 0x03 };
        MockSecurityKey msk = new MockSecurityKey();
        Assert.Equal(keyData, msk.EncryptKey(algorithm, keyData));
    }
}

public class MockSecurityKey : SecurityKey
{
    public override int KeySize
    {
        get
        {
            return int.MaxValue;
        }
    }

    public override byte[] EncryptKey(string algorithm, byte[] keyData)
    {
        byte[] KD = new byte[] { 0x02, 0x01, 0x04, 0x03 };
        return KD;
    }

    public override byte[] DecryptKey(string algorithm, byte[] keyData)
    {
        throw new NotImplementedException();
    }

    public override bool IsAsymmetricAlgorithm(string algorithm)
    {
        throw new NotImplementedException();
    }

    public override bool IsSupportedAlgorithm(string algorithm)
    {
        throw new NotImplementedException();
    }

    public override bool IsSymmetricAlgorithm(string algorithm)
    {
        throw new NotImplementedException();
    }
}

