// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class CustomBindingTest
{
    [Fact]
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
            IRequestChannel channel = factory.CreateChannel(new EndpointAddress("http://localhost/WtfProjectNService.svc"));
            channel.Open();
        }
        catch (Exception ex)
        {
            Assert.True(false, String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }
    }

    [Theory]
    [InlineData("MyCustomBinding")]
    // Create a CustomBinding and set/get its name to validate it was created and usable.
    public static void CustomBinding_Name_Property(string bindingName)
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Name = bindingName;
        string actualBindingName = customBinding.Name;
        Assert.Equal<string>(bindingName, actualBindingName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public static void CustomBinding_Name_Property_Set_Throws(string bindingName)
    {
        CustomBinding customBinding = new CustomBinding();
        Assert.Throws<ArgumentException>(() =>
        {
            customBinding.Name = bindingName;
        });
    }
}
