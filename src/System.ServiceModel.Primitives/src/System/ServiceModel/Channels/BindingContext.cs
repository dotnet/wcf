// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.ServiceModel.Description;
using System.Text;

namespace System.ServiceModel.Channels
{
    public class BindingContext
    {
        private BindingElementCollection _remainingBindingElements;  // kept to ensure each BE builds itself once

        public BindingContext(CustomBinding binding, BindingParameterCollection parameters)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
            }

            Initialize(binding, binding.Elements, parameters);
        }

        private BindingContext(CustomBinding binding,
                       BindingElementCollection remainingBindingElements,
                       BindingParameterCollection parameters)
        {
            Initialize(binding, remainingBindingElements, parameters);
        }

        private void Initialize(CustomBinding binding,
                        BindingElementCollection remainingBindingElements,
                        BindingParameterCollection parameters)
        {
            Binding = binding;

            _remainingBindingElements = new BindingElementCollection(remainingBindingElements);
            BindingParameters = new BindingParameterCollection(parameters);
        }

        public CustomBinding Binding { get; private set; }

        public BindingParameterCollection BindingParameters { get; private set; }

        public Uri ListenUriBaseAddress { get; set; }

        public ListenUriMode ListenUriMode { get; set; }

        public string ListenUriRelativeAddress { get; set; }

        public BindingElementCollection RemainingBindingElements
        {
            get { return _remainingBindingElements; }
        }

        public IChannelFactory<TChannel> BuildInnerChannelFactory<TChannel>()
        {
            return RemoveNextElement().BuildChannelFactory<TChannel>(this);
        }

        public bool CanBuildInnerChannelFactory<TChannel>()
        {
            BindingContext clone = Clone();
            return clone.RemoveNextElement().CanBuildChannelFactory<TChannel>(clone);
        }

        public T GetInnerProperty<T>()
            where T : class
        {
            if (_remainingBindingElements.Count == 0)
            {
                return null;
            }
            else
            {
                BindingContext clone = Clone();
                return clone.RemoveNextElement().GetProperty<T>(clone);
            }
        }

        public BindingContext Clone()
        {
            return new BindingContext(Binding, _remainingBindingElements, BindingParameters);
        }

        private BindingElement RemoveNextElement()
        {
            BindingElement element = _remainingBindingElements.Remove<BindingElement>();
            if (element != null)
            {
                return element;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(
                SRP.NoChannelBuilderAvailable, Binding.Name, Binding.Namespace)));
        }

        internal void ValidateBindingElementsConsumed()
        {
            if (RemainingBindingElements.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (BindingElement bindingElement in RemainingBindingElements)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                        builder.Append(" ");
                    }
                    string typeString = bindingElement.GetType().ToString();
                    builder.Append(typeString.Substring(typeString.LastIndexOf('.') + 1));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.NotAllBindingElementsBuilt, builder.ToString())));
            }
        }
    }
}
