// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.MessageInterceptor
{
    public class InterceptingBindingElement
                 : BindingElement
                 , IPolicyExportExtension
    {
        private ChannelMessageInterceptor _interceptor;

        public InterceptingBindingElement(ChannelMessageInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        protected InterceptingBindingElement(InterceptingBindingElement other)
            : base(other)
        {
            _interceptor = other.Interceptor;
        }

        public ChannelMessageInterceptor Interceptor
        {
            get
            {
                if (_interceptor != null)
                {
                    return _interceptor.Clone();
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

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return new InterceptingChannelFactory<TChannel>(this.Interceptor, context);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            return new InterceptingChannelListener<TChannel>(this.Interceptor, context);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ChannelMessageInterceptor))
            {
                return (T)(object)this.Interceptor;
            }

            return context.GetInnerProperty<T>();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _interceptor.OnExportPolicy(exporter, context);
        }


        private class NullMessageInterceptor : ChannelMessageInterceptor
        {
            public override ChannelMessageInterceptor Clone()
            {
                return new NullMessageInterceptor();
            }
        }
    }
}

