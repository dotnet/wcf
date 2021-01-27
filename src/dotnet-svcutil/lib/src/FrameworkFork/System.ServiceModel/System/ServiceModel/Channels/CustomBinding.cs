// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    public class CustomBinding : Binding
    {
        private BindingElementCollection _bindingElements = new BindingElementCollection();

        public CustomBinding()
            : base()
        {
        }

        public CustomBinding(params BindingElement[] bindingElementsInTopDownChannelStackOrder)
            : base()
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                _bindingElements.Add(element);
            }
        }

        public CustomBinding(string name, string ns, params BindingElement[] bindingElementsInTopDownChannelStackOrder)
            : base(name, ns)
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                _bindingElements.Add(element);
            }
        }

        public CustomBinding(IEnumerable<BindingElement> bindingElementsInTopDownChannelStackOrder)
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                _bindingElements.Add(element);
            }
        }

        internal CustomBinding(BindingElementCollection bindingElements)
            : base()
        {
            if (bindingElements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
            }

            for (int i = 0; i < bindingElements.Count; i++)
            {
                _bindingElements.Add(bindingElements[i]);
            }
        }

        public CustomBinding(Binding binding)
            : this(binding, SafeCreateBindingElements(binding))
        {
        }

        private static BindingElementCollection SafeCreateBindingElements(Binding binding)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            }
            return binding.CreateBindingElements();
        }

        internal CustomBinding(Binding binding, BindingElementCollection elements)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            }
            if (elements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("elements");
            }

            this.Name = binding.Name;
            this.Namespace = binding.Namespace;
            this.CloseTimeout = binding.CloseTimeout;
            this.OpenTimeout = binding.OpenTimeout;
            this.ReceiveTimeout = binding.ReceiveTimeout;
            this.SendTimeout = binding.SendTimeout;

            for (int i = 0; i < elements.Count; i++)
            {
                _bindingElements.Add(elements[i]);
            }
        }

        public BindingElementCollection Elements
        {
            get
            {
                return _bindingElements;
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            return _bindingElements.Clone();
        }

        public override string Scheme
        {
            get
            {
                TransportBindingElement transport = _bindingElements.Find<TransportBindingElement>();
                if (transport == null)
                {
                    return String.Empty;
                }

                return transport.Scheme;
            }
        }
    }
}
