// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 


using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class RequireDerivedKeysTest
{
    [WcfFact]
    public static void RequireDerivedKeys_IsGettable()
    {
        MockSecurityTokenParameters mstp = new MockSecurityTokenParameters();
        Assert.True(mstp.RequireDerivedKeys);
    }

    [WcfFact]
    public static void RequireDerivedKeys_IsSettable()
    {
        MockSecurityTokenParameters mstp = new MockSecurityTokenParameters();
        mstp.RequireDerivedKeys = false;
        Assert.False(mstp.RequireDerivedKeys);
    }
}

public class MockSecurityTokenParameters : SecurityTokenParameters
{
    protected override bool HasAsymmetricKey => throw new System.NotImplementedException();

    protected override bool SupportsClientAuthentication => throw new System.NotImplementedException();

    protected override bool SupportsServerAuthentication => throw new System.NotImplementedException();

    protected override bool SupportsClientWindowsIdentity => throw new System.NotImplementedException();

    protected override SecurityTokenParameters CloneCore()
    {
        throw new System.NotImplementedException();
    }

    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
    {
        throw new System.NotImplementedException();
    }
}

