// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class WebSocketTransportSettingsTest
{
    [WcfFact]
    public static void DisablePayloadMasking_Property_Set_PNSE_Throws()
    {
        var setting = new WebSocketTransportSettings();
        Assert.Throws<PlatformNotSupportedException>(() => setting.DisablePayloadMasking = true);
    }

    [WcfFact]
    public static void DisablePayloadMasking_Property_Get_PNSE_Throws()
    {
        var setting = new WebSocketTransportSettings();
        bool disablePayloadMasking = false;
        Assert.Throws<PlatformNotSupportedException>(() => disablePayloadMasking = setting.DisablePayloadMasking);
        Assert.False(disablePayloadMasking);
    }
}
