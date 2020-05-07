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
        private CustomBinding _binding;
        private BindingParameterCollection _bindingParameters;
        private Uri _listenUriBaseAddress;
        private ListenUriMode _listenUriMode;
        private string _listenUriRelativeAddress;
        private BindingElementCollection _remainingBindingElements;  // kept to ensure each BE builds itself once

        public BindingContext(CustomBinding binding, BindingParameterCollection parameters)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
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
            _binding = binding;

            _remainingBindingElements = new BindingElementCollection(remainingBindingElements);
            _bindingParameters = new BindingParameterCollection(parameters);
        }

        public CustomBinding Binding
        {
            get { return _binding; }
        }

        public BindingParameterCollection BindingParameters
        {
            get { return _bindingParameters; }
        }

        public Uri ListenUriBaseAddress
        {
            get { return _listenUriBaseAddress; }
            set { _listenUriBaseAddress = value; }
        }

        public ListenUriMode ListenUriMode
        {
            get { return _listenUriMode; }
            set { _listenUriMode = value; }
        }

        public string ListenUriRelativeAddress
        {
            get { return _listenUriRelativeAddress; }
            set { _listenUriRelativeAddress = value; }
        }

        public BindingElementCollection RemainingBindingElements
        {
            get { return _remainingBindingElements; }
        }

        public IChannelFactory<TChannel> BuildInnerChannelFactory<TChannel>()
        {
            return this.RemoveNextElement().BuildChannelFactory<TChannel>(this);
        }

        public bool CanBuildInnerChannelFactory<TChannel>()
        {
            BindingContext clone = this.Clone();
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
                BindingContext clone = this.Clone();
                return clone.RemoveNextElement().GetProperty<T>(clone);
            }
        }

        public BindingContext Clone()
        {
            return new BindingContext(_binding, _remainingBindingElements, _bindingParameters);
        }

        private BindingElement RemoveNextElement()
        {
            BindingElement element = _remainingBindingElements.Remove<BindingElement>();
            if (element != null)
                return element;
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                SRServiceModel.NoChannelBuilderAvailable, _binding.Name, _binding.Namespace)));
        }

        internal void ValidateBindingElementsConsumed()
        {
            if (this.RemainingBindingElements.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (BindingElement bindingElement in this.RemainingBindingElements)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                        builder.Append(" ");
                    }
                    string typeString = bindingElement.GetType().ToString();
                    builder.Append(typeString.Substring(typeString.LastIndexOf('.') + 1));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.NotAllBindingElementsBuilt, builder.ToString())));
            }
        }
    }
}
