// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using Infrastructure.Common;
using Xunit;

public class SecurityTokenManagerTest
{
    [WcfFact]
    public static void Methods_Override()
    {
        var tokenManager = new SecurityTokenManagerImpl();
        SecurityTokenVersionImpl tokenVersion = new SecurityTokenVersionImpl();
        SecurityTokenRequirement tokenRequirement = new SecurityTokenRequirement();

        SecurityTokenAuthenticator authenticator = tokenManager.CreateSecurityTokenAuthenticator(tokenRequirement, out SecurityTokenResolver resolver);
        SecurityTokenProvider provider = tokenManager.CreateSecurityTokenProvider(tokenRequirement);
        SecurityTokenSerializer serializer = tokenManager.CreateSecurityTokenSerializer(tokenVersion);

        Assert.IsType<SecurityTokenAuthenticatorImpl>(authenticator);
        Assert.IsType<SecurityTokenResolverImpl>(resolver);
        Assert.IsType<SecurityTokenProviderSyncImpl>(provider);
        Assert.IsType<SecurityTokenSerializerImpl>(serializer);
    }
}

public class SecurityTokenManagerImpl : SecurityTokenManager
{
    public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
    {
        Assert.IsType<SecurityTokenRequirement>(tokenRequirement);
        outOfBandTokenResolver = new SecurityTokenResolverImpl();
        return new SecurityTokenAuthenticatorImpl();
    }

    public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
    {
        Assert.IsType<SecurityTokenRequirement>(tokenRequirement);
        return new SecurityTokenProviderSyncImpl();
    }

    public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion tokenVersion)
    {
        Assert.IsType<SecurityTokenVersionImpl>(tokenVersion);
        return new SecurityTokenSerializerImpl();
    }
}

public class SecurityTokenVersionImpl : SecurityTokenVersion
{
    public override ReadOnlyCollection<string> GetSecuritySpecifications()
    {
        return null;
    }
}

public class SecurityTokenAuthenticatorImpl : SecurityTokenAuthenticator
{
    protected override bool CanValidateTokenCore(SecurityToken token)
    {
        return false;
    }

    protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token)
    {
        return null;
    }
}

public class SecurityTokenResolverImpl : SecurityTokenResolver
{
    protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
    {
        key = null;
        return false;
    }

    protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
    {
        token = null;
        return false;
    }

    protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
    {
        token = null;
        return false;
    }
}
