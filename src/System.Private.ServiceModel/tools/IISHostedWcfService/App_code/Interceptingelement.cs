// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.MessageInterceptor
{
    /// <summary>
    /// Configuration class for InterceptingBindingElement. To make your InterceptingBindingElement
    /// configurable, derive from InterceptingElementInterceptingElement and override CreateMessageInterceptor()
    /// </summary>
    public abstract class InterceptingElement : BindingElementExtensionElement
    {
        protected InterceptingElement()
            : base()
        {
        }

        public override Type BindingElementType
        {
            get
            {
                return typeof(InterceptingBindingElement);
            }
        }

        protected abstract ChannelMessageInterceptor CreateMessageInterceptor();

        protected override BindingElement CreateBindingElement()
        {
            return new InterceptingBindingElement(CreateMessageInterceptor());
        }
    }
}
