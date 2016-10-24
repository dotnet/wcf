// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;

public class MockTransportBindingElement : TransportBindingElement
{
    public MockTransportBindingElement()
    {
        SchemePropertyOverride = DefaultGetSchemeProperty;
        BuildChannelFactoryOverride = (Type t, BindingContext bc) => DefaultBuildChannelFactory(t, bc);
    }

    public Func<String> SchemePropertyOverride { get; set; }
    public Func<Type,BindingContext,IChannelFactory> BuildChannelFactoryOverride { get; set; }

    public override string Scheme
    {
        get
        {
            return SchemePropertyOverride();
        }
    }

    public string DefaultGetSchemeProperty()
    {
        return "myprotocol";
    }

    public override BindingElement Clone()
    {
        MockTransportBindingElement element = new MockTransportBindingElement();

        // Propagate the overrides
        element.SchemePropertyOverride = this.SchemePropertyOverride;
        element.BuildChannelFactoryOverride = this.BuildChannelFactoryOverride;
        return element;
    }

    public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
    {
        return true;
    }

    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
    {
        return (IChannelFactory<TChannel>)BuildChannelFactoryOverride(typeof(TChannel), context);
    }

    public IChannelFactory DefaultBuildChannelFactory(Type tChannel, BindingContext context)
    {
        // Default is a MockChannelFactory<IRequestChannel>.
        // If you need a different TChannel, supply a delegate to BuildChannelFactoryOverride
        // and construct the kind you need.
        return new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
    }
}