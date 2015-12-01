// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using Xunit;

public static class WebSocketTransportSettingsTest
{
    [Fact]
    public static void DisablePayloadMasking_Property_Set_PNSE_Throws()
    {
        var setting = new WebSocketTransportSettings();
        Assert.Throws<PlatformNotSupportedException>(() => setting.DisablePayloadMasking = true);
    }

    [Fact]
    public static void DisablePayloadMasking_Property_Get_PNSE_Throws()
    {
        var setting = new WebSocketTransportSettings();
        bool disablePayloadMasking = false;
        Assert.Throws<PlatformNotSupportedException>(() => disablePayloadMasking = setting.DisablePayloadMasking);
        Assert.False(disablePayloadMasking);
    }
}
