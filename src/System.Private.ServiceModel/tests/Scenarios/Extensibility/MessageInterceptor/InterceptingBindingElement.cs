// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;

public class InterceptingBindingElement : BindingElement
{
    ChannelMessageInterceptor interceptor;

    public InterceptingBindingElement(ChannelMessageInterceptor interceptor)
    {
        this.interceptor = interceptor;
    }

    protected InterceptingBindingElement(InterceptingBindingElement other)
        : base(other)
    {
        this.interceptor = other.Interceptor;
    }

    public ChannelMessageInterceptor Interceptor
    {
        get
        {
            if (this.interceptor != null)
            {
                return this.interceptor.Clone();
            }
            else
            {
                return new NullMessageInterceptor();
            }
        }
    }

    public override BindingElement Clone()
    {
        return new InterceptingBindingElement(this);
    }

    public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
    {
        return context.CanBuildInnerChannelFactory<TChannel>();
    }

    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
    {
        return new InterceptingChannelFactory<TChannel>(this.Interceptor, context);
    }

    public override T GetProperty<T>(BindingContext context)
    {
        if (typeof(T) == typeof(ChannelMessageInterceptor))
        {
            return (T)(object)this.Interceptor;
        }

        return context.GetInnerProperty<T>();
    }

    class NullMessageInterceptor : ChannelMessageInterceptor
    {
        public override ChannelMessageInterceptor Clone()
        {
            return new NullMessageInterceptor();
        }
    }
}

