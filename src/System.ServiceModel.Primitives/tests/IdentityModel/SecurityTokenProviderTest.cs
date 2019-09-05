// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class SecurityTokenProviderTest
{
    [WcfFact]
    public static void SynchronousImplementationOnly()
    {
        var provider = new SecurityTokenProviderSyncImpl();

        var token = provider.GetToken(TimeSpan.Zero);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        token = provider.RenewToken(TimeSpan.Zero, token);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        provider.CancelToken(TimeSpan.Zero, token);
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);

        var iar = provider.BeginGetToken(TimeSpan.Zero, null, null);
        token = provider.EndGetToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        iar = provider.BeginRenewToken(TimeSpan.Zero, token, null, null);
        token = provider.EndRenewToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        iar = provider.BeginCancelToken(TimeSpan.Zero, token, null, null);
        provider.EndCancelToken(iar);
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);

        token = provider.GetTokenAsync(TimeSpan.Zero).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        token = provider.RenewTokenAsync(TimeSpan.Zero, token).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        provider.CancelTokenAsync(TimeSpan.Zero, token).Wait();
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);
    }

    [WcfFact]
    public static void SyncAndApmImplementation()
    {
        var provider = new SecurityTokenProviderApmImpl();

        var token = provider.GetToken(TimeSpan.Zero);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        token = provider.RenewToken(TimeSpan.Zero, token);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        provider.CancelToken(TimeSpan.Zero, token);
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);

        var iar = provider.BeginGetToken(TimeSpan.Zero, null, null);
        token = provider.EndGetToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("BeginGetTokenCore", token.Id);
        iar = provider.BeginRenewToken(TimeSpan.Zero, token, null, null);
        token = provider.EndRenewToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("BeginRenewTokenCore", token.Id);
        iar = provider.BeginCancelToken(TimeSpan.Zero, token, null, null);
        provider.EndCancelToken(iar);
        Assert.Equal("BeginCancelTokenCore", provider.CancelTokenMethodCalled);

        token = provider.GetTokenAsync(TimeSpan.Zero).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("BeginGetTokenCore", token.Id);
        token = provider.RenewTokenAsync(TimeSpan.Zero, token).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("BeginRenewTokenCore", token.Id);
        provider.CancelTokenAsync(TimeSpan.Zero, token).Wait();
        Assert.Equal("BeginCancelTokenCore", provider.CancelTokenMethodCalled);
    }

    [WcfFact]
    public static void SyncAndAsyncImplementation()
    {
        // In .NET Framework, if you don't override the APM methods, the base class calls the sync implementation.
        // Verifying that this is still the case when providing an Async implementation.
        var provider = new SecurityTokenProviderAsyncImpl();

        var token = provider.GetToken(TimeSpan.Zero);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        token = provider.RenewToken(TimeSpan.Zero, token);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        provider.CancelToken(TimeSpan.Zero, token);
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);

        var iar = provider.BeginGetToken(TimeSpan.Zero, null, null);
        token = provider.EndGetToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCore", token.Id);
        iar = provider.BeginRenewToken(TimeSpan.Zero, token, null, null);
        token = provider.EndRenewToken(iar);
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCore", token.Id);
        iar = provider.BeginCancelToken(TimeSpan.Zero, token, null, null);
        provider.EndCancelToken(iar);
        Assert.Equal("CancelTokenCore", provider.CancelTokenMethodCalled);

        token = provider.GetTokenAsync(TimeSpan.Zero).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("GetTokenCoreAsync", token.Id);
        token = provider.RenewTokenAsync(TimeSpan.Zero, token).Result;
        Assert.IsType<DummySecurityToken>(token);
        Assert.Equal("RenewTokenCoreAsync", token.Id);
        provider.CancelTokenAsync(TimeSpan.Zero, token).Wait();
        Assert.Equal("CancelTokenCoreAsync", provider.CancelTokenMethodCalled);
    }
}

public class SecurityTokenProviderSyncImpl : SecurityTokenProvider
{
    public string CancelTokenMethodCalled { get; set; } = string.Empty;

    protected override SecurityToken GetTokenCore(TimeSpan timeout)
    {
        return new DummySecurityToken();
    }

    protected override SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
    {
        Assert.IsType<DummySecurityToken>(tokenToBeRenewed);
        return new DummySecurityToken();
    }

    protected override void CancelTokenCore(TimeSpan timeout, SecurityToken token)
    {
        Assert.IsType<DummySecurityToken>(token);
        CancelTokenMethodCalled = nameof(CancelTokenCore);
    }
}

public class SecurityTokenProviderApmImpl : SecurityTokenProvider
{
    public string CancelTokenMethodCalled { get; set; } = string.Empty;

    protected override SecurityToken GetTokenCore(TimeSpan timeout)
    {
        return new DummySecurityToken();
    }

    protected override IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
    {
        var result = Task.FromResult<SecurityToken>(new DummySecurityToken());
        callback?.Invoke(result);
        return result;
    }

    protected override SecurityToken EndGetTokenCore(IAsyncResult result)
    {
        return ((Task<SecurityToken>)result).Result;
    }

    protected override SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
    {
        Assert.IsType<DummySecurityToken>(tokenToBeRenewed);
        return new DummySecurityToken();
    }

    protected override IAsyncResult BeginRenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
    {
        Assert.IsType<DummySecurityToken>(tokenToBeRenewed);
        var result = Task.FromResult<SecurityToken>(new DummySecurityToken());
        callback?.Invoke(result);
        return result;
    }

    protected override SecurityToken EndRenewTokenCore(IAsyncResult result)
    {
        return ((Task<SecurityToken>)result).Result;
    }

    protected override void CancelTokenCore(TimeSpan timeout, SecurityToken token)
    {
        Assert.IsType<DummySecurityToken>(token);
        CancelTokenMethodCalled = nameof(CancelTokenCore);
    }

    protected override IAsyncResult BeginCancelTokenCore(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state)
    {
        Assert.IsType<DummySecurityToken>(token);
        CancelTokenMethodCalled = nameof(BeginCancelTokenCore);
        var result = Task.CompletedTask;
        callback?.Invoke(result);
        return result;
    }

    protected override void EndCancelTokenCore(IAsyncResult result)
    {
        ((Task)result).GetAwaiter().GetResult();
    }
}

public class SecurityTokenProviderAsyncImpl : SecurityTokenProvider
{
    public string CancelTokenMethodCalled { get; set; } = string.Empty;

    protected override SecurityToken GetTokenCore(TimeSpan timeout)
    {
        return new DummySecurityToken();
    }

    protected override Task<SecurityToken> GetTokenCoreAsync(TimeSpan timeout)
    {
        return Task.FromResult<SecurityToken>(new DummySecurityToken());
    }

    protected override SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
    {
        Assert.IsType<DummySecurityToken>(tokenToBeRenewed);
        return new DummySecurityToken();
    }

    protected override Task<SecurityToken> RenewTokenCoreAsync(TimeSpan timeout, SecurityToken tokenToBeRenewed)
    {
        Assert.IsType<DummySecurityToken>(tokenToBeRenewed);
        return Task.FromResult<SecurityToken>(new DummySecurityToken());
    }

    protected override void CancelTokenCore(TimeSpan timeout, SecurityToken token)
    {
        Assert.IsType<DummySecurityToken>(token);
        CancelTokenMethodCalled = nameof(CancelTokenCore);
    }

    protected override Task CancelTokenCoreAsync(TimeSpan timeout, SecurityToken token)
    {
        Assert.IsType<DummySecurityToken>(token);
        CancelTokenMethodCalled = nameof(CancelTokenCoreAsync);
        return Task.CompletedTask;
    }
}

public class DummySecurityToken : SecurityToken
{
    public DummySecurityToken([CallerMemberName] string callingMethod = "")
    {
        Id = callingMethod;
    }

    public override string Id { get; }
    public override ReadOnlyCollection<SecurityKey> SecurityKeys => default;
    public override DateTime ValidFrom => default;
    public override DateTime ValidTo => default;
}

