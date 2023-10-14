// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class UnixDomainSocketSecurityTest
{
    [WcfFact]
    public static void Ctor_Default_Initializes_Properties()
    {
        UnixDomainSocketSecurity security = new UnixDomainSocketSecurity();
        UnixDomainSocketSecurityMode mode = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                UnixDomainSocketSecurityMode.Transport : UnixDomainSocketSecurityMode.TransportCredentialOnly;
        Assert.Equal<UnixDomainSocketSecurityMode>(mode, security.Mode);
    }

    [WcfTheory]
    [InlineData(UnixDomainSocketSecurityMode.None)]
    [InlineData(UnixDomainSocketSecurityMode.Transport)]
    public static void Mode_Property_Sets(UnixDomainSocketSecurityMode mode)
    {
        UnixDomainSocketSecurity security = new UnixDomainSocketSecurity();
        security.Mode = mode;
        Assert.Equal<UnixDomainSocketSecurityMode>(mode, security.Mode);
    }

    [WcfFact]
    public static void Mode_Property_Set_Invalid_Value_Throws()
    {
        UnixDomainSocketSecurity security = new UnixDomainSocketSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => security.Mode = (UnixDomainSocketSecurityMode)999);
    }

    [WcfFact]
    public static void Transport_Property_Sets()
    {
        UnixDomainSocketSecurity security = new UnixDomainSocketSecurity();

        UnixDomainSocketTransportSecurity newSecurity = new UnixDomainSocketTransportSecurity();
        security.Transport = newSecurity;
        Assert.Equal<UnixDomainSocketTransportSecurity>(newSecurity, security.Transport);
    }
}
