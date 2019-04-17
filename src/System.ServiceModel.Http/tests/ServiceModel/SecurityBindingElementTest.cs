// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class SecurityBindingElementTest
{
    [WcfTheory]
    [InlineData(BasicHttpSecurityMode.TransportCredentialOnly)]
    [InlineData(BasicHttpSecurityMode.Transport)]
    [InlineData(BasicHttpSecurityMode.None)]
    public static void Create_HttpBinding_SecurityMode_Without_SecurityBindingElement(BasicHttpSecurityMode securityMode)
    {
        BasicHttpBinding binding = new BasicHttpBinding(securityMode);
        var bindingElements = binding.CreateBindingElements();

        var securityBindingElement = bindingElements.FirstOrDefault(x => x is SecurityBindingElement) as SecurityBindingElement;
        Assert.True(securityBindingElement == null, string.Format("securityBindingElement should be null when BasicHttpSecurityMode is '{0}'", securityMode));

        Assert.True(binding.CanBuildChannelFactory<IRequestChannel>(), string.Format("CanBuildChannelFactory should return true for BasicHttpSecurityMode:'{0}'", securityMode));
        binding.BuildChannelFactory<IRequestChannel>();
    }

    [WcfTheory]
    [InlineData(BasicHttpSecurityMode.Message)]
    // BasicHttpSecurityMode.Message is not supported
    public static void Create_HttpBinding_SecurityMode_Message_Throws_NotSupported(BasicHttpSecurityMode securityMode)
    {
        Assert.Throws<PlatformNotSupportedException>(() =>
        {
            new BasicHttpBinding(securityMode);
        });
    }
}
