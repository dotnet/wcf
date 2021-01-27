// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Channels;
    using System.Globalization;
    using Microsoft.Xml;

    public abstract class BindingElementExtensionElement //TODO: ServiceModelExtensionElement
    {
        public virtual void ApplyConfiguration(BindingElement bindingElement)
        {
            // Some items make sense just as tags and have no other configuration
            if (null == bindingElement)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElement");
            }
        }

        public abstract Type BindingElementType
        {
            get;
        }

        protected internal abstract BindingElement CreateBindingElement();
    }
}
