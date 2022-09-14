// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;

namespace System.ServiceModel.Channels
{
    public abstract class BindingElement
    {
        protected BindingElement()
        {
        }

        protected BindingElement(BindingElement elementToBeCloned)
        {
        }

        public abstract BindingElement Clone();

        public virtual IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            return context.BuildInnerChannelFactory<TChannel>();
        }

        public virtual bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public abstract T GetProperty<T>(BindingContext context) where T : class;

        internal T GetIndividualProperty<T>() where T : class
        {
            return GetProperty<T>(new BindingContext(new CustomBinding(), new BindingParameterCollection()));
        }

        internal virtual bool IsMatch(BindingElement b)
        {
            Fx.Assert(true, "Should not be called unless this binding element is used in one of the standard bindings. In which case, please re-implement the IsMatch() method.");
            return false;
        }
    }
}
