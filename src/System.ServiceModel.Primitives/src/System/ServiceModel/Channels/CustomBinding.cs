// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    public class CustomBinding : Binding
    {
        public CustomBinding()
            : base()
        {
        }

        public CustomBinding(params BindingElement[] bindingElementsInTopDownChannelStackOrder)
            : base()
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(bindingElementsInTopDownChannelStackOrder));
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                Elements.Add(element);
            }
        }

        public CustomBinding(string name, string ns, params BindingElement[] bindingElementsInTopDownChannelStackOrder)
            : base(name, ns)
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(bindingElementsInTopDownChannelStackOrder));
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                Elements.Add(element);
            }
        }

        public CustomBinding(IEnumerable<BindingElement> bindingElementsInTopDownChannelStackOrder)
        {
            if (bindingElementsInTopDownChannelStackOrder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(bindingElementsInTopDownChannelStackOrder));
            }

            foreach (BindingElement element in bindingElementsInTopDownChannelStackOrder)
            {
                Elements.Add(element);
            }
        }

        internal CustomBinding(BindingElementCollection bindingElements)
            : base()
        {
            if (bindingElements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(bindingElements));
            }

            for (int i = 0; i < bindingElements.Count; i++)
            {
                Elements.Add(bindingElements[i]);
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
            }
            return binding.CreateBindingElements();
        }

        internal CustomBinding(Binding binding, BindingElementCollection elements)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
            }
            if (elements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(elements));
            }

            Name = binding.Name;
            Namespace = binding.Namespace;
            CloseTimeout = binding.CloseTimeout;
            OpenTimeout = binding.OpenTimeout;
            ReceiveTimeout = binding.ReceiveTimeout;
            SendTimeout = binding.SendTimeout;

            for (int i = 0; i < elements.Count; i++)
            {
                Elements.Add(elements[i]);
            }
        }

        public BindingElementCollection Elements { get; } = new BindingElementCollection();

        public override BindingElementCollection CreateBindingElements()
        {
            return Elements.Clone();
        }

        public override string Scheme
        {
            get
            {
                TransportBindingElement transport = Elements.Find<TransportBindingElement>();
                if (transport == null)
                {
                    return String.Empty;
                }

                return transport.Scheme;
            }
        }
    }
}
