// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class CustomBindingTest
{
    [WcfFact]
    // Create the channel factory and open the channel for the request-reply message exchange pattern.
    public static void RequestReplyChannelFactory_Open()
    {
        try
        {
            BindingElement[] bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElements);

            // Create the channel factory
            IChannelFactory<IRequestChannel> factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel and open it.  Success is anything other than an exception.
            IRequestChannel channel = factory.CreateChannel(new EndpointAddress("http://localhost/WcfProjectNService.svc"));
            channel.Open();
        }
        catch (Exception ex)
        {
            Assert.Fail(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }
    }

    [WcfTheory]
    [InlineData("MyCustomBinding")]
    // Create a CustomBinding and set/get its name to validate it was created and usable.
    public static void CustomBinding_Name_Property(string bindingName)
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Name = bindingName;
        string actualBindingName = customBinding.Name;
        Assert.Equal(bindingName, actualBindingName);
    }

    [WcfTheory]
    [InlineData("")]
    [InlineData(new object[] { null } )]
    public static void CustomBinding_Name_Property_Set_Throws(string bindingName)
    {
        CustomBinding customBinding = new CustomBinding();
        Assert.Throws<ArgumentException>(() =>
        {
            customBinding.Name = bindingName;
        });
    }
}
